using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Sledge : MondainQuester
    {
        [Constructable]
        public Sledge()
            : base("Sledge", "The Versatile")
        { 
        }

        public Sledge(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(IngenuityQuest),
                    typeof(PointyEarsQuest)
                };
            }
        }
        public override void InitBody()
        { 
            this.Female = false;
            this.Race = Race.Human;		
		
            base.InitBody();
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());		
            this.AddItem(new ElvenBoots(0x736));
            this.AddItem(new LongPants(0x521));
            this.AddItem(new Tunic(0x71E));
            this.AddItem(new Cloak(0x59));		
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