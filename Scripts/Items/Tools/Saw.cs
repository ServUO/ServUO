using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x1034, 0x1035)]
    public class Saw : BaseTool
    {
        [Constructable]
        public Saw()
            : base(0x1034)
        {
            Weight = 2.0;
        }

        [Constructable]
        public Saw(int uses)
            : base(uses, 0x1034)
        {
            Weight = 2.0;
        }

        public Saw(Serial serial)
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