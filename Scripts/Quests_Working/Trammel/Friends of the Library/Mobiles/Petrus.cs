using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Petrus : MondainQuester
    {
        [Constructable]
        public Petrus()
            : base("Petrus", "the bee keeper")
        { 
        }

        public Petrus(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(SomethingToWailAboutQuest),
                    typeof(RunawaysQuest),
                    typeof(ViciousPredatorQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x840C;
            this.HairItemID = 0x203C;
            this.HairHue = 0x3B3;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Sandals(0x1BB));
            this.AddItem(new ShortPants(0x71C));
            this.AddItem(new Tunic(0x5EF));
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