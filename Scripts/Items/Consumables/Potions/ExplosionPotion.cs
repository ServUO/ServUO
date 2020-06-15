namespace Server.Items
{
    public class ExplosionPotion : BaseExplosionPotion
    {
        [Constructable]
        public ExplosionPotion()
            : base(PotionEffect.Explosion)
        {
        }

        public ExplosionPotion(Serial serial)
            : base(serial)
        {
        }

        public override int MinDamage => 10;
        public override int MaxDamage => 20;
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