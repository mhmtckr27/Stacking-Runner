using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<StackUpgrade> stackUpgrades;
    [SerializeField] private List<int> platformCounts;
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject finishLinePrefab;

    private List<Platform> currentLevelPlatforms;
    public float levelStartPointZ;
    public float levelEndPointZ;


    private int startStackAmount;
    private bool isLevelStarted;

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

        CurrentLevel = 1;
        currentLevelPlatforms = new List<Platform>();
        SpawnPlatforms();
    }

    private void OnLevelWasLoaded(int level)
    {
        CurrentLevel = level + 1;
        SpawnPlatforms();
    }

    private void Start()
    {
        TapToPlayScreen.OnTapToPlay += StartLevel;
        FinishLine.OnLevelFinished += FinishLevel;
        Invoke(nameof(LateStart), 0.1f);
    }

    private void FinishLevel()
    {
        if (isLevelStarted == true)
        {
            PlayerController.Instance.StartRunning(false);
            isLevelStarted = false;
        }
    }

    private void LateStart()
    {
        GoldAmount = 500;
        CurrentStackUpgradeIndex = 0;
    }

    private void OnDestroy()
    {
        TapToPlayScreen.OnTapToPlay -= StartLevel;
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
        for(int i = 0; i < platformCounts[CurrentLevel]; i++)
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