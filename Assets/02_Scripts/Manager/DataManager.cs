using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class DataManager : Singleton<DataManager>
{
    private string savePath;
    private PlayerData playerData;

    protected override void Awake()
    {
        base.Awake();
        savePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        Debug.Log($"DataManager - 저장 경로: {savePath}");
        LoadData();
    }

    public PlayerData GetPlayerData()
    {
        if (playerData == null)
        {
            playerData = new PlayerData();
        }
        return playerData;
    }

    public void SaveData()
    {
        if (playerData == null)
        {
            playerData = new PlayerData();
        }

        string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented);
        File.WriteAllText(savePath, jsonData);
        Debug.Log($"데이터 저장 완료: {savePath}");
        Debug.Log($"저장된 플레이어 데이터: 골드={playerData.Gold}, 수락한 퀘스트={string.Join(",", playerData.AcceptedQuests)}, 완료한 퀘스트={string.Join(",", playerData.ClearedQuests)}");
    }

    public void LoadData()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            playerData = JsonConvert.DeserializeObject<PlayerData>(jsonData);
            Debug.Log($"데이터 로드 완료: {savePath}");
            
            // Inventory가 이미 초기화되어 있다면 골드 값을 Inventory에 적용
            if (Inventory.HasInstance && Inventory.Instance != null)
            {
                Inventory.Instance.SetGold(playerData.Gold);
                Debug.Log($"인벤토리에 골드 정보 적용: {playerData.Gold}");
            }
        }
        else
        {
            playerData = new PlayerData();
            Debug.Log("저장된 데이터가 없어 새로운 데이터를 생성합니다.");
        }
    }
}