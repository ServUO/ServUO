
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class MalabrancheHelm : BoneHelm
  {
public override int ArtifactRarity{ get{ return 666; } }
public override int BaseColdResistance{ get{ return 23; } } 
		public override int BaseEnergyResistance{ get{ return 23; } } 
		public override int BasePhysicalResistance{ get{ return 23; } } 
		public override int BasePoisonResistance{ get{ return 23; } } 
		public override int BaseFireResistance{ get{ return 23; } } 

      
      [Constructable]
		public MalabrancheHelm()
		{
			Weight = 5;
          Name = "Helm of the Malabranche";
      ArmorAttributes.SelfRepair = 23;
      Attributes.AttackChance = 23;
      Attributes.BonusDex = 23;
      Attributes.BonusHits = 23;
      Attributes.BonusInt = 23;
      Attributes.BonusMana = 23;
      Attributes.BonusStam = 23;
      Attributes.DefendChance = 23;
      Attributes.LowerManaCost = 23;
      Attributes.LowerRegCost = 23;
      Attributes.Luck = 666;
      Attributes.ReflectPhysical = 23;
      Attributes.WeaponDamage = 23;
      Attributes.BonusMana = 23;
		}

		public MalabrancheHelm( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
