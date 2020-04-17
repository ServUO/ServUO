namespace Server.Items
{
    public class LesserHealPotion : BaseHealPotion
    {
        [Constructable]
        public LesserHealPotion()
            : base(PotionEffect.HealLesser)
        {
        }

        public LesserHealPotion(Serial serial)
            : base(serial)
        {
        }

        public override int MinHeal => 6;
        public override int MaxHeal => 8;
        public override double Delay => 3.0;
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
