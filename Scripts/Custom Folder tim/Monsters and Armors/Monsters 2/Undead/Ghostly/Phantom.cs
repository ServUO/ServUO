using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a phantom corpse" )]
	public class Phantom : BaseCreature
	{
		[Constructable]
		public Phantom() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a phantom";
			Body = 26;
			Hue = 2949;
			BaseSoundID = 0x482;

			SetStr( 86, 100 );
			SetDex( 86, 95 );
			SetInt( 180, 200 );

			SetHits( 106, 128 );

			SetDamage( 20, 28 );

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Energy, 80 );

			SetResistance( ResistanceType.Physical, 85, 100 );
			SetResistance( ResistanceType.Cold, 45, 85 );
			SetResistance( ResistanceType.Energy, 80, 100 );

			SetSkill( SkillName.EvalInt, 85.1, 100.0 );
			SetSkill( SkillName.Magery, 85.1, 100.0 );
			SetSkill( SkillName.MagicResist, 85.1, 100.0 );
			SetSkill( SkillName.Tactics, 105.1, 120.0 );
			SetSkill( SkillName.Wrestling, 45.1, 55.0 );

			Fame = 12000;
			Karma = -12000;

			VirtualArmor = 28;

			PackReg( 10 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}
		
		public override bool BleedImmune{ get{ return true; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public Phantom( Serial serial ) : base( serial )
		{
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