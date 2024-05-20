using HarmonyLib;
using LethalLevelLoader;
using UnityEngine;

namespace RandomEnemiesSize.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class PatchEnemySize
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void PatchStart(EnemyAI __instance)
        {
            if (!__instance.IsServer || !__instance.IsOwner) return;


            var scale = Random.Range(RandomEnemiesSize.instance.minSizeOutdoorEntry.Value,
                RandomEnemiesSize.instance.maxSizeOutdoorEntry.Value);

            if (!__instance.isOutside)
                scale = Random.Range(RandomEnemiesSize.instance.minSizeIndoorEntry.Value,
                    RandomEnemiesSize.instance.maxSizeIndoorEntry.Value);

            var customEnemy = RandomEnemiesSize.instance.GetCustomEnemySize(__instance.enemyType.enemyName);
            if (customEnemy.found) scale = Random.Range(customEnemy.minValue, customEnemy.maxValue);

            if (!__instance.isOutside && DungeonManager.CurrentExtendedDungeonFlow?.DungeonName != null)
                //Debug.Log($"ACTUAL DUNGEON NAME {DungeonManager.CurrentExtendedDungeonFlow.DungeonName}");

                scale *= RandomEnemiesSize.instance.GetInteriorMultiplier(__instance.enemyType.enemyName,
                    DungeonManager.CurrentExtendedDungeonFlow.DungeonName);

            //server dispawn gameobject, change scale, and respawn it to sync with clients

            var originalScale = __instance.gameObject.transform.localScale;
            var newScale = originalScale * scale;

            if (RandomEnemiesSize.instance.funModeEntry.Value)
            {
                var funXSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value,
                    RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);
                var funZSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value,
                    RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);

                if (RandomEnemiesSize.instance.lockFunModeHorizontalEnrty.Value) funZSize = funXSize;

                newScale = new Vector3(newScale.x * funXSize, newScale.y, newScale.z * funZSize);
            }

            //changes on every client

            NetworkSize.UpdateEnemyClientRpc(__instance.NetworkObjectId, newScale, scale);


            Debug.Log($"ENEMY ({__instance.gameObject.name}) SPAWNED WITH RANDOM SIZE {newScale.ToString()}");
        }
    }
}