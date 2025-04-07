using System.Collections.Generic;
using UnityEngine;
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<Transform> parents;
    private List<UIBase> uiList = new List<UIBase>();

    public static void SetParents(List<Transform> parents)
    {
        Instance.parents = parents;
        Instance.uiList.Clear();
    }

    public static T Show<T>(params object[] param) where T : UIBase 
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
        //uiList�� ���ٸ�
        if (ui == null)
        {
            //UI ���� , (Clone) ����
            var prefab = ResourceManager.Instance.LoadUIToKey<T>("UI/" + typeof(T).ToString());
            ui = Instantiate(prefab, Instance.parents[(int)prefab.uiPosition]);
            ui.name = ui.name.Replace("(Clone)", ""); 
            Instance.uiList.Add(ui);
        }
        if (ui.uiPosition == eUIType.UI) //Show�Ѱ� UI���
        {
            //����� UI ���� ����.
            Instance.uiList.ForEach(obj =>
            {
                if (obj.uiPosition == eUIType.UI) obj.gameObject.SetActive(false);
            });
        }
        //UI�� ���ش�.
        ui.gameObject.SetActive(true);
        //UI �ʱ�ȭ
        ui.Opened(param);
        return (T)ui;
    }

    public static void Hide<T>(params object[] param) where T : UIBase
    {
        //�ִ��� ���� ã�ƺ���.
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());

        if (ui != null)
        {
            //����Ʈ���� ����
            Instance.uiList.Remove(ui);
            if (ui.uiPosition == eUIType.UI) //UI��� 
            {
                //���� �����ִ� UI�� ã�Ƽ� ������
                var prevUI = Instance.uiList.FindLast(obj => obj.uiPosition == eUIType.UI);
                prevUI.gameObject.SetActive(true);
            }
            Destroy(ui.gameObject);
        }
    }

    public static T Get<T>() where T : UIBase
    {
        //�����ִ� UI����
        return (T)Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
    }

    public static bool IsOpened<T>() where T : UIBase //UI ���ȴ��� üũ
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
        return ui != null && ui.gameObject.activeInHierarchy;
    }
}