using Server.Items;

namespace Server.Mobiles
{
    public class HirePeasant : BaseHire
    {
        [Constructable]
        public HirePeasant()
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                switch (Utility.Random(2))
                {
                    case 0:
                        AddItem(new Skirt(Utility.RandomNeutralHue()));
                        break;
                    case 1:
                        AddItem(new Kilt(Utility.RandomNeutralHue()));
                        break;
                }
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }
            Title = "the peasant";
            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            AddItem(new Katana());

            SetStr(26, 26);
            SetDex(21, 21);
            SetInt(16, 16);

            SetDamage(10, 23);

            SetSkill(SkillName.Tactics, 5, 27);
            SetSkill(SkillName.Wrestling, 5, 5);
            SetSkill(SkillName.Swords, 5, 27);

            Fame = 0;
            Karma = 0;

            AddItem(new Sandals(Utility.RandomNeutralHue()));
            switch (Utility.Random(2))
            {
                case 0:
                    AddItem(new Doublet(Utility.RandomNeutralHue()));
                    break;
                case 1:
                    AddItem(new Shirt(Utility.RandomNeutralHue()));
                    break;
            }

            PackGold(0, 25);
        }

        public HirePeasant(Serial serial)
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