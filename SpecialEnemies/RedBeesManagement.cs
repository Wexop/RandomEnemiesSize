using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace RandomEnemiesSize.SpecialEnemies
{
    public class RedBeesManagement
    {

        public static RedBeesManagement instance;

        public Dictionary<ulong, RedBees> BeesDictionary = new Dictionary<ulong, RedBees>();

        public static void Init()
        {
            if (instance == null)
            {
                instance = new RedBeesManagement();
            }
        }
        public static void ChangeSize(EnemyAI enemyAI, float scaleMultiplier)
        {
            
            var redLocustBees = enemyAI.GetComponent<RedLocustBees>();
            
            if (scaleMultiplier > 1)
            {
                redLocustBees.defenseDistance = Mathf.RoundToInt(redLocustBees.defenseDistance * scaleMultiplier);
            }

            var visualEffect = enemyAI.GetComponentInChildren<VisualEffect>();

            if (visualEffect != null)
            {
                RedBees redBees = new RedBees();
                redBees.baseScale = visualEffect.transform.localScale;
                
                visualEffect.transform.localScale *= scaleMultiplier;
                
                redBees.SizedScale = visualEffect.transform.localScale;
                redBees.GameObject = visualEffect.gameObject;
                redBees.multiplier = scaleMultiplier;
                
                if (instance.BeesDictionary.ContainsKey(redLocustBees.NetworkObjectId))
                {
                    instance.BeesDictionary.Remove(redLocustBees.NetworkObjectId);
                }
                
                instance.BeesDictionary.Add(redLocustBees.NetworkObjectId, redBees);
            }
            



            if (redLocustBees != null) redLocustBees.StartCoroutine(ChangeHiveSize(redLocustBees, scaleMultiplier));
        }

        public static IEnumerator ChangeHiveSize(RedLocustBees redLocustBees, float multiplier)
        {
            yield return new WaitUntil(() => redLocustBees.hive != null);

            redLocustBees.hive.gameObject.transform.localScale *= multiplier;
            
            var physicsProp = redLocustBees.hive.GetComponent<PhysicsProp>();
            var cloneHide = Object.Instantiate(physicsProp.itemProperties);

            if (RandomEnemiesSize.instance.influenceBeehiveEntry.Value)
            {

                if (multiplier > 1)
                {
                    cloneHide.weight = 1 + (multiplier * 0.1f);
                }
                
                physicsProp.SetScrapValue(Mathf.RoundToInt(physicsProp.scrapValue * multiplier));
            }
            physicsProp.originalScale = redLocustBees.hive.gameObject.transform.localScale;
            physicsProp.itemProperties = cloneHide;
        }

        public void TargetPlayerRescale(ulong networkId)
        {
            var bees = instance.BeesDictionary[networkId];
            if(bees == null) return;
            bees.GameObject.transform.localScale = bees.baseScale;
        }

        public void StopTargetingPlayerRescale(ulong networkId)
        {
            var bees = instance.BeesDictionary[networkId];
            if(bees == null) return;
            bees.GameObject.transform.localScale = bees.SizedScale;
        }
    }
}