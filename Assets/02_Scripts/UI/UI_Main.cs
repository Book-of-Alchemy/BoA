using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Main : UIBase
{
    [Header("Buttons")]
    [SerializeField] private Button[] _menuBtn;
    

    private void Start()
    {
        _menuBtn[0].Select();
    }

    public void OnClickNewGame()
    {
        UIManager.Show<UI_HUD>();
    }

    public void OnClickLoadGame()
    {
        
    }

    public void OnClickSetting()
    {
        UIManager.Show<UI_Setting>();
    }

    public void OnClickQuitGame()
    {
#if UNITY_EDITOR // 에디터 상 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else //그 외 종료
        Application.Quit();
#endif
    }

    public override void Opened(params object[] param)
    {
        
    }

    public override void HideDirect() 
    {
        UIManager.Hide<UI_Main>();
    }
}
