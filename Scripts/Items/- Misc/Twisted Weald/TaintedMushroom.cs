using System;

namespace Server.Items
{
    public class TaintedMushroom : Item
    {
        [Constructable]
        public TaintedMushroom()
            : base(Utility.RandomMinMax(0x222E, 0x2231))
        {
        }

        public TaintedMushroom(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075088;
            }
        }// Dread Horn Tainted Mushroom
        public override bool ForceShowProperties
        {
            get
            {
                return true;
            }
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