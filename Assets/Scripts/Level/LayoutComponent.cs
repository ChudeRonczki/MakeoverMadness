using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutComponent : MonoBehaviour
{
    public Vector2 RequiredSpace = new Vector2(1, 1);
    public Vector2 OccupiedX = new Vector2(1, 1);
    public Vector2 OccupiedZ = new Vector2(1, 1);

    public int FromX => Mathf.RoundToInt(transform.position.x);
    public int FromZ => Mathf.RoundToInt(transform.position.z);
    public int ToX => Mathf.RoundToInt(transform.position.x + RequiredSpace.x);
    public int ToZ => Mathf.RoundToInt(transform.position.z + RequiredSpace.y);
    
    public int OccFromX => Mathf.RoundToInt(transform.position.x + OccupiedX.x);
    public int OccFromZ => Mathf.RoundToInt(transform.position.z + OccupiedZ.x);
    public int OccToX => Mathf.RoundToInt(transform.position.x + OccupiedX.y);
    public int OccToZ => Mathf.RoundToInt(transform.position.z + OccupiedZ.y);

}
