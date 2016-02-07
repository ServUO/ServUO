using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Lissbet : BaseEscort
    {
        [Constructable]
        public Lissbet()
            : base()
        { 
            this.Name = "Lissbet";
            this.Title = "The Flower Girl";
        }

        public Lissbet(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(ResponsibilityQuest)
                };
            }
        }
        public override void InitBody()
        { 
            this.Female = false;
            this.Race = Race.Human;		
			
            this.Hue = 0x8411;
            this.HairItemID = 0x203D;
            this.HairHue = 0x1BB;
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());		
            this.AddItem(new Sandals());
            this.AddItem(new FancyShirt(0x6BF));
            this.AddItem(new Kilt(0x6AA));	
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