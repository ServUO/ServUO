using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "-Oh My-" )]
	public class BattleChickenLizardTerra : BaseCreature
	{
		[Constructable]
		public BattleChickenLizardTerra() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "-Terra Battle Lizard-";
			Body = 716;
            Hue = Utility.RandomMinMax( 1150, 1156 );
            NameHue = 0x480;

            SetStr(550, 650);
            SetDex(250, 350);
            SetInt(150, 250);

            SetHits(300, 550);

            SetDamage(13, 19);

			SetDamageType( ResistanceType.Physical, 20 );
            SetDamageType(ResistanceType.Cold, 20 );
            SetDamageType(ResistanceType.Fire, 20 );
            SetDamageType(ResistanceType.Energy, 20 );
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 60);
            SetResistance(ResistanceType.Fire, 60);
            SetResistance(ResistanceType.Cold, 60);
            SetResistance(ResistanceType.Poison, 60);
            SetResistance(ResistanceType.Energy, 60);

            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);
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

		public BattleChickenLizardTerra( Serial serial ) : base( serial )
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