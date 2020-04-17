using System;

namespace Server.Items
{
    [Flipable]
    public class WhiteHangingLantern : BaseLight
    {
        [Constructable]
        public WhiteHangingLantern()
            : base(0x24C6)
        {
            Movable = true;
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 3.0;
        }

        public WhiteHangingLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (ItemID == 0x24C6)
                    return 0x24C5;
                else
                    return 0x24C7;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (ItemID == 0x24C5)
                    return 0x24C6;
                else
                    return 0x24C8;
            }
        }
        public void Flip()
        {
            Light = LightType.Circle300;

            switch (ItemID)
            {
                case 0x24C6:
                    ItemID = 0x24C8;
                    break;
                case 0x24C5:
                    ItemID = 0x24C7;
                    break;
                case 0x24C8:
                    ItemID = 0x24C6;
                    break;
                case 0x24C7:
                    ItemID = 0x24C5;
                    break;
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