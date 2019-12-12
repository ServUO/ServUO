using System;
using Server;

namespace Server.Items
{
    public class BlackPowder : Item, ICommodity
    {
        public override int LabelNumber { get { return 1095826; } } // black powder

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        [Constructable]
        public BlackPowder()
            : this(1)
        {
        }

        [Constructable]
        public BlackPowder(int amount)
            : base(0x423A)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1109;
        }

        public BlackPowder(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
