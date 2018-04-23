//Created with Maraks Script Creator 4
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class HelmoftheSea : DragonHelm
  {
public override int ArtifactRarity{ get{ return 666; } }


      
      [Constructable]
		public HelmoftheSea()
		{
          Name = "Helm of the Sea";
          Hue = 2242;
      ArmorAttributes.SelfRepair = 5;
      Attributes.AttackChance = 20;
      Attributes.BonusDex = 10;
      Attributes.BonusHits = 10;
      Attributes.BonusInt = 10;
      Attributes.BonusMana = 10;
      Attributes.BonusStam = 10;
      Attributes.DefendChance = 20;
      Attributes.LowerManaCost = 20;
      Attributes.LowerRegCost = 20;
      Attributes.ReflectPhysical = 20;
      Attributes.WeaponDamage = 20;
      Attributes.BonusMana = 10;
		}

		public HelmoftheSea( Serial serial ) : base( serial )
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
