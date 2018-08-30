// Created by GreyWolf
// Created On: 10/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class ExpertTailoringGearBag : Bag
	{
           	[Constructable]
           	public ExpertTailoringGearBag()
           	{
                Name = "Bag of Expert Tailoring Gear +5";
                Hue = 62;

			DropItem (new LegsofExpertTailoring() );    	
			DropItem(new ArmsofExpertTailoring());
			DropItem(new GlovesofExpertTailoring());
			DropItem(new CapofExpertTailoring());
			DropItem(new GorgetofExpertTailoring());
			DropItem( new TunicofExpertTailoring() );
           	}

           	[Constructable]
           	public ExpertTailoringGearBag(int amount)
           	{
           	}


        public ExpertTailoringGearBag(Serial serial) : base(serial)
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
