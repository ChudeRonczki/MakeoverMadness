using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    public static ScoringSystem Instance;
    [SerializeField] private float MaxDistance;
    [SerializeField] private float MaxAngle;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Dodatkowo uzupelnia pola DistanceScore i AngleScore w TargetLocations, moze sie przydac na podsumowanie
    /// </summary>
    public float CalculateScore(List<ScoringComponent> TargetLocations, List<ScoringComponent> PlacedFurniture)
    {
        float TotalScore = 0.0f;
        
        foreach (var Target in TargetLocations)
        {
            float BestScore = 0.0f;
            
            foreach (var Furniture in PlacedFurniture)
            {
                if (Target.ID != Furniture.ID)
                    continue;
                
                float Distance = Target.GetDistance(Furniture);
                float Angle = Target.GetAngle(Furniture);

                if (Distance > MaxDistance || Angle > MaxAngle)
                    continue;

                float DistanceScore = Distance / MaxDistance;
                float AngleScore = Angle / MaxAngle;
                float Score = 1.0f - 0.5f * (DistanceScore + AngleScore);

                if (BestScore < Score)
                {
                    BestScore = Score;
                    Target.DistanceScore = DistanceScore;
                    Target.AngleScore = AngleScore;
                }
            }

            TotalScore += BestScore;
        }

        return TotalScore;
    }
}
