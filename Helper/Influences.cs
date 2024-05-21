using System;
using System.Collections.Generic;
using UnityEngine;

namespace RandomEnemiesSize
{
    public class Influences
    {
        public bool hpInfluence;
        public bool soundInfluence;
        public float soundInfluenceMax;
        public float soundInfluenceMin;

        public void GetInfos()
        {
            hpInfluence = RandomEnemiesSize.instance.influenceHpConfig.Value;
            soundInfluence = RandomEnemiesSize.instance.influenceSoundConfig.Value;
            soundInfluenceMin = RandomEnemiesSize.instance.influenceSoundMinEntry.Value;
            soundInfluenceMax = RandomEnemiesSize.instance.influenceSoundMaxEntry.Value;
        }

        public int InfluenceHp(int baseHp, float multiplier)
        {
            if (!hpInfluence) return baseHp;

            var newHp = (int) Math.Round(baseHp * multiplier, 0);

            if (newHp < 0) newHp = 1;

            return newHp;
        }

        public void InfluenceSound(EnemyAI enemyAI, float multiplier)
        {
            if (!soundInfluence) return;

            var audioSources = new List<AudioSource>();
            audioSources.AddRange(enemyAI.GetComponents<AudioSource>());
            audioSources.AddRange(enemyAI.GetComponentsInChildren<AudioSource>());
            var value = 1f - (multiplier - 1);
            var pitch = Mathf.Clamp(value, soundInfluenceMin, soundInfluenceMax);
            enemyAI.creatureVoice.pitch = pitch;
            enemyAI.creatureSFX.pitch = pitch;
            foreach (var audioSource in audioSources) audioSource.pitch = pitch;
        }
    }
}