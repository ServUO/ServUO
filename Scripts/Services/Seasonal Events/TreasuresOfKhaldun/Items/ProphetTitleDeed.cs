using System;
using Server.Mobiles;

namespace Server.Items
{
    public class ProphetTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1158683; } } // Prophet

        [Constructable]
        public ProphetTitleDeed()
        {
        }

        public ProphetTitleDeed(Serial serial)
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
