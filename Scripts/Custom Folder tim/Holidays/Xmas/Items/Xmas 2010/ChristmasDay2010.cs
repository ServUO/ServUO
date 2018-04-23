/*
 * Created by SharpDevelop.
 * User: Sharon
 * Date: 10/22/2005
 * Time: 7:09 AM
 * 
 * Halloween2005 Gift Giving
 */

using System;
using Server;
using Server.Items;

namespace Server.Misc
{
	public class ChristmasDay2010 : GiftGiver
	{
		public static void Initialize()
		{
			GiftGiving.Register( new ChristmasDay2010() );
		}

		public override DateTime Start{ get{ return new DateTime( 2010, 12, 20 ); } }//make 21
		public override DateTime Finish{ get{ return new DateTime( 2010, 12, 28 ); } }

		public override void GiveGift( Mobile mob )
		{
			RedStocking bag = new RedStocking();
			bag.DropItem( new HolidayTreeDeed() );
			bag.DropItem( new GingerBreadCookie() );
			bag.DropItem( new CandyCane() );
			bag.DropItem( new HearthOfHomeFireDeed() );
			bag.DropItem( new HolidayGarland2010() );
			bag.DropItem( new HolidayGarland2010() );
			bag.DropItem( new BlueSnowflake() );
			bag.DropItem( new WhiteSnowflake() );
			
			RudolphStatue deer = new RudolphStatue();
			deer.Name = "Rudolph the Riendeer raised by " + mob.Name;
			bag.DropItem( deer );
			
			SantasReindeer2 deer2 = new SantasReindeer2();
			deer2.Name = "A Reindeer raised by " + mob.Name;
			bag.DropItem( deer2 );
/*			
			int random = Utility.Random( 100 );

			if ( random < 30 )
				bag.DropItem( new ShazzyToken() );
			else
				bag.DropItem( new HeritageToken() );
*/			
			switch ( GiveGift( mob, bag ) )
			{
				case GiftResult.Backpack:
					mob.SendMessage( 0x482, "Happy Holidays! A Red Holiday Stocking has been placed in your backpack." );
					break;
				case GiftResult.BankBox:
					mob.SendMessage( 0x482, "Happy Holidays! A Red Holiday Stocking has been placed in your bank box." );
					break;
			}
			GreenStocking bag2 = new GreenStocking();
			bag2.DropItem( new GingerBreadCookie() );
			bag2.DropItem( new CandyCane() );
			bag2.DropItem( new WreathDeed() );
			bag2.DropItem( new SantasSleighDeed() );
			bag2.DropItem( new HolidayGarland2010() );
			bag2.DropItem( new HolidayGarland2010() );
			
			RedPoinsettia sman = new RedPoinsettia();
			sman.Name = "A Red Poinsettia grown by " + mob.Name;
			bag2.DropItem( sman );
			
			WhitePoinsettia white = new WhitePoinsettia();
			white.Name = "A White Poinsettia grown by " + mob.Name;
			bag2.DropItem( white );
			
			//int random = Utility.Random( 100 );
/*
			if ( random < 30 )
				bag2.DropItem( new HeritageToken() );
			else
				bag2.DropItem( new ShazzyToken() );
			
*/			switch ( GiveGift( mob, bag2 ) )
			{
				case GiftResult.Backpack:
					mob.SendMessage( 0x482, "Happy Holidays from Shazzy's Shard! A Green Holiday Stocking has been placed in your backpack." );
					break;
				case GiftResult.BankBox:
					mob.SendMessage( 0x482, "Happy Holidays from Shazzy's Shard! A Green Holiday Stocking has been placed in your bank box." );
					break;
			}
			
		}
	}
}
