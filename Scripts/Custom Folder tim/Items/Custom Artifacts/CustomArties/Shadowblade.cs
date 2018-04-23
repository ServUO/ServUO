using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x26CE, 0x26CF )]
	public class ShadowBlade : Longsword, ITokunoDyable
	{
		public override int ArtifactRarity{ get{ return 12; } }

		public override int InitMinHits{ get{ return 225; } }
		public override int InitMaxHits{ get{ return 225; } }

		[Constructable]
		public ShadowBlade()
		{
			Name = "Blade of the Shadows";
			Attributes.AttackChance = 30;
                  Attributes.BonusDex = 5;
		  Attributes.CastSpeed = 1;
                  Attributes.ReflectPhysical = 15;
                  Attributes.RegenHits = 5;
                  Attributes.SpellChanneling = 1;
                  Attributes.SpellDamage = 20;
			Attributes.WeaponDamage = 45;
			Attributes.WeaponSpeed = 30;
			WeaponAttributes.HitFireball = 25;
                  WeaponAttributes.HitLeechMana = 30;
                  WeaponAttributes.HitLeechStam = 40;
			WeaponAttributes.SelfRepair = 3;
			Hue = 1899;
		}

		public ShadowBlade( Serial serial ) : base( serial )
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
