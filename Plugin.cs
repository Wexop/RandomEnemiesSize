using System.Globalization;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using RandomEnemiesSize.Patches;

namespace RandomEnemiesSize
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class RandomEnemiesSize : BaseUnityPlugin
    {


        const string GUID = "wexop.random_enemies_size";
        const string NAME = "RandomEnemiesSize";
        const string VERSION = "1.0.5";

        public static RandomEnemiesSize instance;
        
        public ConfigEntry<float> minSizeIndoorEntry;
        public ConfigEntry<float> maxSizeIndoorEntry;
        public ConfigEntry<float> maxSizeOutdoorEntry;
        public ConfigEntry<float> minSizeOutdoorEntry;
        public ConfigEntry<string> customEnemyEntry;
        
        
        public ConfigEntry<bool> funModeEntry;
        public ConfigEntry<float> funModeHorizontalMinEntry;
        public ConfigEntry<float> funModeHorizontalMaxEntry;
        public ConfigEntry<bool> lockFunModeHorizontalEnrty;

        void Awake()
        {


            instance = this;

            Logger.LogInfo($"RandomEnemiesSize starting....");

            minSizeIndoorEntry = Config.Bind("General", "MinMonstersSizeIndoor", 0.4f, "Change the minimum size of monsters in the factory. No need to restart the game :)");
            CreateFloatConfig(minSizeIndoorEntry);
            
            maxSizeIndoorEntry = Config.Bind("General", "MaxMonstersSizeIndoor", 1.5f, "Change the maximum size of monsters in the factory. No need to restart the game :)");
            CreateFloatConfig(maxSizeIndoorEntry);
            
            minSizeOutdoorEntry = Config.Bind("General", "MinMonstersSizeOutdoor", 0.5f, "Change the minimum size of monsters outside the factory. No need to restart the game :)");
            CreateFloatConfig(minSizeOutdoorEntry);
            
            maxSizeOutdoorEntry = Config.Bind("General", "MaxMonstersSizeOutdoor", 3f, "Change the maximum size of monsters outside the factory. No need to restart the game :)");
            CreateFloatConfig(maxSizeOutdoorEntry);
            
            customEnemyEntry = Config.Bind("Custom", "CustomEnemiesSize", "", "Custom the size for an enemy wanted with his EXACT name. RECOMMENDED: Go to the thunderstore mod page, you can find a generator to make easier this config. Manual example -> ForestGiant:0.4:5;FlowerMan:0.2:6. Dont forgot the separator ';' between each monsters. No need to restart the game :)");
            CreateStringConfig(customEnemyEntry);

            funModeEntry = Config.Bind("FunMode", "FunMode", false, "Activate the fun mode to randomize the size in every space directions (verticaly, horizontaly). No need to restart the game :)");
            CreateBoolConfig(funModeEntry);
            
            funModeHorizontalMinEntry = Config.Bind("FunMode", "FunModeHorizontalSizeMin", 0.5f, "If fun mode is activated, it will change the minimum horizontal size of monsters (axis x and z). No need to restart the game :)");
            CreateFloatConfig(funModeHorizontalMinEntry);    
            
            funModeHorizontalMaxEntry = Config.Bind("FunMode", "FunModeHorizontalSizeMax", 1.5f, "If fun mode is activated, it will change the maximum horizontal size of monsters (axis x and z). No need to restart the game :)");
            CreateFloatConfig(funModeHorizontalMaxEntry);
            
            lockFunModeHorizontalEnrty = Config.Bind("FunMode", "LockHorizontalAxis", false, "If fun mode is activated, it will change the horizontal axis (x and z) with the same value. No need to restart the game :)");
            CreateBoolConfig(lockFunModeHorizontalEnrty);
            
            Harmony.CreateAndPatchAll(typeof(PatchEnemySize));
            
            Logger.LogInfo($"RandomEnemiesSize Patched !!");
            
        }
        
        private void CreateFloatConfig( ConfigEntry<float> configEntry)
        {
            var exampleSlider = new FloatSliderConfigItem(configEntry, new FloatSliderOptions() 
            {
                Min = 0f,
                Max = 30f,
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }
        
        private void CreateStringConfig( ConfigEntry<string> configEntry)
        {
            var exampleSlider = new TextInputFieldConfigItem(configEntry, new TextInputFieldOptions() 
            {
                RequiresRestart = false,
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }
        
        private void CreateBoolConfig( ConfigEntry<bool> configEntry)
        {
            var exampleSlider = new BoolCheckBoxConfigItem(configEntry, new BoolCheckBoxOptions() 
            {
                RequiresRestart = false,
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }

        public CustomEnemySize GetCustomEnemySize(string nameValue)
        {
            var customEnemy = new CustomEnemySize();

            string customEnemies = customEnemyEntry.Value.ToLower();

            while (customEnemies.Contains(" "))
            {
                customEnemies = customEnemies.Replace(" ", "");
            }

            float minvalue = 1;
            float maxvalue = 1;

            var name = nameValue.ToLower();
            while (name.Contains(" "))
            {
                name = name.Replace(" ", "");
            }

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


    }

    public class CustomEnemySize
    {
        public float maxValue = 1;
        public float minValue = 1;
        public bool found = false;
    }
}