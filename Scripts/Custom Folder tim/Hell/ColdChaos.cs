using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an cold chaos corpse" )]
	public class ColdChaos : BaseCreature
	{
		[Constructable]
		public ColdChaos () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Cold Chaos";
			Body = 43;
			BaseSoundID = 357;

			SetStr( 876, 905 );
			SetDex( 276, 295 );
			SetInt( 301, 325 );

			SetHits( 1226, 1243 );

			SetDamage( 28, 49 );

			SetDamageType( ResistanceType.Cold, 100 );

			SetSkill( SkillName.EvalInt, 80.1, 90.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.MagicResist, 75.1, 95.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 70;

			if( Utility.RandomDouble() <= 0.01 ) PackItem( new VoiceOfTheFallenKing() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 1; } }

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			c.DropItem( new ClearVision() );
		}

		public ColdChaos( Serial serial ) : base( serial )
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