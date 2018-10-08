using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.VialVitirol")]
    public class VialOfVitriol : Item, ICommodity
    {
        [Constructable]
        public VialOfVitriol()
            : this(1)
        {
        }

        [Constructable]
        public VialOfVitriol(int amount)
            : base(0x5722)
        {
            Stackable = true;
            Amount = amount;
        }

        public VialOfVitriol(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

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
}
