using System;

namespace Server.Items
{
    [FlipableAttribute(40699, 40700)]
    public class Tuxedo : BaseShirt
    {
        [Constructable]
        public Tuxedo()
            : this(0)
        {
        }

        [Constructable]
        public Tuxedo(int hue)
            : base(40699, hue)
        {
            Name = "Tuxedo";
			LootType = LootType.Blessed;
			this.Weight = 2.0;
        }

        public Tuxedo(Serial serial)
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