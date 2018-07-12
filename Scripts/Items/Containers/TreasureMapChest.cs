using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class TreasureMapChest : LockableContainer
    {
        public static Type[] Artifacts { get { return m_Artifacts; } }
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

        public static Type[] ArtifactsLevelFiveToSeven { get { return m_LevelFiveToSeven; } }
        private static Type[] m_LevelFiveToSeven = new Type[]
        {
            typeof(ForgedPardon), typeof(ManaPhasingOrb), typeof(RunedSashOfWarding), typeof(SurgeShield)
        };

        public static Type[] ArtifactsLevelSeven { get { return m_LevelSevenOnly; } }
        private static Type[] m_LevelSevenOnly = new Type[]
        {
            typeof(CoffinPiece), typeof(MasterSkeletonKey)
        };

        public static Type[] SOSArtifacts { get { return m_SOSArtifacts; } }
        private static Type[] m_SOSArtifacts = new Type[]
        {
            typeof(AntiqueWeddingDress), 
            typeof(KelpWovenLeggings),   
            typeof(RunedDriftwoodBow),
            typeof(ValkyrieArmor)
        };
        public static Type[] SOSDecor { get { return m_SOSDecor; } }
        private static Type[] m_SOSDecor = new Type[]
        {
            typeof(GrapeVine),
            typeof(LargeFishingNet) 
        };


        private static Type[] m_ImbuingIngreds =
		{
			typeof(AbyssalCloth),   typeof(EssencePrecision), typeof(EssenceAchievement), typeof(EssenceBalance), 
			typeof(EssenceControl), typeof(EssenceDiligence), typeof(EssenceDirection),   typeof(EssenceFeeling), 
			typeof(EssenceOrder),   typeof(EssencePassion),   typeof(EssencePersistence), typeof(EssenceSingularity)
		};

        private int m_Level;
        private DateTime m_DeleteTime;
        private Timer m_Timer;
        private Mobile m_Owner;
        private bool m_Temporary;
        private bool m_FirstOpenedByOwner;
        private TreasureMap m_TreasureMap;
        private List<Mobile> m_Guardians;
        private List<Item> m_Lifted = new List<Item>();

        [Constructable]
        public TreasureMapChest(int level)
            : this(null, level, false)
        {
        }

        public TreasureMapChest(Mobile owner, int level, bool temporary)
            : base(0xE40)
        {
            m_Owner = owner;
            m_Level = level;
            m_DeleteTime = DateTime.UtcNow + TimeSpan.FromHours(3.0);

            m_Temporary = temporary;
            m_Guardians = new List<Mobile>();

            m_Timer = new DeleteTimer(this, m_DeleteTime);
            m_Timer.Start();

            int luck = 0;
            Map map = Map.Trammel;

            if (owner != null)
            {
                luck = owner is PlayerMobile ? ((PlayerMobile)owner).RealLuck : owner.Luck;
                map = owner.Map;
            }

            Fill(this, luck, level, false, map);
        }

        public TreasureMapChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 3000541;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return m_Owner;
            }
            set
            {
                m_Owner = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeleteTime
        {
            get
            {
                return m_DeleteTime;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Temporary
        {
            get
            {
                return m_Temporary;
            }
            set
            {
                m_Temporary = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool FirstOpenedByOwner
        {
            get
            {
                return m_FirstOpenedByOwner;
            }
            set
            {
                m_FirstOpenedByOwner = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TreasureMap TreasureMap
        {
            get
            {
                return m_TreasureMap;
            }
            set
            {
                m_TreasureMap = value;
            }
        }
        public List<Mobile> Guardians
        {
            get
            {
                return m_Guardians;
            }
        }
        public override bool IsDecoContainer
        {
            get
            {
                return false;
            }
        }

        public static void Fill(LockableContainer cont, int luck, int level, bool isSos, Map map)
        {
            cont.Movable = false;
            cont.Locked = true;
            int numberItems;

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

                cont.DropItem(new Gold(level * 5000));

                for (int i = 0; i < level * 5; ++i)
                    cont.DropItem(Loot.RandomScroll(0, 63, SpellbookType.Regular));

				double propsScale = 1.0;
                if (Core.SE)
                {
                    switch (level)
                    {
                        case 1:
                            numberItems = 32;
							propsScale = 0.5625;
                            break;
                        case 2:
                            numberItems = 40;
							propsScale = 0.6875;
                            break;
                        case 3:
                            numberItems = 48;
							propsScale = 0.875;
                            break;
                        case 4:
                            numberItems = 56;
                            break;
                        case 5:
                            numberItems = 64;
                            break;
                        case 6:
                            numberItems = 72;
                            break;
                        case 7:
                            numberItems = 80;
                            break;
                        default:
                            numberItems = 0;
                            break;
                    }
                }
                else
                    numberItems = level * 6;

                for (int i = 0; i < numberItems; ++i)
                {
                    Item item;

                    if (Core.AOS)
                        item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();
                    else
                        item = Loot.RandomArmorOrShieldOrWeapon();

                    if (item != null && Core.HS && RandomItemGenerator.Enabled)
                    {
                        int min, max;
                        GetRandomItemStat(out min, out max, propsScale);

                        RunicReforging.GenerateRandomItem(item, luck, min, max, map);

                        cont.DropItem(item);
                    }
                    else if (item is BaseWeapon)
                    {
                        BaseWeapon weapon = (BaseWeapon)item;

                        if (Core.AOS)
                        {
                            int attributeCount;
                            int min, max;

                            GetRandomAOSStats(out attributeCount, out min, out max);

                            BaseRunicTool.ApplyAttributesTo(weapon, attributeCount, min, max);
                        }
                        else
                        {
                            weapon.DamageLevel = (WeaponDamageLevel)Utility.Random(6);
                            weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                            weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                        }

                        cont.DropItem(item);
                    }
                    else if (item is BaseArmor)
                    {
                        BaseArmor armor = (BaseArmor)item;

                        if (Core.AOS)
                        {
                            int attributeCount;
                            int min, max;

                            GetRandomAOSStats(out attributeCount, out min, out max);

                            BaseRunicTool.ApplyAttributesTo(armor, attributeCount, min, max);
                        }
                        else
                        {
                            armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(6);
                            armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);
                        }

                        cont.DropItem(item);
                    }
                    else if (item is BaseHat)
                    {
                        BaseHat hat = (BaseHat)item;

                        if (Core.AOS)
                        {
                            int attributeCount;
                            int min, max;

                            GetRandomAOSStats(out attributeCount, out min, out max);

                            BaseRunicTool.ApplyAttributesTo(hat, attributeCount, min, max);
                        }

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

            int reagents;
            if (level == 0)
                reagents = 12;
            else
                reagents = level + 1;

            for (int i = 0; i < reagents; i++)
            {
                Item item = Loot.RandomPossibleReagent();
                item.Amount = Utility.RandomMinMax(40, 60);
                cont.DropItem(item);
            }

            int gems;
            if (level == 0)
                gems = 2;
            else
                gems = (level * 3) + 1;

            for (int i = 0; i < gems; i++)
            {
                Item item = Loot.RandomGem();
                cont.DropItem(item);
            }

            if (level > 1)
            {
                Item item = Loot.Construct(m_ImbuingIngreds[Utility.Random(m_ImbuingIngreds.Length)]);

                item.Amount = level;
                cont.DropItem(item);
            }

            Item arty = null;
            Item special = null;

            if (isSos)
            {
                if (0.004 * level > Utility.RandomDouble())
                    arty = Loot.Construct(m_SOSArtifacts);
                if (0.006 * level > Utility.RandomDouble())
                    special = Loot.Construct(m_SOSDecor);
                else if (0.009 * level > Utility.RandomDouble())
                    special = new TreasureMap(Utility.RandomMinMax(level, Math.Min(7, level + 1)), cont.Map);

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
                else if (.10 > Utility.RandomDouble())
                {
                    special = GetRandomSpecial(level, cont.Map);
                }
            }

            if (arty != null)
            {
                Container pack = new Backpack();
                pack.Hue = 1278;

                pack.DropItem(arty);
                cont.DropItem(pack);
            }

            if (special != null)
                cont.DropItem(special);

            if (Core.SA)
            {
                int rolls = 2;

                if (level >= 5)
                    rolls += level - 2;

                RefinementComponent.Roll(cont, rolls, 0.10);
            }
        }

        private static Item GetRandomSpecial(int level, Map map)
        {
            Item special;

            switch (Utility.Random(8))
            {
                default:
                case 0: special = new CreepingVine(); break;
                case 1: special = new MessageInABottle(); break;
                case 2: special = new ScrollofAlacrity(PowerScroll.Skills[Utility.Random(PowerScroll.Skills.Count)]); break;
                case 3: special = new Skeletonkey(); break;
                case 4: special = new TastyTreat(5); break;
                case 5: special = new TreasureMap(Utility.RandomMinMax(level, Math.Min(7, level + 1)), map); break;
                case 6: special = GetRandomRecipe(); break;
                case 7: special = ScrollofTranscendence.CreateRandom(1, 5); break;
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
            List<Server.Engines.Craft.Recipe> recipes = new List<Server.Engines.Craft.Recipe>(Server.Engines.Craft.Recipe.Recipes.Values);

            return new RecipeScroll(recipes[Utility.Random(recipes.Count)]);
        }

        public override bool CheckLocked(Mobile from)
        {
            if (!Locked)
                return false;

            if (Level == 0 && from.AccessLevel < AccessLevel.GameMaster)
            {
                foreach (Mobile m in Guardians)
                {
                    if (m.Alive)
                    {
                        from.SendLocalizedMessage(1046448); // You must first kill the guardians before you may open this chest.
                        return true;
                    }
                }

                LockPick(from);
                return false;
            }
            else
            {
                return base.CheckLocked(from);
            }
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
                    TreasureMap.Spawn(m_Level, GetWorldLocation(), Map, from, false);
            }

            base.OnItemLifted(from, item);
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

            writer.Write((int)3); // version

            writer.Write(m_FirstOpenedByOwner);
            writer.Write(m_TreasureMap);

            writer.Write(m_Guardians, true);
            writer.Write((bool)m_Temporary);

            writer.Write(m_Owner);

            writer.Write((int)m_Level);
            writer.WriteDeltaTime(m_DeleteTime);
            writer.Write(m_Lifted, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    m_FirstOpenedByOwner = reader.ReadBool();
                    m_TreasureMap = reader.ReadItem() as TreasureMap;
                    goto case 2;
                case 2:
                    {
                        m_Guardians = reader.ReadStrongMobileList();
                        m_Temporary = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                    {
                        m_Owner = reader.ReadMobile();

                        goto case 0;
                    }
                case 0:
                    {
                        m_Level = reader.ReadInt();
                        m_DeleteTime = reader.ReadDeltaTime();
                        m_Lifted = reader.ReadStrongItemList();

                        if (version < 2)
                            m_Guardians = new List<Mobile>();

                        break;
                    }
            }

            if (!m_Temporary)
            {
                m_Timer = new DeleteTimer(this, m_DeleteTime);
                m_Timer.Start();
            }
            else
            {
                Delete();
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

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

            if (Map != null && Core.SA && 0.05 >= Utility.RandomDouble())
            {
                var grubber = new Grubber();
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
            }
        }

        public override void DisplayTo(Mobile to)
        {
            base.DisplayTo(to);

            if (!m_FirstOpenedByOwner && to == m_Owner)
            {
                m_TreasureMap.OnChestOpened((PlayerMobile)to, this);
                m_FirstOpenedByOwner = true;
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
            if (Deleted || from != m_Owner || !from.InRange(GetWorldLocation(), 3))
                return;

            from.SendLocalizedMessage(1048124, "", 0x8A5); // The old, rusted chest crumbles when you hit it.
            Delete();
        }

        private static void GetRandomAOSStats(out int attributeCount, out int min, out int max)
        {
            int rnd = Utility.Random(15);

            if (Core.SE)
            {
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
            else
            {
                if (rnd < 1)
                {
                    attributeCount = Utility.RandomMinMax(2, 5);
                    min = 20;
                    max = 70;
                }
                else if (rnd < 3)
                {
                    attributeCount = Utility.RandomMinMax(2, 4);
                    min = 20;
                    max = 50;
                }
                else if (rnd < 6)
                {
                    attributeCount = Utility.RandomMinMax(2, 3);
                    min = 20;
                    max = 40;
                }
                else if (rnd < 10)
                {
                    attributeCount = Utility.RandomMinMax(1, 2);
                    min = 10;
                    max = 30;
                }
                else
                {
                    attributeCount = 1;
                    min = 10;
                    max = 20;
                }
            }
        }

        private bool CheckLoot(Mobile m, bool criminalAction)
        {
            if (m_Temporary)
                return false;

            if (m.AccessLevel >= AccessLevel.GameMaster || m_Owner == null || m == m_Owner)
                return true;

            Party p = Party.Get(m_Owner);

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

        private class DeleteTimer : Timer
        {
            private readonly Item m_Item;
            public DeleteTimer(Item item, DateTime time)
                : base(time - DateTime.UtcNow)
            {
                m_Item = item;
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                m_Item.Delete();
            }
        }
    }
}
