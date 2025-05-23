
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TrapFactory
{
    private static List<TrapData> TrapData => SODataManager.Instance.tileDataBase.trapData;
    public static TrapBase CreateTrap(int id, Level level, Tile tile, bool isCreatedByPlayer = false)
    {
        
        int trapID = id;
        TrapBase TrapGO = UnityEngine.Object.Instantiate
            (
            TrapData[trapID].trapPrefab,
            new Vector3Int(tile.gridPosition.x, tile.gridPosition.y, 0),
            Quaternion.identity
            ).GetComponent<TrapBase>();

        tile.TrpaOnTile = TrapGO;
        TrapGO.spriteRenderer.sortingOrder = - tile.gridPosition.y * 10;
        TrapGO.transform.SetParent(level.transform);
        bool detect = false;
        if (GameManager.Instance.PlayerTransform.equipArtifacts.Any(e => e is TrapDetector))
        {
            if(Random.value > 0.1)
                detect = true;
        }
        TrapGO.IsDetected = detect;
        return TrapGO;
    }
}
