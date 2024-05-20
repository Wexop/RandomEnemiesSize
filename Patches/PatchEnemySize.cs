
using HarmonyLib;
using LethalLevelLoader;
using Unity.Netcode;
using UnityEngine;

namespace RandomEnemiesSize.Patches
{
    [HarmonyPatch(typeof(EnemyAI) )]
    internal class PatchEnemySize
    {

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void PatchStart(EnemyAI __instance)
        {

            var originalMonsters = RandomEnemiesSize.instance.OriginalMonsters;
            var originalScale = __instance.gameObject.transform.localScale;
            
            //if we are client
            if (!__instance.IsServer || !__instance.IsOwner)
            {
                var multiplier = originalMonsters.GetMultiplierFromScale(__instance.enemyType.enemyName, originalScale.x);

                if (multiplier.HasValue)
                {
                    __instance.enemyHP = originalMonsters.GetMonsterHpInfluenced(multiplier.Value, __instance.enemyHP);
                }
                Debug.Log($"CLIENT GET ENEMY HP {__instance.enemyHP}");
                return;
            }
                

            var scale = Random.Range(RandomEnemiesSize.instance.minSizeOutdoorEntry.Value, RandomEnemiesSize.instance.maxSizeOutdoorEntry.Value);

            if (!__instance.isOutside)
            {
                scale = Random.Range(RandomEnemiesSize.instance.minSizeIndoorEntry.Value, RandomEnemiesSize.instance.maxSizeIndoorEntry.Value);
            }

            var customEnemy = RandomEnemiesSize.instance.GetCustomEnemySize(__instance.enemyType.enemyName);
            if (customEnemy.found)
            {
                scale = Random.Range(customEnemy.minValue, customEnemy.maxValue);
            }
            
            //server dispawn gameobject, change scale, and respawn it to sync with clients


            var newScale = originalScale * scale;

            if (RandomEnemiesSize.instance.funModeEntry.Value)
            {
                
                var funXSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value, RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);
                var funZSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value, RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);
                
                newScale = new Vector3(newScale.x * funXSize, newScale.y, newScale.z * funZSize);
            }
            
            __instance.gameObject.GetComponent<NetworkObject>().Despawn(destroy: false);
            
            //change size
            __instance.gameObject.transform.localScale = newScale ;
            
            //change hp
            if (RandomEnemiesSize.instance.influenceHpConfig.Value)
            {
                var m = originalMonsters.GetMultiplierFromScale(__instance.enemyType.enemyName, originalScale.x);
                if (m.HasValue)
                {
                    __instance.enemyHP = originalMonsters.GetMonsterHpInfluenced(m.Value, __instance.enemyHP);
                    Debug.Log($"SERVER GET ENEMY HP {__instance.enemyHP}");
                }

                if (!__instance.isOutside)
                {
                    
                    //Debug.Log($"ACTUAL DUNGEON NAME {DungeonManager.CurrentExtendedDungeonFlow.DungeonName}");

                    scale *= RandomEnemiesSize.instance.GetInteriorMultiplier(__instance.enemyType.enemyName,
                        DungeonManager.CurrentExtendedDungeonFlow.DungeonName);
                }

                //server dispawn gameobject, change scale, and respawn it to sync with clients


                if (RandomEnemiesSize.instance.funModeEntry.Value)
                {
                    
                    var funXSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value, RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);
                    var funZSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value, RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);

                    if (RandomEnemiesSize.instance.lockFunModeHorizontalEnrty.Value)
                    {
                        funZSize = funXSize;
                    }
                    
                    newScale = new Vector3(newScale.x * funXSize, newScale.y, newScale.z * funZSize);
                }
                
                __instance.gameObject.GetComponent<NetworkObject>().Despawn(destroy: false);
                
                //change size
                __instance.gameObject.transform.localScale = newScale ;

                __instance.gameObject.GetComponent<NetworkObject>().Spawn();
                

                
                Debug.Log($"ENEMY ({__instance.gameObject.name}) SPAWNED WITH RANDOM SIZE {newScale.ToString()}");
        }
        
    }
    
}