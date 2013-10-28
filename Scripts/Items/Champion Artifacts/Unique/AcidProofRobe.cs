using System;

namespace Server.Items
{
    public class AcidProofRobe : Robe
    {
        [Constructable]
        public AcidProofRobe()
        {
            this.Hue = 0x455;
            this.LootType = LootType.Blessed;
        }

        public AcidProofRobe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1095236;
            }
        }// Acid-Proof Robe [Replica]
        public override int BaseFireResistance
        {
            get
            {
                return 4;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
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

            if (version < 1 && this.Hue == 1)
            {
                this.Hue = 0x455;
            }
        }
    }
}