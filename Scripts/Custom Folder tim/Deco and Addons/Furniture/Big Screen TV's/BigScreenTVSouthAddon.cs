
////////////////////////////////////////
//                                    //
//      Generated by CEO's YAAAG      //
// (Yet Another Arya Addon Generator) //
//                                    //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BigScreenTVSouthAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {4400, 1, 1, 0}, {6226, 0, 0, 20}, {1824, -1, 0, 15}// 1	2	3	
			, {1824, -1, 0, 10}, {1824, -1, 0, 5}, {4241, -1, 0, 10}// 4	5	6	
			, {1824, 1, 0, 15}, {1824, 2, 0, 0}, {1824, 2, 0, 5}// 8	9	10	
			, {1837, 3, 0, 0}, {1834, -2, 0, 0}, {1836, -2, 1, 0}// 11	12	13	
			, {9269, 0, 1, 0}, {1826, 2, 1, 0}, {1826, -1, 1, 0}// 14	15	16	
			, {4241, 0, 1, 0}, {1824, 2, 0, 15}, {1824, 0, 0, 0}// 17	18	19	
			, {1824, 1, 0, 0}, {9269, 1, 1, 0}, {1824, 2, 0, 10}// 20	21	23	
			, {1824, -1, 0, 0}, {1824, 0, 0, 15}, {1835, 3, 1, 0}// 24	25	26	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new BigScreenTVSouthAddonDeed();
			}
		}

		[ Constructable ]
		public BigScreenTVSouthAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 6732, 1, 0, 5, 6, -1, "TVScreen" );// 7
			AddComplexComponent( (BaseAddon) this, 6732, 0, 0, 5, 6, -1, "TVScreen" );// 22

		}

		public BigScreenTVSouthAddon( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
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

	public class BigScreenTVSouthAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new BigScreenTVSouthAddon();
			}
		}

		[Constructable]
		public BigScreenTVSouthAddonDeed()
		{
			Name = "BigScreenTVSouth";
		}

		public BigScreenTVSouthAddonDeed( Serial serial ) : base( serial )
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