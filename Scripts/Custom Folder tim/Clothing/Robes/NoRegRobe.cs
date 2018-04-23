
using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	
	public class NoRegRobe : Robe
	{

		[Constructable]
		public NoRegRobe()
		{
            ItemID = 9860;
            Name = "Shroud of Power";
            Hue = 1195;
		Attributes.LowerRegCost = 100;
			StrRequirement = 0;

            LootType = LootType.Blessed;

		}

        public NoRegRobe(Serial serial): base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override void OnDoubleClick( Mobile from )
		{
            Item y = from.Backpack.FindItemByType(typeof(NoRegRobe));
			if ( y !=null )
			{

                if (this.ItemID == 9860) this.ItemID = 7939;
                else if (this.ItemID == 7939) this.ItemID = 9860;

			}
			else
			{ 
                               	from.SendMessage( "You must have the item in your pack to take down the hood it." ); 
                        }
		}
	}
}