using System;

namespace Server.Items
{
    [Flipable]
    public class RedHangingLantern : BaseLight
    {
        [Constructable]
        public RedHangingLantern()
            : base(0x24C2)
        {
            Movable = true;
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 3.0;
        }

        public RedHangingLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (ItemID == 0x24C2)
                    return 0x24C1;
                else
                    return 0x24C3;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (ItemID == 0x24C1)
                    return 0x24C2;
                else
                    return 0x24C4;
            }
        }
        public void Flip()
        {
            Light = LightType.Circle300;

            switch (ItemID)
            {
                case 0x24C2:
                    ItemID = 0x24C4;
                    break;
                case 0x24C1:
                    ItemID = 0x24C3;
                    break;
                case 0x24C4:
                    ItemID = 0x24C2;
                    break;
                case 0x24C3:
                    ItemID = 0x24C1;
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