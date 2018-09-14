#region References
using Server.Items;
using Server.Misc;
using Server.Targeting;
#endregion

namespace Server.Mobiles
{
    [CorpseName("a human corpse")]
    public class KhalAnkurWarriors : BaseCreature
	{
		[Constructable]
		public KhalAnkurWarriors()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            string[] titles = { "the Scout", "the Corporal", "the Lieutenant", "the Captain", "the General" };

            Title = titles[Utility.Random(titles.Length)];
            
			BaseSoundID = 0x45A;

            SetStr(150, 250);
            SetDex(150);
            SetInt(25, 44);

            SetDamage(10, 22);

            SetDamageType(ResistanceType.Physical, 100);

			SetResistance(ResistanceType.Physical, 30, 50);
			SetResistance(ResistanceType.Fire, 30, 50);
			SetResistance(ResistanceType.Cold, 30, 50);
			SetResistance(ResistanceType.Poison, 30, 50);
			SetResistance(ResistanceType.Energy, 30, 50);
            
            SetSkill(SkillName.Fencing, 100.0);
            SetSkill(SkillName.Macing, 100.0);
            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Swords, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Archery, 100.0);

            Fame = 5000;
			Karma = -5000;

            switch (Utility.Random(4))
            {
                case 0:
                    {
                        Hue = 2697;
                        SetWearable(new ChainChest(), Hue);                        
                        SetWearable(new ChainCoif(), Hue);
                        SetWearable(new Cloak(), Hue);

                        break;
                    }
                case 1:
                    {
                        Hue = 2697;
                        SetWearable(new PlateChest(), Hue);
                        SetWearable(new PlateLegs(), Hue);
                        SetWearable(new Cloak(), Hue);
                        break;
                    }
                case 2:
                    {
                        Hue = 2684;
                        SetWearable(new PlateChest(), Hue);
                        SetWearable(new PlateLegs(), Hue);
                        SetWearable(new PlateArms(), Hue);
                        break;
                    }
                case 3:
                    {
                        Hue = 2697;
                        SetWearable(new ChainChest(), Hue);
                        SetWearable(new ChainLegs(), Hue);
                        SetWearable(new Boots(Hue));
                        break;
                    }
            }            

            switch (Utility.Random(5))
            {
                case 0: SetWearable(new Spear()); break;
                case 1: SetWearable(new QuarterStaff()); break;
                case 2: SetWearable(new BlackStaff()); break;
                case 3:
                    {
                        switch (Utility.Random(4))
                        {
                            case 0: SetWearable(new Yumi()); break;
                            case 1: SetWearable(new Crossbow()); break;
                            case 2: SetWearable(new RepeatingCrossbow()); break;
                            case 3: SetWearable(new HeavyCrossbow()); break;
                        }

                        RangeFight = 3;
                        AI = AIType.AI_Archer;

                        break;
                    }
                case 4: SetWearable(new Tessen()); break;
            }

            int hairHue = Utility.RandomNondyedHue();

            Utility.AssignRandomHair(this, hairHue);

            if (Utility.Random(7) != 0)
                Utility.AssignRandomFacialHair(this, hairHue);
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        public KhalAnkurWarriors(Serial serial)
			: base(serial)
		{
        }

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Rich);
		}

        public override WeaponAbility GetWeaponAbility()
        {
            BaseWeapon wep = Weapon as BaseWeapon;

            if (wep != null && !(wep is Fists))
            {
                if (Utility.RandomDouble() > 0.5)
                    return wep.PrimaryAbility;

                return wep.SecondaryAbility;
            }

            return null;
        }

        public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}
