namespace Server.Items
{
    public class MedusaKey : MasterKey
    {
        public override int Lifespan => 600;
        public override int LabelNumber => 1112303;  // Medusa's Lair

        public MedusaKey()
            : base(0x1012)
        {
            Hue = 0x481;
        }

        public MedusaKey(Serial serial)
            : base(serial)
        {
        }

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

        public override bool CanOfferConfirmation(Mobile from)
        {
            if (from.Region != null && from.Region.IsPartOf("MedusasLair"))
                return base.CanOfferConfirmation(from);

            return false;
        }
    }
}