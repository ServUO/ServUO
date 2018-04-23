// Created by GreyWolf
// Created On: 11/4/2007

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class ExpertMiningGearBag : Bag
	{
           	[Constructable]
           	public ExpertMiningGearBag()
           	{
                Name = "Bag of Expert Mining Gear +5";
                Hue = 1318;

			DropItem (new LegsofExpertMining() );    	
			DropItem(new ArmsofExpertMining());
			DropItem(new GlovesofExpertMining());
			DropItem(new CapofExpertMining());
			DropItem(new GorgetofExpertMining());
			DropItem( new TunicofExpertMining() );
           	}

           	[Constructable]
           	public ExpertMiningGearBag(int amount)
           	{
           	}

		
           	public ExpertMiningGearBag(Serial serial) : base( serial )
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
