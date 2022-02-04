using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameScreen : ScreenBase
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private LevelUI levelUI;

    private void Start()
    {
        TapToPlayScreen.OnTapToPlay += OnLevelStart;
    }

    private void OnDestroy()
    {
        TapToPlayScreen.OnTapToPlay -= OnLevelStart;
    }

    private void OnLevelStart()
    {
        levelUI.UpdateLevelUI(GameManager.Instance.CurrentLevel);
        InvokeRepeating(nameof(UpdateProgressBar), 0, 0.02f);
    }

    private void UpdateProgressBar()
    {
        progressBar.value = PlayerController.Instance.transform.position.z / (GameManager.Instance.levelEndPointZ - GameManager.Instance.levelStartPointZ);
    }
}
