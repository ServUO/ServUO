#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Commands;
using Xanthos.Utilities;

namespace Arya.Abay
{
	/// <summary>
	/// Staff control gump
	/// </summary>
	public class AbayControlGump : Gump
	{
		Mobile m_User;
		AbayItem m_Abay;

		public AbayControlGump( Mobile user, AbayItem Abay, AbayViewGump view ) : base( 50, 50 )
		{
			m_User = user;
			m_Abay = Abay;

			m_User.CloseGump( typeof( AbayControlGump ) );

			view.X = 400;
			m_User.SendGump( view );

			MakeGump();
		}

		private void MakeGump()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddBackground(0, 0, 300, 385, 9270);
			AddAlphaRegion(10, 10, 280, 365);

			AddLabel(99, 15, AbayLabelHue.kGreenHue, AbaySystem.ST[ 213 ] );
			AddImageTiled(15, 35, 270, 1, 9304);
			AddImageTiled(30, 55, 1, 65, 9304);

			// Abay owner
			bool owner = m_Abay.Owner != null;
			AddLabel(15, 35, AbayLabelHue.kRedHue, string.Format( AbaySystem.ST[ 216 ], owner ? m_Abay.Owner.Name : AbaySystem.ST[ 78 ] ));

			// Owner Props : 1
			if ( owner )
			{
				AddButton(35, 58, 9702, 9703, 1, GumpButtonType.Reply, 0);
			}
			AddLabel(55, 55, AbayLabelHue.kLabelHue, AbaySystem.ST[ 214 ] );;
			
			// Owner Account: 2
			if ( owner && m_Abay.Owner.Account != null && m_User.AccessLevel == AccessLevel.Administrator )
			{
				AddButton(35, 78, 9702, 9703, 2, GumpButtonType.Reply, 0);
			}
			AddLabel(55, 75, AbayLabelHue.kLabelHue, string.Format( AbaySystem.ST[ 215 ], ( owner && m_Abay.Owner.Account != null ) ? ( m_Abay.Owner.Account as Server.Accounting.Account ).Username : AbaySystem.ST[ 78 ] ) );

			bool online = m_Abay.Owner != null && m_Abay.Owner.NetState != null;
			
			// Client button : 3
			if ( online )
			{
				AddButton(35, 98, 9702, 9703, 3, GumpButtonType.Reply, 0);
			}
			AddLabel(55, 95, online ? AbayLabelHue.kGreenHue : AbayLabelHue.kRedHue, online ? AbaySystem.ST[ 217 ] : AbaySystem.ST[ 218 ] );

			AddImageTiled(15, 120, 270, 1, 9304);
			AddLabel(20, 125, AbayLabelHue.kRedHue, AbaySystem.ST[ 219 ]);
			AddImageTiled(30, 145, 1, 65, 9304);

			// Item Properties : 4
			AddButton(35, 150, 9702, 9703, 4, GumpButtonType.Reply, 0);
			AddLabel(55, 147, AbayLabelHue.kLabelHue, AbaySystem.ST[ 214 ]);

			bool item = m_Abay.Item != null;

			// Get item : 5
			AddButton(35, 170, 9702, 9703, 5, GumpButtonType.Reply, 0);
			AddLabel(55, 167, AbayLabelHue.kLabelHue, AbaySystem.ST[ 220 ] );
            
			// Return item: 6
			AddButton(35, 190, 9702, 9703, 6, GumpButtonType.Reply, 0);
			AddLabel(55, 187, AbayLabelHue.kLabelHue, AbaySystem.ST[ 221 ] );

			AddImageTiled(15, 210, 270, 1, 9304);
			AddLabel(20, 215, AbayLabelHue.kRedHue, AbaySystem.ST[ 222 ] );
			AddImageTiled(30, 235, 1, 110, 9304);

			// Abay Properties: 7
			AddButton(35, 240, 9702, 9703, 7, GumpButtonType.Reply, 0);
			AddLabel(55, 237, AbayLabelHue.kLabelHue, AbaySystem.ST[ 214 ] );

