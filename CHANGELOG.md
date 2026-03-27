# ServUO Custom Shard Changes

## Config Changes
- **Siege.cfg** — Enabled siege mode (`IsSiege=true`) for Felucca rules on all maps
- **CurrentExpansion.cs** — Enabled insurance on siege shard
- **PlayerMobile.cs** — Disabled siege bless item, disabled fastwalk prevention
- **Fastwalk.cs** — Disabled old fastwalk system

## Skill & Loot System
- **SkillCheck.cs** — Faster skill gains (25% min chance, GGS enabled, siege ROT removed, faster GGS timers)
- **TrainingDummies.cs** — Raised max skill to 120
- **AdvancedTrainingDummy.cs** — Raised max skill to 120
- **ItemPropertyInfo.cs** — Hit spells min 25%, max 50%, no overcap
- **RandomItemGenerator.cs** — Added Szavetra to boss entry (+250 budget)
- **Szavetra.cs** — SuperBoss x4 loot
- **BaseChampion.cs** — 5% custom artifact drops per eligible player, 120 power scroll chance increased to 15%
- **RangersCloakOfAugmentation.cs** — Updated stats (kinetic eater 10%, SDI 5%, LMC 8%)
- **WardensArmorOfAugmentation.cs** — Updated stats (same as above)

## Custom Armor

**Gloves of the Holy Warrior** — PlateGloves / GargishPlateKilt
- `[add GlovesOfTheHolyWarrior` / `[add GargishKiltOfTheHolyWarrior`
- Chivalry +15, Necromancy +15, Reactive Holy Light 10%, Str +5, Int +5, HP +5, Stam +10, Mana +15, LMC 8%, All Resist 15

**Sentinel's Mempo** — PlateMempo / GargishNecklace
- `[add SentinelsMempo` / `[add SentinelsNecklace`
- Str +4, Dex +4, HP +8, Stam +12, Mana +8, HCI 5%, DCI 5%, LMC 8%, DI 25%, All Resist 15

**Shugenja's Raiment** — PlateDo / GargishPlateChest
- `[add ShugenjasRaiment` / `[add GargishShugenjasRaiment`
- Int +5, HP +5, HP Regen 3, Mana Regen 3, SDI 5%, FCR 1, LMC 8%, LRC 20%, Mage Armor, All Resist 15

**Hexweaver's Visage** — ElvenGlasses / GargishGlasses
- `[add HexweaversVisage` / `[add GargishHexweaversVisage`
- Random +10 Necro/Myst/Magery/Chiv, Casting Focus 2%, Str +5, Int +5, HP +5, Mana +8, SDI 5%, LMC 8%, LRC 20%, Mage Armor, All Resist 15

**Umbrascale Champion's Aegis** — ElvenGlasses / GargishGlasses
- `[add UmbrascaleChampionsAegis` / `[add GargishUmbrascaleChampionsAegis`
- Random +15 Sword/Mace/Fencing/Archery/Throwing, Str +5, Dex +5, Int +5, HP +5, Stam +8, Mana +8, LMC 8%, All Resist 15

**Corrupted Paladin Vambraces** — PlateArms / GargishPlateArms
- `[add CorruptedPaladinVambraces` / `[add GargishCorruptedPaladinVambraces`
- Str +5, Dex +5, Stam +10, Mana +10, HP Regen 4, Stam Regen 4, Mana Regen 4, LMC 8%, All Resist 15

**Gloves of the Archlich** — BoneGloves / GargishPlateKilt
- `[add GlovesOfTheArchlich` / `[add GargishKiltOfTheArchlich`
- Fire Eater 15%, Str +5, Int +5, HP +5, Mana +8, HP Regen 3, Mana Regen 3, LMC 10%, LRC 20%, Mage Armor, All Resist 15

**Balron Bone Armor** — BoneChest / GargishPlateChest
- `[add BalronBoneArmor` / `[add GargishBalronBoneArmor`
- Str +5, Dex +5, Int +5, HP +5, Stam +8, Mana +8, HCI 5%, DCI 5%, LMC 8%, All Resist 15 (Fire 20), 60 Str Req

## Custom Clothing

**Scabbard of Juo'nar** — SwordBelt / GargoyleHalfApron
- `[add ScabbardOfJuonar` / `[add GargishScabbardOfJuonar`
- Resist +10, Int +10, HP +5, SDI 5%, FC 1

**General Lethe's Epaulettes** — Epaulette / GargishEpaulette
- `[add GeneralLethesEpaulettes` / `[add GargishGeneralLethesEpaulettes`
- Eval Int +10, Mana +8, Mana Regen 1, FCR 1, LRC 10%

**Lord Morphius' Epaulettes** — Epaulette / GargishEpaulette
- `[add LordMorphiusEpaulettes` / `[add GargishLordMorphiusEpaulettes`
- Anatomy +10, Stam +8, Stam Regen 2, LMC 5%, SSI 10%

