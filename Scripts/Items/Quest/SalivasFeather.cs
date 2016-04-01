using System;

namespace Server.Items
{
    public class SalivasFeather : Item
    {
        [Constructable]
        public SalivasFeather()
            : base(0x1020)
        {
            this.LootType = LootType.Blessed;
            this.Hue = 0x5C;
        }

        public SalivasFeather(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074234;
            }
        }// Saliva's Feather
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