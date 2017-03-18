using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
	public enum HonestyTileType
	{
		North = 0,
		West = 1,
	}

	public class HonestyVirtueTileAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new HonestyVirtueTileDeed(); } }

		private HonestyTileType m_HonestyTileType;

		private int offset;

		[Constructable]
		public HonestyVirtueTileAddon( HonestyTileType type )
		{
			m_HonestyTileType = type;

			offset = 0;

			if ( type == HonestyTileType.North )
			{
				offset = 4;
			}

			AddComponent( new AddonComponent( 5279 + offset ), 0, 0, 0 );
			AddComponent( new AddonComponent( 5280 + offset ), 0, 1, 0 );
			AddComponent( new AddonComponent( 5281 + offset ), 1, 1, 0 );
			AddComponent( new AddonComponent( 5282 + offset ), 1, 0, 0 );
		}

		public HonestyVirtueTileAddon( Serial serial )
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

            int version = reader.ReadInt();
        }
	}

	public class HonestyVirtueTileDeed : BaseAddonDeed, IRewardOption
	{
		public override BaseAddon Addon { get { return new HonestyVirtueTileAddon( m_HonestyTileType ); } }
		public override int LabelNumber { get { return 1080488; } } // Honesty Virtue Tile Deed

		private HonestyTileType m_HonestyTileType;

		[Constructable]
		public HonestyVirtueTileDeed()
		{
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( RewardOptionGump ) );
				from.SendGump( new RewardOptionGump( this ) );
			}
			else
				from.SendLocalizedMessage( 1062334 ); // This item must be in your backpack to be used.       	
		}

		public HonestyVirtueTileDeed( Serial serial )
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

            int version = reader.ReadInt();
        }

		public void GetOptions( RewardOptionList list )
		{
			list.Add( (int) HonestyTileType.North, 1080215 );
			list.Add( (int) HonestyTileType.West, 1080214 );
		}


		public void OnOptionSelected( Mobile from, int choice )
		{
			m_HonestyTileType = (HonestyTileType) choice;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}
	}
}
