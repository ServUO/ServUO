//Created by Cherokee/Mule II aka Hotshot
using System;
using Server;

namespace Server.Items
{
	public class GlovesOfTheLeprechaun : PlateGloves
	{
		public override int ArtifactRarity{ get{ return 146; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public GlovesOfTheLeprechaun()
		{
			Hue = 69;
                        Name = "Gloves Of The Leprechaun";
                        Attributes.Luck = 1000;
			Attributes.BonusInt = 10;
			Attributes.RegenMana = 2;
                        Attributes.LowerManaCost = 10;
                        Attributes.RegenHits = 1;
         Attributes.BonusHits = 5;
         Attributes.BonusMana = 5;
			FireBonus = 5;
         Attributes.CastSpeed = 1;
         Attributes.SpellDamage = 5;
         Attributes.CastRecovery = 1;
			ColdBonus = 5;
                        PoisonBonus = 5;
                        PhysicalBonus = 5;
                        EnergyBonus = 5;
		}

		public GlovesOfTheLeprechaun( Serial serial ) : base( serial )
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