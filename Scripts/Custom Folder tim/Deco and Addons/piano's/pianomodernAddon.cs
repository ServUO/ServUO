//Lagatha

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class pianomodernAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {25532, 1, 1, 0}, {25533, 0, 1, 0}, {25538, 1, -1, 0}// 1	2	3	
			, {25537, 1, -1, 0}, {25536, 0, -1, 0}, {25535, 1, 0, 0}// 4	5	6	
			, {25534, 0, 0, 0}// 7	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new pianomodernAddonDeed();
			}
		}

		[ Constructable ]
		public pianomodernAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public pianomodernAddon( Serial serial ) : base( serial )
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

	public class pianomodernAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new pianomodernAddon();
			}
		}

		[Constructable]
		public pianomodernAddonDeed()
		{
			Name = "pianomodern";
		}

		public pianomodernAddonDeed( Serial serial ) : base( serial )
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