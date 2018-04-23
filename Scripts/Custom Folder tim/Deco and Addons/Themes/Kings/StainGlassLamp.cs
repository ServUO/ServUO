using System;

namespace Server.Items
{
    public class StainGlassLamp : BaseLight
    {
        [Constructable]
        public StainGlassLamp()
            : base(0x4C50)
        {
            this.Movable = false;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 5.0;
        }

        public StainGlassLamp(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0x4C51;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0x4C50;
            }
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