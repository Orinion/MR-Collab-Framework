using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUIController : MonoBehaviour
{
    /// <summary>
    /// The button that generates an offer to a given target
    /// </summary>
    [Tooltip("The button that generates an offer to a given target")]
    public PressableButtonHoloLens2 HideMenuButton;

    public GameObject[] objectsToHide;
    private bool isMenuEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        HideMenuButton.GetComponent<Interactable>().OnClick.AddListener(() => {
            ToggleMenus();
        });
    }

    void ToggleMenus()
    {
        isMenuEnabled = !isMenuEnabled;
        foreach (var g in objectsToHide)
        {
            g.SetActive(isMenuEnabled);
        }
        MarkerSyncronizer.machine.GetComponent<MeshRenderer>().enabled = isMenuEnabled;

        var dataProviderAccess = CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess;
        var observers = dataProviderAccess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();
        foreach (var observer in observers)
            observer.DisplayOption = isMenuEnabled ? SpatialAwarenessMeshDisplayOptions.Visible : SpatialAwarenessMeshDisplayOptions.None;
    }
}
