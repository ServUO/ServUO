#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public class MatingGump : Gump
	{
		private Mobile m_From;
		private Mobile m_Mobile;
		private IEvoCreature m_Female;
		private IEvoCreature m_Male;

		public MatingGump( Mobile from, Mobile mobile, IEvoCreature female, IEvoCreature male ) : base( 25, 50 )
		{
			Closable = Dragable = false;

			m_From = from;
			m_Mobile = mobile;
			m_Female = female;
			m_Male = male;

			AddPage( 0 );

			AddBackground( 25, 10, 420, 200, 5054 );

			AddImageTiled( 33, 20, 401, 181, 2624 );
			AddAlphaRegion( 33, 20, 401, 181 );

			AddLabel( 125, 148, 1152, m_From.Name +" would like to mate "+ ((BaseCreature)m_Female).Name +" with" );
			AddLabel( 125, 158, 1152, ((BaseCreature)m_Male).Name +"." );

			AddButton( 100, 50, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddLabel( 130, 50, 1152, "Allow them to mate." );
			AddButton( 100, 75, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddLabel( 130, 75, 1152, "Do not allow them to mate." );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			string action = " decline";

			if ( from == null )
				return;

			if ( info.ButtonID == 1 )
			{
				m_Female.Pregnant = true;
				action = " accept";
			}
			m_From.SendMessage( m_Mobile.Name + action + "s your request to mate the two " + m_Female.Breed + "s." );
			m_Mobile.SendMessage( "You" + action + " " + m_From.Name +"'s request to mate the two " + m_Female.Breed + "s." );
		}
	}
}