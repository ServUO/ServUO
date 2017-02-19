using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a plant corpse" )]
	public class Vasanord : BaseVoidCreature
	{
        public override VoidEvolution Evolution { get { return VoidEvolution.Survival; } }
        public override int Stage { get { return 3; } }

		[Constructable]
		public Vasanord() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.6, 1.2 )
		{
			Name = "vasanord";
			Body = 780;

			SetStr( 805, 869 );
			SetDex( 51, 64 );
			SetInt( 38, 48 );

			SetHits( 5000, 5200 );
			SetMana( 40, 70 );
			SetStam( 50, 80 );

			SetDamage( 10, 23 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Cold, 20 );
			SetDamageType( ResistanceType.Poison, 20 );
			SetDamageType( ResistanceType.Energy, 20 );

			SetResistance( ResistanceType.Physical, 30, 50 );
			SetResistance( ResistanceType.Fire, 20, 50 );
			SetResistance( ResistanceType.Cold, 20, 40 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 20, 50 );

			SetSkill( SkillName.MagicResist, 72.8, 77.7 );
			SetSkill( SkillName.Tactics, 50.7, 110.0 );
			SetSkill( SkillName.EvalInt, 99.5, 120.0 );
			SetSkill( SkillName.Magery, 95.5, 106.9 );
			SetSkill( SkillName.Wrestling, 53.6, 98.6 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 28;

		    PackItem( new DaemonBone( 30 ) );
		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.6)
                c.DropItem(new TaintedSeeds(2));
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
		}

		public override bool BardImmune{ get{ return !Core.AOS; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public Vasanord( Serial serial ) : base( serial )
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