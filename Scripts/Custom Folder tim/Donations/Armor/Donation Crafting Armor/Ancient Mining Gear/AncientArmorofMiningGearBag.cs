// Created by GreyWolf
// Created On: 10/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class AncientArmorofMiningGearBag : Bag
	{
           	[Constructable]
           	public AncientArmorofMiningGearBag()
           	{
                Name = "Bag of Ancient Armor of Mining +10";
                Hue = 1141;

                DropItem(new AncientArmorofMiningLegs());
                DropItem(new AncientArmorofMiningArms());
                DropItem(new AncientArmorofMiningGloves());
                DropItem(new AncientArmorofMiningCap());
                DropItem(new AncientArmorofMiningGorget());
                DropItem(new AncientArmorofMiningChest());
           	}

           	[Constructable]
           	public AncientArmorofMiningGearBag(int amount)
           	{
           	}


            public AncientArmorofMiningGearBag(Serial serial)
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
