using System;
using System.Collections.Generic;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RandomEnemiesSize.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    public class PatchRoundManager
    {

	    [HarmonyPatch("SpawnEnemyGameObject")]
	    [HarmonyPrefix]
	    public static bool SpawnEnemyGameObjectPatch(RoundManager __instance, ref NetworkObjectReference __result, Vector3 spawnPosition, float yRot, int enemyNumber, EnemyType enemyType = null)
        {
            //inside enemies
            Debug.Log("ENEMY SPAWNED FROM PATCH SpawnEnemyGameObjectPatch !");
            
            if (!__instance.IsServer)
            {
	            __result = __instance.currentLevel.Enemies[0].enemyType.enemyPrefab.GetComponent<NetworkObject>();
	            return true;
            }
            if (enemyType != null)
            {
	            //CHANGES ARE HERE

	            GameObject baseObject = enemyType.enemyPrefab.gameObject;
	            baseObject.GetComponent<EnemyAI>().enemyHP = 353;;
	            baseObject.transform.localScale *= 6;

	            Debug.Log("SPAWNED WITH BASEOBJECT 0");
	            
	            GameObject gameObject = UnityEngine.Object.Instantiate(baseObject, spawnPosition, Quaternion.Euler(new Vector3(0f, yRot, 0f)));
	            gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
	            __instance.SpawnedEnemies.Add(gameObject.GetComponent<EnemyAI>());
	            __result = gameObject.GetComponentInChildren<NetworkObject>();
	            return true;
            }
            int index = enemyNumber;
            switch (enemyNumber)
            {
	            case -1:
		            index = UnityEngine.Random.Range(0, __instance.currentLevel.Enemies.Count);
		            break;
	            case -2:
	            {
		            
		            //CHANGES ARE HERE

		            GameObject baseObject1 = __instance.currentLevel.DaytimeEnemies[UnityEngine.Random.Range(0, __instance.currentLevel.DaytimeEnemies.Count)].enemyType.enemyPrefab.gameObject;
		            baseObject1.GetComponent<EnemyAI>().enemyHP = 353;;
		            baseObject1.transform.localScale *= 6;
		            
		            Debug.Log("SPAWNED WITH BASEOBJECT 1");

		            
		            GameObject gameObject3 = UnityEngine.Object.Instantiate(baseObject1, spawnPosition, Quaternion.Euler(new Vector3(0f, yRot, 0f)));
		            gameObject3.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
		            __instance.SpawnedEnemies.Add(gameObject3.GetComponent<EnemyAI>());
		            __result = gameObject3.GetComponentInChildren<NetworkObject>();
		            return true;
	            }
	            case -3:
	            {
		            //CHANGES ARE HERE

		            GameObject baseObject2 = __instance.currentLevel.OutsideEnemies[UnityEngine.Random.Range(0, __instance.currentLevel.OutsideEnemies.Count)].enemyType.enemyPrefab.gameObject;
		            baseObject2.GetComponent<EnemyAI>().enemyHP = 353;;
		            baseObject2.transform.localScale *= 6;
		            
		            Debug.Log("SPAWNED WITH BASEOBJECT 2");

		            
		            GameObject gameObject2 = UnityEngine.Object.Instantiate(baseObject2, spawnPosition, Quaternion.Euler(new Vector3(0f, yRot, 0f)));
		            gameObject2.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
		            __instance.SpawnedEnemies.Add(gameObject2.GetComponent<EnemyAI>());
		            __result = gameObject2.GetComponentInChildren<NetworkObject>();
		            return true;
	            }
            }

            GameObject baseObject3 = __instance.currentLevel.Enemies[index].enemyType.enemyPrefab.gameObject;
            baseObject3.GetComponent<EnemyAI>().enemyHP = 353;;
            baseObject3.transform.localScale *= 6;
            
            Debug.Log("SPAWNED WITH BASEOBJECT 3");

            
            GameObject gameObject4 = UnityEngine.Object.Instantiate(baseObject3, spawnPosition, Quaternion.Euler(new Vector3(0f, yRot, 0f)));
            gameObject4.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
            __instance.SpawnedEnemies.Add(gameObject4.GetComponent<EnemyAI>());
            __result = gameObject4.GetComponentInChildren<NetworkObject>();
            return true;

        }
        
        public static bool SpawnEnemyOutsidePatch(RoundManager __instance, GameObject[] spawnPoints, float timeUpToCurrentHour)
        {
            //outside enemies
            Debug.Log("ENEMY SPAWNED FROM PATCH SpawnEnemyOutsidePatch !");
	            
	        List<int> SpawnProbabilities =
	            Traverse.Create(__instance).Field("SpawnProbabilities").GetValue() as List<int>;
	        
	        Nullable<bool> firstTimeSpawningOutsideEnemiesValue = Traverse.Create(__instance).Field("firstTimeSpawningOutsideEnemies").GetValue() as Nullable<bool>;

	        var firstTimeSpawningOutsideEnemies = firstTimeSpawningOutsideEnemiesValue.Value;
	            
	        SpawnProbabilities.Clear();
			int num = 0;
			for (int i = 0; i < __instance.currentLevel.OutsideEnemies.Count; i++)
			{
				EnemyType enemyType = __instance.currentLevel.OutsideEnemies[i].enemyType;
				if (firstTimeSpawningOutsideEnemies)
				{
					enemyType.numberSpawned = 0;
				}
				if (enemyType.PowerLevel > __instance.currentMaxOutsidePower - __instance.currentOutsideEnemyPower || enemyType.numberSpawned >= enemyType.MaxCount || enemyType.spawningDisabled)
				{
					SpawnProbabilities.Add(0);
					continue;
				}
				int num2 = ((__instance.increasedOutsideEnemySpawnRateIndex == i) ? 100 : ((!enemyType.useNumberSpawnedFalloff) ? ((int)((float)__instance.currentLevel.OutsideEnemies[i].rarity * enemyType.probabilityCurve.Evaluate(timeUpToCurrentHour / __instance.timeScript.totalTime))) : ((int)((float)__instance.currentLevel.OutsideEnemies[i].rarity * (enemyType.probabilityCurve.Evaluate(timeUpToCurrentHour / __instance.timeScript.totalTime) * enemyType.numberSpawnedFalloff.Evaluate((float)enemyType.numberSpawned / 10f))))));
				SpawnProbabilities.Add(num2);
				num += num2;
			}
			Traverse.Create(__instance).Field("firstTimeSpawningOutsideEnemies").SetValue(false);
			if (num <= 0)
			{
				_ = __instance.currentOutsideEnemyPower;
				_ = __instance.currentMaxOutsidePower;
				return false;
			}
			bool result = false;
			int randomWeightedIndex = __instance.GetRandomWeightedIndex(SpawnProbabilities.ToArray(), __instance.OutsideEnemySpawnRandom);
			EnemyType enemyType2 = __instance.currentLevel.OutsideEnemies[randomWeightedIndex].enemyType;
			if (enemyType2.requireNestObjectsToSpawn)
			{
				bool flag = false;
				EnemyAINestSpawnObject[] array = UnityEngine.Object.FindObjectsByType<EnemyAINestSpawnObject>(FindObjectsSortMode.None);
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].enemyType == enemyType2)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			float num3 = Mathf.Max(enemyType2.spawnInGroupsOf, 1);
			for (int k = 0; (float)k < num3; k++)
			{
				if (enemyType2.PowerLevel > __instance.currentMaxOutsidePower - __instance.currentOutsideEnemyPower)
				{
					break;
				}
				__instance.currentOutsideEnemyPower += __instance.currentLevel.OutsideEnemies[randomWeightedIndex].enemyType.PowerLevel;
				Vector3 position = spawnPoints[__instance.AnomalyRandom.Next(0, spawnPoints.Length)].transform.position;
				position = __instance.GetRandomNavMeshPositionInBoxPredictable(position, 10f, default(NavMeshHit), __instance.AnomalyRandom, __instance.GetLayermaskForEnemySizeLimit(enemyType2));
				position = __instance.PositionWithDenialPointsChecked(position, spawnPoints, enemyType2);
				
				//CHANGES ARE HERE

				GameObject baseObject = enemyType2.enemyPrefab.gameObject;
				baseObject.GetComponent<EnemyAI>().enemyHP = 353;;
					
				baseObject.transform.localScale *= 6;

					
				Debug.Log($"SPAWNED ENEMY {baseObject.name} WITH SCALE {baseObject.GetComponent<EnemyAI>().enemyHP} FROM ROUND MANAGER");

				GameObject gameObject = UnityEngine.Object.Instantiate(baseObject, position, Quaternion.Euler(Vector3.zero));
				gameObject.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
				
				__instance.SpawnedEnemies.Add(gameObject.GetComponent<EnemyAI>());
				gameObject.GetComponent<EnemyAI>().enemyType.numberSpawned++;
				result = true;
			}
			Debug.Log("Spawned enemy: " + enemyType2.enemyName);
			return result;
            
            
        }
        
        public static bool SpawnRandomDaytimeEnemyPatch(RoundManager __instance, GameObject[] spawnPoints, float timeUpToCurrentHour)
        {
            //daytime enemies
            //CODE COPIED FROM GAME CODE, ONLY SPAWN LOGIC CHANGE
            Debug.Log("ENEMY SPAWNED FROM PATCH SpawnRandomDaytimeEnemy!");

            List<int> SpawnProbabilities =
	            Traverse.Create(__instance).Field("SpawnProbabilities").GetValue() as List<int>;
            
            Nullable<bool> firstTimeSpawningDaytimeEnemiesValue = Traverse.Create(__instance).Field("firstTimeSpawningDaytimeEnemies").GetValue() as Nullable<bool>;

            var firstTimeSpawningDaytimeEnemies = firstTimeSpawningDaytimeEnemiesValue.Value;
            
            Debug.Log("HERE 1");

            SpawnProbabilities.Clear();
			int num = 0;
			for (int i = 0; i < __instance.currentLevel.DaytimeEnemies.Count; i++)
			{
				EnemyType enemyType = __instance.currentLevel.DaytimeEnemies[i].enemyType;
				if (firstTimeSpawningDaytimeEnemies)
				{
					enemyType.numberSpawned = 0;
				}
				if (enemyType.PowerLevel > (float)__instance.currentLevel.maxDaytimeEnemyPowerCount - __instance.currentDaytimeEnemyPower || enemyType.numberSpawned >= __instance.currentLevel.DaytimeEnemies[i].enemyType.MaxCount || enemyType.normalizedTimeInDayToLeave < TimeOfDay.Instance.normalizedTimeOfDay || enemyType.spawningDisabled)
				{
					SpawnProbabilities.Add(0);
					continue;
				}
				int num2 = (int)((float)__instance.currentLevel.DaytimeEnemies[i].rarity * enemyType.probabilityCurve.Evaluate(timeUpToCurrentHour / __instance.timeScript.totalTime));
				SpawnProbabilities.Add(num2);
				num += num2;
			}
			
			Traverse.Create(__instance).Field("firstTimeSpawningDaytimeEnemies").SetValue(false);
			
			Debug.Log("HERE 2");
			
			if (num <= 0)
			{
				_ = __instance.currentDaytimeEnemyPower;
				_ = (float)__instance.currentLevel.maxDaytimeEnemyPowerCount;
				return false;
			}
			int randomWeightedIndex = __instance.GetRandomWeightedIndex(SpawnProbabilities.ToArray(), __instance.EnemySpawnRandom);
			EnemyType enemyType2 = __instance.currentLevel.DaytimeEnemies[randomWeightedIndex].enemyType;
			bool result = false;
			float num3 = Mathf.Max(enemyType2.spawnInGroupsOf, 1);
			for (int j = 0; (float)j < num3; j++)
			{
				if (enemyType2.PowerLevel > (float)__instance.currentLevel.maxDaytimeEnemyPowerCount - __instance.currentDaytimeEnemyPower)
				{
					Debug.Log("HERE 3");
					break;
				}
				__instance.currentDaytimeEnemyPower += __instance.currentLevel.DaytimeEnemies[randomWeightedIndex].enemyType.PowerLevel;
				Vector3 position = spawnPoints[__instance.AnomalyRandom.Next(0, spawnPoints.Length)].transform.position;
				position = __instance.GetRandomNavMeshPositionInBoxPredictable(position, 10f, default(NavMeshHit), __instance.EnemySpawnRandom, __instance.GetLayermaskForEnemySizeLimit(enemyType2));
				position = __instance.PositionWithDenialPointsChecked(position, spawnPoints, enemyType2);
				
				//CHANGES ARE HERE

				GameObject baseObject = enemyType2.enemyPrefab.gameObject;
				EnemyAI enemyAI = baseObject.GetComponent<EnemyAI>();
				
				baseObject.transform.localScale *= 6;
				enemyAI.enemyHP = 353;
				
				Debug.Log($"SPAWNED ENEMY {baseObject.name} WITH SCALE {enemyAI.enemyHP} FROM ROUND MANAGER");
				
				GameObject gameObject = UnityEngine.Object.Instantiate(baseObject, position, Quaternion.Euler(Vector3.zero));

				//CHANGE END HERE

				gameObject.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
				__instance.SpawnedEnemies.Add(gameObject.GetComponent<EnemyAI>());
				gameObject.GetComponent<EnemyAI>().enemyType.numberSpawned++;
				result = true;
			}
			return result;
            
        }
    }
}