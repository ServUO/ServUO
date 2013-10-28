using System;

namespace Server.Items
{
    public class ExcellentIronMaiden : Item
    {
        [Constructable]
        public ExcellentIronMaiden()
            : base(0x3f15)
        {
        }

        public ExcellentIronMaiden(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 5;
            }
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