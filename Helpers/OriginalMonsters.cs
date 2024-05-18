using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RandomEnemiesSize.Helpers
{
    public class OriginalMonsters
    {

        public List<MonsterDetails> MonsterDetailsList;

        public void Init()
        {
            
            foreach (var networkPrefabsList in NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists)
            {
                foreach (var prefab in networkPrefabsList.PrefabList)
                {
                    var gameobject = prefab.Prefab;
                    var enemyAI = gameobject.GetComponent<EnemyAI>();
                    if ( enemyAI != null)
                    {
                        var monster = new MonsterDetails();
                        monster.scale = gameobject.transform.localScale.x;
                        monster.hp = enemyAI.enemyHP;
                        monster.name = enemyAI.enemyType.enemyName;

                        MonsterDetailsList.Add(monster);
                    }
                   
                    
                }
            }
        }

        public Nullable<float> GetMultiplierFromScale(string name, float scale)
        {

            foreach (var monster in MonsterDetailsList)
            {
                if (monster.name == name)
                {
                    return scale / monster.scale;
                }
            }
            
            return null;
        }

        public int GetMonsterHpInfluenced(float multiplier, int originalHp)
        {
            var hp = multiplier;
            if (hp <= 0)
            {
                hp = 1;
            }

            return Mathf.FloorToInt(originalHp * multiplier);
        }
    }
}