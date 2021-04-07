using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Controller : Singleton<Controller>
{
    public MarkerPlaceMachine markerPlace;
    public CustomNodeDssSignalerUI customSignaler;
    public CustomObserver customObserver;
    public MaintenanceTaskController taskController;

    public ProgressIndicatorLoadingBar progressIndicator;
    
    List<IProgressInterface> tasks;
    int currentTask = 0;

    public bool ReadyToStartConnection() { return MarkerSyncronizer.machine != null; }//return currentTask > 0; /** only send offer when Camera is available again **/ }

    public bool ReadyToSendMesh() { return taskController.RecievedMessage(); /** only send Mesh when the connection is established **/ }


    // Start is called before the first frame update
    void Start()
    {
        tasks = new List<IProgressInterface>(new IProgressInterface[]{ markerPlace, customSignaler, customObserver, taskController });
        UpdateProgressBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void UpdateProgressBar()
    {
        Debug.Log("starting progressbar");
        await progressIndicator.OpenAsync();
        float lastProgress = tasks[0].getProgress();
        progressIndicator.Message = tasks[0].getMesssage();

        while (currentTask < tasks.Count)
        {
            await Task.Yield();
            if (progressIndicator.State != ProgressIndicatorState.Open)
                return;
            var newProgress = tasks[currentTask].getProgress();
            if (lastProgress == newProgress && lastProgress != 1.0f)
                continue;
            lastProgress = newProgress;
            progressIndicator.Progress = lastProgress;
            progressIndicator.Message = tasks[currentTask].getMesssage();
            if (lastProgress == 1.0f)
            {
                //Wait for 1 second to let the user better understand the programm flow.
                float time = Time.time;
                while (Time.time - time > 1)
                    await Task.Yield();
                lastProgress = -1;
                currentTask += 1;
            }
        }
        progressIndicator.Message = "All Tasks Finished";

        await progressIndicator.CloseAsync();
        Debug.Log("Progressbar finished");
    }

}
