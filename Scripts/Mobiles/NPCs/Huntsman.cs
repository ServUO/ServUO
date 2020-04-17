using System;

namespace Server.Engines.Quests
{
    public class Huntsman : MondainQuester
    {
        [Constructable]
        public Huntsman()
            : base("Huntsman")
        {
        }

        public Huntsman(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(TheBalanceOfNatureQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 101;
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