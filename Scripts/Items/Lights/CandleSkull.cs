using System;

namespace Server.Items
{
    public class CandleSkull : BaseLight
    {
        [Constructable]
        public CandleSkull()
            : base(0x1853)
        {
            if (Burnout)
                this.Duration = TimeSpan.FromMinutes(25);
            else
                this.Duration = TimeSpan.Zero;

            this.Burning = false;
            this.Light = LightType.Circle150;
            this.Weight = 5.0;
        }

        public CandleSkull(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (this.ItemID == 0x1583 || this.ItemID == 0x1854)
                    return 0x1854;

                return 0x1858;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0x1853 || this.ItemID == 0x1584)
                    return 0x1853;

                return 0x1857;
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