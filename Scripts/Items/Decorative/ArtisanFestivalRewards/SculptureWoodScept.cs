using System;

namespace Server.Items
{
    public class SculptureWoodScept : BaseLight
    {
        public override int LabelNumber { get { return 1029241; } } // sculpture

        public override int LitItemID { get { return 0xA4A9; } }
        public override int UnlitItemID { get { return 0xA4A8; } }

        public override int LitSound { get { return 480; } }
        public override int UnlitSound { get { return 482; } }

        [Constructable]
        public SculptureWoodScept()
            : base(0xA4A8)
        {
            Weight = 1;
        }

        public SculptureWoodScept(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
