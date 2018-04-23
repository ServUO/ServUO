using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a battle rend corpse" )]	
	public class BattleRend : Reptalon
	{

        public override bool InitialInnocent { get { return true; } }
        public override bool DeleteOnRelease { get { return true; } }
        [Constructable]
		public BattleRend() : base()
		{
			Name = "A Battle Rend";
			Hue = 0x455;

			SetStr( 1233 );
			SetDex( 588 );
			SetInt( 766 );

			SetHits( 1500 );
            SetStam(688);

			SetDamage( 25, 35 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 75 );
			SetResistance( ResistanceType.Fire, 75 );
			SetResistance( ResistanceType.Cold, 75 );
			SetResistance( ResistanceType.Poison, 75 );
			SetResistance( ResistanceType.Energy, 75 );

			SetSkill( SkillName.Wrestling, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0 );

            Fame = 0;
            Karma = 1000;

			Tamable = true;
            ControlSlots = 4;
			MinTameSkill = 0;	
			
			//if ( Paragon.ChestChance > Utility.RandomDouble() )
				//PackItem( new ParagonChest( Name, TreasureMapLevel ) );
		}
				
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosUltraRich, 4 );
		}
		
		public override WeaponAbility GetWeaponAbility()
		{
			switch ( Utility.Random( 2 ) )
			{
				case 0: return WeaponAbility.ParalyzingBlow;
				case 1: return WeaponAbility.BleedAttack;
			}
		
			return null;
		}
		
		public override bool CanAnimateDead{ get{ return false; } }
		public override BaseCreature Animates{ get{ return new SkeletalDragon(); } }
		public override int AnimateScalar{ get{ return 50; } } // dragon loses 50% hits & str

		public BattleRend( Serial serial ) : base( serial )
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