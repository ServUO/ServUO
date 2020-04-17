namespace Server.Items
{
    public class RighteousAnger : ElvenMachete
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RighteousAnger()
        {
            Hue = 0x284;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 5;
            Attributes.WeaponSpeed = 35;
            Attributes.WeaponDamage = 40;
        }

        public RighteousAnger(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075049;// Righteous Anger
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}