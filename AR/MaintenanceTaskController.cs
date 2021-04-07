
/// <summary>
/// This component is recieving the TaskData from the VR User
/// </summary>
public class MaintenanceTaskController : LocalDataReciever<TaskData>, IProgressInterface
{
    private float currentProgress = 0;
    private string currentDescription = "Waiting to recieve TaskData";

    public MaintenanceTaskController() : base(DataChannels.TaskData, true, true)
    {
    }

    public string getMesssage()
    {
        return currentDescription;
    }

    public float getProgress()
    {
        return currentProgress;
    }

    public bool RecievedMessage()
    {
        return currentProgress > 0;
    }

    public override void Recieve(TaskData t)
    {
        currentProgress = t.progress * 0.9f + 0.1f;
        currentDescription = t.description;
    }
     
}
