using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a giant turkey corpse" )]
	public class GiantTurkey : BaseCreature
	{
		[Constructable]
		public GiantTurkey()
            : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a giant turkey";
			Body = 1026;
			BaseSoundID = 0x66A;

            SetStr(1200, 1400);
            SetDex(170, 260);
            SetInt(430, 560);

            SetHits(25000);
            SetMana(1000);

            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 70);
            SetResistance(ResistanceType.Fire, 70, 90);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.MagicResist, 85.0, 100.0);
            SetSkill(SkillName.Tactics, 100.0, 110.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);
            SetSkill(SkillName.Anatomy, 75.0, 80.0);

            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetAreaEffect(AreaEffect.EssenceOfDisease);
            SetSpecialAbility(SpecialAbility.HowlOfCacophony);
            SetWeaponAbility(WeaponAbility.Dismount);
        }

        public override int Meat { get { return 1; } }
        public override MeatType MeatType { get { return MeatType.Bird; } }
        public override FoodType FavoriteFood { get { return FoodType.GrainsAndHay; } }
        public override int Feathers { get { return 25; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich);
        }

        public override int GetIdleSound()
        {
            return 0x66A;
        }

        public override int GetAngerSound()
        {
            return 0x66A;
        }

        public override int GetHurtSound()
        {
            return 0x66B;
        }

        public override int GetDeathSound()
        {
            return 0x66B;
        }

		public GiantTurkey(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}
