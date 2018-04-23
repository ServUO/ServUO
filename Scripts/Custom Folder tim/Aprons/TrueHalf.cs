using System; 
using Server; 

namespace Server.Items
{ 
	public class TrueHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public TrueHalf()

		{
			Name = "Truth";
			Hue = 1153;

			Attributes.BonusDex = 10;
			Attributes.BonusStr = 5;
			Attributes.BonusInt = 15;

			Resistances.Physical = 5;
			Resistances.Cold = 5;
			
			Attributes.DefendChance = 10;
		}

		public TrueHalf( Serial serial ) : base( serial )
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