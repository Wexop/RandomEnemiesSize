
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

                var scale = Random.Range(RandomEnemiesSize.instance.minSizeEntry.Value, RandomEnemiesSize.instance.maxSizeEntry.Value);
                
                __instance.gameObject.GetComponent<NetworkObject>().Despawn(destroy: false);
                __instance.gameObject.transform.localScale = new Vector3(1, 1, 1) * scale ;
                __instance.gameObject.GetComponent<NetworkObject>().Spawn();

                
                Debug.Log($"ENEMY ({__instance.gameObject.name}) SPAWNED WITH RANDOM SIZE {scale}");
        }
        
    }
    
}