using System;

namespace Server.Items
{
    [FlipableAttribute(0x11EA, 0x11EB)]
    public class Sand : Item, ICommodity
    {
        [Constructable]
        public Sand()
            : this(1)
        {
        }

        [Constructable]
        public Sand(int amount)
            : base(0x11EA)
        {
            this.Stackable = Core.ML;
            this.Weight = 1.0;
        }

        public Sand(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1044626;
            }
        }// sand
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

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 2) // Reset from Resource System
                reader.ReadString();

            if (version == 0 && this.Name == "sand")
                this.Name = null;
        }
    }
}