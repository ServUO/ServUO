using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.ACC.PG
{
	[Flags]
	public enum	Conditions
	{
		None      = 0x00000000,
		Adding    = 0x00000001,
		Category  = 0x00000002,
	}

	public class PGAddEditGump : Gump
	{
		private Conditions m_Conditions;
		private int        m_CurCat;
		private int        m_CurLoc;
		private PGCategory m_Cat;
		private PGLocation m_Loc;
		private PublicGate m_Gate;

		private bool GetFlag( Conditions flag ) { return( (m_Conditions & flag) != 0 ); }

		public PGAddEditGump( Conditions conditions, int curC, int curL, PublicGate gate ) : base( 0, 0 )
		{
			if( !PGSystem.Running )
				return;

			m_Conditions = conditions;
			m_CurCat     = curC;
			m_CurLoc     = curL;
			m_Gate       = gate;

			if( !GetFlag( Conditions.Category ) || (GetFlag( Conditions.Category ) && !GetFlag( Conditions.Adding )) )
				m_Cat = PGSystem.CategoryList[curC];
			if( m_Cat != null && (!GetFlag( Conditions.Category ) && !GetFlag( Conditions.Adding )) )
				m_Loc = m_Cat.Locations[curL];


			string Name = "";
			if( !GetFlag( Conditions.Adding ) )
			{
				if( GetFlag( Conditions.Category ) )
					Name = m_Cat.Name;
				else
					Name = m_Loc.Name;
			}

			Point3D Loc = new Point3D( 0, 0, 0 );
			Map Map = Map.Trammel;
			bool Gen, Staff, Reds, Charge, Young;
			int Hue, Cost;
			Gen = Staff = Reds = Charge = Young = false;
			Hue = Cost = 0;

			if( GetFlag( Conditions.Category ) && !GetFlag( Conditions.Adding ) )
			{
				Gen    = m_Cat.GetFlag( EntryFlag.Generate );
				Staff  = m_Cat.GetFlag( EntryFlag.StaffOnly );
				Reds   = m_Cat.GetFlag( EntryFlag.Reds );
				Charge = m_Cat.GetFlag( EntryFlag.Charge );
				Young  = m_Cat.GetFlag( EntryFlag.Young );
				Cost   = m_Cat.Cost;
			}

			if( !GetFlag( Conditions.Category ) && !GetFlag( Conditions.Adding ) )
			{
				Loc    = m_Loc.Location;
				Map    = m_Loc.Map;
				Gen    = m_Loc.GetFlag( EntryFlag.Generate );
				Staff  = m_Loc.GetFlag( EntryFlag.StaffOnly );
				Reds   = m_Loc.GetFlag( EntryFlag.Reds );
				Charge = m_Loc.GetFlag( EntryFlag.Charge );
				Young  = m_Loc.GetFlag( EntryFlag.Young );
				Hue    = m_Loc.Hue;
				Cost   = m_Loc.Cost;
			}

			Closable   = true;
			Disposable = true;
			Dragable   = true;
			Resizable  = false;

			AddPage(0);

			AddBackground( 530, 100, 230, 410, 2600 );
			AddLabel( 602, 120, 0, string.Format("{0} {1}", (GetFlag(Conditions.Adding) ? "Add" : "Edit"), (GetFlag(Conditions.Category) ? "Category" : "Location")) );

			AddLabel( 625, 145, 0, "Name :" );
			AddImage( 555, 170, 2446 );
			AddTextEntry( 565, 170, 160, 20, 0, 2, Name );

			AddLabel( 715, 235, 0, ": C" );
			AddImage( 650, 235, 2443 );
			AddTextEntry( 655, 235, 55, 20, 0, 15, Cost.ToString() );

			if( !GetFlag(Conditions.Category) )
			{

				AddLabel( 560, 210, 0, "X :" );
				AddImage( 580, 210, 2443 );
				AddTextEntry( 585, 210, 55, 20, 0, 3, Loc.X.ToString() );

				AddLabel( 560, 235, 0, "Y :" );
				AddImage( 580, 235, 2443 );
				AddTextEntry( 585, 235, 55, 20, 0, 4, Loc.Y.ToString() );

				AddLabel( 560, 260, 0, "Z :" );
				AddImage( 580, 260, 2443 );
				AddTextEntry( 585, 260, 55, 20, 0, 5, Loc.Z.ToString() );

				AddLabel( 715, 210, 0, ": H" );
				AddImage( 650, 210, 2443 );
				AddTextEntry( 655, 210, 55, 20, 0, 14, Hue.ToString() );

				AddLabel( 585, 315, 0, "Trammel" );
				AddRadio( 555, 315, 208, 209, (Map == Map.Trammel ? true : false), 6 );

				AddLabel( 585, 340, 0, "Felucca" );
				AddRadio( 555, 340, 208, 209, (Map == Map.Felucca ? true : false), 7 );

				AddLabel( 685, 315, 0, "Malas" );
				AddRadio( 655, 315, 208, 209, (Map == Map.Malas ? true : false), 8 );

				AddLabel( 685, 345, 0, "Ilshenar" );
				AddRadio( 655, 340, 208, 209, (Map == Map.Ilshenar ? true : false), 9 );

				AddLabel( 585, 365, 0, "Tokuno" );
				AddRadio( 555, 370, 208, 209, (Map == Map.Tokuno ? true : false), 10 );

				AddLabel( 685, 365, 0, "TerMur" );
				AddRadio( 655, 370, 208, 209, (Map == Map.TerMur ? true : false), 20 );
			}


			AddLabel( 585, 395, 0, "Generate?" );
			AddCheck( 555, 395, 210, 211, Gen, 11 );

			AddLabel( 665, 395, 0, "Young?" );
			AddCheck( 715, 395, 210, 211, Young, 16 );

			AddLabel( 585, 420, 0, "Reds?" );
			AddCheck( 555, 420, 210, 211, Reds, 13 );

			AddLabel( 658, 420, 0, "Charge?" );
			AddCheck( 715, 420, 210, 211, Charge, 17 );

			AddLabel( 585, 445, 0, "Staff Only?" );
			AddCheck( 555, 445, 210, 211, Staff, 12 );

			AddButton( 700, 450, 1417, 1417, 1, GumpButtonType.Reply, 0 );
			AddLabel( 728, 481, 69, "Apply" );
		}

		private EntryFlag Flags;
		private void SetFlag( EntryFlag flag, bool value )
		{
			if( value )
				Flags |= flag;
			else Flags &= ~flag;
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if( state.Mobile.AccessLevel < PGSystem.PGAccessLevel )
				return;

			Mobile from = state.Mobile;
			int BID = info.ButtonID;
			if( BID == 0 )
				return;

			SetFlag( EntryFlag.Generate, info.IsSwitched( 11 ) );
			SetFlag( EntryFlag.StaffOnly, info.IsSwitched( 12 ) );
			SetFlag( EntryFlag.Reds, info.IsSwitched( 13 ) );
			SetFlag( EntryFlag.Young, info.IsSwitched( 16 ) );
			SetFlag( EntryFlag.Charge, info.IsSwitched( 17 ) );

			Map Map = null;
			for( int i = 0; i < info.Switches.Length; i++ )
			{
				int m = info.Switches[i];
				switch( m )
				{
					case 6:  Map = Map.Trammel;  break;
					case 7:  Map = Map.Felucca;  break;
					case 8:  Map = Map.Malas;    break;
					case 9:  Map = Map.Ilshenar; break;
					case 10: Map = Map.Tokuno;   break;
                   		 	case 20: Map = Map.TerMur;   break;

				}
			}


			TextRelay NR = info.GetTextEntry( 2 );
			TextRelay XR = info.GetTextEntry( 3 );
			TextRelay YR = info.GetTextEntry( 4 );
			TextRelay ZR = info.GetTextEntry( 5 );
			TextRelay HR = info.GetTextEntry( 14 );
			TextRelay CR = info.GetTextEntry( 15 );
			string NS = (NR == null ? null : NR.Text.Trim());
			string XS = (XR == null ? null : XR.Text.Trim());
			string YS = (YR == null ? null : YR.Text.Trim());
			string ZS = (ZR == null ? null : ZR.Text.Trim());
			string HS = (HR == null ? null : HR.Text.Trim());
			string CS = (CR == null ? null : CR.Text.Trim());

			if( BID == 1 )
			{
				if( GetFlag( Conditions.Category ) )
				{
					if( GetFlag( Conditions.Adding ) )
					{
						if( NS == null || NS.Length == 0 || CS == null || CS.Length == 0 )
						{
							from.SendMessage( "Please enter a name and cost for this Category." );
							from.CloseGump( typeof( PGAddEditGump ) );
							from.SendGump( new PGAddEditGump( m_Conditions, m_CurCat, m_CurLoc, m_Gate ) );
							return;
						}

						int c = 0;
						try
						{
							c = Int32.Parse( CS );
							PGSystem.CategoryList.Add( new PGCategory( NS, Flags, c ) );
							from.SendMessage( "Added Category." );
						}
						catch
						{
							from.SendMessage( "Bad cost value, defaulting to 0." );
							PGSystem.CategoryList.Add( new PGCategory( NS, Flags ) );
						}
					}

					else
					{
						if( NS == null || NS.Length == 0 )
						{
							from.SendMessage( "Removed the Category." );
							PGSystem.CategoryList.RemoveAt( m_CurCat );
							m_CurCat = 0;
						}

						else
						{
							from.SendMessage( "Changed the Category." );
							PGSystem.CategoryList[m_CurCat].Name = NS;
							PGSystem.CategoryList[m_CurCat].Flags = Flags;
							if( CS == null || CS.Length == 0 )
								PGSystem.CategoryList[m_CurCat].Cost = 0;
							else
							{
								int c = 0;

								try
								{
									c = Int32.Parse( CS );
									PGSystem.CategoryList[m_CurCat].Cost = c;
								}
								catch
								{
									PGSystem.CategoryList[m_CurCat].Cost = 0;
								}
							}
						}
					}
				}

				else
				{
					if( NS == null || NS.Length == 0 ||
						XS == null || XS.Length == 0 ||
						YS == null || YS.Length == 0 ||
						ZS == null || ZS.Length == 0 ||
						HS == null || HS.Length == 0 ||
						CS == null || CS.Length == 0 )
					{
						if( GetFlag( Conditions.Adding ) )
						{
							from.SendMessage( "Please fill in each field." );
							from.CloseGump( typeof( PGAddEditGump ) );
							from.SendGump( new PGAddEditGump( m_Conditions, m_CurCat, m_CurLoc, m_Gate ) );
							return;
						}

						from.SendMessage( "Removed the Location." );
						PGSystem.CategoryList[m_CurCat].Locations.RemoveAt( m_CurLoc );
					}

					else if( Map == null )
					{
						from.SendMessage( "Please select a Map." );
						from.CloseGump( typeof( PGAddEditGump ) );
						from.SendGump( new PGAddEditGump( m_Conditions, m_CurCat, m_CurLoc, m_Gate ) );
						return;
					}

					else
					{
						int x, y, z, h, c = 0;
						Point3D Loc;
						int Hue;
						int Cost;
						try
						{
							x = Int32.Parse( XS );
							y = Int32.Parse( YS );
							z = Int32.Parse( ZS );
							h = Int32.Parse( HS );
							c = Int32.Parse( CS );
							Loc = new Point3D( x, y, z );
							Hue = h;
							Cost = c;
						}
						catch
						{
							from.SendMessage( "Please enter an integer in each of the info fields. (X, Y, Z, H, C)" );
							from.CloseGump( typeof( PGAddEditGump ) );
							from.SendGump( new PGAddEditGump( m_Conditions, m_CurCat, m_CurLoc, m_Gate ) );
							return;
						}

						PGLocation PGL = new PGLocation( NS, Flags, Loc, Map, Hue, Cost );
						if( PGL == null )
						{
							from.SendMessage( "Bad Location information, can't add!" );
							from.CloseGump( typeof( PGAddEditGump ) );
							from.SendGump( new PGAddEditGump( m_Conditions, m_CurCat, m_CurLoc, m_Gate ) );
							return;
						}

						if( GetFlag( Conditions.Adding ) )
						{
							from.SendMessage( "Added the Location." );
							PGSystem.CategoryList[m_CurCat].Locations.Add( PGL );
						}
						else
						{
							from.SendMessage( "Changed the Location." );
							PGSystem.CategoryList[m_CurCat].Locations[m_CurLoc] = PGL;
						}
					}
				}

				from.CloseGump( typeof( PGGump ) );
				from.SendGump( new PGGump( from, m_CurCat, m_Gate ) );
			}
		}
	}
}