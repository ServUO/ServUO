using System;

namespace Server.Items
{
    public class LampPost3 : BaseLight
    {
        [Constructable]
        public LampPost3()
            : base(0xb25)
        {
            this.Movable = false;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 40.0;
        }

        public LampPost3(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0xB24;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0xB25;
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