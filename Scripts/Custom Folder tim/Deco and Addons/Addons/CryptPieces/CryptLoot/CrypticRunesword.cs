using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF61, 0xF60 )]
	public class CrypticRunesword : BaseSword
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int AosStrengthReq{ get{ return 35; } }
		public override int AosMinDamage{ get{ return 15; } }
		public override int AosMaxDamage{ get{ return 16; } }
		public override int AosSpeed{ get{ return 30; } }

		public override int OldStrengthReq{ get{ return 25; } }
		public override int OldMinDamage{ get{ return 5; } }
		public override int OldMaxDamage{ get{ return 33; } }
		public override int OldSpeed{ get{ return 35; } }

		public override int DefHitSound{ get{ return 0x237; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		[Constructable]
		public CrypticRunesword() : base( 0xF61 )
		{
			Weight = 7.0;
			Hue = 1230;
			Name = "Cryptic Runesword";
			Attributes.SpellChanneling = 1;
			Attributes.SpellDamage = -10;
			Attributes.WeaponDamage = 15;
			SkillBonuses.SetValues( 0, SkillName.Necromancy, 5.0 );
			WeaponAttributes.HitMagicArrow = 10;
			WeaponAttributes.HitHarm = 7;
			WeaponAttributes.HitFireball = 5;
			WeaponAttributes.HitLightning = 3;
		}

		public CrypticRunesword( Serial serial ) : base( serial )
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