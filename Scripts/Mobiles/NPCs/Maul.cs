using System;

namespace Server.Engines.Quests
{
    public class MaulBear : MondainQuester
    {
        [Constructable]
        public MaulBear()
            : base("Maul")
        {
        }

        public MaulBear(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(SeasonsQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 212;
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