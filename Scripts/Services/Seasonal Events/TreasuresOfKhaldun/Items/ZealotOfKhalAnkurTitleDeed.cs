using System;
using Server.Mobiles;

namespace Server.Items
{
    public class ZealotOfKhalAnkurTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1158684; } } // Zealot of Khal Ankur

        [Constructable]
        public ZealotOfKhalAnkurTitleDeed()
        {
        }

        public ZealotOfKhalAnkurTitleDeed(Serial serial)
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
