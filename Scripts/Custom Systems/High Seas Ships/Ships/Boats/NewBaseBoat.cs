using System;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Movement;
using Server.Network;

namespace Server.Multis
{
    public abstract class NewBaseBoat : BaseShip
    {
		private bool _anchored;
        protected abstract int NorthID { get; }
        private static readonly TimeSpan SlowInterval = TimeSpan.FromSeconds(1.00);
        private static readonly TimeSpan FastInterval = TimeSpan.FromSeconds(0.25);		

        #region Properties
		[CommandProperty(AccessLevel.GameMaster)]
        public CourseMoveTimer CurrentCourseMoveTimer { get; private set; }			
		
        [CommandProperty(AccessLevel.GameMaster)]
        public NewTillerMan TillerMan { get; set; }	
		
        [CommandProperty(AccessLevel.GameMaster)]
        public NewPlank PPlank { get; protected set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public NewPlank SPlank { get; protected set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public NewHold NewHold { get; protected set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual BaseDockedNewBoat DockedNewBoat { get { return null; } }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Anchored
        {
            get { return _anchored; }
            set
            {
                if (_anchored == value)
                    return;

                if (IsMovingShip)
                    EndMove();

                _anchored = value;
            }
        }		
        #endregion

        protected NewBaseBoat(int ItemID, ushort maxDurability, Point3D tillermanOffset, Point3D pPlankOffset, Point3D sPlankOffset, Point3D holdOffset)
            : base(ItemID)
        {
			_anchored = false;
            TillerMan = new NewTillerMan(this, tillermanOffset);
            PPlank = new NewPlank(this, pPlankOffset, PlankSide.Port, 0);
            SPlank = new NewPlank(this, sPlankOffset, PlankSide.Starboard, 0);
            NewHold = new NewHold(this, holdOffset);
        }

        public NewBaseBoat(Serial serial)
            : base(serial)
        { }

        public bool LowerAnchor(bool message)
		{			
			if (CheckDecay())
				return false;
		   
			if(Anchored)
			{
				if (message && TillerMan != null)
					TillerMan.Say(501445); // Ar, the anchor was already dropped sir.					
					
				return false;
			}
	   
			EndMove();
	   
			Anchored = true;

			if (message && TillerMan != null)
				TillerMan.Say(501444); // Ar, anchor dropped sir.
	   
			return true;
		}
     
		public bool RaiseAnchor(bool message)
		{	
			if (CheckDecay())
				return false;
	   
			if (!Anchored)
			{
				if (message && TillerMan != null)
					TillerMan.Say(501447); // Ar, the anchor has not been dropped sir.
 
				return false;
			}
	   
			Anchored = false;
	   
			if (message && TillerMan != null)
				TillerMan.Say(501446); // Ar, anchor raised sir.
	   
			return true;
		}

        public override void SetName(SpeechEventArgs e)
        {
            if (e.Mobile.AccessLevel < AccessLevel.GameMaster && e.Mobile != Owner)
            {
                if (TillerMan != null)
                    TillerMan.Say(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!e.Mobile.Alive)
            {
                if (TillerMan != null)
                    TillerMan.Say(502582); // You appear to be dead.

                return;
            }

			base.SetName(e);
        }
		
        public override void Rename(string newName)
        {
            if (CheckDecay())
                return;

            if (newName != null && newName.Length > 40)
                newName = newName.Substring(0, 40);			
			
            if (ShipName == newName)
            {
                if (TillerMan != null)
                    TillerMan.Say(502531); // Yes, sir.

                return;
            }

            ShipName = newName;

            if (TillerMan != null && ShipName != null)
                TillerMan.Say(1042885, ShipName); // This ship is now called the ~1_NEW_SHIP_NAME~.
            else if (TillerMan != null)
                TillerMan.Say(502534); // This ship now has no name.
        }	

        public override void RemoveName(Mobile m)
        {
            if (CheckDecay())
                return;

            if (m.AccessLevel < AccessLevel.GameMaster && m != Owner)
            {
                if (TillerMan != null)
                    TillerMan.Say(1042880); // Arr! Only the owner of the ship may change its name!
					
                return;
            }
            else if (!m.Alive)
            {
                if (TillerMan != null)
                    TillerMan.Say(502582); // You appear to be dead.
					
                return;
            }

            if (ShipName == null)
            {
                if (TillerMan != null)
                    TillerMan.Say(502526); // Ar, this ship has no name.

                return;
            }

            ShipName = null;

            if (TillerMan != null)
                TillerMan.Say(502534); // This ship now has no name.
        }	

        public override void GiveName(Mobile m)
        {
            if (CheckDecay())
                return;

            if (ShipName == null)
			{
				if (TillerMan != null)
					TillerMan.Say(502526); // Ar, this ship has no name.
			}
            else
			{
				if (TillerMan != null)
					TillerMan.Say(1042881, ShipName); // This is the ~1_BOAT_NAME~.
			}
        }

        public override void BeginRename(Mobile from)
        {
            if (from.AccessLevel < AccessLevel.GameMaster && from != Owner)
            {
                if (TillerMan != null)
                    TillerMan.Say(Utility.Random(1042876, 4)); // Arr, don't do that! | Arr, leave me alone! | Arr, watch what thour'rt doing, matey! | Arr! Do that again and I’ll throw ye overhead!

                return;
            }

            if (TillerMan != null)
                TillerMan.Say(502580); // What dost thou wish to name thy ship?

            base.BeginRename(from);
        }

		public override void EndRename(Mobile from, string newName)
		{
			if (from.AccessLevel < AccessLevel.GameMaster && from != Owner)
			{
				if (TillerMan != null)
					TillerMan.Say(1042880); // Arr! Only the owner of the ship may change its name!

				return;
			}
			else if (!from.Alive)
			{
				if (TillerMan != null)
					TillerMan.Say(502582); // You appear to be dead.

				return;
			}

			base.EndRename(from, newName);
		}		
		
        protected override bool BeginMove(Direction dir, SpeedCode speed)
        {
            if (Anchored)
                return false;
				
            if (base.BeginMove(dir, speed))
                return true;
			
			return false;
        }
		
		protected override bool Move(Direction dir, SpeedCode speed)			
		{ 
			if (base.Move(dir, speed))
				return true;
			else
			{
				if (TillerMan != null)
					TillerMan.Say(501424); //Ar, we've stopped sir.
				
				return false;
			}
		}		
		
        protected override bool EndMove()
        {
            if (!base.EndMove())
                return false;
			
			if (TillerMan != null)
			{
				TillerMan.Say(501424); //Ar, we've stopped sir.
			}
			
            return true;
        }		

        protected override bool BeginTurn(Direction newDirection)
        {
            if (Anchored)
                return false;

            return base.BeginTurn(newDirection);
        }		
		
        protected override int GetMultiId(Direction newFacing)
        {
            return NorthID + ((int)newFacing / 2);
        }

        protected override bool IsEnabledLandID(int landID)
        {
            if (landID > 167 && landID < 172)
                return true;

            if (landID == 310 || landID == 311)
                return true;

            return false;
        }
		
		public override void OnSpeech(SpeechEventArgs e)
        {
            if (IsDriven)
                return;

            Mobile from = e.Mobile;
			
			GetMovingEntities();

            if (IsOnBoard(from))
            {
				if (from.Mounted)
				{
					return;
				}
				
                for (int i = 0; i < e.Keywords.Length; ++i)
                {
                    int keyword = e.Keywords[i];

                    if (keyword >= 0x42 && keyword <= 0x6B)
                    {
                        switch (keyword)
                        {
                            //case 0x42: SetName(e); break;
                            //case 0x43: RemoveName(e.Mobile); break;
                            //case 0x44: GiveName(e.Mobile); break;
                            case 0x45: BeginMove(ComputeDirection(BoatDirectionCommand.Forward), SpeedCode.Fast); break;
                            case 0x46: BeginMove(ComputeDirection(BoatDirectionCommand.Backward), SpeedCode.Fast); break;
                            case 0x47: BeginMove(ComputeDirection(BoatDirectionCommand.Left), SpeedCode.Fast); break;
                            case 0x48: BeginMove(ComputeDirection(BoatDirectionCommand.Right), SpeedCode.Fast); break;
                            case 0x4B: BeginMove(ComputeDirection(BoatDirectionCommand.ForwardLeft), SpeedCode.Fast); break;
                            case 0x4C: BeginMove(ComputeDirection(BoatDirectionCommand.ForwardRight), SpeedCode.Fast); break;
                            case 0x4D: BeginMove(ComputeDirection(BoatDirectionCommand.BackwardLeft), SpeedCode.Fast); break;
                            case 0x4E: BeginMove(ComputeDirection(BoatDirectionCommand.BackwardRight), SpeedCode.Fast); break;
                            case 0x4F: EndMove(); break;
                            case 0x50: BeginMove(ComputeDirection(BoatDirectionCommand.Left), SpeedCode.Slow); break;
                            case 0x51: BeginMove(ComputeDirection(BoatDirectionCommand.Right), SpeedCode.Slow); break;
                            case 0x52: BeginMove(ComputeDirection(BoatDirectionCommand.Forward), SpeedCode.Slow); break;
                            case 0x53: BeginMove(ComputeDirection(BoatDirectionCommand.Backward), SpeedCode.Slow); break;
                            case 0x54: BeginMove(ComputeDirection(BoatDirectionCommand.ForwardLeft), SpeedCode.Slow); break;
                            case 0x55: BeginMove(ComputeDirection(BoatDirectionCommand.ForwardRight), SpeedCode.Slow); break;
                            case 0x56: BeginMove(ComputeDirection(BoatDirectionCommand.BackwardRight), SpeedCode.Slow); break;
                            case 0x57: BeginMove(ComputeDirection(BoatDirectionCommand.BackwardLeft), SpeedCode.Slow); break;
                            case 0x58: BeginMove(ComputeDirection(BoatDirectionCommand.Left), SpeedCode.One); break;
                            case 0x59: BeginMove(ComputeDirection(BoatDirectionCommand.Right), SpeedCode.One); break;
                            case 0x5A: BeginMove(ComputeDirection(BoatDirectionCommand.Forward), SpeedCode.One); break;
                            case 0x5B: BeginMove(ComputeDirection(BoatDirectionCommand.Backward), SpeedCode.One); break;
                            case 0x5C: BeginMove(ComputeDirection(BoatDirectionCommand.ForwardLeft), SpeedCode.One); break;
                            case 0x5D: BeginMove(ComputeDirection(BoatDirectionCommand.ForwardRight), SpeedCode.One); break;
                            case 0x5E: BeginMove(ComputeDirection(BoatDirectionCommand.BackwardRight), SpeedCode.One); break;
                            case 0x5F: BeginMove(ComputeDirection(BoatDirectionCommand.BackwardLeft), SpeedCode.One); break;
                            case 0x49:
                            case 0x65: BeginTurn(TurnCode.Right); break; // turn right
                            case 0x4A:
                            case 0x66: BeginTurn(TurnCode.Left); break; // turn left
                            case 0x67: BeginTurn(TurnCode.Around); break; // turn around, come about
                            case 0x68: BeginMove(ComputeDirection(BoatDirectionCommand.Forward), SpeedCode.Fast); break;
                            case 0x69: EndMove(); break;
                            case 0x6A: LowerAnchor(true); break;
                            case 0x6B: RaiseAnchor(true); break;
                            case 0x60: GiveNavPoint(); break; // nav
                            case 0x61: NextNavPoint = 0; StartCourse(false, true); break; // start
                            case 0x62: StartCourse(false, true); break; // continue
                            case 0x63: StartCourse(e.Speech, false, true); break; // goto*
                            case 0x64: StartCourse(e.Speech, true, true); break; // single*
                        }

                        break;
                    }
                }
            }
        }		

        protected override bool IsEnabledStaticID(int staticID)
        {
            if (staticID > 0x1795 && staticID < 0x17B3)
                return true;

            return false;
        }
		
		public uint CreateKeys(Mobile  m)
		{
			uint value = Key.RandomValue();

			Key packKey = new Key(KeyType.Gold, value, this);
			Key bankKey = new Key(KeyType.Gold, value, this);
			
			packKey.MaxRange = 10;
			bankKey.MaxRange = 10;

			packKey.Name = "a ship key";
			bankKey.Name = "a ship key";

			if (m != null)
			{				
				BankBox box = m.BankBox;

				if (!box.TryDropItem(m, bankKey, false))
					bankKey.Delete();
				else
					m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502484); // A ship's key is now in my safety deposit box.

				if ( m.AddToBackpack( packKey ) )
					m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502485); // A ship's key is now in my backpack.
				else
					m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502483); // A ship's key is now at my feet.
			}
			
			return value;
		}	
		
        public override bool CheckDecay()
        {
            if (Decaying)
                return true;

            if (!IsMovingShip && DateTime.Now >= TimeOfDecay)
            {
                new DecayTimer(this).Start();
                Decaying = true;
                return true;
            }
			
			if (TimeSpan.FromDays(0.0) <= (TimeOfDecay - DateTime.Now) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(1.0))
			{
				Phase = DecayPhase.Collapsing;
			}
			else if (TimeSpan.FromDays(1.0) <= (TimeOfDecay - DateTime.Now) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(4.0))
			{
				Phase = DecayPhase.GreatlyWorn;
			}
			else if (TimeSpan.FromDays(4.0) <= (TimeOfDecay - DateTime.Now) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(7.0))
			{
				Phase = DecayPhase.FairlyWorn;
			}
			else if (TimeSpan.FromDays(7.0) <= (TimeOfDecay - DateTime.Now) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(10.0))
			{
				Phase = DecayPhase.SomewhatWorn;
			}
			else if (TimeSpan.FromDays(10.0) <= (TimeOfDecay - DateTime.Now) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(12.5))
			{
				Phase = DecayPhase.SlightlyWorn;
			}
			else if (TimeSpan.FromDays(12.5) <= (TimeOfDecay - DateTime.Now) && (TimeOfDecay - DateTime.Now) <= TimeSpan.FromDays(13.0))
			{
				Phase = DecayPhase.New;
			}
			
            return false;
        }		

		#region DryDock
        public enum DryDockResult { Valid, Dead, NoKey, NotAnchored, Mobiles, Items, Hold, Decaying }

        public DryDockResult CheckDryDock(Mobile from)
        {
            if (CheckDecay())
                return DryDockResult.Decaying;

            if (!from.Alive)
                return DryDockResult.Dead;

            Container pack = from.Backpack;
            if ((SPlank == null || !Key.ContainsKey(pack, SPlank.KeyValue)) && (PPlank == null || !Key.ContainsKey(pack, PPlank.KeyValue)))
                return DryDockResult.NoKey;

            if (!Anchored)
                return DryDockResult.NotAnchored;

            if (NewHold != null && NewHold.Items.Count > 0)
                return DryDockResult.Hold;

            Map map = Map;

            if (map == null || map == Map.Internal)
                return DryDockResult.Items;

            MultiComponentList mcl = Components;

            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o == this || o is NewPlank || o is NewTillerMan || o is NewHold )
                    continue;

                if (o is Item && Contains((Item)o))
                {
                    eable.Free();
                    return DryDockResult.Items;
                }
                else if (o is Mobile && Contains((Mobile)o))
                {
                    eable.Free();
                    return DryDockResult.Mobiles;
                }
            }

