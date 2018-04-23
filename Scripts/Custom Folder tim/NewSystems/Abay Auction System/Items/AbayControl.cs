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

namespace Arya.Abay
{
	/// <summary>
	/// This is the Abay control stone. This item should NOT be deleted
	/// </summary>
	public class AbayControl : Item
	{
		/// <summary>
		/// This item holds all the current Abays
		/// </summary>
		private ArrayList m_Abays;
		/// <summary>
		/// This lists all Abays whose reserve hasn't been met
		/// </summary>
		private ArrayList m_Pending;
		/// <summary>
		/// Flag used to force the deletion of the system
		/// </summary>
		private bool m_Delete = false;
		/// <summary>
		/// The max number of concurrent Abays for each account
		/// </summary>
		private int m_MaxAbaysParAccount = 5;
		/// <summary>
		/// The minimum number of days an Abay must last
		/// </summary>
		private int m_MinAbayDays = 1;
		/// <summary>
		/// The max number of days an Abay can last
		/// </summary>
		private int m_MaxAbayDays = 14;

		/// <summary>
		/// Gets or sets the list of current Abay entries
		/// </summary>
		public ArrayList Abays
		{
			get { return m_Abays; }
			set { m_Abays = value; }
		}

		/// <summary>
		/// Gets or sets the pending Abay entries
		/// </summary>
		public ArrayList Pending
		{
			get { return m_Pending; }
			set { m_Pending = value; }
		}

		[ CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator ) ]
		/// <summary>
		/// Gets or sets the max number of Abays a single account can have
		/// </summary>
		public int MaxAbaysParAccount
		{
			get { return m_MaxAbaysParAccount; }
			set { m_MaxAbaysParAccount = value; }
		}

		[ CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator ) ]
		/// <summary>
		/// Gets or sets the minimum days an Abay must last
		/// </summary>
		public int MinAbayDays
		{
			get { return m_MinAbayDays; }
			set { m_MinAbayDays = value; }
		}

		[ CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator ) ]
		/// <summary>
		/// Gets or sets the max number of days an Abay can last
		/// </summary>
		public int MaxAbayDays
		{
			get { return m_MaxAbayDays; }
			set { m_MaxAbayDays = value; }
		}

		public AbayControl() : base( 4484 )
		{
			Name = "Abay System";
			Visible = false;
			Movable = false;
			m_Abays = new ArrayList();
			m_Pending = new ArrayList();

			AbaySystem.ControlStone = this;
		}

		public AbayControl( Serial serial ) : base( serial )
		{
			m_Abays = new ArrayList();
			m_Pending = new ArrayList();
		}

		#region Serialization

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( 1 ); // Version

			// Version 1 : changes in AbayItem
			// Version 0
			writer.Write( m_MaxAbaysParAccount );
			writer.Write( m_MinAbayDays );
			writer.Write( m_MaxAbayDays );

			writer.Write( m_Abays.Count );

			foreach( AbayItem Abay in m_Abays )
			{
				Abay.Serialize( writer );
			}

			writer.Write( m_Pending.Count );

			foreach( AbayItem Abay in m_Pending )
			{
				Abay.Serialize( writer );
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				case 0:
					m_MaxAbaysParAccount = reader.ReadInt();
					m_MinAbayDays = reader.ReadInt();
					m_MaxAbayDays = reader.ReadInt();

					int count = reader.ReadInt();

					for ( int i = 0; i < count; i++ )
					{
						m_Abays.Add( AbayItem.Deserialize( reader, version ) );
					}

					count = reader.ReadInt();

					for ( int i = 0; i < count; i++ )
					{
						m_Pending.Add( AbayItem.Deserialize( reader, version ) );
					}
					break;
			}

			AbaySystem.ControlStone = this;
		}

		#endregion

		public override void OnDelete()
		{
			// Don't allow users to delete this item unless it's done through the control gump
			if ( !m_Delete )
			{
				AbayControl newStone = new AbayControl();
				newStone.m_Abays.AddRange( this.m_Abays );
				newStone.MoveToWorld( this.Location, this.Map );
				
				newStone.Items.AddRange( Items );
				Items.Clear();
				foreach( Item item in newStone.Items )
				{
					item.Parent = newStone;
				}

				newStone.PublicOverheadMessage( Server.Network.MessageType.Regular, 0x40, false, AbaySystem.ST[ 121 ] );
			}

			base.OnDelete ();
		}

		/// <summary>
		/// Deletes the item from the world without triggering the auto-recreation
		/// This function also closes all current Abays
		/// </summary>
		public void ForceDelete()
		{
			m_Delete = true;
			Delete();
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties (list);

			list.Add( AbaySystem.Running ? 3005117 : 3005118 ); // [Active] - [Inactive]
		}
	}
}