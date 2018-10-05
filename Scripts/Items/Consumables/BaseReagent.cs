using System;

namespace Server.Items
{
    public abstract class BaseReagent : Item, ICommodity
    {
        public BaseReagent(int itemID)
            : this(itemID, 1)
        {
        }

        public BaseReagent(int itemID, int amount)
            : base(itemID)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public BaseReagent(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
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
