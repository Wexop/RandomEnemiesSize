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
        const string VERSION = "1.0.0";

        public static RandomEnemiesSize instance;
        
        public ConfigEntry<float> minSizeEntry;
        public ConfigEntry<float> maxSizeEntry;

        void Awake()
        {


            instance = this;

            Logger.LogInfo($"RandomEnemiesSize starting....");
            
            minSizeEntry = Config.Bind("Size", "MinMonstersSize", 0.5f, "Change the minimum size of monsters");
            CreateFloatConfig(minSizeEntry);
            
            maxSizeEntry = Config.Bind("Size", "MaxMonstersSize", 3f, "Change the maximum size of monsters");
            CreateFloatConfig(maxSizeEntry);
            
            Harmony.CreateAndPatchAll(typeof(PatchEnemySize));
            
            Logger.LogInfo($"RandomEnemiesSize Patched !!");
            
        }
        
        private void CreateFloatConfig( ConfigEntry<float> configEntry)
        {
            var exampleSlider = new FloatSliderConfigItem(configEntry, new FloatSliderOptions() 
            {
                Min = 0f,
                Max = 50f
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }




    }
}