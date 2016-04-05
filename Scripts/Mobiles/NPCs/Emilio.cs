using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Emilio : MondainQuester
    {
        [Constructable]
        public Emilio()
            : base("Emilio", "the tortured artist")
        { 
        }

        public Emilio(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(UnfadingMemoriesOneQuest) };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x83EB;
            this.HairItemID = 0x2048;
            this.HairHue = 0x470;
            this.FacialHairItemID = 0x204C;
            this.FacialHairHue = 0x470;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Sandals(0x721));
            this.AddItem(new LongPants(0x51B));
            this.AddItem(new FancyShirt(0x517));
            this.AddItem(new FloppyHat(0x584));
            this.AddItem(new BodySash(0x13));
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