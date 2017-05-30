using System;

namespace Server.Items
{
    [Flipable(0x1F03, 0x1F04)]
    public class RobeOfTheEquinox : BaseOuterTorso, ICanBeElfOrHuman
	{
		public override bool IsArtifact { get { return true; } }

        private bool _ElfOnly;
        public override Race RequiredRace { get { return _ElfOnly ? Race.Elf : null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ElfOnly { get { return _ElfOnly; } set { _ElfOnly = value; InvalidateProperties(); } }


        [Constructable]
        public RobeOfTheEquinox()
            : base(0x1F04, 0xD6)
        {
            Weight = 3.0;
            _ElfOnly = true;
            Attributes.Luck = 95;
        }

        public RobeOfTheEquinox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075042;
            }
        }// Robe of the Equinox

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