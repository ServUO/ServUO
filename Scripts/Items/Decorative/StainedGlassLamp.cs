using System;

namespace Server.Items
{
    public class StainedGlassLamp : BaseLight
    {
        public override int LitItemID => 0x4C51;
        public override int UnlitItemID => 0x4C50;

        [Constructable]
        public StainedGlassLamp()
            : base(0x4C50)
        {
            Duration = Burnout ? TimeSpan.FromMinutes(60) : TimeSpan.Zero;
            Burning = false;
            Light = LightType.Circle225;
            Weight = 1.0;
        }

        public StainedGlassLamp(Serial serial)
            : base(serial)
        {
        }

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