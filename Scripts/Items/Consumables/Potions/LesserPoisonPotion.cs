namespace Server.Items
{
    public class LesserPoisonPotion : BasePoisonPotion
    {
        [Constructable]
        public LesserPoisonPotion()
            : base(PotionEffect.PoisonLesser)
        {
        }

        public LesserPoisonPotion(Serial serial)
            : base(serial)
        {
        }

        public override Poison Poison => Poison.Lesser;
        public override double MinPoisoningSkill => 0.0;
        public override double MaxPoisoningSkill => 60.0;
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