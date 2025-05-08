using UnityEngine;

public class HUDPresenter 
{
    private UI_HUD _hud;
    private PlayerStats _playerStats;

    public HUDPresenter(UI_HUD hud)
    {
        _hud = hud;
        if(GameManager.Instance.PlayerTransform != null)
        {
            //PlayerStats 찾아서 이벤트 구독
            _playerStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
            _playerStats.OnHealthRatioChanged += UpdateUIBar;
        }
    }

    public void UpdateUIBar()
    {
        //UI 바 업데이트
        float val = _playerStats.CurrentHealth/_playerStats.MaxHealth;
        _hud.UpdateHp(val);
    }

}
