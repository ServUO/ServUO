using System;

namespace Server.Engines.Quests
{
    public class Strongroot : MondainQuester
    {
        [Constructable]
        public Strongroot()
            : base("Strongroot")
        {
        }

        public Strongroot(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(CaretakerOfTheLandQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 301;
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