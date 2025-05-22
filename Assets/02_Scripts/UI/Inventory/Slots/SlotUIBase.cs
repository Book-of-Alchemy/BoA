using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public interface IItemObservable
{
    event Action OnItemChanged;
    bool IsEmpty { get; }
}

public abstract class SlotUIBase<T> : MonoBehaviour where T : class
{
    [SerializeField] protected Image _icon;
    [SerializeField] protected TextMeshProUGUI _countTxt;

    protected T _data;
    public T Data => _data;
    public virtual bool HasData => _data != null;

    public int Index { get; set; }

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

    protected abstract void UpdateUI(T data);
    protected abstract void ClearUI();

    public abstract void OnClick();
}
