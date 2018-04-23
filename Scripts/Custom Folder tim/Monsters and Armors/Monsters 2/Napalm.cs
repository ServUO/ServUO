using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a napalm corpse" )]
	public class Napalm : BaseCreature
	{
		[Constructable]
		public Napalm() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a napalm";
			Body = 94;
			BaseSoundID = 456;
			Hue = Utility.RandomList(1355,1356,1357,1358,1359,1360,1255,1256,1257,1258,1259,1260,2125,1161,1196,2114,53);

			SetStr( 48, 50 );
			SetDex( 46, 51 );
			SetInt( 46, 50 );

			SetHits( 43, 67 );

			SetDamage( 13, 19 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Fire, 90 );

			SetResistance( ResistanceType.Physical, 15, 20 );
			SetResistance( ResistanceType.Fire, 70, 80 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.MagicResist, 5.1, 10.0 );
			SetSkill( SkillName.Tactics, 49.3, 54.0 );
			SetSkill( SkillName.Wrestling, 25.3, 40.0 );

			Fame = 850;
			Karma = -850;

			VirtualArmor = 38;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Gems, Utility.RandomMinMax( 1, 2 ) );
		}

		public Napalm( Serial serial ) : base( serial )
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