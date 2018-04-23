using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an arcane liche's corpse" )]
	public class ArcaneLich : BaseCreature
	{
		[Constructable]
		public ArcaneLich() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an arcane lich";
			Body = 79;
			BaseSoundID = 412;
			Hue = 2951;

			SetStr( 416, 505 );
			SetDex( 146, 155 );
			SetInt( 682, 745 );

			SetHits( 350, 403 );

			SetDamage( 18, 24 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Cold, 60 );
			SetDamageType( ResistanceType.Energy, 80 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 80, 100 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 80, 100 );

			SetSkill( SkillName.EvalInt, 120.1, 140.0 );
			SetSkill( SkillName.Magery, 120.1, 140.0 );
			SetSkill( SkillName.Focus, 120.1, 140.0 );
			SetSkill( SkillName.MagicResist, 150.5, 200.0 );
			SetSkill( SkillName.Tactics, 60.1, 80.0 );
			SetSkill( SkillName.Wrestling, 60.1, 80.0 );

			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 60;
			PackItem( new GnarledStaff() );
			PackNecroReg( 12, 40 );
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 4; } }

		public ArcaneLich( Serial serial ) : base( serial )
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