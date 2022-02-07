using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : BaseCollectible
{
    [SerializeField] private Animator anim;
    public override CollectibleType collectibleType => CollectibleType.Diamond;

    public override void GetCollected()
    {
        base.GetCollected();
        Debug.Log(name + " : I am collected!");
        anim.SetTrigger("OnCollected");
        Vibration.Vibrate(5);
        //Destroy(gameObject);
    }

   /* private IEnumerator DestroyRoutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {

        }
    }*/
}
