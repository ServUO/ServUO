using System;

namespace Server.Items
{
    public class CrystallineBlackrock : Item
    {
        [Constructable]
        public CrystallineBlackrock()
            : this(1)
        {
        }

        [Constructable]
        public CrystallineBlackrock(int amount)
            : base(0x5732)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public CrystallineBlackrock(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113344;
            }
        }// crystalline blackrock
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