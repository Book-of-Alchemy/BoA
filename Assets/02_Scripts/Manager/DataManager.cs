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
    }

    public void LoadData()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            playerData = JsonConvert.DeserializeObject<PlayerData>(jsonData);
            
            // Inventory가 이미 초기화되어 있다면 골드 값을 Inventory에 적용
            if (Inventory.HasInstance && Inventory.Instance != null)
            {
                Inventory.Instance.SetGold(playerData.Gold);
            }
            GameManager.Instance.killTracker = playerData.KillTracker;//임시로 추가해봄
        }
        else
        {
            playerData = new PlayerData();
        }
    }
    
    public bool HasSaveData()
    {
        return File.Exists(savePath);
    }
    
    public void ResetData()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        
        playerData = new PlayerData();
        
        if (Inventory.HasInstance && Inventory.Instance != null)
        {
            Inventory.Instance.SetGold(0);
        }
        
    }
}