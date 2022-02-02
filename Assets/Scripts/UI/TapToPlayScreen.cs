using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapToPlayScreen : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Text stackUpgradeAmountText;
    [SerializeField] private Text stackUpgradePriceText;
    [SerializeField] private GameObject levels;
    [SerializeField] private Slider levelsSlider;

    public static event Action OnTapToPlay;

    private List<LevelUI> levelUIs;

    private void Awake()
    {
        levelUIs = new List<LevelUI>();
        levelUIs.AddRange(levels.GetComponentsInChildren<LevelUI>());
    }

    private void Start()
    {
        GameManager.Instance.OnCurrentStackUpgradeIndexChange += UpdateUpgradeButton;
    }


    private void OnDestroy()
    {
        GameManager.Instance.OnCurrentStackUpgradeIndexChange -= UpdateUpgradeButton;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnTapToPlay?.Invoke();
    }

    private void UpdateUpgradeButton(StackUpgrade newStackUpgradeData)
    {
        stackUpgradeAmountText.text = string.Concat("+", newStackUpgradeData.UpgradeAmount.ToString());
        stackUpgradePriceText.text = newStackUpgradeData.Price.ToString();
    }

    private void UpdateLevelsBar(int currentLevel)
    {
        int levelsBarStart = currentLevel / 5 * 5 + 1;
        for(int i = 0; i < levelUIs.Count; i++)
        {
            int level = levelsBarStart + i;
            LevelState state = (currentLevel < level) ? LevelState.NotAchieved : ((currentLevel > level) ? LevelState.Achieved : LevelState.Current);
            levelUIs[i].UpdateLevel(level, state);
        }
        levelsSlider.value = (currentLevel - levelsBarStart) * ((float)1 / (levels.transform.childCount - 1));
    }

    public void ToggleVisibility(bool enable)
    {
        gameObject.SetActive(enable);
        if (enable)
        {
            UpdateLevelsBar(GameManager.Instance.CurrentLevel);
        }
    }
}

public enum LevelState
{
    NotAchieved,
    Achieved,
    Current
}