using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> dontDestroyOnLoadObjects;
    [SerializeField] private List<StackUpgrade> stackUpgrades;
    [SerializeField] private List<int> platformCounts;
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject nonCollidingPlatformPrefab;
    [SerializeField] private GameObject finishLinePrefab;
    [SerializeField] private int currentLevelStartValueForDebugDeleteLater;

    private List<Platform> currentLevelPlatforms;
    public float levelStartPointZ;
    public float levelEndPointZ;

    private int highScore;
    private int finalScoreThisLevel;
    private int startStackAmount;
    private bool isLevelStarted;
    public int collectedGoldThisLevel;
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

    public event Action<StackUpgrade> OnCurrentStackUpgradeIndexChange;
    public event Action<int> OnGoldChange;
    public event Action<bool> IsNewHighScore;

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

        CurrentLevel = currentLevelStartValueForDebugDeleteLater;
        currentLevelPlatforms = new List<Platform>();
        SpawnPlatforms();
    }

    private void OnLevelWasLoaded(int level)
    {
        CurrentLevel = level + 1;
        collectedGoldThisLevel = 0;
        collectedDiamondThisLevel = startStackAmount;
        PlayerController.Instance.transform.position = Vector3.zero;
        SpawnPlatforms();
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
        Invoke(nameof(LateStart), 0.1f);
    }

    private void OnDestroy()
    {
        TapToPlayScreen.OnTapToPlay -= StartLevel;
        FinishLine.OnFinishLine -= StopLevel;
        LevelEndScreen.Instance.OnTapToContinue -= LevelEndScreen_OnTapToContinue;
    }

    private void LevelEndScreen_OnTapToContinue()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void StopLevel()
    {
        if (isLevelStarted == true)
        {
            PlayerController.Instance.StartRunning(false);
            isLevelStarted = false;
            GoldAmount += collectedGoldThisLevel;

            //TODO: add a score bonus multiplier or sth idk.
            finalScoreThisLevel = collectedDiamondThisLevel * 1;
            IsNewHighScore?.Invoke(highScore < finalScoreThisLevel);
            if (highScore < finalScoreThisLevel)
            {
                highScore = finalScoreThisLevel;
            }
        }
    }

    private void LateStart()
    {
        GoldAmount = 500;
        CurrentStackUpgradeIndex = 0;
    }

    private void StartLevel()
    {
        if (isLevelStarted == false)
        {
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
        }

        if(CurrentStackUpgradeIndex < stackUpgrades.Count - 1)
        {
            CurrentStackUpgradeIndex++;
        }
    }

    private void SpawnPlatforms()
    {
        currentLevelPlatforms.Clear();
        BoxCollider platformCollider = platformPrefab.GetComponentInChildren<BoxCollider>();

        Instantiate(nonCollidingPlatformPrefab, new Vector3(0, 0, -1 * platformCollider.size.z), Quaternion.identity);

        int platformCount = (platformCounts.Count > CurrentLevel) ? platformCounts[CurrentLevel] : 1; 

        for(int i = 0; i < platformCount; i++)
        {
            currentLevelPlatforms.Add(Instantiate(platformPrefab, new Vector3(0, 0, i * platformCollider.size.z), Quaternion.identity).GetComponent<Platform>());
        }
        
        levelStartPointZ = currentLevelPlatforms[0].boxCollider.bounds.min.z;
        levelEndPointZ = currentLevelPlatforms[currentLevelPlatforms.Count - 1].boxCollider.bounds.max.z;
        Vector3 finishLinePos = currentLevelPlatforms[currentLevelPlatforms.Count - 1].transform.position;
        finishLinePos.z = levelEndPointZ;
        Instantiate(finishLinePrefab, finishLinePos, Quaternion.identity);
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