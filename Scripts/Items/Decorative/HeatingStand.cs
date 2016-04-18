using System;

namespace Server.Items
{
    public class HeatingStand : BaseLight
    {
        [Constructable]
        public HeatingStand()
            : base(0x1849)
        {
            if (Burnout)
                this.Duration = TimeSpan.FromMinutes(25);
            else
                this.Duration = TimeSpan.Zero;

            this.Burning = false;
            this.Light = LightType.Empty;
            this.Weight = 1.0;
        }

        public HeatingStand(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0x184A;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0x1849;
            }
        }
        public override void Ignite()
        {
            base.Ignite();

            if (this.ItemID == this.LitItemID)
                this.Light = LightType.Circle150;
            else if (this.ItemID == this.UnlitItemID)
                this.Light = LightType.Empty;
        }

        public override void Douse()
        {
            base.Douse();

            if (this.ItemID == this.LitItemID)
                this.Light = LightType.Circle150;
            else if (this.ItemID == this.UnlitItemID)
                this.Light = LightType.Empty;
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