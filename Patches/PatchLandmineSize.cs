using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace RandomEnemiesSize.Patches
{
    [HarmonyPatch(typeof(Landmine))]
    public class PatchLandmineSize
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void PatchStart(Landmine __instance)
        {
            if (!__instance.IsServer || !__instance.IsOwner) return;

            if (!RandomEnemiesSize.instance.customAffectMineEntry.Value) return;

            var scale = Random.Range(RandomEnemiesSize.instance.minSizeMineEntry.Value,
                RandomEnemiesSize.instance.maxSizeMineEntry.Value);

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

            NetworkSize.UpdateLandMineClientRpc(networkObject.NetworkObjectId, newScale, scale, influences);
        }
    }
}