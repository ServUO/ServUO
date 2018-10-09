using System;

namespace Server.Items
{
    public class VoidEssence : Item, ICommodity
    {
        [Constructable]
        public VoidEssence()
            : this(1)
        {
        }

        [Constructable]
        public VoidEssence(int amount)
            : base(0x4007)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public VoidEssence(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public override int LabelNumber
        {
            get
            {
                return 1112327;
            }
        }// void essence
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
