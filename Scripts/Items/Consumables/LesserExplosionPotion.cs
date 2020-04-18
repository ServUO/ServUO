namespace Server.Items
{
    public class LesserExplosionPotion : BaseExplosionPotion
    {
        [Constructable]
        public LesserExplosionPotion()
            : base(PotionEffect.ExplosionLesser)
        {
        }

        public LesserExplosionPotion(Serial serial)
            : base(serial)
        {
        }

        public override int MinDamage => 5;
        public override int MaxDamage => 10;
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