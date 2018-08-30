// Created by GreyWolf
// Created On: 10/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class ExpertSmithyGearBag : Bag
	{
           	[Constructable]
           	public ExpertSmithyGearBag()
           	{
                Name = "Bag of Expert Smithy Gear +5";
                Hue = 93;

			DropItem (new LegsofExpertSmithy() );    	
			DropItem(new ArmsofExpertSmithy());
			DropItem(new GlovesofExpertSmithy());
			DropItem(new CapofExpertSmithy());
			DropItem(new GorgetofExpertSmithy());
			DropItem( new TunicofExpertSmithy() );
           	}

           	[Constructable]
           	public ExpertSmithyGearBag(int amount)
           	{
           	}

		
           	public ExpertSmithyGearBag(Serial serial) : base( serial )
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
