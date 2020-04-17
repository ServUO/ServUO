using Server.Engines.Craft;

namespace Server.Items
{
    public class FlourSifter : BaseTool
    {
        [Constructable]
        public FlourSifter()
            : base(0x103E)
        {
            Weight = 1.0;
        }

        [Constructable]
        public FlourSifter(int uses)
            : base(uses, 0x103E)
        {
            Weight = 1.0;
        }

        public FlourSifter(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem => DefCooking.CraftSystem;
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