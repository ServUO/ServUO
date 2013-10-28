using System;
using Server.Items;

namespace Server.Mobiles 
{
    public class HirePaladin : BaseHire 
    {
        [Constructable] 
        public HirePaladin()
        {
            this.SpeechHue = Utility.RandomDyedHue();
            this.Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool()) 
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else 
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }
            this.Title = "the paladin";
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.Race.RandomFacialHair(this);

            switch( Utility.Random(5) )
            {
                case 0:
                    break;
                case 1:
                    this.AddItem(new Bascinet());
                    break;
                case 2:
                    this.AddItem(new CloseHelm());
                    break;
                case 3:
                    this.AddItem(new NorseHelm());
                    break;
                case 4:
                    this.AddItem(new Helmet());
                    break;
            }

            this.SetStr(86, 100);
            this.SetDex(81, 95);
            this.SetInt(61, 75);

            this.SetDamage(10, 23);

            this.SetSkill(SkillName.Swords, 66.0, 97.5);
            this.SetSkill(SkillName.Anatomy, 65.0, 87.5);
            this.SetSkill(SkillName.MagicResist, 25.0, 47.5);
            this.SetSkill(SkillName.Healing, 65.0, 87.5);
            this.SetSkill(SkillName.Tactics, 65.0, 87.5);
            this.SetSkill(SkillName.Wrestling, 15.0, 37.5);
            this.SetSkill(SkillName.Parry, 45.0, 60.5);
            this.SetSkill(SkillName.Chivalry, 85, 100);

            this.Fame = 100;
            this.Karma = 250;

            this.AddItem(new Shoes(Utility.RandomNeutralHue()));
            this.AddItem(new Shirt());
            this.AddItem(new VikingSword());
            this.AddItem(new MetalKiteShield());
 
            this.AddItem(new PlateChest());
            this.AddItem(new PlateLegs());
            this.AddItem(new PlateArms());
            this.AddItem(new LeatherGorget());
            this.PackGold(20, 100);
        }

        public HirePaladin(Serial serial)
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