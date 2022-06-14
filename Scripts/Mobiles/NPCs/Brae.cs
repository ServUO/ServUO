using Server.Items;
using System;

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

        public override Type[] Quests => new Type[]
                {
                    typeof(AllThatGlittersIsNotGoodQuest),
                    typeof(FiendishFriendsQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x80BF;
            HairItemID = 0x2FC2;
            HairHue = 0x8E;
        }

        public override void InitOutfit()
        {
            SetWearable(new ElvenBoots(), 0x901, 1);
            SetWearable(new GemmedCirclet(), dropChance: 1);
            SetWearable(new FemaleElvenRobe(), 0x44, 1);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}