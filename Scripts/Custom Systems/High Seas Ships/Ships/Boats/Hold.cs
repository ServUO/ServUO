using Server.Multis;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class NewHold : BaseShipContainer, IFacingChange
    {
        private NewBaseBoat _boat;

        [CommandProperty(AccessLevel.GameMaster)]
        public NewBaseBoat Boat { get { return _boat; } }	
	
        public override bool IsDecoContainer { get { return false; } }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public override bool ShareHue { get { return true; } }

        public NewHold(NewBaseBoat boat, Point3D initOffset)
            : base(boat, 0x3EAE, initOffset)
        {
			_boat = boat;
        }

        public NewHold(Serial serial)
            : base(serial)
        {
        }

        public void SetFacing(Direction OldFacing, Direction newFacing)
        {
            switch (newFacing)
            {
                case Direction.East: _boat.SetItemIDOnSmooth(this, 0x3E65); break;
                case Direction.West: _boat.SetItemIDOnSmooth(this, 0x3E93); break;
                case Direction.North: _boat.SetItemIDOnSmooth(this, 0x3EAE); break;
                case Direction.South: _boat.SetItemIDOnSmooth(this, 0x3EB9); break;
            }
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (Ship == null || !Ship.IsOnBoard(from) || Ship.IsMoving)
                return false;

            return base.OnDragDrop(from, item);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (Ship == null || !Ship.IsOnBoard(from) || Ship.IsMoving)
                return false;

            return base.OnDragDropInto(from, item, p);
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (item != this && (Ship == null || !Ship.IsOnBoard(from) || Ship.IsMoving))
                return false;

            return base.CheckItemUse(from, item);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (Ship == null || !Ship.IsOnBoard(from) || Ship.IsMoving)
                return false;

            return base.CheckLift(from, item, ref reject);
        }

        public override void OnDoubleClick(Mobile from)
        {
            NewBaseBoat ship = Ship as NewBaseBoat;
            if (ship == null || !ship.IsOnBoard(from))
            {
                if (ship != null && ship.TillerMan != null)
                    ship.TillerMan.Say(502490); // You must be on the ship to open the hold.
            }
            else if (ship != null && ship.IsMovingShip)
            {
                if (ship != null && ship.TillerMan != null)
                    ship.TillerMan.Say(502491); // I can not open the hold while the ship is moving.
            }
            else
            {
                base.OnDoubleClick(from);               
			}
        }
       
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
			
			writer.Write((NewBaseBoat)_boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            switch (version)
            {
                case 1:
                    {
                        _boat = reader.ReadItem() as NewBaseBoat;
                        break;
                    }			
			}
        }
    }
}