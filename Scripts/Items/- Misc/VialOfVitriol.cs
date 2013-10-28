using System;

namespace Server.Items
{
    public class VialOfVitriol : Item
    {
        [Constructable]
        public VialOfVitriol()
            : base(0x5722)
        {
        }

        public VialOfVitriol(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113331;
            }
        }// vial of vitriol
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

    public class VialVitirol : Item
    {
        [Constructable]
        public VialVitirol()
            : this(1)
        {
        }

        [Constructable]
        public VialVitirol(int amount)
            : base(0x5722)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public VialVitirol(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113331;
            }
        }// vial vitirol
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