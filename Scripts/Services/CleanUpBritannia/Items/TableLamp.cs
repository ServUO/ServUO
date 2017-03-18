using System;

namespace Server.Items
{
    public class TableLamp : Item
    {
        [Constructable]
        public TableLamp()
            : base(0x49C2)
        {
            this.Weight = 1;
        }

        public TableLamp(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1151220;
            }
        }// table lamp

        public override void OnDoubleClick(Mobile from)
        {
            if (this.ItemID == 0x49C2)
                this.ItemID = 0x49C1;
            else if (this.ItemID == 0x49C1)
                this.ItemID = 0x49C2;
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