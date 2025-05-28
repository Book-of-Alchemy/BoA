using UnityEngine;
using UnityEngine.UI;

public class UI_Main : UIBase
{
    [Header("Buttons")]
    [SerializeField] private Button[] _menuBtn;

    public override bool IsClosable => false;
    private QuestData _currentQuest;
    private void Start()
    {
        _menuBtn[0].Select();
    }

    public void OnClickNewGame()
    {
        // 데이터 초기화
        if (DataManager.HasInstance && DataManager.Instance != null&& QuestManager.Instance.AcceptedQuest != null)
        {
            int questId = QuestManager.Instance.AcceptedQuest.Data.id;
            QuestManager.Instance.AcceptedQuest = null;
            DataManager.Instance.ResetData();
        }
        else
        {
            DataManager.Instance.ResetData();
        }
            //// 시설 데이터 초기화
            //if (FacilityManager.HasInstance && FacilityManager.Instance != null)
            //{
            //    FacilityManager.Instance.ResetFacilityData();
            //}

            GameSceneManager.Instance.ChangeScene(SceneType.Town);
    }

    public void OnClickLoadGame()
    {
        if(DataManager.Instance.HasSaveData())
        {
            if (QuestManager.Instance != null && QuestManager.Instance.AcceptedQuest != null)
            {
                int questId = QuestManager.Instance.AcceptedQuest.Data.id;
                DataManager.Instance.GetPlayerData().AcceptedQuests.Clear();
                QuestManager.Instance.AcceptedQuest = null;
                DataManager.Instance.SaveData();
            }
            GameSceneManager.Instance.ChangeScene(SceneType.Town);
        }
        else
        {
            UIManager.ShowOnce<UI_Text>("저장 데이터가 없습니다.");
        }
        
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
