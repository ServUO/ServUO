//Created by Insane Froggy
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class RobesOfTheHighCouncil : Robe
  {
public override int ArtifactRarity{ get{ return 18; } }


      
      [Constructable]
		public RobesOfTheHighCouncil()
		{
          Name = "Robes of the High Council";
          Hue = 1154;
      Attributes.CastRecovery = 2;
      Attributes.LowerManaCost = 5;
      Attributes.LowerRegCost = 20;
      Attributes.RegenMana = 3;
      Attributes.SpellDamage = 10;
		}

		public RobesOfTheHighCouncil( Serial serial ) : base( serial )
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
