using UnityEngine;


/// <summary>
/// This class is used to send the position and rotation of a single local object and recieve one from the remote person
/// </summary>
public class LocalPositionSource : Microsoft.MixedReality.WebRTC.Unity.LocalDataSource<TransformS>
{
    public LocalPositionSource() : base(DataChannels.Head, true, false) { }
    
    /// <summary>
    /// The Gameobject that represents the position of the other person.
    /// </summary>
    public GameObject destination;

    /// <summary>
    /// The Gameobject that gives the position that is send to the other player.
    /// </summary>
    public Transform source;

    // this variable stores the last send position
    private TransformS lastSendPos = new TransformS(Vector3.zero, Quaternion.identity);

    public override void Recieve(TransformS t)
    {
        // update the position and rotation
        destination.transform.SetPositionAndRotation(t.pos, t.rot);
        // draw the model if the position isnt zero
        destination.GetComponentInChildren<MeshRenderer>().enabled = !t.pos.Equals(Vector3.zero);
    }

    public override TransformS Send()
    {
        var t = new TransformS(source);
        // Avoid sending new Data when it has not changed
        if (t.Equals(lastSendPos)) 
            return null;
        // remember new position
        lastSendPos = t;
        return t;
    }

    void Start()
    {
        // dont draw the model untill the peer connection is established
        if (destination != null)
            destination.GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}

