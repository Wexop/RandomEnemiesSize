using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace RandomEnemiesSize.SpecialEnemies
{
    public class RedBeesManagement
    {
        public void ChangeSize(EnemyAI enemyAI, float scaleMultiplier)
        {
            var redLocustBees = enemyAI.GetComponent<RedLocustBees>();

            var visualEffect = enemyAI.GetComponentInChildren<VisualEffect>();

            Debug.Log("FOUND RED BEES");

            if (visualEffect != null) visualEffect.transform.localScale *= scaleMultiplier;


            if (redLocustBees != null) redLocustBees.StartCoroutine(ChangeHiveSize(redLocustBees, scaleMultiplier));
        }

        public IEnumerator ChangeHiveSize(RedLocustBees redLocustBees, float multiplier)
        {
            Debug.Log("FOUND RED BEES SCRIPT");

            yield return new WaitUntil(() => redLocustBees.hive != null);

            Debug.Log("FOUND HIVE");
            redLocustBees.hive.gameObject.transform.localScale *= multiplier;
            var physicsProp = redLocustBees.hive.GetComponent<PhysicsProp>();
            physicsProp.originalScale = redLocustBees.hive.gameObject.transform.localScale;
        }
    }
}