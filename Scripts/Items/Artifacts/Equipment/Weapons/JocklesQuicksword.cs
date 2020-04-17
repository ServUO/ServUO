namespace Server.Items
{
    public class JocklesQuicksword : Longsword
    {
        public override bool IsArtifact => true;
        [Constructable]
        public JocklesQuicksword()
        {
            LootType = LootType.Blessed;
            Attributes.AttackChance = 5;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 25;
        }

        public JocklesQuicksword(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1077666;// Jockles' Quicksword
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