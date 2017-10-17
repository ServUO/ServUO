using System;
using Server;

namespace Server.Items
{
    public class CentaurCostume : BaseCostume
    {

        [Constructable]
        public CentaurCostume() : base()
        {
            this.CostumeBody = 101;
        }
		
		public override int LabelNumber
        {
            get
            {
                return 1114235;
            }
        }// centaur costume

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
