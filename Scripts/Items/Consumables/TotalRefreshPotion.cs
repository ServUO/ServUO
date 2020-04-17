namespace Server.Items
{
    public class TotalRefreshPotion : BaseRefreshPotion
    {
        [Constructable]
        public TotalRefreshPotion()
            : base(PotionEffect.RefreshTotal)
        {
        }

        public TotalRefreshPotion(Serial serial)
            : base(serial)
        {
        }

        public override double Refresh => 1.0;
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