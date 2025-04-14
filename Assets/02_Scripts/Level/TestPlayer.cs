using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TestPlayer : MonoBehaviour
{
    public Tile curTile;

    private void Start()
    {
        curTile = new Tile {
            gridPosition = new Vector2Int(Mathf.RoundToInt( transform.position.x), Mathf.RoundToInt(transform.position.y)),
            tileType = TileType.ground,
            environment = EnvironmentType.none,
            isDoorPoint = false,
            isOccupied = false,
            isExplored = false,
            isOnSight = false
        };
    }
}
