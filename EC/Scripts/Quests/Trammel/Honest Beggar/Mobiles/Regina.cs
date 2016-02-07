using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Regina : MondainQuester
    {
        [Constructable]
        public Regina()
            : base("Regina", "the noble")
        { 
        }

        public Regina(Serial serial)
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
			
            this.Female = true;
            this.Race = Race.Human;
			
            this.Hue = 0x83EE;
            this.HairItemID = 0x2049;
            this.HairHue = 0x599;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Boots());
            this.AddItem(new GildedDress());
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