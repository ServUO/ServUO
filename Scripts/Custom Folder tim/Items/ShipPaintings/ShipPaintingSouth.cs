using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class ShipPaintingSouth : BaseAddon, IRewardItem
	{
		public override BaseAddonDeed Deed
		{ 
			get
			{ 
				ShipPaintingSouthDeed deed = new ShipPaintingSouthDeed();
				deed.IsRewardItem = m_IsRewardItem;
				deed.LargePowder = m_LargePowder;

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
		
		private int m_LargePowder;		

		[CommandProperty( AccessLevel.GameMaster )]
		public int LargePowder
		{
			get{ return m_LargePowder; }
			set{ m_LargePowder = value; InvalidateProperties(); }
		}

		private Timer m_Timer;
		
		[Constructable]
		public ShipPaintingSouth() : base()
		{
			AddComponent( new AddonComponent( 0x4C26 ), 0, 0, 0 );
			
			m_Timer = Timer.DelayCall( TimeSpan.FromDays( 1 ), TimeSpan.FromDays( 1 ), new TimerCallback( GiveLargePowder ) );
		}

		public ShipPaintingSouth( Serial serial ) : base( serial )
		{
		}
		
		private void GiveLargePowder()
		{
			m_LargePowder = Math.Min( 10, m_LargePowder +1 );
			
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
				if ( m_LargePowder > 0 )
				{
					Item LargePowder = null;
					LargePowder = new HeavyPowderCharge();

					/*switch ( Utility.Random( 2 ) )
					{
						case 0: LargePowder = new HeavyPowderCharge(); break;
						case 1: LargePowder = new Saltpeter(); break;
					
					}*/

					int amount = Math.Min( 1, m_LargePowder );
					LargePowder.Amount = amount;
					
					if ( !from.PlaceInBackpack( LargePowder ) )
					{
						from.SendLocalizedMessage( 1078837 ); // Your backpack is full! Please make room and try again.
					}
					else
					{
						m_LargePowder -= amount;
						from.SendMessage( "A heavy powder charge has been placed in your backpack" ); // A treasure map has been placed in your backpack.
					}
				}
				else 
					from.SendMessage( "The item only produces resources once per day." ); // The item only produces resources once per week.
			}
			else 
				from.SendLocalizedMessage( 1061637 ); // You are not allowed to access this.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
			
			writer.Write( (bool) m_IsRewardItem );
			writer.Write( (int) m_LargePowder );

			if ( m_Timer != null )
				writer.Write( (DateTime) m_Timer.Next );
			else
				writer.Write( (DateTime) DateTime.UtcNow + TimeSpan.FromDays( 1 ) );
		}
            
        public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			
				m_IsRewardItem = reader.ReadBool();
			m_LargePowder = reader.ReadInt();

			DateTime next = reader.ReadDateTime();

			if ( next < DateTime.UtcNow )
				next = DateTime.UtcNow;	

			m_Timer = Timer.DelayCall( next - DateTime.UtcNow, TimeSpan.FromDays( 1 ), new TimerCallback( GiveLargePowder ) );
		}
		
	}
	
	public class ShipPaintingSouthDeed : BaseAddonDeed, IRewardItem
	{
		
		
		public override int LabelNumber{ get{ return 1154180; } } //Ship Painting 
		public override BaseAddon Addon
		{ 
			get
			{ 
				ShipPaintingSouth addon = new ShipPaintingSouth();
				addon.IsRewardItem = m_IsRewardItem;
				addon.LargePowder = m_LargePowder;

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

		private int m_LargePowder;		

		[CommandProperty( AccessLevel.GameMaster )]
		public int LargePowder
		{
			get{ return m_LargePowder; }
			set{ m_LargePowder = value; InvalidateProperties(); }
		}
		

		[Constructable]
		public ShipPaintingSouthDeed() 
		{
			LootType = LootType.Blessed;
		}

		public ShipPaintingSouthDeed( Serial serial ) : base( serial )
		{
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if ( m_IsRewardItem )
				list.Add( 1041001 ); //Happy 10th Anniversary! or  1041001 Happy 300th Anniversary! 
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( (bool) m_IsRewardItem );
			writer.Write( (int) m_LargePowder );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			
			m_IsRewardItem = reader.ReadBool();
			m_LargePowder = reader.ReadInt();
		}
	}
}