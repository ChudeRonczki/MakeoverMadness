using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int SizeX = 20, SizeZ = 20;
    public int BigAmount = 1, MediumAmount = 1, SmallAmount = 1;
    public GameObject TempCube;
    
    
    private List<ScoringComponent> TargetLocations;
    private int[,] EmptySpaces;

    public List<LayoutComponent> BigLayouts;
    public List<LayoutComponent> MediumLayouts;
    public List<LayoutComponent> SmallLayouts;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel(int seed = 0)
    {
        GameObject[] EmptySpaceObjects = GameObject.FindGameObjectsWithTag("EmptySpace");
        
        EmptySpaces = new int[SizeX, SizeZ];
        for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeZ; j++)
            {
                EmptySpaces[i, j] = 0;
            }
        }

        foreach (var e in EmptySpaceObjects)
        {
            int x = Mathf.RoundToInt(e.transform.position.x);
            int z = Mathf.RoundToInt(e.transform.position.z);
            if (x >= 0 && x < SizeX && z >= 0 && z < SizeZ)
            {
                EmptySpaces[x, z] = 1;
            }
        }
        
        /*for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeY; j++)
            {
                if (EmptySpaces[i, j] == 1)
                {
                    Instantiate(TempCube, new Vector3(i, 0, j), Quaternion.identity, transform);
                }
            }
        }*/
        
        TargetLocations = new List<ScoringComponent>();
        ShuffleLayouts();
        PlaceLayouts();
    }

    private void ShuffleLayouts()
    {
        for (int i = 0; i < BigLayouts.Count; i++)
        {
            var temp = BigLayouts[i];
            int idx = Random.Range(0, BigLayouts.Count);
            BigLayouts[i] = BigLayouts[idx];
            BigLayouts[idx] = temp;
        }
        
        for (int i = 0; i < MediumLayouts.Count; i++)
        {
            var temp = MediumLayouts[i];
            int idx = Random.Range(0, MediumLayouts.Count);
            MediumLayouts[i] = MediumLayouts[idx];
            MediumLayouts[idx] = temp;
        }
        
        for (int i = 0; i < SmallLayouts.Count; i++)
        {
            var temp = SmallLayouts[i];
            int idx = Random.Range(0, SmallLayouts.Count);
            SmallLayouts[i] = SmallLayouts[idx];
            SmallLayouts[idx] = temp;
        }
    }

    private void PlaceLayouts()
    {
        for (int i = 0, placedBig = 0; i < BigLayouts.Count && placedBig < BigAmount; i++)
        {
            var Layout = BigLayouts[i];
            if (CheckLayoutFit(Layout))
            {
                Layout.gameObject.SetActive(true);
                ApplyLayoutBounds(Layout);
                placedBig++;
                
                ScoringComponent[] ScoringComps = Layout.GetComponentsInChildren<ScoringComponent>();
                foreach (var comp in ScoringComps)
                {
                    TargetLocations.Add(comp);
                }
            }
        }
        
        for (int i = 0, placedMedium = 0; i < MediumLayouts.Count && placedMedium < MediumAmount; i++)
        {
            var Layout = MediumLayouts[i];
            if (CheckLayoutFit(Layout))
            {
                Layout.gameObject.SetActive(true);
                ApplyLayoutBounds(Layout);
                placedMedium++;
                
                ScoringComponent[] ScoringComps = Layout.GetComponentsInChildren<ScoringComponent>();
                foreach (var comp in ScoringComps)
                {
                    TargetLocations.Add(comp);
                }
            }
        }
        
        for (int i = 0, placedSmall = 0; i < SmallLayouts.Count && placedSmall < SmallAmount; i++)
        {
            var Layout = SmallLayouts[i];
            if (CheckLayoutFit(Layout))
            {
                Layout.gameObject.SetActive(true);
                ApplyLayoutBounds(Layout);
                placedSmall++;
                
                ScoringComponent[] ScoringComps = Layout.GetComponentsInChildren<ScoringComponent>();
                foreach (var comp in ScoringComps)
                {
                    TargetLocations.Add(comp);
                }
            }
        }
    }
    
    private bool CheckLayoutFit(LayoutComponent Layout)
    {
        for (int x = Layout.FromX; x < Layout.ToX; x++)
        {
            for (int z = Layout.FromZ; z < Layout.ToZ; z++)
            {
                if (EmptySpaces[x, z] != 1)
                    return false;
            }
        }

        return true;
    }

    private void ApplyLayoutBounds(LayoutComponent Layout)
    {
        for (int x = Layout.OccFromX; x <= Layout.OccToX; x++)
        {
            for (int z = Layout.OccFromZ; z <= Layout.OccToZ; z++)
            {
                EmptySpaces[x, z] = 0;
            }
        }
    }
}
