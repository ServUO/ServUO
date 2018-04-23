using System;
using Server;
using Server.Items;

namespace Server.Misc
{
	public class Thanksgiving : GiftGiver
	{
		public static void Initialize()
		{
			GiftGiving.Register( new Thanksgiving() );
		}

		public override DateTime Start{ get{ return new DateTime( 2013, 11, 10 ); } }
		public override DateTime Finish{ get{ return new DateTime( 2013, 12, 01 ); } }

		public override void GiveGift( Mobile mob )
		{
			GiftBox box = new GiftBox();

			box.DropItem( new PlentifulHarvest() );
			box.DropItem( new RoastedTurkey() );

			switch ( GiveGift( mob, box ) )
			{
				case GiftResult.Backpack:
					mob.SendMessage( 0x482, "Happy Holidays from the team!  Gift items have been placed in your backpack." );
					break;
				case GiftResult.BankBox:
					mob.SendMessage( 0x482, "Happy Holidays from the team!  Gift items have been placed in your bank box." );
					break;
			}
		}
	}
}