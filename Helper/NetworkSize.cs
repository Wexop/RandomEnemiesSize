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
                Debug.Log($"ENEMIE NOT FOUND {networkId}");
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

                if (enemieFound.enemyType.enemyName == "Red Locust Bees")
                    new RedBeesManagement().ChangeSize(enemieFound, scaleMultiplier);
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
                Debug.Log($"TURRET NOT FOUND {networkId}");
            }
            else
            {
                Debug.Log($"TURRET WITH NEW SCALE : {newScale}");
                turretFound.transform.localScale = newScale;
                influences.InfluenceTurretSound(turretFound.GetComponentInChildren<Turret>(), scaleMultiplier);
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
                Debug.Log($"LANDMINE NOT FOUND {networkId}");
            }
            else
            {
                Debug.Log($"LANDMINE WITH NEW SCALE : {newScale}");
                mineFound.transform.localScale = newScale;
                influences.InfluenceMineSound(mineFound.GetComponentInChildren<Landmine>(), scaleMultiplier);
            }
        }
    }
}