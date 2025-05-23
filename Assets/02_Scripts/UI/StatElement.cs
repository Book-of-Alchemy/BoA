using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatElement : MonoBehaviour
{
    [SerializeField] private StatType _statType;
    public StatType StatType => _statType;

    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI _statNameText;
    [SerializeField] private TextMeshProUGUI _valueText;

    public void SetName(string displayName)
    {
        _statNameText.text = displayName;
    }

    public void SetValue(float value)
    {
        _valueText.text = value.ToString("N0");
    }
}