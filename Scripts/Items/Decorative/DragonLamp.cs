using System;
using Server;

namespace Server.Items
{
	public class CraftableDragonLamp : BaseLight, IFlippable
	{
		public override int LitItemID
		{
			get
			{
                if(ItemID == 0x4C4C)
                    return 0x4C4D;

                return 0x4C4F;
			}
		}

		public override int UnlitItemID
		{
			get
			{
                if(ItemID == 0x4C4D)
				    return 0x4C4C;

                return 0x4C4E;
			}
		}

        public int NorthID
        {
            get
            {
                if (Burning)
                    return 0x4C4D;
                else
                    return 0x4C4C;
            }
        }

        public int WestID
        {
            get
            {
                if (Burning)
                    return 0x4C4F;
                else
                    return 0x4C4E;
            }
        }

		[Constructable]
        public CraftableDragonLamp()
            : base(0x4C4C)
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

        public CraftableDragonLamp(Serial serial)
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