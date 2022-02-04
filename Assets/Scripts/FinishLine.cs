using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public static event Action OnFinishLine;

    private void OnTriggerEnter(Collider other)
    {
        OnFinishLine?.Invoke();
    }
}
