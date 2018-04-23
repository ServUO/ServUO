// Created by GreyWolf
// Created On: 11/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class SkilledTinkeringGearBag : Bag
	{
           	[Constructable]
           	public SkilledTinkeringGearBag()
           	{
                Name = "Bag of Skilled Tinkering Gear +3";
                Hue = 702;

			DropItem (new LegsofSkilledTinkering() );    	
			DropItem(new ArmsofSkilledTinkering());
			DropItem(new GlovesofSkilledTinkering());
			DropItem(new CapofSkilledTinkering());
			DropItem(new GorgetofSkilledTinkering());
			DropItem( new TunicofSkilledTinkering() );
           	}

           	[Constructable]
           	public SkilledTinkeringGearBag(int amount)
           	{
           	}

		
           	public SkilledTinkeringGearBag(Serial serial) : base( serial )
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
