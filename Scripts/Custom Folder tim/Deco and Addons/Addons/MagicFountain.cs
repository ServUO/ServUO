// created on 01/02/2004 at 6:10 AM

// MagicFountain.cs by Alari (alarihyena@gmail.com)
// usage: MagicFountainAddon [ bool sandstone ]

/*
TODO yet:

make fountain able to be poisoned
make fountain timer so fountain randomly goes poisoned/clear
& add a toggle bool that controls this
make tasteid identify a poisoned fountain

Probably not going to do any of the above,
feel free to implement this.
*/


using System;
using Server;
using Server.Items;
using Server.ContextMenus;
using System.Collections;
using System.Collections.Generic;
using Server.Prompts;
using Server.Mobiles;


namespace Server.Items
{
	public class MagicFountainAddon : BaseAddon, IWaterSource
	{
		public int Quantity
		{
			get{ return 500; }
			set{}
		}
		
		private bool m_Magic;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Magic
		{
			get{ return m_Magic; }
			set{ m_Magic = value; }
		}

		private int m_Jackpot;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Jackpot
		{
			get{ return m_Jackpot; }
			set{ m_Jackpot = value < 0 ? 0 : value;  }
		}

		public enum FountainMessage
		{
			Cool  = 1,
			Warm  = 2,
			Clean = 3,
			Calm  = 4,
			Fruit = 5,
		}
		
		private FountainMessage m_MessageNum;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public FountainMessage MessageNum
		{
			get{ return m_MessageNum; }
			set{ m_MessageNum = value; }
		}
		
		[Constructable]
		public MagicFountainAddon() : this( false )
		{
		}
		
		[Constructable]
		public MagicFountainAddon( bool sandstone )
		{
			int itemID;
			
			if ( sandstone )
				itemID = 0x19C3; // sandstone
			else
				itemID = 0x1731; // stone
					
			AddComponent( new MagicFountainPiece( this, itemID++ ), -2, +1, 0 );
			AddComponent( new MagicFountainPiece( this, itemID++ ), -1, +1, 0 );
			AddComponent( new MagicFountainPiece( this, itemID++ ), +0, +1, 0 );
			AddComponent( new MagicFountainPiece( this, itemID++ ), +1, +1, 0 );
			
			AddComponent( new MagicFountainPiece( this, itemID++ ), +1, +0, 0 );
			AddComponent( new MagicFountainPiece( this, itemID++ ), +1, -1, 0 );
			AddComponent( new MagicFountainPiece( this, itemID++ ), +1, -2, 0 );
			
			AddComponent( new MagicFountainPiece( this, itemID++ ), +0, -2, 0 );
			AddComponent( new MagicFountainPiece( this, itemID++ ), +0, -1, 0 );
			AddComponent( new MagicFountainPiece( this, itemID++ ), +0, +0, 0 );
			
			AddComponent( new MagicFountainPiece( this, itemID++ ), -1, +0, 0 );
			AddComponent( new MagicFountainPiece( this, itemID++ ), -2, +0, 0 );
			
			AddComponent( new MagicFountainPiece( this, itemID++ ), -2, -1, 0 );
			AddComponent( new MagicFountainPiece( this, itemID++ ), -1, -1, 0 );
			
			AddComponent( new MagicFountainPiece( this, itemID++ ), -1, -2, 0 );
			AddComponent( new MagicFountainPiece( this, ++itemID ), -2, -2, 0 );
			
			switch( Utility.RandomMinMax( 1, 5 ) )
			{
				case 1: m_MessageNum = FountainMessage.Cool; break;
				case 2: m_MessageNum = FountainMessage.Warm; break;
				case 3: m_MessageNum = FountainMessage.Clean; break;
				case 4: m_MessageNum = FountainMessage.Calm; break;
				case 5: m_MessageNum = FountainMessage.Fruit; break;
			}
			
			m_Magic = false;  // Must be set by a gamemaster or better to be magical
		}
		
		public MagicFountainAddon( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 1 ); // version
			
