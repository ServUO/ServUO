// Created by GreyWolf
// Created On: 10/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class SkilledSmithyGearBag : Bag
	{
           	[Constructable]
           	public SkilledSmithyGearBag()
           	{
                Name = "Bag of Skilled Smithy Gear +3";
                Hue = 88;

			DropItem (new LegsofSkilledSmithy() );    	
			DropItem(new ArmsofSkilledSmithy());
			DropItem(new GlovesofSkilledSmithy());
			DropItem(new CapofSkilledSmithy());
			DropItem(new GorgetofSkilledSmithy());
			DropItem( new TunicofSkilledSmithy() );
           	}

           	[Constructable]
           	public SkilledSmithyGearBag(int amount)
           	{
           	}

		
           	public SkilledSmithyGearBag(Serial serial) : base( serial )
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
