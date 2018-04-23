
////////////////////////////////////////
//                                     //
//   Generated by CEO's YAAAG - Ver 2  //
// (Yet Another Arya Addon Generator)  //
//    Modified by Hammerhand for       //
//      SA & High Seas content         //
//     Created by Hiro 12-30-2016      //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class GreyLargeFPEastAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1305, 1, 2, 0}, {1305, 1, -2, 0}, {1305, 1, 1, 0}// 1	2	3	
			, {1305, 1, 0, 0}, {1305, 1, -1, 0}, {1822, -1, 2, 5}// 4	5	6	
			, {1822, 0, 2, 0}, {1822, -1, 2, 0}, {1305, 0, 2, 0}// 7	8	9	
			, {1305, -1, 2, 0}, {1822, 0, 2, 5}, {1822, 0, 2, 10}// 10	11	12	
			, {1822, -1, 2, 10}, {1997, -1, 2, 15}, {1997, 0, 2, 15}// 13	14	15	
			, {1822, -1, -1, 5}, {1822, -1, -2, 0}, {1822, -1, 0, 5}// 22	23	24	
			, {1822, -1, 1, 5}, {1305, -1, -2, 0}, {1822, -1, 1, 0}// 25	26	27	
			, {1822, -1, 0, 0}, {1822, -1, -1, 0}, {1822, -1, -2, 5}// 28	29	30	
			, {1822, 0, -2, 0}, {1305, 0, 0, 0}, {1305, 0, -1, 0}// 31	32	33	
			, {1305, 0, -2, 0}, {1305, 0, 1, 0}, {1305, -1, 0, 0}// 34	35	36	
			, {1305, -1, 1, 0}, {1305, -1, -1, 0}, {1822, 0, 0, 10}// 37	38	39	
			, {1822, 0, 1, 10}, {1822, 0, -1, 10}, {1822, 0, -2, 10}// 40	41	42	
			, {1822, -1, -2, 10}, {1822, 0, -2, 5}, {1997, -1, 1, 15}// 43	44	45	
			, {1997, -1, -1, 15}, {1997, -1, 0, 15}, {1997, 0, -2, 15}// 46	47	48	
			, {1997, 0, -1, 15}, {1997, -1, -2, 15}, {1997, 0, 0, 15}// 49	50	51	
			, {1997, 0, 1, 15}, {1822, -1, -1, 10}, {1822, -1, 0, 10}// 52	53	54	
			, {1822, -1, 1, 10}// 55	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new GreyLargeFPEastAddonDeed();
			}
		}

		[ Constructable ]
		public GreyLargeFPEastAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 3555, 0, 1, 0, 0, 29, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 3555, 0, 1, 4, 0, 29, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 3555, 0, 0, 0, 0, 29, "", 1);// 18
			AddComplexComponent( (BaseAddon) this, 3555, 0, 0, 4, 0, 29, "", 1);// 19
			AddComplexComponent( (BaseAddon) this, 3555, 0, -1, 0, 0, 29, "", 1);// 20
			AddComplexComponent( (BaseAddon) this, 3555, 0, -1, 4, 0, 29, "", 1);// 21

		}

		public GreyLargeFPEastAddon( Serial serial ) : base( serial )
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

	public class GreyLargeFPEastAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new GreyLargeFPEastAddon();
			}
		}

		[Constructable]
		public GreyLargeFPEastAddonDeed()
		{
			Name = "GreyLargeFPEast";
		}

		public GreyLargeFPEastAddonDeed( Serial serial ) : base( serial )
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
