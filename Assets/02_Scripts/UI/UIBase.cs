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
    public abstract void Opened(params object[] param); //UI 오픈시 초기화를 위한 메서드
    public abstract void HideDirect(); // UI닫을때 호출
}