using System;

namespace Server.Items
{
    public class PorkSandwich : Food
    {
        [Constructable]
        public PorkSandwich()
            : base(1, 0x99A0)
        {
            this.Name = "Pulled Pork Sandwich"; //1123352
            this.Weight = 1.0;
            this.FillFactor = 3;
            this.Stackable = false;
        }

        public PorkSandwich(Serial serial)
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