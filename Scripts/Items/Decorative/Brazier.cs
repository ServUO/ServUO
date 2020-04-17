using System;

namespace Server.Items
{
    public class Brazier : BaseLight
    {
        [Constructable]
        public Brazier()
            : base(0xE31)
        {
            Movable = false;
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = true;
            Light = LightType.Circle225;
            Weight = 20.0;
        }

        public Brazier(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID => 0xE31;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}