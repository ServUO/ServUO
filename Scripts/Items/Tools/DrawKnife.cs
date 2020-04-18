using Server.Engines.Craft;

namespace Server.Items
{
    public class DrawKnife : BaseTool
    {
        [Constructable]
        public DrawKnife()
            : base(0x10E4)
        {
            Weight = 1.0;
        }

        [Constructable]
        public DrawKnife(int uses)
            : base(uses, 0x10E4)
        {
            Weight = 1.0;
        }

        public DrawKnife(Serial serial)
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
        }
    }
}