using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class SlotUIBase<TData> : MonoBehaviour
{
    [SerializeField] protected Image _icon;
    [SerializeField] protected TextMeshProUGUI _countTxt;

    protected TData _data;
    public TData Data { get { return _data; } }
    public virtual bool HasData => _data != null;

    public int Index { get; set; }

    public virtual void SetData(TData data)
    {
        _data = data;
        UpdateUI(data);
    }

    public virtual void RemoveData()
    {
        _data = default;
        ClearUI();
    }

    protected abstract void UpdateUI(TData data);
    protected abstract void ClearUI();

    public abstract void OnClick();
}
