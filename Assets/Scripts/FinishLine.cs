using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public static event Action OnLevelFinished;

    private void OnTriggerEnter(Collider other)
    {
        OnLevelFinished?.Invoke();
    }
}
