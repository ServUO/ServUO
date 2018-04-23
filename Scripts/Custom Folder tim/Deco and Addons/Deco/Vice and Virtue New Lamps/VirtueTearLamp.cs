using System;

namespace Server.Items
{
    [Flipable]
    public class VirtueTearLamp : BaseLight
    {
        [Constructable]
        public VirtueTearLamp()
            : base(0x9DE7)
        {
            this.Movable = true;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 3.0;
        }

        public VirtueTearLamp(Serial serial)
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
                if (this.ItemID == 0x9DE7)
                    return 0x9E01;
                else
                    return 0x9E11;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0x9E01)
                    return 0x9DE7;
                else
                    return 0x9E0C;
            }
        }
        public void Flip()
        {
            this.Light = LightType.Circle300;

            switch ( this.ItemID )
            {
                case 0x9DE7:
                    this.ItemID = 0x9E0C;
                    break;
                case 0x9E01:
                    this.ItemID = 0x9E11;
                    break;
                case 0x9E0C:
                    this.ItemID = 0x9DE7;
                    break;
                case 0x9E11:
                    this.ItemID = 0x9E01;
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