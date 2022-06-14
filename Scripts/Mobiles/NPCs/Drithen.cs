using Server.Items;
using System;

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

        public override Type[] Quests => new Type[]
                {
                    typeof(TaleOfTailQuest),
                    typeof(PointyEarsQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;
            Hue = 0x840F;
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack());
            SetWearable(new ElvenBoots(), 0x723, 1);
            SetWearable(new LongPants(), 0x549, 1);
            SetWearable(new Tunic(), 0x72B, 1);
            SetWearable(new Cloak(), 0x30, 1);
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