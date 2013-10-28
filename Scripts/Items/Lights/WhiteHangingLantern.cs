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
            this.Movable = true;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 3.0;
        }

        public WhiteHangingLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (this.ItemID == 0x24C6)
                    return 0x24C5;
                else
                    return 0x24C7;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0x24C5)
                    return 0x24C6;
                else
                    return 0x24C8;
            }
        }
        public void Flip()
        {
            this.Light = LightType.Circle300;

            switch ( this.ItemID )
            {
                case 0x24C6:
                    this.ItemID = 0x24C8;
                    break;
                case 0x24C5:
                    this.ItemID = 0x24C7;
                    break;
                case 0x24C8:
                    this.ItemID = 0x24C6;
                    break;
                case 0x24C7:
                    this.ItemID = 0x24C5;
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