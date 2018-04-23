using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an undead tumor's corpse" )]
	public class TumorOfUndeath : BaseCreature
	{
		[Constructable]
		public TumorOfUndeath() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a tumor of undeath";
			Body = 22;
			Hue = 2968;
			BaseSoundID = 442;

			SetStr( 76, 100 );
			SetDex( 76, 95 );
			SetInt( 86, 100 );

			SetHits( 240, 284 );

			SetDamage( 22, 28 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Cold, 55, 85 );
			SetResistance( ResistanceType.Poison, 60, 80 );

			SetSkill( SkillName.EvalInt, 65.1, 80.0 );
			SetSkill( SkillName.Magery, 85.1, 120.0 );
			SetSkill( SkillName.MagicResist, 105.1, 120.0 );
			SetSkill( SkillName.Tactics, 85.1, 100.0 );
			SetSkill( SkillName.Wrestling, 65.1, 85.0 );

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 28;

			PackReg( 10 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
		}

		public override bool BleedImmune{ get{ return true; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public TumorOfUndeath( Serial serial ) : base( serial )
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