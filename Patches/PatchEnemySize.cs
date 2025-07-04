﻿using HarmonyLib;
using UnityEngine;

namespace RandomEnemiesSize.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class PatchEnemySize
    {

        [HarmonyPatch(typeof(MaskedPlayerEnemy))]
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void PatchStartMasked(MaskedPlayerEnemy __instance)
        {
            PatchStart(__instance);
        }
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void PatchStart(EnemyAI __instance)
        {
            if (!__instance.IsServer || !__instance.IsOwner) return;

            var isVannila = new VanillaEnemies().IsAVanillaEnemy(__instance.enemyType.enemyName);

            if (isVannila && !RandomEnemiesSize.instance.CustomAffectVanillaEntry.Value ||
                !isVannila && !RandomEnemiesSize.instance.CustomAffectModEntry.Value) return;

            //RANDOM PERCENT
            var randomPercent = Random.Range(0f, 100f);

            if (RandomEnemiesSize.instance.randomPercentChanceEntry.Value < randomPercent)
            {
                if (RandomEnemiesSize.instance.devLogEntry.Value)
                    Debug.Log(
                        $"RANDOM PERCENT NOT RANDOM SIZE : {randomPercent} FOR ENEMY {__instance.gameObject.name}");
                return;
            }


            var scale = Random.Range(RandomEnemiesSize.instance.minSizeOutdoorEntry.Value,
                RandomEnemiesSize.instance.maxSizeOutdoorEntry.Value);

            if (!__instance.isOutside)
                scale = Random.Range(RandomEnemiesSize.instance.minSizeIndoorEntry.Value,
                    RandomEnemiesSize.instance.maxSizeIndoorEntry.Value);

            var customEnemy = RandomEnemiesSize.instance.GetCustomEnemySize(__instance.enemyType.enemyName);
            if (customEnemy.found) scale = Random.Range(customEnemy.minValue, customEnemy.maxValue);

            if (RandomEnemiesSize.instance.devLogEntry.Value)
                Debug.Log(
                    $"PLANET NAME : {StartOfRound.Instance.currentLevel.PlanetName}");

            var customMoonEnemy = RandomEnemiesSize.instance.GetCustomMoonEnemySize(__instance.enemyType.enemyName, StartOfRound.Instance.currentLevel.PlanetName);
            if (customMoonEnemy.found) scale = Random.Range(customMoonEnemy.minValue, customMoonEnemy.maxValue);


            if (RandomEnemiesSize.instance.LethalLevelLoaderIsHere)
            {
                var interiorName = RandomEnemiesSize.GetDungeonName();
                //Debug.Log($"ACTUAL DUNGEON NAME {interiorName}");
                if (!__instance.isOutside && interiorName != null)
                {
                    var interiorMult = RandomEnemiesSize.instance.GetInteriorMultiplier(__instance.enemyType.enemyName,
                        interiorName);
                    //Debug.Log($"BEFORE INTERIOR MULT, SCALE IS {scale} AND MULT IS {interiorMult}");
                    scale *= interiorMult;

                    //Debug.Log($"AFTER INTERIOR MULT, SCALE IS {scale} ");
                }
            }

            var originalScale = __instance.gameObject.transform.localScale;
            var newScale = originalScale * scale;

            if (RandomEnemiesSize.instance.funModeEntry.Value)
            {
                var funXSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value,
                    RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);
                var funZSize = Random.Range(RandomEnemiesSize.instance.funModeHorizontalMinEntry.Value,
                    RandomEnemiesSize.instance.funModeHorizontalMaxEntry.Value);

                if (RandomEnemiesSize.instance.funModeLockHorizontalEnrty.Value) funZSize = funXSize;

                newScale = new Vector3(newScale.x * funXSize, newScale.y, newScale.z * funZSize);
            }

            //changes on every client

            var influences = new Influences();
            influences.GetInfos();

            NetworkSize.UpdateEnemyClientRpc(__instance.NetworkObjectId, newScale, scale, influences);


        }
    }
}