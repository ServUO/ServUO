using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Andros : MondainQuester
    {
        [Constructable]
        public Andros()
            : base("Andros", "the blacksmith")
        { 
        }

        public Andros(Serial serial)
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
			
            this.Hue = 0x8409;
            this.HairItemID = 0x2049;
            this.HairHue = 0x45E;
            this.FacialHairItemID = 0x2041;
            this.FacialHairHue = 0x45E;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Boots(0x901));
            this.AddItem(new LongPants(0x1BB));
            this.AddItem(new FancyShirt(0x60B));
            this.AddItem(new FullApron(0x901));
            this.AddItem(new SmithHammer());
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