            eable.Free();
            return DryDockResult.Valid;
        }
		
        public void BeginDryDock(Mobile from)
        {
            if (CheckDecay())
                return;

            DryDockResult result = CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!
            else if (result == DryDockResult.Valid)
                from.SendGump(new NewBoatConfirmDryDockGump(from, this));
        }

        public void EndDryDock(Mobile from)
        {
            if (Deleted || CheckDecay())
                return;

            DryDockResult result = CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!

            if (result != DryDockResult.Valid)
                return;

            BaseDockedNewBoat boat = DockedNewBoat;

            if (boat == null)
                return;

            RemoveKeys(from);

            from.AddToBackpack(boat);

            Delete();			
        }	
		
        public void RemoveKeys(Mobile m)
        {
            uint keyValue = 0;

            if (PPlank != null)
                keyValue = PPlank.KeyValue;

            if (keyValue == 0 && SPlank != null)
                keyValue = SPlank.KeyValue;

            Key.RemoveKeys(m, keyValue);
        }	
		#endregion
		
		#region map navigation		
		public void AssociateMap(MapItem map)
		{
			if (CheckDecay())
				return;

			if (map is BlankMap)
			{
				if (TillerMan != null)
					TillerMan.Say(502575); // Ar, that is not a map, tis but a blank piece of paper!
			}
			else if (map.Pins.Count == 0)
			{
				if (TillerMan != null)
					TillerMan.Say(502576); // Arrrr, this map has no course on it!
			}
			else
			{
				StopMove(false);
				MapItem = map;
				NextNavPoint = -1;

				if (TillerMan != null)
					TillerMan.Say(502577); // A map!
			}
		}
		
