using System;

namespace Server.Items
{
    public class TerMurStyleCandelabra : BaseLight
    {
        public override int LitItemID { get { return 0x40BE; } }
        public override int UnlitItemID { get { return 0x4039; } }

        [Constructable]
        public TerMurStyleCandelabra()
            : base(0x4039)
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle225;
            Weight = 10.0;
        }

        public TerMurStyleCandelabra(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}