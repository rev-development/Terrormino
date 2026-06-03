using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MatchGrabHand : XRGrabInteractable
{
    public Transform LeftAttach;
    public Transform RightAttach;

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            attachTransform = LeftAttach;
        }
        else if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            attachTransform = RightAttach;
        }

        base.OnSelectEntering(args);
    }
}
