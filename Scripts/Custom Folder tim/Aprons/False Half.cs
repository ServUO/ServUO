using System; 
using Server; 

namespace Server.Items
{ 
	public class FalseHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public FalseHalf()
		{
			Name = "False";
			Hue = 0xB;

			Attributes.BonusDex = 10;
			Attributes.BonusStr = 10;
			Attributes.BonusInt = 10;

			Resistances.Physical = 1;
			Resistances.Fire = 1;
			Resistances.Cold = 1;
			Resistances.Poison = 1;
			Resistances.Energy = 1;

			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
		}

		public FalseHalf( Serial serial ) : base( serial )
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