using System;
using Server;

namespace Server.Items
{
	public class Glovesofthemagi : DragonGloves
	{
		public override int ArtifactRarity{ get{ return 146; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public Glovesofthemagi()
		{
			Hue = 1153;
                        Name = "Gloves Of The Magi";
			Attributes.BonusInt = 60;
			Attributes.RegenMana = 15;
                        Attributes.LowerManaCost = 50;
                        Attributes.RegenHits = 30;
         Attributes.BonusHits = 35;
         Attributes.BonusMana = 50;
			FireBonus = 55;
         Attributes.CastSpeed = 1;
         Attributes.SpellDamage = 15;
         Attributes.CastRecovery = 1;
			ColdBonus = 25;
                        PoisonBonus = 25;
                        PhysicalBonus = 15;
                        EnergyBonus = 15;
		}

		public Glovesofthemagi( Serial serial ) : base( serial )
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