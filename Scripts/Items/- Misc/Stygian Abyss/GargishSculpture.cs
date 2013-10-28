using System;

namespace Server.Items
{
    [FlipableAttribute(0x403F, 0x4040)]
    public class GargishSculpture : Item
    {
        [Constructable]
        public GargishSculpture()
            : base(0x403F)
        {
            this.Weight = 10;
        }

        public GargishSculpture(Serial serial)
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