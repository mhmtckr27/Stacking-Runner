using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldAmountText;

    [SerializeField] private TapToPlayScreen tapToPlayScreen;
    [SerializeField] private GameObject inGameScreen;


    private void Start()
    {
        GameManager.Instance.OnGoldChange += UpgradeGoldText;
        TapToPlayScreen.OnTapToPlay += OnGameStarted;

        tapToPlayScreen.ToggleVisibility(true);
        inGameScreen.SetActive(false);
    }

    private void OnGameStarted()
    {
        tapToPlayScreen.ToggleVisibility(false);
        inGameScreen.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGoldChange -= UpgradeGoldText;
        TapToPlayScreen.OnTapToPlay -= OnGameStarted;
    }

    private void UpgradeGoldText(int newGoldAmount)
    {
        goldAmountText.text = newGoldAmount.ToString();
    }
}
