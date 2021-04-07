using System;
using System.Collections.Concurrent;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Microsoft.MixedReality.WebRTC.Unity
{
    /// <summary>
    /// This class encapsulates a dataChannel and allows the sending and recieving of objects of the T class.
    /// To implement this class you have to implement the Send and Recieve method.
    /// </summary>
    /// <typeparam name="T">a class that is serializable and will be send through the dataChannel</typeparam>
    public abstract class LocalDataSource<T> : MonoBehaviour
    {

        /// <summary>
        /// Peer connection this local data source will add a datachannel to.
        /// </summary>
        public PeerConnection peerConnection;
        
        /// <summary>
        /// Queue that contains Recieve calls to dequeue and run during the Update call.
        /// </summary>
        protected ConcurrentQueue<Action> _mainThreadWorkQueue = new ConcurrentQueue<Action>();
        // formatter used for serializing/deserializing
        protected BinaryFormatter binaryFormatter = new BinaryFormatter();
        protected DataChannel dataChannel = null;
        // the frequency at which update is called
        public float refreshrate = 0.1f;
        // the channelID the dataChannel has
        public DataChannels ChannelID;
        // whether the packet have to be recieved in order
        private bool ordered;
        // whether packets are important or can be skipped
        private bool reliable;
        // the last time that send was called
        protected float update = 0;

        public LocalDataSource(DataChannels ChannelID, bool ordered, bool reliable)
        {
            this.ChannelID = ChannelID;
            this.ordered = ordered;
            this.reliable = reliable;
        }
        
        protected void Awake()
        {
            if (peerConnection == null)
                throw new Exception("No peer Connection provided");
            peerConnection.OnInitialized.AddListener(OnPeerInitialized);
        }

        protected void OnDestroy()
        {
            peerConnection.OnInitialized.RemoveListener(OnPeerInitialized);
        }

        /// <summary>
        /// Callback when the Unity component is enabled. This is the proper way to enable the
        /// video source and get it to start video capture and enqueue video frames.
        /// </summary>
        protected void OnEnable()
        {
            var nativePeer = peerConnection?.Peer;
            if ((nativePeer != null) && nativePeer.Initialized)
            {
                DoAutoStartActions(nativePeer);
            }
        }

        /// <summary>
        /// performs necessary setup after the PeerConnection is initialized
        /// </summary>
        private void OnPeerInitialized()
        {
            var nativePeer = peerConnection.Peer;

            // Only perform auto-start actions (add track, start capture) if the component
            // is enabled. Otherwise just do nothing, this component is idle.
            if (enabled)
            {
                DoAutoStartActions(nativePeer);
            }
        }

        /// <summary>
        /// creates a dataChannel on the nativePeer and subscribes to its messages.
        /// </summary>
        /// <param name="nativePeer"></param>
        private void DoAutoStartActions(WebRTC.PeerConnection nativePeer)
        {
            //(ushort id), string label, bool ordered, bool reliable
            nativePeer.AddDataChannelAsync((ushort)ChannelID, ChannelID.ToString(), ordered, reliable).ContinueWith((prevTask) =>
            {
                if (prevTask.Exception != null)
                {
                    throw prevTask.Exception;
                }
                dataChannel = prevTask.Result;
                dataChannel.MessageReceived += PosMessageReceived;
            });

        }

        /// <summary>
        /// This method is called when a new T object was recieved.
        /// </summary>
        public abstract void Recieve(T t);

        /// <summary>
        /// This method is called by the dataChannel when new data is recieved.
        /// It will deserialize the data into a T object and call Recieve.
        /// </summary>
        protected void PosMessageReceived(byte[] message)
        {
            try
            {
                T t = (T)binaryFormatter.Deserialize(new MemoryStream(message));
                _mainThreadWorkQueue.Enqueue(() =>
                {
                    Recieve(t);
                });
            }
            catch (InvalidCastException)
            {
                Debug.LogError("Failed to desilialize C:" + ChannelID.ToString() + " s:" + message.Length);
            }
        }

        /// <summary>
        /// This method is called to get the next serializable object to send.
        /// Called once every "refreshrate" Seconds. If it is not nessesary to send something return null.
        /// </summary>
        public abstract T Send();

        /// <summary>
        /// This method is called when new data could be send.
        /// It will call send() and serialize the data and send it using the dataChannel.
        /// </summary>
        protected void PosMessageSend()
        {
            var r = Send();
            if (r == null)
                return;
            using (var ms = new MemoryStream()) { 
                binaryFormatter.Serialize(ms, r);
                dataChannel.SendMessage(ms.ToArray());
            }
        }

        /// <summary>
        /// Unity Engine Update() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
        /// </remarks>
        protected void Update()
        {
            // Execute any pending work enqueued by background tasks
            while (_mainThreadWorkQueue.TryDequeue(out Action workload))
            {
                workload();
            }
            if (dataChannel != null && dataChannel.State == DataChannel.ChannelState.Open)
            {
                update += Time.deltaTime;
                if (refreshrate == 0.0f || update > refreshrate)
                {
                    update %= refreshrate;
                    PosMessageSend();
                }

            }
        }
    }
}
