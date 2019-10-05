using System;

namespace Server.Items
{
    public class CrimsonMaceBelt : MaceBelt
    {
        public override int LabelNumber { get { return 1159211; } } // crimson mace belt

        [Constructable]
        public CrimsonMaceBelt()
            : base()
        {
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 10;
            Attributes.RegenHits = 2;
        }

        public CrimsonMaceBelt(Serial serial)
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