			// End Abay now: 8
			AddButton(35, 260, 9702, 9703, 8, GumpButtonType.Reply, 0);
			AddLabel(55, 257, AbayLabelHue.kLabelHue, AbaySystem.ST[ 223 ] );

			// Close and return: 9
			AddButton(35, 280, 9702, 9703, 9, GumpButtonType.Reply, 0);
			AddLabel(55, 277, AbayLabelHue.kLabelHue, AbaySystem.ST[ 224 ] );
			
			// Close and get: 10
			AddButton(35, 300, 9702, 9703, 10, GumpButtonType.Reply, 0);
			AddLabel(55, 297, AbayLabelHue.kLabelHue, AbaySystem.ST[ 225 ] );

			// Close and delete: 11
			AddButton(35, 320, 9702, 9703, 11, GumpButtonType.Reply, 0);
			AddLabel(55, 317, AbayLabelHue.kLabelHue, AbaySystem.ST[ 226 ] );

			AddImageTiled(15, 345, 270, 1, 9304);

			// Exit: 0
			AddButton(15, 350, 9702, 9703, 0, GumpButtonType.Reply, 0);
			AddLabel(35, 348, AbayLabelHue.kLabelHue, AbaySystem.ST[ 14 ] );
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			switch ( info.ButtonID )
			{
				case 0: // Exit
					break;

				case 1: // Owner Props
					m_User.SendGump( this );
					m_User.SendGump( new PropertiesGump( m_User, m_Abay.Owner ) );
					break;

				case 2: // Owner Account
					m_User.SendGump( this );
					m_User.SendGump( new AdminGump( m_User, AdminGumpPage.AccountDetails_Information, 0, null, "Request from the Abay system", m_Abay.Account ) );
					break;

				case 3: // Owner Client
					m_User.SendGump( this );

					if ( m_Abay.Owner.NetState != null )
					{
						m_User.SendGump( new ClientGump( m_User, m_Abay.Owner.NetState ) );
					}
					break;

				case 4: // Item Props
					m_User.SendGump( this );
					m_User.SendGump( new PropertiesGump( m_User, m_Abay.Item ) );
					break;

				case 5: // Get Item
					m_User.SendGump( this );

					if ( m_Abay.Creature && m_Abay.Pet != null )
					{
						m_User.SendMessage( "That cannot be used on pets" );
					}
					else
					{
						m_User.Backpack.AddItem( m_Abay.Item );
					}
					AbayLog.WriteViewItem( m_Abay, m_User );
					break;

				case 6: // Return Item
					if ( m_Abay.Creature )
					{
						m_Abay.Pet.SetControlMaster( null );
						m_Abay.Pet.ControlOrder = Server.Mobiles.OrderType.Stay;
						m_Abay.Pet.Internalize();
					}
					else
					{
						Item item = m_Abay.Item;

						if ( item != null )
						{
							if ( item.Parent is Mobile)
							{
								( (Mobile) item.Parent ).RemoveItem( item );
							}
							else if ( item.Parent is Item)
							{
								( (Item) item.Parent ).RemoveItem( item );
							}
						}

						AbaySystem.ControlStone.AddItem( m_Abay.Item );
						m_Abay.Item.Parent = AbaySystem.ControlStone;
					}
					AbayLog.WriteReturnItem( m_Abay, m_User );
					m_User.SendGump( this );
					break;

				case 7: // Abay Props
					m_User.SendGump( this );
					m_User.SendGump( new PropertiesGump( m_User, m_Abay ) );
					break;

				case 8: // End Abay
					m_Abay.End( m_User );
					goto case 0;

				case 9: // End and Return
					m_Abay.StaffDelete( m_User, ItemFate.ReturnToOwner );
					goto case 0;

				case 10: // End and Get
					m_Abay.StaffDelete( m_User, ItemFate.ReturnToStaff );
					goto case 0;

				case 11: // End and Delete
					m_Abay.StaffDelete( m_User, ItemFate.Delete );
					goto case 0;
			}
		}
	}
}