using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private string horizontalAxis;
    [SerializeField] private string verticalAxis;
    [SerializeField] private string pickDropButton;
    [SerializeField] private string liftButton;
    
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    public Collider pickableDetector;
    public Joint pickableJoint;

    public float maxLift;
    public float perTapLift;
    public float liftFallSpeed;
    private Vector3 startAnchor;
    private float currentLift;
    private bool liftTapped;
    
    [NonSerialized] public Pickable objectToPickUp;
    [NonSerialized] public Pickable pickedUpObject;
    private Rigidbody rb;
    private Vector3 lastMoveVector;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown(pickDropButton))
        {
            if (objectToPickUp)
            {
                pickedUpObject = objectToPickUp;
                objectToPickUp.HandlePickedUp();
                objectToPickUp = null;

                var pickedUpObjectRb = pickedUpObject.GetComponent<Rigidbody>();
                pickableJoint = gameObject.AddComponent<FixedJoint>();
                pickableJoint.connectedBody = pickedUpObjectRb;
                startAnchor = pickableJoint.anchor;
                pickableJoint.autoConfigureConnectedAnchor = false;
            }
            else if (pickedUpObject)
            {
                pickedUpObject.HandleDropped();
                pickedUpObject = null;
                Destroy(pickableJoint);
            }
        }

        if (Input.GetButtonDown(liftButton))
            liftTapped = true;
    }

    void FixedUpdate()
    {
        var moveVector = new Vector3(Input.GetAxis(horizontalAxis), 0, Input.GetAxis(verticalAxis));

        if (moveVector.sqrMagnitude > .1f)
            rb.velocity = moveVector * movementSpeed;
        else
        {
            rb.velocity = Vector3.zero;
            moveVector = lastMoveVector;
        }
        
        float angularVelocityY = Vector3.SignedAngle(transform.forward, moveVector, Vector3.up);
        rb.angularVelocity = new Vector3(0f, angularVelocityY, 0f);

        lastMoveVector = moveVector;

        if (pickableJoint)
            UpdateLift();

//        rb.AddForce(moveVector * movementSpeed, ForceMode.Acceleration);
//        var angle = Vector3.SignedAngle(transform.forward, moveVector, Vector3.up);
//        rb.AddTorque(0f, rotationSpeed * angle * angle * Mathf.Sign(angle), 0f, ForceMode.Acceleration);
    }

    private void UpdateLift()
    {
        if (liftTapped)
        {
            currentLift = Mathf.Min(maxLift, currentLift + perTapLift);
            liftTapped = false;
        }
        else if (currentLift > 0f)
        {
            currentLift = Mathf.Max(0f, currentLift - liftFallSpeed * Time.fixedDeltaTime);
        }
        else
            return;

        pickableJoint.anchor = startAnchor + new Vector3(0f, currentLift, 0f);
    }
}