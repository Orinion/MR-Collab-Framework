using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class MarkerPlaceMachine : MonoBehaviour, IProgressInterface
{
    public MarkerSyncronizer markerSyncronizer;
    public PressableButtonHoloLens2 placeMachineButton;
    public Material previewMaterial;
    
    private GameObject previewMachine = null;
    private int spatialLayer = 1 << 31 ;
    // Start is called before the first frame update
    void Start()
    {
        placeMachineButton.GetComponent<Interactable>().OnClick.AddListener(() => {
            OnButtonPress();
        });

#if UNITY_EDITOR
        StartCoroutine(ExecuteAfterTime());
#endif
    }

    public void OnButtonPress()
    {
        if (previewMachine == null)
        {
            RaycastHit hit;
            Ray forwardRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (!Physics.Raycast(forwardRay, out hit, 10))
                return;
            previewMachine = Instantiate(markerSyncronizer.machine_prefab, hit.transform.position, hit.transform.rotation);
            previewMachine.name = "Preview Machine";
            previewMachine.GetComponent<MeshRenderer>().material = previewMaterial;
            var i = previewMachine.AddComponent<Interactable>();
            i.IsGlobal = true;
            i.OnClick.AddListener(() => {
                OnClickEvent();
            });
        }
        else
        {
            OnClickEvent();
        }
    }

    public void OnClickEvent()
    {
        if (previewMachine != null)
        {
            var p = previewMachine.transform.position;
            var r = previewMachine.transform.rotation;
            Destroy(previewMachine);
            Place_Machine(p, r);
        }
    }

#if UNITY_EDITOR
    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(4);
        Debug.Log("placing Machine");
        OnButtonPress();
        yield return new WaitForSeconds(4);
        OnButtonPress();


        yield return new WaitForSeconds(10);
        Debug.Log("placing Machine");
        OnButtonPress();
        yield return new WaitForSeconds(4);
        OnButtonPress();
    }
#endif

    void Place_Machine(Vector3 position, Quaternion rotation) 
    {
        if (MarkerSyncronizer.machine != null)
        {
            if (Controller.Instance.ReadyToSendMesh())
                return;
            Destroy(MarkerSyncronizer.machine.GetComponent<WorldAnchor>());
            MarkerSyncronizer.machine.transform.SetPositionAndRotation(position, rotation);
            MarkerSyncronizer.machine.AddComponent<WorldAnchor>();
            return;
        }
        //Send marker at location
        GameObject marker = Instantiate(markerSyncronizer.machine_prefab, position, rotation);
        marker.AddComponent<WorldAnchor>();
        markerSyncronizer.AddNewMarker(marker, -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (previewMachine == null)
            return;

        RaycastHit hit;
        Ray forwardRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(forwardRay, out hit, 3, spatialLayer))
        {
            //Set marker at location
            var euler_y = Camera.main.transform.rotation.eulerAngles.y;
            previewMachine.transform.SetPositionAndRotation(hit.point , Quaternion.Euler(0, euler_y + 180, 0));
        }
    }

    public float getProgress()
    {
        if (MarkerSyncronizer.machine != null)
            return 1;
        if (previewMachine != null)
            return 0.5f;
        return 0.0f;
    }

    public string getMesssage()
    {
        if (MarkerSyncronizer.machine != null)
            return "Placed Machine";
        if (previewMachine != null)
            return "Tap Anywhere To Place The Machine";
        return "Tap The Mark Machine Button";
    }
}
