using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an atropal scion corpse" )]
	public class AtropalScion : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.MortalStrike;
		}

		[Constructable]
		public AtropalScion() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an atropal scion";
			Hue = 35;
			BaseSoundID = 959;

			switch ( Utility.Random( 2 ) )
			{
				case 0: // ghoul
					Body = 153;
					BaseSoundID = 471;
					break;
				case 1: // kaze kemono
					Body = 196;
					BaseSoundID = 0x39D;
					break;
			}


			SetStr( 126, 150 );
			SetDex( 126, 140 );
			SetInt( 406, 420 );

			SetHits( 276, 280 );

			SetDamage( 14, 16 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 60 );
			SetDamageType( ResistanceType.Poison, 20 );

			SetResistance( ResistanceType.Physical, 70, 80 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 180, 200 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 180, 200 );

			SetSkill( SkillName.MagicResist, 120.1, 148.0 );
			SetSkill( SkillName.Tactics, 145.1, 160.0 );
			SetSkill( SkillName.Wrestling, 80.1, 90.0 );

			Fame = 28500;
			Karma = -28500;

			VirtualArmor = 100;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override bool BleedImmune{ get{ return true; } }

		public AtropalScion( Serial serial ) : base( serial )
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