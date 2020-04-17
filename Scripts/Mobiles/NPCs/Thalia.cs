using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Thalia : MondainQuester
    {
        [Constructable]
        public Thalia()
            : base("Thaliae", "the bride")
        {
        }

        public Thalia(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => null;
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;

            Hue = 0x8412;
            HairItemID = 0x2049;
            HairHue = 0x470;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Sandals(0x8FD));
            AddItem(new FancyDress(0x8FD));
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