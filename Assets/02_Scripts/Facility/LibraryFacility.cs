using UnityEngine;

public class LibraryFacility : MonoBehaviour, IFacilityUI
{
    [SerializeField] private int facilityId = 270002;

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
        // 시설 레벨 가져오기
        int level = FacilityManager.Instance.GetFacilityLevel(facilityId);

        switch (level)
        {
            case 0:
                UIManager.Show<UI_Text>("지식의 전당은 아직 들어갈수없습니다.");
                return;

            case 1:
                Debug.Log("지식의 전당 열림");
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