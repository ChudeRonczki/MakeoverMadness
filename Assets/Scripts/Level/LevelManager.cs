using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int GameLengthSeconds = 60;
    public int SizeX = 20, SizeZ = 20;
    public int BigAmount = 1, MediumAmount = 1, SmallAmount = 1;
    public RenderTexture CameraRenderTexture;
    public TMP_Text BigText;
    public TMP_Text CountdownText;
    
    private List<ScoringComponent> TargetLocations = new List<ScoringComponent>();
    private List<ScoringComponent> FurnitureLocations = new List<ScoringComponent>();
    private int[,] LevelGrid;

    public Vector2 EmptyFrom;
    public Vector2 EmptyTo;
    
    public Vector2 StartingFrom;
    public Vector2 StartingTo;

    public List<LayoutComponent> BigLayouts;
    public List<LayoutComponent> MediumLayouts;
    public List<LayoutComponent> SmallLayouts;
    public List<RoomConfig> Rooms;
    private PlayerController[] Players;
    [SerializeField] private bool UseOldSystem;
    [SerializeField] private float RoomStayChance = 0.66f;
    [SerializeField] private string levelId;
    [SerializeField] private ParticleSystem StarsParticles;
    
    private char[] ScoreName = new[] {'_', '_', '_'};

    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private AudioClip tickTockClip;
    [SerializeField] private AudioClip endClip;
    [SerializeField] private AudioClip typeClip;

    [SerializeField] private Material[] FurnitureTextures;
    private int[] MaterialIndexes = new int[18];
    private bool MaterialsInitialized = false;
    private int RoomsStayed = 0;
    private int RoomsProcessed = 0;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AudioSource.PlayClipAtPoint(levelMusic, Vector3.zero);
        
        CachePlayers();
        
        GenerateLevel();

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        EnableInput(false);

        float time = 3.9f;

        while (time > 1f)
        {
            time -= Time.deltaTime;
            int number = (int)time;

            if (number > 0)
            {
                BigText.text = "" + number;
                BigText.transform.localScale = Vector3.one * (time % 1f);
            }

            yield return null;
        }
        
        BigText.text = "GO!";
        BigText.transform.localScale = Vector3.one;
        EnableInput(true);
        
        while (time > 0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        
        BigText.text = "";

        AudioSource tickTockComponent = null;
        int seconds = GameLengthSeconds;
        while (seconds > 0)
        {
            CountdownText.text = "" + seconds;
            seconds--;

            if (seconds <= 10 && tickTockComponent == null)
            {
                tickTockComponent = gameObject.AddComponent<AudioSource>();
                tickTockComponent.clip = tickTockClip;
                tickTockComponent.loop = true;
                tickTockComponent.volume = .05f;
                tickTockComponent.Play();
            }
            
            yield return new WaitForSeconds(1f);
        }
        
        Destroy(tickTockComponent);
        AudioSource.PlayClipAtPoint(endClip, Vector3.zero);
        
        EnableInput(false);
        CountdownText.text = "";

        int percent = Mathf.RoundToInt(100f * ScoringSystem.Instance.CalculateScore(TargetLocations, FurnitureLocations));

        StartCoroutine(DrawStars());

        for (float t = 0f; t < 5f; t += Time.deltaTime)
        {
            float perc = t / 5f;
            int i = Mathf.RoundToInt(Mathf.Sqrt(perc) * percent - 1);

            BigText.text = "TIME'S UP!\n\nSCORE: " + i + "%";
            
            yield return null;
        }
        
        yield return new WaitForSeconds(0.5f);
        
        BigText.text = "TIME'S UP!\n\nSCORE: " + percent + "%";
        
        yield return new WaitForSeconds(3f);

        var highscore = new Highscore(levelId);

        if (highscore.ShouldAdd(percent))
        {
            yield return EnterNameRoutine();
            highscore.Add(new Score { Name = "" + ScoreName[0] + ScoreName[1] + ScoreName[2], Percent = percent});
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("HIGH SCORES:");
        foreach (var score in highscore.scores)
        {
            sb.AppendLine($"{score.Name} {score.Percent}");
        }
        BigText.text = sb.ToString();

        yield return new WaitForSeconds(.2f);
        
        while (true)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene("MainMenu");
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator DrawStars()
    {
        foreach (var scoringComp in TargetLocations)
        {
            if (scoringComp.Score > 0.1f)
            {
                var vfx = Instantiate<ParticleSystem>(StarsParticles, transform);
                vfx.transform.position = scoringComp.transform.position + Vector3.up;
                vfx.transform.localScale = Vector3.one;
                
                yield return new WaitForSeconds(0.2f);
            }
        }

        yield return null;
    }
    
    private IEnumerator EnterNameRoutine()
    {
        bool refresh = true;
        int filledIn = 0;
        while (true)
        {
            if (refresh)
            {
                BigText.text = "ENTER YOUR INITIALS:\n" + ScoreName[0] + ScoreName[1] + ScoreName[2];
                refresh = false;
            }
            
            
            
            if (Input.GetKeyDown(KeyCode.Return) && filledIn == 3)
                yield break;

            if (Input.GetKeyDown(KeyCode.Backspace) && filledIn > 0)
            {
                ScoreName[--filledIn] = '_';
                refresh = true;
                AudioSource.PlayClipAtPoint(typeClip, Vector3.zero);
            }

            if (filledIn < 3)
            {
                for (int i = (int) KeyCode.A; i <= (int) KeyCode.Z; ++i)
                {
                    if (Input.GetKeyDown((KeyCode) i))
                    {
                        ScoreName[filledIn++] = ((KeyCode) i).ToString()[0];
                        refresh = true;
                        AudioSource.PlayClipAtPoint(typeClip, Vector3.zero);
                        if (filledIn == 3)
                            break;
                    }
                }
            }

            yield return null;
        }
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            string result = "Score: " + ScoringSystem.Instance.CalculateScore(TargetLocations, FurnitureLocations);
            foreach (var location in TargetLocations)
            {
                result += "\n" + location.gameObject.name + ": ds=" + location.DistanceScore + " as=" + location.AngleScore + " total=" + location.Score;
            }
            Debug.Log(result);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
        {
            Highscore.Clear();
        }
    }

    public void CachePlayers()
    {
        Players = FindObjectsOfType<PlayerController>();
    }

    public void EnableInput(bool Enabled)
    {
        foreach (var player in Players)
        {
            player.enabled = Enabled;
        }
    }
    
    public void GenerateLevel(int seed = 0)
    {
        if (FurnitureTextures.Length > 0)
        {
            for (int i = 0; i < MaterialIndexes.Length; i++)
            {
                MaterialIndexes[i] = Random.Range(0, FurnitureTextures.Length);
            }
            MaterialsInitialized = true;
        }
        
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

        for (int x = (int)EmptyFrom.x; x < (int)EmptyTo.x; x++)
        {
            for (int z = (int)EmptyFrom.y; z < (int)EmptyTo.y; z++)
            {
                LevelGrid[x, z] = 1;
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
        
        for (int x = (int)StartingFrom.x; x < (int)StartingTo.x; x++)
        {
            for (int z = (int)StartingFrom.y; z < (int)StartingTo.y; z++)
            {
                LevelGrid[x, z] = 2;
            }
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

        if (UseOldSystem)
        {
            ShuffleLayouts();
            PlaceLayouts();
        }
        else
        {
            ApplyRooms();
        }
        
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

    private void ApplyRooms()
    {
        foreach (var room in Rooms)
        {
            var Variants = room.Variants.ToArray();
            ApplyRandomRoomVariant(Variants);
        }
    }
    
    private void ApplyRandomRoomVariant(GameObject[] RoomVariants)
    {
        var Room = RoomVariants[Random.Range(0, RoomVariants.Length)];
        Room.SetActive(true);
        var ScoringComps2 = DuplicateRoom(Room);
        
        ScoringComponent[] ScoringComps = Room.GetComponentsInChildren<ScoringComponent>();
        foreach (var comp in ScoringComps)
        {
            TargetLocations.Add(comp);
            var Pickable = comp.GetComponent<Pickable>();
            foreach (var pickPointsCollider in Pickable.pickPointsColliders)
            {
                pickPointsCollider.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        
        foreach (var comp in ScoringComps2)
        {
            FurnitureLocations.Add(comp);
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

    private ScoringComponent[] DuplicateRoom(GameObject room)
    {
        var children = room.GetComponentsInChildren<ScoringComponent>();
        ScoringComponent[] scorings = new ScoringComponent[children.Length];
        for(int s = 0; s < children.Length; s++)
        {
            var child = children[s];
            RoomsProcessed++;

            if (MaterialsInitialized)
            {
                var mr = child.GetComponent<MeshRenderer>();
                var mats = mr.materials;
                for (int m = 0; m < mats.Length; m++)
                {
                    mats[m] = FurnitureTextures[MaterialIndexes[child.ID]];
                }
                mr.materials = mats;
            }
            
            var Duplicate = Instantiate(child.gameObject, Vector3.zero, Quaternion.identity);
            Duplicate.transform.localScale = Vector3.one;
            scorings[s] = Duplicate.GetComponent<ScoringComponent>();

            float roomsRatio = RoomsStayed / (float)RoomsProcessed;
            bool addNextRoom = true;
            
            if (roomsRatio / RoomStayChance > 1.2f)    // too many stayed
            {
                addNextRoom = false;
            }
            else if (roomsRatio / RoomStayChance < 0.8f)    // too little stayed
            {
                addNextRoom = true;
            }
            else if (Random.value < RoomStayChance)    // about right, so random
            {
                addNextRoom = false;
            }
            
            if (addNextRoom)
            {
                Duplicate.transform.position = child.transform.position;
                Duplicate.transform.rotation = child.transform.rotation;
                RoomsStayed++;
            }
            else
            {
                bool found = false;
                
                for (int i = 0; i < SizeX; i++)
                {
                    for (int j = 0; j < SizeZ; j++)
                    {
                        if (CanPlaceDuplicate(i, j,
                            Mathf.RoundToInt(child.GridSize.x),
                            Mathf.RoundToInt(child.GridSize.y)))
                        {
                            Vector2 offset = (child.GridSize + Vector2.one) * 0.5f;
                        
                            Duplicate.transform.position = new Vector3(i + offset.x, 0f, j + offset.y);
                            i = SizeX;
                            j = SizeZ;
                            found = true;
                        }
                    }
                }

                if (!found)
                {
                    Duplicate.transform.position = child.transform.position;
                    Duplicate.transform.rotation = child.transform.rotation;
                    RoomsStayed++;
                }
            }
        }

        return scorings;
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
        foreach (var player in Players)
        {
            player.SkinnedRenderer.enabled = false;
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
        foreach (var player in Players)
        {
            player.SkinnedRenderer.enabled = true;
        }
    }
}

internal struct Score
{
    public string Name { get; set; }
    public int Percent { get; set; }
}

internal class Highscore
{   
    private const string HIGH_SCORE_PREFIX = "HS";
    private const int CAPACITY = 5;
    private string levelId;
    
    public List<Score> scores = new List<Score>();

    public Highscore(string _levelId)
    {
        levelId = _levelId;
        var serializedScores = PlayerPrefs.GetString(HIGH_SCORE_PREFIX + levelId);
        var splitScores = serializedScores.Split(';');

        for (int i = 0; i + 1 < splitScores.Length; i += 2)
        {
            scores.Add(new Score {Name = splitScores[i], Percent = int.Parse(splitScores[i + 1])});
        }
    }

    public bool ShouldAdd(int percent)
    {
        if (scores.Count < CAPACITY)
            return true;

        return scores.Select(score => score.Percent).Min() < percent;
    }

    public void Add(Score score)
    {
        scores.Add(score);
        scores.Sort((score1, score2) => score2.Percent.CompareTo(score1.Percent));
        
        while (scores.Count > CAPACITY)
            scores.RemoveAt(scores.Count - 1);
        
        Save();
    }

    public void Save()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var score in scores)
        {
            sb.Append($"{score.Name};{score.Percent};");
        }
        
        PlayerPrefs.SetString(HIGH_SCORE_PREFIX + levelId, sb.ToString());
        PlayerPrefs.Save();
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
