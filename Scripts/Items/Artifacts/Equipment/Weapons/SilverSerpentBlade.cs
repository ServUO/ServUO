namespace Server.Items
{
    public class SilverSerpentBlade : Kryss
    {
        public override bool IsArtifact => true;
        [Constructable]
        public SilverSerpentBlade()
        {
            LootType = LootType.Blessed;
            Attributes.AttackChance = 5;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 25;
        }

        public SilverSerpentBlade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1078163;// Silver Serpent Blade
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