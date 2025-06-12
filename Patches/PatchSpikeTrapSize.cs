using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace RandomEnemiesSize.Patches
{
    [HarmonyPatch(typeof(SpikeRoofTrap))]
    public class PatchSpikeTrapSize
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void PatchStart(SpikeRoofTrap __instance)
        {
            if (!__instance.IsServer || !__instance.IsOwner) return;

            if (!RandomEnemiesSize.instance.customAffectSpikeTrapEntry.Value) return;

            //RANDOM PERCENT

            var randomPercent = Random.Range(0f, 100f);

            if (RandomEnemiesSize.instance.randomPercentChanceEntry.Value < randomPercent)
            {
                if (RandomEnemiesSize.instance.devLogEntry.Value)
                    Debug.Log(
                        $"RANDOM PERCENT NOT RANDOM SIZE : {randomPercent} FOR ENEMY {__instance.gameObject.name}");
                return;
            }

            var scale = Random.Range(RandomEnemiesSize.instance.minSizeSpikeTrapEntry.Value,
                RandomEnemiesSize.instance.maxSizeSpikeTrapEntry.Value);
            
            var customMoonEnemy = RandomEnemiesSize.instance.GetCustomMoonEnemySize(__instance.gameObject.name, StartOfRound.Instance.currentLevel.PlanetName);
            if (customMoonEnemy.found) scale = Random.Range(customMoonEnemy.minValue, customMoonEnemy.maxValue);

            var networkObject = __instance.NetworkObject;

            var originalScale = new Vector3(1,0,1);
            var newScale = originalScale * scale;

            if (RandomEnemiesSize.instance.funModeEntry.Value)
            {
                var funXSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value,
                    RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);
                var funZSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value,
                    RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);

                if (RandomEnemiesSize.instance.funModeLockHorizontalEnrty.Value) funZSize = funXSize;

                newScale = new Vector3(newScale.x * funXSize, newScale.y, newScale.z * funZSize);
            }

            //changes on every client

            var influences = new Influences();
            influences.GetInfos();

            newScale = new Vector3(newScale.x, 1, newScale.z);

            NetworkSize.UpdateSpikeTrapClientRpc(networkObject.NetworkObjectId, newScale, scale, influences);
        }
    }
}