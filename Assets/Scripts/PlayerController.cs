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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var moveVector = new Vector3(Input.GetAxis(horizontalAxis), 0, Input.GetAxis(verticalAxis));

        if (Math.Abs(moveVector.sqrMagnitude) > .1f)
        {
            transform.localPosition += moveVector * Time.deltaTime * movementSpeed;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation,
                Quaternion.LookRotation(moveVector), rotationSpeed * Time.deltaTime);
        }
        
        
    }
}