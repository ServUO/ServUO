using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Andros : MondainQuester
    {
        [Constructable]
        public Andros()
            : base("Andros", "the blacksmith")
        {
        }

        public Andros(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => null;
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 0x8409;
            HairItemID = 0x2049;
            HairHue = 0x45E;
            FacialHairItemID = 0x2041;
            FacialHairHue = 0x45E;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Boots(0x901));
            AddItem(new LongPants(0x1BB));
            AddItem(new FancyShirt(0x60B));
            AddItem(new FullApron(0x901));
            AddItem(new SmithHammer());
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