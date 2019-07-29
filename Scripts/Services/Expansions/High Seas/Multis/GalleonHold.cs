using Server;
using System;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;

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
                if (Galleon is GargishGalleon)
                    return 12000;
                if (Galleon is OrcishGalleon)
                    return 14000;
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

        public override void OnItemAdded(Item item)
        {
            Galleon?.InvalidateHoldPorperties();
            base.OnItemAdded(item);
        }

        public override void OnItemRemoved(Item item)
        {
            Galleon?.InvalidateHoldPorperties();
            base.OnItemRemoved(item);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon == null || from.AccessLevel > AccessLevel.Player)
                base.OnDoubleClick(from);
            else if (!Galleon.Contains(from))
            {
                if(Galleon.TillerMan != null)
                    Galleon.TillerManSay(502490); // You must be on the ship to open the hold.
            }
            else if (Galleon.Owner is PlayerMobile && !Galleon.Scuttled && Galleon.GetSecurityLevel(from) < SecurityLevel.Officer)
                from.SendMessage("You must be at least an officer to access the cargo hold.");
            else
                base.OnDoubleClick(from);
        }

        public GalleonHold(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
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
        public override int LabelNumber => 1149699;
        public override bool ForceShowProperties => true;

        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonHold Hold { get; private set; }

        public HoldItem(GalleonHold hold, int itemid) : base(itemid)
        {
            Hold = hold;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Hold?.Galleon == null || !from.InRange(Location, 2))
                return;

            Hold.OnDoubleClick(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1072241, $"{Hold.TotalItems}\t{Hold.MaxItems}\t{Hold.TotalWeight}\t{Hold.MaxWeight}"); // Contents: ~1_COUNT~/~2_MAXCOUNT~ items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones
        }

        public HoldItem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
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
