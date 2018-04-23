using System;

namespace Server.Items
{
    [Flipable]
    public class VirtueChaliceLantern : BaseLight
    {
        [Constructable]
        public VirtueChaliceLantern()
            : base(0x9DE5)
        {
            this.Movable = true;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 3.0;
        }

        public VirtueChaliceLantern(Serial serial)
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
                if (this.ItemID == 0x9DE5)
                    return 0x9DFE;
                else
                    return 0x9E0E;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0x9DFE)
                    return 0x9DE5;
                else
                    return 0x9E0A;
            }
        }
        public void Flip()
        {
            this.Light = LightType.Circle300;

            switch ( this.ItemID )
            {
                case 0x9DE5:
                    this.ItemID = 0x9E0A;
                    break;
                case 0x9DFE:
                    this.ItemID = 0x9E0E;
                    break;
                case 0x9E0A:
                    this.ItemID = 0x9DE5;
                    break;
                case 0x9E0E:
                    this.ItemID = 0x9DFE;
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