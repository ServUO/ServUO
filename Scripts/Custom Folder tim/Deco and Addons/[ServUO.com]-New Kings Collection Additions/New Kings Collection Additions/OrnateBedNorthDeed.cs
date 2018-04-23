
////////////////////////////////////////
//                                     //
//   Generated by CEO's YAAAG - Ver 2  //
// (Yet Another Arya Addon Generator)  //
//    Modified by Hammerhand for       //
//      SA & High Seas content         //
//                                     //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class OrnateBedNorth : BaseAddon
	{
         
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new OrnateBedNorthDeed();
			}
		}

		[ Constructable ]
		public OrnateBedNorth()
		{



			AddComplexComponent( (BaseAddon) this, 19575, 0, -1, 0, 1154, -1, "(6,19575): 0, -1, 0", 1);// 1
			AddComplexComponent( (BaseAddon) this, 19577, 0, 1, 0, 1154, -1, "(2,19577): 0, 1, 0", 1);// 2
			AddComplexComponent( (BaseAddon) this, 19572, 1, 0, 0, 1154, -1, "(3,19572): 1, 0, 0", 1);// 3
			AddComplexComponent( (BaseAddon) this, 19574, 1, -1, 0, 1154, -1, "(5,19574): 1, -1, 0", 1);// 4
			AddComplexComponent( (BaseAddon) this, 19573, 1, 1, 0, 1154, -1, "(1,19573): 1, 1, 0", 1);// 5
			AddComplexComponent( (BaseAddon) this, 19576, 0, 0, 0, 1154, -1, "(4,19576): 0, 0, 0", 1);// 6

		}

		public OrnateBedNorth( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null && name.Length > 0)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType) lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class OrnateBedNorthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new OrnateBedNorth();
			}
		}

		[Constructable]
		public OrnateBedNorthDeed()
		{
			Name = "Ornate Bed North Deed";
		}

		public OrnateBedNorthDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
