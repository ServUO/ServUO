using System;

namespace Server.Items
{
    public class SculptureWoodEven : BaseLight
    {
        public override int LabelNumber { get { return 1029241; } } // sculpture

        public override int LitItemID { get { return 0xA49A; } }
        public override int UnlitItemID { get { return 0xA499; } }

        public override int LitSound { get { return 480; } }
        public override int UnlitSound { get { return 482; } }

        [Constructable]
        public SculptureWoodEven()
            : base(0xA499)
        {
            Weight = 1;
        }

        public SculptureWoodEven(Serial serial)
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
