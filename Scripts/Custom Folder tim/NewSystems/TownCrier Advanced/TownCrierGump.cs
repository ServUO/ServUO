using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Targets;

namespace Server.Gumps
{
	public class TownCrierbGump : Gump
	{
		public TownCrierb m_Crier;
		public TownCrierStone m_Stone;

		public TownCrierbGump( TownCrierStone stone, TownCrierb crier ) : base( 50, 50 )
		{
			m_Stone = stone;
			m_Crier = crier;

			AddPage( 0 );
			AddImageTiled( 0, 0, 410, 144, 0x52 );
			AddAlphaRegion( 1, 1, 408, 142 );

			if ( m_Stone == null )
			{
				AddLabel( 160, 12, 2100, "TownCrier Controls" );

				string custom = Convert.ToString( m_Crier.Custom );
				AddButton( 10, 34, 4005, 4007, 1, GumpButtonType.Reply, 0 );
				AddLabel( 50, 34, 2100, "Custom = "+ custom );

				string active = Convert.ToString( m_Crier.Active );
				AddButton( 10, 56, 4005, 4007, 2, GumpButtonType.Reply, 0 );
				AddLabel( 50, 56, 2100, "Active = "+ active );

				string random = Convert.ToString( m_Crier.Random );
				AddButton( 200, 34, 4005, 4007, 3, GumpButtonType.Reply, 0 );
				AddLabel( 240, 34, 2100, "Random = "+ random );

				string delay = Convert.ToString( m_Crier.Delay );
				AddButton( 200, 56, 4005, 4007, 4, GumpButtonType.Reply, 0 );
				AddImageTiled( 240, 56, 70, 20, 0xBBC );
				AddImageTiled( 241, 57, 68, 18, 0x2426 );
				AddTextEntry( 241, 57, 68, 18, 0x480, 0, delay );
				AddLabel( 315, 56, 2100, "Delay" );

				AddButton( 10, 78, 4005, 4007, 5, GumpButtonType.Reply, 0 );
				AddLabel( 50, 78, 2100, "Edit News" );

				if ( crier.Stone == null )
				{
					AddButton( 200, 78, 4005, 4007, 6, GumpButtonType.Reply, 0 );
					AddLabel( 240, 78, 2100, "Control Stone = null" );
				}
			}
			else
			{
				AddLabel( 140, 12, 2100, "TownCrier Gobal Controls" );

				string active = Convert.ToString( m_Stone.Active );
				AddButton( 10, 34, 4005, 4007, 7, GumpButtonType.Reply, 0 );
				AddLabel( 50, 34, 2100, "Active = "+ active );

				string random = Convert.ToString( m_Stone.Random );
				AddButton( 10, 56, 4005, 4007, 8, GumpButtonType.Reply, 0 );
				AddLabel( 50, 56, 2100, "Random = "+ random );

				string delay = Convert.ToString( m_Stone.Delay );
				AddButton( 200, 34, 4005, 4007, 9, GumpButtonType.Reply, 0 );
				AddImageTiled( 240, 34, 70, 20, 0xBBC );
				AddImageTiled( 241, 35, 68, 18, 0x2426 );
				AddTextEntry( 241, 35, 68, 18, 0x480, 0, delay );
				AddLabel( 315, 34, 2100, "Delay" );

				AddButton( 200, 56, 4005, 4007, 10, GumpButtonType.Reply, 0 );
				AddLabel( 240, 56, 2100, "Edit News" );

				AddButton( 10, 78, 4005, 4007, 11, GumpButtonType.Reply, 0 );
				AddLabel( 50, 78, 2100, "Give Control Gem" );

				AddButton( 200, 78, 4005, 4007, 12, GumpButtonType.Reply, 0 );
				AddLabel( 240, 78, 2100, "Place TownCrier ( Frozen )" );

				AddButton( 10, 100, 4005, 4007, 13, GumpButtonType.Reply, 0 );
				AddLabel( 50, 100, 2100, "Place TownCrier ( Wondering )" );
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;
			switch( info.ButtonID )
			{
				case 1:
				{
					if ( m_Crier.Custom )
						m_Crier.Custom = false;
					else
						m_Crier.Custom = true;

					from.SendGump( new TownCrierbGump( null, m_Crier ) );
					break;
				}
				case 2:
				{
					if ( m_Crier.Active )
						m_Crier.Active = false;
					else
						m_Crier.Active = true;

					from.SendGump( new TownCrierbGump( null, m_Crier ) );
					break;
				}
				case 3:
				{
					if ( m_Crier.Random )
						m_Crier.Random = false;
					else
						m_Crier.Random = true;

					from.SendGump( new TownCrierbGump( null, m_Crier ) );
					break;
				}
				case 4:
				{
					TextRelay text = info.GetTextEntry( 0 );
               
					if ( text != null )
					{
						try
						{
							string[] temp = text.Text.Split(':');
							TimeSpan time = new TimeSpan( Convert.ToInt32( temp[0] ), Convert.ToInt32( temp[1] ), Convert.ToInt32( temp[2] ) );
							m_Crier.Delay = time;

							if ( m_Crier.Active )
							{
								m_Crier.Active = false;
								m_Crier.Active = true;
							}
						}
						catch
						{
							from.SendMessage( 0x35, "Bad format. ##:##:## expected" );
						}
					}

					from.SendGump( new TownCrierbGump( null, m_Crier ) );
					break;
				}
				case 5:
				{
					from.SendGump( new EditNewsGump( m_Crier, null ) );
					break;
				}
				case 6:
				{
					from.SendMessage( 0x35, "Target the TownCrier Control Stone." );
					from.Target = new SetStoneTarget( m_Crier );
					break;
				}
				case 7:
				{
					if ( m_Stone.Active )
						m_Stone.Active = false;
					else
						m_Stone.Active = true;

					from.SendGump( new TownCrierbGump( m_Stone, null ) );
					break;
				}
				case 8:
				{
					if ( m_Stone.Random )
						m_Stone.Random = false;
					else
						m_Stone.Random = true;

					from.SendGump( new TownCrierbGump( m_Stone, null ) );
					break;
				}
				case 9:
				{
					TextRelay text = info.GetTextEntry( 0 );

					if ( text != null )
					{
						try
						{
							string[] temp = text.Text.Split(':');
							TimeSpan time = new TimeSpan( Convert.ToInt32( temp[0] ), Convert.ToInt32( temp[1] ), Convert.ToInt32( temp[2] ) );
							m_Stone.Delay = time;

							if ( m_Stone.Active )
							{
								m_Stone.Active = false;
								m_Stone.Active = true;
							}
						}
						catch
						{
							from.SendMessage( 0x35, "Bad format. ##:##:## expected" );
						}
					}

					from.SendGump( new TownCrierbGump( m_Stone, null ) );
					break;
				}
				case 10:
				{
					from.SendGump( new EditNewsGump( null, m_Stone ) );
					break;
				}
				case 11:
				{
					ControlGem gem = new ControlGem();
					gem.Stone = m_Stone;
					from.AddToBackpack( gem );
					from.SendGump( new TownCrierbGump( m_Stone, null ) );
					break;
				}
				case 12:
				{
					from.Target = new MakeCrierTarget( m_Stone, false );
					break;
				}
				case 13:
				{
					from.Target = new MakeCrierTarget( m_Stone, true );
					break;
				}
			}
		}

		public class MakeCrierTarget : Target
		{
			public TownCrierStone m_Stone;
			public bool m_Wondering;

			public MakeCrierTarget( TownCrierStone stone, bool wondering ) : base( -1, true, TargetFlags.None )
			{
				m_Stone = stone;
				m_Wondering = wondering;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				IPoint3D p = (IPoint3D)o;
			
				if ( p != null )
				{
					if ( m_Wondering )
					{
						TownCrierb m_Crier = new TownCrierb();
						m_Crier.StoneActive = m_Stone.Active;
						m_Crier.Stone = m_Stone;
						m_Crier.Map = m.Map;
						m_Crier.Location = new Point3D( p.X, p.Y, p.Z );
						m_Crier.Home = new Point3D( p.X, p.Y, p.Z );
						m.SendGump( new RangeHomeGump( m_Stone, m_Crier ) );
					}
					else
					{
						TownCrierb m_Crier = new TownCrierb();
						m_Crier.StoneActive = m_Stone.Active;
						m_Crier.Stone = m_Stone;
						m_Crier.Frozen = true;
						m_Crier.Map = m.Map;
						m_Crier.Location = new Point3D( p.X, p.Y, p.Z );
						m.SendGump( new TownCrierbGump( m_Stone, null ) );
					}
				}
			}
		}

		public class SetStoneTarget : Target
		{
			public TownCrierb m_Crier;

			public SetStoneTarget( TownCrierb crier ) : base( -1, false, TargetFlags.None )
			{
				m_Crier = crier;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is TownCrierStone )
				{
					m_Crier.Stone = (TownCrierStone)targeted;
					from.SendGump( new TownCrierbGump( null, m_Crier ) );
				}
				else
				{
					from.SendMessage( 0x35, "Invalid Item! Target the TownCrier Control Stone." );
					from.Target = new SetStoneTarget( m_Crier );
				}
			}
		}
	}

