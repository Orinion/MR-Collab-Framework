using Microsoft.MixedReality.WebRTC.Unity;

/// <summary>
/// This class is based on the LocalDataSource but allows only the recieving of objects of the T class.
/// To implement this class you have to implement the Recieve method.
/// </summary>
/// <typeparam name="T">a class that is serializable and will be send through the dataChannel</typeparam>
public abstract class LocalDataReciever<T> : LocalDataSource<T>
{

    public LocalDataReciever(DataChannels ChannelID, bool ordered, bool reliable) : base(ChannelID, ordered, reliable)
    {
    }

    /// <summary>
    /// We implement the send function so that only Recieve has to be implemented
    /// </summary>
    public override T Send()
    {
        return default;
    }


    // override update to not send messages
    new void Update()
    {
        // Execute any pending work enqueued by background tasks
        while (_mainThreadWorkQueue.TryDequeue(out System.Action workload))
        {
            workload();
        }
    }
}
