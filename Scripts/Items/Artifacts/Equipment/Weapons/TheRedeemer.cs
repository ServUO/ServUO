namespace Server.Items
{
    public class TheRedeemer : PaladinSword
    {
        public override int LabelNumber => 1077442;  // The Redeemer
        public override bool IsArtifact => true;

        public override int ArtifactRarity => 7;

        public override int InitMinHits => 100;
        public override int InitMaxHits => 100;

        [Constructable]
        public TheRedeemer()
        {
            Hue = 2304;
            Slayer = SlayerName.Silver;
            Slayer2 = SlayerName.Exorcism;
            Attributes.WeaponDamage = 55;
        }

        public TheRedeemer(Serial serial)
            : base(serial)
        {
        }

        public override bool CanFortify => false;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
