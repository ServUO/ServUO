namespace Server.Items
{
    public class JadeWarAxe : WarAxe
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1115445;  // Jade War Axe

        [Constructable]
        public JadeWarAxe()
        {
            Hue = 1162;
            AbsorptionAttributes.EaterFire = 10;
            Slayer = SlayerName.ReptilianDeath;
            WeaponAttributes.HitFireball = 30;
            WeaponAttributes.HitLowerDefend = 60;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 50;
        }

        public JadeWarAxe(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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