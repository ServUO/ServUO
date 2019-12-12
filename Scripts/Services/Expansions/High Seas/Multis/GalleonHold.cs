using Server;
using System;
using Server.Multis;
using Server.Mobiles;

namespace Server.Items
{
    public class GalleonHold : Container
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get; private set; }

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

        public override bool IsDecoContainer { get { return false; } }

        public override Rectangle2D Bounds { get { return new Rectangle2D(46, 74, 150, 110); } }
        public override int DefaultGumpID { get { return 0x4C; } }
        public override int DefaultDropSound { get { return 0x42; } }

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
            writer.Write((int)0);

            writer.Write(Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Galleon = reader.ReadItem() as BaseGalleon;

            if (ItemID == 33648)
                ItemID = 23648;
        }
    }

    public class HoldItem : Item
    {
        public override int LabelNumber { get { return 1149699; } } // cargo hold
        public override bool ForceShowProperties { get { return true; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonHold Hold { get; private set; }

        public HoldItem(GalleonHold hold, int itemid) : base(itemid)
        {
            Hold = hold;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Hold == null || Hold.Galleon == null || !from.InRange(Location, 2))
                return;

            Hold.OnDoubleClick(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072241, String.Format("{0}\t{1}\t{2}\t{3}", Hold.TotalItems, Hold.MaxItems, Hold.TotalWeight, Hold.MaxWeight)); // Contents: ~1_COUNT~/~2_MAXCOUNT~ items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones
        }

        public HoldItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(Hold);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Hold = reader.ReadItem() as GalleonHold;
        }
    }
}
