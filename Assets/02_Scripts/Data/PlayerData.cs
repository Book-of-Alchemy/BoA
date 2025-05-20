using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class PlayerData
{
    public int Gold { get; set; }
    public List<int> AcceptedQuests { get; set; } = new List<int>();
    public List<int> ClearedQuests { get; set; } = new List<int>();

    public PlayerData()
    {
        Gold = 0;
        AcceptedQuests = new List<int>();
        ClearedQuests = new List<int>();
    }
} 