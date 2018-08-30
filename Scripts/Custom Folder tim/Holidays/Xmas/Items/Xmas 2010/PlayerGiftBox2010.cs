using System;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	[Furniture]
	[Flipable( 0x232A, 0x232B )]
	public class PlayerGiftBox2010 : Container, IChopable
	{
		public override int DefaultMaxWeight{ get{ return 0; } } // A value of 0 signals unlimited weight
	 	
	 	public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
		{

			from.SendMessage("This is one of Santas' Lost Presents! You can't add things to his gifts!");
				return false;

			return base.TryDropItem( from, dropped, sendFullMessage );
	 	}


		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{

			from.SendMessage("This is one of Santas' Lost Presents! You should bring it back to him!");
				return false;

			return base.OnDragDropInto( from, item, p );
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			
			from.SendMessage("This is one of Santas' Lost Presents! You shouldn't open someone elses gift!");
			
		}
		
		[Constructable]
		public PlayerGiftBox2010() : base(0x232A)
		{
			Weight = 1.0;
			Hue = Utility.RandomList( 1150, 1151, 1152, 1153, 1157, 32 );
			Name = "A Lost Christmas Present";
					
		}		
	
		public void OnChop( Mobile from )
		{
			if ( !this.Deleted && IsChildOf( from.Backpack ) )
			{
				this.Chop( from );
			}
			else
			{
				from.SendMessage( "That Christmas Present must be in your backpack IF your going to open it." );
			}
		}

		private void Chop( Mobile from )
		{
			from.SendMessage( "You make sure no one is looking and rip the gift open!" );

			switch (Utility.Random(12))  //picks one of the following
                {
                    case 0:
                        from.AddToBackpack( new HolidayGarland2010() ); break;
                    case 1:
                        from.AddToBackpack( new SnowDrift() ); break;
                  	case 2:
                        from.AddToBackpack( new SantasReindeer1() ); break;
                    case 3:
                        from.AddToBackpack( new SantasReindeer3() ); break;
                    case 4:
                        from.AddToBackpack( new HolidayGarland2010() ); break;
                    case 5:
                        from.AddToBackpack( new SantasBoots() ); break;
                    case 6:
                        from.AddToBackpack( new HolidayBobbles() ); break;
                    case 7:
                        from.AddToBackpack( new SantasChairAddonDeed() ); break;
                  	case 8:
                        from.AddToBackpack( new SantasElfBoots() ); break;
                    case 9:
                        from.AddToBackpack( new HolidayRose() ); break;
                    case 10:
                        from.AddToBackpack( new SnowDrift() ); break;
                    case 11:
                        from.AddToBackpack( new HolidayGarland2010() ); break;

                }

			this.Delete();
		}

		public PlayerGiftBox2010( Serial serial ) : base( serial )
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


			Weight = 1.0;

		}
	}
}
