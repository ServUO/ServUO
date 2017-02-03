using System;

namespace Server.Items
{
    public class CandlewoodTorch : BaseShield
    {
		public override bool IsArtifact { get { return true; } }
        public bool Burning { get { return ItemID == 0xA12; } }

        [Constructable]
        public CandlewoodTorch()
            : base(0xF6B)
        { 
            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastSpeed = -1;
        }

        public CandlewoodTorch(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094957;
            }
        }//Candlewood Torch
        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                if (this.ItemID == 0xF6B)
                {
                    this.ItemID = 0xA12;
                }
                else if (this.ItemID == 0xA12)
                {
                    this.ItemID = 0xF6B;
                }
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