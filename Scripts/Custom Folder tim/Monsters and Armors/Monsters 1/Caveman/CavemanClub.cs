
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class CavemanClub : Club
  {
		public override int OldMinDamage{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 20; } }
		public override int OldMaxDamage{ get{ return 25; } }
		public override int AosMaxDamage{ get{ return 25; } }
		public override int AosSpeed{ get{ return 45; } }
		public override int DefMaxRange{ get{ return 3; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

      [Constructable]
		public CavemanClub()
		{
          Name = "Club of the Caveman";
          
	  Layer = Layer.OneHanded;
      WeaponAttributes.HitFireball = 40;
      WeaponAttributes.HitLeechHits = 50;
      WeaponAttributes.HitLeechMana = 50;
      WeaponAttributes.HitLeechStam = 50;
      WeaponAttributes.HitLightning = 40;
      WeaponAttributes.HitLowerAttack = 25;
      WeaponAttributes.HitLowerDefend = 25;
      WeaponAttributes.HitMagicArrow = 40;
      WeaponAttributes.SelfRepair = 5;
      Attributes.SpellChanneling = 1;
      Attributes.AttackChance = 35;
     
      Attributes.DefendChance = 35;
      Attributes.SpellDamage = 15;
      Attributes.WeaponDamage = 30;
      Attributes.WeaponSpeed = 20;
    
		
		}
		
		
		public CavemanClub( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
