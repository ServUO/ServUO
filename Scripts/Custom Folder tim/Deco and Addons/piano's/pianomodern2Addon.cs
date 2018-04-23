//Lagatha

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class pianomodern2Addon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {25545, -1, 0, 0}, {25544, 0, 0, 0}, {25543, 0, 1, 0}// 1	2	3	
			, {25542, 1, 0, 0}, {25541, 1, 1, 0}, {25540, 2, 1, 0}// 4	5	6	
			, {25539, 2, 0, 0}// 7	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new pianomodern2AddonDeed();
			}
		}

		[ Constructable ]
		public pianomodern2Addon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public pianomodern2Addon( Serial serial ) : base( serial )
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

	public class pianomodern2AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new pianomodern2Addon();
			}
		}

		[Constructable]
		public pianomodern2AddonDeed()
		{
			Name = "pianomodern2";
		}

		public pianomodern2AddonDeed( Serial serial ) : base( serial )
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