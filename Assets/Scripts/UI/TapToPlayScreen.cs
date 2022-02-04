using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapToPlayScreen : ScreenBase, IPointerDownHandler
{
    [SerializeField] private Text stackUpgradeAmountText;
    [SerializeField] private Text stackUpgradePriceText;

    public static event Action OnTapToPlay;

    protected override void Start()
    {
        base.Start();
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

    public override void ToggleVisibility(bool enable)
    {
        gameObject.SetActive(enable);
        if (enable)
        {
            //UpdateLevelsBar(GameManager.Instance.CurrentLevel);
        }
    }
}

public enum LevelState
{
    NotAchieved,
    Achieved,
    Current
}