# Changelog

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

