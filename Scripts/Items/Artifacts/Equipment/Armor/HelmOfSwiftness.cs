using System;

namespace Server.Items
{
    public class HelmOfSwiftness : WingedHelm, ICanBeElfOrHuman
	{
		public override bool IsArtifact { get { return true; } }

        private bool _ElfOnly;
        public override Race RequiredRace { get { return _ElfOnly ? Race.Elf : null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ElfOnly { get { return _ElfOnly; } set { _ElfOnly = value; InvalidateProperties(); } }

        [Constructable]
        public HelmOfSwiftness()
            : base()
        {
            Hue = 0x592;
            ItemID = 0x2689;

            _ElfOnly = true;

            Attributes.BonusInt = 5;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 2;
            ArmorAttributes.MageArmor = 1;
        }

        public HelmOfSwiftness(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075037;
            }
        }// Helm of Swiftness
        public override int BasePhysicalResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 8;
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

            if (ItemID != 0x2689)
            {
                ItemID = 0x2689;
            }
        }
    }
}