using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class DolphinRugNS : BaseAddon, IRewardItem
	{
		public override BaseAddonDeed Deed
		{ 
			get
			{ 
				DolphinRugNSDeed deed = new DolphinRugNSDeed();
				deed.IsRewardItem = m_IsRewardItem;
				deed.MiB = m_MiB;

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

		private int m_MiB;		

		[CommandProperty( AccessLevel.GameMaster )]
		public int MiB
		{
			get{ return m_MiB; }
			set{ m_MiB = value; InvalidateProperties(); }
		}

		private Timer m_Timer;
		

		[Constructable]
		public DolphinRugNS() : base()
		{
			AddComponent( new AddonComponent( 18284 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 18288 ),  0, 2, 0 );
			AddComponent( new AddonComponent( 18289 ), 	1, 2, 0 );
			AddComponent( new AddonComponent( 18282 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 18277 ), 	0, 1, 0 );
			AddComponent( new AddonComponent( 18276 ), 	1, 1, 0 );
			AddComponent( new AddonComponent( 18281 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 18275 ),  0, 0, 0 );
			AddComponent( new AddonComponent( 18283 ),  1, 0, 0 );
			AddComponent( new AddonComponent( 18278 ), -1,-1, 0 );
			AddComponent( new AddonComponent( 18279 ),  0,-1, 0 );
			AddComponent( new AddonComponent( 18280 ),  1,-1, 0 );
			AddComponent( new AddonComponent( 18287 ), -1,-2, 0 );
			AddComponent( new AddonComponent( 18285 ),  0,-2, 0 );
			AddComponent( new AddonComponent( 18286 ),  1,-2, 0 );
			
			m_Timer = Timer.DelayCall( TimeSpan.FromDays( 7 ), TimeSpan.FromDays( 7 ), new TimerCallback( GiveMiB ) );
		}

		public DolphinRugNS( Serial serial ) : base( serial )
		{
		}
		
		private void GiveMiB()
		{
			m_MiB = Math.Min( 10, m_MiB +1 );
			
		}
		
		public override void OnComponentUsed( AddonComponent c, Mobile from )
		{
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return;
            }
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if (!from.InRange(GetWorldLocation(), 2) || !from.InLOS(this) || !((from.Z - Z) > -3 && (from.Z - Z) < 3))
			{
				from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
			}
			else if ( house != null && house.HasSecureAccess( from, SecureLevel.Friends ) )
			{
				if ( m_MiB > 0 )
				{
					Item MiB = null;

					MiB = new MessageInABottle();// break;

					int amount = Math.Min( 1, m_MiB );
					MiB.Amount = amount;
					
					if ( !from.PlaceInBackpack( MiB ) )
					{
						from.SendLocalizedMessage( 1078837 ); // Your backpack is full! Please make room and try again.
					}
					else
					{
						m_MiB -= amount;
						from.SendLocalizedMessage( 1150086 ); // A bottle has been placed in your backpack.
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
			writer.Write( (int) m_MiB );

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
			m_MiB = reader.ReadInt();

			DateTime next = reader.ReadDateTime();

			if ( next < DateTime.Now )
				next = DateTime.Now;	

			m_Timer = Timer.DelayCall( next - DateTime.Now, TimeSpan.FromDays( 7 ), new TimerCallback( GiveMiB ) );
		}
		
	}

	public class DolphinRugNSDeed : BaseAddonDeed, IRewardItem
	{
		
		public override int LabelNumber{ get{ return 1150051; } } // A Dolphin Rug (East)
		
		public override BaseAddon Addon
		{ 
			get
			{ 
				DolphinRugNS addon = new DolphinRugNS();
				addon.IsRewardItem = m_IsRewardItem;
				addon.MiB = m_MiB;

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

		private int m_MiB;		

		[CommandProperty( AccessLevel.GameMaster )]
		public int MiB
		{
			get{ return m_MiB; }
			set{ m_MiB = value; InvalidateProperties(); }
		}
		

		[Constructable]
		public DolphinRugNSDeed() 
		{
			LootType = LootType.Blessed;
		}

		public DolphinRugNSDeed( Serial serial ) : base( serial )
		{
		}
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            base.OnDoubleClick(from);
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
			writer.Write( (int) m_MiB );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			
			m_IsRewardItem = reader.ReadBool();
			m_MiB = reader.ReadInt();
		}
	}
}