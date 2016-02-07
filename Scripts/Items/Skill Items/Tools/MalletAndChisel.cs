using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class MalletAndChisel : BaseTool
    {
        [Constructable]
        public MalletAndChisel()
            : base(0x12B3)
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public MalletAndChisel(int uses)
            : base(uses, 0x12B3)
        {
            this.Weight = 1.0;
        }

        public MalletAndChisel(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem
        {
            get
            {
                return DefMasonry.CraftSystem;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}