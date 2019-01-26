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
    [SerializeField] private string dashButton;
    
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    
    [SerializeField] private float dashSpeedMultiplier;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    private float dashEndTime;
    private float dashAvailableTime;

    public Collider pickableDetector;
    public Joint pickableJoint;

    private Vector3 startAnchor;
    
    [NonSerialized] public Pickable objectToPickUp;
    [NonSerialized] public Pickable pickedUpObject;
    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown(pickDropButton))
        {
            if (pickedUpObject)
            {
                pickedUpObject.HandleDropped(this);
                pickedUpObject = null;
                Destroy(pickableJoint);
            }
            else if (objectToPickUp && objectToPickUp.CanPickUp(this))
            {
                pickedUpObject = objectToPickUp;
                objectToPickUp.HandlePickedUp(this);
                objectToPickUp = null;

                var pickedUpObjectRb = pickedUpObject.GetComponent<Rigidbody>();
                pickableJoint = gameObject.AddComponent<FixedJoint>();
                pickableJoint.connectedBody = pickedUpObjectRb;
                startAnchor = pickableJoint.anchor;
                pickableJoint.autoConfigureConnectedAnchor = false;
            }
        }

        if (Input.GetButtonDown(liftButton) && pickedUpObject)
            pickedUpObject.HandleLiftTapped(this);

        if (Input.GetButtonDown(dashButton) && !pickedUpObject && dashAvailableTime < Time.timeSinceLevelLoad)
        {
            dashEndTime = Time.timeSinceLevelLoad + dashTime;
            dashAvailableTime = Time.timeSinceLevelLoad + dashCooldown;
        }
    }

    void FixedUpdate()
    {
        var moveVector = new Vector3(Input.GetAxis(horizontalAxis), 0, Input.GetAxis(verticalAxis));

        if (moveVector.sqrMagnitude > .1f)
        {
            rb.velocity = moveVector * movementSpeed * CurrentDashMultiplier;
            float angularVelocityY = Vector3.SignedAngle(transform.forward, moveVector, Vector3.up);
            rb.angularVelocity = new Vector3(0f, angularVelocityY, 0f);

        }
        else
            rb.velocity = rb.angularVelocity = Vector3.zero;

//        rb.AddForce(moveVector * movementSpeed, ForceMode.Acceleration);
//        var angle = Vector3.SignedAngle(transform.forward, moveVector, Vector3.up);
//        rb.AddTorque(0f, rotationSpeed * angle * angle * Mathf.Sign(angle), 0f, ForceMode.Acceleration);
    }

    float CurrentDashMultiplier
    {
        get
        {
            if (pickedUpObject)
                return 1f;

            return dashEndTime >= Time.timeSinceLevelLoad ? dashSpeedMultiplier : 1f;
        }
    }

    public void UpdateLift(float currentLift)
    {
        pickableJoint.anchor = startAnchor + new Vector3(0f, currentLift, 0f);
    }
}