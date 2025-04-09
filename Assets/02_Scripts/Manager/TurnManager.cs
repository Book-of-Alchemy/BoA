using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public enum Turn { Player, Enemy }
    public Turn currentTurn = Turn.Player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void EndTurn()
    {
        currentTurn = (currentTurn == Turn.Player) ? Turn.Enemy : Turn.Player;
    }
}
