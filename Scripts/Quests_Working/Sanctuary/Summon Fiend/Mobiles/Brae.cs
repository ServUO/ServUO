using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Brae : MondainQuester
    { 
        [Constructable]
        public Brae()
            : base("Elder Brae", "the wise")
        { 
        }

        public Brae(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(AllThatGlittersIsNotGoodQuest),
                    typeof(FiendishFriendsQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x80BF;
            this.HairItemID = 0x2FC2;
            this.HairHue = 0x8E;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x901));
            this.AddItem(new GemmedCirclet());
            this.AddItem(new FemaleElvenRobe(0x44));
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