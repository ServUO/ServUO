// Created by GreyWolf
// Created On: 10/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class AncientArmorofTailoringGearBag : Bag
	{
           	[Constructable]
           	public AncientArmorofTailoringGearBag()
           	{
                Name = "Bag of Ancient Armor of Tailoring";
                Hue = 93;

                DropItem(new AncientArmorofTailoringLegs());
                DropItem(new AncientArmorofTailoringArms());
                DropItem(new AncientArmorofTailoringGloves());
                DropItem(new AncientArmorofTailoringCap());
                DropItem(new AncientArmorofTailoringGorget());
                
                DropItem(new AncientArmorofTailoringChest());
           	}
        
           	[Constructable]
           	public AncientArmorofTailoringGearBag(int amount)
           	{
           	}


            public AncientArmorofTailoringGearBag(Serial serial)
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
