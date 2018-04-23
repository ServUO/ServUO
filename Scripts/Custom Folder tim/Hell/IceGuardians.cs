using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a ice guardian corpse" )]
	public class IceGuardians : BaseCreature
	{
		[Constructable]
		public IceGuardians() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a ice guardian";
			Body = 311;
                        Hue = 1364;
			BaseSoundID = 589;

			SetStr( 216, 290 );
			SetDex( 86, 115 );
			SetInt( 36, 60 );

			SetHits( 178, 190 );

			SetDamage( 11, 21 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 40, 60 );

			SetSkill( SkillName.MagicResist, 85.1, 90.0 );
			SetSkill( SkillName.Tactics, 95.1, 100.0 );
			SetSkill( SkillName.Swords, 95.1, 100.0 );
			SetSkill( SkillName.Wrestling, 94.1, 100.0 );

			Fame = 6000;
			Karma = -6000;

			VirtualArmor = 65;

			
			switch ( Utility.Random( 6 ) )
			{
				case 0: PackItem( new PlateArms() ); break;
				case 1: PackItem( new PlateChest() ); break;
				case 2: PackItem( new PlateGloves() ); break;
				case 3: PackItem( new PlateGorget() ); break;
				case 4: PackItem( new PlateLegs() ); break;
				case 5: PackItem( new PlateHelm() ); break;
			}

			PackItem( new NoDachi() );
			PackItem( new HeaterShield() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
		}

		public override bool BleedImmune{ get{ return true; } }

		public IceGuardians( Serial serial ) : base( serial )
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