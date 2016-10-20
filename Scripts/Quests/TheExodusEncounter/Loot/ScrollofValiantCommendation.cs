using System;
using Server.Network;
using Server.Items;
using Server.Gumps;

namespace Server.Items
{
    public class ScrollofValiantCommendation : Item
    {      
		private int m_StatBonus = 5;      
		public bool GumpOpen = false;
				     
        [CommandProperty( AccessLevel.GameMaster )] 
        public int StatBonus
        {
	        get { return m_StatBonus; }
	        set { m_StatBonus = value; }
        }

        [Constructable]
        public ScrollofValiantCommendation(int StatBonus) : base(0x46AE)
        {
			m_StatBonus = StatBonus;
            this.Weight = 1;
      	}
      
        [Constructable]
        public ScrollofValiantCommendation() : base(0x46AE)
        {
            this.Weight = 1;
        }

        public ScrollofValiantCommendation(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1153521;
            }
        }// Scroll of Valiant Commendation [Replica]

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1152708); // Double click to permanently gain +5 to your maximum stats 
        }

        public override void OnDoubleClick( Mobile from )
        {
	        if ( (this.StatBonus == 0) && (from.AccessLevel < AccessLevel.GameMaster) )
            {
                from.SendMessage("This Scroll of Valiant Commendation isn't working. Please page for a GM.");
	            return;
	        }
	        else if ( (from.AccessLevel >= AccessLevel.GameMaster) && (this.StatBonus == 0) )
            {
	            from.SendGump( new PropertiesGump( from, this ) );
	            return;
	        }

	        if ( !IsChildOf( from.Backpack ) ) 
	            from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
	        else if (!GumpOpen)
            {
 	            GumpOpen = true;
                from.SendGump(new ScrollofValiantCommendationGump(from, this));
	        }
	        else if (GumpOpen)
                from.SendMessage("You're already using the Scroll of Valiant Commendation.");
        }

        public override void Serialize( GenericWriter writer )
        {
	        base.Serialize( writer );
	        writer.Write( (int) 0 ); // version
	        writer.Write( m_StatBonus );
        }

        public override void Deserialize( GenericReader reader )
        {
	        base.Deserialize( reader );

	        int version = reader.ReadInt();

	        switch (version)
            {
	            case 0 : 
                    {
	                    m_StatBonus = reader.ReadInt();
	                    break;
	                }
	        }
        }
    }
}

namespace Server.Gumps
{
    public class ScrollofValiantCommendationGump : Gump
    {
        private ScrollofValiantCommendation m_svc;

        public ScrollofValiantCommendationGump(Mobile from, ScrollofValiantCommendation svc) : base(20, 30)
        {
            m_svc = svc;

            AddPage(0);
            AddBackground(0, 0, 260, 115, 5054);

            AddImageTiled(10, 10, 240, 23, 0x52);
            AddImageTiled(11, 11, 238, 21, 0xBBC);

            AddLabel(65, 11, 0, "Stats you can raise");

            int Strength = from.RawStr; // (sic!)
            int Dexterity = from.RawDex;
            int Intelligence = from.RawInt;

            if ((Strength + m_svc.StatBonus) <= 350)
            {
                AddImageTiled(10, 32 + (0 * 22), 240, 23, 0x52);
                AddImageTiled(11, 33 + (0 * 22), 238, 21, 0xBBC);

                AddLabelCropped(13, 33 + (0 * 22), 150, 21, 0, "Strength");
                AddImageTiled(180, 34 + (0 * 22), 50, 19, 0x52);
                AddImageTiled(181, 35 + (0 * 22), 48, 17, 0xBBC);
                AddLabelCropped(182, 35 + (0 * 22), 234, 21, 0, Strength.ToString("F1"));

                if (from.AccessLevel >= AccessLevel.Player)
                    AddButton(231, 35 + (0 * 22), 0x15E1, 0x15E5, 1, GumpButtonType.Reply, 0);
                else
                    AddImage(231, 35 + (0 * 22), 0x2622);
            }

            if ((Dexterity + m_svc.StatBonus) <= 350)
            {
                AddImageTiled(10, 32 + (1 * 22), 240, 23, 0x52);
                AddImageTiled(11, 33 + (1 * 22), 238, 21, 0xBBC);

                AddLabelCropped(13, 33 + (1 * 22), 150, 21, 0, "Dexterity");
                AddImageTiled(180, 34 + (1 * 22), 50, 19, 0x52);
                AddImageTiled(181, 35 + (1 * 22), 48, 17, 0xBBC);
                AddLabelCropped(182, 35 + (1 * 22), 234, 21, 0, Dexterity.ToString("F1"));

                if (from.AccessLevel >= AccessLevel.Player)
                    AddButton(231, 35 + (1 * 22), 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0);
                else
                    AddImage(231, 35 + (1 * 22), 0x2622);
            }

            if ((Intelligence + m_svc.StatBonus) <= 350)
            {
                AddImageTiled(10, 32 + (2 * 22), 240, 23, 0x52);
                AddImageTiled(11, 33 + (2 * 22), 238, 21, 0xBBC);

                AddLabelCropped(13, 33 + (2 * 22), 150, 21, 0, "Intelligence");
                AddImageTiled(180, 34 + (2 * 22), 50, 19, 0x52);
                AddImageTiled(181, 35 + (2 * 22), 48, 17, 0xBBC);
                AddLabelCropped(182, 35 + (2 * 22), 234, 21, 0, Intelligence.ToString("F1"));

                if (from.AccessLevel >= AccessLevel.Player)
                    AddButton(231, 35 + (2 * 22), 0x15E1, 0x15E5, 3, GumpButtonType.Reply, 0);
                else
                    AddImage(231, 35 + (2 * 22), 0x2622);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if ((from == null) || (m_svc.Deleted))
                return;

            m_svc.GumpOpen = false;

            if (info.ButtonID > 0)
            {

                int count = 0;
                count = from.RawStr + from.RawDex + from.RawInt;

                if ((count + m_svc.StatBonus) > (from.StatCap))
                {
                    from.SendMessage("You cannot exceed the Stat cap.");
                    return;
                }

                switch (info.ButtonID)
                {
                    case 1:
                        {
                            if (from.RawStr + m_svc.StatBonus <= 350)
                            {
                                from.RawStr += m_svc.StatBonus;
                                m_svc.Delete();
                            }
                            else
                                from.SendMessage("You have to choose another Stat.");
                            break;
                        }
                    case 2:
                        {
                            if (from.RawDex + m_svc.StatBonus <= 350)
                            {
                                from.RawDex += m_svc.StatBonus;
                                m_svc.Delete();
                            }
                            else
                                from.SendMessage("You have to choose another Stat.");
                            break;
                        }
                    case 3:
                        {
                            if (from.RawInt + m_svc.StatBonus <= 350)
                            {
                                from.RawInt += m_svc.StatBonus;
                                m_svc.Delete();
                            }
                            else
                                from.SendMessage("You have to choose another Stat.");
                            break;
                        }
                }
            }
        }
    }
}