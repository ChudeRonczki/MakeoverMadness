using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringComponent : MonoBehaviour
{
    public enum SymmetryType
    {
        None = 0,
        OneAxis,
        TwoAxis,
        IgnoreRotation
    }
    
    public int ID;
    public SymmetryType Symmetry;

    public float DistanceScore;
    public float AngleScore;
    public float Score => 0.5f * (DistanceScore + AngleScore);
    
    public float GetDistance(ScoringComponent Other)
    {
        return Vector3.Distance(transform.position, Other.transform.position);
    }

    public float GetAngle(ScoringComponent Other)
    {
        float angle = Vector3.Angle(transform.forward, Other.transform.forward);

        if (Symmetry == SymmetryType.OneAxis)
        {
            angle = Mathf.Min(angle, 180f - angle);
        }
        else if (Symmetry == SymmetryType.TwoAxis)
        {
            angle = Mathf.Min(angle, 180f - angle, Mathf.Abs(90f - angle));
        }
        else if (Symmetry == SymmetryType.IgnoreRotation)
        {
            angle = 0f;
        }

        return angle;
    }
}