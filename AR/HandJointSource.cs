using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;


public class HandJointSource : LocalPositionSource
{
    public TrackedHandJoint joint;
    public Handedness hand;

    public HandJointSource() : base()
    {}

    // Start is called before the first frame update
    void Start()
    {
        var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        if (handJointService != null)
        {
            source = handJointService.RequestJointTransform(joint, hand);
        }
        else
            Debug.Log("Hand tracking failed");
    }

}
