using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class GalleonHold : Container, IGalleonFixture
    {
        public override int LabelNumber => 1149699;  // cargo hold
        public override bool ForceShowProperties => true;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get; set; }

        public override int DefaultMaxWeight
        {
            get
            {
                if (Galleon is BritannianShip)
                    return 28000;
                else if (Galleon is GargishGalleon)
                    return 12000;
                else if (Galleon is OrcishGalleon)
                    return 14000;
                else
                    return 16000;
            }
        }

        public override bool IsDecoContainer => false;

        public override Rectangle2D Bounds => new Rectangle2D(46, 74, 150, 110);
        public override int DefaultGumpID => 0x4C;
        public override int DefaultDropSound => 0x42;

        public GalleonHold(BaseGalleon galleon, int itemID) : base(itemID)
        {
            Galleon = galleon;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon == null || from.AccessLevel > AccessLevel.Player)
            {
                base.OnDoubleClick(from);
            }
            else if (!Galleon.Contains(from))
            {
                if (Galleon.TillerMan != null)
                    Galleon.TillerManSay(502490); // You must be on the ship to open the hold.
            }
            else if (Galleon.Owner is PlayerMobile && !Galleon.Scuttled && Galleon.GetSecurityLevel(from) < SecurityLevel.Officer)
            {
                from.Say(1010436); // You do not have permission to do this.
            }
            else
                base.OnDoubleClick(from);
        }

        public GalleonHold(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                reader.ReadItem();
            }
        }
    }

    public class HoldItem : Item, IGalleonFixture
    {
        public override int LabelNumber => 1149699;  // cargo hold
        public override bool ForceShowProperties => true;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get; set; }

        public HoldItem(BaseGalleon g, int itemid)
            : base(itemid)
        {
            Galleon = g;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon == null || Galleon.GalleonHold == null || !from.InRange(Location, 2))
                return;

            Galleon.GalleonHold.OnDoubleClick(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Galleon != null && Galleon.GalleonHold != null)
            {
                GalleonHold hold = Galleon.GalleonHold;
                list.Add(1072241, string.Format("{0}\t{1}\t{2}\t{3}", hold.TotalItems, hold.MaxItems, hold.TotalWeight, hold.MaxWeight)); // Contents: ~1_COUNT~/~2_MAXCOUNT~ items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones
            }
        }

        public HoldItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                reader.ReadInt();
            }
        }
    }
}
