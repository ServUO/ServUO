using System;

namespace Server.Mobiles
{
    public class CrystalWisp : Wisp
    {
        [Constructable]
        public CrystalWisp()
        {
            this.Name = "a crystal wisp";
            this.Hue = 0x482;

            this.PackArcaneScroll(0, 1);
        }

        public CrystalWisp(Serial serial)
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