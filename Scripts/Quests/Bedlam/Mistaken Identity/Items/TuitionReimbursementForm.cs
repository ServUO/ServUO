using System;

namespace Server.Items
{
    public class TuitionReimbursementForm : Item
    {
        [Constructable]
        public TuitionReimbursementForm()
            : base(0xE3A)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1;
            this.Hue = 0x395;
        }

        public TuitionReimbursementForm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074610;
            }
        }// Tuition Reimbursement Form (in triplicate)
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