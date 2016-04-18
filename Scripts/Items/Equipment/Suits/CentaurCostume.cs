using System;
using Server;

namespace Server.Items
{
    public class CentaurCostume : BaseCostume
    {

        [Constructable]
        public CentaurCostume() : base()
        {
            Name = "a Centaur halloween Costume";
            this.CostumeBody = 101;
        }

        public CentaurCostume(Serial serial)
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

            int version = reader.ReadInt();
        }
    }
}
