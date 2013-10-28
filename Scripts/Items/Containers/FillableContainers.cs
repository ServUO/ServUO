using System;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;

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
        protected FillableContent m_Content;
        protected DateTime m_NextRespawnTime;
        protected Timer m_RespawnTimer;
        public FillableContainer(int itemID)
            : base(itemID)
        {
            this.Movable = false;
        }

        public FillableContainer(Serial serial)
            : base(serial)
        {
        }

        public virtual int MinRespawnMinutes
        {
            get
            {
                return 60;
            }
        }
        public virtual int MaxRespawnMinutes
        {
            get
            {
                return 90;
            }
        }
        public virtual bool IsLockable
        {
            get
            {
                return true;
            }
        }
        public virtual bool IsTrapable
        {
            get
            {
                return this.IsLockable;
            }
        }
        public virtual int SpawnThreshold
        {
            get
            {
                return 2;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextRespawnTime
        {
            get
            {
                return this.m_NextRespawnTime;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public FillableContentType ContentType
        {
            get
            {
                return FillableContent.Lookup(this.m_Content);
            }
            set
            {
                this.Content = FillableContent.Lookup(value);
            }
        }
        public FillableContent Content
        {
            get
            {
                return this.m_Content;
            }
            set
            {
                if (this.m_Content == value)
                    return;

                this.m_Content = value;

                for (int i = this.Items.Count - 1; i >= 0; --i)
                {
                    if (i < this.Items.Count)
                        this.Items[i].Delete();
                }

                this.Respawn();
            }
        }
        public override void OnMapChange()
        {
            base.OnMapChange();
            this.AcquireContent();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);
            this.AcquireContent();
        }

        public virtual void AcquireContent()
        {
            if (this.m_Content != null)
                return;

            this.m_Content = FillableContent.Acquire(this.GetWorldLocation(), this.Map);

            if (this.m_Content != null)
                this.Respawn();
        }

        public override void OnItemRemoved(Item item)
        {
            this.CheckRespawn();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_RespawnTimer != null)
            {
                this.m_RespawnTimer.Stop();
                this.m_RespawnTimer = null;
            }
        }

        public int GetItemsCount()
        {
            int count = 0;

            foreach (Item item in this.Items)
            {
                count += item.Amount;
            }

            return count;
        }

        public void CheckRespawn()
        {
            bool canSpawn = (this.m_Content != null && !this.Deleted && this.GetItemsCount() <= this.SpawnThreshold && !this.Movable && this.Parent == null && !this.IsLockedDown && !this.IsSecure);

            if (canSpawn)
            {
                if (this.m_RespawnTimer == null)
                {
                    int mins = Utility.RandomMinMax(this.MinRespawnMinutes, this.MaxRespawnMinutes);
                    TimeSpan delay = TimeSpan.FromMinutes(mins);

                    this.m_NextRespawnTime = DateTime.UtcNow + delay;
                    this.m_RespawnTimer = Timer.DelayCall(delay, new TimerCallback(Respawn));
                }
            }
            else if (this.m_RespawnTimer != null)
            {
                this.m_RespawnTimer.Stop();
                this.m_RespawnTimer = null;
            }
        }

        public void Respawn()
        {
            if (this.m_RespawnTimer != null)
            {
                this.m_RespawnTimer.Stop();
                this.m_RespawnTimer = null;
            }

            if (this.m_Content == null || this.Deleted)
                return;

            this.GenerateContent();

            if (this.IsLockable)
            {
                this.Locked = true;

                int difficulty = (this.m_Content.Level - 1) * 30;

                this.LockLevel = difficulty - 10;
                this.MaxLockLevel = difficulty + 30;
                this.RequiredSkill = difficulty;
            }

            if (this.IsTrapable && (this.m_Content.Level > 1 || 4 > Utility.Random(5)))
            {
                if (this.m_Content.Level > Utility.Random(5))
                    this.TrapType = TrapType.PoisonTrap;
                else
                    this.TrapType = TrapType.ExplosionTrap;

                this.TrapPower = this.m_Content.Level * Utility.RandomMinMax(10, 30);
                this.TrapLevel = this.m_Content.Level;
            }
            else
            {
                this.TrapType = TrapType.None;
                this.TrapPower = 0;
                this.TrapLevel = 0;
            }

            this.CheckRespawn();
        }

        public virtual void GenerateContent()
        {
            if (this.m_Content == null || this.Deleted)
                return;

            int toSpawn = this.GetSpawnCount();

            for (int i = 0; i < toSpawn; ++i)
            {
                Item item = this.m_Content.Construct();

                if (item != null)
                {
                    List<Item> list = this.Items;

                    for (int j = 0; j < list.Count; ++j)
                    {
                        Item subItem = list[j];

                        if (!(subItem is Container) && subItem.StackWith(null, item, false))
                            break;
                    }

                    if (item != null && !item.Deleted)
                        this.DropItem(item);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            writer.Write((int)this.ContentType);

            if (this.m_RespawnTimer != null)
            {
                writer.Write(true);
                writer.WriteDeltaTime((DateTime)this.m_NextRespawnTime);
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

            switch( version )
            {
                case 1:
                    {
                        this.m_Content = FillableContent.Lookup((FillableContentType)reader.ReadInt());
                        goto case 0;
                    }
                case 0:
                    {
                        if (reader.ReadBool())
                        {
                            this.m_NextRespawnTime = reader.ReadDeltaTime();

                            TimeSpan delay = this.m_NextRespawnTime - DateTime.UtcNow;
                            this.m_RespawnTimer = Timer.DelayCall(delay > TimeSpan.Zero ? delay : TimeSpan.Zero, new TimerCallback(Respawn));
                        }
                        else
                        {
                            this.CheckRespawn();
                        }

                        break;
                    }
            }
        }

        protected virtual int GetSpawnCount()
        {
            int itemsCount = this.GetItemsCount();

            if (itemsCount > this.SpawnThreshold)
                return 0;

            int maxSpawnCount = (1 + this.SpawnThreshold - itemsCount) * 2;

            return Utility.RandomMinMax(0, maxSpawnCount);
        }
    }

    [Flipable(0xA97, 0xA99, 0xA98, 0xA9A, 0xA9B, 0xA9C)]
    public class LibraryBookcase : FillableContainer
    {
        [Constructable]
        public LibraryBookcase()
            : base(0xA97)
        {
            this.Weight = 1.0;
        }

        public LibraryBookcase(Serial serial)
            : base(serial)
        {
        }

        public override bool IsLockable
        {
            get
            {
                return false;
            }
        }
        public override int SpawnThreshold
        {
            get
            {
                return 5;
            }
        }
        public override void AcquireContent()
        {
            if (this.m_Content != null)
                return;

            this.m_Content = FillableContent.Library;

            if (this.m_Content != null)
                this.Respawn();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0 && this.m_Content == null)
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AcquireContent));
        }

        protected override int GetSpawnCount()
        {
            return (5 - this.GetItemsCount());
        }
    }

    [Flipable(0xE3D, 0xE3C)]
    public class FillableLargeCrate : FillableContainer
    {
        [Constructable]
        public FillableLargeCrate()
            : base(0xE3D)
        {
            this.Weight = 1.0;
        }

        public FillableLargeCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
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
            this.Weight = 1.0;
        }

        public FillableSmallCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
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
            this.Weight = 4.0;
        }

        public FillableWoodenBox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
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

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0 && this.Weight == 3)
                this.Weight = -1;
        }
    }

    public class FillableBarrel : FillableContainer
    {
        [Constructable]
        public FillableBarrel()
            : base(0xE77)
        {
        }

        public FillableBarrel(Serial serial)
            : base(serial)
        {
        }

        public override bool IsLockable
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0 && this.Weight == 25)
                this.Weight = -1;
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

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 25)
                this.Weight = -1;
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

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 25)
                this.Weight = -1;
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

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 2)
                this.Weight = -1;
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
            this.m_Weight = weight;
            this.m_Types = types;
        }

        public FillableEntry(int weight, Type[] types, int offset, int count)
        {
            this.m_Weight = weight;
            this.m_Types = new Type[count];

            for (int i = 0; i < this.m_Types.Length; ++i)
                this.m_Types[i] = types[offset + i];
        }

        public Type[] Types
        {
            get
            {
                return this.m_Types;
            }
        }
        public int Weight
        {
            get
            {
                return this.m_Weight;
            }
        }
        public virtual Item Construct()
        {
            Item item = Loot.Construct(this.m_Types);

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
            this.m_Content = content;
        }

        public BeverageType Content
        {
            get
            {
                return this.m_Content;
            }
        }
        public override Item Construct()
        {
            Item item;

            int index = Utility.Random(this.m_Types.Length);

            if (this.m_Types[index] == typeof(BeverageBottle))
            {
                item = new BeverageBottle(this.m_Content);
            }
            else if (this.m_Types[index] == typeof(Jug))
            {
                item = new Jug(this.m_Content);
            }
            else
            {
                item = base.Construct();

                if (item is BaseBeverage)
                {
                    BaseBeverage bev = (BaseBeverage)item;

                    bev.Content = this.m_Content;
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
                typeof(Mobiles.Alchemist)
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
                //new FillableEntry( 8, typeof( IronOre ) ), TODO: Smaller ore
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
                // Four different types
                //new FillableEntry( 1, typeof( FoldedCloth ) ),
                //new FillableEntry( 1, typeof( FoldedCloth ) ),
                //new FillableEntry( 1, typeof( FoldedCloth ) ),
                //new FillableEntry( 1, typeof( FoldedCloth ) ),
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
                //new FillableEntry( 1, typeof( SmallFish ) ),
                //new FillableEntry( 1, typeof( SmallFish ) ),
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
                new FillableEntry(1, typeof(DeadWood)),
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
                new FillableEntry(12, Loot.RegularScrollTypes, 0, 8),
                new FillableEntry(11, Loot.RegularScrollTypes, 8, 8),
                new FillableEntry(10, Loot.RegularScrollTypes, 16, 8),
                new FillableEntry(9, Loot.RegularScrollTypes, 24, 8),
                new FillableEntry(8, Loot.RegularScrollTypes, 32, 8),
                new FillableEntry(7, Loot.RegularScrollTypes, 40, 8),
                new FillableEntry(6, Loot.RegularScrollTypes, 48, 8),
                new FillableEntry(5, Loot.RegularScrollTypes, 56, 8)
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
                //new FillableEntry( 2, typeof( IronOre ) ),	TODO: Smaller Ore
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
                new FillableEntry(1, typeof(DirtyFrypan)),
                new FillableEntry(1, typeof(DirtyPan)),
                new FillableEntry(1, typeof(DirtyKettle)),
                new FillableEntry(1, typeof(DirtySmallRoundPot)),
                new FillableEntry(1, typeof(DirtyRoundPot)),
                new FillableEntry(1, typeof(DirtySmallPot)),
                new FillableEntry(1, typeof(DirtyPot)),
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
                //new FillableEntry( 1, typeof( KeyRing ) ),
                new FillableEntry(2, typeof(Clock)),
                new FillableEntry(2, typeof(ClockParts)),
                new FillableEntry(2, typeof(AxleGears)),
                new FillableEntry(2, typeof(Gears)),
                new FillableEntry(2, typeof(Hinge)),
                //new FillableEntry( 1, typeof( ArrowShafts ) ),
                new FillableEntry(2, typeof(Sextant)),
                new FillableEntry(2, typeof(SextantParts)),
                new FillableEntry(2, typeof(Axle)),
                new FillableEntry(2, typeof(Springs)),
                new FillableEntry(5, typeof(TinkerTools)),
                new FillableEntry(4, typeof(Key)),
                new FillableEntry(1, typeof(DecoArrowShafts)),
                new FillableEntry(1, typeof(Lockpicks)),
                new FillableEntry(1, typeof(ToolKit))
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
                //new FillableEntry( 1, typeof( Wheat ) ),
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
            this.m_Level = level;
            this.m_Vendors = vendors;
            this.m_Entries = entries;

            for (int i = 0; i < entries.Length; ++i)
                this.m_Weight += entries[i].Weight;
        }

        public int Level
        {
            get
            {
                return this.m_Level;
            }
        }
        public Type[] Vendors
        {
            get
            {
                return this.m_Vendors;
            }
        }
        public FillableContentType TypeID
        {
            get
            {
                return Lookup(this);
            }
        }
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

            foreach (Mobile mob in map.GetMobilesInRange(loc, 20))
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

            return content;
        }

        public virtual Item Construct()
        {
            int index = Utility.Random(this.m_Weight);

            for (int i = 0; i < this.m_Entries.Length; ++i)
            {
                FillableEntry entry = this.m_Entries[i];

                if (index < entry.Weight)
                    return entry.Construct();

                index -= entry.Weight;
            }

            return null;
        }
    }
}