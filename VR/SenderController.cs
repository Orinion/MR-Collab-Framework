using TMPro;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// This component is sending the TaskData to the HoloLens
/// </summary>
public class SenderController : LocalDataSender<TaskData>
{
    /// <summary>
    /// This object will display the current task to the VR User
    /// </summary>
    public TextMeshPro text;

    /// <summary>
    /// This is the predefined list of steps of the maintenance task
    /// </summary>
    private readonly string[] tasks = { "unplug Device", "remove screw on back of the case", "remove upper panel", "remove screws on top of the case","remove side panels", 
        "unplug hdd", "remove screws from harddrive", "remove hdd", "put in replacement hdd", "screw in replacement hdd", "plug in harddrive",
        "add sidepanels", "screw in side panels", "add upper panel", "screw in upper panel"};

    // the index of the current task
    int currentTask = 0;

    // the last send task index
    private int lastTaskSend = -1;

    public SenderController() : base(DataChannels.TaskData, true, false)    {}

    public override TaskData Send()
    {
        if (currentTask != lastTaskSend)
        {
            // send new task
            text.text = tasks[currentTask];
            lastTaskSend = currentTask;
            return new TaskData((float)currentTask / tasks.Length, tasks[currentTask]);
        }
        return null;
    }

    /// <summary>
    /// This sets the current task to the next one
    /// </summary>
    public void NextTask() { 
        currentTask += 1;  
        currentTask %= tasks.Length; 
    }


    private void OnGUI()
    {
        GUI.Label(new Rect(40, 40, 100, 20), tasks[currentTask]);
    }
}