		public bool StopMove(bool message)
		{
			if (CheckDecay())
				return false;

			if (CurrentMoveTimer == null)
			{
				if (message && TillerMan != null)
					TillerMan.Say(501443); // Er, the ship is not moving sir.

				return false;
			}

			Moving = Direction.North;
			
			if (CurrentMoveTimer != null)
				CurrentMoveTimer.Stop();

			if (message && TillerMan != null)
				TillerMan.Say(501429); // Aye aye sir.

			return true;
		}		
		
        public void GiveNavPoint()
        {
            if (TillerMan == null || CheckDecay())
                return;

            if (NextNavPoint < 0)
			{					
				if (TillerMan != null)
					TillerMan.Say(1042882); // I have no current nav point.
			}	
            else
			{
				if (TillerMan != null)
					TillerMan.Say(1042883, (NextNavPoint + 1).ToString()); // My current destination navpoint is nav ~1_NAV_POINT_NUM~.
			}
        }		

        public Direction GetMovementFor(int x, int y, out int maxSpeed)
        {
            int dx = x - X;
            int dy = y - Y;

            int adx = Math.Abs(dx);
            int ady = Math.Abs(dy);

            Direction dir = Utility.GetDirection(this, new Point2D(x, y));
            int iDir = (int)dir;

            // Compute the maximum distance we can travel without going too far away
            if (iDir % 2 == 0) // North, East, South and West
                maxSpeed = Math.Abs(adx - ady);
			else // Right, Down, Left and Up
                maxSpeed = Math.Min(adx, ady);

			return dir;
            //return (Direction)((iDir - (int)Facing) & 0x7);
        }
	
