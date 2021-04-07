using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// This is component is used to subscribe to the SteamVR input Events
/// </summary>
public class SteamVRController : MonoBehaviour
{
    public MarkerSystem markerSystem;

    public SteamVR_Action_Vector2 MovePlayer;
    public SteamVR_Action_Boolean PlaceMarker;
    public SteamVR_Action_Boolean ChooseMarker;
    public SteamVR_Action_Vector2 ResizeMarker;

    public SteamVR_Input_Sources handTypeMarker;
    public SteamVR_Input_Sources handTypeMove;
    public float speed = 2.0f;

    Vector2 last_vector_move;

    // Start is called before the first frame update
    void Start()
    {
        MovePlayer.AddOnChangeListener(OnChangeMovePlayer, handTypeMove);
        PlaceMarker.AddOnChangeListener(OnChangePlace, handTypeMarker);
        ChooseMarker.AddOnChangeListener(OnChangeChoose, handTypeMove);
        ResizeMarker.AddOnChangeListener(OnChangeResize, handTypeMarker);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayer();
    }

    public void OnChangeMovePlayer(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        last_vector_move = axis;
    }

    public void OnChangeChoose(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        markerSystem.OnChangeChoose(newState);
    }

    public void OnChangeResize(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        markerSystem.OnChangeResize(axis);
    }

    public void OnChangePlace(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        markerSystem.Placemarker(newState);
    }

    private void UpdatePlayer()
    {
        Player player = Player.instance;
        if (!player)
        {
            return;
        }
        // allow the user to "fly" using the touchpad
        Vector3 orientation = Camera.main.transform.forward;
        Vector3 moveDirection = orientation * last_vector_move.y + Camera.main.transform.rotation * Vector3.right * last_vector_move.x;
        Vector3 pos = player.transform.position;
        pos += moveDirection * speed * Time.deltaTime;
        player.transform.position = pos;
    }
}
