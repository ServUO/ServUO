using System;

namespace Server.Items
{
    public class UndamagedIronBeetleScale : Item
    {
        public override int LabelNumber { get { return 1112905; } } // Undamaged Iron Beetle Scale

        [Constructable]
        public UndamagedIronBeetleScale()
            : this(1)
        {
        }

        [Constructable]
        public UndamagedIronBeetleScale(int amount)
            : base(0x26B3)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public UndamagedIronBeetleScale(Serial serial)
            : base(serial)
        {
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