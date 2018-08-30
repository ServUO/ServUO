
using System;
using Server;

namespace Server.Items
{
	public class LegsOfTheLeprechaun: PlateLegs
	{
		public override int ArtifactRarity{ get{ return 146; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public LegsOfTheLeprechaun()
		{
			Hue = 69; 
                        Name = "Legs Of The Leprechaun";
                        Attributes.Luck = 1000;
                        Attributes.LowerManaCost = 5;
                        Attributes.BonusInt = 5;
			Attributes.BonusDex = 5;
                        Attributes.BonusHits = 5;
                        Attributes.BonusMana = 5;
			Attributes.RegenHits = 1;
                        Attributes.BonusHits = 10;
			Attributes.RegenMana = 1;
                        Attributes.CastSpeed = 1;
                        Attributes.SpellDamage = 10;
                        Attributes.CastRecovery = 1;
			Attributes.RegenStam = 5;
			FireBonus = 5;
			ColdBonus = 5;
                        PoisonBonus = 5;
                        PhysicalBonus = 5;
                        EnergyBonus = 5;
		}

		public LegsOfTheLeprechaun( Serial serial ) : base( serial )
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