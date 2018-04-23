// Created by GreyWolf
// Created On: 11/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class SkilledMiningGearBag : Bag
	{
           	[Constructable]
           	public SkilledMiningGearBag()
           	{
                Name = "Bag of Skilled Mining Gear +3";
                Hue = 1418;

			DropItem (new LegsofSkilledMining() );    	
			DropItem(new ArmsofSkilledMining());
			DropItem(new GlovesofSkilledMining());
			DropItem(new CapofSkilledMining());
			DropItem(new GorgetofSkilledMining());
			DropItem( new TunicofSkilledMining() );
           	}

           	[Constructable]
           	public SkilledMiningGearBag(int amount)
           	{
           	}

		
           	public SkilledMiningGearBag(Serial serial) : base( serial )
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
