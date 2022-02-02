using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapToPlayScreen : MonoBehaviour, IPointerDownHandler
{
    public static event Action OnTapToPlay;

    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        OnTapToPlay?.Invoke();
    }
}
