using System;

namespace Server.Items
{
    public class GoblinBlood : Item
    {
        [Constructable]
        public GoblinBlood()
            : this(1)
        {
        }

        [Constructable]
        public GoblinBlood(int amount)
            : base(0x572C)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public GoblinBlood(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113335;
            }
        }// goblin blood
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