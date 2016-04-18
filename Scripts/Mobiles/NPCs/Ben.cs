using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Ben : MondainQuester
    {
        [Constructable]
        public Ben()
            : base("Ben", "the apprentice necromancer")
        { 
        }

        public Ben(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(GhostOfCovetousQuest) };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x83FD;
            this.HairItemID = 0x2048;
            this.HairHue = 0x463;
            this.FacialHairItemID = 0x204C;
            this.FacialHairHue = 0x463;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x901));
            this.AddItem(new LongPants(0x1BB));
            this.AddItem(new FancyShirt(0x756));
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