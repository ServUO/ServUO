using System;

namespace Server.Items
{
    public class OrnamentOfTheMagician : GoldBracelet
    {
        [Constructable]
        public OrnamentOfTheMagician()
        {
            this.Hue = 0x554;
            this.Attributes.CastRecovery = 3;
            this.Attributes.CastSpeed = 2;
            this.Attributes.LowerManaCost = 10;
            this.Attributes.LowerRegCost = 20;
            this.Resistances.Energy = 15;
        }

        public OrnamentOfTheMagician(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061105;
            }
        }// Ornament of the Magician
        public override int ArtifactRarity
        {
            get
            {
                return 11;
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

            if (this.Hue == 0x12B)
                this.Hue = 0x554;
        }
    }
}