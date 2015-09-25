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
    public enum GalleonStatus
    {
        Full = 2,
        Half = 1,
        Low = 0
    }
	
	public enum GalleonCondition
	{
		Pristine = 4,
		SlightlyDamaged = 3,
		ModeratelyDamaged = 2,
		HeavilyDamaged = 1,
		ExtremelyDamaged = 0
	}		

    public abstract class BaseGalleon : BaseShip
    {
        private static readonly TimeSpan SlowInterval = TimeSpan.FromSeconds(1.00);
        private static readonly TimeSpan FastInterval = TimeSpan.FromSeconds(0.25);
		
        protected abstract int[,] itemIDMods { get; }
        protected abstract int[] multiIDs { get; }	
		
		private ushort _durability;

        #region Properties
		[CommandProperty(AccessLevel.GameMaster)]
        public CourseMoveTimer CurrentCourseMoveTimer { get; private set; }	
		
        [CommandProperty(AccessLevel.GameMaster)]
        public TillerManHS TillerMan { get; set; }		

        [CommandProperty(AccessLevel.GameMaster)]
        public ushort MaxDurability { get; protected set;}		
		
        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonCondition Condition { get; protected set; }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonHold Hold { get; set; }		
		
		[CommandProperty( AccessLevel.GameMaster )]
		public byte CanModifySecurity{ get; set; }	//0: Never //1: Leader //2: Member	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public byte Public{ get; set; }	//0 : N/A //1: Passenger //2: Crew //3: Officer //4: Captain //5: Deny Access
		
		[CommandProperty( AccessLevel.GameMaster )]
		public byte Party{ get; set; }	//0 : N/A //1: Passenger //2: Crew //3: Officer //4: Captain //5: Deny Access
		
		[CommandProperty( AccessLevel.GameMaster )]
		public byte Guild{ get; set; }	//0 : N/A //1: Passenger //2: Crew //3: Officer //4: Captain //5: Deny Access
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Dictionary<PlayerMobile, byte> PlayerAccess{ get; set; } //0 : N/A //1: Passenger //2: Crew //3: Officer //4: Captain //5: Deny Access
		
        [CommandProperty(AccessLevel.GameMaster)]
        public List<BoatRope> Ropes { get; protected set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RopesCount { get { return Ropes.Count; } }		
		
        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonStatus Status { get; protected set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextRepairTime { get; protected set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public abstract BaseDockedGalleon DockedGalleon { get; }
		
        public int CurrentItemIdModifier { get; protected set; }
		
		public int CannonItemIdModifier { get; protected set; }
		
		public int CannonItemIdModifierSx { get; protected set; }
		
		public int CannonItemIdModifierDx { get; protected set; }	

        [CommandProperty(AccessLevel.GameMaster)]
        public ushort Durability
        {
            get { return _durability; }
            set { SetDurability(value); }
        }		
        #endregion
		
		protected BaseGalleon(int northMultiID)
			: this(northMultiID, 100)
		{
		}
		
        protected BaseGalleon(int northMultiID, ushort maxDurability)
            : base(northMultiID)
        {
            Status = GalleonStatus.Full;
			Condition = GalleonCondition.Pristine;
            MaxDurability = maxDurability;
            _durability = maxDurability;
			CanModifySecurity = 0;
			Public = 0;
			Party = 0;
			Guild = 0;
			PlayerAccess = new Dictionary<PlayerMobile,byte>();
			
            if (Ropes == null)
                Ropes = new List<BoatRope>();			
        }

        public BaseGalleon(Serial serial)
            : base(serial)
        { }
		
		public override void OnSpeech(SpeechEventArgs e)
        {
			Mobile from;
			if (e != null)
				from = e.Mobile;
			else
				return;
		
			if (Owner != null && Owner != from)
			{
				if (PlayerAccess != null && PlayerAccess.ContainsKey((PlayerMobile)from) && (PlayerAccess[(PlayerMobile)from] < 2))
				{
					return;
				}
				else if(from.Guild == Owner.Guild && Guild < 2) 
				{
					return;
				}
				else if(from.Party == Owner.Party && Party < 2)
				{
					return;
				}
				else if(Public < 2)
				{
					return;
				}
			}
			
            if (IsDriven)
                return;
			
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
                            //case 0x6A: LowerAnchor(true); break;
                            //case 0x6B: RaiseAnchor(true); break;
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

        #region Status, Condition and Durability
        protected override void OnDurabilityChange(ushort oldDurability)
        {
            GalleonStatus newStatus = (GalleonStatus)(Durability / Math.Ceiling(MaxDurability / 3.0));
            
			if (newStatus != Status)
            {
                Status = newStatus;
                ItemID = GetMultiId(Facing);
                OnStatusChange();
            }
			
			GalleonCondition newCondition = (GalleonCondition)(Durability / Math.Ceiling(MaxDurability / 4.0));
			
			if (newCondition != Condition)
			{
				Condition = newCondition;
				
				if (TillerMan != null)
					TillerMan.RefreshTillerMan();
			}
        }

        protected virtual void OnStatusChange()
        {
            ContainedObjects.ForEachItem(item =>
            {
                if (item is IRefreshItemID)
                    ((IRefreshItemID)item).RefreshItemID(CurrentItemIdModifier);
            });
        }
		
        private void SetDurability(int newDurability)
        {
            ushort oldDurability = Durability;
			
            if (newDurability > MaxDurability)
                Durability = MaxDurability;
            else if (newDurability <= 0)
			{
                new DecayTimer((BaseGalleon)this).Start();
                Decaying = true;
			}
            else
                Durability = (ushort)newDurability;

            OnDurabilityChange(oldDurability);
        }
        #endregion	
		
        public override void SetName(SpeechEventArgs e)
        {
            if (e.Mobile.AccessLevel < AccessLevel.GameMaster && e.Mobile != Owner)
            {
                if (TillerMan != null)
                    TillerMan.TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!

                return;
            }
            else if (!e.Mobile.Alive)
            {
                if (TillerMan != null)
                    TillerMan.TillerManSay(502582); // You appear to be dead.

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
                    TillerMan.TillerManSay(502531); // Yes, sir.

                return;
            }

            ShipName = newName;
				
            if (TillerMan != null && ShipName != null)
                TillerMan.TillerManSay(1042885, ShipName); // This ship is now called the ~1_NEW_SHIP_NAME~.
            else if (TillerMan != null)
                TillerMan.TillerManSay(502534); // This ship now has no name.
        }

        public override void RemoveName(Mobile m)
        {
            if (CheckDecay())
                return;

            if (m.AccessLevel < AccessLevel.GameMaster && m != Owner)
            {
                if (TillerMan != null)
                    TillerMan.TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!
					
                return;
            }
            else if (!m.Alive)
            {
                if (TillerMan != null)
                    TillerMan.TillerManSay(502582); // You appear to be dead.
					
                return;
            }

            if (ShipName == null)
            {
                if (TillerMan != null)
                    TillerMan.TillerManSay(502526); // Ar, this ship has no name.

                return;
            }

            ShipName = null;

            if (TillerMan != null)
                TillerMan.TillerManSay(502534); // This ship now has no name.
        }

        public override void GiveName(Mobile m)
        {
            if (CheckDecay())
                return;

            if (ShipName == null)
			{
				if (TillerMan != null)
					TillerMan.TillerManSay(502526); // Ar, this ship has no name.
			}
            else
			{
				if (TillerMan != null)
					TillerMan.TillerManSay(1042881, ShipName); // This is the ~1_BOAT_NAME~.
			}
        }	

        public override void BeginRename(Mobile from)
        {
            if (from.AccessLevel < AccessLevel.GameMaster && from != Owner)
            {
                if (TillerMan != null)
                    TillerMan.TillerManSay(Utility.Random(1042876, 4)); // Arr, don't do that! | Arr, leave me alone! | Arr, watch what thour'rt doing, matey! | Arr! Do that again and I’ll throw ye overhead!

                return;
            }

            if (TillerMan != null)
                TillerMan.TillerManSay(502580); // What dost thou wish to name thy ship?

            base.BeginRename(from);
        }	

		public override void EndRename( Mobile from, string newName )
		{
			if ( from.AccessLevel < AccessLevel.GameMaster && from != Owner )
			{
				if ( TillerMan != null )
					TillerMan.TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!

				return;
			}
			else if ( !from.Alive )
			{
				if ( TillerMan != null )
					TillerMan.TillerManSay(502582); // You appear to be dead.

				return;
			}

			base.EndRename(from, newName);
		}		
	
        public override void Refresh()
        {
            base.Refresh();
			
            if (TillerMan != null)
                TillerMan.RefreshTillerMan();
        }	
		
        protected override bool BeginMove(Direction dir, SpeedCode speed)
        {				
			if ( speed == SpeedCode.Slow || speed == SpeedCode.Medium || speed == SpeedCode.Fast )
			{
				switch (Status)
				{
					case GalleonStatus.Half:
					{
						if (speed == SpeedCode.Fast)
							speed = SpeedCode.Medium;						
						
						break;
					}
					case GalleonStatus.Low: 
					{
						if (speed == SpeedCode.Fast || speed == SpeedCode.Medium)
							speed = SpeedCode.Slow;								
						
						break;
					}
				}				
			}

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
					TillerMan.TillerManSay(501424); //Ar, we've stopped sir.
				
				return false;
			}
		}		
		
        protected override bool EndMove()
        {
            if (!base.EndMove())
                return false;

			if (TillerMan != null)
				TillerMan.RefreshTillerMan();
			
            return true;
        }

        protected override void UpdateContainedItemsFacing(Direction oldFacing, Direction newFacing, int count)
        {
            OnStatusChange();
            base.UpdateContainedItemsFacing(oldFacing, newFacing, count);
        }

        protected virtual int UpdateIDModifiers(GalleonStatus status, Direction facing)
        {
            int intStatus = (int)status;
            int intFacing = (int)facing / 2;

            CurrentItemIdModifier = itemIDMods[intStatus, intFacing];
			
			switch (intFacing)
			{
				case 0: 
				{ 
					CannonItemIdModifier = 0; 
					CannonItemIdModifierSx = -1; 
					CannonItemIdModifierDx = 1; 					
					
					break;
				}
				case 1:
				{
					CannonItemIdModifier = 1; 
					CannonItemIdModifierSx = 0; 
					CannonItemIdModifierDx = -2; 					
					
					break;
				}
				case 2: 
				{
					CannonItemIdModifier = -2;
					CannonItemIdModifierSx = 1; 
					CannonItemIdModifierDx = -1; 		
					
					break;
				}
				case 3: 
				{
					CannonItemIdModifier = -1;
					CannonItemIdModifierSx = -2; 
					CannonItemIdModifierDx = 0; 					
					
					break;
				}
			}
			
            return multiIDs[intStatus] + intFacing;
        }

        protected override int GetMultiId(Direction newFacing)
        {
            return UpdateIDModifiers(Status, newFacing);
        }

        protected override bool IsEnabledLandID(int landID)
        {
            if (landID > 167 && landID < 172)
                return true;

            if (landID == 310 || landID == 311)
                return true;

            return false;
        }

        protected override bool IsEnabledStaticID(int staticID)
        {
            if (staticID > 0x1795 && staticID < 0x17B3)
                return true;

            return false;
        }
		
        public enum DryDockResult { Valid, Dead, NoKey, NotAnchored, Mobiles, Items, Hold, Decaying }

        public DryDockResult CheckDryDock(Mobile from)
        {
            if (CheckDecay())
                return DryDockResult.Decaying;

            if (!from.Alive)
                return DryDockResult.Dead;

            Container pack = from.Backpack;
            if ((Ropes == null || !Key.ContainsKey(pack, Ropes[0].KeyValue)))
                return DryDockResult.NoKey;

            if (Hold != null && Hold.Items.Count > 0)
                return DryDockResult.Hold;

            Map map = Map;

            if (map == null || map == Map.Internal)
                return DryDockResult.Items;

            MultiComponentList mcl = Components;

            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o == this || o is BoatRope || o is TillerManHS || o is GalleonHold || o is SingleCannonPlace || o is SingleHelm || o is GalleonMultiComponent || o is MultiCannonPlace || o is MultiHelm || o is MainMast || o is BritainHull )
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
                from.SendGump(new GalleonConfirmDryDockGump(from, this));
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

            BaseDockedGalleon boat = DockedGalleon;

            if (boat == null)
                return;

            RemoveKeys(from);

            from.AddToBackpack(boat);
			
			if (TillerMan != null)
				TillerMan.Delete();
			
            Delete();			
        }
		
		public void RemoveKeys(Mobile m)
        {
            uint keyValue = 0;

            if (this.Ropes != null)
                keyValue = this.Ropes[0].KeyValue;

            Key.RemoveKeys(m, keyValue);
        }
		
		public uint CreateKeys(Mobile m)
		{
			uint value = Key.RandomValue();

			Key packKey = new Key(KeyType.Gold, value, this);
			Key bankKey = new Key(KeyType.Gold, value, this);
			
			packKey.ItemID = 0x1F14;
			bankKey.ItemID = 0x1F14;

			packKey.MaxRange = 10;
			bankKey.MaxRange = 10;

			packKey.Name = "a ship rune";
			bankKey.Name = "a ship rune";
			
			packKey.Hue = 0x04F2;
			bankKey.Hue = 0x04F2;

			BankBox box = m.BankBox;

			if (!box.TryDropItem(m, bankKey, false))
				bankKey.Delete();
			else
				m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502484); // A ship's key is now in my safety deposit box.

			if (m.AddToBackpack(packKey))
				m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502485); // A ship's key is now in my backpack.
			else
				m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502483); // A ship's key is now at my feet.

			return value;
		}	 		
		
        public static BaseGalleon FindGalleonAt(IPoint2D loc, Map map)
        {
            Sector sector = map.GetSector(loc);

            for (int i = 0; i < sector.Multis.Count; i++)
            {
                BaseGalleon galleon = sector.Multis[i] as BaseGalleon;

                if (galleon != null && galleon.Contains(loc.X, loc.Y))
                    return galleon;
            }

            return null;
        }		
		
        public override void OnAfterDelete()
        {
            foreach (IEntity obj in ContainedObjects.ToList()) // toList necessary for enumeration modification
            {
                if (obj is Item)
                    ((Item)obj).Delete();
                else if (obj is ICrew)
                    ((ICrew)obj).Delete();
            }

            if (CurrentMoveTimer != null)
                CurrentMoveTimer.Stop();

            if (CurrentTurnTimer != null)
                CurrentTurnTimer.Stop();

			this.TillerMan.Delete();	
				
            Instances.Remove(this);			
        }

		public void EmergencyRepairs()
		{
			if (NextRepairTime < DateTime.Now)
			{
				//TillerMan.Say(1116597, "5, 5, 5");
				Durability = 50;
				NextRepairTime = DateTime.Now + TimeSpan.FromMinutes(5.0);
			}
			else
			{
				TimeSpan TimeBeforeRepair = NextRepairTime - DateTime.Now;
				TillerMan.Say(1116592, TimeBeforeRepair.ToString());
			}			
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
			
			if ( TimeSpan.FromDays(0.0) <= ( TimeOfDecay - DateTime.Now ) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(1.0) )
			{
				Phase = DecayPhase.Collapsing;
			}
			else if ( TimeSpan.FromDays(1.0) <= ( TimeOfDecay - DateTime.Now ) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(4.0) )
			{
				Phase = DecayPhase.GreatlyWorn;
			}
			else if ( TimeSpan.FromDays(4.0) <= ( TimeOfDecay - DateTime.Now ) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(7.0) )
			{
				Phase = DecayPhase.FairlyWorn;
			}
			else if ( TimeSpan.FromDays(7.0) <= ( TimeOfDecay - DateTime.Now ) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(10.0) )
			{
				Phase = DecayPhase.SomewhatWorn;
			}
			else if ( TimeSpan.FromDays(10.0) <= ( TimeOfDecay - DateTime.Now ) && (TimeOfDecay - DateTime.Now) < TimeSpan.FromDays(12.5) )
			{
				Phase = DecayPhase.SlightlyWorn;
			}
			else if ( TimeSpan.FromDays(12.5) <= ( TimeOfDecay - DateTime.Now ) && (TimeOfDecay - DateTime.Now) <= TimeSpan.FromDays(13.0) )
			{
				Phase = DecayPhase.New;
			}
			
            return false;
        }	

		#region map navigation		
		public void AssociateMap(MapItem map)
		{
			if (CheckDecay())
				return;

			if (map is BlankMap)
			{
				if (TillerMan != null)
					TillerMan.TillerManSay(502575); // Ar, that is not a map, tis but a blank piece of paper!
			}
			else if (map.Pins.Count == 0)
			{
				if (TillerMan != null)
					TillerMan.TillerManSay(502576); // Arrrr, this map has no course on it!
			}
			else
			{
				StopMove(false);

				MapItem = map;
				NextNavPoint = -1;

				if (TillerMan != null)
					TillerMan.TillerManSay(502577); // A map!
			}
		}
		
		public bool StopMove(bool message)
		{
			if (CheckDecay())
				return false;

			if (CurrentMoveTimer == null)
			{
				if (message && TillerMan != null)
					TillerMan.TillerManSay(501443); // Er, the ship is not moving sir.

				return false;
			}

			Moving = Direction.North;
			
			if (CurrentMoveTimer != null)
				CurrentMoveTimer.Stop();

			if (message && TillerMan != null)
				TillerMan.TillerManSay(501429); // Aye aye sir.

			return true;
		}		
		
        public void GiveNavPoint()
        {
            if (TillerMan == null || CheckDecay())
                return;

            if (this.NextNavPoint < 0)
			{
				if (TillerMan != null)
					TillerMan.TillerManSay(1042882); // I have no current nav point.					
			}	
            else
			{
				if (TillerMan != null)
					TillerMan.TillerManSay(1042883, (this.NextNavPoint + 1).ToString()); // My current destination navpoint is nav ~1_NAV_POINT_NUM~.
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
                    TillerMan.TillerManSay(502513); // I have seen no map, sir.						
					
                return false;
            }
            else if (Map != MapItem.Map || !Contains(MapItem.GetWorldLocation()))
            {
                if (TillerMan != null)
                    TillerMan.TillerManSay(502514); // The map is too far away from me, sir.						
					
                return false;
            }
            else if ((Map != Map.Trammel && Map != Map.Felucca) || NextNavPoint < 0 || NextNavPoint >= MapItem.Pins.Count)
            {
                if (TillerMan != null)
                    TillerMan.TillerManSay(1042551); // I don't see that navpoint, sir.					
					
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
                        TillerMan.TillerManSay(1042874, (NextNavPoint + 1).ToString()); // We have arrived at nav point ~1_POINT_NUM~ , sir.					
						
                    if (NextNavPoint + 1 < MapItem.Pins.Count)
                    {
                        NextNavPoint++;

                        if (Order == BoatOrder.Course)
                        {
                            if (TillerMan != null)
                                TillerMan.TillerManSay(1042875, (NextNavPoint + 1).ToString()); // Heading to nav point ~1_POINT_NUM~, sir.								
								
                            return true;
                        }

                        return false;
                    }
                    else
                    {
                        this.NextNavPoint = -1;

                        if (Order == BoatOrder.Course && TillerMan != null)
                            TillerMan.TillerManSay(502515); // The course is completed, sir.						
							
                        return false;
                    }
                }

				speed = (SpeedCode)Math.Min((int)SpeedCode.Fast, maxSpeed);							
            }
			
			Turn(dir);
			
			return BeginCourse(dir, speed);
        }	
		
        protected virtual bool BeginCourse(Direction dir, SpeedCode speed)
        {			
			Order = BoatOrder.Course;
			
			if (speed == SpeedCode.One)	
				return true;

            if (base.BeginMove(dir, speed))
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

                    if (this.MapItem == null || number < 0 || number >= this.MapItem.Pins.Count)
                    {
                        number = -1;
                    }
                }
            }

            if (number == -1)
            {
                if (message && TillerMan != null)
                    TillerMan.TillerManSay(1042551); // I don't see that navpoint, sir.					

                return false;
            }

            NextNavPoint = number;
            return StartCourse(single, message);
        }

        public virtual bool StartCourse(bool single, bool message)
        {
            if (CheckDecay())
                return false;

			if (MapItem == null || MapItem.Deleted)
            {
                if (message && TillerMan != null)
                    TillerMan.TillerManSay(502513); // I have seen no map, sir.					

                return false;
            }
            else if (Map != MapItem.Map || !Contains(MapItem.GetWorldLocation()))
            {
                if (message && TillerMan != null)
                    TillerMan.TillerManSay(502514); // The map is too far away from me, sir.		

                return false;
            }
            else if ((Map != Map.Trammel && Map != Map.Felucca) || NextNavPoint < 0 || NextNavPoint >= MapItem.Pins.Count)
            {
                if (message && TillerMan != null)
                    TillerMan.TillerManSay(1042551); // I don't see that navpoint, sir.				

                return false;
            }

            Order = single ? BoatOrder.Single : BoatOrder.Course;

            if (CurrentMoveTimer != null)
                CurrentMoveTimer.Stop();			
			
			CurrentCourseMoveTimer = new CourseMoveTimer(this, FastInterval, SpeedCode.Fast, false);
			CurrentCourseMoveTimer.Start();

            if (message && TillerMan != null)
                TillerMan.TillerManSay(501429); // Aye aye sir.			

            return true;
        }	
		
        public virtual bool CourseTurn(int offset, bool message)
		{
            if (CurrentTurnTimer != null)
                CurrentTurnTimer.Stop();           

            if (CheckDecay())
                return false;

            if (SetFacing((Direction)(((int)Facing + offset) & 0x7)))		
			{
                return true;
            }
            else
            {
                if (message)
                    TillerMan.TillerManSay(501423); // Ar, can't turn sir.

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
			private readonly BaseGalleon _galleon;
			
			public CourseMoveTimer(BaseGalleon galleon, TimeSpan interval, SpeedCode speed, bool single)
				: base(interval, interval, single ? 1 : 0)
			{
	            _galleon = galleon;
                Priority = TimerPriority.TwentyFiveMS;			
			}
			
            protected override void OnTick()
            {
                if (!_galleon.DoMove() || _galleon.Speed == SpeedCode.One)				
                    _galleon.EndCourse();				
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
					//Properties
					Hold = reader.ReadItem() as GalleonHold;
					TillerMan = reader.ReadMobile() as TillerManHS;					
					MaxDurability = reader.ReadUShort();
					_durability = reader.ReadUShort();					
					NextRepairTime = reader.ReadDeltaTime();
					
					//New Security System
					CanModifySecurity = reader.ReadByte();
					Public = reader.ReadByte();
					Party = reader.ReadByte();
					Guild = reader.ReadByte();
					
					int listSize = reader.ReadInt();
					PlayerAccess = new Dictionary<PlayerMobile, byte>();					
					PlayerMobile player;
					byte access;
					for (int i = 0; i < listSize; i++)
					{
						player = (PlayerMobile)reader.ReadMobile();
						access = (byte)reader.ReadByte();
						PlayerAccess.Add(player, access);
					}					
					
					//mooring lines
					int toread = reader.ReadInt();
					Ropes = new List<BoatRope>();
					for (int i = 0; i < toread; i++)
					{
						BoatRope rope = reader.ReadItem() as BoatRope;
						Ropes.Add(rope);		
					}
					
					goto case 0;
				}
				
				case 0:
				{				
					Status = (GalleonStatus)(Durability / Math.Ceiling(MaxDurability / 3.0));
					Condition = (GalleonCondition)(Durability / Math.Ceiling(MaxDurability / 4.0));
					ItemID = GetMultiId(Facing);
					
					break;
				}
			}
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
			
			//version 1
			//Properties
			writer.Write((Item)Hold);
			writer.Write((Mobile)TillerMan);
			writer.Write((ushort)MaxDurability);
            writer.Write((ushort)Durability);			
			writer.WriteDeltaTime(NextRepairTime);
			
			//New Security System
			writer.Write(CanModifySecurity);
			writer.Write(Public);
			writer.Write(Party);
			writer.Write(Guild);
			
			int listSize = 0;
			if (PlayerAccess != null)
				listSize = PlayerAccess.Count;
			
			writer.Write((int)listSize);

			if (listSize > 0)
			{
				foreach(KeyValuePair<PlayerMobile, byte> entry in PlayerAccess)
				{		
					writer.Write((Mobile)entry.Key);
					writer.Write((byte)entry.Value);
				}	
			}			

			//Mooring Lines
            writer.Write((int)Ropes.Count);
            for (int i = 0; i < Ropes.Count; i++)
			{
				BoatRope rope = (BoatRope)Ropes[i];
				writer.Write((Item)rope);		
			}
        }
        #endregion
		
        public class DecayTimer : Timer
        {
            private BaseGalleon _galleon;
            private int _count;

            public DecayTimer(BaseGalleon galleon)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(5.0))
            {
                _galleon = galleon;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (_count == 5)
                {
                    _galleon.Delete();
                    Stop();
                }
                else
                {
                    _galleon.Location = new Point3D(_galleon.X, _galleon.Y, _galleon.Z - 1);

                    if (_galleon.TillerMan != null)
                        _galleon.TillerMan.TillerManSay(1007168 + _count);

                    ++_count;
                }
            }
        }		
    }
}