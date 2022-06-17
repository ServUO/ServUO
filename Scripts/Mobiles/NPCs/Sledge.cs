using Server.Items;
using System;

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

        public override Type[] Quests => new Type[]
                {
                    typeof(IngenuityQuest),
                    typeof(PointyEarsQuest)
                };
        public override void InitBody()
        {
            Female = false;
            Race = Race.Human;

            base.InitBody();
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack());
            SetWearable(new ElvenBoots(), 0x736, 1);
            SetWearable(new LongPants(), 0x521, 1);
            SetWearable(new Tunic(), 0x71E, 1);
			SetWearable(new Cloak(), 0x59, 1);
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