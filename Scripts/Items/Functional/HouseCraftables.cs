using System;
using Server;
using Server.Mobiles;
using Server.Engines.Craft;
using Server.Multis;

namespace Server.Items
{
    public enum CraftableAddonType
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
        EndCurledMetalSignHanger,
    }

    public class CraftableHouseAddonComponent : AddonComponent, IFlippable
    {
        public int NorthID { get { return 0; } }
        public int WestID { get { return 0; } }

        public CraftableHouseAddonComponent(int id)
            : base(id)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1153494); // House Only
        }

        public void OnFlip()
        {
            CraftableHouseAddon addon = Addon as CraftableHouseAddon;
            int[] list = CraftableHouseAddon.IDs[(int)addon.AddonType];
            //Utility.WriteLine(ConsoleColor.Yellow, String.Format("Trying to Flip: {0} / ID Length: {1}", addon.CanFlip ? "YES" : "NO", list.Length));

            if (addon != null && addon.CanFlip && list.Length > 2)
            {
                for (int i = 1; i < list.Length; i++)
                {
                    int id = list[i];

                    if (this.ItemID == id)
                    {
                        if (i >= list.Length - 1)
                        {
                            this.ItemID = list[1];
                            break;
                        }
                        else
                        {
                            this.ItemID = list[i + 1];
                            break;
                        }
                    }
                }
            }
        }

        public CraftableHouseAddonComponent(Serial serial)
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

    public class CraftableHouseAddon : BaseAddon
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

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanFlip
        {
            get
            {
                int value = (int)_Type;

                return (value >= 0 && value <= 13) || value >= 17;
            }
        }

        private CraftableAddonType _Type;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftableAddonType AddonType
        {
            get { return _Type; }
            set
            {
                _Type = value;
                InvalidateID();
            }
        }

        public override BaseAddonDeed Deed { get { return new CraftableHouseAddonDeed(AddonType); } }
        public override bool RestrictToClassicHouses { get { return true; } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public CraftableHouseAddon(CraftableAddonType type)
        {
            AddComponent(new CraftableHouseAddonComponent(IDs[(int)type][1]), 0, 0, 0);
            _Type = type;
        }

        public void InvalidateID()
        {
            if (Components.Count > 0)
                Components[0].ItemID = IDs[(int)_Type][1];
        }

        public CraftableHouseAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            _Type = (CraftableAddonType)reader.ReadInt();
        }
    }

    public class CraftableHouseAddonDeed : BaseAddonDeed, ICraftable
    {
        public CraftableAddonType AddonType { get; set; }
        public override BaseAddon Addon { get { return new CraftableHouseAddon(AddonType); } }

        public override int LabelNumber
        {
            get
            {
                return CraftableHouseAddon.IDs[(int)AddonType][0];
            }
        }

        public CraftableHouseAddonDeed()
        {
        }

        [Constructable]
        public CraftableHouseAddonDeed(CraftableAddonType type)
        {
            AddonType = type;
        }

        public override int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            if (craftItem != null && craftItem.Data is CraftableAddonType)
            {
                AddonType = (CraftableAddonType)craftItem.Data;
            }

            return quality;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1153494); // House Only
        }

        public CraftableHouseAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)AddonType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            AddonType = (CraftableAddonType)reader.ReadInt();
        }
    }

    public class CraftableHouseDoorDeed : Item, ICraftable
    {
        public override int LabelNumber
        {
            get
            {
                switch (this.Type)
                {
                    default:
                    case DoorType.StoneDoor_S_In: return 1156078;
                    case DoorType.StoneDoor_S_Out: return 1156348;
                    case DoorType.StoneDoor_E_In: return 1156349;
                    case DoorType.StoneDoor_E_Out: return 1156079;
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

        [Constructable]
        public CraftableHouseDoorDeed()
            : base(0x14F0)
        {
        }

        [Constructable]
        public CraftableHouseDoorDeed(DoorType type)
            : base(0x14F0)
        {
            this.Type = type;
        }

        public CraftableHouseDoorDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.BeginTarget(10, true, Server.Targeting.TargetFlags.None, (m, targeted) =>
                {
                    if (IsChildOf(from.Backpack))
                    {
                        IPoint3D p = targeted as IPoint3D;
                        Map map = from.Map;

                        if (p == null || map == null || Deleted)
                            return;

                        Server.Spells.SpellHelper.GetSurfaceTop(ref p);

                        BaseHouse house = null;
                        Item door;

                        if (this.Type < DoorType.LeftMetalDoor_S_In)
                            door = new CraftableStoneHouseDoor(this.Type, GetDoorFacing(this.Type));
                        else
                            door = new CraftableMetalHouseDoor(this.Type, GetDoorFacing(this.Type));

                        if (door is CraftableMetalHouseDoor)
                            ((CraftableMetalHouseDoor)door).Resource = _Resource;
                        else if (door is CraftableStoneHouseDoor)
                            ((CraftableStoneHouseDoor)door).Resource = _Resource; 

                        AddonFitResult res = CouldFit(door, p, map, from, ref house);

                        switch (res)
                        {
                            case AddonFitResult.Valid:
                                PlaceDoor(door, p, map, house);
                                return;
                            case AddonFitResult.Blocked:
                                from.SendLocalizedMessage(500269); // You cannot build that there.
                                break;
                            case AddonFitResult.NotInHouse:
                                from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                                break;
                            case AddonFitResult.DoorsNotClosed:
                                from.SendMessage("You must close all house doors before placing this.");
                                break;
                            case AddonFitResult.DoorTooClose:
                                from.SendLocalizedMessage(500271); // You cannot build near the door.
                                break;
                            case AddonFitResult.BadHouse:
                                from.SendLocalizedMessage(500269); // You cannot build that there.
                                break;
                        }

                        door.Delete();
                    }
                    else
                        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                });
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public void PlaceDoor(Item door, IPoint3D p, Map map, BaseHouse house)
        {
            door.MoveToWorld(new Point3D(p), map);

            if (house != null)
                house.Addons.Add(door);

            this.Delete();
        }

        public static AddonFitResult CouldFit(Item item, IPoint3D p, Map map, Mobile from, ref BaseHouse house)
        {
            Point3D p3D = new Point3D(p);

            if (!map.CanFit(p3D.X, p3D.Y, p3D.Z, item.ItemData.Height, false, true, (item.Z == 0)))
                return AddonFitResult.Blocked;
            else if (!BaseAddon.CheckHouse(from, p3D, map, item.ItemData.Height, ref house))
                return AddonFitResult.NotInHouse;
            else if (house is HouseFoundation)
                return AddonFitResult.BadHouse;

            if (house != null)
            {
                System.Collections.ArrayList doors = house.Doors;

                for (int i = 0; i < doors.Count; ++i)
                {
                    BaseDoor door = doors[i] as BaseDoor;

                    if (door != null && door.Open)
                        return AddonFitResult.DoorsNotClosed;

                    Point3D doorLoc = door.GetWorldLocation();
                    int doorHeight = door.ItemData.CalcHeight;

                    int height = item.ItemData.CalcHeight;

                    if (Utility.InRange(doorLoc, p3D, 1) && (p3D.Z == doorLoc.Z || ((p3D.Z + height) > doorLoc.Z && (doorLoc.Z + doorHeight) > p3D.Z)))
                        return AddonFitResult.DoorTooClose;
                }
            }

            return AddonFitResult.Valid;
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            if (craftItem != null && craftItem.Data is DoorType)
            {
                this.Type = (DoorType)craftItem.Data;

                Type resourceType = typeRes;

                if (resourceType == null)
                    resourceType = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(resourceType);
            }

            return quality;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)this.Type);
            writer.Write((int)_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            this.Type = (DoorType)reader.ReadInt();
            _Resource = (CraftResource)reader.ReadInt();
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

    public class CraftableMetalHouseDoor : MetalHouseDoor, IChopable
    {
        public DoorType Type { get; set; }

        public override int LabelNumber
        {
            get
            {
                switch (this.Type)
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
            this.Type = type;
        }

        public virtual void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsOwner(from) && house.Addons.Contains(this))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                Delete();

                house.Addons.Remove(this);
                var deed = new CraftableHouseDoorDeed();

                deed.Type = this.Type;
                deed.Resource = _Resource;

                if (deed != null)
                {
                    from.AddToBackpack(deed);
                }
            }
        }

        public CraftableMetalHouseDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)this.Type);
            writer.Write((int)_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            this.Type = (DoorType)reader.ReadInt();
            _Resource = (CraftResource)reader.ReadInt();
        }
    }

    public class CraftableStoneHouseDoor : BaseHouseDoor, IChopable
    {
        public DoorType Type { get; set; }

        public override int LabelNumber
        {
            get
            {
                switch (this.Type)
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
            : base(facing, 0x324 + (2 * (int)facing), 0x325 + (2 * (int)facing), 0xED, 0xF4, BaseDoor.GetOffset(facing))
        {
            this.Type = type;
        }

        public virtual void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsOwner(from) && house.Addons.Contains(this))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                Delete();

                house.Addons.Remove(this);
                var deed = new CraftableHouseDoorDeed();

                if (deed != null)
                {
                    deed.Type = this.Type;

                    if (deed is CraftableHouseDoorDeed)
                        ((CraftableHouseDoorDeed)deed).Resource = _Resource;

                    from.AddToBackpack(deed);
                }
            }
        }

        public CraftableStoneHouseDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)this.Type);
            writer.Write((int)_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            this.Type = (DoorType)reader.ReadInt();
            _Resource = (CraftResource)reader.ReadInt();
        }
    }
}