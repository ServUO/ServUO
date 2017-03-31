# Change Log
All notable changes to this project will be documented in this file.

## March 2017

### Added
- World chat feature, working with latest clients.
    - Including logging facilities (user messages and channel events) for auditing purposes.
    - Not supported features have been removed, such as room moderators, voice, kicking, etc.
- Added *New Wrong* dungeon to `CreateWorld` gump.
- Support for 64 bit build in mono.
- Added text feedback system message when using the `[move` command.
- New overridable methods in the `Region` class:
    - `AllowFlying`, which determines whether a player can start flying or not inside the region.
    - `AllowAutoClaim`, which determines whether the pet auto claim functionality should be triggered when the player logs in inside the region.
- `.editorconfig` file with default configuration for code editors
- Enabling possiblity of SaveMetrics for Windows Performance Counters.
    - Requires to be run as Administrator once.
- Added *Mephitis* web special ability.
- Added *Gargish stone amulet* to the list of *Masonry* craftables.

### Changed
- Slayer opposition calculation logic has been overhauled: opposition type for each creature is now defined in the creature class, rather than in a global clunky list.
- `PublicMoongate` staff overrides: staff can now use public moongates regardless of status or range.
- Updates to *Spell Plague* mystic spell,
    - Remove Curse and Cleansing Winds spells now remove *Spell Plague*.
    - Now only triggers on damage by spell.
    - Reduced explosion damage as per EA.
- Valley of Eodon Volcano only send messages to players now, not staff.
- Mysticism spells have been moved to `Server.Spells.Mysticism` for broader compatibility.
- `BaseAI.AcquireFocusMob` combatant acquisition optimization.
- Allow creatures to run when using faster AI speeds.
- Reduced lockpicking delay from 3 seconds to 200 milliseconds, as per EA.
- Increased speed of *Demonic Jailor*.
- *Mage Armor* property now drops on armor loot.
- Metal Keg is now craftable using Blacksmithy.
- Tangle Apron is now alterable.
- Add *Navrey* to loot bump.
- Added ability to lockdown items in between walls in castles.
- Adjusted shadowguard spawn.
- Adjusted loot quality.
- *Ozymandias* now uses Melee AI.

### Fixed
- Fixed crash with *White Tiger Form* ninja mastery.
- Fixed bug that allowed players to auto claim their pets upon login while inside jail.
- Fixed *Myrmidex* battle spawner routes.
- Fixed *Wrong dungeon* exit teleporter.
- Non-normal creatures (i.e. bosses, champions, etc.) can no longer be controlled via *Command Undead* spell.
- Using *Command Undead* spell on Doom summoned skeletal dragon now resets.
- *Savage Shaman* can now be female only.
- *Savage Rider* can now be male only.
- Healing stone/spell stone can no longer be added to other containers.
- Added zero delay timer to `AddBlood to prevent console warnings.
- *Sleep* spell effects are now canceled on damage.
- Fixed region extension crash.
- Fixed VvV gump crash.
- Added support for murderers on heritage quest.
- Vendors no longer take gold from locked/trapped containers.
- Added delay to delete corpses in invasion region to ensure corpse has been created.
- Quest items can no longer be toggled while withing a sub pack.
- *Wildfire* spell items are now deleted on server start.
- Added new plants.
- *Myrmidex invasion* spawn corpses are now deleted on death.
- Only gargoyle can convert VvV item
- Fixed issue where poison charges weren't decreasing.
- Fixed splintering weapon.
- Holy fist spell no longer gives eval gains.
