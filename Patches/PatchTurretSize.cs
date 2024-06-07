using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace RandomEnemiesSize.Patches
{
    [HarmonyPatch(typeof(Turret))]
    public class PatchTurretSize
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void PatchStart(Turret __instance)
        {
            if (!__instance.IsServer || !__instance.IsOwner) return;

            if (!RandomEnemiesSize.instance.customAffectTurretEntry.Value) return;

            //RANDOM PERCENT

            var randomPercent = Random.Range(0f, 100f);

            if (RandomEnemiesSize.instance.randomPercentChanceEntry.Value < randomPercent)
            {
                if (RandomEnemiesSize.instance.devLogEntry.Value)
                    Debug.Log(
                        $"RANDOM PERCENT NOT RANDOM SIZE : {randomPercent} FOR ENEMY {__instance.gameObject.name}");
                return;
            }

            var scale = Random.Range(RandomEnemiesSize.instance.minSizeTurretEntry.Value,
                RandomEnemiesSize.instance.maxSizeTurretEntry.Value);

            var networkObject = __instance.gameObject.GetComponentInParent<NetworkObject>();

            var originalScale = networkObject.transform.localScale;
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

            NetworkSize.UpdateTurretClientRpc(networkObject.NetworkObjectId, newScale, scale, influences);
        }
    }
}