using System.Linq;
using StaticNetcodeLib;
using Unity.Netcode;
using UnityEngine;

namespace RandomEnemiesSize
{
    [StaticNetcode]
    public class NetworkSize
    {
        [ClientRpc]
        public static void UpdateEnemyClientRpc(ulong networkId, Vector3 newScale, float scaleMultiplier)
        {
            var enemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None).ToList();
            var enemieFound = enemies.Find(e => e.NetworkObjectId == networkId);

            if (enemieFound == null)
            {
                Debug.Log($"ENEMIE NOT FOUND {networkId}");
            }
            else
            {
                Debug.Log($"ENEMIE FOUND {enemieFound.gameObject.name} NEW SCALE IS {newScale}");
                enemieFound.transform.localScale = newScale;
            }
        }
    }
}