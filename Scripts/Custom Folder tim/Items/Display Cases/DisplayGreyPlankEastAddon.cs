
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
	public class DisplayGreyPlankEastAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1999, 1, -1, 5}, {1999, 1, 1, 5}, {1998, 1, 0, 5}// 1	2	3	
			, {1822, 1, 1, 0}, {1822, 1, 0, 0}, {1822, 1, -1, 0}// 4	5	6	
			, {2000, 0, -1, 10}, {1999, 0, 0, 10}, {1998, 0, 1, 10}// 7	8	9	
			, {1822, 0, 1, 5}, {1822, 0, 0, 5}, {1822, 0, -1, 5}// 10	11	12	
			, {1822, 0, 1, 0}, {1822, 0, 0, 0}, {1822, 0, -1, 0}// 13	14	15	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new DisplayGreyPlankEastAddonDeed();
			}
		}

		[ Constructable ]
		public DisplayGreyPlankEastAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public DisplayGreyPlankEastAddon( Serial serial ) : base( serial )
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

	public class DisplayGreyPlankEastAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new DisplayGreyPlankEastAddon();
			}
		}

		[Constructable]
		public DisplayGreyPlankEastAddonDeed()
		{
			Name = "DisplayGreyPlankEast";
		}

		public DisplayGreyPlankEastAddonDeed( Serial serial ) : base( serial )
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