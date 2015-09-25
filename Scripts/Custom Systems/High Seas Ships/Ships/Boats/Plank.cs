using System;

using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class NewPlank : BaseShipItem, ILockable, IFacingChange
    {
        private Timer _closeTimer;

        #region Properties
		
        [CommandProperty(AccessLevel.GameMaster, true)]
        public override bool ShareHue
        {
            get { return true; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NewBaseBoat Boat { get; set; }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public PlankSide Side { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public uint KeyValue { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsOpen { get { return (ItemID == 0x3ED5 || ItemID == 0x3ED4 || ItemID == 0x3E84 || ItemID == 0x3E89); } }

        private bool Starboard { get { return (Side == PlankSide.Starboard); } }
        #endregion

        public NewPlank(NewBaseBoat boat, Point3D initOffset, PlankSide side, uint keyValue)
            : base(boat, 0x3EB1 + (int)side, initOffset)
        {
			Boat = boat;
            Side = side;
            KeyValue = keyValue;
            Locked = true;
        }

        public NewPlank(Serial serial)
            : base(serial)
        {
        }

        public void SetFacing(Direction oldFacing, Direction newFacing)
        {
            if (IsOpen)
            {
                switch (newFacing)
                {
                    case Direction.North: Boat.SetItemIDOnSmooth(this, Starboard ? 0x3ED4 : 0x3ED5); break;
                    case Direction.East: Boat.SetItemIDOnSmooth(this, Starboard ? 0x3E84 : 0x3E89); break;
                    case Direction.South: Boat.SetItemIDOnSmooth(this, Starboard ? 0x3ED5 : 0x3ED4); break;
                    case Direction.West: Boat.SetItemIDOnSmooth(this, Starboard ? 0x3E89 : 0x3E84); break;
                }
            }
            else
            {
                switch (newFacing)
                {
                    case Direction.North: Boat.SetItemIDOnSmooth(this, Starboard ? 0x3EB2 : 0x3EB1); break;
                    case Direction.East: Boat.SetItemIDOnSmooth(this, Starboard ? 0x3E85 : 0x3E8A); break;
                    case Direction.South: Boat.SetItemIDOnSmooth(this, Starboard ? 0x3EB1 : 0x3EB2); break;
                    case Direction.West: Boat.SetItemIDOnSmooth(this, Starboard ? 0x3E8A : 0x3E85); break;
                }
            }
        }

        public void Open()
        {
            if (IsOpen || Deleted)
                return;

            if (_closeTimer != null)
                _closeTimer.Stop();

            _closeTimer = new CloseTimer(this);
            _closeTimer.Start();

            switch (ItemID)
            {
                case 0x3EB1: ItemID = 0x3ED5; break;
                case 0x3E8A: ItemID = 0x3E89; break;
                case 0x3EB2: ItemID = 0x3ED4; break;
                case 0x3E85: ItemID = 0x3E84; break;
            }
        }

        public override bool OnMoveOver(Mobile from)
        {
            if (IsOpen)
            {
                if ((from.Direction & Direction.Running) != 0 || (Ship != null && !Ship.IsOnBoard(from)))
                {
                    Ship.Embark(from);
                    return true;
                }

                Map map = Map;

                if (map == null)
                    return false;

                int rx = 0, ry = 0;

                if (ItemID == 0x3ED4)
                    rx = 1;
                else if (ItemID == 0x3ED5)
                    rx = -1;
                else if (ItemID == 0x3E84)
                    ry = 1;
                else if (ItemID == 0x3E89)
                    ry = -1;

                for (int i = 1; i <= 6; ++i)
                {
                    int x = X + (i * rx);
                    int y = Y + (i * ry);
                    int z;

                    for (int j = -8; j <= 8; ++j)
                    {
                        z = from.Z + j;

                        if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                        {
                            if (i == 1 && j >= -2 && j <= 2)
                                return true;

                            from.Location = new Point3D(x, y, z);
							Ship.Disembark(from);
                            return false;
                        }
                    }

                    z = map.GetAverageZ(x, y);

                    if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                    {
                        if (i == 1)
                            return true;

                        from.Location = new Point3D(x, y, z);
						Ship.Disembark(from);
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanClose()
        {
            Map map = Map;

            if (map == null || Deleted)
                return false;

            foreach (object o in this.GetObjectsInRange(0))
            {
                if (o != this)
                    return false;
            }

            return true;
        }

        public void Close()
        {
            if (!IsOpen || !CanClose() || Deleted)
                return;

            if (_closeTimer != null)
                _closeTimer.Stop();

            _closeTimer = null;

            switch (ItemID)
            {
                case 0x3ED5: ItemID = 0x3EB1; break;
                case 0x3E89: ItemID = 0x3E8A; break;
                case 0x3ED4: ItemID = 0x3EB2; break;
                case 0x3E84: ItemID = 0x3E85; break;
            }
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Ship == null)
                return;

            if (from.InRange(GetWorldLocation(), 8))
            {
                if (Ship.IsOnBoard(from))
                {
                    if (IsOpen)
                        Close();
                    else
                        Open();
                }
                else
                {
                    if (!IsOpen)
                    {
                        if (!Locked)
                            Open();
                        else if (from.AccessLevel >= AccessLevel.GameMaster)
                        {
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502502); // That is locked but your godly powers allow access
                            Open();
                        }
                        else
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502503); // That is locked.
                    }
                    else if (!Locked)
                    {
                        from.Location = new Point3D(this.X, this.Y, this.Z + 3);
						
						Ship.Embark(from);
						
						if ( Boat != null )
							Boat.Refresh();
                    }
                    else if (from.AccessLevel >= AccessLevel.GameMaster)
                    {
                        from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502502); // That is locked but your godly powers allow access
                        from.Location = new Point3D(this.X, this.Y, this.Z + 3);
						
						Ship.Embark(from);
						
						if ( Boat != null )
							Boat.Refresh();
                    }
                    else
                    {
                        from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502503); // That is locked.
                    }
                }
            }
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);//version

            writer.Write((int)Side);
            writer.Write(Locked);
            writer.Write(KeyValue);
			writer.Write(Boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Side = (PlankSide)reader.ReadInt();
            Locked = reader.ReadBool();
            KeyValue = reader.ReadUInt();
			Boat = reader.ReadItem() as NewBaseBoat;

            if (IsOpen)
            {
                _closeTimer = new CloseTimer(this);
                _closeTimer.Start();
            }
        }
        #endregion

        private class CloseTimer : Timer
        {
            private NewPlank _plank;

            public CloseTimer(NewPlank plank)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                _plank = plank;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                _plank.Close();
            }
        }
    }
}