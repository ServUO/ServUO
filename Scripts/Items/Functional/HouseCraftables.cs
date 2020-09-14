using Server.Engines.Craft;
using Server.Multis;
using System;

namespace Server.Items
{
    public enum CraftableItemType
    {
        // Walls
        RoughWindowless,
        RoughWindow,
        RoughArch,
        RoughPillar,
        RoughRoundedArch,
        RoughSmallArch,
        RoughAngledPillar,
        ShortRough,
        // Stairs
        RoughBlock,
        RoughSteps,
        RoughCornerSteps,
        RoughRoundedCornerSteps,
        RoughInsetSteps,
        RoughRoundedInsetSteps,
        // Floors
        LightPaver,
        MediumPaver,
        DarkPaver,
        //Hangers
        LightWoodenSignHanger,
        DarkWoodenSignHanger,
        CurledMetalSignHanger,
        FlourishedMetalSignHanger,
        InwardCurledMetalSignHanger,
        EndCurledMetalSignHanger
    }

    public class CraftableHouseItem : Item, IFlipable, ICraftable
    {
        public static int[][] IDs =
        {
            new int[] { 1155794, 464, 465, 466, 463 },      // RoughWindowless
			new int[] { 1155797, 467, 468 },                // RoughWindow
			new int[] { 1155799, 469, 470, 471, 472, 473 }, // RoughArch
			new int[] { 1155804, 474, },                    // RoughPillar
			new int[] { 1155805, 475, 476, 477, 478, 479 }, // RoughRoundedArch
			new int[] { 1155810, 480, 481, 482, 484 },      // RoughSmallArch
			new int[] { 1155814, 486, 487 },                // RoughAngledPillar
			new int[] { 1155816, 488, 489, 490, 491 },      // ShortRough
			new int[] { 1155821, 1928 },                    // RoughBlock
			new int[] { 1155822, 1929, 1930, 1931, 1932 },  // RoughSteps
			new int[] { 1155826, 1934, 1933, 1935, 1936 },  // RoughCornerSteps
			new int[] { 1155830, 1938, 1940, 1937, 1939 },  // RoughRoundedCornerSeps
			new int[] { 1155834, 1941, 1942, 1943, 1944 },  // RoughInsetSteps
			new int[] { 1155838, 1945, 1946, 1947, 1948 },  // RoughRoundedIsetSteps
			new int[] { 1155878, 1305 },                    // LightPaver
			new int[] { 1155879, 1309 },                    // Medium Paver
			new int[] { 1155880, 1313 },                    // DarkPaver
            new int[] { 1155850, 2969, 2970 },              // LightWoodenSIgnHanger
            new int[] { 1155849, 2967, 2968 },              // DarkWoodenSIgnHanger
			new int[] { 1155851, 2971, 2972 },              // CurledMetalSIgnHanger
			new int[] { 1155852, 2973, 2974 },              // FlourishedCurledSignHanger
			new int[] { 1155853, 2975, 2976 },              // InwardCurledCurledMetalSIgnHanger
			new int[] { 1155854, 2977, 2978 },              // EndCurledMetalSignHanger
		};

        private CraftableItemType _Type;
        private CraftResource _Resource;

