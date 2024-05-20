Every human don't have the same height no ? So why monsters do ?

This mod make monsters spawn with a random size !

You can configure the range size for :

- Outside enemies
- Indoor enemies
- Custom enemies

**Note that:**

- Others mods enemies should be impacted by this mod.
- Only the **player who host the game** config will be taken.

**FOR CUSTOM ENEMIES**

**Recommended:**

You can use [this generator](https://wexop.github.io/RandomEnemiesSizeCustomGeneraror) and copy the result in the
input 'CustomEnemiesSize' to custom the range size of each monsters!

**Manualy:**

This is an input, you can for example write -> ForestGiant:1:2;NutCracker:1:2 Make sure to have a separator ';' between
monsters, even if you have only one custom enemies !

**FOR CUSTOM INTERIORS**

You can multiply the previous configs size for every interiors (and every monster in every interior !)

**Recommended:**

You can use [the generator](https://wexop.github.io/RandomEnemiesSizeCustomGeneraror) and copy the result in the input '
CustomInteriorsSize'!

This is an input, you can for example write -> HauntedMansion#any:1,Puffer:2;

The formula is interiorName, an # to tell this is the end of the name, then you can write any (or not) and tell the
mulitplier (here its one), or write any enemie (list
on [the generator](https://wexop.github.io/RandomEnemiesSizeCustomGeneraror)).

Make sure to have a separator ';' between interiors, even if you have only one !

**INFLUENCES**

- Hp influence is activated by default, the hp of enemies will be multiplie with their size (bigger enemy -> more hp,
  smaller -> less hp)

**FUN MODE**
You can activate the fun mode to randomize the size of every axis (x,y,z). The height range is based on initial configs,
but you can config horizontals ranges (x,z)

You can lock the horizontal axis (x and z) to have the same value.

This normaly should work with modded enemies. For any issues, create an issue on the git page :)

Have fun :)

**QUESTIONS**

- Why LethalLevelLoader as dependence ?

LLL give an easy way to know the dungeon name (for custom interiors multiplier), and most of the mods use this api to
create custom dungeon, so can find the interior name easily !