using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class PlainWeddingCake : Food
	{
		[Constructable]
		public PlainWeddingCake() : base( 40630 )
		{
            Name = "Plain Wedding Cake";
			Weight = 10.0;
			LootType = LootType.Blessed;
		}

		public PlainWeddingCake( Serial serial ) : base( serial )
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
	public class GoldenWeddingCake : Food
	{
		[Constructable]
		public GoldenWeddingCake() : base( 40624 )
		{
            Name = "Golden Wedding Cake";
			Weight = 10.0;
			LootType = LootType.Blessed;
		}

		public GoldenWeddingCake( Serial serial ) : base( serial )
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
	public class WhiteWeddingCake : Food
	{
		[Constructable]
		public WhiteWeddingCake() : base( 40625 )
		{
            Name = "White Wedding Cake";
			Weight = 10.0;
			LootType = LootType.Blessed;
		}

		public WhiteWeddingCake( Serial serial ) : base( serial )
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
	public class ChocolateWeddingCake : Food
	{
		[Constructable]
		public ChocolateWeddingCake() : base( 40626 )
		{
            Name = "Chocolate Wedding Cake";
			Weight = 10.0;
			LootType = LootType.Blessed;
		}

		public ChocolateWeddingCake( Serial serial ) : base( serial )
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
	public class DetailedWeddingCake : Food
	{
		[Constructable]
		public DetailedWeddingCake() : base( 40627 )
		{
            Name = "Detailed Wedding Cake";
			Weight = 10.0;
			LootType = LootType.Blessed;
		}

		public DetailedWeddingCake( Serial serial ) : base( serial )
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
	public class SilverWeddingCake : Food
	{
		[Constructable]
		public SilverWeddingCake() : base( 40628 )
		{
            Name = "Silver Wedding Cake";
			Weight = 10.0;
			LootType = LootType.Blessed;
		}

		public SilverWeddingCake( Serial serial ) : base( serial )
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
	public class EarthyWeddingCake : Food
	{
		[Constructable]
		public EarthyWeddingCake() : base( 40629 )
		{
            Name = "Earthy Wedding Cake";
			Weight = 10.0;
			LootType = LootType.Blessed;
		}

		public EarthyWeddingCake( Serial serial ) : base( serial )
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
	public class FloweryWeddingCake : Food
	{
		[Constructable]
		public FloweryWeddingCake() : base( 40660 )
		{
            Name = "Flowery Wedding Cake";
			Weight = 10.0;
			LootType = LootType.Blessed;
		}

		public FloweryWeddingCake( Serial serial ) : base( serial )
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