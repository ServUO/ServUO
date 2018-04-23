
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
	public class ChampsixAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1822, -2, 0, 0}, {1822, -2, -1, 0}, {1822, -2, -2, 0}// 1	2	3	
			, {1865, -3, 3, 0}, {1847, -2, -3, 0}, {1846, 4, 3, 0}// 4	5	6	
			, {1822, 0, -2, 0}, {1822, 1, 2, 0}, {1822, 1, 1, 0}// 7	8	9	
			, {1822, 1, 0, 0}, {1822, 1, -1, 0}, {1822, 0, -1, 0}// 10	11	12	
			, {1822, -2, 2, 0}, {1822, -2, 3, 0}, {1822, -1, 3, 0}// 13	14	15	
			, {1822, 0, 3, 0}, {1822, 1, 3, 0}, {1822, 2, 3, 0}// 16	17	18	
			, {1822, 3, 3, 0}, {1822, 0, 0, 0}, {1822, 0, 2, 0}// 19	20	21	
			, {1822, -1, -1, 0}, {1822, -1, 0, 0}, {1822, -1, 1, 0}// 22	23	24	
			, {1822, -1, 2, 0}, {1823, -1, 4, 0}, {1823, 0, 4, 0}// 25	26	27	
			, {1823, 1, 4, 0}, {1823, 2, 4, 0}, {1823, 3, 4, 0}// 28	29	30	
			, {1846, 4, 2, 0}, {1846, 4, 1, 0}, {1846, 4, 0, 0}// 31	32	33	
			, {1846, 4, -1, 0}, {1846, 4, -2, 0}, {1847, 3, -3, 0}// 34	35	36	
			, {1847, 2, -3, 0}, {1847, 1, -3, 0}, {1847, 0, -3, 0}// 37	38	39	
			, {1847, -1, -3, 0}, {1866, -3, -3, 0}, {1867, 4, 4, 0}// 40	41	42	
			, {1868, 4, -3, 0}, {1869, -3, 4, 0}, {1865, -3, 2, 0}// 43	44	45	
			, {1865, -3, 1, 0}, {1865, -3, 0, 0}, {1865, -3, -1, 0}// 46	47	48	
			, {1865, -3, -2, 0}, {1823, -2, 4, 0}, {1822, -2, 1, 0}// 49	50	51	
			, {1822, 2, 0, 0}, {1822, 2, -2, 0}, {1822, 3, 0, 0}// 52	53	54	
			, {1822, 3, 2, 0}, {1822, 2, 2, 0}, {1822, 2, -1, 0}// 55	56	57	
			, {1822, 1, -2, 0}, {1822, 3, 1, 0}, {1822, 3, -2, 0}// 58	59	60	
			, {1822, -1, -2, 0}, {1822, 2, 1, 0}, {1822, 0, 1, 0}// 61	62	63	
			, {1822, 3, -1, 0}// 64	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new ChampsixAddonDeed();
			}
		}

		[ Constructable ]
		public ChampsixAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public ChampsixAddon( Serial serial ) : base( serial )
		{
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

	public class ChampsixAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new ChampsixAddon();
			}
		}

		[Constructable]
		public ChampsixAddonDeed()
		{
			Name = "Champsix";
		}

		public ChampsixAddonDeed( Serial serial ) : base( serial )
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