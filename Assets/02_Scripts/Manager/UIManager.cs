using System.Collections.Generic;
using UnityEngine;
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<Transform> parents;
    [SerializeField] private List<UIBase> uiList = new List<UIBase>(); // SerializeField for Debugging

    private void Start()
    {
        Show<UI_Main>();
    }


    public static void SetParents(List<Transform> parents)
    {
        Instance.parents = parents;
        Instance.uiList.Clear();
    }

    public static T Show<T>(params object[] param) where T : UIBase 
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
        if(IsOpened<T>())
        {
            Hide<T>();
            return (T)ui;
        }

        //uiList에 없다면
        if (ui == null)
        {
            //UI 생성 , (Clone) 제거
            var prefab = ResourceManager.Instance.LoadUIToKey<T>("UI/" + typeof(T).ToString());
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
                if(prevUI != null)
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