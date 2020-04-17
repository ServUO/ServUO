namespace Server.Items
{
    public class KingsGildedStatue : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override bool ShowArtifactRarity => false;
        public override int ArtifactRarity => 8;

        public override double DefaultWeight => 10.0;
        public override string DefaultName => "A Gilded Statue from the Personal Collection of the King";

        [Constructable]
        public KingsGildedStatue()
            : base(0x139D)
        {
            Hue = 2721;
        }

        public KingsGildedStatue(Serial serial)
            : base(serial)
        {
        }

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
