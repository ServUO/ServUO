using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Misc;

namespace Server.Gumps
{
	public class MannequinControl : Gump
	{
		public Mobile m_Controller;
		public Mannequin m_Controlled;
		public int m_CurrentPage;

		public MannequinControl( Mannequin controlled, Mobile controller, int curpage )
			: base( 0, 0 )
		{
			controller.SendMessage( "CurrentPage = {0}", curpage );
			m_Controlled = controlled;
			m_Controller = controller;
			m_CurrentPage = curpage;

			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);
			AddBackground(0, 0, 200/*180*/, 300, 9270);

			AddLabel(60, 20, 68, m_Controlled.Name);

			AddButton(20,  75, 2117, 2118, 1, GumpButtonType.Reply, 0);
			AddButton(20, 125, 2117, 2118, 2, GumpButtonType.Reply, 0);
			AddButton(20, 175, 2117, 2118, 3, GumpButtonType.Reply, 0);
			AddButton(20, 225, 2117, 2118, 4, GumpButtonType.Reply, 0);
			AddLabel(45, 75, 49, "Move Me");
			AddLabel(45, 125, 49, "Face Me");
			AddLabel(45, 175, 49, "Change Hue");
			AddLabel(45, 225, 49, "Change Gender");

			AddPage(1);

