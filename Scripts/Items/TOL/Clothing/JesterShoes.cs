using System;

namespace Server.Items
{
    public class JesterShoes : BaseShoes
    {
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }

        [Constructable]
        public JesterShoes()
            : this(0)
        {
        }

        [Constructable]
        public JesterShoes(int hue)
            : base(0x7819, hue)
        {
            Weight = 2.0;
        }

        public JesterShoes(Serial serial)
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