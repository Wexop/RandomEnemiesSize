using System.Linq;
using RandomEnemiesSize.SpecialEnemies;
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
                if (RandomEnemiesSize.instance.devLogEntry.Value) Debug.Log($"ENEMIE NOT FOUND {networkId}");
            }
            else
            {
                //Debug.Log($"ENEMIE FOUND {enemieFound.gameObject.name} NEW SCALE IS {newScale} WITH A MULTIPLIER OF {scaleMultiplier}");
                //change scale
                enemieFound.transform.localScale = newScale;
                //change hp
                enemieFound.enemyHP = influences.InfluenceHp(enemieFound.enemyHP, scaleMultiplier);
                //change pitch
                influences.InfluenceSound(enemieFound, scaleMultiplier);
                
                //STORE ENEMY FOR OTHERS MODS COMPATIBILITY

                EnemyResized enemyResized = new EnemyResized();
                enemyResized.influences = influences;
                enemyResized.isHazard = false;
                enemyResized.newtorkId = networkId;
                enemyResized.multiplier = scaleMultiplier;
                enemyResized.gameObject = enemieFound.gameObject;
                enemyResized.scale = newScale;
                
                RandomEnemiesSize.instance.RandomEnemiesSizeDataDictionnary.Add(networkId, enemyResized);

                if (enemieFound.enemyType.enemyName == "Red Locust Bees")
                    RedBeesManagement.ChangeSize(enemieFound, scaleMultiplier);
            }
        }

        [ClientRpc]
        public static void UpdateTurretClientRpc(ulong networkId, Vector3 newScale, float scaleMultiplier,
            Influences influences)
        {
            var turrets = Object.FindObjectsByType<Turret>(FindObjectsSortMode.None).ToList();
            var turretObjectFound = turrets.Find(e => e.NetworkObjectId == networkId);
            var turretFound = turretObjectFound.GetComponentInParent<NetworkObject>().gameObject;
            if (turretFound == null)
            {
                if (RandomEnemiesSize.instance.devLogEntry.Value) Debug.Log($"TURRET NOT FOUND {networkId}");
            }
            else
            {
                if (RandomEnemiesSize.instance.devLogEntry.Value) Debug.Log($"TURRET WITH NEW SCALE : {newScale}");
                turretFound.transform.localScale = newScale;
                influences.InfluenceTurretSound(turretFound.GetComponentInChildren<Turret>(), scaleMultiplier);
                
                //STORE ENEMY FOR OTHERS MODS COMPATIBILITY

                EnemyResized enemyResized = new EnemyResized();
                enemyResized.influences = influences;
                enemyResized.isHazard = true;
                enemyResized.newtorkId = networkId;
                enemyResized.multiplier = scaleMultiplier;
                enemyResized.gameObject = turretFound.gameObject;
                enemyResized.scale = newScale;
                
                RandomEnemiesSize.instance.RandomEnemiesSizeDataDictionnary.Add(networkId, enemyResized);
                
            }
        }

        [ClientRpc]
        public static void UpdateLandMineClientRpc(ulong networkId, Vector3 newScale, float scaleMultiplier,
            Influences influences)
        {
            var mines = Object.FindObjectsByType<Landmine>(FindObjectsSortMode.None).ToList();
            var minesObjectFound = mines.Find(e => e.NetworkObjectId == networkId);
            var mineFound = minesObjectFound.GetComponentInParent<NetworkObject>().gameObject;
            if (mineFound == null)
            {
                if (RandomEnemiesSize.instance.devLogEntry.Value) Debug.Log($"LANDMINE NOT FOUND {networkId}");
            }
            else
            {
                if (RandomEnemiesSize.instance.devLogEntry.Value) Debug.Log($"LANDMINE WITH NEW SCALE : {newScale}");
                mineFound.transform.localScale = newScale;
                influences.InfluenceMineSound(mineFound.GetComponentInChildren<Landmine>(), scaleMultiplier);
                
                //STORE ENEMY FOR OTHERS MODS COMPATIBILITY

                EnemyResized enemyResized = new EnemyResized();
                enemyResized.influences = influences;
                enemyResized.isHazard = true;
                enemyResized.newtorkId = networkId;
                enemyResized.multiplier = scaleMultiplier;
                enemyResized.gameObject = mineFound.gameObject;
                enemyResized.scale = newScale;
                
                RandomEnemiesSize.instance.RandomEnemiesSizeDataDictionnary.Add(networkId, enemyResized);
                
            }
        }
        
        [ClientRpc]
        public static void UpdateSpikeTrapClientRpc(ulong networkId, Vector3 newScale, float scaleMultiplier,
            Influences influences)
        {
            var spikeTraps = Object.FindObjectsByType<SpikeRoofTrap>(FindObjectsSortMode.None).ToList();
            var spikeTrapsFoundObject = spikeTraps.Find(e => e.NetworkObjectId == networkId);
            var spikeTrapFound = spikeTrapsFoundObject.GetComponentInParent<NetworkObject>().gameObject;
            if (spikeTrapFound == null)
            {
                if (RandomEnemiesSize.instance.devLogEntry.Value) Debug.Log($"SPIKE TRAP NOT FOUND {networkId}");
            }
            else
            {
                if (RandomEnemiesSize.instance.devLogEntry.Value) Debug.Log($"SPIKE TRAP WITH NEW SCALE : {newScale}");
                spikeTrapFound.transform.localScale = newScale;
                influences.InfluenceSpikeTrapSound(spikeTrapFound.GetComponentInChildren<SpikeRoofTrap>(), scaleMultiplier);
                
                //STORE ENEMY FOR OTHERS MODS COMPATIBILITY

                EnemyResized enemyResized = new EnemyResized();
                enemyResized.influences = influences;
                enemyResized.isHazard = true;
                enemyResized.newtorkId = networkId;
                enemyResized.multiplier = scaleMultiplier;
                enemyResized.gameObject = spikeTrapFound.gameObject;
                enemyResized.scale = newScale;
                
                RandomEnemiesSize.instance.RandomEnemiesSizeDataDictionnary.Add(networkId, enemyResized);
            }
        }
    }
}