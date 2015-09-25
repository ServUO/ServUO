using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Movement;
using Server.Network;

namespace Server.Multis
{
    public enum BoatDirectionCommand
    {
        Forward = 0,
        ForwardLeft = -1,
        ForwardRight = +1,
        Left = -2,
        Right = +2,
        BackwardLeft = -3,
        BackwardRight = +3,
        Backward = 4
    }
	
	public enum DecayPhase
	{
		New = 5,
		SlightlyWorn = 4,
		SomewhatWorn = 3,
		FairlyWorn = 2,
		GreatlyWorn = 1,
		Collapsing = 0
	}

    public abstract class BaseShip : BaseSmoothMulti
    {
        private static readonly Direction Forward = Direction.North;
        private static readonly Direction ForwardLeft = Direction.Up;
        private static readonly Direction ForwardRight = Direction.Right;
        private static readonly Direction Backward = Direction.South;
        private static readonly Direction BackwardLeft = Direction.Left;
        private static readonly Direction BackwardRight = Direction.Down;
        private static readonly Direction Left = Direction.West;
        private static readonly Direction Right = Direction.East;
        private static readonly Direction Port = Left;
        private static readonly Direction Starboard = Right;
		private static TimeSpan BoatDecayDelay = TimeSpan.FromDays(13.0);

        #region Properties		
        public override bool HandlesOnSpeech { get { return true; } }			

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsMovingShip{ get{ return ( CurrentMoveTimer != null ); } }
        				
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName { get; set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeOfDecay { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
        public DecayPhase Phase { get; protected set; }				
		
        [CommandProperty(AccessLevel.GameMaster)]
        public BoatOrder Order { get; protected set; }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public MapItem MapItem{ get; protected set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public int NextNavPoint{ get; protected set; }

		[CommandProperty( AccessLevel.GameMaster )]
        public virtual Point3D MarkOffset{ get{ return Point3D.Zero; } }			
        #endregion
        
        protected BaseShip(int itemId)
            : base(itemId)
        {
			TimeOfDecay = DateTime.Now + BoatDecayDelay;
			Phase = DecayPhase.New;
			NextNavPoint = -1;
        }

        public BaseShip(Serial serial): base(serial)
        {
        }

        protected virtual void OnDurabilityChange(ushort oldDurability)
        {			
        }	
		
		#region Commands
        protected override bool BeginMove(Direction dir, SpeedCode speed)
        {				
			Order = BoatOrder.Move;

            if (base.BeginMove(dir, speed))
                return true;

            return false;
        }

        protected override bool BeginTurn(Direction newDirection)
        {
            return base.BeginTurn(newDirection);
        }
		
		protected override bool Move(Direction dir, SpeedCode speed)			
		{ 
			return base.Move(dir, speed);
		}

        protected override bool EndMove()
        {
            if (!base.EndMove())
                return false;

            return true;
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
                        }

                        break;
                    }
                }
            }
        }
        #endregion

        protected Direction ComputeDirection(BoatDirectionCommand cmd)
        {
            return (Direction)((int)Facing + (int)cmd) & Direction.Mask;
        }	

        public Point3D GetMarkedLocation()
        {
            Point3D p = new Point3D(this.X + this.MarkOffset.X, this.Y + this.MarkOffset.Y, this.Z + this.MarkOffset.Z);

			int rx = p.X - Location.X;
			int ry = p.Y - Location.Y;
			int count = (int)Facing / 2;

			for ( int i = 0; i < count; ++i )
			{
				int temp = rx;
				rx = -ry;
				ry = temp;
			}

			return new Point3D( Location.X + rx, Location.Y + ry, p.Z );			
        }		
		
        public bool CheckKey(uint keyValue)
        {
			object objValue = this.GetPropertyValue("PPlank");
			NewPlank PPlank = objValue as NewPlank;
		
            if (PPlank != null)
				if (PPlank.KeyValue == keyValue)
					return true;
					

			objValue = this.GetPropertyValue("SPlank");
			NewPlank SPlank = objValue as NewPlank;
		
            if (SPlank != null)
				if (SPlank.KeyValue == keyValue)
					return true;
					
					
			objValue = this.GetPropertyValue("Ropes");
			List<BoatRope> Ropes = objValue as List<BoatRope>;
		
            if (Ropes != null)
				if (Ropes[0].KeyValue == keyValue)
					return true;

            return false;
        }
		
        public virtual void SetName(SpeechEventArgs e)
        {
            if (CheckDecay())
                return;

            if (e.Speech.Length > 8)
            {
                string newName = e.Speech.Substring(8).Trim();

                if (newName.Length == 0)
                    newName = null;

                Rename(newName);
            }
        }

        public virtual void Rename(string newName)
        {

        }

        public virtual void RemoveName(Mobile m)
        {

        }

        public virtual void GiveName(Mobile m)
        {

        }

        protected bool Decaying;

        public virtual void Refresh()
        {
            TimeOfDecay = DateTime.Now + BoatDecayDelay;
			CheckDecay();
        }

        public virtual bool CheckDecay()
		{
			return false;
		}
		
		public bool CanCommand( Mobile m )
		{
			return true;
		}
		
        public virtual void BeginRename(Mobile from)
        {
            if (CheckDecay())
                return;

            from.Prompt = new NewRenameBoatPrompt(this);
        }
		
		public virtual void EndRename( Mobile from, string newName )
		{
			if ( Deleted || CheckDecay() )
				return;

			newName = newName.Trim();

			if ( newName.Length == 0 )
				newName = null;

			Rename( newName );
		}	
		
        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);//version
			
			//version 1
			writer.Write((int)Phase);
			writer.Write((Mobile)Owner);
			writer.Write((Item)MapItem);
			writer.Write((int)NextNavPoint);
			writer.WriteDeltaTime(TimeOfDecay);
			writer.Write((string)ShipName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
	
			switch ( version )
			{					
				case 1:
				{	
					Phase = (DecayPhase) reader.ReadInt();
					Owner = reader.ReadMobile();
					MapItem = reader.ReadItem() as MapItem;
					NextNavPoint = reader.ReadInt();
					TimeOfDecay = reader.ReadDeltaTime();
					ShipName = reader.ReadString();
					
					break;
				}
			}			
        }
        #endregion		
    }
	
	public static class PropertyHelper
	{
		public static object GetPropertyValue<T>(this T classInstance, string propertyName)
		{
			PropertyInfo property = classInstance.GetType().GetProperty(propertyName);
			if (property != null)
				return property.GetValue(classInstance, null);
			return null;
		}
	}	
}
