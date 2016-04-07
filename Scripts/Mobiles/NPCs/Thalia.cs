using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Thalia : MondainQuester
    {
        [Constructable]
        public Thalia()
            : base("Thaliae", "the bride")
        { 
        }

        public Thalia(Serial serial)
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
			
            this.Hue = 0x8412;
            this.HairItemID = 0x2049;
            this.HairHue = 0x470;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Sandals(0x8FD));
            this.AddItem(new FancyDress(0x8FD));
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