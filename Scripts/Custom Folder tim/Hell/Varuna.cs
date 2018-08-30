using System;
using Server;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a glowing corpse" )]
	public class Varuna : BaseCreature
	{
		public override bool InitialInnocent{ get{ return true; } }

		[Constructable]
		public Varuna() : base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "Varuna";
			Body = 16;
                        Hue = 1676;
			BaseSoundID = 476;

			SetStr( 321, 430 );
			SetDex( 301, 400 );
			SetInt( 201, 250 );

			SetHits( 1003, 1118 );

			SetDamage( 9, 25 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 100.0 );
			SetSkill( SkillName.Meditation, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 10.1, 20.0 );
			SetSkill( SkillName.Wrestling, 10.1, 12.5 );

			Fame = 17000;
			Karma = 17000;

			VirtualArmor = 100;
			if ( 0.02 > Utility.RandomDouble() )
				PackStatue();
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.LowScrolls );
			AddLoot( LootPack.Gems, 2 );
		}

		public Varuna( Serial serial ) : base( serial )
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