        public int NorthID => 0;
        public int WestID => 0;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanFlip
        {
            get
            {
                int value = (int)_Type;

                return (value >= 0 && value <= 13) || value >= 17;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return _Resource; }
            set
            {
                if (_Resource != value)
                {
                    _Resource = value;
                    Hue = CraftResources.GetHue(_Resource);

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftableItemType ItemType
        {
            get { return _Type; }
            set
            {
                _Type = value;
                InvalidateID();
            }
        }

        public override bool IsArtifact => true;

        public CraftableHouseItem()
            : base(1)
        {
        }

        [Constructable]
        public CraftableHouseItem(CraftableItemType type)
            : base(IDs[(int)type][1])
        {
            _Type = type;
        }

        public void InvalidateID()
        {
            ItemID = IDs[(int)_Type][1];
        }

        public void OnFlip(Mobile from)
        {
            int[] list = IDs[(int)_Type];

            if (CanFlip && list.Length > 2)
            {
                for (int i = 1; i < list.Length; i++)
                {
                    int id = list[i];

                    if (ItemID == id)
                    {
                        if (i >= list.Length - 1)
                        {
                            ItemID = list[1];
                            break;
                        }
                        else
                        {
                            ItemID = list[i + 1];
                            break;
                        }
                    }
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1153494); // House Only
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            if (craftItem != null && craftItem.Data is CraftableItemType)
            {
                ItemType = (CraftableItemType)craftItem.Data;

                Type resourceType = typeRes;

                if (resourceType == null)
                    resourceType = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(resourceType);
            }

            return quality;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (!CraftResources.IsStandard(_Resource))
            {
                list.Add(1050039, string.Format("#{0}\t{1}", CraftResources.GetLocalizationNumber(_Resource).ToString(), GetNameString())); // ~1_NUMBER~ ~2_ITEMNAME~
            }
            else
            {
                base.AddNameProperty(list);
            }
        }

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
            {
                name = string.Format("#{0}", LabelNumber);
            }

            return name;
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            BaseHouse h = BaseHouse.FindHouseAt(p, from.Map, ItemData.Height);

            if (h != null && h.IsCoOwner(from))
            {
                return base.DropToWorld(from, p);
            }

            if (from.Backpack == null || !from.Backpack.TryDropItem(from, this, false))
            {
                Delete();
            }

            return false;
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (target.Backpack is StrongBackpack)
            {
                return false;
            }

            return base.DropToMobile(from, target, p);
        }

        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (target.Movable || target is StrongBackpack)
                return false;

            return base.OnDroppedInto(from, target, p);
        }

        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            if (target is Container && target.Movable || target is StrongBackpack)
                return false;

            return base.OnDroppedOnto(from, target);
        }

        public CraftableHouseItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write((int)_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            _Type = (CraftableItemType)reader.ReadInt();
        }

        public static void Replace(Item oldItem, CraftableItemType type)
        {
            BaseAddon addon = oldItem is AddonComponent ? ((AddonComponent)oldItem).Addon : null;

            CraftableHouseItem item = new CraftableHouseItem(type);

            if (oldItem.Parent is Container)
            {
                ((Container)oldItem.Parent).DropItem(item);
            }
            else
            {
                BaseHouse house = BaseHouse.FindHouseAt(oldItem);

                item.MoveToWorld(oldItem.Location, oldItem.Map);

                item.IsLockedDown = oldItem.IsLockedDown;
                item.IsSecure = oldItem.IsSecure;
                item.Movable = oldItem.Movable;

                if (house != null)
                {
                    if (house.LockDowns.ContainsKey(oldItem))
                    {
                        house.LockDowns.Remove(oldItem);
                        house.LockDowns.Add(item, house.Owner);
                    }
                    else if (house.IsSecure(oldItem))
                    {
                        house.ReleaseSecure(house.Owner, oldItem);
                        house.AddSecure(house.Owner, item);
                    }
                    else if (addon != null)
                    {
                        if (house.Addons.ContainsKey(addon))
                        {
                            house.Addons.Remove(addon);
                        }

                        house.LockDowns.Add(item, house.Owner);
                    }
                }

                item.InvalidateProperties();
            }

            if (addon != null)
                addon.Delete();
            else
                oldItem.Delete();
        }
    }

    public class CraftableHouseAddonComponent : AddonComponent
    {
        public CraftableHouseAddonComponent(int id)
            : base(id)
        {
        }

