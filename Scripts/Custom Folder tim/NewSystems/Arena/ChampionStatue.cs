using System;

namespace Server.Items
{
	public class ChampionStatue : Item
	{
		
		[Constructable]
		public ChampionStatue() : base( 0x1227 )
		{
			Name = String.Format( "Arena Champion {0}", DateTime.Now );
			LootType = LootType.Blessed;
			Hue = 38;
			Weight = 1;
		}

		public ChampionStatue( Serial serial ) : base( serial )
		{
		}

		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }

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
}