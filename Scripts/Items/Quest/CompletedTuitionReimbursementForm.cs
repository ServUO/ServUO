using System;

namespace Server.Items
{
    public class CompletedTuitionReimbursementForm : Item
    {
        [Constructable]
        public CompletedTuitionReimbursementForm()
            : base(0x14F0)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public CompletedTuitionReimbursementForm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074625;
            }
        }// Completed Tuition Reimbursement Form
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