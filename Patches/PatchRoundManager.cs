using HarmonyLib;

namespace RandomEnemiesSize.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class PatchRoundManager
{
    [HarmonyPatch(nameof(RoundManager.LoadNewLevel))]
    [HarmonyPrefix]
    private static void PatchLoadLevel()
    {
        RandomEnemiesSize.instance.RandomEnemiesSizeDataDictionary.Clear();
    }
}