        public bool DoMove()
        {
			Direction dir;
			SpeedCode speed;

            if (Order == BoatOrder.Move)
            {
				dir = Moving;
                speed = Speed;
            }			
            else if (MapItem == null || MapItem.Deleted)
            {
                if (TillerMan != null)
                    TillerMan.Say(502513); // I have seen no map, sir.						
					
                return false;
            }
            else if (Map != MapItem.Map || !Contains(MapItem.GetWorldLocation()))
            {					
                if (TillerMan != null)
                    TillerMan.Say(502514); // The map is too far away from me, sir.	
					
                return false;
            }
            else if ((Map != Map.Trammel && Map != Map.Felucca) || NextNavPoint < 0 || NextNavPoint >= MapItem.Pins.Count)
            {	
                if (TillerMan != null)
                    TillerMan.Say(1042551); // I don't see that navpoint, sir.
					
                return false;
            }
            else
            {
                Point2D dest = (Point2D)MapItem.Pins[NextNavPoint];

                int x, y;
                MapItem.ConvertToWorld(dest.X, dest.Y, out x, out y);

                int maxSpeed;
				dir = GetMovementFor(x, y, out maxSpeed);
				
                if (maxSpeed == 0)
                {
                    if (Order == BoatOrder.Single && TillerMan != null)
                        TillerMan.Say(1042874, (NextNavPoint + 1).ToString()); // We have arrived at nav point ~1_POINT_NUM~ , sir.						
						
                    if (NextNavPoint + 1 < MapItem.Pins.Count)
                    {
                        NextNavPoint++;

                        if (Order == BoatOrder.Course)
                        {
                            if (TillerMan != null)
                                TillerMan.Say(1042875, (NextNavPoint + 1).ToString()); // Heading to nav point ~1_POINT_NUM~, sir.								
								
                            return true;
                        }

                        return false;
                    }
                    else
                    {
                        this.NextNavPoint = -1;

                        if (Order == BoatOrder.Course && TillerMan != null)
                            TillerMan.Say(502515); // The course is completed, sir.
							
                        return false;
                    }
                }

				speed = (SpeedCode)Math.Min((int)SpeedCode.Fast, maxSpeed);							
            }
			
			Turn(dir);
			
			return BeginCourse(dir, speed);
        }			
		
