// SCRIPTED BY TASANAR OF WWW.TRUEUO.COM
// KEEP HEADER FOR CREDIT
using Server;
using System;

namespace Server.Items
{
    public class WhiteLeatherDyeTub : DyeTub
	{
		[Constructable]
		public WhiteLeatherDyeTub()
		{
			Name = "White Leather Dye Tub";
			LootType = LootType.Blessed;
			Hue = DyedHue = 2498;
			Redyable = false;
		}

        public WhiteLeatherDyeTub(Serial serial)
            : base(serial)
		{
		}
		
		public override bool AllowDyables
        {
            get
            {
                return false;
            }
        }
        public override bool AllowLeather
        {
            get
            {
                return true;
            }
        }
        public override int TargetMessage
        {
            get
            {
                return 1042416;
            }
        }// Select the leather item to dye.
        public override int FailMessage
        {
            get
            {
                return 1042418;
            }
        }// You can only dye leather with this tub.

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
