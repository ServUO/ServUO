using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Belulah : MondainQuester
    {
        [Constructable]
        public Belulah()
            : base("Belulah", "The Scorned")
        { 
        }

        public Belulah(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(AllSeasonAdventurerQuest)
                };
            }
        }
        public override void InitBody()
        { 
            this.Female = true;
            this.Race = Race.Human;		
			
            this.Hue = 0x83F7;
            this.HairItemID = 0x2046;
            this.HairHue = 0x463;
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());		
            this.AddItem(new Boots());
            this.AddItem(new LongPants(0x6C7));
            this.AddItem(new FancyShirt(0x6BB));
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