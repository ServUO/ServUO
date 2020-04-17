using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Petrus : MondainQuester
    {
        [Constructable]
        public Petrus()
            : base("Petrus", "the bee keeper")
        {
        }

        public Petrus(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(SomethingToWailAboutQuest),
                    typeof(RunawaysQuest),
                    typeof(ViciousPredatorQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 0x840C;
            HairItemID = 0x203C;
            HairHue = 0x3B3;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Sandals(0x1BB));
            AddItem(new ShortPants(0x71C));
            AddItem(new Tunic(0x5EF));
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