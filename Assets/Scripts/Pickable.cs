using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    private static float MASSIVE_MASS = 10000f;
    
    public Collider[] pickPointsColliders;
    public float pickUpMass;
    public int pickUpPlayersCount;
    
    public float maxLift;
    public float perTapLift;
    public float liftFallSpeed;
    private float currentLift;

    private Rigidbody rb;
    private List<PlayerController> pickingUp;
    private float drag;
    private float angularDrag;
    private float defaultMass;

    private Dictionary<PlayerController, bool> liftTapped;
    private Dictionary<PlayerController, Collider> usedPoints;
    [SerializeField] private AudioClip hitClip;


    private void Awake()
    {
        pickingUp = new List<PlayerController>();
        liftTapped = new Dictionary<PlayerController, bool>();
        usedPoints = new Dictionary<PlayerController, Collider>();
        
        rb = GetComponent<Rigidbody>();
        
        if (pickUpPlayersCount == 0)
            pickUpPlayersCount = 1;

        if (pickUpMass == 0f)
            pickUpMass = rb.mass;
        
        drag = rb.drag;
        angularDrag = rb.angularDrag;
        defaultMass = rb.mass;

        if (pickUpPlayersCount == 1)
        {
            foreach (var point in pickPointsColliders)
            {
                point.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public bool CanPickUp(PlayerController player)
    {
        return !pickingUp.Contains(player) && pickingUp.Count != pickUpPlayersCount;
    }

    public void HandlePickedUp(PlayerController player, Collider usedPoint)
    {
        pickingUp.Add(player);
        liftTapped.Add(player, false);
        usedPoints.Add(player, usedPoint);
        
        if (pickingUp.Count == pickUpPlayersCount)
        {
            foreach (var collider in pickPointsColliders)
            {
                collider.enabled = false;
                collider.GetComponent<MeshRenderer>().enabled = false;

            }
            
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.useGravity = false;
            rb.mass = pickUpMass;
        }
        else
        {
            usedPoint.enabled = false;
            usedPoint.GetComponent<MeshRenderer>().enabled = false;
            rb.mass = MASSIVE_MASS;
        }
    }

    public void HandleDropped(PlayerController player)
    {
        pickingUp.Remove(player);
        liftTapped.Remove(player);
        usedPoints.Remove(player);

        foreach (var collider in pickPointsColliders)
        {
            if (usedPoints.ContainsValue(collider))
                continue;
            
            collider.enabled = true;
            if (pickUpPlayersCount > 1)
                collider.GetComponent<MeshRenderer>().enabled = true;
        }

        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.useGravity = true;
        rb.mass = pickingUp.Count == 0 ? defaultMass : MASSIVE_MASS;
    }

    public void HandleLiftTapped(PlayerController playerController)
    {
        liftTapped[playerController] = true;
    }

    private void FixedUpdate()
    {
        var previousLift = currentLift;
        
        if (pickingUp.Count > 0 && pickingUp.Count < pickUpPlayersCount)
            currentLift = 0f;
        else
        {
            int liftTappedCount = liftTapped.Count(pair => pair.Value);
            if (liftTappedCount > 0)
            {
                currentLift = Mathf.Min(maxLift, currentLift + perTapLift * liftTappedCount);
                foreach (var player in pickingUp)
                {
                    liftTapped[player] = false;
                }
            }
            else
                currentLift = Mathf.Max(0f, currentLift - liftFallSpeed * Time.fixedDeltaTime);
        }

        if (currentLift != previousLift)
        {
            foreach (var player in pickingUp)
            {
                player.UpdateLift(currentLift);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        AudioSource.PlayClipAtPoint(hitClip, other.contacts[0].point);
    }
}
