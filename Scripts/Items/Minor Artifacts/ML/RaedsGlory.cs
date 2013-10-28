using System;

namespace Server.Items
{
    public class RaedsGlory : WarCleaver
    {
        [Constructable]
        public RaedsGlory()
        {
            this.ItemID = 0x2D23;
            this.Hue = 0x1E6;

            this.Attributes.BonusMana = 8;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.WeaponSpeed = 20;

            this.WeaponAttributes.HitLeechHits = 40;
        }

        public RaedsGlory(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075036;
            }
        }// Raed's Glory
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

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}