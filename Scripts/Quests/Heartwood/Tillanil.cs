using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Tillanil : MondainQuester
    { 
        [Constructable]
        public Tillanil()
            : base("Tillanil", "the wine tender")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Tillanil(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TheSongOfTheWindQuest),
                    typeof(BeerGogglesQuest),
                    typeof(MessageInBottleQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x8383;
            this.HairItemID = 0x2FD0;
            this.HairHue = 0x127;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x1BB));
            this.AddItem(new Tunic(0x712));
            this.AddItem(new ShortPants(0x30));
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