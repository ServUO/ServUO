using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Mielan : MondainQuester
    { 
        [Constructable]
        public Mielan()
            : base("Mielan", "the arcanist")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Mielan(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(CircleOfLifeQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8376;
            this.HairItemID = 0x2FCE;
            this.HairHue = 0x368;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x901));
            this.AddItem(new ElvenPants(0x901));
            this.AddItem(new ElvenShirt());
            this.AddItem(new GemmedCirclet());
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