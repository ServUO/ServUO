using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefCarpentry), typeof(GargishSkullGnarledStaff))]
    [FlipableAttribute(41795, 41796)]
    public class SkullGnarledStaff : GnarledStaff
    {
        public override int LabelNumber { get { return 1125819; } } // skull gnarled staff

        [Constructable]
        public SkullGnarledStaff()
        {
            ItemID = 41795;
        }

        public SkullGnarledStaff(Serial serial)
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

    [FlipableAttribute(41799, 41800)]
    public class GargishSkullGnarledStaff : GnarledStaff
    {
        public override int LabelNumber { get { return 1125823; } } // gargish skull gnarled staff
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishSkullGnarledStaff()
        {
            ItemID = 41799;
        }

        public GargishSkullGnarledStaff(Serial serial)
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
