using HarmonyLib;
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

            var isVannila = new VanillaEnemies().IsAVanillaEnemy(__instance.enemyType.enemyName);

            if (isVannila && !RandomEnemiesSize.instance.CustomAffectVanillaEntry.Value ||
                !isVannila && !RandomEnemiesSize.instance.CustomAffectModEntry.Value) return;


            var scale = Random.Range(RandomEnemiesSize.instance.minSizeOutdoorEntry.Value,
                RandomEnemiesSize.instance.maxSizeOutdoorEntry.Value);

            if (!__instance.isOutside)
                scale = Random.Range(RandomEnemiesSize.instance.minSizeIndoorEntry.Value,
                    RandomEnemiesSize.instance.maxSizeIndoorEntry.Value);

            var customEnemy = RandomEnemiesSize.instance.GetCustomEnemySize(__instance.enemyType.enemyName);
            if (customEnemy.found) scale = Random.Range(customEnemy.minValue, customEnemy.maxValue);


            if (RandomEnemiesSize.instance.LethalLevelLoaderIsHere)
            {
                var interiorName = RandomEnemiesSize.GetDungeonName();
                //Debug.Log($"ACTUAL DUNGEON NAME {interiorName}");
                if (!__instance.isOutside && interiorName != null)
                {
                    var interiorMult = RandomEnemiesSize.instance.GetInteriorMultiplier(__instance.enemyType.enemyName,
                        interiorName);
                    //Debug.Log($"BEFORE INTERIOR MULT, SCALE IS {scale} AND MULT IS {interiorMult}");
                    scale *= interiorMult;

                    //Debug.Log($"AFTER INTERIOR MULT, SCALE IS {scale} ");
                }
            }


            //server dispawn gameobject, change scale, and respawn it to sync with clients

            var originalScale = __instance.gameObject.transform.localScale;
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

            NetworkSize.UpdateEnemyClientRpc(__instance.NetworkObjectId, newScale, scale, influences);


            Debug.Log($"ENEMY ({__instance.gameObject.name}) SPAWNED WITH RANDOM SIZE {newScale.ToString()}");
        }
    }
}