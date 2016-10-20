using System;

namespace Server.Items
{
    public class NexusCore : Item
    {
        [Constructable]
        public NexusCore() : this(1)
        {
        }

        [Constructable]
        public NexusCore(int amount) : base(0x4B82)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1;
        }

        public NexusCore(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber { get { return 1153501; } } // nexus core 

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