using UnityEngine;

public enum eUIType
{
    UI,
    Popup,
    Top,
}

public abstract class UIBase : MonoBehaviour
{
    public eUIType uiPosition;
    public abstract void Opened(params object[] param);
    public abstract void HideDirect();
}