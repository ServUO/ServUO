// Created by GreyWolf
// Created On: 10/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class SkilledTailoringGearBag : Bag
	{
           	[Constructable]
           	public SkilledTailoringGearBag()
           	{
                Name = "Bag of Skilled Tailoring Gear +3";
                Hue = 62;

			DropItem (new LegsofSkilledTailoring() );    	
			DropItem(new ArmsofSkilledTailoring());
			DropItem(new GlovesofSkilledTailoring());
			DropItem(new CapofSkilledTailoring());
			DropItem(new GorgetofSkilledTailoring());
			DropItem( new TunicofSkilledTailoring() );
           	}

           	[Constructable]
           	public SkilledTailoringGearBag(int amount)
           	{
           	}


        public SkilledTailoringGearBag(Serial serial) : base(serial)
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
