Every human don't have the same height no ? So why monsters do ?

This mod make monsters spawn with a random size !

You can configure the range size for :

- Outside enemies
- Indoor enemies
- Custom enemies
- Hazards (landmines, turrets, spike traps, modded hazards)

You can configure if this mod affect vanilla and / or modded enemies (both activated by default)

You can configure the chance for each monster to have a random size (0% never to 100% always).

**Note that:**

- Others mods enemies and map hazards should be impacted by this mod.
- Only the **player who host the game** config will be taken.

### FOR CUSTOM ENEMIES

#### Recommended:

You can use [this generator](https://wexop.github.io/RandomEnemiesSizeCustomGeneraror) and copy the result in the
input 'CustomEnemiesSize' to custom the range size of each monsters or modded map hazards!

#### Manualy:

This is an input, you can for example write -> ForestGiant:1:2;NutCracker:1:2 Make sure to have a separator ';' between
monsters, even if you have only one custom enemies !

### FOR CUSTOM INTERIORS

To use this feature **you need install the mod LethalLevelLoader**.

You can multiply the previous configs size for every interior (and every monster in every interior !)

#### Recommended:

You can use [the generator](https://wexop.github.io/RandomEnemiesSizeCustomGeneraror) and copy the result in the input '
CustomInteriorsSize'!

#### Manualy:

This is an input, you can for example write -> HauntedMansion#any:1,Puffer:2;

The formula is interiorName, an # to tell this is the end of the name, then you can write any (or not) and tell the
mulitplier (here its one), or write any enemie (list
on [the generator](https://wexop.github.io/RandomEnemiesSizeCustomGeneraror)).

Make sure to have a separator ';' between interiors, even if you have only one !

### INFLUENCES

- **Hp influence** is activated by default, the hp of enemies will be multiplied with their size (bigger enemy -> more
  hp, smaller -> less hp)
- **Sound influence** is activated by default, the sound pitch of every audio sources of the monster will be multiplied
  with their size. You can also set the minimum and maximum pitch value in configs
- **Beehive influence** is disabled by default, the beehive weight and price are affected by the size.

### FUN MODE
You can activate the fun mode to randomize the size of every axis (x,y,z). The height range is based on initial configs,
but you can config horizontals ranges (x,z)

You can lock the horizontal axis (x and z) to have the same value.

This normaly should work with modded enemies. For any issues, create an issue on the git page :)

Have fun :)

### QUESTIONS

- Why LethalLevelLoader as soft dependence ?

LLL give an easy way to know the dungeon name (for custom interiors multiplier), and most of the mods use this api to
create custom dungeon, so can find the interior name easily !

### RELATED MODS

- [LittleCompany](https://thunderstore.io/c/lethal-company/p/Toybox/LittleCompany/) - A Lethal Company mod for anything size-related! Enemies behaviour change depending on the size of enemies and yours!
- [MapHazardsMoves](https://thunderstore.io/c/lethal-company/p/Wexop/MapHazardsMoves/) - A Lethal Company mod that make map hazards like turret, landmine, spike trap walk ! Try it with RandomEnemiesSize to be chased by a big scary spike trap ! 

### MOD COMPATIBILITY

I created a way to have an easy compatibility with RandomEnemiesSize. You can access to a dictionary from

```RandomEnemiesSize.RandomEnemiesSizeDataDictionary```

This dictionary entry is the NetworkId (ulong type) of any monster with a random size.
It returns an object like this :

``` 
 public class EnemyResized
{
    public Vector3 scale;
    public float multiplier;
    public ulong newtorkId;
    public Influences influences;
    public GameObject gameObject;
    public bool isHazard;
    public string enemyName;
}
 ```

**This dictionary is cleared each time RoundManager.LoadNewLevel is call**

Don't forget to check if the enemy networkId is here before trying to access to the object.

This should work for host and client. But remember that RandomEnemiesSize take only host configs, so check host may be better.

Feel free to use this dictionary for any compatibility with the mod.

### MOD COMPATIBILITY FOR MAP HAZARDS

RandomEnemiesSize can normally affect modded map hazards. 

If a modded map hazard cannot be affected by default by the mod, here is how you can have a compatibility :

Do a soft dependency on RandomEnemiesSize :
``` 
[BepInDependency("wexop.random_enemies_size", BepInDependency.DependencyFlags.SoftDependency)]
 ```
Now on your map hazard script, on the start check if RandomEnemiesSize is here, and add the component MapHazardSizeRandomizer : 

``` 
if (Chainloader.PluginInfos.ContainsKey("wexop.random_enemies_size"))
{
    gameObject.addComponent<MapHazardSizeRandomizer>();
}
 ```

For any question, suggestion, need, feel free to open an issue on GitHub or to ping me in the modded lethal company server :)

