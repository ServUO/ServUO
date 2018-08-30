using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "Good Luck Getting Your Contract!" )]
	public class MonsterContractDealer : Mobile
	{
		
		public virtual bool IsInvulnerable{ get{ return true; } }
		
		[Constructable]
		public MonsterContractDealer()
		{
			Name = "-Grim-";
			Title = "The Monster Slayer";
			Body = 0x190;
			CantWalk = true;
			Hue = Utility.RandomSkinHue();
			
			AddItem( ItemSet( new Cloak() ) );
			AddItem( ItemSet( new Robe() ) );
			AddItem( ItemSet1( new ShepherdsCrook() ) );

			Item Boots = new Boots();
			Boots.Hue = 2112;
			Boots.Movable = false;
			AddItem( Boots );

			Item FancyShirt = new FancyShirt();
			FancyShirt.Hue = 1267;
			FancyShirt.Movable = false;
			AddItem( FancyShirt );

			Item LongPants = new LongPants();
			LongPants.Hue = 847;
			LongPants.Movable = false;
			AddItem( LongPants );

			int hairHue = 1814;

			switch ( Utility.Random( 1 ) )
			{
					case 0: AddItem( new PonyTail( hairHue ) ); break;
					case 1: AddItem( new Goatee( hairHue ) ); break;
			}
			
			Blessed = true;
			
		}
		
		public static Item ItemSet( Item item )
		{
			item.Movable = false;
			item.Hue = 1109;
			
			return item;
		}
		
		public static Item ItemSet1( Item item )
		{
			item.Movable = false;
			item.Hue = 1153;
			
			return item;
		}


		public MonsterContractDealer( Serial serial ) : base( serial )
		{
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			list.Add( new MonsterContractDealerEntry( from, this ) );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
	
	public class MonsterContractDealerEntry : ContextMenuEntry
	{
		private static TimeSpan Delay = TimeSpan.FromHours(1);
		
		private static Dictionary<PlayerMobile,DateTime> LastUsers = new Dictionary<PlayerMobile,DateTime>();//pas sérialisé car c'est pas si important
		
		private Mobile m_Mobile;
		private Mobile m_Giver;
		
		public MonsterContractDealerEntry( Mobile from, Mobile giver ) : base( 6146, 3 )
		{
			m_Mobile = from;
			m_Giver = giver;
		}

		public override void OnClick()
		{
			if( !( m_Mobile is PlayerMobile ) )
				return;
			
			PlayerMobile mobile = (PlayerMobile) m_Mobile;

			if(CanGetContract(mobile))
			{
				if ( ! mobile.HasGump( typeof( MonsterContractDealerGump ) ) )
				{
					mobile.SendGump( new MonsterContractDealerGump( mobile ));
					mobile.AddToBackpack( new MonsterContract() );
				}
			}
		}
		
		private bool CanGetContract(PlayerMobile asker)
		{
			if(asker.AccessLevel > AccessLevel.Player)return true;
			
			if(!LastUsers.ContainsKey(asker))
			{
				LastUsers.Add(asker,DateTime.Now);
				return true;
			}
			
			else
			{
				if(DateTime.Now-LastUsers[asker] < Delay)
				{
					return false;
				}
				else
				{
					LastUsers[asker]=DateTime.Now;
					return true;
				}
			}
		}
	}
	
}
