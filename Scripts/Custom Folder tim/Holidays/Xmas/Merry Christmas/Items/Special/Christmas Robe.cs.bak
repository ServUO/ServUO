using System;
using System.Collections;
using Server;

namespace Server.Items
{
	public class GhostCostume : BaseOuterTorso
	{
		private int m_OrigHue;

		[CommandProperty( AccessLevel.GameMaster )]
		public int OrigHue
		{
			get{ return m_OrigHue; }
			set{ m_OrigHue = value; InvalidateProperties(); }
		}

		[Constructable]
		public GhostCostume() : base( 0x2684 )
		{
			Hue = 1153;
			Name = "Halloween Ghost Costume";
			
			OrigHue = 33770;
		}

		public GhostCostume( Serial serial ) : base( serial )
		{
		}
		
		public override bool OnEquip( Mobile from )
		{
			from.SendMessage( "You put on your Halloween ghost costume." );
			
			m_OrigHue = from.Hue;
			from.Hue = 0x4001;
				
			return true;
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = ( Mobile ) parent;
				
				from.Hue = m_OrigHue;
				
				from.SendMessage( "You remove your ghost costume." );
			}

			base.OnRemoved( parent );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int ) 0 );
			
			writer.WriteEncodedInt( (int) m_OrigHue );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			
			switch ( version )
			{
				case 0:
				{
					m_OrigHue = reader.ReadEncodedInt();

					break;
				}
			}
		}
	}
}