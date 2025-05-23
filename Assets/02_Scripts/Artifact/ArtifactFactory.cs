using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ArtifactFactory
{
    public static List<ArtifactData> copyArtifacts = new List<ArtifactData>(SODataManager.Instance.ArtifactDataBase.artifacts);
    public static Artifact EquipArtifact(int id)
    {
        Artifact artifact = CreateArtifact(id);
        GameManager.Instance.PlayerTransform.equipArtifacts.Add(artifact);
        artifact.Equip(GameManager.Instance.PlayerTransform);
        copyArtifacts.Remove(artifact.data);
        return artifact;
    }

    public static ArtifactData[] RandomArtifacts()
    {
        List<ArtifactData> commonArtifacts = copyArtifacts.Where(artifact => artifact.rarity == Rarity.Common).ToList();
        List<ArtifactData> uncommonArtifacts = copyArtifacts.Where(artifact => artifact.rarity == Rarity.Uncommon).ToList();
        List<ArtifactData> rareArtifacts = copyArtifacts.Where(artifact => artifact.rarity == Rarity.Rare).ToList();
        List<List<ArtifactData>> artifactsList = new List<List<ArtifactData>>();
        List<ArtifactData> selectArtifacts = new List<ArtifactData>();

        artifactsList.Add(commonArtifacts);
        artifactsList.Add(uncommonArtifacts);
        artifactsList.Add(rareArtifacts);
        int[] weight = { 60, 30, 10 }; // 확률 가중치
        int total = weight.Sum(); // 확률 합(100)

        for(int i = 0; i < 3; i++)
        {
            int rand = UnityEngine.Random.Range(1, total + 1);
            int cumulative = 0;

            for (int j = 0; j < weight.Length; j++)
            {
                cumulative += weight[j];
                if (rand <= cumulative)
                {
                    if (artifactsList[j].Count == 0)
                    {
                        for(int k = 0; k < weight.Length; k++)
                        {
                            if (artifactsList[k].Count != 0)
                            {
                                int randItemIndex2 = UnityEngine.Random.Range(0, artifactsList[k].Count);
                                selectArtifacts.Add(artifactsList[k][randItemIndex2]);
                                artifactsList[k].Remove(artifactsList[k][randItemIndex2]);
                                break;
                            }
                        }
                    }
                    else 
                    {
                        int randItemIndex = UnityEngine.Random.Range(0, artifactsList[j].Count);
                        selectArtifacts.Add(artifactsList[j][randItemIndex]);
                        artifactsList[j].Remove(artifactsList[j][randItemIndex]);
                        break;
                    }
                    break;
                }
            }
        }
        //Debug.Log($"아티팩트 리스트{selectArtifacts[0]},{selectArtifacts[1]},{selectArtifacts[2]}");
        return selectArtifacts.ToArray();

    }
    public static Artifact CreateArtifact(int id)
    {
        if (SODataManager.Instance.ArtifactDataBase == null)
            return null;
        Dictionary<int, ArtifactData> kvp = SODataManager.Instance.ArtifactDataBase.artifactsDataById;

        switch (id)
        {
            case 190000: return new EssenceOfFire(kvp[id]);
            case 190001: return new EssenceOfWater(kvp[id]);
            case 190002: return new EssenceofCold(kvp[id]);
            case 190003: return new EssenceofWind(kvp[id]);
            case 190004: return new EssenceofLightning(kvp[id]);
            case 190005: return new EssenceofEarth(kvp[id]);
            case 190006: return new EssemceofLight(kvp[id]);
            case 190007: return new EssenceofDark(kvp[id]);
            case 190008: return new TastyBread(kvp[id]);
            case 190009: return new FreshWater(kvp[id]);
            case 190010: return new AmuletofProtect(kvp[id]);
            case 190011: return new BlessofFire(kvp[id]);
            case 190012: return new BlessofWater(kvp[id]);
            case 190013: return new BlessofCold(kvp[id]);
            case 190014: return new BlessofWind(kvp[id]);
            case 190015: return new BlessofLightning(kvp[id]);
            case 190016: return new BlessofEarth(kvp[id]);
            case 190017: return new BlessofLight(kvp[id]);
            case 190018: return new BlessofDark(kvp[id]);
            case 190019: return new EnhancedAttack(kvp[id]);
            case 190020: return new EnhancedThrowing(kvp[id]);
            case 190021: return new EnhancedScroll(kvp[id]);
            case 190022: return new EnhancedTrap(kvp[id]);
            case 190100: return new StrengthTraining(kvp[id]);
            case 190101: return new MagicTraining(kvp[id]);
            case 190102: return new CraftTraining(kvp[id]);
            case 190103: return new BlessofFairy(kvp[id]);
            case 190104: return new WisdomofFairy(kvp[id]);
            case 190105: return new ProtectofFairy(kvp[id]);
            case 190107: return new EmergencyStelthkit(kvp[id]);
            case 190108: return new DeathMark(kvp[id]);
            case 190109: return new Purity(kvp[id]);
            case 190110: return new MarkofPyromania(kvp[id]);
            case 190111: return new FireAmplifier(kvp[id]);
            case 190112: return new HandoftheDrowned(kvp[id]);
            case 190113: return new WaterAmplifier(kvp[id]);
            case 190114: return new IceBreaker(kvp[id]);
            case 190115: return new ColdAmplifier(kvp[id]);
            case 190116: return new BlessOfWindFairy(kvp[id]);
            case 190117: return new WindAmpllifier(kvp[id]);
            case 190118: return new Capacitor(kvp[id]);
            case 190119: return new LightningAmplifier(kvp[id]);
            case 190120: return new AftershockTotem(kvp[id]);
            case 190121: return new EarthAmplifier(kvp[id]);
            case 190122: return new AttackoftheLight(kvp[id]);
            case 190123: return new LightAmplifier(kvp[id]);
            case 190124: return new RaiderintheDarkness(kvp[id]);
            case 190125: return new DarkAmplifier(kvp[id]);
            case 190126: return new UnstableGuardian(kvp[id]);
            case 190127: return new MarkofRaider(kvp[id]);
            case 190128: return new OverwhelmingOdds(kvp[id]);
            case 190129: return new ShieldAmplifier(kvp[id]);
            case 190130: return new AmuletofRestore(kvp[id]);
            case 190131: return new TrapDetector(kvp[id]);
            case 190200: return new Marksman(kvp[id]);
            case 190201: return new Sharpeye(kvp[id]);
            case 190202: return new ManaOverload(kvp[id]);
            case 190203: return new PrecisionAimforMagic(kvp[id]);
            case 190205: return new EasyInstallationKit(kvp[id]);
            default: throw new Exception("Unknown status ID: " + id);
        }
    }


}
