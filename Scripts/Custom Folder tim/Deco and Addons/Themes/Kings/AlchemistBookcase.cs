using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x4C24, 0x4C25)]
    public class AlchemistBookcase : BaseContainer
    {
        [Constructable]
        public AlchemistBookcase()
            : base(0x4C24)
        {
        }

        public AlchemistBookcase(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 1.0)
                this.Weight = -1;
        }
    }
}
