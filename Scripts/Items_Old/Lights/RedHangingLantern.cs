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
            this.Movable = true;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 3.0;
        }

        public RedHangingLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (this.ItemID == 0x24C2)
                    return 0x24C1;
                else
                    return 0x24C3;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0x24C1)
                    return 0x24C2;
                else
                    return 0x24C4;
            }
        }
        public void Flip()
        {
            this.Light = LightType.Circle300;

            switch ( this.ItemID )
            {
                case 0x24C2:
                    this.ItemID = 0x24C4;
                    break;
                case 0x24C1:
                    this.ItemID = 0x24C3;
                    break;
                case 0x24C4:
                    this.ItemID = 0x24C2;
                    break;
                case 0x24C3:
                    this.ItemID = 0x24C1;
                    break;
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