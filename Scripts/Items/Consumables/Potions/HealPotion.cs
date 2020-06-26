namespace Server.Items
{
    public class HealPotion : BaseHealPotion
    {
        [Constructable]
        public HealPotion()
            : base(PotionEffect.Heal)
        {
        }

        public HealPotion(Serial serial)
            : base(serial)
        {
        }

        public override int MinHeal => 13;
        public override int MaxHeal => 16;
        public override double Delay => 8.0;
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
