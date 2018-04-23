using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a mangled corpse" )]
	public class ReturnedFotter : BaseCreature
	{
		[Constructable]
		public ReturnedFotter() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a returned fotter";
			Hue = 1441;

			switch ( Utility.Random( 5 ) )
			{
				case 0: // zombie
					Body = 3;
					BaseSoundID = 471;
					break;
				case 1: // headless one
					Body = 31;
					BaseSoundID = 0x39D;
					break;
				case 2: // skeleton
					Body = Utility.RandomList( 50, 56 );
					BaseSoundID = 0x48D;
					break;
				case 3: // ghoul
					Body = 153;
					BaseSoundID = 0x482;
					break;
				case 4: // patchwork skeleton
					Body = 309;
					BaseSoundID = 0x48D;
					break;
				default:
				case 5: // slime
					Body = 51;
					BaseSoundID = 456;
					break;
			}


			SetStr( 76, 80 );
			SetDex( 42, 60 );
			SetInt( 26, 40 );

			SetHits( 68, 74 );

			SetDamage( 8, 12 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 65, 70 );
			SetResistance( ResistanceType.Cold, 60, 80 );
			SetResistance( ResistanceType.Poison, 12, 22 );

			SetSkill( SkillName.MagicResist, 15.1, 40.0 );
			SetSkill( SkillName.Tactics, 48.1, 60.0 );
			SetSkill( SkillName.Wrestling, 68.1, 80.0 );

			Fame = 1400;
			Karma = -1400;

			VirtualArmor = 28;
			
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
			AddLoot( LootPack.Rich );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public ReturnedFotter( Serial serial ) : base( serial )
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