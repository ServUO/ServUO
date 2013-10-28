using System;

namespace Server.Items
{
    public class GamanHorns : Item
    {
        [Constructable]
        public GamanHorns()
            : this(1)
        {
        }

        [Constructable]
        public GamanHorns(int amount)
            : base(0x1084)
        {
            this.LootType = LootType.Blessed;
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1;
            this.Hue = 0x395;
        }

        public GamanHorns(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074557;
            }
        }// Gaman Horns
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