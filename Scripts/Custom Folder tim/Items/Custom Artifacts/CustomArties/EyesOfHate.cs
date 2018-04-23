using System;
using Server;

namespace Server.Items
{
	public class EyesOfHate : Glasses, ITokunoDyable
  {
public override int ArtifactRarity{ get{ return 12; } }

		
	
		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get{ return 10; } }
		public override int BaseColdResistance{ get{ return 10; } }
		public override int BasePoisonResistance{ get{ return 10; } }
		public override int BaseEnergyResistance{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public EyesOfHate() : base()
		{
			Name = "Eyes Of Hate";
			Hue = 137;
		
			Attributes.BonusStr = 5;
			Attributes.BonusDex = 10;
			Attributes.DefendChance = 15;
			Attributes.AttackChance = 10;
			ArmorAttributes.SelfRepair = 5;
			
			WeaponAttributes.HitLowerAttack = 30;
		}

		public EyesOfHate( Serial serial ) : base( serial )
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