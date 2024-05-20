using System.Collections.Generic;
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
        public static void UpdateEnemyClientRpc(ulong networkId, Vector3 newScale, float scaleMultiplier,
            Influences influences)
        {
            var enemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None).ToList();
            var enemieFound = enemies.Find(e => e.NetworkObjectId == networkId);

            if (enemieFound == null)
            {
                Debug.Log($"ENEMIE NOT FOUND {networkId}");
            }
            else
            {
                Debug.Log(
                    $"ENEMIE FOUND {enemieFound.gameObject.name} NEW SCALE IS {newScale} WITH A MULTIPLIER OF {scaleMultiplier}");
                //change scale
                enemieFound.transform.localScale = newScale;
                //change hp
                enemieFound.enemyHP = influences.InfluenceHp(enemieFound.enemyHP, scaleMultiplier);
                //change pitch
                var audioSources = new List<AudioSource>();
                audioSources.AddRange(enemieFound.GetComponents<AudioSource>());
                audioSources.AddRange(enemieFound.GetComponentsInChildren<AudioSource>());
                var value = 1f - (scaleMultiplier - 1);
                Debug.Log($"PITCH VALUE {value}");
                var pitch = Mathf.Clamp(value, 0.1f, 0.8f);
                enemieFound.creatureVoice.pitch = pitch;
                enemieFound.creatureSFX.pitch = pitch;
                foreach (var audioSource in audioSources) audioSource.pitch = pitch;
            }
        }
    }
}