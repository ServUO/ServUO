using System;

namespace Server.Items
{
    [Flipable]
    public class ViceMoonLamp : BaseLight
    {
        [Constructable]
        public ViceMoonLamp()
            : base(0x9DE6)
        {
            this.Movable = true;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 3.0;
        }

        public ViceMoonLamp(Serial serial)
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
                if (this.ItemID == 0x9DE6)
                    return 0x9E04;
                else
                    return 0x9E14;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0x9E04)
                    return 0x9DE6;
                else
                    return 0x9E0B;
            }
        }
        public void Flip()
        {
            this.Light = LightType.Circle300;

            switch ( this.ItemID )
            {
                case 0x9DE6:
                    this.ItemID = 0x9E0B;
                    break;
                case 0x9E04:
                    this.ItemID = 0x9E14;
                    break;
                case 0x9E0B:
                    this.ItemID = 0x9DE6;
                    break;
                case 0x9E14:
                    this.ItemID = 0x9E04;
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