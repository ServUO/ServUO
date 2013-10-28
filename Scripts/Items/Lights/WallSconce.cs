using System;

namespace Server.Items
{
    [Flipable]
    public class WallSconce : BaseLight
    {
        [Constructable]
        public WallSconce()
            : base(0x9FB)
        {
            this.Movable = false;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.WestBig;
            this.Weight = 3.0;
        }

        public WallSconce(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (this.ItemID == 0x9FB)
                    return 0x9FD;
                else
                    return 0xA02;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0x9FD)
                    return 0x9FB;
                else
                    return 0xA00;
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
                case 0x9FB:
                    this.ItemID = 0xA00;
                    break;
                case 0x9FD:
                    this.ItemID = 0xA02;
                    break;
                case 0xA00:
                    this.ItemID = 0x9FB;
                    break;
                case 0xA02:
                    this.ItemID = 0x9FD;
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