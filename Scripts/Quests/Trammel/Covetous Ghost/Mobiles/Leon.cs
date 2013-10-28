using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Leon : MondainQuester
    {
        [Constructable]
        public Leon()
            : base("Leon", "the alchemist")
        { 
        }

        public Leon(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return null;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x83EB;
            this.HairItemID = 0x203C;
            this.HairHue = 0x454;
            this.FacialHairItemID = 0x204C;
            this.FacialHairHue = 0x454;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x901));
            this.AddItem(new Robe(0x657));
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