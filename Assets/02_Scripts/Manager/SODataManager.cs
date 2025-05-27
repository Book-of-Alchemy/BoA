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
    public FacilityDataBase facilityDataBase;
    public EnvRuleDataBase envRuleDataBase;
    public ResearchDataBase researchDataBase;
    public GameObject playerPrefab;


    protected override void Awake()
    {
        base.Awake();
        //tileDataBase.Arrange();
        //enemyDataBase.Arrange();
        //itemDataBase.Arrange();
        //StatusEffectDataBase.Arrange();
        //ArtifactDataBase.Arrange();
        //environmentalDataBase.Arrange();
        //envRuleDataBase.Arrange();
    }
    public QuestDataBase GetQuestDataBase()
    {
        return questDataBase;
    }
}
