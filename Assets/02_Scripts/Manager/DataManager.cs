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

    // 골드 관련 메서드
    public void AddGold(int amount)
    {
        GetPlayerData().Gold += amount;
        SaveData();
        
        // Inventory에도 반영
        if (Inventory.HasInstance && Inventory.Instance != null)
        {
            // 동기화를 위해 직접 SetGold 사용 (IncreaseGold를 사용하면 중복 저장됨)
            Inventory.Instance.SetGold(GetPlayerData().Gold);
            Debug.Log($"DataManager: {amount} 골드 추가, 현재 골드: {GetPlayerData().Gold}");
        }
    }

    public bool SpendGold(int amount)
    {
        if (GetPlayerData().Gold >= amount)
        {
            GetPlayerData().Gold -= amount;
            SaveData();
            
            // Inventory에도 반영
            if (Inventory.HasInstance && Inventory.Instance != null)
            {
                // 동기화를 위해 직접 SetGold 사용 (DecreaseGold를 사용하면 중복 저장됨)
                Inventory.Instance.SetGold(GetPlayerData().Gold);
                Debug.Log($"DataManager: {amount} 골드 사용, 현재 골드: {GetPlayerData().Gold}");
            }
            
            return true;
        }
        return false;
    }

    // 퀘스트 관련 메서드
    public void AcceptQuest(int questId)
    {
        if (!GetPlayerData().AcceptedQuests.Contains(questId) && !GetPlayerData().ClearedQuests.Contains(questId))
        {
            GetPlayerData().AcceptedQuests.Add(questId);
            SaveData();
        }
    }

    public void CompleteQuest(int questId)
    {
        if (GetPlayerData().AcceptedQuests.Contains(questId))
        {
            GetPlayerData().AcceptedQuests.Remove(questId);
            GetPlayerData().ClearedQuests.Add(questId);
            SaveData();
        }
    }
}