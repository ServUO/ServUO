using System;

namespace Server.Items
{
    public class DirtySnowballs : SnowPile
    {
        public override int LabelNumber { get { return 1158833; } } // dirty snowballs

        [Constructable]
        public DirtySnowballs()
        {
            ItemID = 0xE74;
            Hue = 1301;
        }

        public DirtySnowballs(Serial serial)
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
