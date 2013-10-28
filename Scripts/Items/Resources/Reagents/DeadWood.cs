using System;

namespace Server.Items
{
    public class DeadWood : BaseReagent, ICommodity
    {
        [Constructable]
        public DeadWood()
            : this(1)
        {
        }

        [Constructable]
        public DeadWood(int amount)
            : base(0xF90, amount)
        {
        }

        public DeadWood(Serial serial)
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