using System;
using System.Collections;
using Server;
using Server.Mobiles;

/*
** TimedLever, TimedSwitch, and XmlLatch class and TimedSwitchableItem
** Version 1.01
** updated 5/06/04
** ArteGordon
*/
namespace Server.Items
{

	public class XmlLatch : Item
	{

		private TimeSpan m_MinDelay;
		private TimeSpan m_MaxDelay;
		private DateTime m_End;
		private InternalTimer m_Timer;
		private int m_State = 0;
		private int m_ResetState = 0;


    
		[Constructable]
		public XmlLatch() : base( 0x1BBF )
		{
			Movable = false;
		}

		public XmlLatch(int itemID) : base( itemID )
		{
		}

		public XmlLatch( Serial serial ) : base( serial )
		{
		}


		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan MinDelay
		{
			get { return m_MinDelay; }
			set 
			{
				m_MinDelay = value;
				InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan MaxDelay
		{
			get { return m_MaxDelay; }
			set 
			{
				m_MaxDelay = value;
				InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan TimeUntilReset
		{
			get
			{
				if ( m_Timer != null && m_Timer.Running )
					return m_End - DateTime.Now;
				else
					return TimeSpan.FromSeconds( 0 );
			}
			set
			{
				DoTimer( value );
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int ResetState
		{
				get{ return m_ResetState; }
			set 
			{
				m_ResetState = value;
				if ( m_Timer != null && m_Timer.Running )
					m_Timer.Stop();
				InvalidateProperties();}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int State
		{
				get{ return m_State; }
			set 
			{
				m_State = value;
				StartTimer();
				InvalidateProperties();}
		}
      
		public void StartTimer()
		{
			if(m_State != m_ResetState && (m_MinDelay > TimeSpan.Zero || m_MaxDelay > TimeSpan.Zero))
				DoTimer();
			else
				if ( m_Timer != null && m_Timer.Running )
				m_Timer.Stop();
		}

		public virtual void OnReset()
		{
			State = ResetState;
		}

    
		public void DoTimer()
		{

			int minSeconds = (int)m_MinDelay.TotalSeconds;
			int maxSeconds = (int)m_MaxDelay.TotalSeconds;

			TimeSpan delay = TimeSpan.FromSeconds( Utility.RandomMinMax( minSeconds, maxSeconds ) );
			DoTimer( delay );
		}

		public void DoTimer( TimeSpan delay )
		{


			m_End = DateTime.Now + delay;

			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = new InternalTimer( this, delay );
			m_Timer.Start();
		}
        
		private class InternalTimer : Timer
		{
			private XmlLatch m_latch;

			public InternalTimer( XmlLatch xmllatch, TimeSpan delay ) : base( delay )
			{
				Priority = TimerPriority.OneSecond;
				m_latch = xmllatch;
			}

			protected override void OnTick()
			{
				if(m_latch != null && !m_latch.Deleted)
				{
					Stop();
					m_latch.OnReset();
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			// version 0
			writer.Write( this.m_State );
			writer.Write( this.m_ResetState );
			writer.Write( this.m_MinDelay );
			writer.Write( this.m_MaxDelay );
			bool running = (m_Timer != null && m_Timer.Running);
			writer.Write( running );
			if ( m_Timer != null && m_Timer.Running )
				writer.Write( this.m_End - DateTime.Now );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					// note this is redundant with the base class serialization, but it is there for older (pre 1.02) version compatibility
					// not needed
					this.m_State = reader.ReadInt();
					this.m_ResetState = reader.ReadInt();
					this.m_MinDelay = reader.ReadTimeSpan();
					this.m_MaxDelay = reader.ReadTimeSpan();
					bool running = reader.ReadBool();
					if(running)
					{
						TimeSpan delay = reader.ReadTimeSpan();
						this.DoTimer( delay );
					}
				}
					break;
			}
		}

	}

	public class TimedLever : XmlLatch, ILinkable
	{
		public enum leverType { Two_State, Three_State}
		private leverType m_LeverType = leverType.Two_State;
		private int m_LeverSound = 936;
		private Item m_TargetItem0 = null;
		private string m_TargetProperty0 = null;
		private Item m_TargetItem1 = null;
		private string m_TargetProperty1 = null;
		private Item m_TargetItem2 = null;
		private string m_TargetProperty2 = null;

		private Item m_LinkedItem = null;
		private bool already_being_activated = false;

		private bool m_Disabled = false;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Disabled
		{
			set { m_Disabled = value; }
			get { return m_Disabled; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Link
		{
			set { m_LinkedItem = value; }
			get { return m_LinkedItem; }
		}

		[Constructable]
		public TimedLever() : base( 0x108C )
		{
			Name = "A lever";
			Movable = false;
		}

		public TimedLever( Serial serial ) : base( serial )
		{
		}


		[CommandProperty(AccessLevel.GameMaster)]
		public override int State
		{
			get { return base.State; }
			set
			{
				// prevent infinite recursion 
				if (!already_being_activated)
				{
					already_being_activated = true;
					Activate(null, value, null);
					already_being_activated = false;
				}

				InvalidateProperties();
			}
		}

		public void Activate(Mobile from, int state, ArrayList links)
		{
			if (Disabled) return;

			string status_str = null;

			if (state < 0) state = 0;
			if (state > 1 && m_LeverType == leverType.Two_State) state = 1;
			if (state > 2) state = 2;

			// assign the latch state and start the timer
			base.State = state;

			// update the graphic
			SetLeverStatic();

			// play the switching sound
			//if (from != null)
			//{
			//	from.PlaySound(m_LeverSound);
			//}
			try
			{
				Effects.PlaySound(Location, Map, m_LeverSound);
			}
			catch { }

			// if a target object has been specified then apply the property modification
			if (state == 0 && m_TargetItem0 != null && !m_TargetItem0.Deleted && m_TargetProperty0 != null && m_TargetProperty0.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty0, m_TargetItem0, null, this, out status_str);
			}
			if (state == 1 && m_TargetItem1 != null && !m_TargetItem1.Deleted && m_TargetProperty1 != null && m_TargetProperty1.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty1, m_TargetItem1, null, this, out status_str);
			}
			if (state == 2 && m_TargetItem2 != null && !m_TargetItem2.Deleted && m_TargetProperty2 != null && m_TargetProperty2.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty2, m_TargetItem2, null, this, out status_str);
			}

			// if the switch is linked, then activate the link as well
			if (Link != null && Link is ILinkable)
			{
				if (links == null)
				{
					links = new ArrayList();
				}
				// activate other linked objects if they have not already been activated
				if (!links.Contains(this))
				{
					links.Add(this);

					((ILinkable)Link).Activate(from, state, links);
				}
			}

			// report any problems to staff
			if (status_str != null && from != null && from.AccessLevel > AccessLevel.Player)
			{
				from.SendMessage("{0}", status_str);
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int LeverSound
		{
				get{ return m_LeverSound; }
			set 
			{
				m_LeverSound = value;
				InvalidateProperties();}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public leverType LeverType
		{
				get{ return m_LeverType; }
			set 
			{
				m_LeverType = value; State = 0;
				InvalidateProperties();}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		new public virtual Direction Direction
		{
			get{ return base.Direction; }
			set { base.Direction = value; SetLeverStatic();InvalidateProperties();}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Item Target0Item
		{
				get{ return m_TargetItem0; }
			set { m_TargetItem0 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target0Property
		{
				get{ return m_TargetProperty0; }
			set { m_TargetProperty0 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target0ItemName
		{      	get{  if(m_TargetItem0 != null && !m_TargetItem0.Deleted) return m_TargetItem0.Name;  else return null;}      }

		[CommandProperty( AccessLevel.GameMaster )]
		public Item Target1Item
		{
				get{ return m_TargetItem1; }
			set { m_TargetItem1 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target1Property
		{
				get{ return m_TargetProperty1; }
			set { m_TargetProperty1 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target1ItemName
		{      	get{  if(m_TargetItem1 != null && !m_TargetItem1.Deleted) return m_TargetItem1.Name; else return null;}      }

		[CommandProperty( AccessLevel.GameMaster )]
		public Item Target2Item
		{
				get{ return m_TargetItem2; }
			set { m_TargetItem2 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target2Property
		{
				get{ return m_TargetProperty2; }
			set { m_TargetProperty2 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target2ItemName
		{      	get{  if(m_TargetItem2 != null && !m_TargetItem2.Deleted) return m_TargetItem2.Name; else return null;}      }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			// version 2
			writer.Write(this.m_Disabled);
			// version 1
			writer.Write(this.m_LinkedItem);
			// version 0
			writer.Write( this.m_LeverSound );
			writer.Write( (int)this.m_LeverType );
			writer.Write( this.m_TargetItem0 );
			writer.Write( this.m_TargetProperty0 );
			writer.Write( this.m_TargetItem1 );
			writer.Write( this.m_TargetProperty1 );
			writer.Write( this.m_TargetItem2 );
			writer.Write( this.m_TargetProperty2 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 2:
					{
						m_Disabled = reader.ReadBool();
						goto case 1;
					}
				case 1:
					{
						m_LinkedItem = reader.ReadItem();
						goto case 0;
					}
				case 0:
				{
					this.m_LeverSound = reader.ReadInt();
					int ltype = reader.ReadInt();
					switch( ltype )
					{
						case (int)leverType.Two_State:		this.m_LeverType = leverType.Two_State;	break;
						case (int)leverType.Three_State:	this.m_LeverType = leverType.Three_State;	break;
                      
					}
					this.m_TargetItem0 = reader.ReadItem();
					this.m_TargetProperty0 = reader.ReadString();
					this.m_TargetItem1 = reader.ReadItem();
					this.m_TargetProperty1 = reader.ReadString();
					this.m_TargetItem2 = reader.ReadItem();
					this.m_TargetProperty2 = reader.ReadString();
				}
					break;
			}
			// refresh the lever static to reflect the state
			SetLeverStatic();
		}

		public void SetLeverStatic()
		{

			switch(this.Direction)
			{
				case Direction.North:
				case Direction.South:
				case Direction.Right:
				case Direction.Up:
					if(m_LeverType == leverType.Two_State)
						this.ItemID = 0x108c+ State*2;
					else
						this.ItemID = 0x108c+ State;
					break;
				case Direction.East:
				case Direction.West:
				case Direction.Left:
				case Direction.Down:
					if(m_LeverType == leverType.Two_State)
						this.ItemID = 0x1093+ State*2;
					else
						this.ItemID = 0x1093+ State;
					break;
				default:
					break;
			}
		}


		public override void OnDoubleClick( Mobile from )
		{
			if(from == null || Disabled) return;

			if ( !from.InRange( GetWorldLocation(), 2 ) || !from.InLOS(this))
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}
			// animate and change state
			int newstate = State+1;
			if(newstate > 1 && m_LeverType == leverType.Two_State) newstate = 0;
			if(newstate > 2) newstate = 0;

			// carry out the switch actions
			Activate(from, newstate, null);
		}
	}

	public class TimedSwitch : XmlLatch, ILinkable
	{
		private int m_SwitchSound = 939;
		private Item m_TargetItem0 = null;
		private string m_TargetProperty0 = null;
		private Item m_TargetItem1 = null;
		private string m_TargetProperty1 = null;

		private Item m_LinkedItem = null;
		private bool already_being_activated = false;

		private bool m_Disabled = false;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Disabled
		{
			set { m_Disabled = value; }
			get { return m_Disabled; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Link
		{
			set { m_LinkedItem = value; }
			get { return m_LinkedItem; }
		}

		[Constructable]
		public TimedSwitch() : base( 0x108F )
		{
			Name = "A switch";
			Movable = false;
		}

		public TimedSwitch( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int State
		{
			get { return base.State; }
			set
			{
				// prevent infinite recursion 
				if (!already_being_activated)
				{
					already_being_activated = true;
					Activate(null, value, null);
					already_being_activated = false;
				}

				InvalidateProperties();
			}
		}

		public void Activate(Mobile from, int state, ArrayList links)
		{
			if (Disabled) return;

			string status_str = null;

			if (state < 0) state = 0;
			if (state > 1) state = 1;

			// assign the latch state and start the timer
			base.State = state;

			// update the graphic
			SetSwitchStatic();

			// play the switching sound
			//if (from != null)
			//{
			//	from.PlaySound(m_SwitchSound);
			//}
			try
			{
				Effects.PlaySound(Location, Map, m_SwitchSound);
			}
			catch { }

			// if a target object has been specified then apply the property modification
			if (state == 0 && m_TargetItem0 != null && !m_TargetItem0.Deleted && m_TargetProperty0 != null && m_TargetProperty0.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty0, m_TargetItem0, null, this, out status_str);
			}
			if (state == 1 && m_TargetItem1 != null && !m_TargetItem1.Deleted && m_TargetProperty1 != null && m_TargetProperty1.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty1, m_TargetItem1, null, this, out status_str);
			}

			// if the switch is linked, then activate the link as well
			if (Link != null && Link is ILinkable)
			{
				if (links == null)
				{
					links = new ArrayList();
				}
				// activate other linked objects if they have not already been activated
				if (!links.Contains(this))
				{
					links.Add(this);

					((ILinkable)Link).Activate(from, state, links);
				}
			}

			// report any problems to staff
			if (status_str != null && from != null && from.AccessLevel > AccessLevel.Player)
			{
				from.SendMessage("{0}", status_str);
			}
		}


		[CommandProperty( AccessLevel.GameMaster )]
		public int SwitchSound
		{
				get{ return m_SwitchSound; }
			set 
			{
				m_SwitchSound = value;
				InvalidateProperties();}
		}
      
		[CommandProperty( AccessLevel.GameMaster )]
		new public virtual Direction Direction
		{
			get{ return base.Direction; }
			set { base.Direction = value; SetSwitchStatic();InvalidateProperties();}
		}
      
		[CommandProperty( AccessLevel.GameMaster )]
		public Item Target0Item
		{
				get{ return m_TargetItem0; }
			set { m_TargetItem0 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target0Property
		{
				get{ return m_TargetProperty0; }
			set { m_TargetProperty0 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target0ItemName
		{      	get{  if(m_TargetItem0 != null && !m_TargetItem0.Deleted) return m_TargetItem0.Name; else return null;}      }

		[CommandProperty( AccessLevel.GameMaster )]
		public Item Target1Item
		{
				get{ return m_TargetItem1; }
			set { m_TargetItem1 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target1Property
		{
				get{ return m_TargetProperty1; }
			set { m_TargetProperty1 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target1ItemName
		{      	get{  if(m_TargetItem1 != null && !m_TargetItem1.Deleted) return m_TargetItem1.Name; else return null;}      }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
			// version 2
			writer.Write(this.m_Disabled);
			// version 1
			writer.Write(this.m_LinkedItem);
			// version 0
			writer.Write( this.m_SwitchSound );
			writer.Write( this.m_TargetItem0 );
			writer.Write( this.m_TargetProperty0 );
			writer.Write( this.m_TargetItem1 );
			writer.Write( this.m_TargetProperty1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 2:
					{
						m_Disabled = reader.ReadBool();
						goto case 1;
					}
				case 1:
					{
						m_LinkedItem = reader.ReadItem();
						goto case 0;
					}
				case 0:
				{

					this.m_SwitchSound = reader.ReadInt();
					this.m_TargetItem0 = reader.ReadItem();
					this.m_TargetProperty0 = reader.ReadString();
					this.m_TargetItem1 = reader.ReadItem();
					this.m_TargetProperty1 = reader.ReadString();
				}
					break;
			}
			// refresh the lever static to reflect the state
			SetSwitchStatic();
		}

		public void SetSwitchStatic()
		{

			switch(this.Direction)
			{
				case Direction.North:
				case Direction.South:
				case Direction.Right:
				case Direction.Up:
					this.ItemID = 0x108f+ State;
					break;
				case Direction.East:
				case Direction.West:
				case Direction.Left:
				case Direction.Down:
					this.ItemID = 0x1091+ State;
					break;
				default:
					this.ItemID = 0x108f+ State;
					break;
			}
		}


		public override void OnDoubleClick( Mobile from )
		{
			if(from == null || Disabled) return;

			if ( !from.InRange( GetWorldLocation(), 2 ) || !from.InLOS(this))
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}
			// animate and change state
			int newstate = State+1;
			if(newstate > 1) newstate = 0;

			// carry out the switch actions
			Activate(from, newstate, null);

		}
	}
   
	public class TimedSwitchableItem : XmlLatch, ILinkable
	{
		private int m_SwitchSound = 939;
		private Item m_TargetItem0 = null;
		private string m_TargetProperty0 = null;
		private Item m_TargetItem1 = null;
		private string m_TargetProperty1 = null;
		private int m_ItemID0 = 0x108F;
		private int m_ItemID1 = 0x1090;

		private Item m_LinkedItem = null;
		private bool already_being_activated = false;

		private Point3D m_Offset = Point3D.Zero;

		private bool m_Disabled = false;
		private bool m_NoDoubleClick = false;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Disabled
		{
			set { m_Disabled = value; }
			get { return m_Disabled; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool NoDoubleClick
		{
			set { m_NoDoubleClick = value; }
			get { return m_NoDoubleClick; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D Offset
		{
			set { m_Offset = value; }
			get { return m_Offset; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Link
		{
			set { m_LinkedItem = value; }
			get { return m_LinkedItem; }
		}

		[Constructable]
		public TimedSwitchableItem() : base( 0x108F )
		{
			Name = "A switchable item";
			Movable = false;
		}

		public TimedSwitchableItem( Serial serial ) : base( serial )
		{
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int State
		{
			get { return base.State; }
			set
			{
				// prevent infinite recursion 
				if (!already_being_activated)
				{
					already_being_activated = true;
					Activate(null, value, null);
					already_being_activated = false;
				}

				InvalidateProperties();
			}
		}


		public void Activate(Mobile from, int state, ArrayList links)
		{
			if (Disabled) return;

			string status_str = null;

			if (state < 0) state = 0;
			if (state > 1) state = 1;

			if (base.State != state)
			{
				// apply the offset
				SetSwitchOffset();
			}
			// assign the latch state and start the timer
			base.State = state;

			// update the graphic
			SetSwitchStatic();

			// play the switching sound
			//if (from != null)
			//{
			//	from.PlaySound(m_SwitchSound);
			//}
			try
			{
				Effects.PlaySound(Location, Map, m_SwitchSound);
			}
			catch { }

			// if a target object has been specified then apply the property modification
			if (state == 0 && m_TargetItem0 != null && !m_TargetItem0.Deleted && m_TargetProperty0 != null && m_TargetProperty0.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty0, m_TargetItem0, null, this, out status_str);
			}
			if (state == 1 && m_TargetItem1 != null && !m_TargetItem1.Deleted && m_TargetProperty1 != null && m_TargetProperty1.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty1, m_TargetItem1, null, this, out status_str);
			}

			// if the switch is linked, then activate the link as well
			if (Link != null && Link is ILinkable)
			{
				if (links == null)
				{
					links = new ArrayList();
				}
				// activate other linked objects if they have not already been activated
				if (!links.Contains(this))
				{
					links.Add(this);

					((ILinkable)Link).Activate(from, state, links);
				}
			}

			// report any problems to staff
			if (status_str != null && from != null && from.AccessLevel > AccessLevel.Player)
			{
				from.SendMessage("{0}", status_str);
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ItemID0
		{
				get{ return m_ItemID0; }
			set 
			{
				m_ItemID0 = value;
				// refresh the lever static to reflect the state
				SetSwitchStatic();
				InvalidateProperties();
			}
		}
      
		[CommandProperty( AccessLevel.GameMaster )]
		public int ItemID1
		{
				get{ return m_ItemID1; }
			set 
			{
				m_ItemID1 = value;
				// refresh the lever static to reflect the state
				SetSwitchStatic();
				InvalidateProperties();}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SwitchSound
		{
				get{ return m_SwitchSound; }
			set 
			{
				m_SwitchSound = value;
				InvalidateProperties();}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Item Target0Item
		{
				get{ return m_TargetItem0; }
			set { m_TargetItem0 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target0Property
		{
				get{ return m_TargetProperty0; }
			set { m_TargetProperty0 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target0ItemName
		{      	get{  if(m_TargetItem0 != null && !m_TargetItem0.Deleted) return m_TargetItem0.Name; else return null;}      }

		[CommandProperty( AccessLevel.GameMaster )]
		public Item Target1Item
		{
				get{ return m_TargetItem1; }
			set { m_TargetItem1 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target1Property
		{
				get{ return m_TargetProperty1; }
			set { m_TargetProperty1 = value;InvalidateProperties();}
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string Target1ItemName
		{      	get{  if(m_TargetItem1 != null && !m_TargetItem1.Deleted) return m_TargetItem1.Name; else return null;}      }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 4 ); // version
			// version 4
			writer.Write(this.m_NoDoubleClick);
			// version 3
			writer.Write(this.m_Disabled);
			writer.Write(this.m_Offset);
			// version 2
			writer.Write(this.m_LinkedItem);
			// version 1
			writer.Write( this.m_ItemID0 );
			writer.Write( this.m_ItemID1 );
			// version 0
			writer.Write( this.m_SwitchSound );
			writer.Write( this.m_TargetItem0 );
			writer.Write( this.m_TargetProperty0 );
			writer.Write( this.m_TargetItem1 );
			writer.Write( this.m_TargetProperty1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 4:
					{
						m_NoDoubleClick = reader.ReadBool();
						goto case 3;
					}
				case 3:
					{
						m_Disabled = reader.ReadBool();
						m_Offset = reader.ReadPoint3D();
						goto case 2;
					}
				case 2:
					{
						m_LinkedItem = reader.ReadItem();
						goto case 1;
					}
				case 1:
				{
					m_ItemID0 = reader.ReadInt();
					m_ItemID1 = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{

					m_SwitchSound = reader.ReadInt();
					m_TargetItem0 = reader.ReadItem();
					m_TargetProperty0 = reader.ReadString();
					m_TargetItem1 = reader.ReadItem();
					m_TargetProperty1 = reader.ReadString();
				}
					break;
			}
			// refresh the lever static to reflect the state
			SetSwitchStatic();
		}

		public void SetSwitchStatic()
		{

			switch(State)
			{
				case 0:
					this.ItemID = ItemID0;
					break;
				case 1:
					this.ItemID = ItemID1;
					break;
			}
		}

		public void SetSwitchOffset()
		{

			switch (State)
			{
				case 0:
					Location = new Point3D(X - m_Offset.X, Y - m_Offset.Y, Z - m_Offset.Z);
					break;
				case 1:
					Location = new Point3D(X + m_Offset.X, Y + m_Offset.Y, Z + m_Offset.Z);
					break;
			}
		}


		public override void OnDoubleClick( Mobile from )
		{
			if(from == null || Disabled || NoDoubleClick) return;

			if ( !from.InRange( GetWorldLocation(), 2 ) || !from.InLOS(this))
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}
			// animate and change state
			int newstate = State+1;
			if(newstate > 1) newstate = 0;

			// carry out the switch actions
			Activate(from, newstate, null);

		}
	}
}
