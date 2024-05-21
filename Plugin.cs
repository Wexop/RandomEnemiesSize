using System.Globalization;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalLevelLoader;
using RandomEnemiesSize.Patches;
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
        private const string VERSION = "1.1.3";

        public static string LethalLevelLoaderReferenceChain = "imabatby.lethallevelloader";

        public static RandomEnemiesSize instance;
        public bool LethalLevelLoaderIsHere;
        public ConfigEntry<string> customEnemyEntry;
        public ConfigEntry<string> customInteriorEntry;

        public ConfigEntry<bool> funModeEntry;
        public ConfigEntry<float> funModeHorizontalMaxEntry;
        public ConfigEntry<float> funModeHorizontalMinEntry;
        public ConfigEntry<bool> funModeLockHorizontalEnrty;

        public ConfigEntry<bool> influenceHpConfig;
        public ConfigEntry<bool> influenceSoundConfig;
        public ConfigEntry<float> influenceSoundMaxEntry;
        public ConfigEntry<float> influenceSoundMinEntry;

        public ConfigEntry<float> maxSizeIndoorEntry;
        public ConfigEntry<float> maxSizeOutdoorEntry;
        public ConfigEntry<float> minSizeIndoorEntry;
        public ConfigEntry<float> minSizeOutdoorEntry;

        private void Awake()
        {
            instance = this;

            Logger.LogInfo("RandomEnemiesSize starting....");

            if (Chainloader.PluginInfos.ContainsKey(LethalLevelLoaderReferenceChain))
            {
                Debug.Log("LethalLevelLoader found !");
                LethalLevelLoaderIsHere = true;
            }

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

            customEnemyEntry = Config.Bind("Custom", "CustomEnemiesSize", "",
                "Custom the size for an enemy wanted with his EXACT name. RECOMMENDED: Go to the thunderstore mod page, you can find a generator to make easier this config. Manual example -> ForestGiant:0.4:5;FlowerMan:0.2:6. Dont forgot the separator ';' between each monsters. No need to restart the game :)");
            CreateStringConfig(customEnemyEntry);

            customInteriorEntry = Config.Bind("Custom", "CustomInteriorsSize", "",
                "THE MOD LethalLevelLoader IS REQUIRED FOR THIS FEATURE. Multiply the base size for an indoor enemy in an interior wanted with his EXACT name. RECOMMENDED: Go to the thunderstore mod page, you can find a generator to make easier this config. Manual example -> mansion#any:1.5,NutCracker:2;customInterior#any:3; No need to restart the game :)");
            CreateInteriorStringConfig(customInteriorEntry);

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

            Harmony.CreateAndPatchAll(typeof(PatchEnemySize));

            Logger.LogInfo("RandomEnemiesSize Patched !!");
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

            Debug.Log($"INTERIOR FOUND: {interiorName}");
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

        public CustomEnemySize GetCustomEnemySize(string nameValue)
        {
            var customEnemy = new CustomEnemySize();

            var customEnemies = customEnemyEntry.Value.ToLower();

            while (customEnemies.Contains(" ")) customEnemies = customEnemies.Replace(" ", "");

            float minvalue = 1;
            float maxvalue = 1;

            var name = nameValue.ToLower();
            while (name.Contains(" ")) name = name.Replace(" ", "");

            if (customEnemies.ToLower().Contains(name))
            {
                var enemies = customEnemies.Split(";");

                foreach (var e in enemies)
                {
                    var values = e.Split(":");
                    if (values[0].Contains(name))
                    {
                        float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out minvalue);
                        float.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out maxvalue);

                        customEnemy.found = true;
                        customEnemy.minValue = minvalue;
                        customEnemy.maxValue = maxvalue;
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