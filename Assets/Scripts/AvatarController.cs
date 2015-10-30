using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(CapsuleCollider))]

public class AvatarController: NetworkBehaviour
{
    public float ForwardSpeed = 10.0f;
    public float TurnSpeed = 100;

    float inputDelay = 0.1f;
    Rigidbody rigidBody;
    CapsuleCollider capsuleCollider;
    Animator animator;
    float turnAmount;
    float forwardAmount;
    Quaternion desiredRotation;

    public Quaternion DesiredRotation
    {
        get { return desiredRotation;  }
    }

    // Input Amounts - Eventually these will come from a separate controller
    float forwardInput = 0.0f;
    float turnInput = 0.0f;

    // Called after we've been spawned
    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer called..");
        base.OnStartLocalPlayer();

        // If we're the local player, find the avatar camera, and attach it to us
        if (isLocalPlayer)
        {
            AvatarCamera avatarCamera = GameObject.FindObjectOfType<AvatarCamera>();
            if (avatarCamera == null)
            {
                Debug.LogError("Unable to locate Avatar Camera for local client..");
                return;
            }
            avatarCamera.AttachAvatar(this.gameObject);
        }
    }


    /// <summary>
    /// Property Initializaion
    /// </summary>
    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        desiredRotation = transform.rotation;
        forwardInput = 0.0f;
        turnInput = 0.0f;
	}

    /// <summary>
    /// Very simple GetInput Function. Eventually, we'll have a separate input controller to handle user
    /// input. But to get this up and running, we'll just grab current inputs..
    /// </summary>
    void GetInput()
    {
        forwardInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// Non physics Updates
    /// </summary>
    void Update ()
    {
        if (isLocalPlayer)
        {
            GetInput();
            DoTurn();
        }
	}

    /// <summary>
    /// Physics Updates get updated here
    /// </summary>
    void FixedUpdate()
    {
        if (isLocalPlayer)
            DoMovement();
    }

    /// <summary>
    /// Calculate Forward Movement Velocity
    /// </summary>
    void DoMovement()
    {
        // Only start to move when we've exceeded the input threshold..
        if (Mathf.Abs(forwardInput) < inputDelay)
        {
            rigidBody.velocity = Vector3.zero;
            animator.SetFloat("ForwardSpeed", 0.0f);
            return;
        }

        // Clamp Backwards Movement to only be half of max forward movement..
        if (forwardInput < 0.0f)
            forwardInput = Mathf.Max(-0.5f, forwardInput);

        // Adjust forward velocity by runspeed * input..
        forwardAmount = forwardInput * ForwardSpeed;
        animator.SetFloat("ForwardSpeed", forwardAmount);
        rigidBody.velocity = transform.forward * forwardAmount;

    }

    void DoTurn()
    {
        if (Mathf.Abs(turnInput) >= inputDelay)
        {
            desiredRotation *= Quaternion.AngleAxis(TurnSpeed * turnInput * Time.deltaTime, Vector3.up);
            rigidBody.rotation = desiredRotation;
        }
    }



}
