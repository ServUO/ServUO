//*^* Created By Plague- *^*//
using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "The corpse of Crockett Scarr" )]
	public class Crockett : BaseCreature
	{
		[Constructable]
		public Crockett() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Crockett Scarr";
			Body = 400;
			BaseSoundID = 0x481;

		    	SetStr( 1250 );
			SetDex( 1500 );
			SetInt( 1150 );

			SetHits( 55800 );
			SetMana( 55500 );

			SetDamage( 25, 35 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 530 );
			SetDamageType( ResistanceType.Fire, 500 );
			SetDamageType( ResistanceType.Energy, 500);
			SetDamageType( ResistanceType.Poison, 520 );

			SetResistance( ResistanceType.Physical, 230 );
			SetResistance( ResistanceType.Fire, 250 );
			SetResistance( ResistanceType.Cold, 100 );
			SetResistance( ResistanceType.Poison, 250 );
			SetResistance( ResistanceType.Energy, 210 );

			SetSkill( SkillName.Anatomy, 85.0, 100.0 );
			SetSkill( SkillName.EvalInt, 100.0, 105.0 );
			SetSkill( SkillName.Magery, 105.0, 110.0 );
			SetSkill( SkillName.MagicResist, 85.0, 100.0 );
			SetSkill( SkillName.Meditation, 50.0, 100.0 );
			SetSkill( SkillName.Wrestling, 95.0, 105.0 );
			

			Fame = 30000;
			Karma = -30000;

			VirtualArmor = 250;
			
			PackGold( 6500, 7025  );
			
			DemonPactChest Chest = new DemonPactChest();
			Chest.Movable = false;
			AddItem(Chest);
			
			DemonPactLegs Legs = new DemonPactLegs();
			Legs.Movable = false;
			AddItem(Legs);
			
			DemonPactGloves Gloves = new DemonPactGloves();
			Gloves.Movable = false;
			AddItem(Gloves);
			
			DemonPactHelm Helm = new DemonPactHelm();
			Helm.Movable = false;
			AddItem(Helm);

	    		DemonPactFork Weapon = new DemonPactFork();
			Weapon.Movable = false;
			AddItem(Weapon);

	    		DemonPactShield Shield = new DemonPactShield();
			Shield.Movable = false;
			AddItem(Shield);
			
		}

		public override bool OnBeforeDeath()

		{

		PackItem( new Obolus() );

		return base.OnBeforeDeath();

		}


		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 3 );
			AddLoot( LootPack.Gems, 17 );
		}

		public override bool AutoDispel{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override bool Unprovokable{ get{ return true; } }
		public override bool Uncalmable{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }

		public Crockett( Serial serial ) : base( serial )
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