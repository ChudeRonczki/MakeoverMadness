using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableDetector : MonoBehaviour
{
    public PlayerController playerController;

    private List<Collider> pickPoints;

    private void Awake()
    {
        pickPoints = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        pickPoints.Add(other);
        RefreshOrder();
        Debug.Log($"Pickable: {other.gameObject.ToString()}");
    }

    private void OnTriggerExit(Collider other)
    {
        pickPoints.Remove(other);
        RefreshOrder();
        Debug.Log($"Nulled pickable: {other.gameObject.ToString()}");
    }

    public void RefreshOrder()
    {
        pickPoints.Sort((a, b) =>
            (a.transform.position - playerController.transform.position).sqrMagnitude.CompareTo(
                (b.transform.position - playerController.transform.position).sqrMagnitude)
        );
    }

    public Collider PickupPoint => pickPoints.Count > 0 ? pickPoints[0] : null;
    public Pickable ObjectToPick => pickPoints.Count > 0 ? pickPoints[0].GetComponentInParent<Pickable>() : null;

    public void Clear()
    {
        pickPoints.Clear();
    }
}
