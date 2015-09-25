using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Concurrent;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public enum SpeedCode
    {
        Stop = 0x0,
        One = 0x1,
        Slow = 0x2,
        Medium = 0x3,
        Fast = 0x4,
    }

    public enum TurnCode
    {
        Left = -2,
        Right = 2,
        Around = -4,
    }

    public interface IFacingChange
    {
        void SetFacing(Direction oldFacing, Direction newFacing);
    }

    public interface IShareHue
    {
        bool ShareHue { get; }
    }

    public interface ICrew
    {
        void Delete();
    }

    public abstract class BaseSmoothMulti : BaseMulti, IMount
    {
        #region Statics Fields
		private static List<BaseSmoothMulti> _instances = new  List<BaseSmoothMulti>();
        private static Dictionary<SpeedCode, TimeSpan> timespanDictionary;
        private static readonly Rectangle2D[] _britWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 5120 - 32, 4096 - 32), new Rectangle2D(5136, 2320, 992, 1760) };
        private static readonly Rectangle2D[] _ilshWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 2304 - 32, 1600 - 32) };
        private static readonly Rectangle2D[] _tokunoWrap = new Rectangle2D[] { new Rectangle2D(16, 16, 1448 - 32, 1448 - 32) };
		
        static BaseSmoothMulti()
        {
            timespanDictionary = new Dictionary<SpeedCode, TimeSpan>();
            timespanDictionary.Add(SpeedCode.One, TimeSpan.FromSeconds(0.50));
            timespanDictionary.Add(SpeedCode.Fast, TimeSpan.FromSeconds(0.25));
            timespanDictionary.Add(SpeedCode.Medium, TimeSpan.FromSeconds(0.50));
            timespanDictionary.Add(SpeedCode.Slow, TimeSpan.FromSeconds(1));
			
			//Activate multi mouse movement request in the packet handlers.
			PacketHandlers.RegisterExtended(0x33, true, MultiMouseMovementRequest);
		}		
		
		public static void MultiMouseMovementRequest(NetState state, PacketReader reader)
		{
            Serial playerSerial = reader.ReadInt32();
            Direction movement = (Direction)reader.ReadByte();
            reader.ReadByte(); // movement direction duplicated
            int speed = reader.ReadByte();

            Mobile mob = World.FindMobile(playerSerial);
            if (mob == null || mob.NetState == null || !mob.Mounted)
                return;

            IMount multi = mob.Mount;
            if (!(multi is BaseSmoothMulti))
                return;

            BaseSmoothMulti smooth = (BaseSmoothMulti)multi;
            smooth.OnMousePilotCommand(mob, movement, speed);
		}
		
		public static List<BaseSmoothMulti> Instances { get { return _instances; } }
        #endregion	
		
        private Direction _facing;
        private Direction _moving;
        private SpeedCode _speed;
        private DynamicComponentList _containedObjects;

        private Mobile _pilot;
        private SmoothMultiMountItem _virtualMount;

        private TurnTimer _currentTurnTimer; // timer for changing facing
        private MoveTimer _currentMoveTimer; // timer for movement animation

		private Packet _containerPacket;		

        #region Properties  
        public Mobile Rider { get { return _pilot; } set { _pilot = value; } }

		public SmoothMovement MovementPacket { get; protected set; }
		
		public virtual void OnRiderDamaged(int amount, Mobile from, bool willKill) { }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public TurnTimer CurrentTurnTimer { get { return _currentTurnTimer; } }

		[CommandProperty(AccessLevel.GameMaster)]
        public MoveTimer CurrentMoveTimer { get { return _currentMoveTimer; } }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public SpeedCode Speed { get { return _speed; } set { _speed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing { get { return _facing; } set { SetFacing(value); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Moving { get { return _moving; } set { _moving = value;} }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsMoving { get { return _speed != SpeedCode.Stop; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsDriven { get { return _pilot != null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Pilot { get { return _pilot; } set { SetPilot(value); } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public DynamicComponentList ContainedObjects { get { return _containedObjects; } }		

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set
            {
                if (base.Hue == value)
                    return;

                base.Hue = value;
                _containedObjects.ForEachItem(item =>
                {
                    if (item is IShareHue && ((IShareHue)item).ShareHue)
                        item.Hue = value;
                });
            }
        }	

		public override bool AllowsRelativeDrop
		{
			get { return true; }
		}			
        #endregion

		[Constructable]
        protected BaseSmoothMulti(int itemID)
            : base(itemID)
        {
            _facing = Direction.North;
            _moving = Direction.North;
            _speed = SpeedCode.Stop;
            _containedObjects = new DynamicComponentList();		
			_virtualMount = new SmoothMultiMountItem(this);
            _instances.Add(this);
        }

        public BaseSmoothMulti(Serial serial)
            : base(serial)
        {
        }

        #region Movement
        protected virtual bool BeginMove(Direction dir, SpeedCode speed)
        {
            if (speed == SpeedCode.Stop)
                return false;

            if (_currentMoveTimer != null)
                _currentMoveTimer.Stop();

            _moving = dir;
            _speed = speed;

            _currentMoveTimer = new MoveTimer(this, timespanDictionary[speed], speed);
            _currentMoveTimer.Start();

            return true;
        }

        protected virtual bool Move(Direction dir, SpeedCode speed)
        {
            Map map = Map;

            if (map == null || Deleted)
                return false;

            int rx = 0, ry = 0;
            Movement.Movement.Offset(dir, ref rx, ref ry);

            Point3D newLocation = new Point3D(X + rx, Y + ry, Z);
            if (!CanFit(newLocation, Map, ItemID))
                return false;
			
			int xOffset =  rx;
            int yOffset =  ry;

            int newX = X + xOffset;
            int newY = Y + yOffset;
			
            Rectangle2D[] wrap = GetWrapFor(map);

            for (int i = 0; i < wrap.Length; ++i)
            {
                Rectangle2D rect = wrap[i];

                if (rect.Contains(new Point2D(X, Y)) && !rect.Contains(new Point2D(newX, newY)))
                {
                    if (newX < rect.X)
                        newX = rect.X + rect.Width - 1;
                    else if (newX >= rect.X + rect.Width)
                        newX = rect.X;

                    if (newY < rect.Y)
                        newY = rect.Y + rect.Height - 1;
                    else if (newY >= rect.Y + rect.Height)
                        newY = rect.Y;

                    for (int j = 1; j <= (int)speed; ++j)
                    {
                        if (!CanFit(new Point3D(newX + (j * rx), newY + (j * ry), Z), Map, ItemID))                      
                            return false;                      
                    }

                    xOffset = newX - X;
                    yOffset = newY - Y;
                }
            }			

			Point3D newWrapLocation = new Point3D(X + xOffset, Y + yOffset, Z);
			
			GetMovingEntities();
			if (newLocation != newWrapLocation)
				Location = newWrapLocation;			
			else
				SetLocationOnSmooth(newLocation);
			
            return true;
        }

        protected virtual bool EndMove()
        {
            if (_speed == SpeedCode.Stop)
                return false;

            if (_currentMoveTimer != null)
                _currentMoveTimer.Stop();

            _speed = SpeedCode.Stop;
			
            return true;
        }

        public virtual void SetLocationOnSmooth(Point3D newLocation)
        {
            Point3D oldLocation = Location;
			int rx = newLocation.X - oldLocation.X;
            int ry = newLocation.Y - oldLocation.Y;	
			int rz = newLocation.Z - oldLocation.Z;						

			NoMoveHS = true;
			_containedObjects.ForEachObject(
                item => item.NoMoveHS = true,
                mob => mob.NoMoveHS = true);	

			Location = newLocation;

			_containedObjects.ForEachMobile(
                mob =>	
				{
					NotifyLocationChangeOnSmooth(mob, new Point3D(mob.Location.X - rx, mob.Location.Y - ry, mob.Location.Z - rz));
				});
			
			NoMoveHS = false;
			_containedObjects.ForEachObject(
                item => item.NoMoveHS = false,
                mob => mob.NoMoveHS = false);							

            IPooledEnumerable eable = Map.GetClientsInRange(Location, GetMaxUpdateRange());
            foreach (NetState state in eable)
            {
                Mobile m = state.Mobile;

                if (!m.CanSee(this))
                    continue;

                if (m.InRange(Location, GetUpdateRange(m)))
                {
                    state.Send(GetMovementPacketFor(state));

                    if (!m.InRange(oldLocation, GetUpdateRange(m)))
                        SendInfoTo(state);
                }

                if (Utility.InUpdateRange(Location, m.Location) && !Utility.InUpdateRange(oldLocation, m.Location))
                    SendInfoTo(state);
            }

            eable.Free();
			
			if (MovementPacket != null)
			{
				MovementPacket.Release();
				MovementPacket = null;
			}					
       }
		
		protected Packet GetMovementPacketFor(NetState state)
		{
			if (MovementPacket == null)
			{
				MovementPacket = new SmoothMovement(this, _containedObjects);
				MovementPacket.SetStatic();
			}

			return MovementPacket;
		}		

		public virtual void SetItemIDOnSmooth(Item item, int itemID)
		{
			if (item.ItemID != itemID)
			{
				int oldPileWeight = item.PileWeight;

				item.ItemID = itemID;
				ReleaseWorldPackets();		

				int newPileWeight = item.PileWeight;

				UpdateTotal(item, TotalType.Weight, newPileWeight - oldPileWeight);

				InvalidateProperties();
			}
		}	

		public void NotifyLocationChangeOnSmooth(Mobile mobile, Point3D oldLocation)
		{
			Map map = mobile.Map;
			Point3D newLocation = mobile.Location;

			if (map != null)
			{
				// First, send a remove message to everyone who can no longer see us. (inOldRange && !inNewRange)
				Packet removeThis = null;

				IPooledEnumerable eable = map.GetClientsInRange(oldLocation);

				foreach (NetState ns in eable)
				{
					if (ns != mobile.NetState && !Utility.InUpdateRange(newLocation, ns.Mobile.Location))
					{
						if (removeThis == null)
							removeThis = mobile.RemovePacket;

						ns.Send(removeThis);
					}
				}

				eable.Free();

				NetState ourState = mobile.NetState;

				// Check to see if we are attached to a client
				if (ourState != null)
				{
					eable = mobile.Map.GetObjectsInRange(newLocation, Core.GlobalMaxUpdateRange);

					// We are attached to a client, so it's a bit more complex. We need to send new items and people to ourself, and ourself to other clients
					foreach (object o in eable)
					{
						if (o is Item)
						{		
							Item item = (Item)o;
							
							if (item.NoMoveHS)
								continue;

							int range = item.GetUpdateRange(mobile);
							Point3D loc = item.Location;

							if (/*!Utility.InRange(oldLocation, loc, range) && */Utility.InRange(newLocation, loc, range) && mobile.CanSee(item))
								item.SendInfoTo(ourState);
						}
						else if (o != mobile && o is Mobile)
						{
							Mobile m = (Mobile)o;				

							if (m.NoMoveHS)
								continue;						
							
							if (!Utility.InUpdateRange(newLocation, m.Location))
								continue;

							bool inOldRange = Utility.InUpdateRange(oldLocation, m.Location);

							if (!inOldRange && m.NetState != null && m.CanSee(mobile))
							{
								m.NetState.Send(new MobileIncoming(m, mobile));

								if (mobile.Poison != null)
									m.NetState.Send(new HealthbarPoison(mobile));

								if (mobile.Blessed || mobile.YellowHealthbar)
									m.NetState.Send(new HealthbarYellow(mobile));

								if (mobile.IsDeadBondedPet)
									m.NetState.Send(new BondedStatus(0, mobile.Serial, 1));

								if (ObjectPropertyList.Enabled)
									m.NetState.Send(m.OPLPacket);
							}

							if (!inOldRange && mobile.CanSee(m))
							{
								ourState.Send(new MobileIncoming(mobile, m));

								if (m.Poisoned)
									ourState.Send(new HealthbarPoison(m));

								if (m.Blessed || m.YellowHealthbar)
									ourState.Send(new HealthbarYellow(m));

								if (m.IsDeadBondedPet)
									ourState.Send(new BondedStatus(0, mobile.Serial, 1));

								if (ObjectPropertyList.Enabled)
									ourState.Send(m.OPLPacket);
							}
						}
					}

					eable.Free();
				}
				else
				{
					if (mobile == null)
						return;
					
					eable = mobile.Map.GetClientsInRange(newLocation);

					// We're not attached to a client, so simply send an Incoming
					foreach (NetState ns in eable)
					{
						if (mobile.NoMoveHS)
							continue;
						
						if (Utility.InUpdateRange(oldLocation, ns.Mobile.Location) && ns.Mobile.CanSee(mobile))
						{
							if (ns.StygianAbyss)
							{
								ns.Send(new MobileIncoming(ns.Mobile, mobile));

								if (mobile.Poison != null)
									ns.Send(new HealthbarPoison(mobile));

								if (mobile.Blessed || mobile.YellowHealthbar)
									ns.Send(new HealthbarYellow(mobile));
							}
							else
							{
								ns.Send(new MobileIncomingOld(ns.Mobile, mobile));
							}

							if (mobile.IsDeadBondedPet)
								ns.Send(new BondedStatus(0, mobile.Serial, 1));

							if (ObjectPropertyList.Enabled)
								ns.Send(mobile.OPLPacket);
						}
					}

					eable.Free();
				}
			}
		}		
        #endregion


        #region Turn
        protected bool BeginTurn(TurnCode turnDir)
        {
            return BeginTurn((Direction)(((int)_facing + (int)turnDir) & 0x7));
        }

        protected virtual bool BeginTurn(Direction newDirection)
        {
            if (_currentTurnTimer != null)
                _currentTurnTimer.Stop();

            _currentTurnTimer = new TurnTimer(this, newDirection);
            _currentTurnTimer.Start();

            return true;
        }

        public bool Turn(Direction newDirection)
        {
            if (_currentTurnTimer != null)
            {
                _currentTurnTimer.Stop();
                _currentTurnTimer = null;
            }

            if (_currentMoveTimer != null)
                _currentMoveTimer.Stop();

            Direction oldFacing = _facing;
            if (SetFacing((int)newDirection % 2 != 0 ? newDirection - 1 : newDirection))
            {
                if (_speed != SpeedCode.Stop && _currentMoveTimer != null)
                {
                    _moving = IsDriven ? newDirection : (Direction)((int)_moving + (_facing - oldFacing)) & Server.Direction.Mask;
                    _currentMoveTimer.Start();
                }
                return true;
            }

            if (_speed != SpeedCode.Stop && _currentMoveTimer != null) // if boat can't turn, restart movement if it was on moving
                _currentMoveTimer.Start();

            return false;
        }

        protected abstract int GetMultiId(Direction newFacing);

        public bool SetFacing(Direction facing)
        {
            if (Parent != null || Map == null || _facing == facing)
                return false;

            int? NewFacingItemID = GetMultiId(facing);

            // check if the multi can fit in new Location
            if (!NewFacingItemID.HasValue || !CanFit(Location, Map, NewFacingItemID.Value))
                return false;

            Map.OnLeave(this);

            Direction oldFacing = _facing;
            _facing = facing;

            int xOffset = 0, yOffset = 0;
            Movement.Movement.Offset(facing, ref xOffset, ref yOffset);
            int count = ((int)(_facing - oldFacing) & 0x7) / 2;

			GetMovingEntities();				
							
            UpdateContainedItemsFacing(oldFacing, facing, count);    // update contained items					
            SetItemIDOnSmooth(NewFacingItemID.Value);                // update multi id
            UpdateContainedMobilesFacing(oldFacing, facing, count);  // update contained mobiles
		
            Map.OnEnter(this);
            return true;
        }

        public virtual void SetItemIDOnSmooth(int itemID)
        {
            SetItemIDOnSmooth(this, itemID);

            IPooledEnumerable eable = GetClientsInRange(GetMaxUpdateRange());
            foreach (NetState state in eable)
            {
                Mobile mob = state.Mobile;
                if (mob.CanSee(this) && mob.InRange(Location, GetUpdateRange(mob)))
                    SendInfoTo(state);
            }
            eable.Free();
        }

        protected virtual void UpdateContainedItemsFacing(Direction oldFacing, Direction newFacing, int count)
        {
            _containedObjects.ForEachItem(item =>
            {
                if (item is IFacingChange)
                    ((IFacingChange)item).SetFacing(oldFacing, newFacing);

                item.Location = Rotate(item, count);
            });
        }

        protected virtual void UpdateContainedMobilesFacing(Direction oldFacing, Direction facing, int count)
        {
            _containedObjects.ForEachMobile(mob =>
            {
                mob.SetDirection((facing + (mob.Direction - oldFacing)) & Direction.Mask);
				mob.SetLocation(Rotate(mob, count), true);
            });
        }

        protected Point3D Rotate(IEntity e, int count)
        {
            Point3D toRotate = e.Location;
            int rx = toRotate.X - Location.X;
            int ry = toRotate.Y - Location.Y;

            for (int i = 0; i < count; ++i)
            {
                int temp = rx;
                rx = -ry;
                ry = temp;
            }

            return new Point3D(Location.X + rx, Location.Y + ry, toRotate.Z);
        }
        #endregion


        #region Checks
        public bool CanFit(Point3D loc, Map map, int itemID)
        {
            if (map == null || map == Map.Internal || Deleted)
                return false;

            MultiComponentList newComponents = MultiData.GetComponents(itemID);		
			
            for (int x = 0; x < newComponents.Width; ++x)
            {
                for (int y = 0; y < newComponents.Height; ++y)
                {			
					if (itemID <= 0x20 && itemID >= 0x18)
					{
						if (_facing == Direction.North)
						{
							if ((x >= 0) && (x <= 4)) 
								continue;
							if ((x >= newComponents.Width - 5) && (x < newComponents.Width))
								continue;
						}
						else if (_facing == Direction.South)
						{
							if ((x >= 0) && (x <= 4)) 
								continue;
							if ((x >= newComponents.Width - 5) && (x < newComponents.Width))
								continue;
						}
						else if (_facing == Direction.East)
						{
							if ((y >= 0) && (y <= 4))
								continue;	
							if ((y >= newComponents.Height - 5) && (y < newComponents.Height))
								continue;
						}
						else
						{
							if ((y >= 0) && (y <= 4))
								continue;	
							if ((y >= newComponents.Height - 5) && (y < newComponents.Height))
								continue;
						}						
					}					
				
                    int tx = loc.X + newComponents.Min.X + x;
                    int ty = loc.Y + newComponents.Min.Y + y;					
					
                    if (newComponents.Tiles[x][y].Length == 0 || Contains(tx, ty))
                        continue;

                    bool isWalkable = false;

                    // landTile check
                    LandTile landTile = map.Tiles.GetLandTile(tx, ty);
                    if (landTile.Z == loc.Z && IsEnabledLandID(landTile.ID))
                        isWalkable = true;
						
                    // staticTiles check
                    foreach (StaticTile tile in map.Tiles.GetStaticTiles(tx, ty, true))
                    {
                        if (IsEnabledStaticID(tile.ID) && (tile.Z == loc.Z))
							isWalkable = true;
                        else if (!IsEnabledStaticID(tile.ID) && (tile.Z >= loc.Z))//else if (!isBridgeEnabledTile(tile, loc.Z, maxMultiZ))
                            return false;
                    }

                    if (!isWalkable)
                        return false;
                }
            }

            
            IPooledEnumerable eable = map.GetItemsInBounds(new Rectangle2D(loc.X + newComponents.Min.X, loc.Y + newComponents.Min.Y, newComponents.Width, newComponents.Height));

            foreach (Item item in eable)
            {
                if (item is BaseSmoothMulti || item.ItemID > TileData.MaxItemValue || item.Z < loc.Z || !item.Visible)
                    continue;

                int x = item.X - loc.X + newComponents.Min.X;
                int y = item.Y - loc.Y + newComponents.Min.Y;

                if (x >= 0 && x < newComponents.Width && y >= 0 && y < newComponents.Height && newComponents.Tiles[x][y].Length == 0)
                    continue;
                else if (IsOnBoard(item))
                    continue;

                eable.Free();
                return false;
            }

            eable.Free();

            return true;
        }

        protected abstract bool IsEnabledStaticID(int staticID);

        protected abstract bool IsEnabledLandID(int landID);

        public virtual bool IsOnBoard(Mobile mob)
        {
            return _containedObjects.Contains(mob);
        }

        public virtual bool IsOnBoard(Item item)
        {
            return _containedObjects.Contains(item);
        }

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            return _containedObjects.Contains(x, y);
        }

        #endregion


        #region Mouse Movement
        public bool TakeCommand(Mobile from)
        {
            return SetPilot(from);
        }

        public void LeaveCommand(Mobile from)
        {
            SetPilot(null);
        }

        private bool CanPilot(Mobile from)
        {
			GetMovingEntities();
			
            if (from == null || !from.CheckAlive() || !IsOnBoard(from) || from.Mounted)
                return false;

            return true;
        }

        private bool SetPilot(Mobile from)
        {
            if (from == null) // no new pilot
            {
                _pilot = null;
                _virtualMount.Internalize();
            }
            else
            {
                if (!CanPilot(from))
                    return false;

                _pilot = from;
                from.Direction = _facing;
                from.AddItem(_virtualMount);
            }           
                        
            if (_currentMoveTimer != null) // stop boat if moving before mouse control
                _currentMoveTimer.Stop();

            return true;
        }

        public void OnMousePilotCommand(Mobile from, Direction movementDir, int rawSpeed)
        {
            if (rawSpeed != 0)
            {
                if (!BeginMove(movementDir, ComputeMouseSpeed(rawSpeed)))
                {
                    EndMove();
                    return;
                }

                if (rawSpeed > 1 && ((_facing - movementDir) & 0x7) > 1)
                    BeginTurn(movementDir);
            }
            else
                EndMove();
        }

        private SpeedCode ComputeMouseSpeed(int rawSpeed)
        {
            switch (rawSpeed)
            {
                case 1:
                    return SpeedCode.Slow;
                case 2:
                    return SpeedCode.Fast;
                default:
                    return SpeedCode.Stop;
            }
        }
        #endregion


        #region OnBoard Object Management
        public void Embark(Item item)
        {
            if (item == null || item.Deleted || item == this)
                return;

			if (_containedObjects != null)
			{
				if (_containedObjects.Contains(item))
					return;
				
				_containedObjects.Add(item);
			}
			
            ReleaseWorldPackets();
            //ReleaseContainerPackets();			
        }

        public void Embark(Mobile mob)
        {
            if (mob == null || mob.Deleted)
                return;
			
			if (_containedObjects != null)
			{
				if (_containedObjects.Contains(mob))
					return;
				
				_containedObjects.Add(mob);
			}
			
            ReleaseWorldPackets();
            //ReleaseContainerPackets();			
        }

        public void Disembark(Item item)
        {
            if (_containedObjects != null && item != null)
                _containedObjects.Remove(item);			

			ReleaseWorldPackets();
			//ReleaseContainerPackets();	
        }

        public void Disembark(Mobile mob)
        {
            if (_containedObjects != null && mob != null)
                _containedObjects.Remove(mob);
			
            ReleaseWorldPackets();
			//ReleaseContainerPackets();			
        }
		
		public virtual void GetMovingEntities()
		{
			//Include new moving entities
			Map map = Map;
			
			if (map == null || map == Map.Internal)
				return;			
			
			MultiComponentList mcl = Components;
			
			foreach (object o in map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height)))
			{
				if (o is Item)
				{
					// Check to avoid stealing other ships' components
					if (o is BaseShipItem && ((BaseShipItem)o).Ship != this)
						continue;
					
					Item item = (Item)o;
					if (Contains(item) && item.Visible && item.Z >= Z)
					{
						Embark(item);
					}
				}
				else if (o is Mobile)
				{
					Mobile m = (Mobile)o;
					if (Contains(m) && m.Z > Z)
						Embark(m);
				}
			}
			
			//Exclude non moving entities
			List<IEntity> list = new List<IEntity>();
			list = _containedObjects.ToList();
			
			foreach (object o in list)
			{			
				if (o is Item)
				{
					Item item = (Item)o;
					if (!Contains(item))
						Disembark(item);
				}
				else if (o is Mobile)
				{
					Mobile m = (Mobile)o;
					if (!Contains(m))
						Disembark(m);
				}
			}		
		}		
        #endregion	
		
		#region wrapping
		public static bool IsValidLocation(Point3D p, Map map)
        {
            Rectangle2D[] wrap = GetWrapFor(map);

            for (int i = 0; i < wrap.Length; ++i)
            {
                if (wrap[i].Contains(p))
                    return true;
            }

            return false;
        }
		
        public static Rectangle2D[] GetWrapFor(Map m)
        {
            if (m == Map.Ilshenar)
                return _ilshWrap;
            else if (m == Map.Tokuno)
                return _tokunoWrap;
            else
                return _britWrap;
        }		
		#endregion
		
        public override void OnMapChange()
        {	
            _containedObjects.ForEachObject(
                item => item.Map = Map,
                mob => mob.Map = Map);
        }	

        public override void OnLocationChange(Point3D oldLoc)
        {	
            int rx = X - oldLoc.X;
            int ry = Y - oldLoc.Y;
            int rz = Z - oldLoc.Z;
			
			// Safety check to avoid stack overflow
			_containedObjects.Remove(this);			
			
            _containedObjects.ForEachObject(
                item => item.Location = new Point3D(item.X + rx, item.Y + ry, item.Z + rz),
                mob => mob.Location = new Point3D(mob.X + rx, mob.Y + ry, mob.Z + rz));
        }			

        protected override Packet GetWorldPacketFor(NetState state)
        {		
			if (state != null)
			{		
				_containerPacket = new ContainerMultiList(state.Mobile, this);
				//_containerPacket.SetStatic();
				return _containerPacket;
			}
			else
				return base.GetWorldPacketFor(state);
        }			

        public override void OnAfterDelete()
        {
            foreach (IEntity obj in _containedObjects.ToList()) // toList necessary for enumeration modification
            {
                if (obj is Item)
                    ((Item)obj).Delete();
                else if (obj is ICrew)
                    ((ICrew)obj).Delete();
            }

            if (_currentMoveTimer != null)
                _currentMoveTimer.Stop();

            if (_currentTurnTimer != null)
                _currentTurnTimer.Stop();

            _instances.Remove(this);
        }

		public void ReleaseContainerPackets()
		{
            if (_containerPacket != null)
            {
                _containerPacket.Release();
                _containerPacket = null;
            }
		}

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((byte)_facing);
            writer.Write((Item)_virtualMount);
            _containedObjects.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _facing = (Direction)reader.ReadByte();
            _virtualMount = reader.ReadItem() as SmoothMultiMountItem;
            _moving = _facing;
            _speed = SpeedCode.Stop;
            _containedObjects = new DynamicComponentList(reader);

            if (_virtualMount == null)
                Delete();
        }
        #endregion

        public class TurnTimer : Timer
        {
            private BaseSmoothMulti _multi;
            private Direction _turnDir;

            public TurnTimer(BaseSmoothMulti multi, Direction turnDir)
                : base(TimeSpan.FromSeconds(0.5))
            {
                _multi = multi;
                _turnDir = turnDir;

                Priority = TimerPriority.TenMS;
            }

            protected override void OnTick()
            {
                if (!_multi.Deleted)
                    _multi.Turn(_turnDir);
            }
        }

        public class MoveTimer : Timer
        {
            private BaseSmoothMulti _multi;

            public MoveTimer(BaseSmoothMulti multi, TimeSpan interval, SpeedCode speed)
                : base(TimeSpan.Zero, interval, speed == SpeedCode.One ? 1 : 0)
            {
                _multi = multi;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (!_multi.Move(_multi.Moving, _multi.Speed) || _multi.Speed == SpeedCode.One)
                    _multi.EndMove();
            }
        }

        private class SmoothMultiMountItem : Item, IMountItem
        {
            private BaseSmoothMulti _mount;

            public IMount Mount { get { return _mount; } }

            public SmoothMultiMountItem(BaseSmoothMulti mount)
                : base(0x3E96)
            {
                Layer = Layer.Mount;

                Movable = false;
                _mount = mount;
            }

            public SmoothMultiMountItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write((Item)_mount);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                _mount = reader.ReadItem() as BaseSmoothMulti;

                if (_mount == null)
                    Delete();
                else
                    Internalize();
            }
        }		
    }

    public class DynamicComponentList
    {
        private Dictionary<Serial, IEntity> _internalList;
        private object _locker;

        public int Count { get { return _internalList.Values.Count; } }
        
        public DynamicComponentList()
        {
            _internalList = new Dictionary<Serial, IEntity>(new InternalComparer());
            _locker = ((ICollection)_internalList).SyncRoot;
        }

        public DynamicComponentList(GenericReader reader)
        {
            Deserialize(reader);
        }

        public void Add(Item i)
        {
            lock (_locker)
            {
                if (!_internalList.ContainsKey(i.Serial))
                    _internalList.Add(i.Serial, i);
            }
        }

        public void Add(Mobile m)
        {
            lock (_locker)
            {
                if (!_internalList.ContainsKey(m.Serial))
                    _internalList.Add(m.Serial, m);
            }
        }

        public void Remove(Item i)
        {
            lock (_locker)
            {
                _internalList.Remove(i.Serial);
            }
        }

        public void Remove(Mobile m)
        {
            lock (_locker)
            {
                _internalList.Remove(m.Serial);
            }
        }

        public bool Contains(Item item)
        {
            return _internalList.ContainsKey(item.Serial);
        }

        public bool Contains(Mobile mob)
        {
            return _internalList.ContainsKey(mob.Serial);
        }

        public bool Contains(int x, int y)
        {
            return _internalList.Values.OfType<Item>().FirstOrDefault(item => item.X == x && item.Y == y) != null;
        }

        public void ForEachItem(Action<Item> itemCmd)
        {
            lock (_locker)
            {
                foreach (IEntity obj in _internalList.Values)
                {
                    if (obj is Item)
                        itemCmd.Invoke((Item)obj);
                }
            }
        }

        public void ForEachMobile(Action<Mobile> mobCmd)
        {
            lock (_locker)
            {
                foreach (IEntity obj in _internalList.Values)
                {
                    if (obj is Mobile)
                        mobCmd.Invoke((Mobile)obj);
                }
            }
        }

        public void ForEachObject(Action<Item> itemCmd, Action<Mobile> mobCmd)
        {
            lock (_locker)
            {
                foreach (IEntity obj in _internalList.Values)
                {
                    if (obj is Item)
                        itemCmd.Invoke((Item)obj);
                    else
                        mobCmd.Invoke((Mobile)obj);
                }
            }
        }

        public List<IEntity> ToList()
        {
            return new List<IEntity>(_internalList.Values);
        }

        #region Serialization
        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            int itemSize = _internalList.Values.Count(obj => obj is Item);
            writer.Write((int)itemSize);

            foreach (Item obj in _internalList.Values.OfType<Item>())
                writer.Write((Item)obj);

            writer.Write((int)_internalList.Values.Count - itemSize);

            foreach (Mobile mob in _internalList.Values.OfType<Mobile>())
                writer.Write((Mobile)mob);
        }

        public void Deserialize(GenericReader reader)
        {
            reader.ReadInt();

            int itemSize = reader.ReadInt();
            _internalList = new Dictionary<Serial, IEntity>(new InternalComparer());
            _locker = ((ICollection)_internalList).SyncRoot;

            Item it;
            for (int i = 0; i < itemSize; i++)
            {
                it = reader.ReadItem();
                _internalList.Add(it.Serial, it);
            }

            int mobSize = reader.ReadInt();

            Mobile mob;
            for (int i = 0; i < mobSize; i++)
            {
                mob = reader.ReadMobile();
                _internalList.Add(mob.Serial, mob);
            }
        }
        #endregion

        private class InternalComparer : IEqualityComparer<Serial>
        {
            public bool Equals(Serial a, Serial b) { return a == b; }

            public int GetHashCode(Serial e) { return e; }
        }
    }
}
