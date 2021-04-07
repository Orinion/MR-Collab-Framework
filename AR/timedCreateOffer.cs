using Microsoft.MixedReality.WebRTC;
using Microsoft.MixedReality.WebRTC.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timedCreateOffer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        Invoke("sendOffer", 2);
#endif
    }

    void sendOffer()
    {
        GetComponent<NodeDssSignaler>().PeerConnection.StartConnection();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
