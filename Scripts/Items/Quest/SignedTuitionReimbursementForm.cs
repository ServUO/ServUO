namespace Server.Items
{
    public class SignedTuitionReimbursementForm : Item
    {
        [Constructable]
        public SignedTuitionReimbursementForm()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public SignedTuitionReimbursementForm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074614;// Signed Tuition Reimbursement Form
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}