			switch( m_CurrentPage )
			{
				case( 1 ):
				{
					AddBackground(180, 0, 180, 250, 9270);
					AddButton(262, 25, 2117, 2118, 5, GumpButtonType.Reply, 0);
					AddButton(262, 140, 2117, 2118, 6, GumpButtonType.Reply, 0);
					AddHtml( 194, 45, 150, 75, "<basefont color=#33aa33>Click here and I will move to your location.</basefont>", false, false);
					AddHtml( 194, 160, 150, 75, "<basefont color=#aa3333>Click here and I will move to where you target.</basefont>", false, false);
					break;
				}

				case( 2 ):
				{
					AddBackground(180, 0, 250, 250, 9270);
					AddImage(205, 25, 5010);
					AddImage( 270, 70, 9000 );

					if( m_Controlled.Direction == Direction.Mask )
						AddImage(298, 60, 1209);
					else if( m_Controlled.Direction == Direction.North )
						AddImage(338, 78, 1209);
					else if( m_Controlled.Direction == Direction.Right )
						AddImage(356, 118, 1209);
					else if( m_Controlled.Direction == Direction.East )
						AddImage(339, 157, 1209);
					else if( m_Controlled.Direction == Direction.Down )
						AddImage(298, 175, 1209);
					else if( m_Controlled.Direction == Direction.South )
						AddImage(258, 158, 1209);
					else if( m_Controlled.Direction == Direction.Left )
						AddImage(240, 118, 1209);
					else if( m_Controlled.Direction == Direction.West )
						AddImage(258, 77, 1209);

					AddButton(281, 8, 4500, 4500, 7, GumpButtonType.Reply, 0);
					AddButton(345, 37, 4501, 4501, 8, GumpButtonType.Reply, 0);
					AddButton(372, 100, 4502, 4502, 9, GumpButtonType.Reply, 0);
					AddButton(342, 164, 4503, 4503, 10, GumpButtonType.Reply, 0);
					AddButton(281, 191, 4504, 4504, 11, GumpButtonType.Reply, 0);
					AddButton(216, 165, 4505, 4505, 12, GumpButtonType.Reply, 0);
					AddButton(188, 100, 4506, 4506, 13, GumpButtonType.Reply, 0);
					AddButton(216, 35, 4507, 4507, 14, GumpButtonType.Reply, 0);
					break;
				}

				case( 3 ):
				{
					AddBackground(180, 0, 180, 250, 9270);
					AddButton(262, 74, 2117, 2118, 15, GumpButtonType.Reply, 0);
					AddButton(262, 118, 2117, 2118, 16, GumpButtonType.Reply, 0);
					AddButton(262, 162, 2117, 2118, 17, GumpButtonType.Reply, 0);
					AddHtml( 194, 89, 150, 75, "<basefont color=#33aa33>Change my hue to white.</basefont>", false, false);
					AddHtml( 194, 133, 150, 75, "<basefont color=#33aa33>Change my hue to black.</basefont>", false, false);
					AddHtml( 194, 175, 150, 75, "<basefont color=#33aa33>Change my hue to grey.</basefont>", false, false);
					break;
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			switch( info.ButtonID )
			{
				case 0:
				{
					m_Controlled.Say( "All done then?  OK." );
					m_Controlled.AI = AIType.AI_Use_Default;
					m_Controlled.CantWalk = true;
					m_Controlled.m_Listen = false;
					break;
				}

				case 1://Move
				{
					from.CloseGump( typeof (MannequinControl) );
					from.SendGump( new MannequinControl( m_Controlled, from, 1 ) );
					break;
				}
				case 2://Face
				{
					from.CloseGump( typeof (MannequinControl) );
					from.SendGump( new MannequinControl( m_Controlled, from, 2 ) );
					break;
				}
				case 3://Hue
				{
					from.CloseGump( typeof (MannequinControl) );
					from.SendGump( new MannequinControl( m_Controlled, from, 3 ) );
					break;
				}
				case 4://Gender
				{
					if( m_Controlled.Female || m_Controlled.BodyValue == 401 )
					{
						m_Controlled.Female = false;
						m_Controlled.BodyValue = 400;
                                                m_Controlled.Name = "Adam";
						m_Controlled.Say( "I'm now a man, thanks." );
					}
					else
					{
						m_Controlled.Female = true;
						m_Controlled.BodyValue = 401;
                                                m_Controlled.Name = "Eve";
						m_Controlled.Say( "You sliced it off!?  Fine." );
					}
					from.SendGump( new MannequinControl( m_Controlled, from, 0 ) );
					break;
				}
				case 5://MoveToPlayer
				{
					m_Controlled.AI = AIType.AI_Melee;
					m_Controlled.m_NextMove = from.Location;

					WayPoint GoHere = new WayPoint();
					GoHere.Map = from.Map;
					GoHere.Location = from.Location;
					m_Controlled.CurrentWayPoint = GoHere;
					m_Controlled.CantWalk = false;
					m_Controlled.Say( "I will gladly move here for you Master" );
					m_Controlled.m_WayPoints.Add( GoHere );
					from.SendGump( new MannequinControl( m_Controlled, from, 1 ) );
					break;
				}
				case 6://MoveToTarget
				{
					from.Target = new MannequinTarget( from, m_Controlled );
					break;
				}
				case 7:
				{
					m_Controlled.m_Direction = Direction.Mask;
					m_Controlled.Direction = Direction.Mask;
					m_Controlled.Say( "I am now facing Up" );
					from.SendGump( new MannequinControl( m_Controlled, from, 2 ) );
					break;
				}
				case 8:
				{
					m_Controlled.m_Direction = Direction.North;
					m_Controlled.Direction = Direction.North;
					m_Controlled.Say( "I am now facing North" );
					from.SendGump( new MannequinControl( m_Controlled, from, 2 ) );
					break;
				}
				case 9:
				{
					m_Controlled.m_Direction = Direction.Right;
					m_Controlled.Direction = Direction.Right;
					m_Controlled.Say( "I am now facing Right" );
					from.SendGump( new MannequinControl( m_Controlled, from, 2 ) );
					break;
				}
				case 10:
				{
					m_Controlled.m_Direction = Direction.East;
					m_Controlled.Direction = Direction.East;
					m_Controlled.Say( "I am now facing East" );
					from.SendGump( new MannequinControl( m_Controlled, from, 2 ) );
					break;
				}
				case 11:
				{
					m_Controlled.m_Direction = Direction.Down;
					m_Controlled.Direction = Direction.Down;
					m_Controlled.Say( "I am now facing Down" );
					from.SendGump( new MannequinControl( m_Controlled, from, 2 ) );
					break;
				}
				case 12:
				{
					m_Controlled.m_Direction = Direction.South;
					m_Controlled.Direction = Direction.South;
					m_Controlled.Say( "I am now facing South" );
					from.SendGump( new MannequinControl( m_Controlled, from, 2 ) );
					break;
				}
				case 13:
				{
					m_Controlled.m_Direction = Direction.Left;
					m_Controlled.Direction = Direction.Left;
					m_Controlled.Say( "I am now facing Left" );
					from.SendGump( new MannequinControl( m_Controlled, from, 2 ) );
					break;
				}
				case 14:
				{
					m_Controlled.m_Direction = Direction.West;
					m_Controlled.Direction = Direction.West;
					m_Controlled.Say( "I am now facing West" );
					from.SendGump( new MannequinControl( m_Controlled, from, 2 ) );
					break;
				}
				case 15:
				{
					m_Controlled.Hue = 0x83EA;
					from.SendGump( new MannequinControl( m_Controlled, from, 3 ) );
					break;
				}
				case 16:
				{
					m_Controlled.Hue = 0x8414;
					from.SendGump( new MannequinControl( m_Controlled, from, 3 ) );
					break;
				}
				case 17:
				{
					m_Controlled.Hue = 0x8000;
					from.SendGump( new MannequinControl( m_Controlled, from, 3 ) );
					break;
				}
			}
		}
	}
}
