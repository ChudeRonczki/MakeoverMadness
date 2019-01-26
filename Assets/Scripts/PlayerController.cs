using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private string horizontalAxis;
    [SerializeField] private string verticalAxis;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    private CharacterController characterController;
    [SerializeField] private float pushForce;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var moveVector = new Vector3(Input.GetAxis(horizontalAxis), 0, Input.GetAxis(verticalAxis));

        if (Math.Abs(moveVector.sqrMagnitude) > .1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation,
                Quaternion.LookRotation(moveVector), rotationSpeed * Time.deltaTime);
            
            characterController.Move(moveVector * Time.deltaTime * movementSpeed);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody == null)
            return;
        
        hit.rigidbody.AddForceAtPosition(pushForce * hit.moveDirection, hit.point);
    }
}