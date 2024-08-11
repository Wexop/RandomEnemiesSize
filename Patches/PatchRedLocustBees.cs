using HarmonyLib;
using RandomEnemiesSize.SpecialEnemies;
using UnityEngine;

namespace RandomEnemiesSize.Patches;

[HarmonyPatch(typeof(RedLocustBees))]
public class PatchRedLocustBees
{
    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void PatchMovingTowardPlayer(RedLocustBees __instance)
    {

        if (!RedBeesManagement.instance.BeesDictionary.ContainsKey(__instance.NetworkObjectId)) return;

        int? beeParticleState = Traverse.Create(__instance).Field("beeParticleState").GetValue() as int?;

        if (beeParticleState.HasValue)
        {
            RedBees redBees = RedBeesManagement.instance.BeesDictionary[__instance.NetworkObjectId];
            __instance.agent.speed = Mathf.Clamp(__instance.agent.speed / redBees.multiplier, 1f, 100f);
            
            if(redBees.lastState == beeParticleState) return;
            
            RedBeesManagement.instance.BeesDictionary[__instance.NetworkObjectId].lastState = beeParticleState.Value;

            if (beeParticleState.Value == 0)
            {
                RedBeesManagement.instance.StopTargetingPlayerRescale(__instance.NetworkObjectId);
            }
            else
            {
                RedBeesManagement.instance.TargetPlayerRescale(__instance.NetworkObjectId);
            }
        }
        
        
    }
}