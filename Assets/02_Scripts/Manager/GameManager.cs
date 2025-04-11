using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Transform PlayerTransform { get; private set; }

    public void RegisterPlayer(Transform player)
    {
        PlayerTransform = player;
    }
}