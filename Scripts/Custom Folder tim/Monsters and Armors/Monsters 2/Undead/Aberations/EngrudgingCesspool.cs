using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an engrudging cesspool corpse" )]
	public class EngrudgingCesspool : BaseCreature
	{
		[Constructable]
		public EngrudgingCesspool() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an engrudging cesspool";
			Body = 163;
			BaseSoundID = 263;
			Hue = 2952;

			SetStr( 226, 255 );
			SetDex( 126, 135 );
			SetInt( 71, 95 );

			SetHits( 496, 513 );

			SetDamage( 14, 18 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 60 );
			SetDamageType( ResistanceType.Cold, 20 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 80, 85 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.MagicResist, 50.1, 65.0 );
			SetSkill( SkillName.Tactics, 80.1, 100.0 );
			SetSkill( SkillName.Wrestling, 80.1, 100.0 );

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 50;

			PackItem( new IronOre( 3 ) );
			PackItem( new BlackPearl( 3 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override bool BleedImmune{ get{ return true; } }

		public override int TreasureMapLevel{ get{ return Utility.RandomList( 2, 3 ); } }

		public EngrudgingCesspool( Serial serial ) : base( serial )
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