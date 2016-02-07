using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Jamal : MondainQuester
    {
        [Constructable]
        public Jamal()
            : base("Jamal", "the fisherman")
        { 
        }

        public Jamal(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(VilePoisonQuest) };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x83FB;
            this.HairItemID = 0x2049;
            this.HairHue = 0x45E;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new ThighBoots(0x901));
            this.AddItem(new ShortPants(0x730));
            this.AddItem(new Shirt(0x1BB));
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