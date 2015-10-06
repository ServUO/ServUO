using Server;
using System;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public enum BinderType
	{
		None,
		StatScroll,
		PowerScroll,
		SOT
	}

	public class ScrollBinderDeed : Item
	{
		private BinderType m_BinderType;
		private SkillName m_Skill;
		private double m_Value;
		private int m_Needed;
		private double m_Has;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public BinderType BinderType { get { return m_BinderType; } set { m_BinderType = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public SkillName Skill { get { return m_Skill; } set { m_Skill = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public double Value { get { return m_Value; } set { m_Value = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int Needed { get { return m_Needed; } set { m_Needed = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public double Has { get { return m_Has; } set { m_Has = value; } }
		
		public override int LabelNumber { get { return 1113135; } } //Scroll Binder
		
		[Constructable]
		public ScrollBinderDeed() : base(0x14F0)
		{
			m_BinderType = BinderType.None;
			m_Skill = SkillName.Alchemy;
			m_Value = 0;
			m_Needed = 0;
			m_Has = 0;
			
			LootType = LootType.Cursed;
            Hue = 1641;
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

            int v = (int)m_Value;

			switch(m_BinderType)
			{
				case BinderType.None: break;
				case BinderType.PowerScroll:
					list.Add(1113149, String.Format("{0}\t#{1}\t{2}\t{3}", v.ToString(), (1044060 + (int)m_Skill).ToString(), ((int)m_Has).ToString(), m_Needed.ToString())); //~1_bonus~ ~2_type~: ~3_given~/~4_needed~
					break;
				case BinderType.StatScroll:
                    list.Add(1113149, String.Format("+{0}\t#{1}\t{2}\t{3}", v - 225, 1049477, ((int)m_Has).ToString(), m_Needed.ToString()));
					break;
				case BinderType.SOT:
                    string value = String.Format("{0:0.##}", m_Has);
                    list.Add(1113620, String.Format("#{0}\t{1}", (1044060 + (int)m_Skill).ToString(), value));
					break;
			}
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if(IsChildOf(from.Backpack))
			{
				from.Target = new InternalTarget(this);

                int cliloc;
                switch (m_BinderType)
                {
                    default:
                    case BinderType.None:
                        cliloc = 1113141; break;
                    case BinderType.PowerScroll:
                        cliloc = 1113138; break;
                    case BinderType.StatScroll:
                        cliloc = 1113140; break;
                    case BinderType.SOT:
                        cliloc = 1113139; break;
                }

                from.SendLocalizedMessage(cliloc); //Target the scroll you wish to bind.
			}
		}
		
		public void OnTarget(Mobile from, object targeted)
		{
            if(targeted is Item && !((Item)targeted).IsChildOf(from.Backpack))
            {
                from.SendMessage("The scroll must be in your backpack to bind.");
                return;
            }

			switch(m_BinderType)
			{
				case BinderType.None:
					{
						if(targeted is PowerScroll)
						{
							PowerScroll ps = (PowerScroll)targeted;
							
							if(ps.Value >= 120)
							    from.SendLocalizedMessage(1113144); //This scroll is already the highest of its type and cannot be bound.
							else
							{
								double value = ps.Value;
                                int needed = 0;
								if(value == 105) needed = 8;
								else if(value == 110) needed = 12;
								else if(value == 115) needed = 10;
								else
									return;
								
								m_Value = value;
								m_Needed = needed;
                                m_Has = 1;
								m_Skill = ps.Skill;
								m_BinderType = BinderType.PowerScroll;
								ps.Delete();
                                from.SendMessage("Binding Powerscroll.");
                                from.PlaySound(0x249);
							}
						}
						else if (targeted is StatCapScroll)
						{
							StatCapScroll ps = (StatCapScroll)targeted;
							
							if(ps.Value >= 250)
							    from.SendLocalizedMessage(1113144); //This scroll is already the highest of its type and cannot be bound.
							else
							{
								double value = ps.Value;
								int needed = 0;
								if(value == 230) needed = 6;
								else if(value == 235) needed = 8;
								else if(value == 240) needed = 8;
								else if(value == 245) needed = 5;
								else
									return;
								
								m_Value = value;
								m_Needed = needed;
                                m_Has = 1;
								m_BinderType = BinderType.StatScroll;
								ps.Delete();
                                from.SendMessage("Binding Stat Scroll.");
                                from.PlaySound(0x249);
							}
						}
						else if (targeted is ScrollofTranscendence)
						{
							ScrollofTranscendence sot = (ScrollofTranscendence)targeted;
							
							m_Skill = sot.Skill;
							m_BinderType = BinderType.SOT;
							m_Needed = 5;
                            m_Has = sot.Value;
							sot.Delete();
                            from.SendLocalizedMessage(1113146); //Binding Scrolls of Transcendence
                            from.PlaySound(0x249);
						}
						else
							from.SendLocalizedMessage(1113142); //You may only bind powerscrolls, stats scrolls or scrolls of transcendence.
							
						break;
					}
				case BinderType.PowerScroll:
					{
						if(targeted is PowerScroll)
						{
							PowerScroll ps = (PowerScroll)targeted;
							
							if(ps.Value == m_Value)
							{
								if(ps.Skill ==  m_Skill)
								{
									m_Has++;
									
									if(m_Has >= m_Needed)
									{
										GiveItem(from, new PowerScroll(m_Skill, m_Value + 5));
										from.SendLocalizedMessage(1113145); //You've completed your binding and received an upgraded version of your scroll!
										ps.Delete();
										Delete();
									}
									else
									{
										ps.Delete();
                                        from.PlaySound(0x249);
										from.SendMessage("Binding Powerscroll.");
									}
								}
								else
                                    from.SendLocalizedMessage(1113143); //This scroll does not match the type currently being bound.
							}
							else
								from.SendLocalizedMessage(1113143); //This scroll does not match the type currently being bound.
						}
						else
							from.SendLocalizedMessage(1113143); //This scroll does not match the type currently being bound.
						break;
					}
				case BinderType.StatScroll:
					{
						if(targeted is StatCapScroll)
						{
							StatCapScroll stat = (StatCapScroll)targeted;
							
							if(stat.Value == m_Value)
							{
								m_Has++;
								
								if(m_Has >= m_Needed)
								{
									GiveItem(from, new StatCapScroll((int)m_Value + 5));
									from.SendLocalizedMessage(1113145); //You've completed your binding and received an upgraded version of your scroll!
									stat.Delete();
									Delete();
								}
								else
								{
									from.SendMessage("Binding Stat Scroll.");
                                    from.PlaySound(0x249);
									stat.Delete();
								}
							}
							else
								from.SendLocalizedMessage(1113143); //This scroll does not match the type currently being bound.
						}
						else
						    from.SendLocalizedMessage(1113143); //This scroll does not match the type currently being bound.
                        break;
					}
				case BinderType.SOT:
					{
						if(targeted is ScrollofTranscendence)
						{
							ScrollofTranscendence sot = (ScrollofTranscendence)targeted;
							
							if(sot.Skill == m_Skill)
							{
								double newValue = sot.Value + m_Has;
								
								if(newValue == m_Needed)
								{
									GiveItem(from, new ScrollofTranscendence(m_Skill, m_Needed));
									from.SendLocalizedMessage(1113145); //You've completed your binding and received an upgraded version of your scroll!
									Delete();
								}
								else if (newValue > m_Needed)
								{
									from.SendGump(new BinderWarningGump(newValue, this, sot, m_Needed));
								}
								else
								{
									m_Has += sot.Value;
									sot.Delete();
                                    from.PlaySound(0x249);
                                    from.SendLocalizedMessage(1113146); //Binding Scrolls of Transcendence
								}
							}
							else
								from.SendLocalizedMessage(1113143); //This scroll does not match the type currently being bound.
						}
                        else
                            from.SendLocalizedMessage(1113143); //This scroll does not match the type currently being bound.
                        break;
					}
			}

            InvalidateProperties();
		}
		
		public void GiveItem(Mobile from, Item item)
		{
			Container pack = from.Backpack;
			
			if(pack == null || !pack.TryDropItem(from, item, false))
				item.MoveToWorld(from.Location, from.Map);
		}
		
		private class InternalTarget : Target
		{
			private ScrollBinderDeed m_Binder;
			
			public InternalTarget(ScrollBinderDeed binder) : base(-1, false, TargetFlags.None)
			{
                m_Binder = binder;
			}
			
			protected override void OnTarget(Mobile from, object targeted)
			{
				if(m_Binder != null && !m_Binder.Deleted && m_Binder.IsChildOf(from.Backpack))
					m_Binder.OnTarget(from, targeted);
			}
		}
		
		private class BinderWarningGump : BaseConfirmGump
		{
			private double m_Value;
			private int m_Needed;
			private ScrollofTranscendence m_Scroll;
			private ScrollBinderDeed m_Binder;
			
			//public override int TitleNumber{ get{ return 1075083; } }
            public override int LabelNumber { get { return 1113147; } }
			
			public BinderWarningGump(double value, ScrollBinderDeed binder, ScrollofTranscendence scroll, int needed)
			{
				m_Value = value;
				m_Needed = needed;
				m_Scroll = scroll;
				m_Binder = binder;
			}
			
			public override void Confirm( Mobile from )
			{		
				if(m_Scroll != null && m_Binder != null)
				{
					m_Binder.GiveItem(from, new ScrollofTranscendence(m_Scroll.Skill, m_Needed));
					m_Scroll.Delete();
					m_Binder.Delete();
                    from.PlaySound(0x249);
                    from.SendLocalizedMessage(1113145); //You've completed your binding and received an upgraded version of your scroll!
				}
			}
		
			public override void Refuse( Mobile from )
			{
			}
		}
		
		public ScrollBinderDeed(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)2);

            writer.Write((int)m_BinderType);
            writer.Write((int)m_Skill);
            writer.Write(m_Value);
            writer.Write(m_Needed);
            writer.Write(m_Has);

            /*            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)m_skillname);
            writer.Write((double)m_skillvalue);
            writer.Write((double)m_maxneeded);
            writer.Write((double)m_count);

            // below - write the switch controller for the refference type
            if (check != null)
            {
                if (check is PowerScroll)
                    writer.Write((int)1);
                if (check is StatCapScroll)
                    writer.Write((int)2);
                if (check is ScrollofTranscendence)
                    writer.Write((int)3);
            }
            else
                writer.Write((int)0);*/
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();

            if (v < 2)
            {
                m_Skill = (SkillName)reader.ReadInt();
                m_Value = reader.ReadDouble();
                m_Needed = (int)reader.ReadDouble();
                m_Has = reader.ReadDouble();

                switch (reader.ReadInt())
                {
                    case 0: m_BinderType = BinderType.None; break;
                    case 1: m_BinderType = BinderType.PowerScroll; break;
                    case 2: m_BinderType = BinderType.StatScroll; break;
                    case 3: m_BinderType = BinderType.SOT; break;
                }
            }
            else
            {
                m_BinderType = (BinderType)reader.ReadInt();
                m_Skill = (SkillName)reader.ReadInt();
                m_Value = reader.ReadDouble();
                m_Needed = reader.ReadInt();
                m_Has = reader.ReadDouble();
            }

            /*base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    {
                        m_skillname = (SkillName)reader.ReadInt();
                        m_skillvalue = reader.ReadDouble();
                        m_maxneeded = reader.ReadDouble();
                        m_count = reader.ReadDouble();

                        //below - didn't waste time looking for how to serialize an invisible item and recall it
                        // so used an embedded switch system to create a new comparable refference
                        int i = reader.ReadInt();
                        if (i != 0)
                        {
                            switch (i)
                            {
                                case 1:
                                    {
                                        check = new PowerScroll(m_skillname, m_skillvalue);
                                        break;
                                    }
                                case 2:
                                    {
                                        check = new StatCapScroll(Convert.ToInt32(m_skillvalue));
                                        break;
                                    }
                                case 3:
                                    {
                                        check = new ScrollofTranscendence(m_skillname, m_skillvalue);
                                        break;
                                    }
                            }

                        }
                    }
                    goto case 0;
                case 0:
                    { break; }

            }
            InvalidateProperties();*/
		}
	}
}