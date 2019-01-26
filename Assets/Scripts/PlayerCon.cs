using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    
    [SerializeField] private string horizontalAxis;
    [SerializeField] private string verticalAxis;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float moveSpeed;

    private Rigidbody _rb;
    private Quaternion _lastLookRotation;
    private Vector3 _lastMoveVector;
    private float _positionY;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _positionY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        _lastMoveVector = new Vector3(Input.GetAxis(horizontalAxis), 0, Input.GetAxis(verticalAxis));
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(_lastMoveVector.sqrMagnitude) > .1f)
        {
            _rb.velocity = _lastMoveVector * moveSpeed;
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }
        
        float angularVelocityY = Vector3.SignedAngle(transform.forward, _lastMoveVector, Vector3.up);
        _rb.angularVelocity = new Vector3(0f, angularVelocityY, 0f);
    }

    private void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
        transform.position = new Vector3(transform.position.x, _positionY, transform.position.z);
    }
}
