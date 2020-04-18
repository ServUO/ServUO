namespace Server.Items
{
    public class PoisonPotion : BasePoisonPotion
    {
        [Constructable]
        public PoisonPotion()
            : base(PotionEffect.Poison)
        {
        }

        public PoisonPotion(Serial serial)
            : base(serial)
        {
        }

        public override Poison Poison => Poison.Regular;
        public override double MinPoisoningSkill => 30.0;
        public override double MaxPoisoningSkill => 70.0;
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