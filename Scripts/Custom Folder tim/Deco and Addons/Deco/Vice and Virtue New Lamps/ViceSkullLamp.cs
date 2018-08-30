using System;

namespace Server.Items
{
    [Flipable]
    public class ViceSkullLamp : BaseLight
    {
        [Constructable]
        public ViceSkullLamp()
            : base(0x9DE8)
        {
            this.Movable = true;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 3.0;
        }

        public ViceSkullLamp(Serial serial)
            : base(serial)
        {
        }
        
        public override int LabelNumber
        {
            get
            {
                return 1124486;
            }
        }// lantern

        public override int LitItemID
        {
            get
            {
                if (this.ItemID == 0x9DE8)
                    return 0x9E07;
                else
                    return 0x9E17;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0x9E07)
                    return 0x9DE8;
                else
                    return 0x9E0D;
            }
        }
        public void Flip()
        {
            this.Light = LightType.Circle300;

            switch ( this.ItemID )
            {
                case 0x9DE8:
                    this.ItemID = 0x9E0D;
                    break;
                case 0x9E07:
                    this.ItemID = 0x9E17;
                    break;
                case 0x9E0D:
                    this.ItemID = 0x9DE8;
                    break;
                case 0x9E17:
                    this.ItemID = 0x9E07;
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