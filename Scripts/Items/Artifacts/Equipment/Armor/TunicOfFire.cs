namespace Server.Items
{
    public class TunicOfFire : ChainChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public TunicOfFire()
        {
            Hue = 0x54F;
            ArmorAttributes.SelfRepair = 5;
            Attributes.NightSight = 1;
            Attributes.ReflectPhysical = 15;
        }

        public TunicOfFire(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061099;// Tunic of Fire
        public override int ArtifactRarity => 11;
        public override int BasePhysicalResistance => 24;
        public override int BaseFireResistance => 34;
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

            if (version < 1)
            {
                if (Hue == 0x54E)
                    Hue = 0x54F;

                if (Attributes.NightSight == 0)
                    Attributes.NightSight = 1;

                PhysicalBonus = 0;
                FireBonus = 0;
            }
        }
    }
}