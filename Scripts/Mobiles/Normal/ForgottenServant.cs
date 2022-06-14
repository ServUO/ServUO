using System;
using Server.Items;

namespace Server.Mobiles
{
    public class ForgottenServant : BaseCreature
    {
        [Constructable]
        public ForgottenServant()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "Forgotten Servant";
            Hue = 768;

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
				SetWearable(new Skirt(), Utility.RandomNeutralHue(), 1);
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
				SetWearable(new ShortPants(), Utility.RandomNeutralHue(), 1);
            }

            SetStr(147, 215);
            SetDex(91, 115);
            SetInt(61, 85);

            SetHits(95, 123);

            SetDamage(4, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 70.1, 85.0);
            SetSkill(SkillName.Swords, 60.1, 85.0);
            SetSkill(SkillName.Tactics, 75.1, 90.0);
            SetSkill(SkillName.Wrestling, 60.1, 85.0);

            Fame = 2500;
            Karma = -2500;

			SetWearable(new Boots(), Utility.RandomNeutralHue(), 1);
			SetWearable(new FancyShirt(), dropChance: 1);
			SetWearable(new Bandana(), dropChance: 1);
			SetWearable((Item)Activator.CreateInstance(Utility.RandomList(_WeaponsList)), dropChance: 1);

			Utility.AssignRandomHair(this);
        }

        public ForgottenServant(Serial serial)
            : base(serial)
        {
        }

		private static readonly Type[] _WeaponsList =
		{
			typeof(Longsword), typeof(Cutlass), typeof(Broadsword), typeof(Axe), typeof(Club), typeof(Dagger), typeof(Spear)
		};

		public override bool ClickTitle => false;
        public override bool AlwaysMurderer => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}