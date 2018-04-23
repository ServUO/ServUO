#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;

using Server;

namespace Arya.Abay
{
	/// <summary>
	/// Summary description for AbayItemCheck.
	/// </summary>
	public class AbayItemCheck : AbayCheck
	{
		private static int ItemSoldHue = 2119;
		private static int ItemReturnedHue = 52;
		private Item m_Item;

		/// <summary>
		/// Creates a check that will deliver an item for the Abay system
		/// </summary>
		/// <param name="Abay">The Abay generating this check</param>
		/// <param name="result">Specifies the reason for the generation of this check</param>
		public AbayItemCheck( AbayItem Abay, AbayResult result )
		{
			Name = Abay.Creature ? AbaySystem.ST[ 131 ] : AbaySystem.ST[ 132 ];

			m_Abay = Abay.ID;
			m_ItemName = Abay.ItemName;			
			m_Item = Abay.Item;

			if ( m_Item != null )
			{
				AbaySystem.ControlStone.RemoveItem( m_Item );
				m_Item.Parent = this; // This will avoid cleanup
			}

			switch ( result )
			{
				// Returning the item to the owner
				case AbayResult.NoBids:
				case AbayResult.PendingRefused:
				case AbayResult.SystemStopped:
				case AbayResult.PendingTimedOut:
				case AbayResult.ItemDeleted:
				case AbayResult.StaffRemoved:

					m_Owner = Abay.Owner;
					Hue = ItemReturnedHue;

					switch ( result )
					{
						case AbayResult.NoBids:
							m_Message = string.Format( AbaySystem.ST[ 133 ], m_ItemName );
							break;

						case AbayResult.PendingRefused:
							m_Message = string.Format( AbaySystem.ST[ 134 ], m_ItemName );
							break;

						case AbayResult.SystemStopped:
							m_Message = string.Format( AbaySystem.ST[ 135 ], m_ItemName );
							break;

						case AbayResult.PendingTimedOut:
							m_Message = AbaySystem.ST[ 127 ];
							break;

						case AbayResult.ItemDeleted:
							m_Message = AbaySystem.ST[ 136 ];
							break;
						case AbayResult.StaffRemoved:
							m_Message = AbaySystem.ST[ 203 ];
							break;
					}
					break;

				case AbayResult.PendingAccepted:
				case AbayResult.Succesful:
				case AbayResult.BuyNow:

					m_Owner = Abay.HighestBid.Mobile;
					Hue = ItemSoldHue;
					m_Message = string.Format( AbaySystem.ST[ 137 ] , m_ItemName, Abay.HighestBid.Amount.ToString("#,0" ));
					break;

				default:
					throw new Exception( string.Format( AbaySystem.ST[ 138 ] , result.ToString() ) );
			}
		}

		public override string ItemName
		{
			get
			{
				return m_ItemName;
			}
		}

		public override Item AbayedItem
		{
			get
			{
				return m_Item;
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties (list);

			list.Add( 1060659, "Item\t{0}", m_ItemName );
		}

		public override bool Deliver( Mobile to )
		{
			if ( Delivered )
				return true;

			Item item = AbayedItem;

			if ( null == item )
			{
				to.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 116 ] );
				return false;
			}
			else if ( to.BankBox.TryDropItem( to, item, false ))
			{
				item.UpdateTotals();
				DeliveryComplete();
				Delete();
				to.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 117 ] );
				return true;
			}

			return false;
		}

		public override void OnDelete()
		{
			if ( Delivered )
				m_Item = null;
			else
				ForceDelete();

			base.OnDelete();
		}

		public void ForceDelete()
		{
			if ( m_Item != null )
			{
				if ( m_Item is MobileStatuette )
				{
					( m_Item as MobileStatuette ).ForceDelete();
				}
				else
				{
					m_Item.Delete();
				}
			}
		}

		#region Serialization

		public AbayItemCheck( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( 0 ); // Version

			writer.Write( m_Item );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();

			m_Item = reader.ReadItem();
		}

		#endregion
	}
}