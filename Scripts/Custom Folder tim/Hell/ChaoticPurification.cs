using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a charred corpse" )]
	public class ChaoticPurification : BaseCreature
	{
		[Constructable]
		public ChaoticPurification() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Chaotic Purification";
			Body = 43;
                        Hue = 1161;
			BaseSoundID = 0x174;

			SetStr( 751, 800 );
			SetDex( 226, 245 );
			SetInt( 326, 350 );

			SetHits( 1211, 1240 );

			SetDamage( 17, 34 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Fire, 90 );

			SetResistance( ResistanceType.Physical, 70, 75 );
			SetResistance( ResistanceType.Fire, 90, 100 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.Anatomy, 85.1, 95.0 );
			SetSkill( SkillName.EvalInt, 90.1, 105.0 );
			SetSkill( SkillName.Magery, 90.1, 105.0 );
			SetSkill( SkillName.Meditation, 90.1, 105.0 );
			SetSkill( SkillName.MagicResist, 90.1, 105.0 );
			SetSkill( SkillName.Tactics, 80.1, 100.0 );
			SetSkill( SkillName.Wrestling, 80.1, 90.0 );

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 52;
			if( Utility.RandomDouble() <= 0.20 ) PackItem( new DaemonGloves() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 13; } }

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			c.DropItem( new SamuraiHelmOfFire() );
		}

		public ChaoticPurification( Serial serial ) : base( serial )
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