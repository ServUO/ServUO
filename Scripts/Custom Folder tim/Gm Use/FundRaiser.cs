using System;
using System.Collections;
using System.Text;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Regions;

namespace Server.Items
{
	public class FundRaiser : Barrel
	{
		string m_FundName;
		int m_Fund;
		ArrayList m_Contributers;
		bool m_ShowTopList;

        public ArrayList Contributers
		{
			get
			{
				if ( m_Contributers == null )
					m_Contributers = new ArrayList();
				for ( int i = 0; i < m_Contributers.Count; i++ )
				{
					Contributer contributer = (Contributer)m_Contributers[i];
					if ( contributer == null || contributer.Mobile == null || contributer.Mobile.Deleted )
					{
						m_Contributers.Remove( contributer );
						i--;
					}
				}
				return m_Contributers;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ShowTopList
		{
			get { return m_ShowTopList; }
			set { m_ShowTopList = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string FundName
		{
			get { return m_FundName; }
			set { m_FundName = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Fund
		{
			get { return m_Fund; }
			set { m_Fund = value; }
		}

		[Constructable]
		public FundRaiser() : base()
		{
			Hue = 2207;
			Movable = false;
			FundName = "Random Fund";
		}

		public FundRaiser( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)1 );

			writer.Write( m_ShowTopList );
			writer.Write( Contributers.Count );
			foreach ( Contributer contributer in Contributers )
			{
				writer.Write( contributer.Amount );
				writer.Write( contributer.Mobile );
				writer.Write( contributer.Date );
			}
			writer.Write( FundName );
			writer.Write( Fund );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_ShowTopList = reader.ReadBool();
					int c = reader.ReadInt();
					for ( int i = 0; i < c; i++ )
					{
						Contributers.Add( new Contributer( reader.ReadInt(), reader.ReadMobile() ) );
						((Contributer)Contributers[Contributers.Count - 1]).Date = reader.ReadDateTime();
					}
					goto case 0;
				}
				case 0:
				{
					FundName = reader.ReadString();
					Fund = reader.ReadInt();
					break;
				}
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, String.Format( "Raiser for: {0}. Gold so far: {1}.", m_FundName, m_Fund ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_ShowTopList )
				from.SendGump( new ContributersGump( this, from ) );
			else
				base.OnDoubleClick( from );
			//LabelTo( from, String.Format( "Raiser for: {0}. Gold so far: {1}.", m_FundName, m_Fund ) );
		}


		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
			{
				AddToFund( from, ((Gold)dropped).Amount );
				dropped.Delete();
				return true;
			}
			else if ( dropped is BankCheck )
			{
				AddToFund( from, ((BankCheck)dropped).Worth );
				dropped.Delete();
				return true;
			}
			else
			{
				from.SendMessage( "You can only drop gold or checks here." );
				return false;
			}
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			return OnDragDrop( from, item );
		}

		private void AddToFund( Mobile from, int amount )
		{
			bool skip = false;
			foreach ( Contributer contributer in Contributers )
				if ( contributer.Mobile == from )
				{
					contributer.Amount += amount;
					contributer.Date = DateTime.Now;
					skip = true;
					break;
				}
			if ( !skip )
				Contributers.Add( new Contributer( amount, from ) );
			Fund += amount;
			PublicOverheadMessage( Network.MessageType.Regular, 0x3B2, false, String.Format( "{0} contributed with {1} gold. Thanks a lot!", from.Name, amount ) );
		}

		protected class Contributer
		{
			int m_Amount;
			Mobile m_Contributer;
			DateTime m_ContributionDate;
			public int Amount
			{
				get { return m_Amount; }
				set { m_Amount = value; }
			}

			public Mobile Mobile
			{
				get { return m_Contributer; }
				set { m_Contributer = value; }
			}

			public DateTime Date
			{
				get { return m_ContributionDate; }
				set { m_ContributionDate = value; }
			}

			public Contributer( int amount, Mobile contributer )
			{
				m_Amount = amount;
				m_Contributer = contributer;
				m_ContributionDate = DateTime.Now;
			}
		}

		private class ContributersGump : Gump
		{
			FundRaiser m_FundRaiser;
			Mobile m_Mobile;

			public ContributersGump( FundRaiser raiser, Mobile from ) : base( 30, 30 )
			{
				m_FundRaiser = raiser;
				m_Mobile = from;

				AddPage( 0 );
				AddBackground( 0, 0, 250, 360, 9270 );
				AddAlphaRegion( 0, 0, 250, 360 );

				m_FundRaiser.Contributers.Sort( new ContributionComparer() );

				AddLabel( 50, 10, 2401, String.Format( "Top {0} Contributors", m_FundRaiser.Contributers.Count > 20 ? 20 : m_FundRaiser.Contributers.Count ) );

				for ( int i = 0; i < m_FundRaiser.Contributers.Count && i < 20; i++ )
				{
					Contributer contributer = (Contributer)m_FundRaiser.Contributers[i];
					AddLabel( 30, 30 + i * 15, 2401, (i + 1).ToString() );
					AddLabel( 50, 30 + i * 15, 2401, contributer.Mobile.Name );
					AddLabel( 170, 30 + i * 15, 2401, contributer.Amount.ToString() );
				}

				if ( m_Mobile.AccessLevel >= AccessLevel.GameMaster )
				{
					AddLabel( 280, 10, 2401, "Clear" );
					AddButton( 250, 10, 4023, 4024, 1, GumpButtonType.Reply, 0 );
                    AddLabel(280, 30, 2401, "Full List");
                    AddButton(250, 30, 4023, 4024, 2, GumpButtonType.Reply, 0);
                }
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile m = state.Mobile;
				if ( m_FundRaiser == null || m == null || m_Mobile == null )
					return;
				int button = info.ButtonID;
				if ( button == 1 && m_Mobile.AccessLevel >= AccessLevel.GameMaster )
				{
					m_FundRaiser.Contributers.Clear();
					m_Mobile.SendGump( new ContributersGump( m_FundRaiser, m_Mobile ) );
				}
                if (button == 2 && m_Mobile.AccessLevel >= AccessLevel.GameMaster)
                {
                    m_Mobile.SendGump(new ContributersStaffGump(m_FundRaiser.Contributers));
                }
            }

			private class ContributionComparer : IComparer
			{
				public int Compare( object a, object b )
				{
					if ( !( a is Contributer ) || !( b is Contributer ) )
						return 0;
					Contributer contributera = (Contributer)a;
					Contributer contributerb = (Contributer)b;
					if ( contributera.Amount > contributerb.Amount )
						return -1;
					else if ( contributera.Amount < contributerb.Amount )
						return 1;
					else if ( contributera.Date > contributerb.Date )
						return -1;
					else if ( contributera.Date < contributerb.Date )
						return 1;
					else
						return 0;
				}
			}
		}

        public class ContributersStaffGump : Gump
        {
            public static readonly int MaxEntriesPerPage = 30;
            private int m_Page;
            private ArrayList m_Contributers;

            public ContributersStaffGump(ArrayList contributers)
                : this(contributers, 0)
            {
            }

            public ContributersStaffGump(ArrayList contributers, int page)
                : base(500, 30)
            {
                m_Page = page;
                m_Contributers = contributers;

                AddImageTiled(0, 0, 300, 425, 0xA40);
                AddAlphaRegion(1, 1, 298, 423);

                AddHtml(10, 10, 280, 20, "<basefont color=#A0A0FF><center>Fund Raiser [Staff View]</center></basefont>", false, false);

                int lastPage = (m_Contributers.Count - 1) / MaxEntriesPerPage;

                string sLog;

                if (page < 0 || page > lastPage)
                {
                    sLog = "";
                }
                else
                {
                    int max = m_Contributers.Count - (lastPage - page) * MaxEntriesPerPage;
                    int min = Math.Max(max - MaxEntriesPerPage, 0);

                    StringBuilder builder = new StringBuilder();

                    for (int i = min; i < max; i++)
                    {
                        Contributer contributer = contributers[i] as Contributer;

                        if (contributer != null && contributer.Mobile != null && contributer.Mobile.Account != null)
                        {
                            if (i != min) builder.Append("<br>");
                            builder.AppendFormat("{0} (<i>{1}</i>): {2}", contributer.Mobile.Name, contributer.Mobile.Account, contributer.Amount);
                        }
                    }

                    sLog = builder.ToString();
                }

                AddHtml(10, 40, 280, 350, sLog, false, true);

                if (page > 0)
                    AddButton(10, 395, 0xFAE, 0xFB0, 1, GumpButtonType.Reply, 0); // Previous page

                AddLabel(45, 395, 0x481, String.Format("Current page: {0}/{1}", page + 1, lastPage + 1));

                if (page < lastPage)
                    AddButton(261, 395, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0); // Next page
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                switch (info.ButtonID)
                {
                    case 1: // Previous page
                        {
                            if (m_Page - 1 >= 0)
                                from.SendGump(new ContributersStaffGump(m_Contributers, m_Page - 1));

                            break;
                        }
                    case 2: // Next page
                        {
                            if ((m_Page + 1) * MaxEntriesPerPage < m_Contributers.Count)
                                from.SendGump(new ContributersStaffGump(m_Contributers, m_Page + 1));

                            break;
                        }
                }
            }
        }

	}
}