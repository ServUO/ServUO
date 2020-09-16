#region

using Server.Multis;

#endregion

namespace Server.Items
{
    public enum FencingType
    {
        Arch,
        NWCornerPiece,
        SouthFacingPieces,
        EastFacingPieces,
        GateSouth,
        GateEast,
        CornerPiece
    }

    [Furniture]
    public class DecorativeStableFencing : Item, IFlipable, IDyable
    {
        public override int LabelNumber => IDs[(int)_Type][0];

        private static readonly int[][] IDs =
        {
			new[] { 1126213, 42189, 42190 },    // Arch
			new[] { 1126197, 42171 },           // NWCornerPiece
			new[] { 1126197, 42172, 42173 },    // SouthFacingPieces
			new[] { 1126197, 42173, 42172 },    // EastFacingPieces
			new[] { 1126211, 42176 },           // GateSouth
			new[] { 1126211, 42187 },           // GateEast
			new[] { 1126197, 42174 }            // CornerPiece
		};

        private FencingType _Type;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanFlip => IDs[(int)_Type].Length > 2;

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        [Constructable]
        public DecorativeStableFencing(FencingType type)
            : base(IDs[(int)type][1])
        {
            LootType = LootType.Blessed;
            _Type = type;
        }

        public void OnFlip(Mobile from)
        {
            var list = IDs[(int)_Type];

            if (CanFlip && list.Length > 2)
            {
                for (var i = 1; i < list.Length; i++)
                {
                    var id = list[i];

                    if (ItemID == id)
                    {
                        if (i >= list.Length - 1)
                        {
                            ItemID = list[1];
                            break;
                        }

                        ItemID = list[i + 1];
                        break;
                    }
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1153494); // House Only
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            var h = BaseHouse.FindHouseAt(p, from.Map, ItemData.Height);

            if (h != null)
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
            if (target.RootParentEntity == null)
            {
                var h = BaseHouse.FindHouseAt(target.Location, target.Map, ItemData.Height);

                if (h == null || target is StrongBackpack)
                {
                    return false;
                }
            }

            return base.OnDroppedInto(from, target, p);
        }

        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            if (target.RootParentEntity == null)
            {
                var h = BaseHouse.FindHouseAt(target.Location, target.Map, ItemData.Height);

                if (h == null || target is StrongBackpack)
                {
                    return false;
                }
            }

            return base.OnDroppedOnto(from, target);
        }

        public DecorativeStableFencing(Serial serial)
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
            var version = reader.ReadInt();

            _Type = (FencingType)reader.ReadInt();
        }
    }
}
