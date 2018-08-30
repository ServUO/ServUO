using System;

namespace Server.Items
{
    [Flipable]
	public class FlamingScarecrow : BaseLight
	{
		[Constructable]
		public FlamingScarecrow() : base( 40755 )//1
		{
			Name = "A Flaming Scarecrow";
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = false;
			Light = LightType.Circle300;
			Weight = 10.0;
		}

		public FlamingScarecrow( Serial serial ) : base( serial )
		{
		}
		
		public override int LitItemID
		{
			get
			{
				if ( this.ItemID == 40755 )//1
					return 40756;//2
				else
					return 40762;//4
			}
		}
		
		public override int UnlitItemID
		{
			get
			{
				if ( this.ItemID == 40756 )//2
					return 40755;//1
				else
					return 40761;//3
			}
		}

		public void Flip()
		{
			this.Light = LightType.Circle300;

			switch ( this.ItemID )
			{
				case 40755: 
					this.ItemID = 40761; 
					break;//unlit
				case 40756: 
					this.ItemID = 40762; 
					break;//lit

				case 40761: 
					this.ItemID = 40755; 
					break;//unlit
				case 40762: 
					this.ItemID = 40756; 
					break;//lit
			}
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}