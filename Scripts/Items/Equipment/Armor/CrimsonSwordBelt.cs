using System;

namespace Server.Items
{
    public class CrimsonSwordBelt : SwordBelt
    {
        public override int LabelNumber { get { return 1159212; } } // crimson sword belt
        
        [Constructable]
        public CrimsonSwordBelt()
            : base()
        {
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 10;
            Attributes.RegenHits = 2;
        }

        public CrimsonSwordBelt(Serial serial)
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
