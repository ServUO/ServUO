using System;

namespace Server.Items
{
    public class BlackPearl : BaseReagent, ICommodity
    {
        [Constructable]
        public BlackPearl()
            : this(1)
        {
        }

        [Constructable]
        public BlackPearl(int amount)
            : base(0xF7A, amount)
        {
        }

        public BlackPearl(Serial serial)
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