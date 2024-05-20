using System;

namespace RandomEnemiesSize
{
    public class Influences
    {
        public bool hpInfluence;

        public void GetInfos()
        {
            hpInfluence = RandomEnemiesSize.instance.influenceHpConfig.Value;
        }

        public int InfluenceHp(int baseHp, float multiplier)
        {
            if (!hpInfluence) return baseHp;

            var newHp = (int) Math.Round(baseHp * multiplier, 0);

            if (newHp < 0) newHp = 1;

            return newHp;
        }
    }
}