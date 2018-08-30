using System;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Misc
{
	public class MerryChristmas
	{
		public static void Initialize()
		{
			// Register our speech handler
			EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
		}

		public static void EventSink_Speech( SpeechEventArgs args )
		{
			Mobile from = args.Mobile;
			int[] keywords = args.Keywords;


			if ( from is PlayerMobile )
			{
				PlayerMobile player = from as PlayerMobile;
				
				if ( args.Speech.ToLower().Equals("merry christmas"))
				{
					Item[] items = from.Backpack.FindItemsByType( typeof( ChristmasBag ) );

					if ( items.Length == 0 )
					{
						from.SendMessage ("You need a goodie bag.");
					}
					else
					{
						bool foundbag = false;
						
						foreach( ChristmasBag tb in items )
						{
							if ( tb.Uses > 0 )
							{ 
								foreach ( Mobile m in from.GetMobilesInRange( 3 ) ) // TODO: Validate range
								{
									if ( !m.Player && m.Body.IsHuman && ( m is BaseVendor ) )
									{
										if (m is BaseCreature && (((BaseCreature)m).IsHumanInTown() ) )
										{
											from.Direction = from.GetDirectionTo( m );
											m.Direction = m.GetDirectionTo( from );
											
											MerryChristmas.GiveTreat( from, m, tb );
											tb.ConsumeUse( from );
											
											return;
										}
									}
								}

								foundbag = true;

								break;
							}
						}
						
						
						if ( !foundbag )
						{
							from.SendMessage("You don't have any uses left on your goodie bags");
						}
					}
				}
			}
		}

		private static void PlaceItemIn( Container parent, int x, int y, Item item )
		{
			parent.AddItem( item );
			item.Location = new Point3D( x, y, 0 );
		}
		
		public static void GiveTreat ( Mobile from, Mobile vendor, Container gb)
		{			
			if ( Utility.Random( 100 ) < 5 )
			{
				//Give special Items.
				vendor.Say ("Well, I am out of goodies, but let me give you something special");
			
				switch ( Utility.Random ( 3) )
				{
					case 0:
						//Give Elf Hat
						PlaceItemIn( gb, 90, 96, new ElfHat() );
						
						break;
					case 1:
						//Give Christmas Slash
						PlaceItemIn( gb, 90, 90, new Christmasslash() );
						
						break;
					case 2:
						//Give Christmas Robe
						PlaceItemIn( gb, 88, 88, new ChristmasRobe() );
						
						break;
				}
				
			}
			else
			{
				switch ( Utility.Random ( 11 ) )
				{
					case 0:
						//Give Muffins
						vendor.Say ("Here's Some Muffins for you.  Merry Christmas");
						PlaceItemIn( gb, 89, 86, new Muffins() );
						
						break;
					case 1:
						//Give Ham
						vendor.Say ("Here's a pear for you.  Merry Christmas");
						PlaceItemIn( gb, 44, 44, new Ham() );
						
						break;
					case 2:
						//Give cake
						vendor.Say ("Here's a cake for you.  Merry Christmas");
						PlaceItemIn( gb, 22, 22, new Cake() );
						
						break;
					case 3:
						//Give cookies
						vendor.Say ("Here's some cookies for you.  Merry Christmas");
						PlaceItemIn( gb, 25, 96, new Cookies() );
						
						break;
					case 4:
						//Give pumpkin pie
						vendor.Say ("Here's a nice pumpkin pie for you.  Merry Christmas");
						PlaceItemIn( gb, 30, 96, new PumpkinPie() );
						
						break;
					case 5:
						//Give Chocolate coin
						vendor.Say ("Here's some candy for you.  Merry Christmas");
						PlaceItemIn( gb, 60, 96, new Chocolatecoin() );
						
						break;
					case 6:
						//Give Ear Of Corn
						vendor.Say ("Here's an Ear Of Corn.  Merry Christmas");
						PlaceItemIn( gb, 40, 96, new EarOfCorn() );
						
						break;
					case 7:
						//Give Apple Pie
						vendor.Say ("Here's an Apple Pie.  Merry Christmas");
						PlaceItemIn( gb, 70, 96, new ApplePie() );
						
						break;
					case 8:
						//Give Fruit Pie
						vendor.Say ("Here's an Fruit pie for you.  Merry Christmas");
						PlaceItemIn( gb, 85, 40, new FruitPie() );
						
						break;
					case 9:
						//Give Summer Sausage
						vendor.Say ("Here's some summer sausage for you.  Merry Christmas");
						PlaceItemIn( gb, 80, 96, new SummerSausage() );
						
						break;	
					case 10:
						//Give Lemon head
						vendor.Say ("Here's an Lemon head for you.  Merry Christmas");
						PlaceItemIn( gb, 93, 96, new Lemonhead() );
						
						break;		
				}
			}
		}
	}
}