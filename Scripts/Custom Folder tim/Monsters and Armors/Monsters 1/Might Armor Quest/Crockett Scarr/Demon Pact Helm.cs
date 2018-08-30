//Created with Maraks Script Creator 4
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class DemonPactHelm : ChainCoif
  {


      
      [Constructable]
		public DemonPactHelm()
		{
          Name = "Demon Pact Helm";
      ArmorAttributes.SelfRepair = 5;
      Attributes.AttackChance = 25;
      Attributes.DefendChance = 25;
      Attributes.ReflectPhysical = 25;
      Attributes.WeaponDamage = 20;
		}

		public DemonPactHelm( Serial serial ) : base( serial )
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
