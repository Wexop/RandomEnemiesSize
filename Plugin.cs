﻿using System.Collections.Generic;
using System.Globalization;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalLevelLoader;
using LethalLib.Modules;
using RandomEnemiesSize.Patches;
using RandomEnemiesSize.SpecialEnemies;
using UnityEngine;

namespace RandomEnemiesSize
{
    [BepInDependency(StaticNetcodeLib.StaticNetcodeLib.Guid)]
    [BepInDependency(Plugin.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class RandomEnemiesSize : BaseUnityPlugin
    {
        private const string GUID = "wexop.random_enemies_size";
        private const string NAME = "RandomEnemiesSize";
        private const string VERSION = "1.1.17";

        public static string LethalLevelLoaderReferenceChain = "imabatby.lethallevelloader";

        public static RandomEnemiesSize instance;
        public bool LethalLevelLoaderIsHere;

        public Dictionary<ulong, EnemyResized> RandomEnemiesSizeDataDictionary = new Dictionary<ulong, EnemyResized>();
        
        public ConfigEntry<bool> customAffectMineEntry;
        public ConfigEntry<bool> CustomAffectModEntry;
        public ConfigEntry<bool> customAffectTurretEntry;
        public ConfigEntry<bool> customAffectSpikeTrapEntry;
        public ConfigEntry<bool> customAffectModdedHazardEntry;
        public ConfigEntry<bool> CustomAffectVanillaEntry;
        public ConfigEntry<string> customEnemyEntry;
        public ConfigEntry<string> customMoonEntry;
        public ConfigEntry<string> customInteriorEntry;
        public ConfigEntry<bool> devLogEntry;

        public ConfigEntry<bool> funModeEntry;
        public ConfigEntry<float> funModeHorizontalMaxEntry;
        public ConfigEntry<float> funModeHorizontalMinEntry;
        public ConfigEntry<bool> funModeLockHorizontalEnrty;


        public ConfigEntry<bool> influenceHpConfig;
        public ConfigEntry<bool> influenceSoundConfig;
        public ConfigEntry<float> influenceSoundMaxEntry;
        public ConfigEntry<float> influenceSoundMinEntry;
        public ConfigEntry<bool> influenceBeehiveEntry;

        public ConfigEntry<float> maxSizeIndoorEntry;
        public ConfigEntry<float> maxSizeMineEntry;
        public ConfigEntry<float> maxSizeOutdoorEntry;
        public ConfigEntry<float> maxSizeTurretEntry;
        public ConfigEntry<float> minSizeIndoorEntry;
        public ConfigEntry<float> minSizeMineEntry;
        public ConfigEntry<float> minSizeOutdoorEntry;
        public ConfigEntry<float> minSizeTurretEntry;
        public ConfigEntry<float> minSizeSpikeTrapEntry;
        public ConfigEntry<float> maxSizeSpikeTrapEntry;
        public ConfigEntry<float> minSizeModdedHazardEntry;
        public ConfigEntry<float> maxSizeModdedHazardEntry;

        public ConfigEntry<float> randomPercentChanceEntry;

        private void Awake()
        {
            instance = this;

            Logger.LogInfo("RandomEnemiesSize starting....");

            if (Chainloader.PluginInfos.ContainsKey(LethalLevelLoaderReferenceChain))
            {
                Debug.Log("LethalLevelLoader found !");
                LethalLevelLoaderIsHere = true;
            }

            //GENERAL

            randomPercentChanceEntry = Config.Bind("General", "ChanceOfRandomSize", 100f,
                "The chance for each monster to have a random size. 100% is always, 0% is never. No need to restart the game :)");
            CreateFloatConfig(randomPercentChanceEntry, 0f, 100f);

            minSizeIndoorEntry = Config.Bind("General", "MinMonstersSizeIndoor", 0.4f,
                "Change the minimum size of monsters in the factory. No need to restart the game :)");
            CreateFloatConfig(minSizeIndoorEntry);

            maxSizeIndoorEntry = Config.Bind("General", "MaxMonstersSizeIndoor", 1.5f,
                "Change the maximum size of monsters in the factory. No need to restart the game :)");
            CreateFloatConfig(maxSizeIndoorEntry);

            minSizeOutdoorEntry = Config.Bind("General", "MinMonstersSizeOutdoor", 0.5f,
                "Change the minimum size of monsters outside the factory. No need to restart the game :)");
            CreateFloatConfig(minSizeOutdoorEntry);

            maxSizeOutdoorEntry = Config.Bind("General", "MaxMonstersSizeOutdoor", 3f,
                "Change the maximum size of monsters outside the factory. No need to restart the game :)");
            CreateFloatConfig(maxSizeOutdoorEntry);

            //CUSTOM

            CustomAffectVanillaEntry = Config.Bind("Custom", "AffectVanillaEnemies", true,
                "Activate to make this mod affect vanilla enemies. No need to restart the game :)");
            CreateBoolConfig(CustomAffectVanillaEntry);

            CustomAffectModEntry = Config.Bind("Custom", "AffectModdedEnemies", true,
                "Activate to make this mod affect modded enemies. No need to restart the game :)");
            CreateBoolConfig(CustomAffectModEntry);

            customEnemyEntry = Config.Bind("Custom", "CustomEnemiesSize", "",
                "Custom the size for an enemy wanted with his EXACT name. RECOMMENDED: Go to the thunderstore mod page, you can find a generator to make this config easier. Manual example -> ForestGiant:0.4:5;FlowerMan:0.2:6. Dont forgot the separator ';' between each monsters. No need to restart the game :)");
            CreateStringConfig(customEnemyEntry);

            customInteriorEntry = Config.Bind("Custom", "CustomInteriorsSize", "",
                "THE MOD LethalLevelLoader IS REQUIRED FOR THIS FEATURE. Multiply the base size for an indoor enemy in an interior wanted with his EXACT name. RECOMMENDED: Go to the thunderstore mod page, you can find a generator to make this config easier. Manual example -> mansion#any:1.5,NutCracker:2;customInterior#any:3; No need to restart the game :)");
            CreateInteriorStringConfig(customInteriorEntry);

            customMoonEntry = Config.Bind("Custom", "CustomMoonEnemiesSize", "",
                "Custom the size for an enemy on a specific moon. RECOMMENDED: Go to the thunderstore mod page, you can find a generator to make this config easier. Manual example -> Assurance#any:0.8:2.5,ForestGiant:0.4:5,FlowerMan:0.2:6;Mars#any:1:2,ForestGiant:1:3 . No need to restart the game :)");
            CreateInteriorStringConfig(customMoonEntry);
            
            // turret

            customAffectTurretEntry = Config.Bind("Turret", "AffectTurretSize", true,
                "Activate to make this mod affect turrets size. No need to restart the game :)");
            CreateBoolConfig(customAffectTurretEntry);

            minSizeTurretEntry = Config.Bind("Turret", "MinTurretSize", 0.25f,
                "Change the minimum size of turrets. No need to restart the game :)");
            CreateFloatConfig(minSizeTurretEntry, 0f, 3f);

            maxSizeTurretEntry = Config.Bind("Turret", "MaxTurretSize", 1.5f,
                "Change the maximum size of turrets. No need to restart the game :)");
            CreateFloatConfig(maxSizeTurretEntry, 0f, 3f);
            
            //landmine

            customAffectMineEntry = Config.Bind("Landmine", "AffectLandMineSize", true,
                "Activate to make this mod affect land mines size. No need to restart the game :)");
            CreateBoolConfig(customAffectMineEntry);

            minSizeMineEntry = Config.Bind("Landmine", "MinLandMinesSize", 0.2f,
                "Change the minimum size of land mines. No need to restart the game :)");
            CreateFloatConfig(minSizeMineEntry, 0f, 5f);

            maxSizeMineEntry = Config.Bind("Landmine", "MaxLandMinesSize", 3f,
                "Change the maximum size of land mines. No need to restart the game :)");
            CreateFloatConfig(maxSizeMineEntry, 0f, 5f);
            
            //spike trap
            
            customAffectSpikeTrapEntry = Config.Bind("SpikeTrap", "AffectSpikeTrapSize", true,
                "Activate to make this mod affect spike traps size. No need to restart the game :)");
            CreateBoolConfig(customAffectSpikeTrapEntry);

            minSizeSpikeTrapEntry = Config.Bind("SpikeTrap", "MinSpikeTrapSize", 0.3f,
                "Change the minimum size of spike traps. No need to restart the game :)");
            CreateFloatConfig(minSizeSpikeTrapEntry, 0f, 5f);

            maxSizeSpikeTrapEntry = Config.Bind("SpikeTrap", "MaxSpikeTrapSize", 2f,
                "Change the maximum size of spike traps. No need to restart the game :)");
            CreateFloatConfig(maxSizeSpikeTrapEntry, 0f, 5f);
            
            //modded hazards
            
            customAffectModdedHazardEntry = Config.Bind("ModdedHazard", "AffectModdedHazard", true,
                "Activate to make this mod affect modded map hazards size. No need to restart the game :)");
            CreateBoolConfig(customAffectModdedHazardEntry);

            minSizeModdedHazardEntry = Config.Bind("ModdedHazard", "MinModdedHazardSize", 0.3f,
                "Change the minimum size of modded map hazards. No need to restart the game :)");
            CreateFloatConfig(minSizeModdedHazardEntry, 0f, 5f);

            maxSizeModdedHazardEntry = Config.Bind("ModdedHazard", "MaxModdedHazardSize", 2f,
                "Change the maximum size of modded map hazards. No need to restart the game :)");
            CreateFloatConfig(maxSizeModdedHazardEntry, 0f, 5f);

            //INFLUENCES

            influenceHpConfig = Config.Bind("Influences", "InfluenceHp", true,
                "Activate to make size influence monsters HP. No need to restart the game :)");
            CreateBoolConfig(influenceHpConfig);

            influenceSoundConfig = Config.Bind("Influences", "InfluenceSound", true,
                "Activate to make size influence monsters sounds pitch. No need to restart the game :)");
            CreateBoolConfig(influenceSoundConfig);

            influenceSoundMinEntry = Config.Bind("Influences", "InfluenceSoundMinPitch", 0.6f,
                "If InfluenceSound is activated, this define the minimum pitch of monsters audio sources. No need to restart the game :)");
            CreateFloatConfig(influenceSoundMinEntry, 0f, 3f);

            influenceSoundMaxEntry = Config.Bind("Influences", "InfluenceSoundMaxPitch", 2.5f,
                "If InfluenceSound is activated, this define the maximum pitch of monsters audio sources. No need to restart the game :)");
            CreateFloatConfig(influenceSoundMaxEntry, 0f, 3f);

            influenceBeehiveEntry = Config.Bind("Influences", "InfluenceBeehive", false,
                "Activate to make size influence beehive weight and price. No need to restart the game :)");
            CreateBoolConfig(influenceBeehiveEntry);

            //FUNMODE

            funModeEntry = Config.Bind("FunMode", "FunMode", false,
                "Activate the fun mode to randomize the size in every space directions (verticaly, horizontaly). No need to restart the game :)");
            CreateBoolConfig(funModeEntry);

            funModeHorizontalMinEntry = Config.Bind("FunMode", "FunModeHorizontalSizeMin", 0.5f,
                "If fun mode is activated, it will change the minimum horizontal size of monsters (axis x and z). No need to restart the game :)");
            CreateFloatConfig(funModeHorizontalMinEntry);

            funModeHorizontalMaxEntry = Config.Bind("FunMode", "FunModeHorizontalSizeMax", 1.5f,
                "If fun mode is activated, it will change the maximum horizontal size of monsters (axis x and z). No need to restart the game :)");
            CreateFloatConfig(funModeHorizontalMaxEntry);

            funModeLockHorizontalEnrty = Config.Bind("FunMode", "LockHorizontalAxis", false,
                "If fun mode is activated, it will change the horizontal axis (x and z) with the same value. No need to restart the game :)");
            CreateBoolConfig(funModeLockHorizontalEnrty);

            //DEV

            devLogEntry = Config.Bind("DEV", "DevLogs", false, "Show the dev logs");
            CreateBoolConfig(devLogEntry);
            
            RedBeesManagement.Init();
            MapObjects.mapObjects.ForEach(m =>
            {
                var name = m.mapObject.prefabToSpawn.name;
                if(name.Contains("TurretContainer") || name.Contains("Landmine") || name.Contains("SpikeRoofTrapHazard")) return;
                var c = m.mapObject.prefabToSpawn.gameObject.GetComponent<MapHazardSizeRandomizer>();
                if(c == null) m.mapObject.prefabToSpawn.gameObject.AddComponent<MapHazardSizeRandomizer>();
            });

            Harmony.CreateAndPatchAll(typeof(PatchEnemySize));
            Harmony.CreateAndPatchAll(typeof(PatchTurretSize));
            Harmony.CreateAndPatchAll(typeof(PatchLandmineSize));
            Harmony.CreateAndPatchAll(typeof(PatchSpikeTrapSize));
            Harmony.CreateAndPatchAll(typeof(PatchRedLocustBees));
            Harmony.CreateAndPatchAll(typeof(PatchRoundManager));

            Logger.LogInfo("RandomEnemiesSize Patched !!");
        }

        public void RegisterResizedGameObject(EnemyResized enemyResized)
        {
            if (instance.RandomEnemiesSizeDataDictionary.ContainsKey(enemyResized.newtorkId))
            {
                instance.RandomEnemiesSizeDataDictionary.Remove(enemyResized.newtorkId);
            }
            instance.RandomEnemiesSizeDataDictionary.Add(enemyResized.newtorkId, enemyResized);
        }

        public static string GetDungeonName()
        {
            var interiorName = "Facility";
            if (!Chainloader.PluginInfos.ContainsKey(LethalLevelLoaderReferenceChain)) return interiorName;
            try
            {
                if (DungeonManager.CurrentExtendedDungeonFlow?.DungeonName != null)
                    interiorName = DungeonManager.CurrentExtendedDungeonFlow?.DungeonName;
            }
            catch
            {
                return interiorName;
            }

            if (instance.devLogEntry.Value) Debug.Log($"INTERIOR FOUND: {interiorName}");
            return interiorName;
        }

        private CanModifyResult CanModifyInterior()
        {
            if (!LethalLevelLoaderIsHere) return (false, "You need the mod LethalLevelLoader to use this feature !");

            return true;
        }

        private void CreateFloatConfig(ConfigEntry<float> configEntry, float min = 0f, float max = 30f)
        {
            var exampleSlider = new FloatSliderConfigItem(configEntry, new FloatSliderOptions
            {
                Min = min,
                Max = max,
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }

        private void CreateStringConfig(ConfigEntry<string> configEntry)
        {
            var exampleSlider = new TextInputFieldConfigItem(configEntry, new TextInputFieldOptions
            {
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }

        private void CreateInteriorStringConfig(ConfigEntry<string> configEntry)
        {
            var exampleSlider = new TextInputFieldConfigItem(configEntry, new TextInputFieldOptions
            {
                RequiresRestart = false,
                CanModifyCallback = CanModifyInterior
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }

        private void CreateBoolConfig(ConfigEntry<bool> configEntry)
        {
            var exampleSlider = new BoolCheckBoxConfigItem(configEntry, new BoolCheckBoxOptions
            {
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }

        public bool CompareEnemyName(string name, string verifiedName)
        {
            var name1 = name.ToLower();
            while (name1.Contains(" ")) name1 = name1.Replace(" ", "");

            var name2 = verifiedName.ToLower();
            while (name2.Contains(" ")) name2 = name2.Replace(" ", "");

            return name1.Contains(name2);
        }

        public CustomEnemySize GetCustomEnemySize(string nameValue)
        {
            var customEnemy = new CustomEnemySize();

            if (customEnemyEntry.Value == "") return customEnemy;

            var customEnemies = customEnemyEntry.Value.ToLower();

            while (customEnemies.Contains(" ")) customEnemies = customEnemies.Replace(" ", "");

            float minvalue = 1;
            float maxvalue = 1;

            var name = nameValue.ToLower();
            while (name.Contains(" ")) name = name.Replace(" ", "");

            var enemies = customEnemies.Split(";");

            if (enemies.Length <= 1)
            {
                enemies = customEnemies.Split(",");
            }

            foreach (var e in enemies)
            {
                var values = e.Split(":");
                
                if(values.Length != 3) continue;
                
                if (name.Contains(values[0]))
                {
                    float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out minvalue);
                    float.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out maxvalue);

                    customEnemy.found = true;
                    customEnemy.minValue = minvalue;
                    customEnemy.maxValue = maxvalue;
                }
            }

            return customEnemy;
        }

        public CustomEnemySize GetCustomMoonEnemySize(string enemyName, string planetName)
        {
            var customEnemy = new CustomEnemySize();

            if (customMoonEntry.Value == "" || enemyName == null || planetName == null || enemyName == "" || planetName == "") return customEnemy;

            var customEnemies = customMoonEntry.Value.ToLower();

            while (customEnemies.Contains(" ")) customEnemies = customEnemies.Replace(" ", "");

            float minvalue = 1;
            float maxvalue = 1;

            var name = enemyName.ToLower();
            while (name.Contains(" ")) name = name.Replace(" ", "");

            var planet = planetName.ToLower();
            while (planet.Contains(" ")) planet = planet.Replace(" ", "");
            
            Logger.LogInfo(name);
            Logger.LogInfo(planet);
            Logger.LogInfo(customEnemies);
            
            var moons = customEnemies.Split(";");
            Logger.LogInfo(moons);

            foreach (var e in moons)
            {
                var data = e.Split("#");
                Logger.LogInfo($"{data[0]}, {data[1]}");

                if (CompareEnemyName(planet, data[0]))
                {
                    var enemies = data[1].Split(",");
                    Logger.LogInfo($"{enemies}, {enemies.Length}");


                    foreach (var enemy in enemies)
                    {
                        Logger.LogInfo(enemy);

                        var values = enemy.Split(":");
                        Logger.LogInfo(values);

                        if(values.Length != 3) continue;
            
                        if (name.Contains(values[0]) || (values[0] == "any" && !customEnemy.found))
                        {
                            Logger.LogInfo(values);

                            float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out minvalue);
                            float.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out maxvalue);

                            customEnemy.found = true;
                            customEnemy.minValue = minvalue;
                            customEnemy.maxValue = maxvalue;
                        }
                    }
                

                }
            }

            return customEnemy;
        }

        public float GetInteriorMultiplier(string enemyNameValue, string interiorNameValue
        )
        {
            //remove spaces
            var enemyName = enemyNameValue.ToLower();
            while (enemyName.Contains(" ")) enemyName = enemyName.Replace(" ", "");

            var interiorName = interiorNameValue.ToLower();
            while (interiorName.Contains(" ")) interiorName = interiorName.Replace(" ", "");

            var interiorsInputValue = customInteriorEntry.Value.ToLower();
            while (interiorsInputValue.Contains(" ")) interiorsInputValue = interiorsInputValue.Replace(" ", "");

            //check
            if ((interiorsInputValue.Contains(enemyName) || interiorsInputValue.Contains("any")) &&
                interiorsInputValue.Contains(interiorName))
            {
                var multiplier = 1f;

                var interiors = interiorsInputValue.Split(";");
                foreach (var i in interiors)
                {
                    var splitInterior = i.Split("#");
                    var name = splitInterior[0];

                    if (name.Contains(interiorName))
                    {
                        var values = splitInterior[1].Split(",");

                        foreach (var v in values)
                        {
                            var monsterValue = v.Split(":");
                            var monsterName = monsterValue[0];
                            var monsterMultiplier = monsterValue[1];
                            //Debug.Log($"MONTSER NAME {monsterName} MONSTER VALUE {monsterMultiplier}");

                            if (monsterName == "any" || monsterName == enemyName)
                                float.TryParse(monsterMultiplier, NumberStyles.Any, CultureInfo.InvariantCulture,
                                    out multiplier);
                        }
                    }
                }

                return multiplier;
            }


            return 1f;
        }
    }

    public class CustomEnemySize
    {
        public bool found;
        public float maxValue = 1;
        public float minValue = 1;
    }
}