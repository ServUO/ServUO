using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    [FlipableAttribute(0x2D6F, 0x2D70)]
    public class SophisticatedElvenTapestry : Item
    {
        [Constructable]
        public SophisticatedElvenTapestry()
            : base(0x2D70)
        {
            this.Weight = 1;
        }

        public override int LabelNumber
        {
            get
            {
                return 1151222;
            }
        }// sophisticated elven tapestry

        public SophisticatedElvenTapestry(Serial serial)
            : base(serial)
        {
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