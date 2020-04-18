using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Regina : MondainQuester
    {
        [Constructable]
        public Regina()
            : base("Regina", "the noble")
        {
        }

        public Regina(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => null;
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;

            Hue = 0x83EE;
            HairItemID = 0x2049;
            HairHue = 0x599;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Boots());
            AddItem(new GildedDress());
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