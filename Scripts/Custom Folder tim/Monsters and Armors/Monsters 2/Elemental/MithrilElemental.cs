using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a mithril elemental corpse" )]
	public class MithrilElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public MithrilElemental() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a mithril elemental";
			Body = 14;
			BaseSoundID = 268;
			Hue = 2401;

			SetStr( 164, 170 );
			SetDex( 180, 205 );
			SetInt( 71, 92 );

			SetHits( 176, 193 );

			SetDamage( 29, 36 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 80, 85 );
			SetResistance( ResistanceType.Fire, 80, 85 );
			SetResistance( ResistanceType.Cold, 60, 68 );
			SetResistance( ResistanceType.Poison, 15, 25 );
			SetResistance( ResistanceType.Energy, 15, 25 );

			SetSkill( SkillName.MagicResist, 50.1, 95.0 );
			SetSkill( SkillName.Tactics, 60.1, 100.0 );
			SetSkill( SkillName.Wrestling, 60.1, 100.0 );

			Fame = 8500;
			Karma = -8500;

			VirtualArmor = 34;
			ControlSlots = 2;

			PackItem( new FertileDirt( Utility.RandomMinMax( 1, 4 ) ) );
			PackItem( new IronOre( 3 ) ); // TODO: Five small iron ore
			PackItem( new MandrakeRoot() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Gems );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }

		public MithrilElemental( Serial serial ) : base( serial )
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