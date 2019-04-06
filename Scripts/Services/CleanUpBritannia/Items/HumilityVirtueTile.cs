using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
	public enum HumilityTileType
	{
		North = 0,
		West = 1,
	}

	public class HumilityVirtueTileAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new HumilityVirtueTileDeed(); } }

		private HumilityTileType m_HumilityTileType;

		private int offset;

		[Constructable]
		public HumilityVirtueTileAddon( HumilityTileType type )
		{
			m_HumilityTileType = type;

			offset = 0;

			if ( type == HumilityTileType.North )
			{
				offset = 4;
			}

			AddComponent( new AddonComponent( 5327 + offset ), 0, 0, 0 );
			AddComponent( new AddonComponent( 5328 + offset ), 0, 1, 0 );
			AddComponent( new AddonComponent( 5329 + offset ), 1, 1, 0 );
			AddComponent( new AddonComponent( 5330 + offset ), 1, 0, 0 );
		}

		public HumilityVirtueTileAddon( Serial serial )
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

	public class HumilityVirtueTileDeed : BaseAddonDeed, IRewardOption
	{
		public override BaseAddon Addon { get { return new HumilityVirtueTileAddon( m_HumilityTileType ); } }
		public override int LabelNumber { get { return 1080483; } } // Humility Virtue Tile Deed

		private HumilityTileType m_HumilityTileType;

		[Constructable]
		public HumilityVirtueTileDeed()
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

		public HumilityVirtueTileDeed( Serial serial )
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
			list.Add( (int) HumilityTileType.North, 1080233 );
			list.Add( (int) HumilityTileType.West, 1080232 );
		}

		public void OnOptionSelected( Mobile from, int choice )
		{
			m_HumilityTileType = (HumilityTileType) choice;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}
	}
}
