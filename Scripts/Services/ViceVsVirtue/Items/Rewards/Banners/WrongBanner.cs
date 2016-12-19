using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.VvV
{
    [FlipableAttribute(39349, 39350)]
    public class WrongBanner : Item
    {
        public override int LabelNumber
        {
            get
            {
                return 1123373;
            }
        }

        [Constructable]
        public WrongBanner()
            : base(39349)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(this.GetWorldLocation(), 2))
            {
                Gump g = new Gump(50, 50);
                g.AddImage(0, 0, 30574);
                m.SendGump(g);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1154937); // vvv item
        }

        public WrongBanner(Serial serial)
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