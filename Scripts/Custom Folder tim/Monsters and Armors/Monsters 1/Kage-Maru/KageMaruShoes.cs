
using System;
using Server;

namespace Server.Items
{
	public class KageMaruShoes : NinjaTabi
	{
		public override int ArtifactRarity{ get{ return 257; } }

		public override int BasePhysicalResistance{ get{ return 25; } }
		public override int BaseFireResistance{ get{ return 23; } }
		public override int BaseColdResistance{ get{ return 22; } }
		public override int BasePoisonResistance{ get{ return 23; } }
		public override int BaseEnergyResistance{ get{ return 22; } }

		public override int InitMinHits{ get{ return 0; } }
		public override int InitMaxHits{ get{ return 0; } }

		[Constructable]
		public KageMaruShoes()
		{
			Name = "Tabi of Kage-Maru";
			Hue = 1107;
		  
		 
                  Attributes.BonusDex = 20;
                  Attributes.BonusHits = 20;
                  Attributes.BonusStam = 15;
                  Attributes.BonusStr = 15;
                  Attributes.CastRecovery = 5;
                  Attributes.CastSpeed = 4;
                  Attributes.AttackChance = 20;
		  Attributes.DefendChance = 20;
                  Attributes.SpellDamage = 20;
		  Attributes.WeaponDamage = 25;
		  Attributes.WeaponSpeed = 15;
		}

		public KageMaruShoes( Serial serial ) : base( serial )
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

			if ( Hue == 0x55A )
				Hue = 0x4F6;
		}
	}
}