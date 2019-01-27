using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float STICK_DEADZONE = .1f;
    
    [SerializeField] private string horizontalAxis;
    [SerializeField] private string verticalAxis;
    [SerializeField] private string pickDropButton;
    [SerializeField] private string liftButton;
    [SerializeField] private string dashButton;
    [SerializeField] private string furniturePreviewButton;
    [SerializeField] private string lockMovementButton;
    [SerializeField] private string horizontalRightAxis;
    [SerializeField] private string verticalRightAxis;
    
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    
    [SerializeField] private float dashSpeedMultiplier;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [SerializeField] private ParticleSystem dustParticleSystem;

    private float dashEndTime;
    private float dashAvailableTime;

    public PickableDetector pickableDetector;
    public Joint pickableJoint;

    private Vector3 startAnchor;
    
    
    [NonSerialized] public Pickable pickedUpObject;
    private Rigidbody rb;

    [Header("Animations")] [SerializeField]
    private Animator m_animator;

    public SkinnedMeshRenderer SkinnedRenderer => m_animator.GetComponentInChildren<SkinnedMeshRenderer>();

    [SerializeField] private string m_forwardSpeedParam = "ForwardSpeed";
    [SerializeField] private string m_sidewaysSpeedParam = "SidewaySpeed";
    [SerializeField] private string m_holdingParam = "Holding";
    [SerializeField] private string m_holdingUpParam = "HoldingHeight";


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        var emission = dustParticleSystem.emission;
        emission.enabled = false;
    }

    private void OnDisable()
    {
        var emission = dustParticleSystem.emission;
        emission.enabled = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        m_animator.SetFloat(m_forwardSpeedParam, 0);
        m_animator.SetFloat(m_sidewaysSpeedParam, 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown(pickDropButton))
        {
            pickableDetector.RefreshOrder();
            var objectToPickUp = pickableDetector.ObjectToPick;
            if (pickedUpObject)
            {
	            m_animator.SetFloat(m_holdingParam, 0);
	            
                pickableDetector.Clear();
                pickedUpObject.HandleDropped(this);
                pickedUpObject = null;
                Destroy(pickableJoint);
            }
            else if (objectToPickUp && objectToPickUp.CanPickUp(this))
            {
	            m_animator.SetFloat(m_holdingParam, 1);
	            
                pickedUpObject = objectToPickUp;
                objectToPickUp.HandlePickedUp(this, pickableDetector.PickupPoint);
                objectToPickUp = null;

                var pickedUpObjectRb = pickedUpObject.GetComponent<Rigidbody>();
                pickableJoint = gameObject.AddComponent<FixedJoint>();
                pickableJoint.connectedBody = pickedUpObjectRb;
                startAnchor = pickableJoint.anchor;
                pickableJoint.autoConfigureConnectedAnchor = false;
                m_animator.SetFloat(m_holdingUpParam, 0f);
            }
        }

        if (Input.GetButtonDown(liftButton) && pickedUpObject)
            pickedUpObject.HandleLiftTapped(this);

        if (Input.GetButtonDown(dashButton) && !pickedUpObject && dashAvailableTime < Time.timeSinceLevelLoad)
        {
            dashEndTime = Time.timeSinceLevelLoad + dashTime;
            dashAvailableTime = Time.timeSinceLevelLoad + dashCooldown;
        }

        if (Input.GetButtonDown(furniturePreviewButton))
        {
            FurniturePreview.ShowPreview(true);
        }
        else if (Input.GetButtonUp(furniturePreviewButton))
        {
            FurniturePreview.ShowPreview(false);
        }
    }

    void FixedUpdate()
    {
        var moveVector = new Vector3(Input.GetAxis(horizontalAxis), 0f, Input.GetAxis(verticalAxis));
        var rotateVector = new Vector3(Input.GetAxis(horizontalRightAxis), 0f, Input.GetAxis(verticalRightAxis));

        if (moveVector.sqrMagnitude > STICK_DEADZONE)
        {
            rb.velocity = Input.GetButton(lockMovementButton) ? Vector3.zero : moveVector * movementSpeed * CurrentDashMultiplier;

            m_animator.SetFloat(m_forwardSpeedParam, Vector3.Dot(rb.velocity, transform.forward));
            m_animator.SetFloat(m_sidewaysSpeedParam, Vector3.Dot(rb.velocity, transform.right));
            var emission = dustParticleSystem.emission;
            emission.enabled = true;
        }
        else
        {
            var emission = dustParticleSystem.emission;
            emission.enabled = false;
	        rb.velocity = Vector3.zero;
	        
	        m_animator.SetFloat(m_forwardSpeedParam, 0);
	        m_animator.SetFloat(m_sidewaysSpeedParam, 0);
        }

        if (moveVector.sqrMagnitude > STICK_DEADZONE && rotateVector.sqrMagnitude <= STICK_DEADZONE)
            rotateVector = moveVector;

        if (rotateVector.sqrMagnitude > STICK_DEADZONE)
        {
            float angularVelocityY = Vector3.SignedAngle(transform.forward, rotateVector, Vector3.up);
            rb.angularVelocity = new Vector3(0f, angularVelocityY, 0f);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

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
        m_animator.SetFloat(m_holdingUpParam, currentLift / pickedUpObject.maxLift);
        pickableJoint.anchor = startAnchor + new Vector3(0f, currentLift, 0f);
    }
}