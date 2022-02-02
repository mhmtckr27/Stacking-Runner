using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectible
{
    CollectibleType collectibleType { get; }
    void GetCollected(); 
}

public enum CollectibleType
{
    Diamond,
    Gold
}