        public CraftableHouseAddonComponent(Serial serial)
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

            Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
            {
                if (Addon is CraftableHouseAddon)
                    CraftableHouseItem.Replace(this, ((CraftableHouseAddon)Addon).ItemType);
            });
        }
    }

    public class CraftableHouseAddon : BaseAddon
    {
        public CraftableItemType ItemType { get; set; }
        public override BaseAddonDeed Deed => null;

        public CraftableHouseAddon()
        {
        }

        public CraftableHouseAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write((int)ItemType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            ItemType = (CraftableItemType)reader.ReadInt();
        }
    }

    public class CraftableHouseAddonDeed : BaseAddonDeed
    {
        public CraftableItemType ItemType { get; set; }
        public override BaseAddon Addon => null;

        public CraftableHouseAddonDeed()
        {
        }

        public CraftableHouseAddonDeed(CraftableItemType type)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
        }

        public CraftableHouseAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write((int)ItemType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            ItemType = (CraftableItemType)reader.ReadInt();

            Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                {
                    CraftableHouseItem.Replace(this, ItemType);
                });
        }
    }

    public class CraftableHouseDoorDeed : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public DoorType Type { get; set; }

        private CraftResource _Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return _Resource; }
            set
            {
                if (_Resource != value)
                {
                    _Resource = value;
                    Hue = CraftResources.GetHue(_Resource);

                    InvalidateProperties();
                }
            }
        }

        public CraftableHouseDoorDeed()
            : base(0x14F0)
        {
        }

        public CraftableHouseDoorDeed(DoorType type)
            : base(0x14F0)
        {
            Type = type;
        }

        public CraftableHouseDoorDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write((int)Type);
            writer.Write((int)_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Type = (DoorType)reader.ReadInt();
            _Resource = (CraftResource)reader.ReadInt();

            Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
            {
                Replace();
            });
        }

        public void Replace()
        {
            BaseDoor door;

            if (Type < DoorType.LeftMetalDoor_S_In)
                door = new CraftableStoneHouseDoor(Type, CraftableMetalHouseDoor.GetDoorFacing(Type));
            else
                door = new CraftableMetalHouseDoor(Type, CraftableMetalHouseDoor.GetDoorFacing(Type));

            if (door is IResource)
                ((IResource)door).Resource = _Resource;

            if (Parent is Container)
            {
                ((Container)Parent).DropItem(door);
            }
            else
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                door.MoveToWorld(Location, Map);

                door.IsLockedDown = IsLockedDown;
                door.IsSecure = IsSecure;
                door.Movable = Movable;

                if (house != null && house.LockDowns.ContainsKey(this))
                {
                    house.LockDowns.Remove(this);
                    house.LockDowns.Add(door, house.Owner);
                }
                else if (house != null && house.IsSecure(this))
                {
                    house.ReleaseSecure(house.Owner, this);
                    house.AddSecure(house.Owner, door);
                }
            }

            Delete();
        }
    }

    public enum DoorType
    {
        StoneDoor_S_In,
        StoneDoor_S_Out,
        StoneDoor_E_In,
        StoneDoor_E_Out,
        LeftMetalDoor_S_In,
        LeftMetalDoor_S_Out,
        RightMetalDoor_S_In,
        RightMetalDoor_S_Out,
        LeftMetalDoor_E_Out,
        LeftMetalDoor_E_In,
        RightMetalDoor_E_Out,
        RightMetalDoor_E_In
    }

    public class CraftableMetalHouseDoor : MetalHouseDoor, ICraftable, IFlipable
    {
        public DoorType Type { get; set; }

        public override int LabelNumber
        {
            get
            {
                switch (Type)
                {
                    default:
                    case DoorType.LeftMetalDoor_S_In: return 1156080;
                    case DoorType.RightMetalDoor_S_In: return 1156081;
                    case DoorType.LeftMetalDoor_E_In: return 1156352;
                    case DoorType.RightMetalDoor_E_In: return 1156353;
                    case DoorType.LeftMetalDoor_S_Out: return 1156350;
                    case DoorType.RightMetalDoor_S_Out: return 1156351;
                    case DoorType.LeftMetalDoor_E_Out: return 1156082;
                    case DoorType.RightMetalDoor_E_Out: return 1156083;
                }
            }
        }

        private CraftResource _Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return _Resource; }
            set
            {
                if (_Resource != value)
                {
                    _Resource = value;
                    Hue = CraftResources.GetHue(_Resource);

                    InvalidateProperties();
                }
            }
        }

        public CraftableMetalHouseDoor(DoorType type, DoorFacing facing)
            : base(facing)
        {
            Type = type;
            Movable = true;
        }

        public static Item Create(Mobile m, CraftItem craftItem, ITool tool)
        {
            DoorType type = DoorType.LeftMetalDoor_S_In;

            if (craftItem.Data is DoorType)
            {
                type = (DoorType)craftItem.Data;
            }

            return new CraftableMetalHouseDoor(type, GetDoorFacing(type));
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            if (craftItem != null)
            {
                Type resourceType = typeRes;

                if (resourceType == null)
                    resourceType = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(resourceType);
            }

            return quality;
        }

        public virtual void OnFlip(Mobile from)
        {
            if (Open)
            {
                from.SendMessage("The door must be closed before you can do that.");
                return; // TODO: Message?
            }

            switch (Type)
            {
                default:
                case DoorType.StoneDoor_S_In:
                    Type = DoorType.StoneDoor_S_Out;
                    break;
                case DoorType.StoneDoor_S_Out:
                    Type = DoorType.StoneDoor_S_In;
                    break;
                case DoorType.StoneDoor_E_In:
                    Type = DoorType.StoneDoor_E_Out;
                    break;
                case DoorType.StoneDoor_E_Out:
                    Type = DoorType.StoneDoor_E_In;
                    break;
                case DoorType.LeftMetalDoor_S_In:
                    Type = DoorType.LeftMetalDoor_S_Out;
                    break;
                case DoorType.RightMetalDoor_S_In:
                    Type = DoorType.RightMetalDoor_S_Out;
                    break;
                case DoorType.LeftMetalDoor_E_In:
                    Type = DoorType.LeftMetalDoor_E_Out;
                    break;
                case DoorType.RightMetalDoor_E_In:
                    Type = DoorType.RightMetalDoor_E_Out;
                    break;
                case DoorType.LeftMetalDoor_S_Out:
                    Type = DoorType.RightMetalDoor_E_Out;
                    break;
                case DoorType.RightMetalDoor_S_Out:
                    Type = DoorType.RightMetalDoor_S_In;
                    break;
                case DoorType.LeftMetalDoor_E_Out:
                    Type = DoorType.LeftMetalDoor_E_In;
                    break;
                case DoorType.RightMetalDoor_E_Out:
                    Type = DoorType.RightMetalDoor_E_In;
                    break;
            }

            Facing = GetDoorFacing(Type);

            ClosedID = 0x675 + (2 * (int)Facing);
            OpenedID = 0x676 + (2 * (int)Facing);

            Offset = GetOffset(Facing);
            InvalidateProperties();
        }

        public static DoorFacing GetDoorFacing(DoorType type)
        {
            switch (type)
            {
                default:
                case DoorType.StoneDoor_S_In: return DoorFacing.EastCW;
                case DoorType.StoneDoor_S_Out: return DoorFacing.WestCW;
                case DoorType.StoneDoor_E_In: return DoorFacing.NorthCW;
                case DoorType.StoneDoor_E_Out: return DoorFacing.SouthCW;
                case DoorType.LeftMetalDoor_S_In: return DoorFacing.WestCCW;
                case DoorType.RightMetalDoor_S_In: return DoorFacing.EastCW;
                case DoorType.LeftMetalDoor_E_In: return DoorFacing.SouthCCW;
                case DoorType.RightMetalDoor_E_In: return DoorFacing.NorthCW;
                case DoorType.LeftMetalDoor_S_Out: return DoorFacing.WestCW;
                case DoorType.RightMetalDoor_S_Out: return DoorFacing.EastCCW;
                case DoorType.LeftMetalDoor_E_Out: return DoorFacing.SouthCW;
                case DoorType.RightMetalDoor_E_Out: return DoorFacing.NorthCCW;
            }
        }

        public override void Use(Mobile from)
        {
            if (!Movable)
                base.Use(from);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (!CraftResources.IsStandard(_Resource))
            {
                list.Add(1050039, string.Format("#{0}\t{1}", CraftResources.GetLocalizationNumber(_Resource).ToString(), GetNameString())); // ~1_NUMBER~ ~2_ITEMNAME~
            }
            else
            {
                base.AddNameProperty(list);
            }
        }

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
            {
                name = string.Format("#{0}", LabelNumber);
            }

            return name;
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            BaseHouse h = BaseHouse.FindHouseAt(p, from.Map, ItemData.Height);

            if (h != null && !(h is HouseFoundation) && h.IsCoOwner(from))
            {
                return base.DropToWorld(from, p);
            }

            if (from.Backpack == null || !from.Backpack.TryDropItem(from, this, false))
            {
                Delete();
            }

            return false;
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (target.Backpack is StrongBackpack)
            {
                return false;
            }

            return base.DropToMobile(from, target, p);
        }

        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (target.Movable || target is StrongBackpack)
                return false;

            return base.OnDroppedInto(from, target, p);
        }

        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            if (target is Container && target.Movable || target is StrongBackpack)
                return false;

            return base.OnDroppedOnto(from, target);
        }

        public CraftableMetalHouseDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
            writer.Write((int)Type);
            writer.Write((int)_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Type = (DoorType)reader.ReadInt();
            _Resource = (CraftResource)reader.ReadInt();

            if (version == 0)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        BaseHouse house = BaseHouse.FindHouseAt(this);

                        if (house != null && house.Addons.ContainsKey(this))
                        {
                            house.Addons.Remove(this);
                            house.LockDowns[this] = house.Owner;

                            IsLockedDown = true;
                            Movable = false;
                        }
                    });
            }
        }
    }

    public class CraftableStoneHouseDoor : BaseHouseDoor, ICraftable, IFlipable
    {
        public DoorType Type { get; set; }

        public override int LabelNumber
        {
            get
            {
                switch (Type)
                {
                    default:
                    case DoorType.StoneDoor_S_In: return 1156078;
                    case DoorType.StoneDoor_S_Out: return 1156348;
                    case DoorType.StoneDoor_E_In: return 1156349;
                    case DoorType.StoneDoor_E_Out: return 1156079;
                }
            }
        }

        private CraftResource _Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return _Resource; }
            set
            {
                if (_Resource != value)
                {
                    _Resource = value;
                    Hue = CraftResources.GetHue(_Resource);

                    InvalidateProperties();
                }
            }
        }

        public CraftableStoneHouseDoor(DoorType type, DoorFacing facing)
            : base(facing, 0x324 + (2 * (int)facing), 0x325 + (2 * (int)facing), 0xED, 0xF4, GetOffset(facing))
        {
            Type = type;
            Movable = true;
        }

        public static Item Create(Mobile m, CraftItem craftItem, ITool tool)
        {
            DoorType type = DoorType.StoneDoor_S_In;

            if (craftItem.Data is DoorType)
            {
                type = (DoorType)craftItem.Data;
            }

            return new CraftableStoneHouseDoor(type, CraftableMetalHouseDoor.GetDoorFacing(type));
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            if (craftItem != null)
            {
                Type resourceType = typeRes;

                if (resourceType == null)
                    resourceType = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(resourceType);
            }

            return quality;
        }

        public override void Use(Mobile from)
        {
            if (!Movable)
                base.Use(from);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (!CraftResources.IsStandard(_Resource))
            {
                list.Add(1050039, string.Format("#{0}\t{1}", CraftResources.GetLocalizationNumber(_Resource).ToString(), GetNameString())); // ~1_NUMBER~ ~2_ITEMNAME~
            }
            else
            {
                base.AddNameProperty(list);
            }
        }

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
            {
                name = string.Format("#{0}", LabelNumber);
            }

            return name;
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            BaseHouse h = BaseHouse.FindHouseAt(p, from.Map, ItemData.Height);

            if (h != null && !(h is HouseFoundation) && h.IsCoOwner(from))
            {
                return base.DropToWorld(from, p);
            }

            if (from.Backpack == null || !from.Backpack.TryDropItem(from, this, false))
            {
                Delete();
            }

            return false;
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (target.Backpack is StrongBackpack)
            {
                return false;
            }

            return base.DropToMobile(from, target, p);
        }

        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (target.Movable || target is StrongBackpack)
                return false;

            return base.OnDroppedInto(from, target, p);
        }

        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            if (target is Container && target.Movable || target is StrongBackpack)
                return false;

            return base.OnDroppedOnto(from, target);
        }

        public virtual void OnFlip(Mobile from)
        {
            if (Open)
            {
                from.SendMessage("The door must be closed before you can do that.");
                return; // TODO: Message?
            }

            switch (Type)
            {
                default:
                case DoorType.StoneDoor_S_In:
                    Type = DoorType.StoneDoor_S_Out;
                    break;
                case DoorType.StoneDoor_S_Out:
                    Type = DoorType.StoneDoor_S_In;
                    break;
                case DoorType.StoneDoor_E_In:
                    Type = DoorType.StoneDoor_E_Out;
                    break;
                case DoorType.StoneDoor_E_Out:
                    Type = DoorType.StoneDoor_E_In;
                    break;
                case DoorType.LeftMetalDoor_S_In:
                    Type = DoorType.LeftMetalDoor_S_Out;
                    break;
                case DoorType.RightMetalDoor_S_In:
                    Type = DoorType.RightMetalDoor_S_Out;
                    break;
                case DoorType.LeftMetalDoor_E_In:
                    Type = DoorType.LeftMetalDoor_E_Out;
                    break;
                case DoorType.RightMetalDoor_E_In:
                    Type = DoorType.RightMetalDoor_E_Out;
                    break;
                case DoorType.LeftMetalDoor_S_Out:
                    Type = DoorType.RightMetalDoor_E_Out;
                    break;
                case DoorType.RightMetalDoor_S_Out:
                    Type = DoorType.RightMetalDoor_S_In;
                    break;
                case DoorType.LeftMetalDoor_E_Out:
                    Type = DoorType.LeftMetalDoor_E_In;
                    break;
                case DoorType.RightMetalDoor_E_Out:
                    Type = DoorType.RightMetalDoor_E_In;
                    break;
            }

            Facing = CraftableMetalHouseDoor.GetDoorFacing(Type);

            ClosedID = 0x324 + (2 * (int)Facing);
            OpenedID = 0x325 + (2 * (int)Facing);

            Offset = GetOffset(Facing);
            InvalidateProperties();
        }

        public CraftableStoneHouseDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
            writer.Write((int)Type);
            writer.Write((int)_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Type = (DoorType)reader.ReadInt();
            _Resource = (CraftResource)reader.ReadInt();

            if (version == 0)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                {
                    BaseHouse house = BaseHouse.FindHouseAt(this);

                    if (house != null && house.Addons.ContainsKey(this))
                    {
                        house.Addons.Remove(this);
                        house.LockDowns[this] = house.Owner;

                        IsLockedDown = true;
                        Movable = false;
                    }
                });
            }
        }
    }
}
