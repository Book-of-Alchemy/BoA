using UnityEngine;

public class GuildFacility : MonoBehaviour, IFacilityUI
{
    [SerializeField] private int facilityId = 270001;
    
    private void Start()
    {
        // 시설 해금 여부에 따라 활성화/비활성화
        UpdateVisibility();
    }
    
    private void UpdateVisibility()
    {
        // 해금 여부 확인
        bool isUnlocked = FacilityManager.Instance != null && 
                         FacilityManager.Instance.IsFacilityUnlocked(facilityId);
    }
    
    public void ShowUI()
    {
        int level = FacilityManager.Instance.GetFacilityLevel(facilityId);

        switch (level)
        {
            case 0:
                UIManager.ShowOnce<UI_Text>("길드는 아직 들어갈 수 없습니다.");
                return;

            case 1:
                UIManager.Show<UI_SelectQuest>();
                break;

            case 2:
                //추가기능

                break;

            case 3:
                //추가기능

                break;
        }
    }
}
