//Redsnow
using System;
using Server;
using Server.Items;
using Server.Engines.Quests;
using Reward = Server.Engines.Quests.BaseReward;

namespace Server.Items
{
	public class Recipebag : Bag
	{
		[Constructable]
        public Recipebag(): this(1)
		{
			//Movable = true;
			Hue = 1163;
			Name = "Random Recipe bag";
		}

		[Constructable]
		public Recipebag( int amount )
		{
			          
            DropItem( Reward.FletcherRecipe() );
            DropItem(Reward.TailorRecipe());
            DropItem(Reward.SmithRecipe());
            DropItem(Reward.TinkerRecipe());
            DropItem(Reward.CarpRecipe());
               
		}

        public Recipebag(Serial serial): base(serial)
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
		}
	}
}