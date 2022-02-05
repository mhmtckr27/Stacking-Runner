using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : BaseCollectible
{
    public override CollectibleType collectibleType => CollectibleType.Gold;

    public override void GetCollected()
    {
        base.GetCollected();
        Debug.Log(name + " : I am collected!");
        Destroy(gameObject);
    }
}