        protected bool BeginCourse(Direction dir, SpeedCode speed)
        {		
            if (Anchored)
                return false;	
			
			Order = BoatOrder.Course;
			
			if (speed == SpeedCode.One)	
				return true;

            if (BeginMove(dir, speed))
                return true;

            return false;
        }		

        public bool StartCourse(string navPoint, bool single, bool message)
        {
            int number = -1;

            int start = -1;
            for (int i = 0; i < navPoint.Length; i++)
            {
                if (Char.IsDigit(navPoint[i]))
                {
                    start = i;
                    break;
                }
            }

            if (start != -1)
            {
                string sNumber = navPoint.Substring(start);

                if (!int.TryParse(sNumber, out number))
                    number = -1;

                if (number != -1)
                {
                    number--;

                    if (MapItem == null || number < 0 || number >= MapItem.Pins.Count)
                    {
                        number = -1;
                    }
                }
            }

            if (number == -1)
            {
                if (message && TillerMan != null)
                    TillerMan.Say(1042551); // I don't see that navpoint, sir.

                return false;
            }

            NextNavPoint = number;
            return StartCourse(single, message);
        }

        public bool StartCourse(bool single, bool message)
        {
            if (Anchored)
            {					
                if (message && TillerMan != null)
                    TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }			
			
            if (CheckDecay())
                return false;

			if (MapItem == null || MapItem.Deleted)
            {	
                if (message && TillerMan != null)
                    TillerMan.Say(502513); // I have seen no map, sir.

                return false;
            }
            else if (Map != MapItem.Map || !Contains(MapItem.GetWorldLocation()))
            {
                if (message && TillerMan != null)
                    TillerMan.Say(502514); // The map is too far away from me, sir.

                return false;
            }
            else if ((Map != Map.Trammel && Map != Map.Felucca) || NextNavPoint < 0 || NextNavPoint >= MapItem.Pins.Count)
            {	
                if (message && TillerMan != null)
                    TillerMan.Say(1042551); // I don't see that navpoint, sir.

                return false;
            }

            Order = single ? BoatOrder.Single : BoatOrder.Course;

            if (CurrentMoveTimer != null)
                CurrentMoveTimer.Stop();			
			
			CurrentCourseMoveTimer = new CourseMoveTimer(this, FastInterval, SpeedCode.Fast, false);
			CurrentCourseMoveTimer.Start();
	
            if (message && TillerMan != null)
                TillerMan.Say(501429); // Aye aye sir.

            return true;
        }	
		
