using System;

namespace Server.Items
{
    [FlipableAttribute(0x4C62, 0x4C63)]
    public class BlackthornPainting1 : Item
    {
        public override int LabelNumber { get { return 1023744; } } // painting
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BlackthornPainting1()
            : base(0x4C62)
        {
        }

        public BlackthornPainting1(Serial serial)
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

    [FlipableAttribute(0x4C64, 0x4C65)]
    public class BlackthornPainting2 : Item
    {
        public override int LabelNumber { get { return 1023744; } } // painting
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BlackthornPainting2()
            : base(0x4C64)
        {
        }

        public BlackthornPainting2(Serial serial)
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