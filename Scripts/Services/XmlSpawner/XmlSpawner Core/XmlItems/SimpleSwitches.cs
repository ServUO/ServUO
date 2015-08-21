using System;
using System.Collections;
using Server;
using Server.Mobiles;

/*
** SimpleLever, SimpleSwitch, and CombinationLock
** Version 1.03
** updated 5/06/04
** ArteGordon
**
*/
namespace Server.Items
{
	public interface ILinkable
	{
		Item Link { set; get; }
		void Activate(Mobile from, int state, ArrayList links);
	}

	public class SimpleLever : Item, ILinkable
	{
		public enum leverType { Two_State, Three_State }

		private int m_LeverState = 0;
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
		public SimpleLever()
			: base(0x108C)
		{
			Name = "A lever";
			Movable = false;
		}

		public SimpleLever(Serial serial)
			: base(serial)
		{
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int LeverState
		{
			get { return m_LeverState; }
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

		[CommandProperty(AccessLevel.GameMaster)]
		public int LeverSound
		{
			get { return m_LeverSound; }
			set
			{
				m_LeverSound = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public leverType LeverType
		{
			get { return m_LeverType; }
			set
			{
				m_LeverType = value; LeverState = 0;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		new public virtual Direction Direction
		{
			get { return base.Direction; }
			set { 
				base.Direction = value; 
				SetLeverStatic(); 
				InvalidateProperties(); 
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Target0Item
		{
			get { return m_TargetItem0; }
			set { m_TargetItem0 = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Target0Property
		{
			get { return m_TargetProperty0; }
			set { m_TargetProperty0 = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Target0ItemName
		{ 
			get 
		{ 
				if (m_TargetItem0 != null && !m_TargetItem0.Deleted) 
					return m_TargetItem0.Name; 
				else 
					return null; 
			} 
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Target1Item
		{
			get { return m_TargetItem1; }
			set { m_TargetItem1 = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Target1Property
		{
			get { return m_TargetProperty1; }
			set { m_TargetProperty1 = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Target1ItemName
		{ 
			get 
			{ 
				if (m_TargetItem1 != null && !m_TargetItem1.Deleted) 
					return 
						m_TargetItem1.Name; 
				else 
					return null; 
			} 
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Target2Item
		{
			get { return m_TargetItem2; }
			set { m_TargetItem2 = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Target2Property
		{
			get { return m_TargetProperty2; }
			set { m_TargetProperty2 = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Target2ItemName
		{ 
			get 
			{ 
				if (m_TargetItem2 != null && !m_TargetItem2.Deleted) 
					return m_TargetItem2.Name; 
				else 
					return null; 
			} 
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)2); // version
			// version 2
			writer.Write(m_Disabled);
			// version 1
			writer.Write(m_LinkedItem);
			// version 0
			writer.Write(this.m_LeverState);
			writer.Write(this.m_LeverSound);
			int ltype = (int)this.m_LeverType;
			writer.Write(ltype);
			writer.Write(this.m_TargetItem0);
			writer.Write(this.m_TargetProperty0);
			writer.Write(this.m_TargetItem1);
			writer.Write(this.m_TargetProperty1);
			writer.Write(this.m_TargetItem2);
			writer.Write(this.m_TargetProperty2);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch (version)
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
						this.m_LeverState = reader.ReadInt();
						this.m_LeverSound = reader.ReadInt();
						int ltype = reader.ReadInt();
						switch (ltype)
						{
							case (int)leverType.Two_State: this.m_LeverType = leverType.Two_State; break;
							case (int)leverType.Three_State: this.m_LeverType = leverType.Three_State; break;

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
		}

		public void SetLeverStatic()
		{

			switch (this.Direction)
			{
				case Direction.North:
				case Direction.South:
				case Direction.Right:
				case Direction.Up:
					if (m_LeverType == leverType.Two_State)
						this.ItemID = 0x108c + m_LeverState * 2;
					else
						this.ItemID = 0x108c + m_LeverState;
					break;
				case Direction.East:
				case Direction.West:
				case Direction.Left:
				case Direction.Down:
					if (m_LeverType == leverType.Two_State)
						this.ItemID = 0x1093 + m_LeverState * 2;
					else
						this.ItemID = 0x1093 + m_LeverState;
					break;
				default:
					break;
			}
		}

		public void Activate(Mobile from, int state, ArrayList links)
		{
			if (Disabled) return;

			string status_str = null;

			// assign the lever state
			m_LeverState = state;

			if (m_LeverState < 0) m_LeverState = 0;
			if (m_LeverState > 1 && m_LeverType == leverType.Two_State) m_LeverState = 1;
			if (m_LeverState > 2) m_LeverState = 2;

			// update the graphic
			SetLeverStatic();

			// play the switching sound if possible
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
			if (m_LeverState == 0 && m_TargetItem0 != null && !m_TargetItem0.Deleted && m_TargetProperty0 != null && m_TargetProperty0.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty0, m_TargetItem0, from, this, out status_str);
			}
			if (m_LeverState == 1 && m_TargetItem1 != null && !m_TargetItem1.Deleted && m_TargetProperty1 != null && m_TargetProperty1.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty1, m_TargetItem1, from, this, out status_str);
			}
			if (m_LeverState == 2 && m_TargetItem2 != null && !m_TargetItem2.Deleted && m_TargetProperty2 != null && m_TargetProperty2.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty2, m_TargetItem2, from, this, out status_str);
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

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null || Disabled) return;

			if (!from.InRange(GetWorldLocation(), 2) || !from.InLOS(this))
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}

			// change the switch state
			m_LeverState = m_LeverState + 1;

			if (m_LeverState > 1 && m_LeverType == leverType.Two_State) m_LeverState = 0;
			if (m_LeverState > 2) m_LeverState = 0;

			// carry out the switch actions
			Activate(from, m_LeverState, null);

		}
	}

	public class SimpleSwitch : Item, ILinkable
	{
		private int m_SwitchState = 0;
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

		[CommandProperty(AccessLevel.GameMaster)]
		public int SwitchState
		{
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
			get { return m_SwitchState; }
		}

		[Constructable]
		public SimpleSwitch()
			: base(0x108F)
		{
			Name = "A switch";
			Movable = false;
		}

		public SimpleSwitch(Serial serial)
			: base(serial)
		{
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int SwitchSound
		{
			get { return m_SwitchSound; }
			set
			{
				m_SwitchSound = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		new public virtual Direction Direction
		{
			get { return base.Direction; }
			set
			{
				base.Direction = value;
				SetSwitchStatic();
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Target0Item
		{
			get { return m_TargetItem0; }
			set
			{
				m_TargetItem0 = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Target0Property
		{
			get { return m_TargetProperty0; }
			set
			{
				m_TargetProperty0 = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Target0ItemName
		{
			get
			{
				if (m_TargetItem0 != null && !m_TargetItem0.Deleted)
					return m_TargetItem0.Name;
				else
					return null;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Target1Item
		{
			get { return m_TargetItem1; }
			set { m_TargetItem1 = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Target1Property
		{
			get { return m_TargetProperty1; }
			set
			{
				m_TargetProperty1 = value;
				InvalidateProperties();
			}
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Target1ItemName
		{
			get
			{
				if (m_TargetItem1 != null && !m_TargetItem1.Deleted)
					return m_TargetItem1.Name;
				else
					return null;
			}
		}


		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)2); // version
			// version 2
			writer.Write(this.m_Disabled);
			// version 1
			writer.Write(this.m_LinkedItem);
			// version 0
			writer.Write(this.m_SwitchState);
			writer.Write(this.m_SwitchSound);
			writer.Write(this.m_TargetItem0);
			writer.Write(this.m_TargetProperty0);
			writer.Write(this.m_TargetItem1);
			writer.Write(this.m_TargetProperty1);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch (version)
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
						this.m_SwitchState = reader.ReadInt();
						this.m_SwitchSound = reader.ReadInt();
						this.m_TargetItem0 = reader.ReadItem();
						this.m_TargetProperty0 = reader.ReadString();
						this.m_TargetItem1 = reader.ReadItem();
						this.m_TargetProperty1 = reader.ReadString();
					}
					break;
			}
		}

		public void SetSwitchStatic()
		{

			switch (this.Direction)
			{
				case Direction.North:
				case Direction.South:
				case Direction.Right:
				case Direction.Up:
					this.ItemID = 0x108f + m_SwitchState;
					break;
				case Direction.East:
				case Direction.West:
				case Direction.Left:
				case Direction.Down:
					this.ItemID = 0x1091 + m_SwitchState;
					break;
				default:
					this.ItemID = 0x108f + m_SwitchState;
					break;
			}
		}

		public void Activate(Mobile from, int state, ArrayList links)
		{
			if (Disabled) return;

			string status_str = null;

			// assign the switch state
			m_SwitchState = state;

			if (m_SwitchState < 0) m_SwitchState = 0;
			if (m_SwitchState > 1) m_SwitchState = 1;

			// update the graphic
			SetSwitchStatic();

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
			if (m_SwitchState == 0 && m_TargetItem0 != null && !m_TargetItem0.Deleted && m_TargetProperty0 != null && m_TargetProperty0.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty0, m_TargetItem0, from, this, out status_str);
			}

			if (m_SwitchState == 1 && m_TargetItem1 != null && !m_TargetItem1.Deleted && m_TargetProperty1 != null && m_TargetProperty1.Length > 0)
			{
				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty1, m_TargetItem1, from, this, out status_str);
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

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null || Disabled) return;

			if (!from.InRange(GetWorldLocation(), 2) || !from.InLOS(this))
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}

			// change the switch state
			m_SwitchState = m_SwitchState + 1;

			if (m_SwitchState > 1) m_SwitchState = 0;

			// activate the switch
			Activate(from, m_SwitchState, null);
		}
	}

	public class CombinationLock : Item
	{
		private int m_Combination = 0;
		private Item m_Digit0Object = null;
		private string m_Digit0Property = null;
		private Item m_Digit1Object = null;
		private string m_Digit1Property = null;
		private Item m_Digit2Object = null;
		private string m_Digit2Property = null;
		private Item m_Digit3Object = null;
		private string m_Digit3Property = null;
		private Item m_Digit4Object = null;
		private string m_Digit4Property = null;
		private Item m_Digit5Object = null;
		private string m_Digit5Property = null;
		private Item m_Digit6Object = null;
		private string m_Digit6Property = null;
		private Item m_Digit7Object = null;
		private string m_Digit7Property = null;
		private Item m_TargetItem = null;
		private string m_TargetProperty = null;
		private int m_CombinationSound = 940;

		[Constructable]
		public CombinationLock()
			: base(0x1BBF)
		{
			Name = "A combination lock";
			Movable = false;
		}

		public CombinationLock(Serial serial)
			: base(serial)
		{
		}

		public int SetDigit(int value)
		{
			if (value < 0) return 0;
			if (value > 9) return 9;
			return value;
		}

		public int CheckDigit(object o, string property)
		{
			if (o == null) return 0;
			if (property == null || property.Length <= 0) return (0);
			Type ptype;
			int ival = -1;
			string testvalue;
			// check to see whether this is a direct value request, or a test
			string[] argtest = BaseXmlSpawner.ParseString(property, 2, "<>!=");
			if (argtest.Length > 1)
			{
				// ok, its a test, so test it
				string status_str;
				if (BaseXmlSpawner.CheckPropertyString(null, o, property, null, out status_str))
				{
					return 1; // true
				}
				else
					return 0; // false
			}
			// otherwise get the value of the property requested
			string result = BaseXmlSpawner.GetPropertyValue(null, o, property, out ptype);

			string[] arglist = BaseXmlSpawner.ParseString(result, 2, "=");
			if (arglist.Length < 2) return -1;
			string[] arglist2 = BaseXmlSpawner.ParseString(arglist[1], 2, " ");
			if (arglist2.Length > 0)
			{
				testvalue = arglist2[0].Trim();
			}
			else
			{
				return -1;
			}

			if (BaseXmlSpawner.IsNumeric(ptype))
			{
				try
				{
					ival = Convert.ToInt32(testvalue, 10);
				}
				catch { }
			}
			return ival;
		}



		[CommandProperty(AccessLevel.GameMaster)]
		public int Combination
		{
			get { return m_Combination; }
			set
			{
				m_Combination = value;
				if (m_Combination < 0) m_Combination = 0;
				if (m_Combination > 99999999) m_Combination = 99999999;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Digit0Object
		{
			get { return m_Digit0Object; }
			set { m_Digit0Object = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Digit0Property
		{
			get { return m_Digit0Property; }
			set { m_Digit0Property = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Digit0
		{ get { return (CheckDigit(m_Digit0Object, m_Digit0Property)); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Digit1Object
		{
			get { return m_Digit1Object; }
			set { m_Digit1Object = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Digit1Property
		{
			get { return m_Digit1Property; }
			set { m_Digit1Property = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Digit1
		{ get { return (CheckDigit(m_Digit1Object, m_Digit1Property)); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Digit2Object
		{
			get { return m_Digit2Object; }
			set { m_Digit2Object = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Digit2Property
		{
			get { return m_Digit2Property; }
			set { m_Digit2Property = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Digit2
		{ get { return (CheckDigit(m_Digit2Object, m_Digit2Property)); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Digit3Object
		{
			get { return m_Digit3Object; }
			set { m_Digit3Object = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Digit3Property
		{
			get { return m_Digit3Property; }
			set { m_Digit3Property = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Digit3
		{ get { return (CheckDigit(m_Digit3Object, m_Digit3Property)); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Digit4Object
		{
			get { return m_Digit4Object; }
			set { m_Digit4Object = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Digit4Property
		{
			get { return m_Digit4Property; }
			set { m_Digit4Property = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Digit4
		{ get { return (CheckDigit(m_Digit4Object, m_Digit4Property)); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Digit5Object
		{
			get { return m_Digit5Object; }
			set { m_Digit5Object = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Digit5Property
		{
			get { return m_Digit5Property; }
			set { m_Digit5Property = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Digit5
		{ get { return (CheckDigit(m_Digit5Object, m_Digit5Property)); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Digit6Object
		{
			get { return m_Digit6Object; }
			set { m_Digit6Object = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Digit6Property
		{
			get { return m_Digit6Property; }
			set { m_Digit6Property = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Digit6
		{ get { return (CheckDigit(m_Digit6Object, m_Digit6Property)); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Digit7Object
		{
			get { return m_Digit7Object; }
			set { m_Digit7Object = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Digit7Property
		{
			get { return m_Digit7Property; }
			set { m_Digit7Property = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Digit7
		{ get { return (CheckDigit(m_Digit7Object, m_Digit7Property)); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Item TargetItem
		{
			get { return m_TargetItem; }
			set { m_TargetItem = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string TargetProperty
		{
			get { return m_TargetProperty; }
			set { m_TargetProperty = value; InvalidateProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string TargetItemName
		{ get { if (m_TargetItem != null && !m_TargetItem.Deleted) return m_TargetItem.Name; else return null; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int CombinationSound
		{
			get { return m_CombinationSound; }
			set
			{
				m_CombinationSound = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Matched
		{
			get { return (m_Combination == CurrentValue); }
		}
		[CommandProperty(AccessLevel.GameMaster)]

		public int CurrentValue
		{
			get
			{
				int value = Digit0 + Digit1 * 10 + Digit2 * 100 + Digit3 * 1000 + Digit4 * 10000 + Digit5 * 100000 + Digit6 * 1000000 + Digit7 * 10000000;
				return value;
			}
		}


		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version

			writer.Write(this.m_Combination);
			writer.Write(this.m_CombinationSound);
			writer.Write(this.m_Digit0Object);
			writer.Write(this.m_Digit0Property);
			writer.Write(this.m_Digit1Object);
			writer.Write(this.m_Digit1Property);
			writer.Write(this.m_Digit2Object);
			writer.Write(this.m_Digit2Property);
			writer.Write(this.m_Digit3Object);
			writer.Write(this.m_Digit3Property);
			writer.Write(this.m_Digit4Object);
			writer.Write(this.m_Digit4Property);
			writer.Write(this.m_Digit5Object);
			writer.Write(this.m_Digit5Property);
			writer.Write(this.m_Digit6Object);
			writer.Write(this.m_Digit6Property);
			writer.Write(this.m_Digit7Object);
			writer.Write(this.m_Digit7Property);
			writer.Write(this.m_TargetItem);
			writer.Write(this.m_TargetProperty);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch (version)
			{
				case 0:
					{
						this.m_Combination = reader.ReadInt();
						this.m_CombinationSound = reader.ReadInt();
						this.m_Digit0Object = reader.ReadItem();
						this.m_Digit0Property = reader.ReadString();
						this.m_Digit1Object = reader.ReadItem();
						this.m_Digit1Property = reader.ReadString();
						this.m_Digit2Object = reader.ReadItem();
						this.m_Digit2Property = reader.ReadString();
						this.m_Digit3Object = reader.ReadItem();
						this.m_Digit3Property = reader.ReadString();
						this.m_Digit4Object = reader.ReadItem();
						this.m_Digit4Property = reader.ReadString();
						this.m_Digit5Object = reader.ReadItem();
						this.m_Digit5Property = reader.ReadString();
						this.m_Digit6Object = reader.ReadItem();
						this.m_Digit6Property = reader.ReadString();
						this.m_Digit7Object = reader.ReadItem();
						this.m_Digit7Property = reader.ReadString();
						this.m_TargetItem = reader.ReadItem();
						this.m_TargetProperty = reader.ReadString();

					}
					break;
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null) return;

			if (!from.InRange(GetWorldLocation(), 2) || !from.InLOS(this))
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}
			string status_str;
			// test the combination and apply the property to the target item
			if (Matched)
			{
				//from.PlaySound(m_CombinationSound);
				try
				{
					Effects.PlaySound(Location, Map, m_CombinationSound);
				}
				catch { }

				BaseXmlSpawner.ApplyObjectStringProperties(null, m_TargetProperty, m_TargetItem, from, this, out status_str);

			}

		}
	}
}