        public bool CourseTurn(int offset, bool message)
		{
            if (CurrentTurnTimer != null)
                CurrentTurnTimer.Stop();           

            if (CheckDecay())
                return false;

            if (Anchored)
            {
                if (message)
                    TillerMan.Say(501419); // Ar, the anchor is down sir!

                return false;
            }
            else if (SetFacing((Direction)(((int)Facing + offset) & 0x7)))		
			{
                return true;
            }
            else
            {
                if (message)
                    TillerMan.Say(501423); // Ar, can't turn sir.

                return false;
            }
        }		
		
        public bool EndCourse()
        {
            if (CheckDecay())
                return false;

            if (CurrentCourseMoveTimer == null)        								
                return false;
           
            CurrentCourseMoveTimer.Stop();
            CurrentCourseMoveTimer = null;
			
			EndMove();

            return true;
        }		

		public class CourseMoveTimer : Timer
		{
			private readonly NewBaseBoat _boat;
			
			public CourseMoveTimer(NewBaseBoat boat, TimeSpan interval, SpeedCode speed, bool single)
				: base(interval, interval, single ? 1 : 0)
			{
	            _boat = boat;
                Priority = TimerPriority.TwentyFiveMS;			
			}
			
            protected override void OnTick()
            {
                if (!_boat.DoMove() || _boat.Speed == SpeedCode.One)				
                    _boat.EndCourse();				
            }			
		}
		#endregion		

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch (version)
			{
				case 1:
				{				
					Anchored = reader.ReadBool();           
					TillerMan = reader.ReadItem() as NewTillerMan;
					PPlank = reader.ReadItem() as NewPlank;
					SPlank = reader.ReadItem() as NewPlank;
					NewHold = reader.ReadItem() as NewHold;
					
					break;
				}
			}			
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

			writer.Write((bool)Anchored);
            writer.Write((Item)TillerMan);
            writer.Write((Item)PPlank);
            writer.Write((Item)SPlank);
            writer.Write((Item)NewHold);
        }
        #endregion
		
		
        public class DecayTimer : Timer
        {
            private NewBaseBoat _boat;
            private int _count;

            public DecayTimer(NewBaseBoat boat)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(5.0))
            {
                _boat = boat;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (_count == 5)
                {
                    _boat.Delete();
                    Stop();
                }
                else
                {
                    _boat.Location = new Point3D(_boat.X, _boat.Y, _boat.Z - 1);

                    if (_boat.TillerMan != null)
                        _boat.TillerMan.Say(1007168 + _count);

                    ++_count;
                }
            }
        }		
    }
}