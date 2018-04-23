#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Arya.Abay
{
	/// <summary>
	/// Defines various conditions that can apply when an Abay is terminated
	/// </summary>
	public enum AbayResult
	{
		/// <summary>
		/// The Abay has been succesful and the item has been sold.
		/// </summary>
		Succesful,
		/// <summary>
		/// The Abay ends and no bids have been made. The item will be returned to the owner.
		/// </summary>
		NoBids,
		/// <summary>
		/// The Abay has ended, and there have been bids but the reserve hasn't been met.
		/// </summary>
		ReserveNotMet,
		/// <summary>
		/// A user has been outbid in an Abay
		/// </summary>
		Outbid,
		/// <summary>
		/// The Abay had pending status and both parts agreed to finalize the Abay
		/// </summary>
		PendingRefused,
		/// <summary>
		/// The Abay had pending status and at least one part decided to cancel the Abay
		/// </summary>
		PendingAccepted,
		/// <summary>
		/// The pending period has timed out
		/// </summary>
		PendingTimedOut,
		/// <summary>
		/// The Abay System has been forced to stop and the Abay ends unsuccesfully
		/// </summary>
		SystemStopped,
		/// <summary>
		/// The Abayed item has been deleted from the world
		/// </summary>
		ItemDeleted,
		/// <summary>
		/// The Abay has been removed from the system by the staff
		/// </summary>
		StaffRemoved,
		/// <summary>
		/// The Abay ended because a buyer used the buy now feature
		/// </summary>
		BuyNow
	}

	/// <summary>
	/// Base class for the Abay system checks
	/// </summary>
	public abstract class AbayCheck : Item
	{
		protected Guid m_Abay;
		protected string m_Message;
		protected string m_ItemName;
		protected Mobile m_Owner;
		private bool m_Delivered;

		/// <summary>
		/// Gets the message accompanying this check
		/// </summary>
		public string Message
		{
			get { return m_Message; }
		}

		/// <summary>
		/// Gets the Abay that originated this check. This value might be null
		/// </summary>
		public AbayItem Abay
		{
			get
			{
				return AbaySystem.Find( m_Abay );
			}
		}

		/// <summary>
		/// True once the Abay item has been delivered
		/// </summary>
		public bool Delivered
		{
			get
			{
				return m_Delivered;
			}
		}

		/// <summary>
		/// Gets the html message used in gumps
		/// </summary>
		public string HtmlDetails
		{
			get { return string.Format( "<basefont color=#FFFFFF>{0}", m_Message ); }
		}

		/// <summary>
		/// Gets the name of the item returned by this check
		/// </summary>
		public abstract string ItemName
		{
			get;
		}

		public AbayCheck() : base( 5360 )
		{
			LootType = LootType.Blessed;
			m_Delivered = false;
		}

		public AbayCheck( Serial serial ) : base ( serial )
		{
		}

		/// <summary>
		/// Gets the item that should be delivered to the players bank
		/// </summary>
		public virtual Server.Item AbayedItem
		{
			get { return null; }
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( AbayedItem is MobileStatuette )
			{
				// Send pet retrieval gump
				from.CloseGump( typeof( CreatureDeliveryGump ) );
				from.SendGump( new CreatureDeliveryGump( this ) );
			}
			else
			{
				// Send item retrieval gump
				from.CloseGump( typeof( AbayDeliveryGump ) );
				from.SendGump( new AbayDeliveryGump( this ) );
			}
		}		

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties (list);

			list.Add( 1060658, "Message\t{0}", m_Message ); // ~1_val~: ~2_val~
		}

		/// <summary>
		/// Delivers the item carried by this check
		/// </summary>
		/// <param name="to">The mobile the check should be delivered to</param>
		/// <returns>True if the item has been delivered to the player's bank</returns>
		public abstract bool Deliver( Mobile to );

		public void DeliveryComplete()
		{
			m_Delivered = true;
		}
		
		#region Serialization

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( 0 ); // Version

			writer.Write( m_Abay.ToString() );
			writer.Write( m_Message );
			writer.Write( m_ItemName );
			writer.Write( m_Owner );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();

			m_Abay = new Guid( reader.ReadString() );
			m_Message = reader.ReadString();
			m_ItemName = reader.ReadString();
			m_Owner = reader.ReadMobile();
		}

		#endregion
	}
}
