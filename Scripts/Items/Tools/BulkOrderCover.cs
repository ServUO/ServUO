using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Engines.BulkOrders;

namespace Server.Items
{
	public class BulkOrderCover : Item, IUsesRemaining
	{
		public override int LabelNumber { get { return m_LabelNumber; } }

		private int m_UsesRemaining;
		private int m_LabelNumber;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		public bool ShowUsesRemaining { get { return true; } set { } }

		[Constructable]
		public BulkOrderCover( int hue, int labelNumber )
			: base( 0x2831 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			Hue = hue;

			m_LabelNumber = labelNumber;
			m_UsesRemaining = 30;
		}

		public BulkOrderCover( Serial serial )
			: base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 1071121 ); // Select the bulk order book you want to replace a cover.
			from.Target = new InternalTarget( this );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( ShowUsesRemaining )
				list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~			
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );

			writer.Write( (int) m_UsesRemaining );
			writer.Write( m_LabelNumber );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
					m_UsesRemaining = reader.ReadInt();
					goto case 0;

				case 0:
					m_LabelNumber = reader.ReadInt();
					break;
			}
		}

		private class InternalTarget : Target
		{
			protected BulkOrderCover m_Cover;

			public InternalTarget( BulkOrderCover cover )
				: base( 1, false, TargetFlags.None )
			{
				m_Cover = cover;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is BulkOrderBook )
				{
					Item book = (Item) targeted;

					if ( book.Hue == m_Cover.Hue )
					{
						from.SendLocalizedMessage( 1071122 ); // You cannot cover it with same color.
					}
					else
					{
						book.Hue = m_Cover.Hue;

						from.SendLocalizedMessage( 1071119 ); // You have successfully given the bulk order book a new cover.
						m_Cover.UsesRemaining -= 1;
						m_Cover.InvalidateProperties();

						if ( m_Cover.UsesRemaining <= 0 )
						{
							from.SendLocalizedMessage( 1071120 ); // You have used up all the bulk order book covers.
							m_Cover.Delete();
						}
					}
				}
				else
				{
					from.SendLocalizedMessage( 1071118 ); // You can only cover a bulk order book with this item.
				}
			}
		}
	}

	public class BagForBulkOrderCovers : Bag
	{
		public override int LabelNumber { get { return 1071116; } } // Bag for bulk order covers

		[Constructable]
		public BagForBulkOrderCovers()
		{
			Hue = 0x19B;

			DropItem( new BulkOrderCover( 0, 1071097 ) ); // Normal
			DropItem( new BulkOrderCover( 2419, 1071101 ) ); // Dull Copper
			DropItem( new BulkOrderCover( 2406, 1071107 ) ); // Shadow Iron
			DropItem( new BulkOrderCover( 2413, 1071108 ) ); // Copper
			DropItem( new BulkOrderCover( 2418, 1071109 ) ); // Bronze
			DropItem( new BulkOrderCover( 2213, 1071112 ) ); // Gold
			DropItem( new BulkOrderCover( 2425, 1071113 ) ); // Agapite
			DropItem( new BulkOrderCover( 2207, 1071114 ) ); // Verite
			DropItem( new BulkOrderCover( 2219, 1071115 ) ); // Valorite
			DropItem( new BulkOrderCover( 2220, 1071098 ) ); // Spined
			DropItem( new BulkOrderCover( 2117, 1071099 ) ); // Horned
			DropItem( new BulkOrderCover( 2129, 1071100 ) ); // Barbed
		}

		public BagForBulkOrderCovers( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}
}
