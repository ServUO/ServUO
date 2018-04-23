using System;
using System.Collections;
using Server.Multis;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class ChristmasBag : BaseContainer, IDyable
	{
		private int m_Uses;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Uses
		{
			get{ return m_Uses; }
			set{ m_Uses = value; InvalidateProperties(); }
		}
		
		public override int DefaultGumpID{ get{ return 0x3D; } }
		public override int DefaultDropSound{ get{ return 0x48; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 29, 34, 108, 94 ); }
		}

		[Constructable]
		public ChristmasBag() : base( 0xE76 )
		{
			Weight = 2.0;
			Name = "a Christmas Goodie bag";
			Hue = 1153;
			
			Uses = 30;
			
			AddItem( new ChristmasBook() );
		}

		public ChristmasBag( Serial serial ) : base( serial )
		{
		}
		
		public override void AddNameProperties( ObjectPropertyList list )
 		{
 			base.AddNameProperties( list );
 			list.Add( 1060658, "Uses remaining\t{0}", m_Uses.ToString() );
 		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted ) return false;

			Hue = sender.DyedHue;

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			
			writer.Write( (int) m_Uses );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			switch ( version )
			{
				case 0:
				{
					m_Uses = (int)reader.ReadInt();

					break;
				}
			}
		}
		
		public bool ConsumeUse( Mobile from )
		{
			--Uses;
			
			if ( Uses == 0 )
			{
				from.SendMessage( "This bag cannot receive any more goodies!" );
				
				return false;
			}
			
			return true;
		}
	}
}