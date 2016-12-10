using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VvV
{
    public class EssenceOfCourage : Item
    {
        public override int LabelNumber { get { return 1155554; } } // Essence of Courage

        [Constructable]
        public EssenceOfCourage()
            : base(3838)
        {
            Hue = 2718;
            //TODO: ID and Stackable?
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.SendLocalizedMessage(1155555); // Feed this to VvV War Steeds to maintain their battle readiness!
            }
        }

        public EssenceOfCourage(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}