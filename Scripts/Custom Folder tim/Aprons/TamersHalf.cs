using System; 
using Server; 

namespace Server.Items
{ 
	public class TamersHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public TamersHalf()
                {
			Name = "Tamers Half";
			Hue = 1366;

			SkillBonuses.SetValues( 0, SkillName.AnimalLore, 10.0 );
                        SkillBonuses.SetValues( 1, SkillName.AnimalTaming, 10.0 );
			
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
		}

		public TamersHalf( Serial serial ) : base( serial )
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