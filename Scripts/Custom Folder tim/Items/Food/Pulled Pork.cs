using System;

namespace Server.Items
{
    public class PorkPlatter : Food
    {
        [Constructable]
        public PorkPlatter()
            : base(1, 0x999F)
        {
            this.Name = "Pulled Pork Platter"; //1123351
            this.Weight = 1.0;
            this.FillFactor = 5;
            this.Stackable = false;
            this.Hue = 32;
        }

        public PorkPlatter(Serial serial)
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