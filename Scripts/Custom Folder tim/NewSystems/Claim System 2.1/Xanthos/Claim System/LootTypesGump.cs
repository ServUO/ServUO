#region AuthorHeader
//
//  Claim System version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Targeting;

namespace Xanthos.Claim
{
	public class LootTypesGump : Gump
	{
		private const int kLabelHue = 0x480;
		private const int kGreenHue = 0x40;
		private const int kRedHue = 0x20;
		private const int kCloseButton = 0;
		private const int kNextButton = 1;
		private const int kPrevButton = 2;
		private const int kNewTypeButton = 3;
		private const int kTypesPerPage = 10;

		private LootBag m_Bag;
		private int m_Page;

		public LootTypesGump( Mobile mobile, LootBag bag, int page ) : base( 50, 50 )
		{
			if ( null != bag && !bag.Deleted )
			{
				m_Bag = bag;
				m_Page = page;

				while ( m_Page * kTypesPerPage >= m_Bag.TypesToLoot.Count )
					m_Page--;

				mobile.CloseGump( typeof( LootTypesGump ) );
				MakeGump();
			}
		}

		public LootTypesGump( Mobile m, LootBag bag ) : this( m, bag, 0 )
		{
		}

		public LootTypesGump( Mobile m, LootBag bag, bool goLastPage ) : this( m, bag, bag.TypesToLoot.Count / kTypesPerPage )
		{
		}

		private void MakeGump()
		{
			List<Type> types = m_Bag.TypesToLoot;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			// Background
			AddPage( 0 );
			AddImageTiled( 49, 39, 302, 352, 3004 );
			AddImageTiled( 50, 40, 300, 350, 2624 );
			AddAlphaRegion( 50, 40, 299, 349 );

			// Header and footer
			AddLabel( 145, 50, kRedHue, @"Types To Collect" );
			if ( types.Count > kTypesPerPage )
				AddLabel( 160, 355, kRedHue, string.Format( "Page {0} of {1}", m_Page + 1, ( types.Count - 1 ) / kTypesPerPage + 1 ));

			// Types
			if ( types.Count > 0 )
			{
				for ( int i = 0; i < kTypesPerPage && (( m_Page * kTypesPerPage ) + i ) < types.Count; i++ )
				{
					AddButton( 70, 80 + i * 20, 4017, 4018, kTypesPerPage + i, GumpButtonType.Reply, 0 );
					AddLabelCropped( 110, 82 + i * 20, 260, 20, kLabelHue, FriendlyName( types[ m_Page * kTypesPerPage + i ].Name ));
				}
			}

			// Add New Type
			AddButton( 70, 300, 5601, 5605, kNewTypeButton, GumpButtonType.Reply, 0 );
			AddLabel( 100, 300, kRedHue, @"Add New Type..." );

			// Next page
			if ( ( m_Page + 1 ) * kTypesPerPage < types.Count )
			{
				AddButton( 306, 355, 4005, 4006, kNextButton, GumpButtonType.Reply, 0 );
			}

			// Previous page
			if ( m_Page > 0 )
			{
				AddButton( 268, 355, 4014, 4015, kPrevButton, GumpButtonType.Reply, 0 );
			}

			// Close
			AddButton( 65, 355, 4023, 4024, kCloseButton, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case kNewTypeButton:
					sender.Mobile.CloseGump( typeof( LootTypesGump ) );
					sender.Mobile.Target = new SetCollectionTypeTarget( sender.Mobile, m_Bag );
					break;

				case kNextButton:
					sender.Mobile.SendGump( new LootTypesGump( sender.Mobile, m_Bag, m_Page + 1 ) );
					break;

				case kPrevButton:
					sender.Mobile.SendGump( new LootTypesGump( sender.Mobile, m_Bag, m_Page - 1 ) );
					break;

				case kCloseButton:
					sender.Mobile.CloseGump( typeof( LootTypesGump ) );
					break;

				default:
					if ( null != m_Bag && !m_Bag.Deleted )
						m_Bag.RemoveTypeAt(( m_Page * kTypesPerPage ) + ( info.ButtonID - kTypesPerPage ));
					sender.Mobile.SendGump( new LootTypesGump( sender.Mobile, m_Bag, m_Page * kTypesPerPage >= m_Bag.TypesToLoot.Count ? m_Page - 1 : m_Page ) );
					break;
			}
		}

		public static string FriendlyName( string name )
		{
			string newName = name.Substring( name.StartsWith( "Base" )? 4 : 0 );

			if ( newName.Equals( "Jewel" ) )	// Just for clarity
				return "Jewelry";

			return Xanthos.Utilities.Misc.GetFriendlyClassName( newName );
		}

		private class SetCollectionTypeTarget : Target
		{
			LootBag m_Bag;

			public SetCollectionTypeTarget( Mobile from, LootBag bag ) : base( -1, false, TargetFlags.None )
			{
				from.SendMessage( "Choose an item to be collected by this loot bag." );
				m_Bag = bag;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				Item item = target as Item;
				Type type;

				if ( null == target )
					return;

				else if ( null == item )
					from.SendMessage( "That is not something that can be collected." );

				else if ( null != m_Bag && !m_Bag.Deleted && ( type = item.GetType() ) != null )
				{
					m_Bag.AddType( type );
					from.SendGump( new LootTypesGump( from, m_Bag, true ) );
				}
			}
		}
	}
}
