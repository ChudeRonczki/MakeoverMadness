using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableDetector : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        playerController.objectToPickUp = other.transform.parent.GetComponent<Pickable>();
        Debug.Log($"Pickable: {other.gameObject.ToString()}");
    }

    private void OnTriggerExit(Collider other)
    {
        var pickable = other.transform.parent.GetComponent<Pickable>();
        if (playerController.objectToPickUp == pickable)
        {
            playerController.objectToPickUp = null;
            Debug.Log($"Nulled pickable: {other.gameObject.ToString()}");
        }
    }
}
