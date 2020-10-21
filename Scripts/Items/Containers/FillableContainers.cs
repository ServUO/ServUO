using Server.Commands;
using Server.Mobiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public enum FillableContentType
    {
        None = -1,
        Weaponsmith,
        Provisioner,
        Mage,
        Alchemist,
        Armorer,
        ArtisanGuild,
        Baker,
        Bard,
        Blacksmith,
        Bowyer,
        Butcher,
        Carpenter,
        Clothier,
        Cobbler,
        Docks,
        Farm,
        FighterGuild,
        Guard,
        Healer,
        Herbalist,
        Inn,
        Jeweler,
        Library,
        Merchant,
        Mill,
        Mine,
        Observatory,
        Painter,
        Ranger,
        Stables,
        Tanner,
        Tavern,
        ThiefGuild,
        Tinker,
        Veterinarian
    }

    public abstract class FillableContainer : LockableContainer
    {
        public static void Initialize()
        {
            CommandSystem.Register("CheckFillables", AccessLevel.Administrator, CheckFillables_OnCommand);

            CheckPostLoad();
        }

        private static List<FillableContainer> _PostLoadCheck = new List<FillableContainer>();

        public static void CheckPostLoad()
        {
            for (int i = 0; i < _PostLoadCheck.Count; i++)
            {
                _PostLoadCheck[i].CheckRespawn();
            }

            ColUtility.Free(_PostLoadCheck);
            _PostLoadCheck = null;
        }

        public static void CheckFillables_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            int count = 0;
            int fail = 0;

            List<FillableContainer> toCheck = new List<FillableContainer>(World.Items.Values.OfType<FillableContainer>().Where(i => i is FillableContainer && i.ContentType == FillableContentType.None));

            foreach (FillableContainer cont in toCheck)
            {
                cont.AcquireContent();

                if (cont.ContentType == FillableContentType.None)
                    fail++;

                count++;
            }

            toCheck.Clear();
            toCheck.TrimExcess();
            m.SendMessage("Fixed {0} fillable containers, while {1} failed.", count, fail);
        }

        private static readonly string _RespawnTimerID = "FillableContainerTimer";

        protected FillableContent m_Content;
        protected DateTime m_NextRespawnTime;

        public FillableContainer(int itemID)
            : base(itemID)
        {
            Movable = false;

            MaxSpawnCount = Utility.RandomMinMax(3, 5);
        }

        public FillableContainer(Serial serial)
            : base(serial)
        {
        }

        public virtual int MinRespawnMinutes => 5;
        public virtual int MaxRespawnMinutes => 30;
        public virtual bool IsLockable => true;
        public virtual bool IsTrapable => IsLockable;
        public virtual int SpawnThreshold => MaxSpawnCount - 1;

        public virtual int AmountPerSpawn => 1;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSpawnCount { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalTraps { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextRespawnTime
        {
            get
            {
                return m_NextRespawnTime;
            }
            set
            {
                m_NextRespawnTime = value;

                if (m_NextRespawnTime > DateTime.UtcNow)
                {
                    StartTimer(m_NextRespawnTime - DateTime.UtcNow);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public FillableContentType ContentType
        {
            get
            {
                return FillableContent.Lookup(m_Content);
            }
            set
            {
                Content = FillableContent.Lookup(value);
            }
        }

        public FillableContent Content
        {
            get
            {
                return m_Content;
            }
            set
            {
                if (m_Content == value)
                    return;

                m_Content = value;

                for (int i = Items.Count - 1; i >= 0; --i)
                {
                    if (i < Items.Count)
                        Items[i].Delete();
                }

                Respawn(true);
            }
        }

        private void StartTimer(TimeSpan delay)
        {
            TimerRegistry.Register(_RespawnTimerID, this, delay, TimeSpan.FromMinutes(1), fc => fc.Respawn());
        }

        private void RemoveTimer()
        {
            TimerRegistry.RemoveFromRegistry(_RespawnTimerID, this);
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            AcquireContent();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);
            AcquireContent();
        }

        public virtual void AcquireContent()
        {
            if (m_Content != null)
                return;

            m_Content = FillableContent.Acquire(GetWorldLocation(), Map);

            if (m_Content != null)
                Respawn();
        }

        public override void OnItemRemoved(Item item)
        {
            CheckRespawn();
        }

        public int GetItemsCount()
        {
            int count = 0;

            foreach (Item item in Items)
            {
                count += item.Amount;
            }

            return count;
        }

        public void CheckRespawn()
        {
            bool canSpawn = (m_Content != null && !Deleted && GetItemsCount() <= SpawnThreshold && !Movable && Parent == null && !IsLockedDown && !IsSecure);

            if (canSpawn)
            {
                if (!TimerRegistry.HasTimer(_RespawnTimerID, this))
                {
                    StartTimer(TimeSpan.FromMinutes(Utility.RandomMinMax(MinRespawnMinutes, MaxRespawnMinutes)));
                }
            }
            else
            {
                RemoveTimer();
            }
        }

        public void Respawn()
        {
            Respawn(false);
        }

        public void Respawn(bool all)
        {
            if (m_Content == null || Deleted)
                return;

            GenerateContent(all);

            if (IsLockable && !Locked)
            {
                Locked = true;

                int difficulty = (m_Content.Level - 1) * 30;

                LockLevel = difficulty - 10;
                MaxLockLevel = difficulty + 30;
                RequiredSkill = difficulty;
            }

            if (IsTrapable && (m_Content.Level > 1 || 4 > Utility.Random(5)))
            {
                ResetTrap();

                TotalTraps = 1;

                if (0.25 > Utility.RandomDouble())
                {
                    TotalTraps++;

                    if (0.25 > Utility.RandomDouble())
                    {
                        TotalTraps++;
                    }
                }
            }
            else
            {
                TrapType = TrapType.None;
                TrapPower = 0;
                TrapLevel = 0;
            }

            CheckRespawn();
        }

        public virtual bool CanSpawnRefinement()
        {
            return Map == Map.Felucca && (ContentType == FillableContentType.Clothier || ContentType == FillableContentType.Blacksmith || ContentType == FillableContentType.Carpenter);
        }

        public virtual void GenerateContent(bool all)
        {
            if (m_Content == null || Deleted)
                return;

            int toSpawn = GetSpawnCount(all);

            bool canspawnRefinement = GetAmount(typeof(RefinementComponent)) == 0 && CanSpawnRefinement();

            for (int i = 0; i < toSpawn; ++i)
            {
                if (canspawnRefinement && RefinementComponent.Roll(this, 1, 0.08))
                {
                    canspawnRefinement = false;
                    continue;
                }

                Item item = m_Content.Construct();

                if (item != null)
                {
                    List<Item> list = Items;

                    for (int j = 0; j < list.Count; ++j)
                    {
                        Item subItem = list[j];

                        if (!(subItem is Container) && subItem.StackWith(null, item, false))
                            break;
                    }

                    if (item != null && !item.Deleted)
                        DropItem(item);
                }
            }
        }

        public override bool ExecuteTrap(Mobile from)
        {
            bool execute = base.ExecuteTrap(from);

            if (execute && --TotalTraps > 0)
            {
                ResetTrap();
            }

            return execute;
        }

        public void ResetTrap()
        {
            if (m_Content.Level > Utility.Random(5))
                TrapType = TrapType.PoisonTrap;
            else
                TrapType = TrapType.ExplosionTrap;

            TrapPower = m_Content.Level * Utility.RandomMinMax(10, 30);
            TrapLevel = m_Content.Level;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(2); // version

            writer.Write(TotalTraps);
            writer.Write(MaxSpawnCount);

            writer.Write((int)ContentType);

            if (TimerRegistry.HasTimer(_RespawnTimerID, this))
            {
                writer.Write(true);
                writer.WriteDeltaTime(m_NextRespawnTime);
            }
            else
            {
                writer.Write(false);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 1)
            {
                MaxSpawnCount = Utility.RandomMinMax(3, 5);
                TotalTraps = 1;
            }

            switch (version)
            {
                case 2:
                    {
                        TotalTraps = reader.ReadInt();
                        MaxSpawnCount = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Content = FillableContent.Lookup((FillableContentType)reader.ReadInt());
                        goto case 0;
                    }
                case 0:
                    {
                        if (reader.ReadBool())
                        {
                            m_NextRespawnTime = reader.ReadDeltaTime();

                            StartTimer(m_NextRespawnTime - DateTime.UtcNow);
                        }
                        else
                        {
                            _PostLoadCheck.Add(this);
                        }

                        break;
                    }
            }
        }

        protected virtual int GetSpawnCount(bool all)
        {
            int itemsCount = GetItemsCount();

            if (itemsCount >= MaxSpawnCount)
                return 0;

            return all ? (MaxSpawnCount - itemsCount) : AmountPerSpawn;
        }
    }

    [Flipable(0xA97, 0xA99, 0xA98, 0xA9A, 0xA9B, 0xA9C)]
    public class LibraryBookcase : FillableContainer
    {
        [Constructable]
        public LibraryBookcase()
            : base(0xA97)
        {
            Weight = 1.0;

            MaxSpawnCount = 5;
        }

        public LibraryBookcase(Serial serial)
            : base(serial)
        {
        }

        public override bool IsLockable => false;

        public override void AcquireContent()
        {
            if (m_Content != null)
                return;

            m_Content = FillableContent.Library;

            if (m_Content != null)
                Respawn();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0 && m_Content == null)
                Timer.DelayCall(TimeSpan.Zero, AcquireContent);

            if (version == 1)
                MaxSpawnCount = 5;
        }

        protected override int GetSpawnCount(bool all)
        {
            return (MaxSpawnCount - GetItemsCount());
        }
    }

    [Flipable(0xE3D, 0xE3C)]
    public class FillableLargeCrate : FillableContainer
    {
        [Constructable]
        public FillableLargeCrate()
            : base(0xE3D)
        {
            Weight = 1.0;
        }

        public FillableLargeCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [Flipable(0x9A9, 0xE7E)]
    public class FillableSmallCrate : FillableContainer
    {
        [Constructable]
        public FillableSmallCrate()
            : base(0x9A9)
        {
            Weight = 1.0;
        }

        public FillableSmallCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [Flipable(0x9AA, 0xE7D)]
    public class FillableWoodenBox : FillableContainer
    {
        [Constructable]
        public FillableWoodenBox()
            : base(0x9AA)
        {
            Weight = 4.0;
        }

        public FillableWoodenBox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x9A8, 0xE80)]
    public class FillableMetalBox : FillableContainer
    {
        [Constructable]
        public FillableMetalBox()
            : base(0x9A8)
        {
        }

        public FillableMetalBox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class FillableBarrel : FillableContainer
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D WorldLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map WorldMap { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextReturn { get; set; }

        [Constructable]
        public FillableBarrel()
            : base(0xE77)
        {
        }

        public FillableBarrel(Serial serial)
            : base(serial)
        {
        }

        public override bool IsLockable => false;

        public void Pour(Mobile from, BaseBeverage beverage)
        {
            if (beverage.Content == BeverageType.Water)
            {
                if (Items.Count > 0)
                {
                    from.SendLocalizedMessage(500848); // Couldn't pour it there.  It was already full.
                    beverage.PrivateOverheadMessage(Network.MessageType.Regular, 0, 500841, from.NetState); // that has somethign in it.
                }
                else
                {
                    WaterBarrel barrel = new WaterBarrel
                    {
                        Movable = false
                    };
                    barrel.MoveToWorld(Location, Map);

                    WorldLocation = Location;
                    WorldMap = Map;
                    NextReturn = DateTime.UtcNow + TimeSpan.FromHours(1);

                    beverage.Pour_OnTarget(from, barrel);

                    Internalize();
                }
            }
        }

        public void TryReturn()
        {
            if (WorldMap != null)
            {
                IPooledEnumerable eable = WorldMap.GetItemsInRange(WorldLocation, 0);

                foreach (Item item in eable)
                {
                    if (item is WaterBarrel && item.Z == WorldLocation.Z)
                    {
                        eable.Free();
                        return;
                    }
                }

                eable.Free();
                NextReturn = DateTime.MinValue;
                MoveToWorld(WorldLocation, WorldMap);
                Respawn();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(2); // version

            writer.Write(WorldLocation);
            writer.Write(WorldMap);

            if (NextReturn != DateTime.MinValue && NextReturn < DateTime.UtcNow)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(20), TryReturn);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 2:
                    WorldLocation = reader.ReadPoint3D();
                    WorldMap = reader.ReadMap();
                    break;
            }

            if (Map == Map.Internal)
            {
                if (WorldMap != null)
                {
                    NextReturn = DateTime.UtcNow;
                    Timer.DelayCall(TimeSpan.FromSeconds(20), TryReturn);
                }
                else
                {
                    Delete();
                }
            }
        }
    }

    [Flipable(0x9AB, 0xE7C)]
    public class FillableMetalChest : FillableContainer
    {
        [Constructable]
        public FillableMetalChest()
            : base(0x9AB)
        {
        }

        public FillableMetalChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(0xE41, 0xE40)]
    public class FillableMetalGoldenChest : FillableContainer
    {
        [Constructable]
        public FillableMetalGoldenChest()
            : base(0xE41)
        {
        }

        public FillableMetalGoldenChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(0xE43, 0xE42)]
    public class FillableWoodenChest : FillableContainer
    {
        [Constructable]
        public FillableWoodenChest()
            : base(0xE43)
        {
        }

        public FillableWoodenChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class FillableEntry
    {
        protected Type[] m_Types;
        protected int m_Weight;
        public FillableEntry(Type type)
            : this(1, new Type[] { type })
        {
        }

        public FillableEntry(int weight, Type type)
            : this(weight, new Type[] { type })
        {
        }

        public FillableEntry(Type[] types)
            : this(1, types)
        {
        }

        public FillableEntry(int weight, Type[] types)
        {
            m_Weight = weight;
            m_Types = types;
        }

        public FillableEntry(int weight, Type[] types, int offset, int count)
        {
            m_Weight = weight;
            m_Types = new Type[count];

            for (int i = 0; i < m_Types.Length; ++i)
                m_Types[i] = types[offset + i];
        }

        public Type[] Types => m_Types;
        public int Weight => m_Weight;
        public virtual Item Construct()
        {
            Item item = Loot.Construct(m_Types);

            if (item is Key)
                ((Key)item).ItemID = Utility.RandomList((int)KeyType.Copper, (int)KeyType.Gold, (int)KeyType.Iron, (int)KeyType.Rusty);
            else if (item is Arrow || item is Bolt)
                item.Amount = Utility.RandomMinMax(2, 6);
            else if (item is Bandage || item is Lockpick)
                item.Amount = Utility.RandomMinMax(1, 3);

            return item;
        }
    }

    public class FillableBvrge : FillableEntry
    {
        private readonly BeverageType m_Content;
        public FillableBvrge(Type type, BeverageType content)
            : this(1, type, content)
        {
        }

        public FillableBvrge(int weight, Type type, BeverageType content)
            : base(weight, type)
        {
            m_Content = content;
        }

        public BeverageType Content => m_Content;
        public override Item Construct()
        {
            Item item;

            int index = Utility.Random(m_Types.Length);

            if (m_Types[index] == typeof(BeverageBottle))
            {
                item = new BeverageBottle(m_Content);
            }
            else if (m_Types[index] == typeof(Jug))
            {
                item = new Jug(m_Content);
            }
            else
            {
                item = base.Construct();

                if (item is BaseBeverage)
                {
                    BaseBeverage bev = (BaseBeverage)item;

                    bev.Content = m_Content;
                    bev.Quantity = bev.MaxQuantity;
                }
            }

            return item;
        }
    }

    public class FillableContent
    {
        public static FillableContent Alchemist = new FillableContent(
            1,
            new Type[]
            {
                typeof(Alchemist)
            },
            new FillableEntry[]
            {
                new FillableEntry(typeof(NightSightPotion)),
                new FillableEntry(typeof(LesserCurePotion)),
                new FillableEntry(typeof(AgilityPotion)),
                new FillableEntry(typeof(StrengthPotion)),
                new FillableEntry(typeof(LesserPoisonPotion)),
                new FillableEntry(typeof(RefreshPotion)),
                new FillableEntry(typeof(LesserHealPotion)),
                new FillableEntry(typeof(LesserExplosionPotion)),
                new FillableEntry(typeof(MortarPestle))
            });
        public static FillableContent Armorer = new FillableContent(
            2,
            new Type[]
            {
                typeof(Armorer)
            },
            new FillableEntry[]
            {
                new FillableEntry(2, typeof(ChainCoif)),
                new FillableEntry(1, typeof(PlateGorget)),
                new FillableEntry(1, typeof(BronzeShield)),
                new FillableEntry(1, typeof(Buckler)),
                new FillableEntry(2, typeof(MetalKiteShield)),
                new FillableEntry(2, typeof(HeaterShield)),
                new FillableEntry(1, typeof(WoodenShield)),
                new FillableEntry(1, typeof(MetalShield))
            });
        public static FillableContent ArtisanGuild = new FillableContent(
            1,
            new Type[]
            {
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(PaintsAndBrush)),
                new FillableEntry(1, typeof(SledgeHammer)),
                new FillableEntry(2, typeof(SmithHammer)),
                new FillableEntry(2, typeof(Tongs)),
                new FillableEntry(4, typeof(Lockpick)),
                new FillableEntry(4, typeof(TinkerTools)),
                new FillableEntry(1, typeof(MalletAndChisel)),
                new FillableEntry(1, typeof(StatueEast2)),
                new FillableEntry(1, typeof(StatueSouth)),
                new FillableEntry(1, typeof(StatueSouthEast)),
                new FillableEntry(1, typeof(StatueWest)),
                new FillableEntry(1, typeof(StatueNorth)),
                new FillableEntry(1, typeof(StatueEast)),
                new FillableEntry(1, typeof(BustEast)),
                new FillableEntry(1, typeof(BustSouth)),
                new FillableEntry(1, typeof(BearMask)),
                new FillableEntry(1, typeof(DeerMask)),
                new FillableEntry(4, typeof(OrcHelm)),
                new FillableEntry(1, typeof(TribalMask)),
                new FillableEntry(1, typeof(HornedTribalMask))
            });
        public static FillableContent Baker = new FillableContent(
            1,
            new Type[]
            {
                typeof(Baker),
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(RollingPin)),
                new FillableEntry(2, typeof(SackFlour)),
                new FillableEntry(2, typeof(BreadLoaf)),
                new FillableEntry(1, typeof(FrenchBread))
            });
        public static FillableContent Bard = new FillableContent(
            1,
            new Type[]
            {
                typeof(Bard),
                typeof(BardGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(LapHarp)),
                new FillableEntry(2, typeof(Lute)),
                new FillableEntry(1, typeof(Drums)),
                new FillableEntry(1, typeof(Tambourine)),
                new FillableEntry(1, typeof(TambourineTassel))
            });
        public static FillableContent Blacksmith = new FillableContent(
            2,
            new Type[]
            {
                typeof(Blacksmith),
                typeof(BlacksmithGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(8, typeof(SmithHammer)),
                new FillableEntry(8, typeof(Tongs)),
                new FillableEntry(8, typeof(SledgeHammer)),
                new FillableEntry(8, typeof(IronIngot)),
                new FillableEntry(1, typeof(IronWire)),
                new FillableEntry(1, typeof(SilverWire)),
                new FillableEntry(1, typeof(GoldWire)),
                new FillableEntry(1, typeof(CopperWire)),
                new FillableEntry(1, typeof(HorseShoes)),
                new FillableEntry(1, typeof(ForgedMetal))
            });
        public static FillableContent Bowyer = new FillableContent(
            2,
            new Type[]
            {
                typeof(Bowyer)
            },
            new FillableEntry[]
            {
                new FillableEntry(2, typeof(Bow)),
                new FillableEntry(2, typeof(Crossbow)),
                new FillableEntry(1, typeof(Arrow))
            });
        public static FillableContent Butcher = new FillableContent(
            1,
            new Type[]
            {
                typeof(Butcher),
            },
            new FillableEntry[]
            {
                new FillableEntry(2, typeof(Cleaver)),
                new FillableEntry(2, typeof(SlabOfBacon)),
                new FillableEntry(2, typeof(Bacon)),
                new FillableEntry(1, typeof(RawFishSteak)),
                new FillableEntry(1, typeof(FishSteak)),
                new FillableEntry(2, typeof(CookedBird)),
                new FillableEntry(2, typeof(RawBird)),
                new FillableEntry(2, typeof(Ham)),
                new FillableEntry(1, typeof(RawLambLeg)),
                new FillableEntry(1, typeof(LambLeg)),
                new FillableEntry(1, typeof(Ribs)),
                new FillableEntry(1, typeof(RawRibs)),
                new FillableEntry(2, typeof(Sausage)),
                new FillableEntry(1, typeof(RawChickenLeg)),
                new FillableEntry(1, typeof(ChickenLeg))
            });
        public static FillableContent Carpenter = new FillableContent(
            1,
            new Type[]
            {
                typeof(Carpenter),
                typeof(Architect),
                typeof(RealEstateBroker)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(ChiselsNorth)),
                new FillableEntry(1, typeof(ChiselsWest)),
                new FillableEntry(2, typeof(DovetailSaw)),
                new FillableEntry(2, typeof(Hammer)),
                new FillableEntry(2, typeof(MouldingPlane)),
                new FillableEntry(2, typeof(Nails)),
                new FillableEntry(2, typeof(JointingPlane)),
                new FillableEntry(2, typeof(SmoothingPlane)),
                new FillableEntry(2, typeof(Saw)),
                new FillableEntry(2, typeof(DrawKnife)),
                new FillableEntry(1, typeof(Log)),
                new FillableEntry(1, typeof(Froe)),
                new FillableEntry(1, typeof(Inshave)),
                new FillableEntry(1, typeof(Scorp))
            });
        public static FillableContent Clothier = new FillableContent(
            1,
            new Type[]
            {
                typeof(Tailor),
                typeof(Weaver),
                typeof(TailorGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(Cotton)),
                new FillableEntry(1, typeof(Wool)),
                new FillableEntry(1, typeof(DarkYarn)),
                new FillableEntry(1, typeof(LightYarn)),
                new FillableEntry(1, typeof(LightYarnUnraveled)),
                new FillableEntry(1, typeof(SpoolOfThread)),
                new FillableEntry(1, typeof(Dyes)),
                new FillableEntry(2, typeof(Leather))
            });
        public static FillableContent Cobbler = new FillableContent(
            1,
            new Type[]
            {
                typeof(Cobbler)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(Boots)),
                new FillableEntry(2, typeof(Shoes)),
                new FillableEntry(2, typeof(Sandals)),
                new FillableEntry(1, typeof(ThighBoots))
            });
        public static FillableContent Docks = new FillableContent(
            1,
            new Type[]
            {
                typeof(Fisherman),
                typeof(FisherGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(FishingPole)),
                // Two different types
                new FillableEntry( 1, typeof( SmallFish ) ),
                new FillableEntry( 1, typeof( SmallFish ) ),
                new FillableEntry(4, typeof(Fish))
            });
        public static FillableContent Farm = new FillableContent(
            1,
            new Type[]
            {
                typeof(Farmer),
                typeof(Rancher)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(Shirt)),
                new FillableEntry(1, typeof(ShortPants)),
                new FillableEntry(1, typeof(Skirt)),
                new FillableEntry(1, typeof(PlainDress)),
                new FillableEntry(1, typeof(Cap)),
                new FillableEntry(2, typeof(Sandals)),
                new FillableEntry(2, typeof(GnarledStaff)),
                new FillableEntry(2, typeof(Pitchfork)),
                new FillableEntry(1, typeof(Bag)),
                new FillableEntry(1, typeof(Kindling)),
                new FillableEntry(1, typeof(Lettuce)),
                new FillableEntry(1, typeof(Onion)),
                new FillableEntry(1, typeof(Turnip)),
                new FillableEntry(1, typeof(Ham)),
                new FillableEntry(1, typeof(Bacon)),
                new FillableEntry(1, typeof(RawLambLeg)),
                new FillableEntry(1, typeof(SheafOfHay)),
                new FillableBvrge(1, typeof(Pitcher), BeverageType.Milk)
            });
        public static FillableContent FighterGuild = new FillableContent(
            3,
            new Type[]
            {
                typeof(WarriorGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(12, Loot.ArmorTypes),
                new FillableEntry(8, Loot.WeaponTypes),
                new FillableEntry(3, Loot.ShieldTypes),
                new FillableEntry(1, typeof(Arrow))
            });
        public static FillableContent Guard = new FillableContent(
            3,
            new Type[]
            {
            },
            new FillableEntry[]
            {
                new FillableEntry(12, Loot.ArmorTypes),
                new FillableEntry(8, Loot.WeaponTypes),
                new FillableEntry(3, Loot.ShieldTypes),
                new FillableEntry(1, typeof(Arrow))
            });
        public static FillableContent Healer = new FillableContent(
            1,
            new Type[]
            {
                typeof(Healer),
                typeof(HealerGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(Bandage)),
                new FillableEntry(1, typeof(MortarPestle)),
                new FillableEntry(1, typeof(LesserHealPotion))
            });
        public static FillableContent Herbalist = new FillableContent(
            1,
            new Type[]
            {
                typeof(Herbalist)
            },
            new FillableEntry[]
            {
                new FillableEntry(10, typeof(Garlic)),
                new FillableEntry(10, typeof(Ginseng)),
                new FillableEntry(10, typeof(MandrakeRoot)),
                new FillableEntry(1, typeof(WhiteDriedFlowers)),
                new FillableEntry(1, typeof(GreenDriedFlowers)),
                new FillableEntry(1, typeof(DriedOnions)),
                new FillableEntry(1, typeof(DriedHerbs))
            });
        public static FillableContent Inn = new FillableContent(
            1,
            new Type[]
            {
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(Candle)),
                new FillableEntry(1, typeof(Torch)),
                new FillableEntry(1, typeof(Lantern))
            });
        public static FillableContent Jeweler = new FillableContent(
            2,
            new Type[]
            {
                typeof(Jeweler)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(GoldRing)),
                new FillableEntry(1, typeof(GoldBracelet)),
                new FillableEntry(1, typeof(GoldEarrings)),
                new FillableEntry(1, typeof(GoldNecklace)),
                new FillableEntry(1, typeof(GoldBeadNecklace)),
                new FillableEntry(1, typeof(Necklace)),
                new FillableEntry(1, typeof(Beads)),
                new FillableEntry(9, Loot.GemTypes)
            });
        public static FillableContent Library = new FillableContent(
            1,
            new Type[]
            {
                typeof(Scribe)
            },
            new FillableEntry[]
            {
                new FillableEntry(8, Loot.LibraryBookTypes),
                new FillableEntry(1, typeof(RedBook)),
                new FillableEntry(1, typeof(BlueBook))
            });
        public static FillableContent Mage = new FillableContent(
            2,
            new Type[]
            {
                typeof(Mage),
                typeof(HolyMage),
                typeof(MageGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(16, typeof(BlankScroll)),
                new FillableEntry(14, typeof(Spellbook)),
                new FillableEntry(12, Loot.MageryScrollTypes, 0, 8),
                new FillableEntry(11, Loot.MageryScrollTypes, 8, 8),
                new FillableEntry(10, Loot.MageryScrollTypes, 16, 8),
                new FillableEntry(9, Loot.MageryScrollTypes, 24, 8),
                new FillableEntry(8, Loot.MageryScrollTypes, 32, 8),
                new FillableEntry(7, Loot.MageryScrollTypes, 40, 8),
                new FillableEntry(6, Loot.MageryScrollTypes, 48, 8),
                new FillableEntry(5, Loot.MageryScrollTypes, 56, 8)
            });
        public static FillableContent Merchant = new FillableContent(
            1,
            new Type[]
            {
                typeof(MerchantGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(CheeseWheel)),
                new FillableEntry(1, typeof(CheeseWedge)),
                new FillableEntry(1, typeof(CheeseSlice)),
                new FillableEntry(1, typeof(Eggs)),
                new FillableEntry(4, typeof(Fish)),
                new FillableEntry(2, typeof(RawFishSteak)),
                new FillableEntry(2, typeof(FishSteak)),
                new FillableEntry(1, typeof(Apple)),
                new FillableEntry(2, typeof(Banana)),
                new FillableEntry(2, typeof(Bananas)),
                new FillableEntry(2, typeof(OpenCoconut)),
                new FillableEntry(1, typeof(SplitCoconut)),
                new FillableEntry(1, typeof(Coconut)),
                new FillableEntry(1, typeof(Dates)),
                new FillableEntry(1, typeof(Grapes)),
                new FillableEntry(1, typeof(Lemon)),
                new FillableEntry(1, typeof(Lemons)),
                new FillableEntry(1, typeof(Lime)),
                new FillableEntry(1, typeof(Limes)),
                new FillableEntry(1, typeof(Peach)),
                new FillableEntry(1, typeof(Pear)),
                new FillableEntry(2, typeof(SlabOfBacon)),
                new FillableEntry(2, typeof(Bacon)),
                new FillableEntry(2, typeof(CookedBird)),
                new FillableEntry(2, typeof(RawBird)),
                new FillableEntry(2, typeof(Ham)),
                new FillableEntry(1, typeof(RawLambLeg)),
                new FillableEntry(1, typeof(LambLeg)),
                new FillableEntry(1, typeof(Ribs)),
                new FillableEntry(1, typeof(RawRibs)),
                new FillableEntry(2, typeof(Sausage)),
                new FillableEntry(1, typeof(RawChickenLeg)),
                new FillableEntry(1, typeof(ChickenLeg)),
                new FillableEntry(1, typeof(Watermelon)),
                new FillableEntry(1, typeof(SmallWatermelon)),
                new FillableEntry(3, typeof(Turnip)),
                new FillableEntry(2, typeof(YellowGourd)),
                new FillableEntry(2, typeof(GreenGourd)),
                new FillableEntry(2, typeof(Pumpkin)),
                new FillableEntry(1, typeof(SmallPumpkin)),
                new FillableEntry(2, typeof(Onion)),
                new FillableEntry(2, typeof(Lettuce)),
                new FillableEntry(2, typeof(Squash)),
                new FillableEntry(2, typeof(HoneydewMelon)),
                new FillableEntry(1, typeof(Carrot)),
                new FillableEntry(2, typeof(Cantaloupe)),
                new FillableEntry(2, typeof(Cabbage)),
                new FillableEntry(4, typeof(EarOfCorn))
            });
        public static FillableContent Mill = new FillableContent(
            1,
            new Type[]
            {
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(SackFlour))
            });
        public static FillableContent Mine = new FillableContent(
            1,
            new Type[]
            {
                typeof(Miner)
            },
            new FillableEntry[]
            {
                new FillableEntry(2, typeof(Pickaxe)),
                new FillableEntry(2, typeof(Shovel)),
                new FillableEntry(2, typeof(IronIngot)),
                new FillableEntry(1, typeof(ForgedMetal))
            });
        public static FillableContent Observatory = new FillableContent(
            1,
            new Type[]
            {
            },
            new FillableEntry[]
            {
                new FillableEntry(2, typeof(Sextant)),
                new FillableEntry(2, typeof(Clock)),
                new FillableEntry(1, typeof(Spyglass))
            });
        public static FillableContent Painter = new FillableContent(
            1,
            new Type[]
            {
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(PaintsAndBrush)),
                new FillableEntry(2, typeof(PenAndInk))
            });
        public static FillableContent Provisioner = new FillableContent(
            1,
            new Type[]
            {
                typeof(Provisioner)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(CheeseWheel)),
                new FillableEntry(1, typeof(CheeseWedge)),
                new FillableEntry(1, typeof(CheeseSlice)),
                new FillableEntry(1, typeof(Eggs)),
                new FillableEntry(4, typeof(Fish)),
                new FillableEntry(1, typeof(Apple)),
                new FillableEntry(2, typeof(Banana)),
                new FillableEntry(2, typeof(Bananas)),
                new FillableEntry(2, typeof(OpenCoconut)),
                new FillableEntry(1, typeof(SplitCoconut)),
                new FillableEntry(1, typeof(Coconut)),
                new FillableEntry(1, typeof(Dates)),
                new FillableEntry(1, typeof(Grapes)),
                new FillableEntry(1, typeof(Lemon)),
                new FillableEntry(1, typeof(Lemons)),
                new FillableEntry(1, typeof(Lime)),
                new FillableEntry(1, typeof(Limes)),
                new FillableEntry(1, typeof(Peach)),
                new FillableEntry(1, typeof(Pear)),
                new FillableEntry(2, typeof(SlabOfBacon)),
                new FillableEntry(2, typeof(Bacon)),
                new FillableEntry(1, typeof(RawFishSteak)),
                new FillableEntry(1, typeof(FishSteak)),
                new FillableEntry(2, typeof(CookedBird)),
                new FillableEntry(2, typeof(RawBird)),
                new FillableEntry(2, typeof(Ham)),
                new FillableEntry(1, typeof(RawLambLeg)),
                new FillableEntry(1, typeof(LambLeg)),
                new FillableEntry(1, typeof(Ribs)),
                new FillableEntry(1, typeof(RawRibs)),
                new FillableEntry(2, typeof(Sausage)),
                new FillableEntry(1, typeof(RawChickenLeg)),
                new FillableEntry(1, typeof(ChickenLeg)),
                new FillableEntry(1, typeof(Watermelon)),
                new FillableEntry(1, typeof(SmallWatermelon)),
                new FillableEntry(3, typeof(Turnip)),
                new FillableEntry(2, typeof(YellowGourd)),
                new FillableEntry(2, typeof(GreenGourd)),
                new FillableEntry(2, typeof(Pumpkin)),
                new FillableEntry(1, typeof(SmallPumpkin)),
                new FillableEntry(2, typeof(Onion)),
                new FillableEntry(2, typeof(Lettuce)),
                new FillableEntry(2, typeof(Squash)),
                new FillableEntry(2, typeof(HoneydewMelon)),
                new FillableEntry(1, typeof(Carrot)),
                new FillableEntry(2, typeof(Cantaloupe)),
                new FillableEntry(2, typeof(Cabbage)),
                new FillableEntry(4, typeof(EarOfCorn))
            });
        public static FillableContent Ranger = new FillableContent(
            2,
            new Type[]
            {
                typeof(Ranger),
                typeof(RangerGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(2, typeof(StuddedChest)),
                new FillableEntry(2, typeof(StuddedLegs)),
                new FillableEntry(2, typeof(StuddedArms)),
                new FillableEntry(2, typeof(StuddedGloves)),
                new FillableEntry(1, typeof(StuddedGorget)),
                new FillableEntry(2, typeof(LeatherChest)),
                new FillableEntry(2, typeof(LeatherLegs)),
                new FillableEntry(2, typeof(LeatherArms)),
                new FillableEntry(2, typeof(LeatherGloves)),
                new FillableEntry(1, typeof(LeatherGorget)),
                new FillableEntry(2, typeof(FeatheredHat)),
                new FillableEntry(1, typeof(CloseHelm)),
                new FillableEntry(1, typeof(TallStrawHat)),
                new FillableEntry(1, typeof(Bandana)),
                new FillableEntry(1, typeof(Cloak)),
                new FillableEntry(2, typeof(Boots)),
                new FillableEntry(2, typeof(ThighBoots)),
                new FillableEntry(2, typeof(GnarledStaff)),
                new FillableEntry(1, typeof(Whip)),
                new FillableEntry(2, typeof(Bow)),
                new FillableEntry(2, typeof(Crossbow)),
                new FillableEntry(2, typeof(HeavyCrossbow)),
                new FillableEntry(4, typeof(Arrow))
            });
        public static FillableContent Stables = new FillableContent(
            1,
            new Type[]
            {
                typeof(AnimalTrainer),
                typeof(GypsyAnimalTrainer)
            },
            new FillableEntry[]
            {
                //new FillableEntry( 1, typeof( Wheat ) ),
                new FillableEntry(1, typeof(Carrot))
            });
        public static FillableContent Tanner = new FillableContent(
            2,
            new Type[]
            {
                typeof(Tanner),
                typeof(LeatherWorker),
                typeof(Furtrader)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(FeatheredHat)),
                new FillableEntry(1, typeof(LeatherArms)),
                new FillableEntry(2, typeof(LeatherLegs)),
                new FillableEntry(2, typeof(LeatherChest)),
                new FillableEntry(2, typeof(LeatherGloves)),
                new FillableEntry(1, typeof(LeatherGorget)),
                new FillableEntry(2, typeof(Leather))
            });
        public static FillableContent Tavern = new FillableContent(
            1,
            new Type[]
            {
                typeof(TavernKeeper),
                typeof(Barkeeper),
                typeof(Waiter),
                typeof(Cook)
            },
            new FillableEntry[]
            {
                new FillableBvrge(1, typeof(BeverageBottle), BeverageType.Ale),
                new FillableBvrge(1, typeof(BeverageBottle), BeverageType.Wine),
                new FillableBvrge(1, typeof(BeverageBottle), BeverageType.Liquor),
                new FillableBvrge(1, typeof(Jug), BeverageType.Cider)
            });
        public static FillableContent ThiefGuild = new FillableContent(
            1,
            new Type[]
            {
                typeof(Thief),
                typeof(ThiefGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(Lockpick)),
                new FillableEntry(1, typeof(BearMask)),
                new FillableEntry(1, typeof(DeerMask)),
                new FillableEntry(1, typeof(TribalMask)),
                new FillableEntry(1, typeof(HornedTribalMask)),
                new FillableEntry(4, typeof(OrcHelm))
            });
        public static FillableContent Tinker = new FillableContent(
            1,
            new Type[]
            {
                typeof(Tinker),
                typeof(TinkerGuildmaster)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(Lockpick)),
                new FillableEntry(2, typeof(Clock)),
                new FillableEntry(2, typeof(ClockParts)),
                new FillableEntry(2, typeof(AxleGears)),
                new FillableEntry(2, typeof(Gears)),
                new FillableEntry(2, typeof(Hinge)),
                new FillableEntry(2, typeof(Sextant)),
                new FillableEntry(2, typeof(SextantParts)),
                new FillableEntry(2, typeof(Axle)),
                new FillableEntry(2, typeof(Springs)),
                new FillableEntry(5, typeof(TinkerTools)),
                new FillableEntry(4, typeof(Key)),
                new FillableEntry(1, typeof(Lockpicks))
            });
        public static FillableContent Veterinarian = new FillableContent(
            1,
            new Type[]
            {
                typeof(Veterinarian)
            },
            new FillableEntry[]
            {
                new FillableEntry(1, typeof(Bandage)),
                new FillableEntry(1, typeof(MortarPestle)),
                new FillableEntry(1, typeof(LesserHealPotion)),
                new FillableEntry(1, typeof(Carrot))
            });
        public static FillableContent Weaponsmith = new FillableContent(
            2,
            new Type[]
            {
                typeof(Weaponsmith)
            },
            new FillableEntry[]
            {
                new FillableEntry(8, Loot.WeaponTypes),
                new FillableEntry(1, typeof(Arrow))
            });
        private static readonly FillableContent[] m_ContentTypes = new FillableContent[]
        {
            Weaponsmith, Provisioner, Mage,
            Alchemist, Armorer, ArtisanGuild,
            Baker, Bard, Blacksmith,
            Bowyer, Butcher, Carpenter,
            Clothier, Cobbler, Docks,
            Farm, FighterGuild, Guard,
            Healer, Herbalist, Inn,
            Jeweler, Library, Merchant,
            Mill, Mine, Observatory,
            Painter, Ranger, Stables,
            Tanner, Tavern, ThiefGuild,
            Tinker, Veterinarian
        };
        private static Hashtable m_AcquireTable;
        private readonly int m_Level;
        private readonly Type[] m_Vendors;
        private readonly FillableEntry[] m_Entries;
        private readonly int m_Weight;
        public FillableContent(int level, Type[] vendors, FillableEntry[] entries)
        {
            m_Level = level;
            m_Vendors = vendors;
            m_Entries = entries;

            for (int i = 0; i < entries.Length; ++i)
                m_Weight += entries[i].Weight;
        }

        public int Level => m_Level;
        public Type[] Vendors => m_Vendors;
        public FillableContentType TypeID => Lookup(this);
        public static FillableContent Lookup(FillableContentType type)
        {
            int v = (int)type;

            if (v >= 0 && v < m_ContentTypes.Length)
                return m_ContentTypes[v];

            return null;
        }

        public static FillableContentType Lookup(FillableContent content)
        {
            if (content == null)
                return FillableContentType.None;

            return (FillableContentType)Array.IndexOf(m_ContentTypes, content);
        }

        public static FillableContent Acquire(Point3D loc, Map map)
        {
            if (map == null || map == Map.Internal)
                return null;

            if (m_AcquireTable == null)
            {
                m_AcquireTable = new Hashtable();

                for (int i = 0; i < m_ContentTypes.Length; ++i)
                {
                    FillableContent fill = m_ContentTypes[i];

                    for (int j = 0; j < fill.m_Vendors.Length; ++j)
                        m_AcquireTable[fill.m_Vendors[j]] = fill;
                }
            }

            Mobile nearest = null;
            FillableContent content = null;

            IPooledEnumerable eable = map.GetMobilesInRange(loc, 60);

            foreach (Mobile mob in eable)
            {
                if (nearest != null && mob.GetDistanceToSqrt(loc) > nearest.GetDistanceToSqrt(loc) && !(nearest is Cobbler && mob is Provisioner))
                    continue;

                FillableContent check = m_AcquireTable[mob.GetType()] as FillableContent;

                if (check != null)
                {
                    nearest = mob;
                    content = check;
                }
            }

            eable.Free();

            return content;
        }

        public virtual Item Construct()
        {
            int index = Utility.Random(m_Weight);

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                FillableEntry entry = m_Entries[i];

                if (index < entry.Weight)
                    return entry.Construct();

                index -= entry.Weight;
            }

            return null;
        }
    }
}
