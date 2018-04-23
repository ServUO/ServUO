using System;
using Server.Network;
using Server.Items;
using Server.Engines.Harvest;

namespace Server.Items
{
	[FlipableAttribute( 0x26BA, 0x26C4 )]
	public class ScytheofNast : BasePoleArm
	{
                
                public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq{ get{ return 100; } }
		public override int AosMinDamage{ get{ return 55; } }
		public override int AosMaxDamage{ get{ return 65; } }
		public override int AosSpeed{ get{ return 50; } }

		public override int OldStrengthReq{ get{ return 45; } }
		public override int OldMinDamage{ get{ return 15; } }
		public override int OldMaxDamage{ get{ return 18; } }
		public override int OldSpeed{ get{ return 32; } }

		public override int InitMinHits{ get{ return 561; } }
		public override int InitMaxHits{ get{ return 561; } }

                public override int AosIntelligenceReq{ get{ return 100; } }

                public override int AosDexterityReq{ get{ return 100; } }
                   
		[Constructable]
		public ScytheofNast() : base( 0x26BA )
		{
                   LootType = LootType.Blessed ;
                   Attributes.SpellChanneling = 1 ;
                   WeaponAttributes.HitFireball = 75 ; 
                   WeaponAttributes.HitLightning = 75 ;
                   Name = "Nast'sScythe" ;
                   Hue = 1153 ;
			Weight = 5.0;
		}

		public ScytheofNast( Serial serial ) : base( serial )
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

			if ( Weight == 15.0 )
				Weight = 5.0; }
       
                    public override int ArtifactRarity{ get{return 5000; } }
		
	}
}