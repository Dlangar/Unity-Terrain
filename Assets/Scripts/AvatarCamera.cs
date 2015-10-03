using UnityEngine;
using System.Collections;

public class AvatarCamera : MonoBehaviour
{

    public Transform TargetObj;
    public float LookSmoothFactor = 0.09f;
    public Vector3 TargetOffset = new Vector3(0, -6, -8);
    public float CameraTilt = 10;

    Vector3 Destination = Vector3.zero;
    AvatarController TargetController;
    float TurnSpeed = 0.0f;


    /// <summary>
    /// Initialization of Controller, and other pertinent things I'm sure. 
    /// Actually this is just a small code change to test out updating of GIT changes and commits. 
    /// </summary>
    void Start ()
    {
        // Init Controller..
        TargetController = TargetObj.GetComponent<AvatarController>();
        if (TargetController == null)
        {
            Debug.LogError("Target Object does not contain an avatar controller.");
            return;
        }
	}
	
	// Update is called once per frame
    /// <summary>
    /// This happens after Update & FixedUdates, so we update our position here, to make sure we're using the most recent information
    /// for the controller, based on this frame
    /// </summary>
	void LateUpdate ()
    {
        MoveToTarget();
        LookAtTarget();
	}

    void MoveToTarget()
    {
        // Rotated Point
        Destination = TargetController.DesiredRotation * TargetOffset;
        Destination += TargetObj.position;
        transform.position = Destination;
    }

    void LookAtTarget()
    {
        // Smooth Damp Angle will peform a nicely smoothed interpreted turn from source to target, returning a rotational speed, given a time to turn by.
        float eulerAngleY = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetObj.eulerAngles.y, ref TurnSpeed, LookSmoothFactor);
        transform.rotation = Quaternion.Euler(TargetObj.eulerAngles.x + (CameraTilt * Mathf.Deg2Rad), eulerAngleY, 0);
        

    }

}
