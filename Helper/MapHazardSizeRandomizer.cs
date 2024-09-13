
using Unity.Netcode;
using UnityEngine;

namespace RandomEnemiesSize;

public class MapHazardSizeRandomizer: NetworkBehaviour
{
    private void Start()
    {
        if (!IsServer || !IsOwner) return;

        if (!RandomEnemiesSize.instance.customAffectModdedHazardEntry.Value) return;

        //RANDOM PERCENT

        var randomPercent = Random.Range(0f, 100f);

        if (RandomEnemiesSize.instance.randomPercentChanceEntry.Value < randomPercent)
        {
            if (RandomEnemiesSize.instance.devLogEntry.Value)
                Debug.Log(
                    $"RANDOM PERCENT NOT RANDOM SIZE : {randomPercent} FOR MAP HAZARD {gameObject.name}");
            return;
        }

        var scale = Random.Range(RandomEnemiesSize.instance.minSizeModdedHazardEntry.Value,
            RandomEnemiesSize.instance.maxSizeModdedHazardEntry.Value);
        
        var customEnemy = RandomEnemiesSize.instance.GetCustomEnemySize(gameObject.name);
        if (customEnemy.found) scale = Random.Range(customEnemy.minValue, customEnemy.maxValue);

        var networkObject = gameObject.GetComponentInParent<NetworkObject>();

        var originalScale = networkObject.transform.localScale;
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

        NetworkSize.UpdateAnyMapHazardClientRpc(networkObject.NetworkObjectId, newScale, scale, influences);
    }
}