**Shadowbane Epaulettes** — Epaulette / GargishEpaulette
- `[add ShadowbaneEpaulettes` / `[add GargishShadowbaneEpaulettes`
- HCI 10%, DCI 10%, LMC 8%

**Mantle of the Archlich** — Robe (no garg version)
- `[add MantleOfTheArchlich`
- Resisting Spells +10, SDI 8%, FC 1

**Feudal Cloak of Elements** — Cloak / GargishLeatherWingArmor
- `[add FeudalCloakOfElements` / `[add WingArmorOfElements`
- Damage Eater 15%, Fire Resist 15%, HP Regen 2, Stam Regen 3, Mana Regen 2, Luck 150

**Feudal Ghostwalkers** — Sandals / LeatherTalons
- `[add FeudalGhostwalkers` / `[add GargishFeudalGhostwalkers`
- Stealth +5, Hiding +5, Ninjitsu +5, Night Sight, 255 durability

**Mushroom Apron** — HalfApron / GargoyleHalfApron
- `[add MushroomApron` / `[add GargishMushroomApron`
- Alchemy +10, HP +5, HP Regen 2, Enhance Potions 15%

**Serpent Skin Quiver** — BaseQuiver / GargishLeatherWingArmor
- `[add SerpentSkinQuiver` / `[add GargishSerpentSkinWingArmor`
- Damage Mod 10%, Ammo 0/1000, Weight Reduction 50%, Lower Ammo Cost 50%, Anatomy +10, Luck 125, SSI 10%, DI 20%

**Ranger's Cloak of Augmentation** — Cloak / GargishLeatherWingArmor
- `[add RangersCloakOfAugmentation` / `[add WardensArmorOfAugmentation`
- Kinetic Eater 10%, SDI 5%, LMC 8%, SSI 5%

## Custom Weapons

**Expor Malas Flamus** — BladedStaff / GargishKatana
- `[add ExporMalasFlamus` / `[add GargishExporMalasFlamus`
- Searing Weapon, Hit Lower Attack 50%, Hit Life Leech 100%, Hit Mana Leech 100%, Hit Stam Leech 50%, Hit Fire Area 70%, DI 30%, Fire Damage 100%

**Shugenja's Wand** — MagicWand (all races)
- `[add ShugenjasWand`
- Spell Channeling, Mage Weapon -0, Mana Regen 10, FCR 2, Physical Damage 100%, 255 durability

## Slayer Weapon Set (84 weapons)

12 weapon types x 7 slayers. Format: `[add {Slayer}{Weapon}`

**Weapon Types:** Leafblade, WarAxe, Broadsword, DoubleAxe, WarHammer, MagicalShortbow, CompositeBow, SoulGlaive, Boomerang, GargishTalwar, GargishKatana, Lajatang

**All slayer weapons have:** Hit Mana Leech 100%, Hit Life Leech 100%, Hit Stam Leech 50%, Hit Lower Defense 50%, Brittle, 255/255 durability

**Reptile** — Poison 100%, Hit Poison Area 50%
- Example: `[add ReptileBroadsword`

**Repond** — Cold 100%, Hit Cold Area 50%
- Example: `[add RepondBroadsword`

**Arachnid** — Fire 100%, Hit Fire Area 50%
- Example: `[add ArachnidBroadsword`

**Undead** — Fire 100%, Hit Fire Area 50%
- Example: `[add UndeadBroadsword`

**Demon** — Cold 100%, Hit Cold Area 50%
- Example: `[add DemonBroadsword`

**Fey** — Fire 100%, Hit Fire Area 50%
- Example: `[add FeyBroadsword`

**Elemental** — Energy 100%, Hit Energy Area 50%
- Example: `[add ElementalBroadsword`

## Custom Spellbooks

All spellbooks: Blessed, full 64 spells, Spell Damage 50%, FCR 1, FC 1, Mana Regen 3

- `[add ReptilianDeathSpellbook` — Tome of Reptilian Death (Reptile Slayer)
- `[add RepondSpellbook` — Tome of Repond (Repond Slayer)
- `[add UndeadSpellbook` — Tome of the Undead (Undead Slayer)
- `[add DemonSpellbook` — Tome of Exorcism (Demon Slayer)
- `[add FeySpellbook` — Tome of the Fey (Fey Slayer)
- `[add ArachnidDoomSpellbook` — Tome of Arachnid Doom (Arachnid Slayer)
- `[add ElementalBanSpellbook` — Tome of Elemental Ban (Elemental Slayer)

## Custom Talisman

- `[add CarvedBoneRelicFromHolmes` — Carved Bone Relic from Holmes (Anatomy +20, Enhance Potions 15%)

## Existing Items Used (Khaldun)

- `[add MaskOfKhalAnkur` — Mask of Khal Ankur (human)
- `[add PendantOfKhalAnkur` — Pendant of Khal Ankur (gargoyle)

## Champion Drop System
All custom artifacts have a 5% chance to drop into an eligible player's backpack when any champion is killed on Felucca.
