using System;

namespace Server.Items
{
    public class NoxCrystal : BaseReagent, ICommodity
    {
        [Constructable]
        public NoxCrystal()
            : this(1)
        {
        }

        [Constructable]
        public NoxCrystal(int amount)
            : base(0xF8E, amount)
        {
        }

        public NoxCrystal(Serial serial)
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