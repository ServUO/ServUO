using System;

namespace Server.Items
{
    public class FeyLeggings : ChainLegs, ICanBeElfOrHuman
	{
		public override bool IsArtifact { get { return true; } }

        private bool _ElfOnly;
        public override Race RequiredRace { get { return _ElfOnly ? Race.Elf : null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ElfOnly { get { return _ElfOnly; } set { _ElfOnly = value; InvalidateProperties(); } }

        [Constructable]
        public FeyLeggings()
        {
            this.Attributes.BonusHits = 6;
            this.Attributes.DefendChance = 20;

            _ElfOnly = true;

            this.ArmorAttributes.MageArmor = 1;
        }

        public FeyLeggings(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075041;
            }
        }// Fey Leggings
        public override int BasePhysicalResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 4;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 19;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }

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