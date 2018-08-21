using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a rotting hydra" )]
	public class TrainingHydra : BaseCreature
	{
		[Constructable]
		public TrainingHydra() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "A Training Hydra";
			Body = 265;
			//BaseSoundID = 362;
            CantWalk = true;
			SetStr( 46, 70 );
			SetDex( 31, 50 );
			SetInt( 26, 40 );

			SetHits( 9999999 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 5, 10 );

			SetSkill( SkillName.MagicResist, 15.1, 40.0 );
			SetSkill( SkillName.Tactics, 35.1, 50.0 );
			SetSkill( SkillName.Wrestling, 35.1, 50.0 );
            Direction = Direction.South; 
			Fame = 600;
			Karma = -600;

			VirtualArmor = 18;
			
			switch ( Utility.Random( 10 ))
			{
				case 0: PackItem( new LeftArm() ); break;
				case 1: PackItem( new RightArm() ); break;
				case 2: PackItem( new Torso() ); break;
				case 3: PackItem( new Bone() ); break;
				case 4: PackItem( new RibCage() ); break;
				case 5: PackItem( new RibCage() ); break;
				case 6: PackItem( new BonePile() ); break;
				case 7: PackItem( new BonePile() ); break;
				case 8: PackItem( new BonePile() ); break;
				case 9: PackItem( new BonePile() ); break;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override bool DisallowAllMoves{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }

		public TrainingHydra( Serial serial ) : base( serial )
		{
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}