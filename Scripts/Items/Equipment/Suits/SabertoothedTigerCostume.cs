using System;
using Server;

namespace Server.Items
{
    public class SabertoothedTigerCostume : BaseCostume
    {
        public override string CreatureName { get { return "saber-toothed tiger"; } }

        [Constructable]
        public SabertoothedTigerCostume() : base()
        {
            CostumeBody = 0x588;
        }

        public override string DefaultName
        {
            get
            {
                return "a saber-toothed tiger costume";
            }
        }

        public SabertoothedTigerCostume(Serial serial) : base(serial)
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
