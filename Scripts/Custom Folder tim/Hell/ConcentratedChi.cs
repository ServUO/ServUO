using System;
using Server;

namespace Server.Items
{
	public class ConcentratedChi : PlateGloves
	{
		public override int ArtifactRarity{ get{ return 21; } }

		public override int BaseColdResistance{ get{ return 3; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 22; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ConcentratedChi()
		{
			Hue = 1373;
                        Name = "Focused Chi";
			Attributes.CastRecovery = 1;
			Attributes.LowerManaCost = 4;
			ArmorAttributes.MageArmor = 1;
			Attributes.Luck = 10;
			Attributes.RegenHits = 1;
			Attributes.LowerRegCost = 4;
			Attributes.NightSight = 1;
			Attributes.ReflectPhysical = 5;
			ArmorAttributes.SelfRepair = 10;
		}

		public ConcentratedChi( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 )
			{
				ColdBonus = 0;
				EnergyBonus = 0;
			}
		}
	}
}