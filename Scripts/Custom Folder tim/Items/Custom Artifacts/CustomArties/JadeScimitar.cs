using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class JadeScimitar : RadiantScimitar, ITokunoDyable
  {
public override int ArtifactRarity{ get{ return 15; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

      [Constructable]
		public JadeScimitar()
		{
          Name = "Jade Scimitar";
          Hue = 2964;
      WeaponAttributes.HitColdArea = 30;
      WeaponAttributes.HitEnergyArea = 25;
      WeaponAttributes.HitFireArea = 30;
      WeaponAttributes.HitPhysicalArea = 50;
      WeaponAttributes.HitPoisonArea = 20;
      WeaponAttributes.UseBestSkill = 1;
      Attributes.AttackChance = 15;
      Attributes.WeaponDamage = 50;
      Attributes.WeaponSpeed = 30;
		}

		public JadeScimitar( Serial serial ) : base( serial )
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
