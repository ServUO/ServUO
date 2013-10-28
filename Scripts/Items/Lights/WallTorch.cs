using System;

namespace Server.Items
{
    [Flipable]
    public class WallTorch : BaseLight
    {
        [Constructable]
        public WallTorch()
            : base(0xA05)
        {
            this.Movable = false;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.WestBig;
            this.Weight = 3.0;
        }

        public WallTorch(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (this.ItemID == 0xA05)
                    return 0xA07;
                else
                    return 0xA0C;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0xA07)
                    return 0xA05;
                else
                    return 0xA0A;
            }
        }
        public void Flip()
        {
            if (this.Light == LightType.WestBig)
                this.Light = LightType.NorthBig;
            else if (this.Light == LightType.NorthBig)
                this.Light = LightType.WestBig;

            switch ( this.ItemID )
            {
                case 0xA05:
                    this.ItemID = 0xA0A;
                    break;
                case 0xA07:
                    this.ItemID = 0xA0C;
                    break;
                case 0xA0A:
                    this.ItemID = 0xA05;
                    break;
                case 0xA0C:
                    this.ItemID = 0xA07;
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