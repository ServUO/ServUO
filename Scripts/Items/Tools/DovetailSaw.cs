using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x1028, 0x1029)]
    public class DovetailSaw : BaseTool
    {
        [Constructable]
        public DovetailSaw()
            : base(0x1028)
        {
            Weight = 2.0;
        }

        [Constructable]
        public DovetailSaw(int uses)
            : base(uses, 0x1028)
        {
            Weight = 2.0;
        }

        public DovetailSaw(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem => DefCarpentry.CraftSystem;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Weight == 1.0)
                Weight = 2.0;
        }
    }
}