using System;
using Server;

namespace Server.Items
{
	public class HelmOfTheScorpion : NorseHelm
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public HelmOfTheScorpion()
		{
			Name = "Helm Of The Scorpion";
			Hue = 69;
			SkillBonuses.SetValues( 0, SkillName.Poisoning, 100.0 );
			Attributes.BonusDex = 5;
			PoisonBonus = 50;
		}

		public HelmOfTheScorpion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}