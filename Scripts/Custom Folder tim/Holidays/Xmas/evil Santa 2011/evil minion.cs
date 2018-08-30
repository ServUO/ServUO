using System;
using Server;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Commands; 


namespace Server.Mobiles
{
	[CorpseName( "Evil Minion" )]
	public class EvilMinionone : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return Utility.RandomBool() ? WeaponAbility.MortalStrike : WeaponAbility.WhirlwindAttack;
		}

		public override bool IgnoreYoungProtection { get { return Core.ML; } }

		[Constructable]
		public EvilMinionone() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "The Evil Minion";
			Body = 400;
			Hue = 33770;
			BaseSoundID = 343;

			SetStr( 301, 420 );
			SetDex( 81, 90 );
			SetInt( 301, 320 );

			SetHits( 478, 595 );

			SetDamage( 40, 120 );

			SetDamageType( ResistanceType.Cold, 20 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 50 );
			SetResistance( ResistanceType.Cold, 15 );
			SetResistance( ResistanceType.Poison, 60 );
			SetResistance( ResistanceType.Energy, 80 );

			SetSkill( SkillName.EvalInt, 200.0 );
			SetSkill( SkillName.Magery, 200.0 );
			SetSkill( SkillName.Meditation, 200.0 );
			SetSkill( SkillName.MagicResist, 200.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 200.0 );

			Fame = 1500;
			Karma = -50000;

			VirtualArmor = 40;

            JesterHat hat = new JesterHat();
			hat.Name = "Elf Hat";
			hat.Hue = 64;
			hat.Movable = false;
			AddItem( hat );

			ElvenPants legs = new ElvenPants();
			legs.Hue = 64;
			legs.Movable = false;
			AddItem( legs );

			FancyShirt chest = new FancyShirt();
			chest.Hue = 64;
			chest.Movable = false;
			AddItem( chest );

			LeatherGloves gloves = new LeatherGloves();
			gloves.Hue = 64;
			gloves.Movable = false;
			AddItem( gloves );

			ElvenBoots boots = new ElvenBoots();
			boots.Hue = 64;
			boots.Movable = false;
			AddItem( boots );

			
			BodySash bodysash = new BodySash();
			bodysash.Hue = 64;
			bodysash.Movable = false;
			AddItem ( bodysash );
			
			HalfApron halfapron = new HalfApron();
			halfapron.Hue = 64;
			halfapron.Movable = false;
			AddItem ( halfapron );

			Cloak cloak = new Cloak();
			cloak.Hue = 64;
			cloak.Movable = false;
			AddItem ( cloak );
			
			PackGold( 6000, 10000);

                }

		public override bool BardImmune{ get{ return !Core.ML; } }
		public override bool Unprovokable{ get{ return Core.ML; } }
		public override bool Uncalmable{ get{ return Core.ML; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 6; } }
            public override bool AlwaysMurderer{ get{ return true; } }


		public EvilMinionone( Serial serial ) : base( serial )
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