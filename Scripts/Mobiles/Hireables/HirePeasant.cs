using System;
using Server.Items;

namespace Server.Mobiles 
{
    public class HirePeasant : BaseHire 
    {
        [Constructable] 
        public HirePeasant()
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
                        this.AddItem(new Skirt(Utility.RandomNeutralHue()));
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
            this.Title = "the peasant";
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.Race.RandomFacialHair(this);

            this.AddItem(new Katana());

            this.SetStr(26, 26);
            this.SetDex(21, 21);
            this.SetInt(16, 16);

            this.SetDamage(10, 23);

            this.SetSkill(SkillName.Tactics, 5, 27);
            this.SetSkill(SkillName.Wrestling, 5, 5);
            this.SetSkill(SkillName.Swords, 5, 27);

            this.Fame = 0;
            this.Karma = 0;

            this.AddItem(new Sandals(Utility.RandomNeutralHue()));
            switch ( Utility.Random(2) )
            {
                case 0:
                    this.AddItem(new Doublet(Utility.RandomNeutralHue()));
                    break;
                case 1:
                    this.AddItem(new Shirt(Utility.RandomNeutralHue()));
                    break;
            }
		
            this.PackGold(0, 25);
        }

        public HirePeasant(Serial serial)
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