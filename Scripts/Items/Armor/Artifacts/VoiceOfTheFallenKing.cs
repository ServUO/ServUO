using System;

namespace Server.Items
{
    public class VoiceOfTheFallenKing : LeatherGorget
    {
        [Constructable]
        public VoiceOfTheFallenKing()
        {
            this.Hue = 0x76D;
            this.Attributes.BonusStr = 8;
            this.Attributes.RegenHits = 5;
            this.Attributes.RegenStam = 3;
        }

        public VoiceOfTheFallenKing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061094;
            }
        }// Voice of the Fallen King
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 18;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 18;
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

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                if (this.Hue == 0x551)
                    this.Hue = 0x76D;

                this.ColdBonus = 0;
                this.EnergyBonus = 0;
            }
        }
    }
}