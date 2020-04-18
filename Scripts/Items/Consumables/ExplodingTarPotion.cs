namespace Server.Items
{
    public class ExplodingTarPotion : BaseExplodingTarPotion
    {
        public override int Radius => 20;

        public override int LabelNumber => 1095147;  // a Greater Confusion Blast potion

        [Constructable]
        public ExplodingTarPotion() : base(PotionEffect.ExplodingTarPotion)
        {
        }

        public ExplodingTarPotion(Serial serial) : base(serial)
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
    }
}
