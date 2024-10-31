using System.Linq;
using HarmonyLib;
using RandomEnemiesSize.Helper;
using UnityEngine;

namespace RandomEnemiesSize.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class PatchRoundManager
{
    [HarmonyPatch(nameof(RoundManager.GenerateNewLevelClientRpc))]
    [HarmonyPrefix]
    private static void PatchLoadLevel(RoundManager __instance)
    {
        RandomEnemiesSize.instance.RandomEnemiesSizeDataDictionary.Clear();

    }
    [HarmonyPatch(nameof(RoundManager.SpawnMapObjects))]
    [HarmonyPrefix]
    private static void PatchLoadNewLevel(RoundManager __instance)
    {
        RandomEnemiesSize.instance.AddRandomSizeToHazards();
        
        var mapHazards = __instance.currentLevel.spawnableMapObjects.ToList();
        foreach (var spawnableMapObject in mapHazards)
        {
            var name = spawnableMapObject.prefabToSpawn.name;
            if(name.Contains("TurretContainer") || name.Contains("Landmine") || name.Contains("SpikeRoofTrapHazard")) return;

            var component = spawnableMapObject.prefabToSpawn.GetComponent<MapHazardSizeRandomizer>();
            if (component == null)
                component = spawnableMapObject.prefabToSpawn.GetComponentInChildren<MapHazardSizeRandomizer>();
            
            if(component == null) spawnableMapObject.prefabToSpawn.AddComponent<MapHazardSizeRandomizer>();
        }
        
        var outsideObjects = __instance.currentLevel.spawnableOutsideObjects.ToList();
        foreach (var spawnableMapObject in outsideObjects)
        {
            var name = spawnableMapObject.spawnableObject.prefabToSpawn.name;
            if(name.Contains("TurretContainer") || name.Contains("Landmine") || name.Contains("SpikeRoofTrapHazard")) return;

            var component = spawnableMapObject.spawnableObject.prefabToSpawn.GetComponent<OutsideMapHazardSizeRandomizer>();
            if (component == null)
                component = spawnableMapObject.spawnableObject.prefabToSpawn.GetComponentInChildren<OutsideMapHazardSizeRandomizer>();
            
            if(component == null) spawnableMapObject.spawnableObject.prefabToSpawn.AddComponent<OutsideMapHazardSizeRandomizer>();
        }

    }
}