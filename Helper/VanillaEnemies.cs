using System.Collections.Generic;

namespace RandomEnemiesSize
{
    public class VanillaEnemies
    {
        public List<string> VanillaNames = new List<string>
        {
            "ForestGiant",
            "Baboon hawk",
            "Blob",
            "Butler",
            "Butler Bees",
            "Centipede",
            "Crawler",
            "Docile Locust Bees",
            "Manticoil",
            "Girl",
            "Flowerman",
            "Tulip Snake",
            "Hoarding bug",
            "Jester",
            "Masked",
            "MouthDog",
            "Nutcracker",
            "Puffer",
            "RadMech",
            "Red Locust Bees",
            "Bunker Spider",
            "Earth Leviathan",
            "Spring",
            "Clay Surgeon",
            "Bush Wolf",
            "Maneater",
        };

        public bool IsAVanillaEnemy(string name)
        {
            var found = false;

            foreach (var vanilla in VanillaNames)
                if (RandomEnemiesSize.instance.CompareEnemyName(name, vanilla))
                    found = true;

            return found;
        }
    }
}