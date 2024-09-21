# Changelog

### v1.1.11

- Try to fix a desync size issue on hazards

### v1.1.10

- Add Maneater into vanilla check
- Mod can now affect modded map hazards
- Add configs for modded map hazards
- Update README with a new section MOD COMPATIBILITY FOR MAP HAZARDS, for modded map hazards that would not be affected by default by the mod.
- Add lethal lib in dependency

### v1.1.9

- Add V55 enemies into vanilla enemies check
- Fix bees spam error log
- Make the mod easier to have compatibility for other devs. Check README => MOD COMPATIBILITY for more information.

### v1.1.8

- New config, Beehive influence, disabled by default. Beehive weight and price are affected by the size.
- New Bees management : Beehive size is always affected, but the weight is not if you disable the Beehive influence config. 
- Also, bees speed is now affected by the size (big bees => slower, small bees have the same speed as normal size). Bees player detection is also affected, so you can get easier big bees.

### v1.1.7

- Spike traps size are now affected by the mod (configurable, activated by default)

### v1.1.6

- You can configure the chance for each monster to have a random size (0% is never, 100% is always)
- You can show dev logs or not (disabled by default)

### v1.1.5

- Landmines and turrets can now have a random size (activated by default)
- Landmines and turrets are affected by FunMode and sound pitch influence if activated

### v1.1.4

- Fix an issue where bees hive don't spawn
- Bees hive is now affected by the mod (size and weight)
- Add two configs to enable the mod to affect or not the vanilla enemies and to affect or not the modded enemies (both
  activated by default)

### v1.1.3

- Size of monsters can now influence the sound pitch of every of their audio sources (configurable, activate by default)
- Added this changelog file :)

### v1.1.2

- Fix "any" keyword for interiors multiplier

### v1.1.1

- LethalLevelLoader is now a soft dependence, you need it for the customInteriors config only.
- Remove LethalLevelLoader from hard dependencies

### v1.1.0

- Cleaned the sync between host - client
- Size of monsters now can influence their HP (configurable, activate by default)
- Fix the test room issue where size is not affected

**new dependencies :**

- StaticNetwork, for sync between every players
- LethalLevelLoader, to get easily the actual interior name, and to easily have a compatibily with most of the interiors
  mod

### v1.0.5

- You can now lock the horizontal axis (x and z ), so they will be multplied with the same value Also some woding fixs

### v1.0.4

- Add a FunMode config (disabled by default) that randomize the scale for every axis direction (x,y,z)
- The height range is still based on initial configs, but you can config horizontals ranges (x,z)

### v1.0.3

- Fix scale issue, now multiply the original scale.
- Fix custom enemies name with space in name.
- Now can use this tool : https://wexop.github.io/RandomEnemiesSizeCustomGeneraror to generate the custom enemies size
  ranges wanted

### v1.0.1

- Add more configs : Outside, Indoor and custom enemies :slight_smile:
- For more infos on custom enemies: https://thunderstore.io/c/lethal-company/p/Wexop/RandomEnemiesSize/

