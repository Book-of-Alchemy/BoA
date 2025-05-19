using System.Collections.Generic;
using UnityEngine;
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<Transform> parents;
    [SerializeField] private List<UIBase> uiList = new List<UIBase>(); // SerializeField for Debugging

    [Tooltip("한번 출력되는 UI의 생명기간")]
    [SerializeField] static private float _fadeOutDuration = 1.4f;
    
    private void Start()
    {

    }

    public static void SetParents(List<Transform> parents)
    {
        Instance.parents = parents;
        Instance.uiList.Clear();
    }

    public static T Show<T>(params object[] param) where T : UIBase
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());

        if (IsOpened<T>()) // 이미 열려있는데 Show를 호출시 UI를 숨기고 리턴
        {
            Hide<T>();
            return (T)ui;
        }

        //uiList에 없다면
        if (ui == null)
        {
            //UI 생성 , (Clone) 제거
            var prefab = UIResourceManager.Instance.LoadUIToKey<T>("UI/" + typeof(T).ToString());
            ui = Instantiate(prefab, Instance.parents[(int)prefab.uiPosition]);
            ui.name = ui.name.Replace("(Clone)", "");
            Instance.uiList.Add(ui);
        }
        if (ui.uiPosition == eUIType.UI) //Show한게 UI라면
        {
            //띄워진 UI 전부 끈다.
            Instance.uiList.ForEach(obj =>
            {
                if (obj.uiPosition == eUIType.UI) obj.gameObject.SetActive(false);
            });
        }
        //UI를 켜준다.
        ui.gameObject.SetActive(true);

        //UI 초기화
        ui.Opened(param);
        return (T)ui;
    }

    //한번만 출력되고 사라지는 UI 
    //현재 UI의 생명시간은 Manager에서 관리
    public static void ShowOnce<T>(params object[] param) where T : UIBase
    {
        var prefab = UIResourceManager.Instance.LoadUIToKey<T>("UI/" + typeof(T).ToString());
        if (prefab == null)
        {
            Debug.LogError($"프리팹 찾을수 없음. {typeof(T).Name}");
            return;
        }

        var ui = Instantiate(prefab, Instance.parents[(int)prefab.uiPosition]);
        ui.name = typeof(T).ToString();
        ui.Opened(param);

        Destroy(ui.gameObject, _fadeOutDuration);
    }

    public static void Hide<T>(params object[] param) where T : UIBase
    {
        //있는지 먼저 찾아본다.
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());

        if (ui != null)
        {
            //리스트에서 제거
            Instance.uiList.Remove(ui);
            if (ui.uiPosition == eUIType.UI) //UI라면 
            {
                //전에 열려있던 UI를 찾아서 열어줌
                var prevUI = Instance.uiList.FindLast(obj => obj.uiPosition == eUIType.UI);
                if (prevUI != null)
                    prevUI.gameObject.SetActive(true);
            }
            Destroy(ui.gameObject);
        }
    }

    public static T Get<T>() where T : UIBase
    {
        //열려있는 UI접근
        return (T)Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
    }

    public static bool IsOpened<T>() where T : UIBase //UI 열렸는지 체크
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
        return ui != null && ui.gameObject.activeInHierarchy;
    }
}