namespace Server.Items
{
    public class FeyLeggings : ChainLegs, ICanBeElfOrHuman
    {
        public override bool IsArtifact => true;

        private bool _ElfOnly;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ElfOnly { get { return _ElfOnly; } set { _ElfOnly = value; InvalidateProperties(); } }

        [Constructable]
        public FeyLeggings()
        {
            Attributes.BonusHits = 6;
            Attributes.DefendChance = 20;

            _ElfOnly = true;

            ArmorAttributes.MageArmor = 1;
        }

        public FeyLeggings(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075041;// Fey Leggings
        public override int BasePhysicalResistance => 12;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 19;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            writer.Write(_ElfOnly);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    _ElfOnly = reader.ReadBool();
                    break;
                case 0:
                    _ElfOnly = true;
                    break;
            }
        }
    }
}
