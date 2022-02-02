using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private void Start()
    {
        TapToPlayScreen.OnTapToPlay += OnGameStart;
    }

    private void OnDestroy()
    {
        TapToPlayScreen.OnTapToPlay -= OnGameStart;
    }

    private void OnGameStart()
    {
        InvokeRepeating(nameof(UpdateProgressBar), 0, 0.1f);
    }

    private void UpdateProgressBar()
    {
        progressBar.value = PlayerController.Instance.transform.position.z / (GameManager.Instance.levelEndPointZ - GameManager.Instance.levelStartPointZ);
    }
}
