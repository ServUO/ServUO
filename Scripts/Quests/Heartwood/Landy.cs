using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Landy : MondainQuester
    {
        [Constructable]
        public Landy()
            : base("Landy", "the soil nurturer")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Landy(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(CreepyCrawliesQuest),
                    typeof(MongbatMenaceQuest),
                    typeof(SpecimensQuest),
                    typeof(ThinningTheHerdQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8384;
            this.HairItemID = 0x2FCE;
            this.HairHue = 0x91;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x901));
            this.AddItem(new Tunic(0x719));
            this.AddItem(new ShortPants(0x1BB));
			
            Item item;
			
            item = new LeafGloves();
            item.Hue = 0x1BB;
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