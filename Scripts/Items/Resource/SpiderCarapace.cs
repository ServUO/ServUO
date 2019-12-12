using System;

namespace Server.Items
{
    public class SpiderCarapace : Item, ICommodity
    {
        [Constructable]
        public SpiderCarapace()
            : this(1)
        {
        }

        [Constructable]
        public SpiderCarapace(int amount)
            : base(0x5720)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public SpiderCarapace(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public override int LabelNumber
        {
            get
            {
                return 1113329;
            }
        }// spider carapace
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
