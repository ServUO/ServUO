using System;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    [FlipableAttribute(0x1EBA, 0x1EBB)]
    public class TaxidermyKit : Item
    {
        private static readonly TrophyInfo[] m_Table = new TrophyInfo[]
        {
            new TrophyInfo(typeof(BrownBear),	0x1E60, 1041093, 1041107),
            new TrophyInfo(typeof(GreatHart),	0x1E61, 1041095, 1041109),
            new TrophyInfo(typeof(BigFish), 0x1E62, 1041096, 1041110),
            new TrophyInfo(typeof(Gorilla), 0x1E63, 1041091, 1041105),
            new TrophyInfo(typeof(Orc), 0x1E64, 1041090, 1041104),
            new TrophyInfo(typeof(PolarBear),	0x1E65, 1041094, 1041108),
            new TrophyInfo(typeof(Troll), 0x1E66, 1041092, 1041106)
        };
        [Constructable]
        public TaxidermyKit()
            : base(0x1EBA)
        {
            this.Weight = 1.0;
        }

        public TaxidermyKit(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041279;
            }
        }// a taxidermy kit
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

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (from.Skills[SkillName.Carpentry].Base < 90.0)
            {
                from.SendLocalizedMessage(1042594); // You do not understand how to use this.
            }
            else
            {
                from.SendLocalizedMessage(1042595); // Target the corpse to make a trophy out of.
                from.Target = new CorpseTarget(this);
            }
        }

        public class TrophyInfo
        {
            private readonly Type m_CreatureType;
            private readonly int m_NorthID;
            private readonly int m_DeedNumber;
            private readonly int m_AddonNumber;
            public TrophyInfo(Type type, int id, int deedNum, int addonNum)
            {
                this.m_CreatureType = type;
                this.m_NorthID = id;
                this.m_DeedNumber = deedNum;
                this.m_AddonNumber = addonNum;
            }

            public Type CreatureType
            {
                get
                {
                    return this.m_CreatureType;
                }
            }
            public int NorthID
            {
                get
                {
                    return this.m_NorthID;
                }
            }
            public int DeedNumber
            {
                get
                {
                    return this.m_DeedNumber;
                }
            }
            public int AddonNumber
            {
                get
                {
                    return this.m_AddonNumber;
                }
            }
        }

        private class CorpseTarget : Target
        {
            private readonly TaxidermyKit m_Kit;
            public CorpseTarget(TaxidermyKit kit)
                : base(3, false, TargetFlags.None)
            {
                this.m_Kit = kit;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Kit.Deleted)
                    return;

                if (!(targeted is Corpse) && !(targeted is BigFish))
                {
                    from.SendLocalizedMessage(1042600); // That is not a corpse!
                }
                else if (targeted is Corpse && ((Corpse)targeted).VisitedByTaxidermist)
                {
                    from.SendLocalizedMessage(1042596); // That corpse seems to have been visited by a taxidermist already.
                }
                else if (!this.m_Kit.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
                else if (from.Skills[SkillName.Carpentry].Base < 90.0)
                {
                    from.SendLocalizedMessage(1042603); // You would not understand how to use the kit.
                }
                else
                {
                    object obj = targeted;

                    if (obj is Corpse)
                        obj = ((Corpse)obj).Owner;

                    if (obj != null)
                    {
                        for (int i = 0; i < m_Table.Length; i++)
                        {
                            if (m_Table[i].CreatureType == obj.GetType())
                            {
                                Container pack = from.Backpack;

                                if (pack != null && pack.ConsumeTotal(typeof(Board), 10))
                                {
                                    from.SendLocalizedMessage(1042278); // You review the corpse and find it worthy of a trophy.
                                    from.SendLocalizedMessage(1042602); // You use your kit up making the trophy.

                                    Mobile hunter = null;
                                    int weight = 0;

                                    if (targeted is BigFish)
                                    {
                                        BigFish fish = targeted as BigFish;

                                        hunter = fish.Fisher;
                                        weight = (int)fish.Weight;

                                        fish.Consume();
                                    }

                                    from.AddToBackpack(new TrophyDeed(m_Table[i], hunter, weight));

                                    if (targeted is Corpse)
                                        ((Corpse)targeted).VisitedByTaxidermist = true;

                                    this.m_Kit.Delete();
                                    return;
                                }
                                else
                                {
                                    from.SendLocalizedMessage(1042598); // You do not have enough boards.
                                    return;
                                }
                            }
                        }
                    }

                    from.SendLocalizedMessage(1042599); // That does not look like something you want hanging on a wall.
                }
            }
        }
    }

    public class TrophyAddon : Item, IAddon
    {
        private int m_WestID;
        private int m_NorthID;
        private int m_DeedNumber;
        private int m_AddonNumber;
        private Mobile m_Hunter;
        private int m_AnimalWeight;
        [Constructable]
        public TrophyAddon(Mobile from, int itemID, int westID, int northID, int deedNumber, int addonNumber)
            : this(from, itemID, westID, northID, deedNumber, addonNumber, null, 0)
        {
        }

        public TrophyAddon(Mobile from, int itemID, int westID, int northID, int deedNumber, int addonNumber, Mobile hunter, int animalWeight)
            : base(itemID)
        {
            this.m_WestID = westID;
            this.m_NorthID = northID;
            this.m_DeedNumber = deedNumber;
            this.m_AddonNumber = addonNumber;

            this.m_Hunter = hunter;
            this.m_AnimalWeight = animalWeight;

            this.Movable = false;

            this.MoveToWorld(from.Location, from.Map);
        }

        public TrophyAddon(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int WestID
        {
            get
            {
                return this.m_WestID;
            }
            set
            {
                this.m_WestID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int NorthID
        {
            get
            {
                return this.m_NorthID;
            }
            set
            {
                this.m_NorthID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int DeedNumber
        {
            get
            {
                return this.m_DeedNumber;
            }
            set
            {
                this.m_DeedNumber = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int AddonNumber
        {
            get
            {
                return this.m_AddonNumber;
            }
            set
            {
                this.m_AddonNumber = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Hunter
        {
            get
            {
                return this.m_Hunter;
            }
            set
            {
                this.m_Hunter = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int AnimalWeight
        {
            get
            {
                return this.m_AnimalWeight;
            }
            set
            {
                this.m_AnimalWeight = value;
                this.InvalidateProperties();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return this.m_AddonNumber;
            }
        }
        public Item Deed
        {
            get
            {
                return new TrophyDeed(this.m_WestID, this.m_NorthID, this.m_DeedNumber, this.m_AddonNumber, this.m_Hunter, this.m_AnimalWeight);
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_AnimalWeight >= 20)
            {
                if (this.m_Hunter != null)
                    list.Add(1070857, this.m_Hunter.Name); // Caught by ~1_fisherman~

                list.Add(1070858, this.m_AnimalWeight.ToString()); // ~1_weight~ stones
            }
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            if (!map.CanFit(p.X, p.Y, p.Z, this.ItemData.Height))
                return false;

            if (this.ItemID == this.m_NorthID)
                return BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map); // North wall
            else
                return BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map); // West wall
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((Mobile)this.m_Hunter);
            writer.Write((int)this.m_AnimalWeight);

            writer.Write((int)this.m_WestID);
            writer.Write((int)this.m_NorthID);
            writer.Write((int)this.m_DeedNumber);
            writer.Write((int)this.m_AddonNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Hunter = reader.ReadMobile();
                        this.m_AnimalWeight = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_WestID = reader.ReadInt();
                        this.m_NorthID = reader.ReadInt();
                        this.m_DeedNumber = reader.ReadInt();
                        this.m_AddonNumber = reader.ReadInt();
                        break;
                    }
            }

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(FixMovingCrate));
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsCoOwner(from))
            {
                if (from.InRange(this.GetWorldLocation(), 1))
                {
                    from.AddToBackpack(this.Deed);
                    this.Delete();
                }
                else
                {
                    from.SendLocalizedMessage(500295); // You are too far away to do that.
                }
            }
        }

        private void FixMovingCrate()
        {
            if (this.Deleted)
                return;

            if (this.Movable || this.IsLockedDown)
            {
                Item deed = this.Deed;

                if (this.Parent is Item)
                {
                    ((Item)this.Parent).AddItem(deed);
                    deed.Location = this.Location;
                }
                else
                {
                    deed.MoveToWorld(this.Location, this.Map);
                }

                this.Delete();
            }
        }
    }

    [Flipable(0x14F0, 0x14EF)]
    public class TrophyDeed : Item
    {
        private int m_WestID;
        private int m_NorthID;
        private int m_DeedNumber;
        private int m_AddonNumber;
        private Mobile m_Hunter;
        private int m_AnimalWeight;
        [Constructable]
        public TrophyDeed(int westID, int northID, int deedNumber, int addonNumber)
            : this(westID, northID, deedNumber, addonNumber, null, 0)
        {
        }

        public TrophyDeed(int westID, int northID, int deedNumber, int addonNumber, Mobile hunter, int animalWeight)
            : base(0x14F0)
        {
            this.m_WestID = westID;
            this.m_NorthID = northID;
            this.m_DeedNumber = deedNumber;
            this.m_AddonNumber = addonNumber;
            this.m_Hunter = hunter;
            this.m_AnimalWeight = animalWeight;
        }

        public TrophyDeed(TaxidermyKit.TrophyInfo info, Mobile hunter, int animalWeight)
            : this(info.NorthID + 7, info.NorthID, info.DeedNumber, info.AddonNumber, hunter, animalWeight)
        {
        }

        public TrophyDeed(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WestID
        {
            get
            {
                return this.m_WestID;
            }
            set
            {
                this.m_WestID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int NorthID
        {
            get
            {
                return this.m_NorthID;
            }
            set
            {
                this.m_NorthID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int DeedNumber
        {
            get
            {
                return this.m_DeedNumber;
            }
            set
            {
                this.m_DeedNumber = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int AddonNumber
        {
            get
            {
                return this.m_AddonNumber;
            }
            set
            {
                this.m_AddonNumber = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Hunter
        {
            get
            {
                return this.m_Hunter;
            }
            set
            {
                this.m_Hunter = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int AnimalWeight
        {
            get
            {
                return this.m_AnimalWeight;
            }
            set
            {
                this.m_AnimalWeight = value;
                this.InvalidateProperties();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return this.m_DeedNumber;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_AnimalWeight >= 20)
            {
                if (this.m_Hunter != null)
                    list.Add(1070857, this.m_Hunter.Name); // Caught by ~1_fisherman~

                list.Add(1070858, this.m_AnimalWeight.ToString()); // ~1_weight~ stones
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((Mobile)this.m_Hunter);
            writer.Write((int)this.m_AnimalWeight);

            writer.Write((int)this.m_WestID);
            writer.Write((int)this.m_NorthID);
            writer.Write((int)this.m_DeedNumber);
            writer.Write((int)this.m_AddonNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Hunter = reader.ReadMobile();
                        this.m_AnimalWeight = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_WestID = reader.ReadInt();
                        this.m_NorthID = reader.ReadInt();
                        this.m_DeedNumber = reader.ReadInt();
                        this.m_AddonNumber = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsCoOwner(from))
                {
                    bool northWall = BaseAddon.IsWall(from.X, from.Y - 1, from.Z, from.Map);
                    bool westWall = BaseAddon.IsWall(from.X - 1, from.Y, from.Z, from.Map);

                    if (northWall && westWall)
                    {
                        switch ( from.Direction & Direction.Mask )
                        {
                            case Direction.North:
                            case Direction.South:
                                northWall = true;
                                westWall = false;
                                break;
                            case Direction.East:
                            case Direction.West:
                                northWall = false;
                                westWall = true;
                                break;
                            default:
                                from.SendMessage("Turn to face the wall on which to hang this trophy.");
                                return;
                        }
                    }

                    int itemID = 0;

                    if (northWall)
                        itemID = this.m_NorthID;
                    else if (westWall)
                        itemID = this.m_WestID;
                    else
                        from.SendLocalizedMessage(1042626); // The trophy must be placed next to a wall.

                    if (itemID > 0)
                    {
                        house.Addons.Add(new TrophyAddon(from, itemID, this.m_WestID, this.m_NorthID, this.m_DeedNumber, this.m_AddonNumber, this.m_Hunter, this.m_AnimalWeight));
                        this.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }
    }
}