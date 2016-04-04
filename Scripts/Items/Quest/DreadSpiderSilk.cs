using System;

namespace Server.Items
{
    public class DreadSpiderSilk : Item
    {
        [Constructable]
        public DreadSpiderSilk()
            : base(0xDF8)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 4.0;
            this.Hue = 0x481;
        }

        public DreadSpiderSilk(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075319;
            }
        }// Dread Spider Silk
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