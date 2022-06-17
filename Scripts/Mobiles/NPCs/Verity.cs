using Server.Items;
using System;

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

        public override Type[] Quests => new Type[]
                {
                    typeof(FriendsOfTheLibraryQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;

            Hue = 0x83EF;
            HairItemID = 0x2047;
            HairHue = 0x3B3;
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack());
            SetWearable(new Shoes(), 0x754, 1);
            SetWearable(new Shirt(), 0x653, 1);
            SetWearable(new Cap(), 0x901, 1);
			SetWearable(new Kilt(), 0x901, 1);
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