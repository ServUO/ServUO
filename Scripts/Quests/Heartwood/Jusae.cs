using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Jusae : MondainQuester
    { 
        [Constructable]
        public Jusae()
            : base("Jusae", "the bowcrafter")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Jusae(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(LethalDartsQuest),
                    typeof(SimpleBowQuest),
                    typeof(IngeniousArcheryPartOneQuest),
                    typeof(IngeniousArcheryPartTwoQuest),
                    typeof(IngeniousArcheryPartThreeQuest),
                    typeof(StopHarpingOnMeQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x83E5;
            this.HairItemID = 0x2FD0;
            this.HairHue = 0x238;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x901));
            this.AddItem(new ShortPants(0x651));
            this.AddItem(new MagicalShortbow());
			
            Item item;
			
            item = new HideChest();
            item.Hue = 0x27B;
            this.AddItem(item);
			
            item = new HidePauldrons();
            item.Hue = 0x27E;
            this.AddItem(item);
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