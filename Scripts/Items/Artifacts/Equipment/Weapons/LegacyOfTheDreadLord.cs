using System;

namespace Server.Items
{
    public class LegacyOfTheDreadLord : Bardiche
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LegacyOfTheDreadLord()
        {
            this.Hue = 0x676;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastRecovery = 3;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 50;
        }

        public LegacyOfTheDreadLord(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1060860;
            }
        }// Legacy of the Dread Lord
        public override int ArtifactRarity
        {
            get
            {
                return 10;
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

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Attributes.CastSpeed == 3)
                this.Attributes.CastRecovery = 3;

            if (this.Hue == 0x4B9)
                this.Hue = 0x676;
        }
    }
}