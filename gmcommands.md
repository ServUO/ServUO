# ServUO GM Commands Reference

## Essential GM Commands

| Command | What it does |
|---------|-------------|
| `[admin` | Opens the admin gump (main control panel) |
| `[props` | Click a target to view/edit its properties |
| `[add` | Add items or NPCs (e.g., `[add horse`) |
| `[remove` | Click to delete a target |
| `[move` | Move an object |
| `[go` | Teleport to a location or coordinates |
| `[where` | Shows your current coordinates |
| `[set` | Change a property (e.g., `[set str 100`) |

## Character & Skills

| Command | What it does |
|---------|-------------|
| `[setskill` | Set a skill value (e.g., `[setskill swords 100`) |
| `[allskills` | Set all skills to a value (e.g., `[allskills 100`) |
| `[set str 125` | Set strength (same for dex/int) |
| `[kill` / `[res` | Kill or resurrect a target |

## World Management

| Command | What it does |
|---------|-------------|
| `[save` | Force a world save |
| `[restart` | Restart the server |
| `[shutdown` | Shut down the server |
| `[broadcast` | Send a message to all players |
| `[spawner` | Opens the spawner gump for mob placement |
| `[xmlspawner` | Advanced spawner system |

## Movement & Visibility

| Command | What it does |
|---------|-------------|
| `[staff` / `[player` | Toggle staff mode on/off |
| `[hide` / `[unhide` | Hide/unhide yourself |
| `[tele` | Click to teleport to a spot |
| `[go britain` | Teleport to named locations |

## Items & Economy

| Command | What it does |
|---------|-------------|
| `[add gold 10000` | Add gold to your pack |
| `[dupe` | Duplicate an item |
| `[interface` | Opens item search interface |

## Common Locations

| Command | Destination |
|---------|------------|
| `[go britain` | Britain |
| `[go luna` | Luna (Malas) |
| `[go destard` | Destard dungeon |
| `[go sanctuary` | Sanctuary dungeon |

## Access Level Hierarchy

| Level | Role |
|-------|------|
| **Owner** | You — full unrestricted access |
| **Developer** | Same as Admin, can also edit core systems |
| **Administrator** | Server management, accounts, most commands |
| **Seer** | Can run events, limited world editing |
| **GameMaster** | Player support, basic moderation |
| **Counselor** | Can answer questions, very limited powers |
| **Player** | Normal player |

## What Admin Can Do (that lower staff can't)

- **Account management** — create, ban, delete accounts via `[admin`
- **Set access levels** — promote/demote other staff (but not above their own level)
- **Server controls** — `[save`, `[restart`, `[shutdown`
- **Edit all properties** — `[props` on any item, mobile, or region
- **Full `[add` access** — spawn any item or creature in the game
- **World building** — place houses, teleporters, decorations, vendors
- **Manage spawners** — control what mobs spawn where
- **View network info** — see connected clients, IPs, kick players

## What Only Owner Can Do (above Admin)

- Promote someone **to** Administrator/Developer
- Access certain protected server-level commands
- The Owner account is created at first startup and cannot be demoted

> In practice, for a personal shard you're the Owner so you have everything. The distinction matters more when you bring on staff to help run a public server.

## Placing Training Dummies

| Command | What it does |
|---------|-------------|
| `[add TrainingDummy` | Place a basic training dummy (trains to 120) |
| `[add AdvancedTrainingDummy` | Place an advanced training dummy (trains to 120) |
| `[remove` | Click on a dummy (or any item) to delete it |

Place these in Britain and Luna for players to train combat skills. Use `[go britain` and `[go luna` to get there.

## Tips

- Start with `[admin` for a GUI to manage accounts, world settings, and more
- Use `[add` to spawn items and creatures to populate your world
- Use `[props` on any object to view and edit all its properties
- Use `[set accesslevel` to promote/demote players