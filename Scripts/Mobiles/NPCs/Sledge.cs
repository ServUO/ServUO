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
            AddItem(new Backpack());
            AddItem(new ElvenBoots(0x736));
            AddItem(new LongPants(0x521));
            AddItem(new Tunic(0x71E));
            AddItem(new Cloak(0x59));
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