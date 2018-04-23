using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Engines.Plants;

namespace Server.Items
{
	public class RoseRugSmallSouth : BaseAddon, IRewardItem
	{
		public override BaseAddonDeed Deed
		{ 
			get
			{ 
				RoseRugSmallSouthDeed deed = new RoseRugSmallSouthDeed();
				deed.IsRewardItem = m_IsRewardItem;
				deed.RSeed = m_RSeed;

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

		private int m_RSeed;	//TODO RANDOM SEED	

		[CommandProperty( AccessLevel.GameMaster )]
		public int RSeed
		{
			get{ return m_RSeed; }
			set{ m_RSeed = value; InvalidateProperties(); }
		}

		private Timer m_Timer;
		

		[Constructable]
		public RoseRugSmallSouth() : base()
		{
			AddComponent( new AddonComponent( 18274 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 18272 ), -2,  0, 0 );
			AddComponent( new AddonComponent( 18273 ), -2,  1, 0 );
			AddComponent( new AddonComponent( 18265 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 18264 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 18263 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 18268 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 18260 ),  0,  0, 0 );
			AddComponent( new AddonComponent( 18266 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 18271 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 18270 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 18269 ),  1,  1, 0 );
			AddComponent( new AddonComponent( 18262 ),  2, -1, 0 );
			AddComponent( new AddonComponent( 18261 ),  2,  0, 0 );
			AddComponent( new AddonComponent( 18267 ),  2,  1, 0 );
			
			m_Timer = Timer.DelayCall( TimeSpan.FromDays( 7 ), TimeSpan.FromDays( 7 ), new TimerCallback( GiveRSeed ) );
		}

		public RoseRugSmallSouth( Serial serial ) : base( serial )
		{
		}
		
		private void GiveRSeed()
		{
			m_RSeed = Math.Min( 10, m_RSeed +1 );
			
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
				if ( m_RSeed > 0 )
				{
					Item RSeed = null;

					switch ( Utility.Random( 4 ) )
					{
						case 0: RSeed = new Seed ( PlantTypeInfo.RandomPeculiarGroupOne(), PlantHue.Plain, false ); break;
						case 1: RSeed = new Seed ( PlantTypeInfo.RandomPeculiarGroupTwo(), PlantHue.Plain, false ); break;
						case 2: RSeed = new Seed ( PlantTypeInfo.RandomPeculiarGroupThree(), PlantHue.Plain, false ); break;
						case 3: RSeed = new Seed ( PlantTypeInfo.RandomPeculiarGroupFour(), PlantHue.Plain, false ); break;
						//case 0: RSeed = new TreasureMap( Utility.RandomMinMax( 1, 6 )/, Map.Trammel ); break;
						//case 1: RSeed = new TreasureMap( Utility.RandomMinMax( 1, 6 ), Map.Felucca ); break;
					
					}

					int amount = Math.Min( 1, m_RSeed );
					RSeed.Amount = amount;
					
					if ( !from.PlaceInBackpack( RSeed ) )
					{
						from.SendLocalizedMessage( 1078837 ); // Your backpack is full! Please make room and try again.
					}
					else
					{
						m_RSeed -= amount;
						from.SendLocalizedMessage( 1150085 ); // A seed has been placed in your backpack.
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
			writer.Write( (int) m_RSeed );

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
			m_RSeed = reader.ReadInt();

			DateTime next = reader.ReadDateTime();

			if ( next < DateTime.Now )
				next = DateTime.Now;	

			m_Timer = Timer.DelayCall( next - DateTime.Now, TimeSpan.FromDays( 7 ), new TimerCallback( GiveRSeed ) );
		}
		
	}

	public class RoseRugSmallSouthDeed : BaseAddonDeed, IRewardItem
	{
		
		public override int LabelNumber{ get{ return 1150048; } } //A Rose Rug (South)
		
		public override BaseAddon Addon
		{ 
			get
			{ 
				RoseRugSmallSouth addon = new RoseRugSmallSouth();
				addon.IsRewardItem = m_IsRewardItem;
				addon.RSeed = m_RSeed;

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

		private int m_RSeed;		

		[CommandProperty( AccessLevel.GameMaster )]
		public int RSeed
		{
			get{ return m_RSeed; }
			set{ m_RSeed = value; InvalidateProperties(); }
		}
		

		[Constructable]
		public RoseRugSmallSouthDeed() 
		{
			LootType = LootType.Blessed;
		}

		public RoseRugSmallSouthDeed( Serial serial ) : base( serial )
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
			writer.Write( (int) m_RSeed );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			
			m_IsRewardItem = reader.ReadBool();
			m_RSeed = reader.ReadInt();
		}
	}
}