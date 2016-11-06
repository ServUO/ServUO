using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
	public enum HorseBardingType
	{
		South = 0,
		East = 1,
	}

	public class HorseBardingAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new HorseBardingDeed(); } }

		private HorseBardingType m_HorseBardingType;

		[Constructable]
		public HorseBardingAddon( HorseBardingType type )
		{
			m_HorseBardingType = type;

			switch ( type )
			{
				case HorseBardingType.South:
					AddComponent( new AddonComponent( 4984 ), 0, 0, 0 );
					AddComponent( new AddonComponent( 4985 ), 0, -1, 0 );
					break;
				case HorseBardingType.East:
					AddComponent( new AddonComponent( 4983 ), 0, 0, 0 );
					AddComponent( new AddonComponent( 4982 ), -1, 0, 0 );
					break;

			}
		}

		public HorseBardingAddon( Serial serial )
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

	public class HorseBardingDeed : BaseAddonDeed, IRewardOption
	{
		public override BaseAddon Addon { get { return new HorseBardingAddon( m_HorseBardingType ); } }

		private HorseBardingType m_HorseBardingType;

		public override int LabelNumber { get { return 1080212; } } // Horse Barding

		[Constructable]
		public HorseBardingDeed()
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

		public HorseBardingDeed( Serial serial )
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
			list.Add( (int) HorseBardingType.South, 1080211 );
			list.Add( (int) HorseBardingType.East, 1080210 );
		}


		public void OnOptionSelected( Mobile from, int choice )
		{
			m_HorseBardingType = (HorseBardingType) choice;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}
	}
}
