using System;
using Server;

namespace Server.Items
{
	public class TallDoubleLamp : BaseLight, IFlippable
	{
		public override int LitItemID
		{
			get
			{
                if(ItemID == 0x4C56)
                    return 0x4C57;

                return 0x4C59;
			}
		}

		public override int UnlitItemID
		{
			get
			{
                if (ItemID == 0x4C57)
                    return 0x4C56;

                return 0x4C58;
			}
		}

        public int NorthID
        {
            get
            {
                if (Burning)
                    return 0x4C57;
                else
                    return 0x4C56;
            }
        }

        public int WestID
        {
            get
            {
                if (Burning)
                    return 0x4C59;
                else
                    return 0x4C58;
            }
        }

		[Constructable]
        public TallDoubleLamp()
            : base(0x4C56)
		{
			if ( Burnout )
				Duration = TimeSpan.FromMinutes( 60 );
			else
				Duration = TimeSpan.Zero;

			Burning = false;
            Light = LightType.Circle225;
			Weight = 5.0;
		}

        public void OnFlip()
        {
            if (ItemID == NorthID)
                ItemID = WestID;
            else if (ItemID == WestID)
                ItemID = NorthID;
        }

        public TallDoubleLamp(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}