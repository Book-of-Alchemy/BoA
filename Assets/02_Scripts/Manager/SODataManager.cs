using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SODataManager : Singleton<SODataManager>
{
    public TileDataBase tileDataBase;
    public EnemyDataBase enemyDataBase;
    public QuestDataBase questDataBase;
    public ProjectileDataBase projectileDataBase;
    public EffectDataBase effectDataBase;
    public ItemDataBase itemDataBase;
    public StatusEffectDataBase StatusEffectDataBase;
    public ArtifactDataBase ArtifactDataBase;
    public EnvironmentalDataBase environmentalDataBase;
    public GameObject playerPrefab;

}
