using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "corpse of Wildebeest" )]
	public class Wildebeest : BaseCreature
	{
		[Constructable]
		public Wildebeest () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Wildebeest";
			Body = 280;
			BaseSoundID = 362;

			SetStr( 500, 600 );
			SetDex( 2500, 2600 );
			SetInt( 1500, 1600 );

			SetHits( 50000 );

			SetDamage( 100, 500 );

			SetDamageType( ResistanceType.Physical, 125 );

			SetResistance( ResistanceType.Physical, 100, 125 );
			SetResistance( ResistanceType.Fire, 100, 125 );
			SetResistance( ResistanceType.Cold, 100, 125 );
			SetResistance( ResistanceType.Poison, 100, 125 );
			SetResistance( ResistanceType.Energy, 100, 125 );

			SetSkill( SkillName.EvalInt, 100.0, 140.0 );
			SetSkill( SkillName.Meditation, 100.0, 140.0 );
			SetSkill( SkillName.MagicResist, 100.0, 140.0 );
			SetSkill( SkillName.Tactics, 100.0, 140.0 );
			SetSkill( SkillName.Wrestling, 10.0, 140.0 );

			Fame = 22500;
			Karma = -22500;
                        
			Tamable = false;
			ControlSlots = 4;
			MinTameSkill = 130.0;
			
			VirtualArmor = 70;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 50000 );
		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		//public override HideType HideType{ get{ return HideType.Barbed; } }
		//public override int Hides{ get{ return 40; } }
		public override int Meat{ get{ return 19; } }
		public override int Scales{ get{ return 12; } }
		public override ScaleType ScaleType{ get{ return (ScaleType)Utility.Random( 4 ); } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 5; } }

		public Wildebeest( Serial serial ) : base( serial )
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