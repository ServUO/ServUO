using System;
using Server.Mobiles;

namespace Server.Items
{
    public class GumshoeTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1158638; } } // Gumpshoe

        [Constructable]
        public GumshoeTitleDeed()
        {
        }

        public GumshoeTitleDeed(Serial serial)
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
            int v = reader.ReadInt();
        }
    }
}