			writer.Write( (int) m_Jackpot );
			writer.Write( (int) m_MessageNum );
			writer.Write( (bool) m_Magic );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			
			switch( version )
			{
				case 1: {
					m_Jackpot = reader.ReadInt();
					m_MessageNum = (FountainMessage)reader.ReadInt();
					m_Magic = reader.ReadBool();
					
					break;
				}
			}
		}
	}

	// component
	
	public class MagicFountainPiece : AddonComponent
	{
		private MagicFountainAddon m_Fountain;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public MagicFountainAddon Fountain
		{
			get{ return m_Fountain; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Magic
		{
			get{ return m_Fountain.Magic; }
			set{ m_Fountain.Magic = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Jackpot
		{
			get{ return m_Fountain.Jackpot; }
			set{ m_Fountain.Jackpot = value < 0 ? 0 : value;  }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public MagicFountainAddon.FountainMessage MessageNum
		{
			get{ return m_Fountain.MessageNum; }
			set{ m_Fountain.MessageNum = value; }
		}

		public MagicFountainPiece( MagicFountainAddon fountain, int itemid ) : base( itemid )
		{
			m_Fountain = fountain;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( m_Fountain.GetWorldLocation(), 4 ) )
			{
				if ( from.Thirst >= 20 )
				{
					from.SendMessage( "You are not thirsty at all." );
				}
				else
				{
					string msg = null;
					
					if ( m_Fountain == null )
					{
						from.SendMessage( "Debug: Parent was null" );
						return;
					}
					
					if ( 0.1 > Utility.RandomDouble() )
					{
						switch( m_Fountain.MessageNum )
						{
							case MagicFountainAddon.FountainMessage.Cool:  msg = "You drink your fill of the cool fountain water. The quiet sounds of the water splashing are musical.";		from.Hits += 10; break;
							case MagicFountainAddon.FountainMessage.Warm:  msg = "The fountain's warm water refreshes you and sets your mind at ease. You drink your fill.";					from.Mana += 10; break;
							case MagicFountainAddon.FountainMessage.Clean: msg = "You drink deeply of the clean fountain water. The shimmering reflections on the surface stir your thoughts.";	from.Mana += 10; break;
							case MagicFountainAddon.FountainMessage.Calm:  msg = "As you drink from the water, an invigorating scent reminds you of memories long forgotten.";					from.Stam += 10; break;
							case MagicFountainAddon.FountainMessage.Fruit: msg = "The fountain's water tastes vaguely of a fruit you can't identify. You drink your fill and feel refreshed.";	from.Stam += 10; break;
						}
						
						from.SendMessage( msg );
					}
					else
					{
						switch( m_Fountain.MessageNum )
						{
							case MagicFountainAddon.FountainMessage.Cool:  msg = "The cool fountain water is refreshing."; break;
							case MagicFountainAddon.FountainMessage.Warm:  msg = "The warm fountain water is refreshing."; break;
							case MagicFountainAddon.FountainMessage.Clean: msg = "You drink deeply from the clean water."; break;
							case MagicFountainAddon.FountainMessage.Calm:  msg = "The calm fountain water is refreshing."; break;
							case MagicFountainAddon.FountainMessage.Fruit: msg = "You drink your fill from the fountain."; break;
						}
						
						from.SendMessage( msg );
					}
					
					from.Thirst = 20;
				}
			}
			else
			{
				from.SendMessage( "Get closer." );
			}
		}
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			
			if ( m_Fountain != null && from.InRange( m_Fountain.GetWorldLocation(), 4 ) && m_Fountain.Magic )
				list.Add( new ActivateEntry( from, m_Fountain ) );
		}
		
		private class ActivateEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private MagicFountainAddon m_Fountain;
			
			public ActivateEntry( Mobile from, MagicFountainAddon fountain ) : base( 6170 ) // 3006170 Activate
			{
				m_From = from;
				m_Fountain = fountain;
			}
			
			public override void OnClick()
			{
				if ( m_From.InRange( m_Fountain.GetWorldLocation(), 4 ) )
				{
					if ( m_From.Backpack.ConsumeTotal( typeof( Gold ), 1 ) )
					{
						m_Fountain.Jackpot++;
						
						switch( m_Fountain.MessageNum )
						{
							
							case MagicFountainAddon.FountainMessage.Cool: {
								if ( 0.001 > Utility.RandomDouble() )
								{
									Gold gold = new Gold();
									gold.Amount = m_Fountain.Jackpot; // no need to test, it's always at least 1
									
									if ( !m_From.AddToBackpack( gold ) )
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain! But you can't hold it! You accidentally drop it back in the fountain, where it vanishes." );
										gold.Delete();
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain!" );
										m_Fountain.Jackpot = 0;
									}
								}
								else if ( 0.001 > Utility.RandomDouble() )
								{
									Gold gold = new Gold();
									gold.Amount = m_Fountain.Jackpot; // no need to test, it's always at least 1
									
									if ( !m_From.AddToBackpack( gold ) )
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain! But you can't hold it! You accidentally drop it back in the fountain, where it vanishes." );
										gold.Delete();
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain!" );
										m_Fountain.Jackpot = 0;
									}
								}
								else
								{
									m_From.SendMessage( "You toss a coin in ... but nothing happens." );
									
									if ( Utility.RandomBool() && Utility.RandomBool() )
										m_Fountain.Jackpot--;
								}
								
								break;
							}
							
							
							case MagicFountainAddon.FountainMessage.Warm: {
								if ( 0.01 > Utility.RandomDouble() )
								{
									Item item = null;
									
									switch( Utility.RandomMinMax( 1, 10 ) )
									{
										case 1:  item = new TinkerTools();				break;
										case 2:  item = new Lockpick(3);				break;
										case 3:  item = new Pitchfork();				break;
										case 4:  item = new Dagger();					break;
										case 5:  item = new Emerald();					break;
										case 6:  item = new Ruby();					break;
										case 7:  item = new Amber();					break;
										case 8:  item = new Server.Engines.Mahjong.MahjongGame();	break;
										case 9:  item = new SewingKit();				break;
										case 10: item = new SmithHammer();				break;
									}
									
									if ( !m_From.AddToBackpack( item ) )
									{
										m_From.SendMessage( "You toss a coin in ... and an item appears! But you can't hold it! You accidentally drop it back in the fountain, where it vanishes." );
										item.Delete();
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and an item appears!" );
										
										if ( m_Fountain.Jackpot > 10 )
											m_Fountain.Jackpot -= 10;
										else
											m_Fountain.Jackpot = 0;
									}
								}
								else if ( 0.001 > Utility.RandomDouble() )
								{
									Gold gold = new Gold();
									gold.Amount = m_Fountain.Jackpot; // no need to test, it's always at least 1
									
									if ( !m_From.AddToBackpack( gold ) )
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain! But you can't hold it! You accidentally drop it back in the fountain, where it vanishes." );
										gold.Delete();
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain!" );
										m_Fountain.Jackpot = 0;
									}
								}
								else
								{
									m_From.SendMessage( "You toss a coin in ... but nothing happens." );
									
									if ( Utility.RandomBool() && Utility.RandomBool() )
										m_Fountain.Jackpot--;
								}
								
								break;
							}
							
							
							case MagicFountainAddon.FountainMessage.Clean: {
								if ( 0.05 > Utility.RandomDouble() )
								{
									Item food = null;
									
									switch( Utility.RandomMinMax( 1, 7 ) )
									{
										//case 1:  food = new BaconSlab();	break;
										case 2:  food = new CookedBird();	break;
										case 3:  food = new Ham();			break;
										case 4:  food = new Ribs();			break;
										case 5:  food = new Sausage();		break;
										case 6:  food = new Cookies();		break;
										case 7:  food = new Muffins(3);		break;
										case 8:  food = new ApplePie();		break;
										//case 9:  food = new PeachCobbler();		break;
										//case 10: food = new KeyLimePie();	break;
									}
									
									if ( !m_From.AddToBackpack( food ) )
									{
										m_From.SendMessage( "You toss a coin in ... and food appears! But you can't hold it! You accidentally drop it back in the fountain, where it vanishes." );
										food.Delete();
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and food appears!" );
										
										if ( m_Fountain.Jackpot > 10 )
											m_Fountain.Jackpot -= 10;
										else
											m_Fountain.Jackpot = 0;
									}
								}
								else if ( 0.001 > Utility.RandomDouble() )
								{
									Gold gold = new Gold();
									gold.Amount = m_Fountain.Jackpot; // no need to test, it's always at least 1
									
									if ( !m_From.AddToBackpack( gold ) )
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain! But you can't hold it! You accidentally drop it back in the fountain, where it vanishes." );
										gold.Delete();
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain!" );
										m_Fountain.Jackpot = 0;
									}
								}
								else
								{
									m_From.SendMessage( "You toss a coin in ... but nothing happens." );
									
									if ( Utility.RandomBool() && Utility.RandomBool() )
										m_Fountain.Jackpot--;
								}
								
								break;
							}
							
							case MagicFountainAddon.FountainMessage.Calm: {
								if ( 0.01 > Utility.RandomDouble() )
								{
									Horse horse = new Horse();
									
									horse.Map = m_From.Map;
									horse.Location = m_From.Location;

									if ( ( m_From.Followers + horse.ControlSlots) <= m_From.FollowersMax )
									{
										horse.Controlled = true;

										horse.ControlMaster = m_From;

										m_From.SendMessage( "You toss a coin in ... and a horse appears!" );
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and a horse appears! But you have too many pets to control this one too. The horse runs off!" );
										horse.Delete();
									}


									if ( m_Fountain.Jackpot > 50 )
										m_Fountain.Jackpot -= 50;
									else
										m_Fountain.Jackpot = 0;
									
								}
								else if ( 0.001 > Utility.RandomDouble() )
								{
									Gold gold = new Gold();
									gold.Amount = m_Fountain.Jackpot; // no need to test, it's always at least 1
									
									if ( !m_From.AddToBackpack( gold ) )
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain! But you can't hold it! You accidentally drop it back in the fountain, where it vanishes." );
										gold.Delete();
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain!" );
										m_Fountain.Jackpot = 0;
									}
								}
								else
								{
									m_From.SendMessage( "You toss a coin in ... but nothing happens." );
									
									if ( Utility.RandomBool() && Utility.RandomBool() )
										m_Fountain.Jackpot--;
								}
								
								break;
							}
							
							
							case MagicFountainAddon.FountainMessage.Fruit: {
								if ( 0.05 > Utility.RandomDouble() )
								{
									Item fruit = null;
									
									switch( Utility.RandomMinMax( 1, 10 ) )
									{
										case 1:  fruit = new Watermelon();	break;
										case 2:  fruit = new Apple();		break;
										case 3:  fruit = new Pear();		break;
										case 4:  fruit = new Peach();		break;
										case 5:  fruit = new Lime();		break;
										case 6:  fruit = new Lemon();		break;
										case 7:  fruit = new Coconut();		break;
										case 8:  fruit = new Grapes();		break;
										case 9:  fruit = new Bananas();		break;
										case 10: fruit = new Cantaloupe();	break;
									}
									
									if ( !m_From.AddToBackpack( fruit ) )
									{
										m_From.SendMessage( "You toss a coin in ... and fruit appears! But you can't hold it! You accidentally drop it back in the fountain, where it vanishes." );
										fruit.Delete();
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and fruit appears!" );
										
										if ( m_Fountain.Jackpot > 10 )
											m_Fountain.Jackpot -= 10;
										else
											m_Fountain.Jackpot = 0;
									}
								}
								else if ( 0.001 > Utility.RandomDouble() )
								{
									Gold gold = new Gold();
									gold.Amount = m_Fountain.Jackpot; // no need to test, it's always at least 1
									
									if ( !m_From.AddToBackpack( gold ) )
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain! But you can't hold it! You accidentally drop it back in the fountain, where it vanishes." );
										gold.Delete();
									}
									else
									{
										m_From.SendMessage( "You toss a coin in ... and a shower of gold leaps back out of the fountain!" );
										m_Fountain.Jackpot = 0;
									}
								}
								else
								{
									m_From.SendMessage( "You toss a coin in ... but nothing happens." );
									
									if ( Utility.RandomBool() && Utility.RandomBool() )
										m_Fountain.Jackpot--;
								}
								
								break;
							}
							
						}
					}
					else
					{
						m_From.SendMessage( "You don't have a coin to toss." );
					}
				}
				else
				{
					m_From.SendMessage( "Get closer." );
				}
			}
		}
		
		public MagicFountainPiece( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
			
			writer.Write( m_Fountain );
			
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			
			switch( version )
			{
				case 0: {
					m_Fountain = reader.ReadItem() as MagicFountainAddon;
					break;
				}
			}
		}
	}
}
