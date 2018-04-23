
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
	public class BlueCouchNorthAddon : BaseAddon
	{
         
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new BlueCouchNorthAddonDeed();
			}
		}

		[ Constructable ]
		public BlueCouchNorthAddon()
		{



			AddComplexComponent( (BaseAddon) this, 5029, 2, 1, 5, 204, -1, "A Pillow", 1);// 1
			AddComplexComponent( (BaseAddon) this, 5029, -2, 1, 5, 204, -1, "A Pillow", 1);// 2
			AddComplexComponent( (BaseAddon) this, 5028, 2, 0, 5, 204, -1, "A Pillow", 1);// 3
			AddComplexComponent( (BaseAddon) this, 5028, -2, 0, 5, 204, -1, "A Pillow", 1);// 4
			AddComplexComponent( (BaseAddon) this, 5692, 0, 1, 5, 204, -1, "A Pillow", 1);// 5
			AddComplexComponent( (BaseAddon) this, 5691, 1, 1, 5, 204, -1, "", 1);// 6
			AddComplexComponent( (BaseAddon) this, 5691, -1, 1, 5, 204, -1, "", 1);// 7
			AddComplexComponent( (BaseAddon) this, 2861, 1, 0, 0, 101, -1, "", 1);// 8
			AddComplexComponent( (BaseAddon) this, 2861, 0, 0, 0, 101, -1, "", 1);// 9
			AddComplexComponent( (BaseAddon) this, 2861, -1, 0, 0, 101, -1, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 1848, 2, 0, 0, 101, -1, "", 1);// 11
			AddComplexComponent( (BaseAddon) this, 1848, 2, 1, 0, 101, -1, "", 1);// 12
			AddComplexComponent( (BaseAddon) this, 1848, 1, 1, 0, 101, -1, "", 1);// 13
			AddComplexComponent( (BaseAddon) this, 1848, 0, 1, 0, 101, -1, "", 1);// 14
			AddComplexComponent( (BaseAddon) this, 1848, -1, 1, 0, 101, -1, "", 1);// 15
			AddComplexComponent( (BaseAddon) this, 1848, -2, 1, 0, 101, -1, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 1848, -2, 0, 0, 101, -1, "", 1);// 17

		}

		public BlueCouchNorthAddon( Serial serial ) : base( serial )
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

	public class BlueCouchNorthAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new BlueCouchNorthAddon();
			}
		}

		[Constructable]
		public BlueCouchNorthAddonDeed()
		{
			Name = "BlueCouchNorth";
		}

		public BlueCouchNorthAddonDeed( Serial serial ) : base( serial )
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