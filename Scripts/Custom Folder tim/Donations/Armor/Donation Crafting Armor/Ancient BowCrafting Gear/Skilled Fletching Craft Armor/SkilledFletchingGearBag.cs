// Created by GreyWolf
// Created On: 10/11/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class SkilledFletchingGearBag : Bag
	{
           	[Constructable]
           	public SkilledFletchingGearBag()
           	{
                Name = "Bag of Skilled Fletching Gear +3";
                Hue = 62;

			DropItem (new LegsofSkilledFletching() );    	
			DropItem(new ArmsofSkilledFletching());
			DropItem(new GlovesofSkilledFletching());
			DropItem(new CapofSkilledFletching());
			DropItem(new GorgetofSkilledFletching());
			DropItem( new TunicofSkilledFletching() );
           	}

           	[Constructable]
           	public SkilledFletchingGearBag(int amount)
           	{
           	}


        public SkilledFletchingGearBag(Serial serial) : base(serial)
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
