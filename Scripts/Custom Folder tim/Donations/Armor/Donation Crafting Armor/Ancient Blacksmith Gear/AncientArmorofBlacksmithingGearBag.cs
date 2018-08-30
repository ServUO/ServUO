// Created by GreyWolf
// Created On: 10/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class AncientArmorofBlacksmithingGearBag : Bag
	{
           	[Constructable]
           	public AncientArmorofBlacksmithingGearBag()
           	{
                Name = "Bag of Ancient Armor of Blacksmithing +10";
                Hue = 1109;

                DropItem(new AncientArmorofBlacksmithingLegs());
                DropItem(new AncientArmorofBlacksmithingArms());
                DropItem(new AncientArmorofBlacksmithingGloves());
                DropItem(new AncientArmorofBlacksmithingCap());
                DropItem(new AncientArmorofBlacksmithingGorget());
                DropItem(new AncientArmorofBlacksmithingChest());
           	}

           	[Constructable]
           	public AncientArmorofBlacksmithingGearBag(int amount)
           	{
           	}


            public AncientArmorofBlacksmithingGearBag(Serial serial)
                : base(serial)
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
