// Created by GreyWolf
// Created On: 10/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class AncientBowCraftandFletchingArmorGearBag : Bag
	{
           	[Constructable]
           	public AncientBowCraftandFletchingArmorGearBag()
           	{
                Name = "Bag of Ancient BowCraft and Fletching Armor +10";
                Hue = 93;

                DropItem(new AncientBowCraftandFletchingArmorLegs());
                DropItem(new AncientBowCraftandFletchingArmorArms());
                DropItem(new AncientBowCraftandFletchingArmorGloves());
                DropItem(new AncientBowCraftandFletchingArmorCap());
                DropItem(new AncientBowCraftandFletchingArmorGorget());
                DropItem(new AncientBowCraftandFletchingArmorChest());
           	}

           	[Constructable]
           	public AncientBowCraftandFletchingArmorGearBag(int amount)
           	{
           	}


            public AncientBowCraftandFletchingArmorGearBag(Serial serial)
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
