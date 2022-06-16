using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Aurelia : MondainQuester
    {
        [Constructable]
        public Aurelia()
            : base("Aurelia", "the architect's daughter")
        {
        }

        public Aurelia(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[] { typeof(AemaethOneQuest) };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;

            Hue = 0x83F7;
            HairItemID = 0x2047;
            HairHue = 0x457;
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack(), dropChance: 1);
            SetWearable(new Sandals(), 0x4B7, 1);
            SetWearable(new Skirt(), 0x4B4, 1);
            SetWearable(new FancyShirt(), 0x659, 1);
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