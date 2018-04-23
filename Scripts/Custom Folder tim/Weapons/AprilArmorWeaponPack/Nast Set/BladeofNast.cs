using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x26BB, 0x26C5 )]
	public class BladeofNast : BaseSword
	{
                public override int ArtifactRarity{ get{return 5001; } }
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq{ get{ return 100; } }
		public override int AosMinDamage{ get{ return 25; } }
		public override int AosMaxDamage{ get{ return 65; } }
		public override int AosSpeed{ get{ return 36; } }

		public override int OldStrengthReq{ get{ return 25; } }
		public override int OldMinDamage{ get{ return 13; } }
		public override int OldMaxDamage{ get{ return 15; } }
		public override int OldSpeed{ get{ return 36; } }
                
                public override int AosDexterityReq{ get{ return 100; } }

                public override int AosIntelligenceReq{ get{ return 100; } }

		public override int DefHitSound{ get{ return 0x23B; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 70; } }

		[Constructable]
		public BladeofNast() : base( 0x26BB )
		{
                       LootType = LootType.Blessed ;
                   Attributes.SpellChanneling = 1 ;
                   WeaponAttributes.HitFireball = 75 ; 
                   WeaponAttributes.HitLightning = 75 ;
                   Name = "BladeofNast" ;
                   Hue = 1608 ;
			Weight = 3.0;
		}

		public BladeofNast( Serial serial ) : base( serial )
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