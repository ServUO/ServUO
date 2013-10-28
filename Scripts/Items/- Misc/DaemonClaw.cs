using System;

namespace Server.Items
{
    public class DaemonClaw : Item
    {
        [Constructable]
        public DaemonClaw()
            : this(1)
        {
        }

        [Constructable]
        public DaemonClaw(int amount)
            : base(0x5721)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public DaemonClaw(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113330;
            }
        }// daemon claw
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