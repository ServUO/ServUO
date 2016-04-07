using Server;
using System;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Targeting;
using Server.Network;

namespace Server.Engines.Despise
{
	public enum LeashLength
	{
		Tight,
		Short,
		Long
	}
		
	public enum Aggression
	{
		Following,
		Defensive, 
		Aggressive
	}
		
	public class WispOrb : Item
	{
		public override int LabelNumber { get { return 1153273; } } // A Wisp Orb
		
		private static readonly int MinPowerToConscript = 4;
		
		private Mobile m_Owner;
		private DespiseCreature m_Pet;
		private LeashLength m_LeashLength;
		private Aggression m_Aggression;
		private Alignment m_Alignment;
		private IPoint3D m_Anchor;
		private bool m_Conscripted;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner 
		{ 
			get { return m_Owner; } 
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public DespiseCreature Pet 
		{ 
			get { return m_Pet; } 
			set 
			{ 
				if(m_Pet != null && value == null)
				{
					m_Pet.Unlink(); 
					//m_Pet = null;
				}
				else
				{
					m_Pet = value; 
					m_Pet.Link(this);
				}

                InvalidateHue();
                InvalidateProperties();
			} 
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public LeashLength LeashLength 
		{ 
			get { return m_LeashLength; } 
			set 
			{ 
				m_LeashLength = value;

                if (m_Pet != null)
                    m_Pet.RangeHome = m_Pet.GetLeashLength();

				InvalidateHue();
                InvalidateProperties();
			} 
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Aggression Aggression 
		{ 
			get { return m_Aggression; } 
			set 
			{
				if(value != m_Aggression)
				{
					m_Aggression = value;

                    if (m_Pet != null)
                    {
                        if (m_Anchor != m_Pet.ControlMaster)
                            m_Anchor = m_Pet.ControlMaster;
                        m_Pet.ControlTarget = m_Pet.ControlMaster;
                        m_Pet.Home = Point3D.Zero;
                        switch (m_Aggression)
                        {
                            case Aggression.Following:
                                m_Pet.ControlOrder = OrderType.Follow;
                                m_Pet.AIObject.Action = ActionType.Backoff;
                                break;
                            case Aggression.Defensive:
                                m_Pet.ControlOrder = OrderType.Guard;
                                m_Pet.AIObject.Action = ActionType.Wander;
                                break;
                            case Aggression.Aggressive:
                                m_Pet.ControlOrder = OrderType.Guard;
                                m_Pet.AIObject.Action = ActionType.Wander;
                                break;
                        }
                    }
				}

				InvalidateHue();
                InvalidateProperties();
			} 
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Alignment Alignment
		{ 
			get { return m_Alignment; } 
			set { m_Alignment = value; InvalidateProperties(); } 
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public IPoint3D Anchor
		{ 
			get { return m_Anchor; } 
			set 
            { 
                m_Anchor = value;

                if (m_Pet != null && m_Anchor == null)
                    m_Pet.Home = Point3D.Zero;

                InvalidateProperties(); 
            } 
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Conscripted
		{ 
			get { return m_Conscripted; } 
			set 
            { 
                m_Conscripted = value;

                if (m_Conscripted && DespiseController.Instance != null && DespiseController.Instance.Sequencing)
                    DespiseController.Instance.TryAddToArmy(this);
            } 
		}
		
		public WispOrb(Mobile owner, Alignment alignment) : base(8448)
		{
			m_Owner = owner;
			LootType = LootType.Blessed;
			m_Alignment = alignment;
			
			m_Orbs.Add(this);
            InvalidateHue();
		}

        public void OnUnlinkPet()
        {
            m_Pet = null;
            m_Anchor = null;
            m_Aggression = Aggression.Following;
            InvalidateProperties();
        }
		
		public bool CheckOwnerAlignment()
		{
			if(m_Owner == null || (m_Owner.Karma > 0 && m_Alignment != Alignment.Good) || (m_Owner.Karma < 0 && m_Alignment != Alignment.Evil))
			{
				if(m_Owner != null)
					m_Owner.SendLocalizedMessage(1153313); // You are no longer aligned with your Wisp Orb. It dissolves into aether!
					
				Delete();
				return false;
			}
			
			return true;
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if(CheckOwnerAlignment() && IsChildOf(from.Backpack) && from == m_Owner)
			{
				int cliloc = m_Pet == null ? 1153274 : 1153277;
				from.SendLocalizedMessage(cliloc); // Target a creature to possess. / Target an object or creature to set the anchor. Target the Wisp Orb to change the leash setting. Target the possessed creature to change its aggression.
				from.Target = new InternalTarget(this);
			}
		}
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries(from, list);

            if (CheckOwnerAlignment() && m_Pet != null && IsChildOf(from.Backpack) && from == m_Owner)
			{
				list.Add(new ConscriptEntry(from, this));
				list.Add(new ReleaseEntry(from, this));
			}
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			
			list.Add(1153329, String.Format("#{0}", GetAlignment())); // Alignment: ~1_VAL~
			list.Add(1153306, String.Format("{0}", GetArmyPower())); // Army Power: ~1_VAL~
			
			if(m_Pet != null)
				list.Add(1153272, m_Pet.Name); // Controlling: ~1_VAL~
			
			object name = GetAnchorName();
			
			if(name != null)					//Anchor: ~1_NAME~
			{
				if(name is int)
					list.Add(1153265, String.Format("#{0}", (int)name));
				else if(name is string)
					list.Add(1153265, (string)name);
			}
			
			int leash = 1153261 + (int)m_LeashLength;
			int aggr = 1153268 + (int)m_Aggression;
			
			list.Add(1153260, String.Format("#{0}", leash.ToString())); // Leash: ~1_VAL~
			list.Add(1153267, String.Format("#{0}", aggr.ToString())); // Aggression: ~1_VAL~
		}
		
		private class ConscriptEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private WispOrb m_Orb;
			
			public ConscriptEntry(Mobile from, WispOrb orb) : base(1153285, -1) // Conscript
			{
				m_From = from;
				m_Orb = orb;

                if (m_Orb.Pet == null || m_Orb.Conscripted || m_Orb.Pet.Alignment != m_Orb.Alignment)
					Flags |= Server.Network.CMEFlags.Disabled;
			}
			
			public override void OnClick()
			{
                if (m_Orb.Pet != null && m_Orb.IsChildOf(m_From.Backpack) && !m_Orb.Conscripted && m_Orb.Pet.Alignment == m_Orb.Alignment)
				{
					if(m_Orb.Pet.Power < WispOrb.MinPowerToConscript)
						m_From.SendLocalizedMessage(1153311); // The creature under control of your Wisp Orb cannot be conscripted at this time.
					else
					{
						m_From.SendLocalizedMessage(1153310); // The creature you are controlling will now fight with you when the Call to Arms sounds. If you do not wish this, then release control of it.
						m_Orb.Conscripted = true;
					}
				}
			}
		}
		
		private class ReleaseEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private WispOrb m_Orb;
			
			public ReleaseEntry(Mobile from, WispOrb orb) : base(1153284, -1) // Release
			{
				m_From = from;
				m_Orb = orb;
				
				if(m_Orb.Pet == null/* || !m_Orb.Conscripted*/)
					Flags |= Server.Network.CMEFlags.Disabled;
			}
			
			public override void OnClick()
			{
                /*if (m_Orb.Pet != null && m_Orb.IsChildOf(m_From.Backpack) && m_Orb.Conscripted)
                {
                    m_From.SendMessage("The creature you are controlling is no longer conscripted.");
                    m_Orb.Conscripted = false;
                }*/

                if (m_Orb.Pet != null)
                {
                    m_Orb.Pet.Unlink();
                }
			}
		}
		
		private class InternalTarget : Target
		{
			private WispOrb m_Orb;
			
			public InternalTarget(WispOrb orb) : base(3, true, TargetFlags.None)
			{
				m_Orb = orb;
			}
			
			protected override void OnTarget(Mobile from, object targeted)
			{
				if(targeted is BaseCreature)
				{
					DespiseCreature creature = targeted as DespiseCreature;

                    if (creature == null)
                        from.SendLocalizedMessage(1153286); // That cannot be possessed by a Wisp Orb.
                    else if (m_Orb.Pet == null)
                    {
                        if (((BaseCreature)targeted).Controlled)
                            from.SendLocalizedMessage(1153287); // That creature is already under the control of a Wisp Orb.
                        else if (creature.Power > 3)
                            from.SendLocalizedMessage(1153336); // That creature is too powerful for you to coerce.
                        else
                        {
                            m_Orb.Anchor = null;

                            m_Orb.Pet = creature;
                            creature.Power = 1;
                            creature.Link(m_Orb);

                            m_Orb.Pet.SetControlMaster(from);
                            m_Orb.Pet.ControlTarget = from;
                            m_Orb.Pet.ControlOrder = OrderType.Follow;

                            from.SendLocalizedMessage(1153276); // Your Wisp Orb takes control of the creature!
                            m_Orb.Pet.PublicOverheadMessage(MessageType.Regular, 0x59, 1153295, from.Name); // * This creature is now under the control of ~1_NAME~ *
                        }
                    }
                    else if (targeted == m_Orb.Pet)
                    {
                        int aggr = (int)m_Orb.Aggression + 1;
                        if (aggr >= 3) aggr = 0;

                        m_Orb.Aggression = (Aggression)aggr;

                        from.SendLocalizedMessage(1153279, m_Orb.Aggression.ToString()); // Your possessed creature's aggression level is now: ~1_VAL~
                    }
                    else
                        from.SendLocalizedMessage(1153357); // Thou can guide but one of us. 
				}
				else if (targeted == m_Orb)
				{
					int length = (int)m_Orb.LeashLength + 1;
					if(length >= 3) length = 0;

                    m_Orb.LeashLength = (LeashLength)length;
					
					from.SendLocalizedMessage(1153278, m_Orb.LeashLength.ToString()); // Your possessed creature's leash is now: ~1_VAL~
				}			
				else if(targeted is IPoint3D && m_Orb.Pet != null)
				{
					m_Orb.TrySetAnchor(from, (IPoint3D)targeted);
				}
			}
		}
		
		private object GetAnchorName()
		{
			if(m_Anchor == null)
				return "None";
				
			if(m_Anchor is Mobile)
				return ((Mobile)m_Anchor).Name;
			
			if(m_Anchor is Item)
			{
				if(((Item)m_Anchor).Name != null)
					return ((Item)m_Anchor).Name;
				
				return ((Item)m_Anchor).LabelNumber;
			}
			
			if(m_Anchor is StaticTarget)
				return String.Format("{0} {1}", ((StaticTarget)m_Anchor).Name, ((StaticTarget)m_Anchor).Location.ToString());
				
			if(m_Anchor is LandTarget)
				return String.Format("{0} {1}", ((LandTarget)m_Anchor).Name, ((LandTarget)m_Anchor).Location.ToString());
				
			Point3D p = new Point3D(m_Anchor);
				
			return p.ToString();
		}
		
		public void TrySetAnchor(Mobile from, IPoint3D p)
		{
            if (!CheckOwnerAlignment() || from != m_Owner)
				return;
				
			if(p is Item && (((Item)p).RootParent != null || ((Item)p).Movable))
			{
				from.SendMessage("That wouldn't be a suitable anchor.");
				return;
			}

            if (p is Mobile)
            {
                Mobile m = p as Mobile;

                Anchor = m;
                from.SendLocalizedMessage(1153280, m == m_Owner ? "You!" : m.Name + ".");

                m_Pet.ControlTarget = m;
                m_Pet.ControlOrder = OrderType.Follow;

                return;
            }

            Point3D point = new Point3D(p);
            bool success = false;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int xx = point.X + x;
                    int yy = point.Y + y;
                    Point3D newPoint = new Point3D(xx, yy, from.Map.GetAverageZ(xx, yy));

                    MovementPath path = new MovementPath(m_Pet, newPoint);

                    if (path.Success)
                    {
                        success = true;
                        break;
                    }
                }

                if (success)
                    break;
            }

			if(success)
			{
				Anchor = p;
				
				object name = GetAnchorName(); // Your possessed creature is now anchored to ~1_NAME~
				
				if(name is int)
					from.SendLocalizedMessage(1153280, String.Format("#{0}", (int) name)); 
				else if (name is string)
					from.SendLocalizedMessage(1153280, (string)name);

                m_Pet.ControlOrder = OrderType.None;
                m_Pet.CurrentSpeed = m_Pet.ActiveSpeed;
                m_Pet.Home = new Point3D(p);
			}
			else
				from.SendMessage("There must be a clear path to set an anchor.");
		}

        private int GetAlignment()
        {
            switch (m_Alignment)
            {
                case Alignment.Neutral: break;
                case Alignment.Good: return 1153330;
                case Alignment.Evil: return 1153331;
            }

            return -1;
        }

        public void InvalidateHue()
        {
            if (m_Pet == null)
                Hue = 1943; // shadow wisp color
            else if (m_Pet.Combatant != null)
                Hue = 1931; // Orange
            else
            {
                switch (m_Aggression)
                {
                    case Aggression.Following: Hue = 1923; break; // Yellow
                    case Aggression.Defensive: Hue = 1927; break; // blue
                    case Aggression.Aggressive: Hue = 1925; break;  // green
                }
            }
        }
		
		public override void Delete()
		{
			if(m_Orbs.Contains(this))
				m_Orbs.Remove(this);

            if (m_Pet != null && m_Pet.Alive)
				m_Pet.Unlink();
				
			base.Delete();
		}
		
		public int GetArmyPower()
		{
            if (m_Pet == null)
                return 0;

			int power = ((DespiseCreature)m_Pet).Power;
			return power * power;
		}
		
		public static void TeleportPet(Mobile owner)
		{
			if(owner == null || owner.Backpack == null)
				return;
				
			Item item = owner.Backpack.FindItemByType(typeof(WispOrb));
			
			if(item != null && item is WispOrb)
			{
				Mobile pet = ((WispOrb)item).Pet;
				
				if(pet != null)
					pet.MoveToWorld(owner.Location, owner.Map);
			}
		}
	
		public WispOrb(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			writer.Write(m_Owner);
			writer.Write(m_Pet);
			writer.Write((int)m_LeashLength);
			writer.Write((int)m_Aggression);
			writer.Write((int)m_Alignment);
            writer.Write(m_Conscripted);

            if (m_Anchor == null)
                writer.Write((int)0);
            else if (m_Anchor is Mobile)
            {
                writer.Write((int)1);
                writer.Write((Mobile)m_Anchor);
            }
            else
            {
                writer.Write((int)2);
                writer.Write(new Point3D(m_Anchor));
            }
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			
			m_Owner = reader.ReadMobile();
			m_Pet = reader.ReadMobile() as DespiseCreature;
			m_LeashLength = (LeashLength)reader.ReadInt();
			m_Aggression = (Aggression)reader.ReadInt();
			m_Alignment = (Alignment)reader.ReadInt();
            m_Conscripted = reader.ReadBool();

            switch (reader.ReadInt())
            {
                case 0: break;
                case 1: m_Anchor = (IPoint3D)reader.ReadMobile(); break;
                case 2: m_Anchor = (IPoint3D)reader.ReadPoint3D(); break;
            }

            if (m_Anchor == null && m_Pet != null)
                m_Pet.Home = m_Pet.Location;

			m_Orbs.Add(this);
		}
		
		private static List<WispOrb> m_Orbs = new List<WispOrb>();
		public static List<WispOrb> Orbs { get { return m_Orbs; } }
	}
}