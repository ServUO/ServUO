// Created by GreyWolf
// Created On: 11/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class ExpertTinkeringGearBag : Bag
	{
           	[Constructable]
           	public ExpertTinkeringGearBag()
           	{
                Name = "Bag of Expert Tinkering Gear +5";
                Hue = 702;

			DropItem (new LegsofExpertTinkering() );    	
			DropItem(new ArmsofExpertTinkering());
			DropItem(new GlovesofExpertTinkering());
			DropItem(new CapofExpertTinkering());
			DropItem(new GorgetofExpertTinkering());
			DropItem( new TunicofExpertTinkering() );
           	}

           	[Constructable]
           	public ExpertTinkeringGearBag(int amount)
           	{
           	}

		
           	public ExpertTinkeringGearBag(Serial serial) : base( serial )
           	{
           	}

          	public override void Serialize(GenericWriter writer)
          	{
           		base.Serialize(writer);

           		writer.Write((int)0); // version 
     		}

           	public override void Deserialize(GenericReader reader)
      	{
           		base.Deserialize(reader);

          		int version = reader.ReadInt();
           	}
	}
}
