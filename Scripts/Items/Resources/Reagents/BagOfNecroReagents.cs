using System;

namespace Server.Items
{
    public class BagOfNecroReagents : Bag
    {
        [Constructable]
        public BagOfNecroReagents()
            : this(50)
        {
        }

        [Constructable]
        public BagOfNecroReagents(int amount)
        {
            this.DropItem(new BatWing(amount));
            this.DropItem(new GraveDust(amount));
            this.DropItem(new DaemonBlood(amount));
            this.DropItem(new NoxCrystal(amount));
            this.DropItem(new PigIron(amount));
        }

        public BagOfNecroReagents(Serial serial)
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