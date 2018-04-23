
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class RobeOfTreason : Robe
  {


      
      [Constructable]
		public RobeOfTreason()
		{
          Name = "Robe Of Treason";
          Hue = 1107;
      Attributes.RegenHits = 5;
      Attributes.RegenMana = 5;
      Attributes.RegenStam = 5;
      Attributes.Luck = 95;
      Attributes.ReflectPhysical = 44;
      Attributes.SpellDamage = 35;
		}

		public RobeOfTreason( Serial serial ) : base( serial )
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
