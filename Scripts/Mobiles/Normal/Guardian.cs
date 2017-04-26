using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class Guardian : BaseCreature 
    { 
        [Constructable] 
        public Guardian()
            : base(AIType.AI_Archer, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        { 
            this.InitStats(100, 125, 25); 
            this.Title = "the guardian"; 

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
            }

            new ForestOstard().Rider = this; 

            PlateChest chest = new PlateChest(); 
            chest.Hue = 0x966; 
            this.AddItem(chest); 
            PlateArms arms = new PlateArms(); 
            arms.Hue = 0x966; 
            this.AddItem(arms); 
            PlateGloves gloves = new PlateGloves(); 
            gloves.Hue = 0x966; 
            this.AddItem(gloves); 
            PlateGorget gorget = new PlateGorget(); 
            gorget.Hue = 0x966; 
            this.AddItem(gorget); 
            PlateLegs legs = new PlateLegs(); 
            legs.Hue = 0x966; 
            this.AddItem(legs); 
            PlateHelm helm = new PlateHelm(); 
            helm.Hue = 0x966; 
            this.AddItem(helm); 

            Bow bow = new Bow(); 

            bow.Movable = false; 
            bow.Crafter = this; 
            bow.Quality = ItemQuality.Exceptional; 

            this.AddItem(bow); 

            this.PackItem(new Arrow(250));
            this.PackGold(250, 500);

            this.Skills[SkillName.Anatomy].Base = 120.0; 
            this.Skills[SkillName.Tactics].Base = 120.0; 
            this.Skills[SkillName.Archery].Base = 120.0; 
            this.Skills[SkillName.MagicResist].Base = 120.0; 
            this.Skills[SkillName.DetectHidden].Base = 100.0; 
        }

        public Guardian(Serial serial)
            : base(serial)
        { 
        }

        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }
}