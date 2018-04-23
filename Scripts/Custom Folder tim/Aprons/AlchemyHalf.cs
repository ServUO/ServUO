using System; 
using Server; 

namespace Server.Items
{ 
	public class AlchemyHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public AlchemyHalf()
		{
                       
			Name = "The Alchemist";
			Hue = 1162;

			SkillBonuses.SetValues( 0, SkillName.Alchemy, 10.0 );
			Resistances.Physical = 5;
			Resistances.Fire = 5;
			Resistances.Cold = 5;
			Resistances.Poison = 5;
			Resistances.Energy = 5;

			Attributes.CastSpeed = 2;
			Attributes.CastRecovery = 2;
		}

		public AlchemyHalf( Serial serial ) : base( serial )
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