using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public Collider[] pickPointsColliders;
    public Rigidbody rb;
    private float drag;
    private float angularDrag;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        drag = rb.drag;
        angularDrag = rb.angularDrag;
    }

    public void HandlePickedUp()
    {
        foreach (var collider in pickPointsColliders)
        {
            collider.enabled = false;
        }

        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.useGravity = false;
    }

    public void HandleDropped()
    {
        foreach (var collider in pickPointsColliders)
        {
            collider.enabled = true;
        }

        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.useGravity = true;
    }
}
