using System;
using Server;

namespace Server.Items
{
	public class ElectroSword : Katana
	{
		public override int ArtifactRarity{ get{ return 25; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ElectroSword()
		{
			Name = "Electro Sword";
			Attributes.WeaponDamage = 60;
			Attributes.WeaponSpeed = 35;
			Attributes.Luck = 200;
			Attributes.SpellChanneling = 1;
			WeaponAttributes.HitLowerAttack = 54;
			Attributes.BonusStr = 7;
			Attributes.AttackChance = 20;
			MaxRange = 3;
			Hue = 1159;
			HitSound = 756;
			MissSound = 476;
			WeaponAttributes.HitLightning = 65;
			LootType = LootType.Blessed;
		}

		public virtual void OnHit( Mobile attacker, Mobile defender )
		{
			attacker.MovingEffect( defender, 14363, 3, 3, false, false );
			base.OnHit( attacker, defender );
		}
		public ElectroSword( Serial serial ) : base( serial )
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