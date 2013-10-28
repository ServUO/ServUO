using System;

namespace Server.Items
{
    [FlipableAttribute(0x4042, 0x4043)]
    public class GargoyleVase : Item
    {
        [Constructable]
        public GargoyleVase()
            : base(0x4042)
        {
            this.Weight = 10;
        }

        public GargoyleVase(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
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