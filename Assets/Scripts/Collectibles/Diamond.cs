using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : BaseCollectible
{
    public override CollectibleType collectibleType => CollectibleType.Diamond;

    public static event Action<int> OnDiamondCollected;

    public override void GetCollected()
    {
        Debug.Log(name + " : I am collected!");
        OnDiamondCollected?.Invoke(1);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetCollected();
    }
}
