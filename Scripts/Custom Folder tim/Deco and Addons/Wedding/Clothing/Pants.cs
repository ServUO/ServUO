using System;

namespace Server.Items
{
    [FlipableAttribute(40695, 40696)]
    public class TuxedoPants : BasePants
    {
        [Constructable]
        public TuxedoPants()
            : this(0)
        {
        }

        [Constructable]
        public TuxedoPants(int hue)
            : base(40695, hue)
        {
            Name = "Tuxedo Pants";
			LootType = LootType.Blessed;
			this.Weight = 2.0;
        }

        public TuxedoPants(Serial serial)
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