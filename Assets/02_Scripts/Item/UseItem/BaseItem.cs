using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public abstract class BaseItem : MonoBehaviour
{
    public abstract void UseItem(ItemData data);
    public abstract void AddItem(ItemData data);
    public abstract void DropItem(ItemData data);

}

