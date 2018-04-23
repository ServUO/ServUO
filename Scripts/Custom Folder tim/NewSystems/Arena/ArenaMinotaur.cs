using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a minotaur corpse" )]
	
	public class ArenaMinotaur : BaseCreature
	{
		[Constructable]
		public ArenaMinotaur() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Minotaur";
			Body = 263;
			BaseSoundID = 427;

			SetStr( 967, 1245 );
			SetDex( 266, 375 );
			SetInt( 146, 170 );

			SetHits( 976, 1352 );

			SetDamage( 40, 65 );

			SetDamageType( ResistanceType.Physical, 100 );			

			SetResistance( ResistanceType.Physical, 75, 85 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 40, 50 );
			SetResistance( ResistanceType.Fire, 60, 70 );

			SetSkill( SkillName.Macing, 145.1, 170.0 );
			SetSkill( SkillName.MagicResist, 145.1, 170.0 );
			SetSkill( SkillName.Tactics, 145.1, 170.0 );
			SetSkill( SkillName.Wrestling, 145.1, 170.0 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 70;

			PackItem( new Club() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 4 );
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Meager );
		}

		public override bool AlwaysMurderer { get { return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		public override bool OnBeforeDeath()
		{
			this.Say( "Meet Your Next Challenge!!!" );
			ArenaKnight an = new ArenaKnight();
			an.MoveToWorld( new Point3D( 2367, 1148, -90 ), Map.Malas );
			an.Team = this.Team;
			an.Combatant = this.Combatant;
		
			return true;
		}

		public ArenaMinotaur( Serial serial ) : base( serial )
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