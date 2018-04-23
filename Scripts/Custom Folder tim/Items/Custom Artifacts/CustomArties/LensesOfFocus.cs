using System;
using Server;

namespace Server.Items
{
    public class LensesOfFocus : Glasses, ITokunoDyable
	{
		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get{ return 12; } }
		public override int BaseColdResistance{ get{ return 11; } }
		public override int BasePoisonResistance{ get{ return 8; } }
		public override int BaseEnergyResistance{ get{ return 13; } }
		public override int ArtifactRarity{ get{ return 15; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public LensesOfFocus() : base()
		{
			Hue = 2105;
			Name = "Lenses Of Focus";
			Attributes.NightSight = 1;
			Attributes.DefendChance = 20;
			Attributes.AttackChance = 20;			
			WeaponAttributes.SelfRepair = 3;
		}

		public LensesOfFocus( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}