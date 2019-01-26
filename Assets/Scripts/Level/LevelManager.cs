using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int SizeX = 20, SizeZ = 20;
    public int BigAmount = 1, MediumAmount = 1, SmallAmount = 1;
    public RenderTexture CameraRenderTexture;
    
    
    private List<ScoringComponent> TargetLocations = new List<ScoringComponent>();
    private List<ScoringComponent> FurnitureLocations = new List<ScoringComponent>();
    private int[,] LevelGrid;

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
        GameObject[] LayoutSpaceObjects = GameObject.FindGameObjectsWithTag("EmptySpace");
        GameObject[] StartingSpaceObjects = GameObject.FindGameObjectsWithTag("StartingSpace");
        
        LevelGrid = new int[SizeX, SizeZ];
        LevelGrid = new int[SizeX, SizeZ];
        for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeZ; j++)
            {
                LevelGrid[i, j] = 0;
            }
        }

        foreach (var e in LayoutSpaceObjects)
        {
            int x = Mathf.RoundToInt(e.transform.position.x);
            int z = Mathf.RoundToInt(e.transform.position.z);
            if (x >= 0 && x < SizeX && z >= 0 && z < SizeZ)
            {
                LevelGrid[x, z] = 1;
            }
            
            e.SetActive(false);
        }
        
        foreach (var e in StartingSpaceObjects)
        {
            int x = Mathf.RoundToInt(e.transform.position.x);
            int z = Mathf.RoundToInt(e.transform.position.z);
            if (x >= 0 && x < SizeX && z >= 0 && z < SizeZ)
            {
                LevelGrid[x, z] = 2;
            }
            
            e.SetActive(false);
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
        
        ShuffleLayouts();
        PlaceLayouts();
        PrepareRenderTexture();

        /*string s = "";
        for (int i = 0; i < SizeZ; i++)
        {
            for (int j = 0; j < SizeX; j++)
            {
                s += LevelGrid[j,i].ToString();
            }
            s += "\n";
        }
        Debug.Log(s);*/
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
            if (TryApplyLayout(BigLayouts[i]))
            {
                placedBig++;
            }
        }
        
        for (int i = 0, placedMedium = 0; i < MediumLayouts.Count && placedMedium < MediumAmount; i++)
        {
            if (TryApplyLayout(MediumLayouts[i]))
            {
                placedMedium++;
            }
        }
        
        for (int i = 0, placedSmall = 0; i < SmallLayouts.Count && placedSmall < SmallAmount; i++)
        {
            if (TryApplyLayout(SmallLayouts[i]))
            {
                placedSmall++;
            }
        }
    }

    private bool TryApplyLayout(LayoutComponent Layout)
    {
        if (CheckLayoutFit(Layout, 1))
        {
            Layout.gameObject.SetActive(true);
            ApplyLayoutBounds(Layout);
            var Duplicate = DuplicateLayout(Layout);    
            
            ScoringComponent[] ScoringComps = Layout.GetComponentsInChildren<ScoringComponent>();
            foreach (var comp in ScoringComps)
            {
                TargetLocations.Add(comp);
                var Pickable = comp.GetComponent<Pickable>();
                foreach (var pickPointsCollider in Pickable.pickPointsColliders)
                {
                    pickPointsCollider.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            
            var ScoringComps2 = Duplicate.GetComponentsInChildren<ScoringComponent>();
            foreach (var comp in ScoringComps2)
            {
                FurnitureLocations.Add(comp);
            }

            return true;
        }
        
        return false;
    }
    
    private bool CheckLayoutFit(LayoutComponent Layout, int GridLayer)
    {
        for (int x = Layout.FromX; x < Layout.ToX; x++)
        {
            for (int z = Layout.FromZ; z < Layout.ToZ; z++)
            {
                if (LevelGrid[x, z] != GridLayer)
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
                LevelGrid[x, z] = 0;
            }
        }
    }

    private LayoutComponent DuplicateLayout(LayoutComponent Layout)
    {
        var Duplicate = Instantiate(Layout.gameObject, Vector3.zero, Quaternion.identity);
        Duplicate.transform.localScale = Vector3.one;
        var Comp = Duplicate.GetComponent<LayoutComponent>();
        
        for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeZ; j++)
            {
                if (CanPlaceDuplicate(i, j,
                    Mathf.RoundToInt(Comp.RequiredSpace.x),
                    Mathf.RoundToInt(Comp.RequiredSpace.y)))
                {
                    Duplicate.transform.position = new Vector3(i, 0f, j);
                    return Comp;
                }
            }
        }

        return Comp;
    }

    private bool CanPlaceDuplicate(int x, int z, int rx, int rz)
    {
        for (int i = x; i < x + rx; i++)
        {
            for (int j = z; j < z + rz; j++)
            {
                if (LevelGrid[i, j] != 2)
                    return false;
            }
        }
        
        for (int i = x; i < x + rx; i++)
        {
            for (int j = z; j < z + rz; j++)
            {
                LevelGrid[i, j] = 0;
            }
        }

        return true;
    }

    void PrepareRenderTexture()
    {
        foreach (var furnitureLocations in FurnitureLocations)
        {
            furnitureLocations.gameObject.SetActive(false);
        }
        
        Camera.main.targetTexture = CameraRenderTexture;
        Camera.main.Render();
        Camera.main.targetTexture = null;

        foreach (var targetLocation in TargetLocations)
        {
            targetLocation.gameObject.SetActive(false);
        }
        foreach (var furnitureLocations in FurnitureLocations)
        {
            furnitureLocations.gameObject.SetActive(true);
        }
    }
}
