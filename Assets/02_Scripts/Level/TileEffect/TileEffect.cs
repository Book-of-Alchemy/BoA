using UnityEngine;

public interface IGround
{

}

public interface IWater
{

}

public interface IAir
{

}

public interface IExpirable
{
    public int LeftTime { get; set; }
    public void Expire();
}

public abstract class TileEffect : MonoBehaviour, ITurnProcessor
{
    protected int currentTime = 0;
    public int CurrentTime { get => currentTime; set => currentTime = value; }
    protected int nextActionTime = 0;
    public int NextActionTime { get => nextActionTime; set => nextActionTime = value; }
    public int ActionCost => 10;
    public bool ActionInProgress { get; private set; }
    protected Tile curTile;
    public Tile CurTile { get => curTile; set => curTile = value; }
    public abstract EnvironmentType EnvType { get; }

    public EnvironmentPrefab prefab;
    protected virtual void Awake()
    {
        prefab = GetComponent<EnvironmentPrefab>();
        prefab.OnReturnEvent += OnReturn;
    }
    protected void OnDestroy()
    {
        prefab.OnReturnEvent -= OnReturn;
    }

    public virtual void Init(Tile tile)
    {
        CurrentTime = TurnManager.Instance.globalTime;
        NextActionTime = CurrentTime + ActionCost;
        CurTile = tile;
        TurnManager.Instance.AddTileEffect(this);
    }
    public abstract void PerformAction();

    public void StartTurn()
    {
        ActionInProgress = true;
        PerformAction();
    }

    public void AdvanceTime(int cost)
    {
        CurrentTime = NextActionTime;
        NextActionTime += cost;
    }

    public void OnTurnEnd()
    {
        if (ActionInProgress)
            ActionInProgress = false;
    }

    public void OnReturn()
    {
        prefab.OnReturnEvent -= OnReturn;
        if (this is IAir && CurTile.airEffect != null)
        {
            CurTile.airEffect = null;
        }
        else if (this is IGround && CurTile.groundEffect != null)
        {
            CurTile.groundEffect = null;
        }
        TurnManager.Instance.RemoveTileEffectt(this);
    }
}
