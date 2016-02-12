using System;

namespace Server.Items
{
    public class EveningGown : BaseOuterTorso
    {
        [Constructable]
        public EveningGown()
            : this(0)
        {
        }

        [Constructable]
        public EveningGown(int hue)
            : base(0x7821, hue)
        {
            Weight = 3.0;
        }

        public EveningGown(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}