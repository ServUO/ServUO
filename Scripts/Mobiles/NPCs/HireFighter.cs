using Server.Items;

namespace Server.Mobiles
{
    public class HireFighter : BaseHire
    {
        [Constructable]
        public HireFighter()
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

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

            Title = "the fighter";
            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            SetStr(91, 91);
            SetDex(91, 91);
            SetInt(50, 50);

            SetDamage(7, 14);

            SetSkill(SkillName.Tactics, 36, 67);
            SetSkill(SkillName.Magery, 22, 22);
            SetSkill(SkillName.Swords, 64, 100);
            SetSkill(SkillName.Parry, 60, 82);
            SetSkill(SkillName.Macing, 36, 67);
            SetSkill(SkillName.Focus, 36, 67);
            SetSkill(SkillName.Wrestling, 25, 47);

            Fame = 100;
            Karma = 100;

            switch (Utility.Random(2))
            {
                case 0:
					SetWearable(new Shoes(), Utility.RandomNeutralHue(), 1);
                    break;
                case 1:
					SetWearable(new Boots(), Utility.RandomNeutralHue(), 1);
                    break;
            }

			SetWearable(new Shirt());

            // Pick a random sword
            switch (Utility.Random(5))
            {
                case 0:
					SetWearable(new Longsword(), dropChance: 1);
                    break;
                case 1:
					SetWearable(new Broadsword(), dropChance: 1);
                    break;
                case 2:
					SetWearable(new VikingSword(), dropChance: 1);
                    break;
                case 3:
					SetWearable(new BattleAxe(), dropChance: 1);
                    break;
                case 4:
					SetWearable(new TwoHandedAxe(), dropChance: 1);
                    break;
            }

            // Pick a random shield
            if (FindItemOnLayer(Layer.TwoHanded) == null)
            {
                switch (Utility.Random(8))
                {
                    case 0:
						SetWearable(new BronzeShield(), dropChance: 1);
                        break;
                    case 1:
						SetWearable(new HeaterShield(), dropChance: 1);
                        break;
                    case 2:
						SetWearable(new MetalKiteShield(), dropChance: 1);
                        break;
                    case 3:
						SetWearable(new MetalShield(), dropChance: 1);
                        break;
                    case 4:
						SetWearable(new WoodenKiteShield(), dropChance: 1);
                        break;
                    case 5:
						SetWearable(new WoodenShield(), dropChance: 1);
                        break;
                    case 6:
						SetWearable(new OrderShield(), dropChance: 1);
                        break;
                    case 7:
						SetWearable(new ChaosShield(), dropChance: 1);
                        break;
                }
            }

            switch (Utility.Random(5))
            {
                case 0:
                    break;
                case 1:
					SetWearable(new Bascinet(), dropChance: 1);
                    break;
                case 2:
					SetWearable(new CloseHelm(), dropChance: 1);
                    break;
                case 3:
					SetWearable(new NorseHelm(), dropChance: 1);
                    break;
                case 4:
					SetWearable(new Helmet(), dropChance: 1);
                    break;
            }
            // Pick some armour
            switch (Utility.Random(4))
            {
                case 0: // Leather
                    SetWearable(new LeatherChest(), dropChance: 1);
                    SetWearable(new LeatherArms(), dropChance: 1);
                    SetWearable(new LeatherGloves(), dropChance: 1);
					SetWearable(new LeatherGorget(), dropChance: 1);
					SetWearable(new LeatherLegs(), dropChance: 1);
                    break;
                case 1: // Studded Leather
                    SetWearable(new StuddedChest(), dropChance: 1);
                    SetWearable(new StuddedArms(), dropChance: 1);
                    SetWearable(new StuddedGloves(), dropChance: 1);
                    SetWearable(new StuddedGorget(), dropChance: 1);
					SetWearable(new StuddedLegs(), dropChance: 1);
                    break;
                case 2: // Ringmail
                    SetWearable(new RingmailChest(), dropChance: 1);
                    SetWearable(new RingmailArms(), dropChance: 1);
                    SetWearable(new RingmailGloves(), dropChance: 1);
					SetWearable(new RingmailLegs(), dropChance: 1);
                    break;
                case 3: // Chain
					SetWearable(new ChainChest(), dropChance: 1);
					//SetWearable(new ChainCoif(), dropChance: 1);
					SetWearable(new ChainLegs(), dropChance: 1);
                    break;
            }

            PackGold(25, 100);
        }

        public HireFighter(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle => false;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);// version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}