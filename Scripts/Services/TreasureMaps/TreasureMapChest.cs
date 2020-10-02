using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class TreasureMapChest : LockableContainer
    {
        public static Type[] Artifacts => m_Artifacts;
        private static readonly Type[] m_Artifacts = new Type[]
        {
            typeof(CandelabraOfSouls), typeof(GoldBricks), typeof(PhillipsWoodenSteed),
            typeof(ArcticDeathDealer), typeof(BlazeOfDeath), typeof(BurglarsBandana),
            typeof(CavortingClub), typeof(DreadPirateHat),
            typeof(EnchantedTitanLegBone), typeof(GwennosHarp), typeof(IolosLute),
            typeof(LunaLance), typeof(NightsKiss), typeof(NoxRangersHeavyCrossbow),
            typeof(PolarBearMask), typeof(VioletCourage), typeof(HeartOfTheLion),
            typeof(ColdBlood), typeof(AlchemistsBauble), typeof(CaptainQuacklebushsCutlass),
            typeof(ShieldOfInvulnerability), typeof(AncientShipModelOfTheHMSCape),
            typeof(AdmiralsHeartyRum)
        };

        public static Type[] ArtifactsLevelFiveToSeven => m_LevelFiveToSeven;
        private static readonly Type[] m_LevelFiveToSeven = new Type[]
        {
            typeof(ForgedPardon), typeof(ManaPhasingOrb), typeof(RunedSashOfWarding), typeof(SurgeShield)
        };

        public static Type[] ArtifactsLevelSeven => m_LevelSevenOnly;
        private static readonly Type[] m_LevelSevenOnly = new Type[]
        {
            typeof(CoffinPiece), typeof(MasterSkeletonKey)
        };

        public static Type[] SOSArtifacts => m_SOSArtifacts;
        private static readonly Type[] m_SOSArtifacts = new Type[]
        {
            typeof(AntiqueWeddingDress),
            typeof(KelpWovenLeggings),
            typeof(RunedDriftwoodBow),
            typeof(ValkyrieArmor)
        };
        public static Type[] SOSDecor => m_SOSDecor;
        private static readonly Type[] m_SOSDecor = new Type[]
        {
            typeof(GrapeVine),
            typeof(LargeFishingNet)
        };

        public static Type[] ImbuingIngreds => m_ImbuingIngreds;
        private static readonly Type[] m_ImbuingIngreds =
        {
            typeof(AbyssalCloth),   typeof(EssencePrecision), typeof(EssenceAchievement), typeof(EssenceBalance),
            typeof(EssenceControl), typeof(EssenceDiligence), typeof(EssenceDirection),   typeof(EssenceFeeling),
            typeof(EssenceOrder),   typeof(EssencePassion),   typeof(EssencePersistence), typeof(EssenceSingularity)
        };

        private static readonly TimeSpan _DeleteTime = TimeSpan.FromHours(3);

        private List<Item> m_Lifted = new List<Item>();

        private ChestQuality _Quality;

        [Constructable]
        public TreasureMapChest(int level)
            : this(null, level, false)
        {
        }

        public TreasureMapChest(Mobile owner, int level, bool temporary)
            : base(0xE40)
        {
            Owner = owner;
            Level = level;
            DeleteTime = DateTime.UtcNow + _DeleteTime;

            Temporary = temporary;
            Guardians = new List<Mobile>();
            AncientGuardians = new List<Mobile>();

            TimerRegistry.Register("TreasureMapChest", this, _DeleteTime, chest => chest.Delete());
        }

        public TreasureMapChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 3000541;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeleteTime { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DigTime { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Temporary { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool FirstOpenedByOwner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TreasureMap TreasureMap { get; set; }

        public Timer Timer { get; set; }

        public List<Mobile> Guardians { get; set; }

        public List<Mobile> AncientGuardians { get; set; }

        public ChestQuality ChestQuality
        {
            get { return _Quality; }
            set
            {
                if (_Quality != value)
                {
                    _Quality = value;

                    switch (_Quality)
                    {
                        case ChestQuality.Rusty: ItemID = 0xA306; break;
                        case ChestQuality.Standard: ItemID = 0xA304; break;
                        case ChestQuality.Gold: ItemID = 0xA308; break;
                    }
                }
            }
        }

        public bool FailedLockpick { get; set; }

        public override bool IsDecoContainer => false;

        public static void Fill(Mobile from, LockableContainer cont, int level, bool isSos)
        {
            Map map = from.Map;
            int luck = from is PlayerMobile ? ((PlayerMobile)from).RealLuck : from.Luck;

            cont.Movable = false;
            cont.Locked = true;
            int count;

            if (level == 0)
            {
                cont.LockLevel = 0; // Can't be unlocked

                cont.DropItem(new Gold(Utility.RandomMinMax(50, 100)));

                if (Utility.RandomDouble() < 0.75)
                    cont.DropItem(new TreasureMap(0, Map.Trammel));
            }
            else
            {
                cont.TrapType = TrapType.ExplosionTrap;
                cont.TrapPower = level * 25;
                cont.TrapLevel = level;

                switch (level)
                {
                    case 1:
                        cont.RequiredSkill = 5;
                        break;
                    case 2:
                        cont.RequiredSkill = 45;
                        break;
                    case 3:
                        cont.RequiredSkill = 65;
                        break;
                    case 4:
                        cont.RequiredSkill = 75;
                        break;
                    case 5:
                        cont.RequiredSkill = 75;
                        break;
                    case 6:
                        cont.RequiredSkill = 80;
                        break;
                    case 7:
                        cont.RequiredSkill = 80;
                        break;
                }

                cont.LockLevel = cont.RequiredSkill - 10;
                cont.MaxLockLevel = cont.RequiredSkill + 40;

                #region Gold
                cont.DropItem(new Gold(isSos ? level * 10000 : level * 5000));
                #endregion

                #region Scrolls
                if (isSos)
                {
                    switch (level)
                    {
                        default: count = 20; break;
                        case 0:
                        case 1: count = Utility.RandomMinMax(2, 5); break;
                        case 2: count = Utility.RandomMinMax(10, 15); break;
                    }
                }
                else
                {
                    count = level * 5;
                }

                for (int i = 0; i < count; ++i)
                    cont.DropItem(Loot.RandomScroll(0, 63, SpellbookType.Regular));
                #endregion

                #region Magical Items
                double propsScale = 1.0;

                switch (level)
                {
                    case 1:
                        count = isSos ? Utility.RandomMinMax(2, 6) : 32;
                        propsScale = 0.5625;
                        break;
                    case 2:
                        count = isSos ? Utility.RandomMinMax(10, 15) : 40;
                        propsScale = 0.6875;
                        break;
                    case 3:
                        count = isSos ? Utility.RandomMinMax(15, 20) : 48;
                        propsScale = 0.875;
                        break;
                    case 4:
                        count = isSos ? Utility.RandomMinMax(15, 20) : 56;
                        break;
                    case 5:
                        count = isSos ? Utility.RandomMinMax(15, 20) : 64;
                        break;
                    case 6:
                        count = isSos ? Utility.RandomMinMax(15, 20) : 72;
                        break;
                    case 7:
                        count = isSos ? Utility.RandomMinMax(15, 20) : 80;
                        break;
                    default:
                        count = 0;
                        break;
                }

                for (int i = 0; i < count; ++i)
                {
                    Item item;

                    item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();

                    if (item != null && RandomItemGenerator.Enabled)
                    {
                        int min, max;
                        GetRandomItemStat(out min, out max, propsScale);

                        RunicReforging.GenerateRandomItem(item, luck, min, max, map);

                        cont.DropItem(item);
                    }
                    else if (item is BaseWeapon)
                    {
                        BaseWeapon weapon = (BaseWeapon)item;

                        int attributeCount;
                        int min, max;

                        GetRandomAOSStats(out attributeCount, out min, out max);

                        BaseRunicTool.ApplyAttributesTo(weapon, attributeCount, min, max);

                        cont.DropItem(item);
                    }
                    else if (item is BaseArmor)
                    {
                        BaseArmor armor = (BaseArmor)item;

                        int attributeCount;
                        int min, max;

                        GetRandomAOSStats(out attributeCount, out min, out max);

                        BaseRunicTool.ApplyAttributesTo(armor, attributeCount, min, max);

                        cont.DropItem(item);
                    }
                    else if (item is BaseHat)
                    {
                        BaseHat hat = (BaseHat)item;

                        int attributeCount;
                        int min, max;

                        GetRandomAOSStats(out attributeCount, out min, out max);

                        BaseRunicTool.ApplyAttributesTo(hat, attributeCount, min, max);

                        cont.DropItem(item);
                    }
                    else if (item is BaseJewel)
                    {
                        int attributeCount;
                        int min, max;

                        GetRandomAOSStats(out attributeCount, out min, out max);

                        BaseRunicTool.ApplyAttributesTo((BaseJewel)item, attributeCount, min, max);

                        cont.DropItem(item);
                    }
                }
            }
            #endregion

            #region Reagents
            if (isSos)
            {
                switch (level)
                {
                    default: count = Utility.RandomMinMax(45, 60); break;
                    case 0:
                    case 1: count = Utility.RandomMinMax(15, 20); break;
                    case 2: count = Utility.RandomMinMax(25, 40); break;
                }
            }
            else
            {
                count = level == 0 ? 12 : Utility.RandomMinMax(40, 60) * (level + 1);
            }

            for (int i = 0; i < count; i++)
            {
                cont.DropItemStacked(Loot.RandomPossibleReagent());
            }
            #endregion

            #region Gems
            if (level == 0)
                count = 2;
            else
                count = (level * 3) + 1;

            for (int i = 0; i < count; i++)
            {
                cont.DropItem(Loot.RandomGem());
            }
            #endregion

            #region Imbuing Ingreds
            if (level > 1)
            {
                Item item = Loot.Construct(m_ImbuingIngreds[Utility.Random(m_ImbuingIngreds.Length)]);

                item.Amount = level;
                cont.DropItem(item);
            }
            #endregion

            Item arty = null;
            Item special = null;
            Item newSpecial = null;

            if (isSos)
            {
                if (0.004 * level > Utility.RandomDouble())
                    arty = Loot.Construct(m_SOSArtifacts);
                if (0.006 * level > Utility.RandomDouble())
                    special = Loot.Construct(m_SOSDecor);
                else if (0.009 * level > Utility.RandomDouble())
                    special = new TreasureMap(Utility.RandomMinMax(level, Math.Min(7, level + 1)), cont.Map);

                if (level >= 4)
                {
                    switch (Utility.Random(4))
                    {
                        case 0: newSpecial = new AncientAquariumFishNet(); break;
                        case 1: newSpecial = new LiveRock(); break;
                        case 2: newSpecial = new SaltedSerpentSteaks(); break;
                        case 3: newSpecial = new OceanSapphire(); break;
                    }
                }
            }
            else
            {
                if (level >= 7)
                {
                    if (0.025 > Utility.RandomDouble())
                        special = Loot.Construct(m_LevelSevenOnly);
                    else if (0.10 > Utility.RandomDouble())
                        special = Loot.Construct(m_LevelFiveToSeven);
                    else if (0.25 > Utility.RandomDouble())
                        special = GetRandomSpecial(level, cont.Map);

                    arty = Loot.Construct(m_Artifacts);
                }
                else if (level >= 6)
                {
                    if (0.025 > Utility.RandomDouble())
                        special = Loot.Construct(m_LevelFiveToSeven);
                    else if (0.20 > Utility.RandomDouble())
                        special = GetRandomSpecial(level, cont.Map);

                    arty = Loot.Construct(m_Artifacts);
                }
                else if (level >= 5)
                {
                    if (0.005 > Utility.RandomDouble())
                        special = Loot.Construct(m_LevelFiveToSeven);
                    else if (0.15 > Utility.RandomDouble())
                        special = GetRandomSpecial(level, cont.Map);
                }
                else if (0.10 > Utility.RandomDouble())
                {
                    special = GetRandomSpecial(level, cont.Map);
                }
            }

            if (arty != null)
            {
                Container pack = new Backpack
                {
                    Hue = 1278
                };

                pack.DropItem(arty);
                cont.DropItem(pack);
            }

            if (special != null)
                cont.DropItem(special);

            if (newSpecial != null)
                cont.DropItem(newSpecial);

            int rolls = 2;

            if (level >= 5)
                rolls += level - 2;

            RefinementComponent.Roll(cont, rolls, 0.10);
        }

        private static Item GetRandomSpecial(int level, Map map)
        {
            Item special;

            switch (Utility.Random(8))
            {
                default:
                case 0: special = new CreepingVine(); break;
                case 1: special = new MessageInABottle(); break;
                case 2: special = new ScrollOfAlacrity(PowerScroll.Skills[Utility.Random(PowerScroll.Skills.Count)]); break;
                case 3: special = new Skeletonkey(); break;
                case 4: special = new TastyTreat(5); break;
                case 5: special = new TreasureMap(Utility.RandomMinMax(level, Math.Min(7, level + 1)), map); break;
                case 6: special = GetRandomRecipe(); break;
                case 7: special = ScrollOfTranscendence.CreateRandom(1, 5); break;
            }

            return special;
        }

        public static void GetRandomItemStat(out int min, out int max, double scale = 1.0)
        {
            int rnd = Utility.Random(100);

            if (rnd <= 1)
            {
                min = 500; max = 1300;
            }
            else if (rnd < 5)
            {
                min = 400; max = 1100;
            }
            else if (rnd < 25)
            {
                min = 350; max = 900;
            }
            else if (rnd < 50)
            {
                min = 250; max = 800;
            }
            else
            {
                min = 100; max = 600;
            }

            min = (int)(min * scale);
            max = (int)(max * scale);
        }

        public static Item GetRandomRecipe()
        {
            List<Engines.Craft.Recipe> recipes = new List<Engines.Craft.Recipe>(Engines.Craft.Recipe.Recipes.Values);

            return new RecipeScroll(recipes[Utility.Random(recipes.Count)]);
        }

        public override bool CheckLocked(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
            {
                return false;
            }

            if (!TreasureMapInfo.NewSystem && Level == 0)
            {
                if (Guardians.Any(g => g.Alive))
                {
                    from.SendLocalizedMessage(1046448); // You must first kill the guardians before you may open this chest.
                    return true;
                }

                LockPick(from);
                return false;
            }
            else if (CanOpen(from))
            {
                return base.CheckLocked(from);
            }

            return true;
        }

        public virtual bool CanOpen(Mobile from)
        {
            if (TreasureMapInfo.NewSystem)
            {
                if (!Locked && TrapType != TrapType.None)
                {
                    from.SendLocalizedMessage(1159008); // That appears to be trapped, using the remove trap skill would yield better results...
                    return false;
                }
                else if (AncientGuardians.Any(ag => ag.Alive))
                {
                    from.SendLocalizedMessage(1046448); // You must first kill the guardians before you may open this chest.
                    return false;
                }
            }

            return !Locked;
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            return CheckLoot(from, item != this) && base.CheckItemUse(from, item);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            return CheckLoot(from, true) && base.CheckLift(from, item, ref reject);
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            bool notYetLifted = !m_Lifted.Contains(item);

            from.RevealingAction();

            if (notYetLifted)
            {
                m_Lifted.Add(item);

                if (0.1 >= Utility.RandomDouble()) // 10% chance to spawn a new monster
                {
                    BaseCreature spawn = TreasureMap.Spawn(Level, GetWorldLocation(), Map, from, false);

                    if (spawn != null)
                    {
                        spawn.Hue = 2725;
                    }
                }
            }

            base.OnItemLifted(from, item);
        }

        public void SpawnAncientGuardian(Mobile from)
        {
            ExecuteTrap(from);

            if (!AncientGuardians.Any(g => g != null && g.Alive))
            {
                BaseCreature spawn = TreasureMap.Spawn(Level, GetWorldLocation(), Map, from, false);

                if (spawn != null)
                {
                    spawn.NoLootOnDeath = true;

                    spawn.Name = "Ancient Chest Guardian";
                    spawn.Title = "(Guardian)";
                    spawn.Tamable = false;

                    if (spawn.HitsMaxSeed >= 0)
                        spawn.HitsMaxSeed = (int)(spawn.HitsMaxSeed * Paragon.HitsBuff);

                    spawn.RawStr = (int)(spawn.RawStr * Paragon.StrBuff);
                    spawn.RawInt = (int)(spawn.RawInt * Paragon.IntBuff);
                    spawn.RawDex = (int)(spawn.RawDex * Paragon.DexBuff);

                    spawn.Hits = spawn.HitsMax;
                    spawn.Mana = spawn.ManaMax;
                    spawn.Stam = spawn.StamMax;

                    spawn.Hue = 1960;

                    for (int i = 0; i < spawn.Skills.Length; i++)
                    {
                        Skill skill = spawn.Skills[i];

                        if (skill.Base > 0.0)
                            skill.Base *= Paragon.SkillsBuff;
                    }

                    AncientGuardians.Add(spawn);
                }
            }
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (m.AccessLevel < AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1048122, "", 0x8A5); // The chest refuses to be filled with treasure again.
                return false;
            }

            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(4); // version

            writer.Write(FailedLockpick);
            writer.Write((int)_Quality);
            writer.Write(DigTime);
            writer.Write(AncientGuardians, true);

            writer.Write(FirstOpenedByOwner);
            writer.Write(TreasureMap);

            writer.Write(Guardians, true);
            writer.Write(Temporary);

            writer.Write(Owner);

            writer.Write(Level);
            writer.WriteDeltaTime(DeleteTime);
            writer.Write(m_Lifted, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    FailedLockpick = reader.ReadBool();
                    _Quality = (ChestQuality)reader.ReadInt();
                    DigTime = reader.ReadDateTime();
                    AncientGuardians = reader.ReadStrongMobileList();
                    goto case 3;
                case 3:
                    FirstOpenedByOwner = reader.ReadBool();
                    TreasureMap = reader.ReadItem() as TreasureMap;
                    goto case 2;
                case 2:
                    {
                        Guardians = reader.ReadStrongMobileList();
                        Temporary = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                    {
                        Owner = reader.ReadMobile();

                        goto case 0;
                    }
                case 0:
                    {
                        Level = reader.ReadInt();
                        DeleteTime = reader.ReadDeltaTime();
                        m_Lifted = reader.ReadStrongItemList();

                        if (version < 2)
                            Guardians = new List<Mobile>();

                        break;
                    }
            }

            if (!Temporary && DeleteTime > DateTime.UtcNow)
            {
                TimerRegistry.Register("TreasureMapChest", this, DeleteTime - DateTime.UtcNow, chest => chest.Delete());
            }
            else
            {
                Delete();
            }
        }

        public override void OnAfterDelete()
        {
            if (Timer != null)
                Timer.Stop();

            Timer = null;

            base.OnAfterDelete();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
                list.Add(new RemoveEntry(from, this));
        }

        public override void LockPick(Mobile from)
        {
            base.LockPick(from);

            if (Map != null && ((TreasureMapInfo.NewSystem && FailedLockpick) || 0.05 >= Utility.RandomDouble()))
            {
                Grubber grubber = new Grubber();
                grubber.MoveToWorld(Map.GetSpawnPosition(Location, 1), Map);

                Item item = null;

                if (Items.Count > 0)
                {
                    do
                    {
                        item = Items[Utility.Random(Items.Count)];
                    }
                    while (item == null || item.LootType == LootType.Blessed);
                }

                grubber.PackItem(item);

                if (TreasureMapInfo.NewSystem)
                {
                    grubber.PrivateOverheadMessage(MessageType.Regular, 33, 1159062, from.NetState); // *A grubber appears and ganks a piece of your loot!*
                }
            }
        }

        public override void DisplayTo(Mobile to)
        {
            base.DisplayTo(to);

            if (!FirstOpenedByOwner && to == Owner)
            {
                if (TreasureMap != null)
                {
                    TreasureMap.OnChestOpened((PlayerMobile)to, this);
                }

                FirstOpenedByOwner = true;
            }
        }

        public override bool ExecuteTrap(Mobile from)
        {
            if (TreasureMapInfo.NewSystem && TrapType != TrapType.None)
            {
                int damage;

                if (TrapLevel > 0)
                    damage = Utility.RandomMinMax(10, 30) * TrapLevel;
                else
                    damage = TrapPower;

                AOS.Damage(from, damage, 0, 100, 0, 0, 0);

                // Your skin blisters from the heat!
                from.LocalOverheadMessage(MessageType.Regular, 0x2A, 503000);

                Effects.SendLocationEffect(from.Location, from.Map, 0x36BD, 15, 10);
                Effects.PlaySound(from.Location, from.Map, 0x307);

                return true;
            }
            else
            {
                return base.ExecuteTrap(from);
            }
        }

        public void BeginRemove(Mobile from)
        {
            if (!from.Alive)
                return;

            from.CloseGump(typeof(RemoveGump));
            from.SendGump(new RemoveGump(from, this));
        }

        public void EndRemove(Mobile from)
        {
            if (Deleted || from != Owner || !from.InRange(GetWorldLocation(), 3))
                return;

            from.SendLocalizedMessage(1048124, "", 0x8A5); // The old, rusted chest crumbles when you hit it.
            Delete();
        }

        private static void GetRandomAOSStats(out int attributeCount, out int min, out int max)
        {
            int rnd = Utility.Random(15);

            if (rnd < 1)
            {
                attributeCount = Utility.RandomMinMax(3, 5);
                min = 50;
                max = 100;
            }
            else if (rnd < 3)
            {
                attributeCount = Utility.RandomMinMax(2, 5);
                min = 40;
                max = 80;
            }
            else if (rnd < 6)
            {
                attributeCount = Utility.RandomMinMax(2, 4);
                min = 30;
                max = 60;
            }
            else if (rnd < 10)
            {
                attributeCount = Utility.RandomMinMax(1, 3);
                min = 20;
                max = 40;
            }
            else
            {
                attributeCount = 1;
                min = 10;
                max = 20;
            }
        }

        private bool CheckLoot(Mobile m, bool criminalAction)
        {
            if (Temporary)
                return false;

            if (m.AccessLevel >= AccessLevel.GameMaster || Owner == null || m == Owner)
                return true;

            Party p = Party.Get(Owner);

            if (p != null && p.Contains(m))
                return true;

            Map map = Map;

            if (map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0)
            {
                if (criminalAction)
                    m.CriminalAction(true);
                else
                    m.SendLocalizedMessage(1010630); // Taking someone else's treasure is a criminal offense!

                return true;
            }

            m.SendLocalizedMessage(1010631); // You did not discover this chest!
            return false;
        }

        private class RemoveGump : Gump
        {
            private readonly Mobile m_From;
            private readonly TreasureMapChest m_Chest;
            public RemoveGump(Mobile from, TreasureMapChest chest)
                : base(15, 15)
            {
                m_From = from;
                m_Chest = chest;

                Closable = false;
                Disposable = false;

                AddPage(0);

                AddBackground(30, 0, 240, 240, 2620);

                AddHtmlLocalized(45, 15, 200, 80, 1048125, 0xFFFFFF, false, false); // When this treasure chest is removed, any items still inside of it will be lost.
                AddHtmlLocalized(45, 95, 200, 60, 1048126, 0xFFFFFF, false, false); // Are you certain you're ready to remove this chest?

                AddButton(40, 153, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(75, 155, 180, 40, 1048127, 0xFFFFFF, false, false); // Remove the Treasure Chest

                AddButton(40, 195, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(75, 197, 180, 35, 1006045, 0xFFFFFF, false, false); // Cancel
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 1)
                    m_Chest.EndRemove(m_From);
            }
        }

        private class RemoveEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly TreasureMapChest m_Chest;
            public RemoveEntry(Mobile from, TreasureMapChest chest)
                : base(6149, 3)
            {
                m_From = from;
                m_Chest = chest;

                Enabled = (from == chest.Owner);
            }

            public override void OnClick()
            {
                if (m_Chest.Deleted || m_From != m_Chest.Owner || !m_From.CheckAlive())
                    return;

                m_Chest.BeginRemove(m_From);
            }
        }
    }
}
