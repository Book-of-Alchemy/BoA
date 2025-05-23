using UnityEngine;
using UnityEngine.UI;

public class UI_Main : UIBase
{
    [Header("Buttons")]
    [SerializeField] private Button[] _menuBtn;

    public override bool IsClosable => false;

    private void Start()
    {
        _menuBtn[0].Select();
    }

    public void OnClickNewGame()
    {
        // 데이터 초기화
        if (DataManager.HasInstance && DataManager.Instance != null)
        {
            DataManager.Instance.ResetData();
            Debug.Log("새 게임을 시작합니다. 데이터를 초기화했습니다.");
        }
        
        //// 시설 데이터 초기화
        //if (FacilityManager.HasInstance && FacilityManager.Instance != null)
        //{
        //    FacilityManager.Instance.ResetFacilityData();
        //    Debug.Log("시설 데이터를 초기화했습니다.");
        //}

        GameSceneManager.Instance.ChangeScene(SceneType.Town);
    }

    public void OnClickLoadGame()
    {
        GameSceneManager.Instance.ChangeScene(SceneType.Town);
    }

    public void OnClickSetting()
    {
        UIManager.Show<UI_Setting>();
        //HideDirect();
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