	public class RangeHomeGump : Gump
	{
		public TownCrierStone m_Stone;
		public TownCrierb m_Crier;

		public RangeHomeGump( TownCrierStone stone, TownCrierb crier ) : base( 50, 50 )
		{
			m_Stone = stone;
			m_Crier = crier;
			AddPage( 0 );
			AddImageTiled( 0, 0, 202, 52, 0x52 );
			AddAlphaRegion( 1, 1, 200, 50 );

			string range = Convert.ToString( m_Crier.RangeHome );
			AddLabel( 20, 2, 2100, "Set TownCrierb's RangeHome" );
			AddButton( 10, 24, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddImageTiled( 50, 24, 140, 20, 0xBBC );
			AddImageTiled( 51, 25, 138, 18, 0x2426 );
			AddTextEntry( 51, 25, 138, 18, 0x480, 0, range );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID == 0 )
				from.SendGump( new TownCrierbGump( m_Stone, null ) );
			if ( info.ButtonID == 1 )
			{
				TextRelay text = info.GetTextEntry( 0 );
				if ( text != null )
				{
					try
					{
						int rangeHome = Convert.ToInt32( text.Text );
						m_Crier.RangeHome = rangeHome;
						from.SendGump( new RangeHomeGump( m_Stone, m_Crier ) );
					}
					catch
					{
						from.SendMessage( 0x35, "Bad format. ## expected!" );
						from.SendGump( new RangeHomeGump( m_Stone, m_Crier ) );
					}
				}
			}
		}
	}

