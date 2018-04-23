using System;
using Server;

namespace Server.Items
{
	public class BlazedGauntlets : RingmailGloves
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BlazedGauntlets()
		{
			Hue = 2993;
                        Name = "Blazed Gauntlets";
			Attributes.BonusStr = 10;
                        Attributes.LowerManaCost = 15;
                        Attributes.RegenHits = 10;
			FireBonus = 20;
			ColdBonus = 20;
         Attributes.WeaponDamage = 25;
         Attributes.WeaponSpeed = 25;
                        PoisonBonus = 20;
                        PhysicalBonus = 20;
                        EnergyBonus = 20;
		}

		public BlazedGauntlets( Serial serial ) : base( serial )
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