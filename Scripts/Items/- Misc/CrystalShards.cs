using System;

namespace Server.Items
{
    public class CrystalShards : Item
    {
        [Constructable]
        public CrystalShards()
            : this(1)
        {
        }

        [Constructable]
        public CrystalShards(int amount)
            : base(0x5738)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public CrystalShards(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113347;
            }
        }// crystal shards
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