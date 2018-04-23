//Created with Maraks Script Creator 4
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class NeptuneShirt : Shirt
  {


      
      [Constructable]
		public NeptuneShirt()
		{
          Name = "Neptune Shirt";
          Hue = 2022;
      Attributes.AttackChance = 75;
      Attributes.DefendChance = 75;
      Attributes.LowerManaCost = 50;
      Attributes.LowerRegCost = 100;
      Attributes.ReflectPhysical = 75;
      Attributes.SpellDamage = 75;
      Attributes.WeaponDamage = 100;
      Attributes.WeaponSpeed = 100;
      LootType = LootType.Blessed;
		}

		public NeptuneShirt( Serial serial ) : base( serial )
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
