using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public int Gold { get; set; }
    public List<int> AcceptedQuests { get; set; } = new List<int>();
    public List<int> ClearedQuests { get; set; } = new List<int>();

    public List<ResearchStat> ResearchProgress { get; set; } = new List<ResearchStat>();
    public KillTracker KillTracker { get; set; }
    public PlayerData()
    {
        Gold = 0;
        AcceptedQuests = new List<int>();
        ClearedQuests = new List<int>();
        KillTracker = new KillTracker();
    }
} 