namespace Server.Items
{
    public class PrimordialDecay : BaseInstrument
    {
        [Constructable]
        public PrimordialDecay()
        {
            Hue = 1927;
            Weight = 4;
            Slayer = SlayerName.ElementalBan;

            UsesRemaining = 450;
        }

        public PrimordialDecay(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1154723;// Primordial Decay
        public override int InitMinUses => 450;
        public override int InitMaxUses => 450;
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