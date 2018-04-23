using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class SkullRugSmallSouth : BaseAddon, IRewardItem
	{
		public override BaseAddonDeed Deed
		{ 
			get
			{ 
				SkullRugSmallSouthDeed deed = new SkullRugSmallSouthDeed();
				deed.IsRewardItem = m_IsRewardItem;
				deed.TMap = m_TMap;

				return deed; 
			} 
		}
		
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		private int m_TMap;		

		[CommandProperty( AccessLevel.GameMaster )]
		public int TMap
		{
			get{ return m_TMap; }
			set{ m_TMap = value; InvalidateProperties(); }
		}

		private Timer m_Timer;
		

		[Constructable]
		public SkullRugSmallSouth() : base()
		{
			AddComponent( new AddonComponent( 0x4709 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 0x470C ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x470F ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x470D ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x470E ),  2, -1, 0 );
			AddComponent( new AddonComponent( 0x4708 ), -2,  0, 0 );
			AddComponent( new AddonComponent( 0x470B ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x4707 ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x4710 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x470A ),  2,  0, 0 );
			AddComponent( new AddonComponent( 0x4714 ), -2,  1, 0 );
			AddComponent( new AddonComponent( 0x4713 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x4711 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x4715 ),  1,  1, 0 );
			AddComponent( new AddonComponent( 0x4712 ),  2,  1, 0 );
			
			m_Timer = Timer.DelayCall( TimeSpan.FromDays( 7 ), TimeSpan.FromDays( 7 ), new TimerCallback( GiveTMap ) );
		}

		public SkullRugSmallSouth( Serial serial ) : base( serial )
		{
		}
		
		private void GiveTMap()
		{
			m_TMap = Math.Min( 10, m_TMap +1 );
			
		}
		
		public override void OnComponentUsed( AddonComponent c, Mobile from )
		{			
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if (!from.InRange(GetWorldLocation(), 2) || !from.InLOS(this) || !((from.Z - Z) > -3 && (from.Z - Z) < 3))
			{
				from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
			}
			else if ( house != null && house.HasSecureAccess( from, SecureLevel.Friends ) )
			{
				if ( m_TMap > 0 )
				{
					Item TMap = null;

					switch ( Utility.Random( 2 ) )
					{
						case 0: TMap = new TreasureMap( Utility.RandomMinMax( 1, 6 ), Map.Trammel ); break;
						case 1: TMap = new TreasureMap( Utility.RandomMinMax( 1, 6 ), Map.Felucca ); break;
					
					}

					int amount = Math.Min( 1, m_TMap );
					TMap.Amount = amount;
					
					if ( !from.PlaceInBackpack( TMap ) )
					{
						from.SendLocalizedMessage( 1078837 ); // Your backpack is full! Please make room and try again.
					}
					else
					{
						m_TMap -= amount;
						from.SendLocalizedMessage( 1150084 ); // A treasure map has been placed in your backpack.
					}
				}
				else 
					from.SendLocalizedMessage( 1150083 ); // The item only produces resources once per week.
			}
			else 
				from.SendLocalizedMessage( 1061637 ); // You are not allowed to access this.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
			
			writer.Write( (bool) m_IsRewardItem );
			writer.Write( (int) m_TMap );

			if ( m_Timer != null )
				writer.Write( (DateTime) m_Timer.Next );
			else
				writer.Write( (DateTime) DateTime.Now + TimeSpan.FromDays( 7 ) );
		}
            
        public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			
			m_IsRewardItem = reader.ReadBool();
			m_TMap = reader.ReadInt();

			DateTime next = reader.ReadDateTime();

			if ( next < DateTime.Now )
				next = DateTime.Now;	

			m_Timer = Timer.DelayCall( next - DateTime.Now, TimeSpan.FromDays( 7 ), new TimerCallback( GiveTMap ) );
		}
		
	}

	public class SkullRugSmallSouthDeed : BaseAddonDeed, IRewardItem
	{
		
		public override int LabelNumber{ get{ return 1150046; } } // A Skull Rug (South)
		
		public override BaseAddon Addon
		{ 
			get
			{ 
				SkullRugSmallSouth addon = new SkullRugSmallSouth();
				addon.IsRewardItem = m_IsRewardItem;
				addon.TMap = m_TMap;

				return addon; 
			} 
		}
		
		//private int m_ItemID;
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}		

		private int m_TMap;		

		[CommandProperty( AccessLevel.GameMaster )]
		public int TMap
		{
			get{ return m_TMap; }
			set{ m_TMap = value; InvalidateProperties(); }
		}
		

		[Constructable]
		public SkullRugSmallSouthDeed() 
		{
			LootType = LootType.Blessed;
		}

		public SkullRugSmallSouthDeed( Serial serial ) : base( serial )
		{
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if ( m_IsRewardItem )
				list.Add( 1079808 ); //Happy 10th Anniversary! or  1041001 Happy 300th Anniversary! 
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( (bool) m_IsRewardItem );
			writer.Write( (int) m_TMap );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			
			m_IsRewardItem = reader.ReadBool();
			m_TMap = reader.ReadInt();
		}
	}
}