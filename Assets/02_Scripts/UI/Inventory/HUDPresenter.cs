using UnityEngine;

public class HUDPresenter 
{
    private UI_HUD _hud;
    private PlayerStats _playerStats;

    public HUDPresenter(UI_HUD hud)
    {
        _hud = hud;

        _playerStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();

        // 체력 변경 이벤트 구독
        _playerStats.OnHealthRatioChanged += UpdateHealth;
        // 마나 변경 이벤트 구독
        _playerStats.OnManaChanged += UpdateMana;
        // 경험치 변경 이벤트 구독
        _playerStats.OnExperienceChanged += UpdateExp;
        _playerStats.OnLevelChanged += UpdateLevelTxt;

        // 초기 UI 업데이트
        UpdateHealth();
        UpdateMana();
        UpdateExp();
    }
    private void UpdateHealth()
    {
        float healthRatio = _playerStats.CurrentHealth / _playerStats.MaxHealth;
        _hud.UpdateHp(healthRatio);
    }

    private void UpdateMana()
    {
        float manaRatio = _playerStats.CurrentMana / _playerStats.MaxMana;
        _hud.UpdateMp(manaRatio);
    }

    private void UpdateExp()
    {
        float expRatio = (float)_playerStats.experience / _playerStats.nextLevelExp;
        _hud.UpdateExp(expRatio);
    }

    private void UpdateLevelTxt(int level)
    {
        _hud.UpdateLevelTxt(level);
    }

    //public void UpdateUIBar()
    //{
    //    //UI 바 업데이트
    //    float val = _playerStats.CurrentHealth/_playerStats.MaxHealth;
    //    _hud.UpdateHp(val);
    //}

}
