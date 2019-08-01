using System;

namespace Server.Items
{
    public class MrPlainsCookies : Food
    {
		public override int LabelNumber { get { return 1153774; } }// Mr. Plain's Cookies
		
        [Constructable]
        public MrPlainsCookies()
            : base(0x160C)
        {
            Weight = 1.0;
            FillFactor = 4;
            Hue = 0xF4;
            Stackable = false;
        }

        public MrPlainsCookies(Serial serial)
            : base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}