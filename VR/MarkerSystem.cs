using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// This component is used to allow the VR user to place, remove, resize and move markers
/// </summary>
public class MarkerSystem : MonoBehaviour
{
    /// <summary>
    /// The MarkerSyncronizer that contains the marker data
    /// </summary>
    public MarkerSyncronizer syncronizer;
    /// <summary>
    /// The controller that manages the Progress of the maintenance task.
    /// </summary>
    public SenderController senderController;
    
    // a reference to the hands
    public Hand left, right;

    /// <summary>
    /// The GameObject attatched to a hand that is used as a preview when placing new markers and serves as the center for the selection menu.
    /// </summary>
    public GameObject previewMarker;

    /// <summary>
    /// The Material used for markers that aren't highlighted
    /// </summary>
    public Material defaultMaterial;
    /// <summary>
    /// The Material used for the marker that is highlighted
    /// </summary>
    public Material highlightedMaterial;

    // the index of the selected model
    private int selectedModel = 0;
    // the scale used for new markers
    private float placeScale = 0.4f;
    // the radius which is used for the selection menu
    private readonly float radius = 0.1f;
    // scale used to resize the entire menu
    private readonly float menuScale = 0.2f;



    // stores the gameobjects used for the marker selection
    private GameObject[] menuObjects;

    // is the choose button currently pressed
    private bool isChooseActive = false;
    // is the place button currently pressed
    private bool isPlaceActive = false;
    // array of vector3's that scale each menu item to the same size
    private Vector3[] scales;

    // the last value of the resize input
    private float lastScale = 0;

    /**
     * update the review marker to the current mesh and size
     */
    private void UpdatePreviewMarker()
    {
        var newMesh = syncronizer.prefab[selectedModel].GetComponent<MeshFilter>().sharedMesh;
        previewMarker.GetComponent<MeshFilter>().mesh = newMesh;
        previewMarker.transform.localScale = placeScale * scales[selectedModel];
    }

    private bool IsHoldingMarker() 
    {
         return left.currentAttachedObject != null || right.currentAttachedObject != null; 
    }

    private GameObject getHeldMarker()
    {
        return left.currentAttachedObject != null ? left.currentAttachedObject : right.currentAttachedObject;
    }
    private int GetClosestToController()
    {
        var closest = -1;
        var shortestDist = float.MaxValue;

        for (int i = 0; i < menuObjects.Length; i++)
        {
            var menuItem = menuObjects[i];
            var dist = Vector3.Distance(menuItem.transform.position, previewMarker.transform.position);
            if (dist < shortestDist) { 
                closest = i;
                shortestDist = dist;
            }
        }
        return closest;
    }
    
    // Method when choose pressed
    public void OnChangeChoose(bool newState)
    {
        isChooseActive = newState;
        foreach (GameObject menuItem in menuObjects)
            menuItem.GetComponent<MeshRenderer>().enabled = isChooseActive;
        
        if (isChooseActive)
        {
            transform.SetPositionAndRotation(previewMarker.transform.position, previewMarker.transform.rotation);
        }
        else
        {
            //Selection ended, select closest to controller
            selectedModel = GetClosestToController();

            if (selectedModel == syncronizer.prefab.Length)
            {
                //Next Task
                senderController.NextTask();
                return;
            }
            //update Mesh of preview
            UpdatePreviewMarker();
        }
    }

    public void Placemarker(bool newState)
    {
        isPlaceActive = newState;
        if (!isPlaceActive)
        {
            if (!IsHoldingMarker())
            {
                GameObject marker = Instantiate(previewMarker, previewMarker.transform.position, previewMarker.transform.rotation);
                marker.name = "Marker " + selectedModel;
                syncronizer.AddNewMarker(marker, selectedModel);
                Throwable t = marker.AddComponent<Throwable>();
                t.scaleReleaseVelocity = 0;
                var r = marker.GetComponent<Rigidbody>();
                r.useGravity = false; r.isKinematic = true;
                marker.AddComponent<MeshCollider>();
                Debug.Log("Create " + marker.name);
            }
            else
            {
                GameObject marker = getHeldMarker();
                Debug.Log("Destroy " + marker.name);
                syncronizer.RemoveMarker(marker);
                Hand h = marker.transform.parent.GetComponent<Hand>();
                h.DetachObject(marker);
                Destroy(marker);
            }
        }
    }

 
    public void OnChangeResize(Vector2 axis)
    {
        lastScale = axis.y;
    }

    // Start is called before the first frame update
    void Start()
    {

        var prefab = syncronizer.prefab;
        menuObjects = new GameObject[prefab.Length + 1]; // , Next Task
        scales = new Vector3[prefab.Length];
        //Create Menu
        for (int i = 0; i < prefab.Length; i++)
        {
            Vector3 bounds = prefab[i].GetComponent<MeshFilter>().sharedMesh.bounds.size;
            scales[i] = Vector3.one / bounds.magnitude;

            //place items in a circle
            float angle = i * Mathf.PI * 2 / prefab.Length;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 0.00f, Mathf.Sin(angle) * radius);
            Quaternion rot = Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 0);

            GameObject menuItem = Instantiate(prefab[i], pos, rot, transform);
            menuItem.GetComponent<MeshRenderer>().enabled = false;
            menuItem.GetComponent<MeshRenderer>().material = defaultMaterial;
            menuItem.transform.localScale = menuScale * scales[i];

            menuObjects[i] = menuItem;
        }

        menuObjects[prefab.Length] = Instantiate(previewMarker, new Vector3(0,0.1f,0), new Quaternion(0,0,90,0), transform);
        menuObjects[prefab.Length].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        UpdatePreviewMarker();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsHoldingMarker()) {
            previewMarker.GetComponent<MeshRenderer>().enabled = false;
            GameObject marker = getHeldMarker();
            marker.transform.localScale *= 1 + lastScale * Time.deltaTime * 3;
        }
        else
            if(selectedModel < scales.Length)
                if (lastScale != 0)
                {
                    placeScale *= 1 + lastScale * Time.deltaTime * 3;
                    // make sure the scale is bound so that the marker remains interactable
                    placeScale = Math.Min(1.5f, Math.Max(0.1f, placeScale));
                    previewMarker.transform.localScale = placeScale * scales[selectedModel];
                    previewMarker.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                    previewMarker.GetComponent<MeshRenderer>().enabled = isPlaceActive;

        if (isChooseActive)
        {
            //highlight selected Material
            var c = GetClosestToController();
            for (int i = 0; i < menuObjects.Length; i++) { 
                menuObjects[i].GetComponent<Renderer>().material = i == c ? highlightedMaterial : defaultMaterial;
                menuObjects[i].transform.Rotate(Vector3.up * Time.deltaTime);//rotate
            }
        }
    }
}
