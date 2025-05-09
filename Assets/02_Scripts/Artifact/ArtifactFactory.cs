using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class ArtifactFactory
{
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
            case 190103: return new BlessofFairy(kvp[id]);
            case 190104: return new WisdomofFairy(kvp[id]);
            case 190105: return new ProtectofFairy(kvp[id]);
            case 190111: return new FireAmplifier(kvp[id]);
            case 190113: return new WaterAmplifier(kvp[id]);
            case 190115: return new ColdAmplifier(kvp[id]);
            case 190117: return new WindAmpllifier(kvp[id]);
            case 190119: return new LightningAmplifier(kvp[id]);
            case 190121: return new EarthAmplifier(kvp[id]);
            case 190123: return new LightAmplifier(kvp[id]);
            case 190125: return new DarkAmplifier(kvp[id]);

            default: throw new Exception("Unknown status ID: " + id);
        }
    }
}
