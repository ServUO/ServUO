using System;
using Server;

namespace Server.Items
{
	public class GlovesOfTheTiger : StuddedGloves
	{
		public override int ArtifactRarity{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public GlovesOfTheTiger()
		{
			Name = "Gloves Of The Tiger";
			Hue = 45;
			Attributes.AttackChance = 20;
			Attributes.BonusDex = 15;
			PhysicalBonus = 12;
		}

		public GlovesOfTheTiger( Serial serial ) : base( serial )
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