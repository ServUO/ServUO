//Created with Script Creator By Marak & Rockstar
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class AtlantisSword : VikingSword
  {

		public override int InitMinHits{ get{ return 20; } }
		public override int InitMaxHits{ get{ return 40; } }

      [Constructable]
		public AtlantisSword()
		{
          Name = "Atlantis Sword";
          Hue = 2716;
      WeaponAttributes.HitColdArea = 100;
      WeaponAttributes.HitHarm = 100;
      WeaponAttributes.HitLeechHits = 100;
      WeaponAttributes.HitLeechMana = 100;
      WeaponAttributes.HitLeechStam = 100;
      WeaponAttributes.HitLowerAttack = 100;
      WeaponAttributes.HitLowerDefend = 100;
      WeaponAttributes.SelfRepair = 20;
      WeaponAttributes.UseBestSkill = 1;
      Attributes.AttackChance = 100;
      Attributes.DefendChance = 100;
      Attributes.SpellChanneling = 1;
      Attributes.WeaponDamage = 100;
      Attributes.WeaponSpeed = 100;
		}

		public AtlantisSword( Serial serial ) : base( serial )
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
