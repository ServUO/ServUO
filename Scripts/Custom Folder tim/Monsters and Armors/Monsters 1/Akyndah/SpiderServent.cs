//modified by Neptune
 
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a spider servant corpse" )]
	public class SpiderServant : BaseCreature
	{
		[Constructable]
		public SpiderServant() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Spider Servent";
			Hue = 2654;
			Body = 173;
			BaseSoundID = 1170;

			SetStr( 1750, 2000 );
			SetDex( 1500, 1750 );
			SetInt( 1060, 1250 );

			SetHits( 11000, 16000 );

			SetDamage( 50, 55 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Poison, 60 );

			SetResistance( ResistanceType.Physical, 50, 60 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 55, 65 );
			SetResistance( ResistanceType.Energy, 40, 50 );
			
			SetSkill( SkillName.Parry, 50.1, 75.0 );
			SetSkill( SkillName.Poisoning, 100.1, 110.0 );
			SetSkill( SkillName.Wrestling, 50.1, 75.0 );
			SetSkill( SkillName.MagicResist, 80.1, 100.0 );
			SetSkill( SkillName.Tactics, 50.1, 75.0 );

			Fame = 6000;
			Karma = -6000;

			VirtualArmor = 50;
			
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
		}
		
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison HitPoison{ get{ return Poison.Regular; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		

		public SpiderServant( Serial serial ) : base( serial )
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

