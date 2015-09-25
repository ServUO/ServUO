using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class RowBoatHelm : RowBoatItem, IHelm
    {
        public RowBoatHelm(RowBoat rowBoat, int northItemID, Point3D initOffset)
            : base(rowBoat, northItemID, initOffset)
        {
            Name = "Wheel";
        }

        public RowBoatHelm(Serial serial)
            : base(serial)
        {
        }
		
        /*public void SetFacing(Direction oldFacing, Direction newFacing)
        {
            switch (newFacing)
            {
                case Direction.South: ItemID = 0x3EC4; break;
                case Direction.North: ItemID = 0x3EBC; break;
                case Direction.West: ItemID = 0x3E76; break;
                case Direction.East: ItemID = 0x3E63; break;
            }

            if (oldFacing == Server.Direction.North)
            {
                Location = (new Point3D(X, Y, Z));
            }
            else if (newFacing == Server.Direction.North)
            {
                switch (oldFacing)
                {
                    case Server.Direction.South: Location = (new Point3D(X, Y, Z)); break;
                    case Server.Direction.East: Location = (new Point3D(X, Y, Z)); break;
                    case Server.Direction.West: Location = (new Point3D(X, Y, Z)); break;
                }
            }
        }*/		

        public override void OnDoubleClick(Mobile from)
        {
            if (Ship != null && Ship.IsDriven)
            {
                Ship.LeaveCommand(from);
                from.SendMessage("You are no longer piloting this vessel");
            }
            else
            {
                if (Ship != null)
                {
					if (!from.Mounted)
					{
						if (from == Ship.Owner)
						{
							from.SendMessage("You are now piloting this vessel");
							Ship.TakeCommand(from);
						}
					}
					else
					{
						from.SendMessage("You can not control the ship while mounted");
					}                   
                }
            }
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //current version is 0
        }
        #endregion
    }

    public class RowBoatRope : RowBoatItem
    {
        private RowBoat _rowBoat;
        private BoatRopeSide _side;

        [CommandProperty(AccessLevel.GameMaster, true)]
        public override bool ShareHue
        {
            get { return false; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public RowBoat RowBoat { get { return _rowBoat; } set { _rowBoat = value; } }

        public RowBoatRope(RowBoat rowBoat, int northItemID, Point3D initOffset)
            : base(rowBoat, northItemID, initOffset)
        {
            _rowBoat = rowBoat;
            Movable = false;
            Name = "Mooring Line";
        }

        public RowBoatRope(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            base.OnDoubleClick(from);
            from.MoveToWorld(Location, Map);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (_rowBoat != null && !_rowBoat.Contains(from))
            {
                _rowBoat.Refresh();
                from.SendMessage("Welcome aboard !");
                base.OnDoubleClick(from);
                from.MoveToWorld(Location, Map);
				Ship.Embark(from);
            }
            else if (_rowBoat != null && _rowBoat.Contains(from))
            {
                Map map = Map;

                if (map == null)
                    return;

                int rx = 0, ry = 0;

				if (_rowBoat.Facing == Direction.North)
					rx = 1;
				else if (_rowBoat.Facing == Direction.South)
					rx = -1;
				else if (_rowBoat.Facing == Direction.East)
					ry = 1;
				else if (_rowBoat.Facing == Direction.West)
					ry = -1;
				
				for (int i = 1; i <= 6; ++i)
                {
                    int x = X + (i * rx);
                    int y = Y + (i * ry);
                    int z;

                    for (int j = -16; j <= 16; ++j)
                    {
                        z = from.Z + j;

                        if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                        {
                            if (i == 1 && j >= -2 && j <= 2)
                                return;

                            from.Location = new Point3D(x, y, z);
							Ship.Disembark(from);
                            return;
                        }
                    }

                    z = map.GetAverageZ(x, y);

                    if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                    {
                        if (i == 1)
                            return;

                        from.Location = new Point3D(x, y, z);
						Ship.Disembark(from);
                        return;
                    }
                }	

				rx = 0; 
				ry = 0;
				
				if (_rowBoat.Facing == Direction.North)
					rx = -1;
				else if (_rowBoat.Facing == Direction.South)
					rx = 1;
				else if (_rowBoat.Facing == Direction.East)
					ry = -1;
				else if (_rowBoat.Facing == Direction.West)
					ry = 1;
                
                for (int i = 1; i <= 6; ++i)
                {
                    int x = X + (i * rx);
                    int y = Y + (i * ry);
                    int z;

                    for (int j = -16; j <= 16; ++j)
                    {
                        z = from.Z + j;

                        if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                        {
                            if (i == 1 && j >= -2 && j <= 2)
                                return;

                            from.Location = new Point3D(x, y, z);
							Ship.Disembark(from);
                            return;
                        }
                    }

                    z = map.GetAverageZ(x, y);

                    if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                    {
                        if (i == 1)
                            return;

                        from.Location = new Point3D(x, y, z);
						Ship.Disembark(from);
                        return;
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version

            writer.Write(_rowBoat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        _rowBoat = reader.ReadItem() as RowBoat;

                        if (_rowBoat == null)
                            Delete();

                        break;
                    }
            }
        }
    }
}
