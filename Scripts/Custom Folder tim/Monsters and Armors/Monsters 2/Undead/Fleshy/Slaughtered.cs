using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a slaughtered corpse" )]
	public class Slaughtered : BaseCreature
	{
		[Constructable]
		public Slaughtered() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "the slaughtered";
			Hue = 1143;
			BaseSoundID = 0x482;

			switch ( Utility.Random( 2 ) )
			{
				case 0: // zombie
					Body = 3;
					break;
				case 1: // ghoul
					Body = 153;
					break;
			}


			SetStr( 46, 60 );
			SetDex( 42, 60 );
			SetInt( 26, 40 );

			SetHits( 82, 84 );

			SetDamage( 12, 14 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 65, 70 );
			SetResistance( ResistanceType.Cold, 60, 80 );
			SetResistance( ResistanceType.Poison, 12, 22 );

			SetSkill( SkillName.MagicResist, 18.1, 40.0 );
			SetSkill( SkillName.Tactics, 58.1, 60.0 );
			SetSkill( SkillName.Wrestling, 68.1, 80.0 );

			Fame = 1600;
			Karma = -1600;

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

		public Slaughtered( Serial serial ) : base( serial )
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