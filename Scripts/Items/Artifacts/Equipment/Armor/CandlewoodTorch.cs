using System;

namespace Server.Items
{
    public class CandlewoodTorch : BaseShield
    {
        public override int LabelNumber { get { return 1094957; } } //Candlewood Torch
        public override bool IsArtifact { get { return true; } }
        public bool Burning { get { return ItemID == 0xA12; } }

        [Constructable]
        public CandlewoodTorch()
            : base(0xF6B)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
        }

        public CandlewoodTorch(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                if (ItemID == 0xF6B)
                {
                    ItemID = 0xA12;
                }
                else if (ItemID == 0xA12)
                {
                    ItemID = 0xF6B;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            if (version < 1)
            {
                LootType = LootType.Blessed;
                Weight = 1.0;
            }
        }
    }
}
