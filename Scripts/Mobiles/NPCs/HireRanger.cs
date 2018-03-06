using System;
using Server.Items;

namespace Server.Mobiles 
{
    public class HireRanger : BaseHire 
    {
        [Constructable] 
        public HireRanger()
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
                AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }

            Title = "the ranger";
            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            SetStr(91, 91);
            SetDex(76, 76);
            SetInt(61, 61);

            SetDamage(13, 24);

            SetSkill(SkillName.Wrestling, 15, 37);
            SetSkill(SkillName.Parry, 45, 60);
            SetSkill(SkillName.Archery, 66, 97);
            SetSkill(SkillName.Magery, 62, 62);
            SetSkill(SkillName.Swords, 35, 57);
            SetSkill(SkillName.Fencing, 15, 37);
            SetSkill(SkillName.Tactics, 65, 87);

            Fame = 100;
            Karma = 125;

            AddItem(new Shoes(Utility.RandomNeutralHue()));
            AddItem(new Shirt());

            // Pick a random sword
            switch ( Utility.Random(3)) 
            {
                case 0:
                    AddItem(new Longsword());
                    break;
                case 1:
                    AddItem(new VikingSword());
                    break;
                case 2:
                    AddItem(new Broadsword());
                    break;
            }

            SetWearable(new StuddedChest(), 0x59C);
            SetWearable(new StuddedArms(), 0x59C);
            SetWearable(new StuddedGloves(), 0x59C);
            SetWearable(new StuddedLegs(), 0x59C);
            SetWearable(new StuddedGorget(), 0x59C);

            PackItem(new Arrow(20));
            PackGold(10, 75);
        }

        public HireRanger(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer) 
        {
            base.Serialize(writer);

            writer.Write((int)0);// version 
        }

        public override void Deserialize(GenericReader reader) 
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}