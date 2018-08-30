using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a plague bat corpse" )]
	public class PlagueBat : BaseCreature
	{
		[Constructable]
		public PlagueBat() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a plague bat";
			Body = 317;
			BaseSoundID = 0x270;
			Hue = 365;

			SetStr( 91, 110 );
			SetDex( 91, 115 );
			SetInt( 26, 50 );

			SetHits( 120, 160 );

			SetDamage( 17, 25 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 35, 25 );
			SetResistance( ResistanceType.Cold, 65, 75 );
			SetResistance( ResistanceType.Poison, 70, 80 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.MagicResist, 70.1, 95.0 );
			SetSkill( SkillName.Tactics, 75.1, 80.0 );
			SetSkill( SkillName.Wrestling, 70.1, 85.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 14;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public override int GetIdleSound()
		{
			return 0x29B;
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public PlagueBat( Serial serial ) : base( serial )
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