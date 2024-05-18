
using HarmonyLib;
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
            
                if (!__instance.IsServer || !__instance.IsOwner) return;
                

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

                var originalScale = __instance.gameObject.transform.localScale;
                
                __instance.gameObject.GetComponent<NetworkObject>().Despawn(destroy: false);
                __instance.gameObject.transform.localScale = originalScale * scale ;
                __instance.gameObject.GetComponent<NetworkObject>().Spawn();

                
                Debug.Log($"ENEMY ({__instance.gameObject.name}) SPAWNED WITH RANDOM SIZE {scale}");
        }
        
    }
    
}