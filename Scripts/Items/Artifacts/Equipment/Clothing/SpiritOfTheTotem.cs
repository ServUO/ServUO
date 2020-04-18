namespace Server.Items
{
    public class SpiritOfTheTotem : BearMask
    {
        public override bool IsArtifact => true;
        [Constructable]
        public SpiritOfTheTotem()
        {
            Hue = 0x455;
            Attributes.BonusStr = 20;
            Attributes.ReflectPhysical = 15;
            Attributes.AttackChance = 15;
        }

        public SpiritOfTheTotem(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061599;// Spirit of the Totem
        public override int ArtifactRarity => 11;
        public override int BasePhysicalResistance => 20;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        Resistances.Physical = 0;
                        break;
                    }
            }
        }
    }
}