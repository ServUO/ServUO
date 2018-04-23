/***************************
**    All Dye Tubs				**
**     X-SirSly-X					**
** www.LandofObsidian.com **
**       ver 1.0					**
**      2006.04.12				**
***************************/
using Server;
using System;
using Server.Items;
using Server.Multis;
using Server.Targeting;
using Server.Mobiles;
using Server.AllHues;

namespace Server.Items
{
	public class AllDyeTubsArmorChain : DyeTub
	{
		private int i_charges;
		private int TheHue = Utility.RandomDyedHue();
//		private int TheHue = AllHuesInfo.Reds;
		private int m_DyedHue;
		private bool m_Redyable = false;
		private bool m_Charged = true;
		private bool m_AllowPack = true;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowPack
		{
			get	{return m_AllowPack;}
			set	{m_AllowPack = value;}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Charged
		{
			get	{return m_Charged;}
			set	{m_Charged = value;}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get { return i_charges; }
			set { i_charges = value; InvalidateProperties(); }
		}

		[Constructable]
		public AllDyeTubsArmorChain(  )
		{
			Name = "Dye Tub [Chain Armor]";
			Weight = 5.0;
			Hue = TheHue;
			DyedHue = TheHue;
			Charges = Utility.RandomMinMax(3,5);
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( Charged )
			{
				list.Add( 1060658, "Uses Remaining \t{0}", this.Charges );
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( this.IsChildOf( from.Backpack ) )
			{
				DoPack( from );
			}
			else
			{
				DoOut( from );
			}
		}

		public void DoPack( Mobile from )
		{
			if( AllowPack )
			{
				DoOut( from );
			}
			else
			{
				from.SendMessage("The dyetub cannot be in your pack.");
			}
		}

		public void DoOut ( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				from.SendMessage( "Select the item to dye" );
				from.Target = new AllDyeTubsArmorChainTarget( this );
			}
			else
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
		}

		public AllDyeTubsArmorChain( Serial serial ) : base( serial ){}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_DyedHue);
			writer.Write( (int) i_charges );

			writer.Write( (bool) m_Redyable );
			writer.Write( (bool) m_Charged );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_DyedHue = reader.ReadInt();
			i_charges = reader.ReadInt();

			m_Redyable = reader.ReadBool();
			m_Charged = reader.ReadBool();
		}

		public class AllDyeTubsArmorChainTarget : Target
		{
			private AllDyeTubsArmorChain m_Tub;

			public AllDyeTubsArmorChainTarget( AllDyeTubsArmorChain dyetub ) : base( 12, false, TargetFlags.None )
			{
				m_Tub = dyetub;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					Item item = (Item) targeted;

					if( item is BaseArmor )
					{
            BaseArmor armor = targeted as BaseArmor; 
						if( !item.IsChildOf(from.Backpack) )
						{
							from.SendMessage("The item must be in your pack.");
						}	

						else if ( armor.MaterialType == ArmorMaterialType.Chainmail )
						{
							item.Hue = m_Tub.DyedHue;

							if (m_Tub.Charged)
							{
								if ( m_Tub.Charges <= 1 )
								{
									m_Tub.Delete();
								}
								m_Tub.Charges = m_Tub.Charges - 1;
							}
							from.PlaySound( 0x23F );
						}
					}
					else
					{
						from.SendMessage("That item cannot be dyed.");
					}
				}
				else
				{
					from.SendMessage("You cannot dye that.");
				}
			}
		}
	}
}