using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Evan : MondainQuester
    {
        [Constructable]
        public Evan()
            : base("Evan", "the beggar")
        { 
        }

        public Evan(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(HonestBeggarQuest) };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x841B;
            this.HairItemID = 0x204A;
            this.HairHue = 0x451;
            this.FacialHairItemID = 0x203F;
            this.FacialHairHue = 0x451;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x737));
            this.AddItem(new ShortPants(0x74C));
            this.AddItem(new FancyShirt(0x535));
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