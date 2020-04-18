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
                Duration = TimeSpan.FromMinutes(25);
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle150;
            Weight = 5.0;
        }

        public CandleSkull(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (ItemID == 0x1583 || ItemID == 0x1854)
                    return 0x1854;

                return 0x1858;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (ItemID == 0x1853 || ItemID == 0x1584)
                    return 0x1853;

                return 0x1857;
            }
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