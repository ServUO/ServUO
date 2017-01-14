using System;

namespace Server.Items
{
    public class BagOfNecromancerReagents : Bag
    {
        [Constructable]
        public BagOfNecromancerReagents()
            : this(50)
        {
        }

        [Constructable]
        public BagOfNecromancerReagents(int amount)
        {
            this.DropItem(new BatWing(amount));
            this.DropItem(new GraveDust(amount));
            this.DropItem(new DaemonBlood(amount));
            this.DropItem(new NoxCrystal(amount));
            this.DropItem(new PigIron(amount));
        }

        public BagOfNecromancerReagents(Serial serial)
            : base(serial)
        {
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