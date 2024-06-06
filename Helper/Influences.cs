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
            AudioSource componentAudioSource = null;

            enemyAI.TryGetComponent(out componentAudioSource);
            if (componentAudioSource != null) audioSources.Add(componentAudioSource);
            audioSources.AddRange(enemyAI.GetComponentsInChildren<AudioSource>());

            var value = 1f - (multiplier - 1);
            var pitch = Mathf.Clamp(value, soundInfluenceMin, soundInfluenceMax);

            if (enemyAI.creatureVoice != null) enemyAI.creatureVoice.pitch = pitch;
            if (enemyAI.creatureSFX != null) enemyAI.creatureSFX.pitch = pitch;

            foreach (var audioSource in audioSources) audioSource.pitch = pitch;
        }

        public void InfluenceTurretSound(Turret turret, float multiplier)
        {
            if (!soundInfluence || turret == null) return;

            var audioSources = new List<AudioSource>();
            AudioSource componentAudioSource = null;

            turret.TryGetComponent(out componentAudioSource);
            if (componentAudioSource != null) audioSources.Add(componentAudioSource);
            audioSources.AddRange(turret.GetComponentsInChildren<AudioSource>());

            var value = 1f - (multiplier - 1);
            var pitch = Mathf.Clamp(value, soundInfluenceMin, soundInfluenceMax);

            if (turret.berserkAudio != null) turret.berserkAudio.pitch = pitch;
            if (turret.farAudio != null) turret.farAudio.pitch = pitch;
            if (turret.mainAudio != null) turret.mainAudio.pitch = pitch;
            if (turret.bulletCollisionAudio != null) turret.bulletCollisionAudio.pitch = pitch;

            foreach (var audioSource in audioSources) audioSource.pitch = pitch;
        }
    }
}