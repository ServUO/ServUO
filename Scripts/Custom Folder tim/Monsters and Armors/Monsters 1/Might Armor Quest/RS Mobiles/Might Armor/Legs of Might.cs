//Created with Maraks Script Creator 4
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class LegsofMight : DragonLegs
  {
public override int ArtifactRarity{ get{ return 666; } }


      
      [Constructable]
		public LegsofMight()
		{
          Name = "Legs of Might";
          Hue = 2022;
      ArmorAttributes.SelfRepair = 5;
      Attributes.AttackChance = 30;
      Attributes.BonusDex = 20;
      Attributes.BonusHits = 20;
      Attributes.BonusInt = 20;
      Attributes.BonusMana = 20;
      Attributes.BonusStam = 20;
      Attributes.DefendChance = 30;
      Attributes.LowerManaCost = 30;
      Attributes.LowerRegCost = 30;
      Attributes.ReflectPhysical = 30;
      Attributes.WeaponDamage = 30;
      Attributes.BonusMana = 20;
		}

		public LegsofMight( Serial serial ) : base( serial )
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
