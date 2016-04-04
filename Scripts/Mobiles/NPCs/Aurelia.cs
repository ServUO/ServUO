using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Aurelia : MondainQuester
    {
        [Constructable]
        public Aurelia()
            : base("Aurelia", "the architect's daughter")
        { 
        }

        public Aurelia(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(AemaethOneQuest) };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Human;
			
            this.Hue = 0x83F7;
            this.HairItemID = 0x2047;
            this.HairHue = 0x457;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Sandals(0x4B7));
            this.AddItem(new Skirt(0x4B4));
            this.AddItem(new FancyShirt(0x659));
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