	public class EditNewsGump : Gump
	{
		public static string path = "Data/news.txt";
		public TownCrierStone m_Stone;
		public TownCrierb m_Crier;

		public EditNewsGump( TownCrierb crier, TownCrierStone stone ) : base( 10, 10 )
		{
			m_Stone = stone;
			m_Crier = crier;

			AddPage( 0 );
			AddImageTiled( 0, 0, 620, 460, 0x52 );
			AddAlphaRegion( 1, 1, 618, 458 );

			AddLabel( 250, 12, 2100, "Edit TownCrier News" );
			AddLabel( 10, 34, 2100, "News Entries" );

			ArrayList m_Lines = GetFile();

			int row = 1;
			for ( int i = 0; i < 15; i++ )
			{
				string line = m_Lines[i] as string;

				AddImageTiled( 10, 34 + (row * 22), 598, 20, 0xBBC );
				AddImageTiled( 11, 34 + (row * 22) + 1, 596, 18, 0x2426 );
				AddTextEntry( 11, 34 + (row++ * 22) + 1, 596, 18, 0x480, i, line );
			}

			AddLabel( 500,  408, 2100, "Update File" );
			AddButton( 576, 408, 4005, 4007, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			switch( info.ButtonID )
			{
				case 0:
				{
					if ( m_Stone != null )
						from.SendGump( new TownCrierbGump( m_Stone, null ) );
					else
						from.SendGump( new TownCrierbGump( null, m_Crier ) );
					break;
				}
				case 1:
				{
					using ( StreamWriter op = new StreamWriter( path ) )
					{
						for ( int i = 0; i < 15; i++ )
						{
							TextRelay relay = info.GetTextEntry( i );
							string text = Convert.ToString( relay.Text );
							if ( text != null )
							{
								if ( text.Length > 0 )
								op.WriteLine( text );
							}
						}
					}
					from.SendMessage( 0x35, "News file has been updated." );
					if ( m_Stone != null )
						from.SendGump( new EditNewsGump( null, m_Stone ) );
					else
						from.SendGump( new EditNewsGump( m_Crier, null ) );
					break;
				}
			}
		}

		public ArrayList GetFile()
		{
			ArrayList m_Lines = new ArrayList();

			if ( !File.Exists( path ) )
			{
				for ( int i = 0; i < 15; i++ )
				{
					m_Lines.Add( "" );
				}
				return m_Lines;
			}

			using ( StreamReader ip = new StreamReader( path ) )
			{
				string line;

				while ( (line = ip.ReadLine()) != null )
				{
					if ( line.Length > 0 )
						m_Lines.Add( line );
				}
			}

			if ( m_Lines.Count < 15 )
			{
				int m_Count = 15 - m_Lines.Count;

				for ( int i = 0; i < m_Count; i++ )
				{
					m_Lines.Add( "" );
				}
			}
			return m_Lines;
		}
	}
}
