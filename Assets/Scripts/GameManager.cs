using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string jsonFileName;
    [SerializeField] private List<GameObject> dontDestroyOnLoadObjects;
    [SerializeField] private List<StackUpgrade> stackUpgrades;
    [SerializeField] private List<int> platformCounts;

    [Header("PREFABS")] [Space]
    [SerializeField] private List<string> prefabNames;
    [SerializeField] private List<GameObject> prefabs;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip upgradeSound;
    [SerializeField] public int maxStackLimit;

    private List<Platform> currentLevelPlatforms;
    public float levelStartPointZ;
    public float levelEndPointZ;

    private int highScore;
    private int finalScoreThisLevel;
    private int startStackAmount;
    private bool isLevelStarted;
    private int collectedDiamondThisLevel;
    public int CurrentLevel { get; private set; }

    private int currentStackUpgradeIndex;
    public int CurrentStackUpgradeIndex
    {
        get => currentStackUpgradeIndex;
        set
        {
            currentStackUpgradeIndex = value;
            OnCurrentStackUpgradeIndexChange?.Invoke(stackUpgrades[currentStackUpgradeIndex]);
        }
    }

    private int goldAmount;
    public int GoldAmount
    {
        get => goldAmount;
        set
        {
            goldAmount = value;
            OnGoldChange?.Invoke(goldAmount);
        }
    }

    private Dictionary<string, GameObject> spawnablePrefabs;

    public event Action<StackUpgrade> OnCurrentStackUpgradeIndexChange;
    public event Action<int> OnGoldChange;
    public event Action<bool, int, int> IsNewHighScore;
    public event Action<int> OnCollectedDiamondChange;

    private static GameManager instance;
    public static GameManager Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        spawnablePrefabs = new Dictionary<string, GameObject>();
        for (int i = 0; i < prefabNames.Count; i++)
        {
            spawnablePrefabs.Add(prefabNames[i], prefabs[i]);
        }

    }

    private void OnCollectedItem(CollectibleType collectedType, int amountToAdd, bool playVFX)
    {
        switch (collectedType)
        {
            case CollectibleType.Gold:
                GoldAmount += amountToAdd;
                if (playVFX)
                {
                    PlayerController.Instance.plusOneVFX.Play();
                }
                break;
            case CollectibleType.Obstacle:
                collectedDiamondThisLevel = Mathf.Clamp(collectedDiamondThisLevel - amountToAdd, 0, maxStackLimit);
                PlayerController.Instance.animator.SetBool("IsStackEmpty", collectedDiamondThisLevel == 0);
                break;
            case CollectibleType.Diamond:
                collectedDiamondThisLevel = Mathf.Clamp(collectedDiamondThisLevel + amountToAdd, 0, maxStackLimit);
                PlayerController.Instance.animator.SetBool("IsStackEmpty", collectedDiamondThisLevel == 0);
                if (playVFX)
                {
                    PlayerController.Instance.plusOneVFX.Play();
                }
                break;
        }
        OnCollectedDiamondChange?.Invoke(collectedDiamondThisLevel);
    }

    private void OnLevelWasLoaded(int level)
    {
        collectedDiamondThisLevel = 0;
        PlayerController.Instance.transform.position = Vector3.zero;
        SpawnPlatforms();
        SpawnCollectablesFromJSON();
    }

    private void Start()
    {
        foreach (GameObject gameObject in dontDestroyOnLoadObjects)
        {
            DontDestroyOnLoad(gameObject);
        }

        TapToPlayScreen.OnTapToPlay += StartLevel;
        FinishLine.OnFinishLine += StopLevel;
        LevelEndScreen.Instance.OnTapToContinue += LevelEndScreen_OnTapToContinue;
        Diamond.OnCollected += OnCollectedItem;

        LoadGameData();
    }

    private void OnDestroy()
    {
        TapToPlayScreen.OnTapToPlay -= StartLevel;
        FinishLine.OnFinishLine -= StopLevel;
        LevelEndScreen.Instance.OnTapToContinue -= LevelEndScreen_OnTapToContinue;
        Diamond.OnCollected -= OnCollectedItem;
    }

    private void LevelEndScreen_OnTapToContinue()
    {
        CurrentLevel++;
        int sceneToLoad = (SceneManager.GetActiveScene().buildIndex + 1) == SceneManager.sceneCountInBuildSettings ? 1 : (SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    private void StopLevel()
    {
        if (isLevelStarted == true)
        {
            PlayerController.Instance.StartRunning(false);
            isLevelStarted = false;

            //TODO: add a score bonus multiplier or sth idk.
            finalScoreThisLevel = collectedDiamondThisLevel * 1;
            goldAmount += finalScoreThisLevel;
            IsNewHighScore?.Invoke(highScore < finalScoreThisLevel, finalScoreThisLevel, highScore);
            if (highScore < finalScoreThisLevel)
            {
                highScore = finalScoreThisLevel;
            }
        }
    }

    private void StartLevel()
    {
        if (isLevelStarted == false)
        {
            OnCollectedItem(CollectibleType.Diamond, startStackAmount, false);
            PlayerController.Instance.StartRunning(true);
            isLevelStarted = true;
        }
    }

    public void IncreaseStartStack()
    {
        if(goldAmount >= stackUpgrades[currentStackUpgradeIndex].Price)
        {
            startStackAmount += stackUpgrades[currentStackUpgradeIndex].UpgradeAmount;
            GoldAmount -= stackUpgrades[currentStackUpgradeIndex].Price;
            CurrentStackUpgradeIndex = Mathf.Clamp(CurrentStackUpgradeIndex + 1, 0, stackUpgrades.Count - 1);
            audioSource.clip = upgradeSound;
            audioSource.Play();
        }
    }

    private void SpawnPlatforms()
    {
        currentLevelPlatforms = new List<Platform>();
        BoxCollider platformCollider = spawnablePrefabs["Platform"].GetComponentInChildren<BoxCollider>();

        Instantiate(spawnablePrefabs["NonCollidingPlatform"], new Vector3(0, 0, -1 * platformCollider.size.z), Quaternion.identity);

        int platformCount = platformCounts[SceneManager.GetActiveScene().buildIndex]; 

        for(int i = 0; i < platformCount; i++)
        {
            currentLevelPlatforms.Add(Instantiate(spawnablePrefabs["Platform"], new Vector3(0, 0, i * platformCollider.size.z), Quaternion.identity).GetComponent<Platform>());
        }


        levelStartPointZ = currentLevelPlatforms[0].boxCollider.bounds.min.z;
        levelEndPointZ = currentLevelPlatforms[currentLevelPlatforms.Count - 1].boxCollider.bounds.max.z;
        Instantiate(spawnablePrefabs["Platform"], new Vector3(0, 0, levelEndPointZ), Quaternion.identity);
        Vector3 finishLinePos = currentLevelPlatforms[currentLevelPlatforms.Count - 1].transform.position;
        finishLinePos.z = levelEndPointZ;
        Instantiate(spawnablePrefabs["FinishLine"], finishLinePos, Quaternion.identity);
    }
    void AccessData(JSONObject jsonObject)
    {
        switch (jsonObject.type)
        {
            case JSONObject.Type.Object:
                for (var i = 0; i < jsonObject.list.Count; i++)
                {
                    var key = jsonObject.keys[i];
                    var value = jsonObject.list[i];
                    Debug.Log(key);
                    AccessData(value);
                }
                break;
            case JSONObject.Type.Array:
                foreach (JSONObject element in jsonObject.list)
                {
                    AccessData(element);
                }
                break;
            case JSONObject.Type.String:
                Debug.Log(jsonObject.stringValue);
                break;
            case JSONObject.Type.Number:
                Debug.Log(jsonObject.floatValue);
                break;
            case JSONObject.Type.Bool:
                Debug.Log(jsonObject.boolValue);
                break;
            case JSONObject.Type.Null:
                Debug.Log("Null");
                break;
            case JSONObject.Type.Baked:
                Debug.Log(jsonObject.stringValue);
                break;
        }
    }

    public void SpawnCollectablesFromJSON()
    {
        string jsonStr = Resources.Load<TextAsset>(jsonFileName).text;
        JSONObject jSONObject = new JSONObject(jsonStr);


        float columnStartPos = currentLevelPlatforms[1].GetComponentInChildren<BoxCollider>().bounds.min.x + 0.25f;
        float rowStartPos = currentLevelPlatforms[1].GetComponentInChildren<BoxCollider>().bounds.min.z + 0.25f;
        float columnEndPos = currentLevelPlatforms[currentLevelPlatforms.Count - 2].GetComponentInChildren<BoxCollider>().bounds.max.x - 0.25f;
        float rowEndPos = currentLevelPlatforms[currentLevelPlatforms.Count - 2].GetComponentInChildren<BoxCollider>().bounds.max.z - 0.25f;
        int rowCount = jSONObject.list[SceneManager.GetActiveScene().buildIndex].count;
        int columnCount = jSONObject.list[SceneManager.GetActiveScene().buildIndex][0].count;

        int[,] jsonMatrix = new int[rowCount, columnCount];

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                jsonMatrix[i, j] = jSONObject.list[SceneManager.GetActiveScene().buildIndex][i][j].intValue;
            }
        }

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                if(jsonMatrix[i, j] == 0)
                {
                    continue;
                }
                Vector3 spawnPos = new Vector3();
                spawnPos.x = Mathf.Lerp(columnStartPos, columnEndPos, (float)j / (columnCount - 1));
                spawnPos.y = 1.25f;
                spawnPos.z = Mathf.Lerp(rowStartPos, rowEndPos, (float)i / (rowCount - 1));
                Instantiate(spawnablePrefabs[((CollectibleType)jsonMatrix[i, j]).ToString()], spawnPos, Quaternion.identity);
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void SaveGameData()
    {
        PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
        PlayerPrefs.SetInt("GoldAmount", GoldAmount);
        PlayerPrefs.SetInt("StartStackAmount", startStackAmount);
        PlayerPrefs.SetInt("CurrentUpgradeIndex", CurrentStackUpgradeIndex);
        PlayerPrefs.SetInt("HighScore", highScore);
    }

    private void LoadGameData()
    {
        CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        GoldAmount = PlayerPrefs.GetInt("GoldAmount", 0);
        startStackAmount = PlayerPrefs.GetInt("StartStackAmount", 0);
        CurrentStackUpgradeIndex = PlayerPrefs.GetInt("CurrentUpgradeIndex", 0);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
}

[System.Serializable]
public struct StackUpgrade
{
    public int UpgradeAmount;
    public int Price;
}


public struct SpawnablePrefabs
{
    public string prefabName;
    public GameObject prefab;
}

public class SpawnMatrixClass
{
    public int[][] _matrix;
}