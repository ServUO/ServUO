using System;
using Server;

namespace Server.Items
{
	public class GladiatorsFist : PlateGloves
	{
		public override int LabelNumber{ get{ return 1094917; } } // Gladiator's Fist [Replica]
		
		public override int BasePhysicalResistance{ get{ return 18; } }
		public override int BaseFireResistance{ get{ return 18; } }
		public override int BaseColdResistance{ get{ return 17; } }
		public override int BasePoisonResistance{ get{ return 18; } }
		public override int BaseEnergyResistance{ get{ return 16; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public GladiatorsFist()
		{
			Name = "Gladiators Fist [Replica]";

			Hue = 0x26d;

			Attributes.BonusHits = 10;
			Attributes.AttackChance = 10;

			ArmorAttributes.MageArmor = 1;
		}

		public GladiatorsFist( Serial serial ) : base( serial )
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
