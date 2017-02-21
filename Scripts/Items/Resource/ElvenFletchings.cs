using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.ElvenFletchings")]
    public class ElvenFletching : Item
    {
        [Constructable]
        public ElvenFletching()
            : this(1)
        {
        }

        [Constructable]
        public ElvenFletching(int amount)
            : base(0x5737)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public ElvenFletching(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113346;
            }
        }// elven fletching
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