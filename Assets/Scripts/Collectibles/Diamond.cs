using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : BaseCollectible
{
    public override CollectibleType collectibleType => CollectibleType.Diamond;

    public override void GetCollected()
    {
        Debug.Log(name + " : I am collected!");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetCollected();
    }
}
