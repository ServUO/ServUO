// Created by GreyWolf
// Created On: 10/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class AncientArmorofTinkeringGearBag : Bag
	{
           	[Constructable]
           	public AncientArmorofTinkeringGearBag()
           	{
                Name = "Bag of Ancient Armor of Tinkering +10";
                Hue = 93;

                DropItem(new AncientArmorofTinkeringLegs());
                DropItem(new AncientArmorofTinkeringArms());
                DropItem(new AncientArmorofTinkeringGloves());
                DropItem(new AncientArmorofTinkeringCap());
                DropItem(new AncientArmorofTinkeringGorget());
                DropItem(new AncientArmorofTinkeringChest());
           	}

           	[Constructable]
           	public AncientArmorofTinkeringGearBag(int amount)
           	{
           	}


            public AncientArmorofTinkeringGearBag(Serial serial)
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
