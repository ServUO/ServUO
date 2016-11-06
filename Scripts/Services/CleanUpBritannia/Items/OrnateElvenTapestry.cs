using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    [FlipableAttribute(0x2D71, 0x2D72)]
    public class OrnateElvenTapestry : Item
    {
        [Constructable]
        public OrnateElvenTapestry()
            : base(0x2D72)
        {
            this.Weight = 1;
        }

        public override int LabelNumber
        {
            get
            {
                return 1031633;
            }
        }// ornate elven tapestry

        public OrnateElvenTapestry(Serial serial)
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