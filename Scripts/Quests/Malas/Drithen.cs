using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Drithen : MondainQuester
    {
        [Constructable]
        public Drithen()
            : base("Drithen", "the fierce")
        { 
        }

        public Drithen(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TaleOfTailQuest),
                    typeof(PointyEarsQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;			
            this.Hue = 0x840F;		
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());			
            this.AddItem(new ElvenBoots(0x723));
            this.AddItem(new LongPants(0x549));
            this.AddItem(new Tunic(0x72B));
            this.AddItem(new Cloak(0x30));
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