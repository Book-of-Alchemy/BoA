using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
public interface IItemObservable
{
    event Action OnItemChanged;
    bool IsEmpty { get; }
}

public abstract class SlotUIBase<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler where T : class
{
    [SerializeField] protected Image _icon;
    [SerializeField] protected TextMeshProUGUI _countTxt;

    protected T _data;
    public T Data => _data;
    public virtual bool HasData => _data != null;

    public int Index { get; set; }
    private bool _isPointerOver = false;

    public virtual void SetData(T data)
    {
        if (_data is IItemObservable oldObs)
            oldObs.OnItemChanged -= Refresh;

        _data = data;

        if (_data is IItemObservable newObs)
            newObs.OnItemChanged += Refresh;

        Refresh();
    }

    protected virtual void Refresh()
    {
        if (this == null || gameObject == null) return;
        
        if (_data is IItemObservable obs && obs.IsEmpty)
        {
            ClearUI();
        }
        else
        {
            if (_data != null)
                UpdateUI(_data);
        }
    }

    public virtual void RemoveData()
    {
        if (_data is IItemObservable obs)
            obs.OnItemChanged -= Refresh;

        _data = null;
        if (this != null && gameObject != null)
            ClearUI();
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (_data == null) return;
        ShowTooltip(_data);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    protected virtual void ShowTooltip(T data)
    {
        UIManager.Show<UI_ItemTooltip>(data);
    }

    protected virtual void HideTooltip()
    {
        UIManager.Hide<UI_ItemTooltip>();
    }
    protected abstract void UpdateUI(T data);
    protected abstract void ClearUI();

    public abstract void OnClick();
}
