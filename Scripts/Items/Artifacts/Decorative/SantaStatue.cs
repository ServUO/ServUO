using System;

namespace Server.Items
{
    [FlipableAttribute(0x4A9A, 0x4A9B)]
    public class SantaStatue : MonsterStatuette
    {
        public override int LabelNumber { get { return 1097968; } } // santa statue

        [Constructable]
        public SantaStatue()
            : base(MonsterStatuetteType.Santa)
        {
            Weight = 10.0;
        }

        public SantaStatue(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
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
