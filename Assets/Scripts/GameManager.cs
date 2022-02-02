using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int startStackAmount;
    private bool isGameStarted;

    private void OnEnable()
    {
        TapToPlayScreen.OnTapToPlay += StartGame;
    }

    private void OnDisable()
    {
        TapToPlayScreen.OnTapToPlay -= StartGame;
    }

    private void StartGame()
    {
        if (isGameStarted == false)
        {
            PlayerController.Instance.StartRunning();
            isGameStarted = true;
        }
    }

    public void IncreaseStartStack()
    {

    }
}
