using System;
using Server;

namespace Server.Items
{
	public class FetidPaladinsHelm : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 7; } }
		public override int BaseFireResistance{ get{ return 2; } }
		public override int BaseColdResistance{ get{ return 7; } }
		public override int BasePoisonResistance{ get{ return 4; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override int AosStrReq{ get{ return 55; } }
		public override int OldStrReq{ get{ return 40; } }

		public override int ArmorBase{ get{ return 30; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public FetidPaladinsHelm() : base( 0x1408 )
		{
			Weight = 5.0;
			Name = "Fetid Paladin's Helm";
			Hue = 2126;
			Attributes.DefendChance = 5;
			Attributes.RegenMana = 1;
			SkillBonuses.SetValues( 0, SkillName.Chivalry, 5.0 );
			SkillBonuses.SetValues( 1, SkillName.Parry, 5.0 );
		}

		public FetidPaladinsHelm( Serial serial ) : base( serial )
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

			if ( Weight == 1.0 )
				Weight = 5.0;
		}
	}
}