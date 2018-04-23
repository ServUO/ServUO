using System;

namespace Server.Items
{
    public class PumpkinGreenWitch : BaseLight
    {
        [Constructable]
        public PumpkinGreenWitch()
            : base(0x9D23)
        {
            //this.Name = "Tall Pumpkin Witch";
            if (Burnout)
                this.Duration = TimeSpan.FromMinutes(20);
            else
                this.Duration = TimeSpan.Zero;

            this.Burning = false;
            this.Light = LightType.Circle150;
            this.Weight = 1.0;
        }

        public PumpkinGreenWitch(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1096944;
            }
        }// carved pumpkin

        public override int LitItemID
        {
            get
            {
                return 0x9D24;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0x9D23;
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