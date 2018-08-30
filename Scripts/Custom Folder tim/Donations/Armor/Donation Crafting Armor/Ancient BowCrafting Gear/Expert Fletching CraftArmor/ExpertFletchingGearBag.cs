// Created by GreyWolf
// Created On: 10/11/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class ExpertFletchingGearBag : Bag
	{
           	[Constructable]
           	public ExpertFletchingGearBag()
           	{
                Name = "Bag of Expert Fletching Gear +5";
                Hue = 62;

			DropItem (new LegsofExpertFletching() );    	
			DropItem(new ArmsofExpertFletching());
			DropItem(new GlovesofExpertFletching());
			DropItem(new CapofExpertFletching());
			DropItem(new GorgetofExpertFletching());
			DropItem( new TunicofExpertFletching() );
           	}

           	[Constructable]
           	public ExpertFletchingGearBag(int amount)
           	{
           	}


        public ExpertFletchingGearBag(Serial serial) : base(serial)
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
