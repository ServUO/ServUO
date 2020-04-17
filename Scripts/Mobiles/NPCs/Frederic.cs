using System;

namespace Server.Engines.Quests
{
    public class Frederic : MondainQuester
    {
        [Constructable]
        public Frederic()
            : base("The Ghost of Frederic Smithson")
        {
        }

        public Frederic(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => null;
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Body = 0x1A;
            Hue = 0x455;
            CantWalk = true;
            Frozen = true;
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