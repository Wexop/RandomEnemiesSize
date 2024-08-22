﻿using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace RandomEnemiesSize.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class PatchRoundManager
{
    [HarmonyPatch(nameof(RoundManager.LoadNewLevel))]
    [HarmonyPrefix]
    private static void PatchLoadLevel(RoundManager __instance)
    {
        RandomEnemiesSize.instance.RandomEnemiesSizeDataDictionary.Clear();
        
        var mapHazards = __instance.currentLevel.spawnableMapObjects.ToList();
        foreach (var spawnableMapObject in mapHazards)
        {
            var name = spawnableMapObject.prefabToSpawn.name;
            if(name.Contains("TurretContainer") || name.Contains("Landmine") || name.Contains("SpikeRoofTrapHazard")) return;
            
            spawnableMapObject.prefabToSpawn.AddComponent<MapHazardSizeRandomizer>();
        }
    }
}