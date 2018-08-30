//Created by Neptune
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class RevelationCloak : Cloak
  {
public override int ArtifactRarity{ get{ return 697; } }


      
      [Constructable]
		public RevelationCloak()
		{
          Name = "Cloak of Revelation";
          Hue = 1700;
      Attributes.AttackChance = 20;
      Attributes.BonusHits = 30;
      Attributes.CastRecovery = 2;
      Attributes.CastSpeed = 2;
      Attributes.DefendChance = 20;
      Attributes.LowerManaCost = 15;
      Attributes.LowerRegCost = 40;
      Attributes.Luck = 500;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 15;
      Attributes.RegenHits = 10;
      Attributes.RegenMana = 10;
      Attributes.RegenStam = 10;
      Attributes.SpellDamage = 15;
		}

		public RevelationCloak( Serial serial ) : base( serial )
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
