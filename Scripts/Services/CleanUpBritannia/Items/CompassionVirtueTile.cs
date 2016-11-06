using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
	public enum CompassionTileType
	{
		North = 0,
		West = 1,
	}

	public class CompassionVirtueTileAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new CompassionVirtueTileDeed(); } }

		private CompassionTileType m_CompassionTileType;

		private int offset;

		[Constructable]
		public CompassionVirtueTileAddon( CompassionTileType type )
		{
			m_CompassionTileType = type;

			offset = 0;

			if ( type == CompassionTileType.North )
			{
				offset = 4;
			}

			AddComponent( new AddonComponent( 5287 + offset ), 0, 0, 0 );
			AddComponent( new AddonComponent( 5288 + offset ), 0, 1, 0 );
			AddComponent( new AddonComponent( 5289 + offset ), 1, 1, 0 );
			AddComponent( new AddonComponent( 5290 + offset ), 1, 0, 0 );
		}

		public CompassionVirtueTileAddon( Serial serial )
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

	public class CompassionVirtueTileDeed : BaseAddonDeed, IRewardOption
	{
		public override BaseAddon Addon { get { return new CompassionVirtueTileAddon( m_CompassionTileType ); } }
		public override int LabelNumber { get { return 1080481; } } // Compassion Virtue Tile Deed

		private CompassionTileType m_CompassionTileType;

		[Constructable]
		public CompassionVirtueTileDeed()
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

		public CompassionVirtueTileDeed( Serial serial )
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
			list.Add( (int) CompassionTileType.North, 1080218 );
			list.Add( (int) CompassionTileType.West, 1080217 );
		}


		public void OnOptionSelected( Mobile from, int choice )
		{
			m_CompassionTileType = (CompassionTileType) choice;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}
	}
}
