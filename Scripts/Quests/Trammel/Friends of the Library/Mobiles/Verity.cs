using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Verity : MondainQuester
    {
        [Constructable]
        public Verity()
            : base("Verity", "the librarian")
        { 
        }

        public Verity(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(FriendsOfTheLibraryQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Human;
			
            this.Hue = 0x83EF;
            this.HairItemID = 0x2047;
            this.HairHue = 0x3B3;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x754));
            this.AddItem(new Shirt(0x653));
            this.AddItem(new Cap(0x901));
            this.AddItem(new Kilt(0x901));
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