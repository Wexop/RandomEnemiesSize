using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using Object = UnityEngine.Object;

namespace RandomEnemiesSize.SpecialEnemies
{
    public class GiantKiwiManagement
    {

        public static GiantKiwiManagement instance;
        
        public static void Init()
        {
            if (instance == null)
            {
                instance = new GiantKiwiManagement();
            }
        }
        public static void ChangeSize(EnemyAI enemyAI, float scaleMultiplier, Influences influences)
        {
            
            var giantKiwi = enemyAI.GetComponent<GiantKiwiAI>();

            if (giantKiwi != null) giantKiwi.StartCoroutine(ChangeEggsSize(giantKiwi, scaleMultiplier, influences));

        }

        public static IEnumerator ChangeEggsSize(GiantKiwiAI giantKiwi, float multiplier, Influences influences)
        {
            yield return new WaitUntil(() => giantKiwi.eggs.Count > 0);

            var nest = giantKiwi.birdNest;
            nest.gameObject.transform.localScale *= multiplier;
            
            giantKiwi.eggs.ForEach(egg =>
            {
                egg.gameObject.transform.localScale *= multiplier;

                var physicsProp = egg;
                var cloneEgg = Object.Instantiate(physicsProp.itemProperties);
                
                if (RandomEnemiesSize.instance.influenceSapsuckersEggsEntry.Value)
                {

                    cloneEgg.weight = Math.Max(1 + (multiplier * 0.1f), 0);
                
                    physicsProp.SetScrapValue(Mathf.RoundToInt(egg.scrapValue * multiplier));
                    
                    influences.InfluenceMapHazardSound(egg.gameObject, multiplier);
                    
                }
                
                physicsProp.originalScale = egg.gameObject.transform.localScale;
                physicsProp.itemProperties = cloneEgg;
            });


        }


    }
}