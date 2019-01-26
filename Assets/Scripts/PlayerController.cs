using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private string horizontalAxis;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [SerializeField] private string verticalAxis;
    [SerializeField] private string pickDropButton;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    public Collider pickableDetector;
    public Joint pickableJoint;
    [NonSerialized] public Pickable objectToPickUp;
    [NonSerialized] public Pickable pickedUpObject;
    private Rigidbody rb;

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

            }
            else if (pickedUpObject)
            {
                pickedUpObject.HandleDropped();
                pickedUpObject = null;
                Destroy(pickableJoint);
            }
        }
    }

    void FixedUpdate()
    {
        var moveVector = new Vector3(Input.GetAxis(horizontalAxis), 0, Input.GetAxis(verticalAxis));
        rb.velocity = moveVector.sqrMagnitude > .1f ? moveVector * movementSpeed : Vector3.zero;
        
        float angularVelocityY = Vector3.SignedAngle(transform.forward, moveVector, Vector3.up);
        rb.angularVelocity = new Vector3(0f, angularVelocityY, 0f);
        
//        rb.AddForce(moveVector * movementSpeed, ForceMode.Acceleration);
//        var angle = Vector3.SignedAngle(transform.forward, moveVector, Vector3.up);
//        rb.AddTorque(0f, rotationSpeed * angle * angle * Mathf.Sign(angle), 0f, ForceMode.Acceleration);
    }
}