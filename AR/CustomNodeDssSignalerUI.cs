// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.WebRTC.Unity;


public class CustomNodeDssSignalerUI : MonoBehaviour, IProgressInterface
{
    public NodeDssSignaler NodeDssSignaler;

    private bool createOfferCalled = false;
    private bool connected = false;

    /// <summary>
    /// The button that generates an offer to a given target
    /// </summary>
    [Tooltip("The button that generates an offer to a given target")]
    public PressableButtonHoloLens2 CreateOfferButton;

    void Start()
    {
        CreateOfferButton.GetComponent<Interactable>().OnClick.AddListener(() => {
            CreateOffer();
            //call again
            if(!Controller.Instance.ReadyToSendMesh())
                StartCoroutine(ExecuteAfterTime());
        });

#if UNITY_EDITOR
        StartCoroutine(ExecuteAfterTime());
#endif
    }

    void Update() {
        if (!connected)
            connected = NodeDssSignaler.PeerConnection.isActiveAndEnabled;
                }

    public void CreateOffer()
    {
        if (!Controller.Instance.ReadyToStartConnection())
            Debug.Log("Not ready yet");
        Debug.Log("Sending Offer:" + NodeDssSignaler?.PeerConnection?.StartConnection());
        createOfferCalled = true;
    }


    System.Collections.IEnumerator ExecuteAfterTime()
    {
        float time = 10.0f;
        while(!connected || !Controller.Instance.ReadyToStartConnection())
            yield return new WaitForSeconds(time);

        if (!Controller.Instance.ReadyToSendMesh())
            CreateOffer();
            //Debug.Log("Send offer " +NodeDssSignaler?.PeerConnection?.Peer?.CreateOffer());

        createOfferCalled = true;
    }


    public float getProgress()
    {

        if (!connected)
            return 0f;
        if (!createOfferCalled)
            return 0.5f;
        if (!Controller.Instance.ReadyToSendMesh())
            return 0.75f;
        return 1;
    }

    public string getMesssage()
    {
        return !connected ? "Establishing connection" : !createOfferCalled ? "Press Connect to continue"  : !Controller.Instance.ReadyToSendMesh() ? "Connecting" : "connected";
    }
}
