using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Olla : MondainQuester
    {
        [Constructable]
        public Olla()
            : base("Olla", "the metal weaver")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Olla(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(CutsBothWaysQuest),
                    typeof(DragonProtectionQuest),
                    typeof(NothingFancyQuest),
                    typeof(TheBulwarkQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x824E;
            this.HairItemID = 0x2FCE;
            this.HairHue = 0x8F;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots());
            this.AddItem(new LongPants(0x3B3));
            this.AddItem(new ElvenShirt());
            this.AddItem(new SmithHammer());
            this.AddItem(new FullApron(0x1BB));
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