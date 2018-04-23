using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "-Could Eat THis Later-" )]
	public class BattleScorpCold : BaseCreature
	{
		[Constructable]
		public BattleScorpCold() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "-Cold Battle Scorpion-";
			Body = 717;
            Hue = Utility.RandomMinMax(1346, 1354);
            NameHue = 0x47F;
			SetStr( 550, 650 );
			SetDex( 250, 350 );
			SetInt( 150, 250 );

			SetHits( 300, 550 );

			SetDamage( 13, 19 );

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Cold, 90);

			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 50 );
            SetResistance(ResistanceType.Cold,  75);
            SetResistance(ResistanceType.Poison, 50);
            SetResistance(ResistanceType.Energy, 50);

			SetSkill( SkillName.MagicResist, 100.0, 120.0 );
			SetSkill( SkillName.Tactics, 100.0, 120.0 );
			SetSkill( SkillName.Wrestling, 100.0, 120.0 );
            SetSkill(SkillName.Anatomy, 100.0, 120.0);
            SetSkill(SkillName.Healing, 100.0, 120.0);

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 100.0;
		}

		public override int Meat{ get{ return 1; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay; } }

		public override int GetIdleSound() { return 1511; } 
		public override int GetAngerSound() { return 1508; } 
		public override int GetHurtSound() { return 1510; } 
		public override int GetDeathSound()	{ return 1509; }

		public BattleScorpCold( Serial serial ) : base( serial )
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