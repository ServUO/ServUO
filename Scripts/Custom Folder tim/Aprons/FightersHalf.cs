using System; 
using Server; 

namespace Server.Items
{ 
	public class FightersHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public FightersHalf()
		{
			Name = "The Fighter";
			Hue = 2075;

			Attributes.BonusDex = 10;
			Attributes.BonusStr = 10;
			Attributes.BonusInt = 10;
                        Attributes.AttackChance = 10;
		}

		public FightersHalf( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
} 