using System;

namespace Server.Items
{
    public class SpidersSilk : BaseReagent, ICommodity
    {
        [Constructable]
        public SpidersSilk()
            : this(1)
        {
        }

        [Constructable]
        public SpidersSilk(int amount)
            : base(0xF8D, amount)
        {
        }

        public SpidersSilk(Serial serial)
            : base(serial)
        {
        }

        int ICommodity.DescriptionNumber
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
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