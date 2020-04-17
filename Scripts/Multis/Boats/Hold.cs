using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class Hold : Container
    {
        public override int LabelNumber => 1149699;  // cargo hold

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat Boat { get; private set; }

        public override int DefaultMaxWeight => 400;

        public Hold(BaseBoat boat) : base(0x3EAE)
        {
            Boat = boat;
            Movable = false;
        }

        public Hold(Serial serial) : base(serial)
        {
        }

        public virtual void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.East: ItemID = 0x3E65; break;
                case Direction.West: ItemID = 0x3E93; break;
                case Direction.North: ItemID = 0x3EAE; break;
                case Direction.South: ItemID = 0x3EB9; break;
            }
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (Boat == null || !Boat.Contains(from) || Boat.IsMoving)
                return false;

            return base.OnDragDrop(from, item);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (Boat == null || !Boat.Contains(from) || Boat.IsMoving)
                return false;

            return base.OnDragDropInto(from, item, p);
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (item != this && (Boat == null || !Boat.Contains(from) || Boat.IsMoving))
                return false;

            return base.CheckItemUse(from, item);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (Boat == null || !Boat.Contains(from) || Boat.IsMoving)
                return false;

            return base.CheckLift(from, item, ref reject);
        }

        public override void OnAfterDelete()
        {
            if (Boat != null)
                Boat.Delete();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Boat == null || !Boat.Contains(from))
            {
                if (Boat.TillerMan != null)
                    Boat.TillerManSay(502490); // You must be on the ship to open the hold.
            }
            else if (Boat.IsMoving && Boat.IsClassicBoat)
            {
                if (Boat.TillerMan != null)
                    Boat.TillerManSay(502491); // I can not open the hold while the ship is moving.
            }
            else
                base.OnDoubleClick(from);
        }

        public override bool IsDecoContainer => false;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        Boat = reader.ReadItem() as BaseBoat;

                        if (Boat == null || Parent != null)
                            Delete();

                        Movable = false;

                        break;
                    }
            }
        }
    }
}
