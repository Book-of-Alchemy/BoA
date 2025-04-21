using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public abstract class BaseItem : MonoBehaviour
{
    public ItemData data;
    public abstract void UseItem();
    public abstract void AddItem();
    public abstract void DropItem();

}

