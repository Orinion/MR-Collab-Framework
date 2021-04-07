using Microsoft.MixedReality.WebRTC;
using Microsoft.MixedReality.WebRTC.Unity;

/// <summary>
/// This class is based on the LocalDataSource but allows only the sending of objects of the T class.
/// To implement this class you have to implement the Send method.
/// </summary>
/// <typeparam name="T">a class that is serializable and will be send through the dataChannel</typeparam>
public abstract class LocalDataSender<T> : LocalDataSource<T>
{
    public LocalDataSender(DataChannels ChannelID, bool ordered, bool reliable) : base(ChannelID, ordered, reliable)
    {
    }

    /// <summary>
    /// We implement the Recieve function so that only Send has to be implemented
    /// </summary>
    public override void Recieve(T t)
    {
        return;
    }

    // override update to not recieve messages
    new void Update()
    {
        if (dataChannel != null && dataChannel.State == DataChannel.ChannelState.Open)
        {
            update += UnityEngine.Time.deltaTime;
            if (refreshrate == 0.0f || update > refreshrate)
            {
                update %= refreshrate;
                PosMessageSend();
            }

        }
    }
}
