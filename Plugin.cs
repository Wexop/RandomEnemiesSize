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
        const string VERSION = "1.0.2";

        public static RandomEnemiesSize instance;
        
        public ConfigEntry<float> minSizeIndoorEntry;
        public ConfigEntry<float> maxSizeIndoorEntry;
        public ConfigEntry<float> maxSizeOutdoorEntry;
        public ConfigEntry<float> minSizeOutdoorEntry;
        public ConfigEntry<string> customEnemyEntry;

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
            
            customEnemyEntry = Config.Bind("Custom", "CustomEnemiesSize", "", "Custom the size for an enemy wanted with his EXACT name. for example -> ForestGiant:0.4:5;FlowerMan:0.2:6. Dont forgot the separator ';' between each monsters. No need to restart the game :)");
            CreateStringConfig(customEnemyEntry);
            
            Harmony.CreateAndPatchAll(typeof(PatchEnemySize));
            
            Logger.LogInfo($"RandomEnemiesSize Patched !!");
            
        }
        
        private void CreateFloatConfig( ConfigEntry<float> configEntry)
        {
            var exampleSlider = new FloatSliderConfigItem(configEntry, new FloatSliderOptions() 
            {
                Min = 0f,
                Max = 50f,
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

        public CustomEnemySize GetCustomEnemySize(string name)
        {
            var customEnemy = new CustomEnemySize();

            string customEnemies = customEnemyEntry.Value;

            float minvalue = 1;
            float maxvalue = 1;

            if (customEnemies.ToLower().Contains(name.ToLower()))
            {
                customEnemy.found = true;
                var enemies = customEnemies.Split(";");

                foreach (var e in enemies)
                {
                    var values = e.Split(":");
                    if (values[0].ToLower().Contains(name.ToLower()))
                    {
                        float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out minvalue);
                        float.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out maxvalue);

                        customEnemy.minValue = minvalue;
                        customEnemy.minValue = maxvalue;
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