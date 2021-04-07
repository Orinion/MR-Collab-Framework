using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to send the position and rotation of a multiple local object and recieve multiple different from the remote person
/// </summary>
public class MultiPositionSync : Microsoft.MixedReality.WebRTC.Unity.LocalDataSource<Marker>
{
    /// <summary>
    /// The list of models you want to send to the peer.
    /// </summary>
    public GameObject[] localObjects;

    /// <summary>
    /// The list of models you want to recieve positions of.
    /// </summary>
    public GameObject[] remoteObjects;

    /// <summary>
    /// a list of last send transforms
    /// </summary>
    private TransformS[] lastSend;

    // the index of the last updated local object to avoid one from blocking the updates of others
    private int lastSendIndex = 0;

    public MultiPositionSync() : base(DataChannels.Head, true, true)
    {
    }

    public override void Recieve(Marker m)
    {
        remoteObjects[m.id].transform.SetPositionAndRotation(m.transform.pos, m.transform.rot);
        remoteObjects[m.id].GetComponentInChildren<MeshRenderer>().enabled = !m.transform.pos.Equals(Vector3.zero);
    }

    public override Marker Send()
    {
        for (int i = 0; i < localObjects.Length; i++)
        {
            var index = (i + lastSendIndex + 1) % localObjects.Length;
            var c = localObjects[index];
            if (c == null)
                continue;
            if (lastSend[index] != null && lastSend[index].Equals(c.transform))
                continue;
            lastSend[index] = new TransformS(c.transform);
            lastSendIndex = index;
            return new Marker(index, 0, lastSend[index]);
        }
        return null;
    }

    void Start()
    {
        if (localObjects.Length != remoteObjects.Length)
            throw new System.Exception("the array length must match.");
        lastSend = new TransformS[localObjects.Length];
        foreach (var g in remoteObjects)
            g.GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}
