using System;
using Server.Items;

namespace Server.Mobiles 
{
    public class HireBard : BaseHire 
    {
        [Constructable] 
        public HireBard()
        {
            this.SpeechHue = Utility.RandomDyedHue();
            this.Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool()) 
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");

                switch ( Utility.Random(2) )
                {
                    case 0:
                        this.AddItem(new Skirt(Utility.RandomDyedHue()));
                        break;
                    case 1:
                        this.AddItem(new Kilt(Utility.RandomNeutralHue()));
                        break;
                }
            }
            else 
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }
            this.Title = "the bard";
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.Race.RandomFacialHair(this);

            this.SetStr(16, 16);
            this.SetDex(26, 26);
            this.SetInt(26, 26);

            this.SetDamage(5, 10);

            this.SetSkill(SkillName.Tactics, 35, 57);
            this.SetSkill(SkillName.Magery, 22, 22);
            this.SetSkill(SkillName.Swords, 45, 67);
            this.SetSkill(SkillName.Archery, 36, 67);
            this.SetSkill(SkillName.Parry, 45, 60);
            this.SetSkill(SkillName.Musicianship, 66.0, 97.5);
            this.SetSkill(SkillName.Peacemaking, 65.0, 87.5);

            this.Fame = 100;
            this.Karma = 100;

            this.AddItem(new Shoes(Utility.RandomNeutralHue()));

            switch ( Utility.Random(2) )
            {
                case 0:
                    this.AddItem(new Doublet(Utility.RandomDyedHue()));
                    break;
                case 1:
                    this.AddItem(new Shirt(Utility.RandomDyedHue()));
                    break;
            }
            switch ( Utility.Random(4) )
            {
                case 0:
                    this.PackItem(new Harp());
                    break;
                case 1:
                    this.PackItem(new Lute());
                    break;
                case 2:
                    this.PackItem(new Drums());
                    break;
                case 3:
                    this.PackItem(new Tambourine());
                    break;
            }

            this.AddItem(new Longsword());
            this.PackItem(new Bow());
            this.PackItem(new Arrow(100));
            this.PackGold(10, 50);
        }

        public HireBard(Serial serial)
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