using System;
using Server;

namespace Server.Items
{
	public class Legsofthemagi: PlateLegs
	{
		public override int ArtifactRarity{ get{ return 146; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public Legsofthemagi()
		{
			Hue = 1153; 
                        Name = "Legs Of The Magi";
                        Attributes.LowerManaCost = 20;
                        Attributes.BonusInt = 15;
			Attributes.BonusDex = 10;
         Attributes.BonusHits = 35;
         Attributes.BonusMana = 50;
			Attributes.RegenHits = 30;
         Attributes.BonusHits = 35;
			Attributes.RegenMana = 15;
         Attributes.CastSpeed = 1;
         Attributes.SpellDamage = 35;
         Attributes.CastRecovery = 1;
			Attributes.RegenStam = 5;
			FireBonus = 25;
			ColdBonus = 25;
                        PoisonBonus = 25;
                        PhysicalBonus = 25;
                        EnergyBonus = 15;
		}

		public Legsofthemagi( Serial serial ) : base( serial )
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