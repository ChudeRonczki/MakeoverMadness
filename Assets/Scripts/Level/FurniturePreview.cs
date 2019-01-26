using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurniturePreview : MonoBehaviour
{
    public static FurniturePreview Instance;
    
    public GameObject PreviewObject;

    void Awake()
    {
        Instance = this;
    }

    public static void ShowPreview(bool show)
    {
        Instance.PreviewObject.SetActive(show);
    }
}
