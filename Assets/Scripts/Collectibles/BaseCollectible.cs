using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCollectible : MonoBehaviour, ICollectible
{
    public abstract CollectibleType collectibleType { get; }

    public abstract void GetCollected();

    private void OnTriggerEnter(Collider other)
    {
        GetCollected();
    }
}
