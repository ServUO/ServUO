#region AuthorHeader
//
//	Shrink System version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.ShrinkSystem
{
	public class PetLeash : Item, IShrinkTool
	{	
		private int m_Charges = ShrinkConfig.ShrinkCharges;

		[CommandProperty( AccessLevel.GameMaster )]
		public int ShrinkCharges
		{
			get { return m_Charges; }
			set
			{
				if ( 0 == m_Charges || 0 == (m_Charges = value ))
					Delete();
				else
					InvalidateProperties();
			}
		}

		[Constructable]
		public PetLeash() : base( 0x1374 )
		{
			Weight = 1.0;
			Movable = true;
			Name = "Pet Leash";
			LootType = ( ShrinkConfig.BlessedLeash ? LootType.Blessed : LootType.Regular );
		}

		public PetLeash( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );

			if ( m_Charges >= 0 )
				list.Add( 1060658, "Charges\t{0}", m_Charges.ToString() );
		}

		public override void OnDoubleClick( Mobile from )
		{
			bool isStaff = from.AccessLevel != AccessLevel.Player;

			if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.

			else if ( isStaff || from.Skills[ SkillName.AnimalTaming ].Value >= ShrinkConfig.TamingRequired )
				from.Target = new ShrinkTarget( from, this, isStaff );
			else
				from.SendMessage( "You must have at least " + ShrinkConfig.TamingRequired + " animal taming to use a pet leash." );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Charges = reader.ReadInt();
		}
	}
}
