
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
	public class MedCaseSouthAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {2719, 2, 0, 3}, {2724, 2, -1, 3}, {2840, 2, 1, 3}// 1	2	3	
			, {2833, 2, 1, 0}, {2836, 2, 0, 0}, {2835, 2, -1, 0}// 4	5	6	
			, {2720, 1, -1, 3}, {2720, 0, -1, 3}, {2719, -1, 0, 3}// 7	8	9	
			, {2720, 1, 1, 3}, {2720, 0, 1, 3}, {2723, -1, -1, 3}// 10	11	12	
			, {2725, -1, 1, 3}, {2832, -1, -1, 0}, {2838, -1, 0, 0}// 13	14	15	
			, {2839, 0, -1, 0}, {2839, 1, -1, 0}, {2837, 0, 1, 0}// 16	17	18	
			, {2837, 1, 1, 0}, {2831, 0, 0, 0}, {2831, 1, 0, 0}// 19	20	21	
			, {2834, -1, 1, 0}// 22	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new MedCaseSouthAddonDeed();
			}
		}

		[ Constructable ]
		public MedCaseSouthAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public MedCaseSouthAddon( Serial serial ) : base( serial )
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

	public class MedCaseSouthAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new MedCaseSouthAddon();
			}
		}

		[Constructable]
		public MedCaseSouthAddonDeed()
		{
			Name = "MedCaseSouth";
		}

		public MedCaseSouthAddonDeed( Serial serial ) : base( serial )
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