using System;
using Server;

namespace Server.Items
{
    public class DragonWolfCostume : BaseCostume
    {
        public override string CreatureName { get { return "dragon wolf"; } }

        [Constructable]
        public DragonWolfCostume() : base()
        {
            CostumeBody = 719;
        }

        public override string DefaultName
        {
            get
            {
                return "a dragon wolf costume";
            }
        }

        public DragonWolfCostume(Serial serial) : base(serial)
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
