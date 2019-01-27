using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRandomizer : MonoBehaviour
{
    public Material[] materialVariants;

    void Awake()
    {
        GetComponent<MeshRenderer>().material = materialVariants[Random.Range(0, materialVariants.Length)];
    }
}