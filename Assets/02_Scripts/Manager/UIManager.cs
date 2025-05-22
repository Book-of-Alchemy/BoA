using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<Transform> parents;
    private Dictionary<System.Type, UIBase> _uiDict = new();
    private Canvas _canvas;

    [Tooltip("한번 출력되는 UI의 생명기간")]
    [SerializeField] static private float _fadeOutDuration = 1.4f;

    public Canvas GetCanvas() => _canvas != null ? _canvas : null;

    private static void EnsureParents()
    {
        if (Instance.parents == null || Instance.parents.Count == 0 || Instance.parents.Exists(p => p == null))
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Debug.LogError("Canvas를 찾을 수 없습니다.");
                return;
            }
            if(Instance._canvas ==null)
                Instance._canvas = canvas.GetComponent<Canvas>();

            Instance.parents = new List<Transform>
            { 
            canvas.transform.Find("UI"),
            canvas.transform.Find("Popup"),
            canvas.transform.Find("Top")
            };
        }
    }

    public static void SetParents(List<Transform> parents)
    {
        Instance.parents = parents;
        Instance._uiDict.Clear();
    }
    public void RefreshUIList()
    {
        _uiDict = FindObjectsOfType<UIBase>(true)
        .Where(ui => ui != null)
        .ToDictionary(ui => ui.GetType(), ui => ui);
    }

    public static T Show<T>(params object[] param) where T : UIBase
    {
        EnsureParents();
        var type = typeof(T);
        Debug.LogWarning(type);

        if (IsOpened<T>()) // 이미 열려있는데 Show를 호출시 UI를 숨기고 리턴
        {
            Hide<T>();
            Debug.Log(typeof(T));
            return Instance._uiDict.TryGetValue(type, out var openedUI) ? (T)openedUI : null;
        }

        //_uiDict에 없다면
        if (!Instance._uiDict.TryGetValue(type, out var ui) || ui == null)
        {
            var key = "UI/" + type;

            //프리팹을 로드
            var prefab = UIResourceManager.Instance.LoadUIToKey<T>(key);
            if (prefab == null)
            {
                Debug.LogError($"프리팹을 찾을 수 없음: {key}");
                return null;
            }

            //프리팹의 부모 선택
            var parent = Instance.parents[(int)prefab.uiPosition];

            //인스턴스 생성 및 등록
            ui = Object.Instantiate(prefab, parent);
            ui.name = type.ToString();

            Instance._uiDict[type] = ui;
        }
        if (ui.uiPosition == eUIType.UI) //Show한게 UI라면
        {
            //띄워진 UI 전부 끈다.
            foreach (var otherUI in Instance._uiDict.Values)
            {
                if (otherUI != null && otherUI.uiPosition == eUIType.UI)
                    otherUI.gameObject.SetActive(false);
            }
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
        EnsureParents();
        var key = "UI/" + typeof(T);
        var ui = UIResourceManager.Instance.InstantiateUI<T>(key, Instance.parents[0]);

        if (ui == null) return;

        ui.Opened(param);
        Destroy(ui.gameObject, _fadeOutDuration);
    }

    public static void Hide<T>(params object[] param) where T : UIBase
    {
        //있는지 먼저 찾아본다.
        var type = typeof(T);
        if (!Instance._uiDict.TryGetValue(type, out var ui) || ui == null) return;

        Instance._uiDict.Remove(type);

        if (ui.uiPosition == eUIType.UI)
        {
            var prevUI = Instance._uiDict.Values.LastOrDefault(obj => obj != null && obj.uiPosition == eUIType.UI);
            if (prevUI != null)
                prevUI.gameObject.SetActive(true);
        }
        Destroy(ui.gameObject);
    }

    public static void CloseLastOpenedUI()
    {
        //마지막에 열린 PopUp UI를 찾음.
        var lastPopUp = Instance._uiDict.Values.LastOrDefault(ui => ui.uiPosition == eUIType.Popup && ui.IsClosable);
        if (lastPopUp != null)
        {
            // PopUp이면 제거
            lastPopUp.HideDirect();
            return;
        }
        // PopUp이 없다면 마지막 UI 타입 찾기
        var lastUI = Instance._uiDict.Values.LastOrDefault(ui => ui.uiPosition == eUIType.UI && ui.IsClosable);
        if (lastUI != null)
        {
            // UI라면 HideDirect 호출 후 비활성화
            lastUI.HideDirect();
        }
    }

    public static T Get<T>() where T : UIBase
    {
        //열려있는 UI접근
        return Instance._uiDict.TryGetValue(typeof(T), out var ui) ? (T)ui : null;
    }

    public static bool IsOpened<T>() where T : UIBase //UI 열렸는지 체크
    {
        return Instance._uiDict.TryGetValue(typeof(T), out var ui) && ui.gameObject.activeInHierarchy;
    }
}