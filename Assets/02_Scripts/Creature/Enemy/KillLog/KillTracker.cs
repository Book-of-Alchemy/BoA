using System.Collections.Generic;
using UnityEngine;

public class KillTracker
{
    private Dictionary<int, int> totalKillCounts = new();   // 전체 킬 수 int key = ID  int value = amount
    private Dictionary<int, int> dungeonKillCounts = new(); // 던전 한정 킬 수

    /// <summary>
    /// 몬스터 처치 보고
    /// </summary>
    public void ReportKill(int monsterID)
    {
        if (!totalKillCounts.ContainsKey(monsterID))
            totalKillCounts[monsterID] = 0;
        totalKillCounts[monsterID]++;

        if (!dungeonKillCounts.ContainsKey(monsterID))
            dungeonKillCounts[monsterID] = 0;
        dungeonKillCounts[monsterID]++;

    }

    /// <summary>
    /// 특정 몬스터의 전체 처치 수
    /// </summary>
    public int GetKillCount(int monsterID)
    {
        return totalKillCounts.TryGetValue(monsterID, out var count) ? count : 0;
    }

    /// <summary>
    /// 특정 몬스터의 던전 내 처치 수
    /// </summary>
    public int GetDungeonKillCount(int monsterID)
    {
        return dungeonKillCounts.TryGetValue(monsterID, out var count) ? count : 0;
    }

    /// <summary>
    /// 전체 킬 수 합산
    /// </summary>
    public int GetTotalKills()
    {
        int total = 0;
        foreach (var count in totalKillCounts.Values)
            total += count;
        return total;
    }

    /// <summary>
    /// 던전 킬 수 합산
    /// </summary>
    public int GetDungeonTotalKills()
    {
        int total = 0;
        foreach (var count in dungeonKillCounts.Values)
            total += count;
        return total;
    }

    public void PrintAllKills()
    {
        foreach (var pair in totalKillCounts)
            Debug.Log($"몬스터 {pair.Key}: {pair.Value}회");
        foreach (var pair in dungeonKillCounts)
            Debug.Log($"몬스터 {pair.Key}: {pair.Value}회");
    }

    /// <summary>
    /// 던전 시작 시 초기화할 수 있도록 선택적으로 제공
    /// </summary>
    public void ResetDungeonKills()
    {
        dungeonKillCounts.Clear();
    }
}
