using Server;
using System;
using Server.Multis;
using Server.Mobiles;

namespace Server.Items
{
    public class DeckItem : Item
    {
        public override int LabelNumber { get { return 1035994; } } // deck

        public DeckItem(int itemID)
            : base(itemID)
        {
            Movable = false;
        }

        public DeckItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WeaponPad : Item
    {
        public override int LabelNumber { get { return 1102376; } } // weapon pad

        public WeaponPad(int itemID)
            : base(itemID)
        {
            Movable = false;
        }

        public WeaponPad(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
