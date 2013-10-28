using System;
using Server.Items;

namespace Server.Mobiles 
{
    public class HireSailor : BaseHire 
    {
        [Constructable] 
        public HireSailor()
        {
            this.SpeechHue = Utility.RandomDyedHue();
            this.Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool()) 
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
                this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }
            this.Title = "the sailor";
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.Race.RandomFacialHair(this);

            this.SetStr(86);
            this.SetDex(66);
            this.SetInt(41);

            this.SetDamage(10, 23);

            this.SetSkill(SkillName.Stealing, 66.0, 97.5);
            this.SetSkill(SkillName.Peacemaking, 65.0, 87.5);
            this.SetSkill(SkillName.MagicResist, 25.0, 47.5);
            this.SetSkill(SkillName.Healing, 65.0, 87.5);
            this.SetSkill(SkillName.Tactics, 65.0, 87.5);
            this.SetSkill(SkillName.Fencing, 65.0, 87.5);
            this.SetSkill(SkillName.Parry, 45.0, 60.5);
            this.SetSkill(SkillName.Lockpicking, 65, 87);
            this.SetSkill(SkillName.Hiding, 65, 87);
            this.SetSkill(SkillName.Snooping, 65, 87);	
            this.Fame = 100;
            this.Karma = 0;

            this.AddItem(new Shoes(Utility.RandomNeutralHue()));
            this.AddItem(new Cutlass());
		
            switch ( Utility.Random(2) )
            {
                case 0:
                    this.AddItem(new Doublet(Utility.RandomDyedHue()));
                    break;
                case 1:
                    this.AddItem(new Shirt(Utility.RandomDyedHue()));
                    break;
            }

            this.PackGold(0, 25);
        }

        public HireSailor(Serial serial)
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