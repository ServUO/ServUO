/*
 * Created by SharpDevelop.
 * User: Sharon
 * Date: 12/4/2007
 * Time: 7:21 AM
 * http://www.shazzyshard.org
 * Santa Claus Quest 2007
 */
 
 using System; 
 using Server;

namespace Server.Items
{
	
	[FlipableAttribute( 0x2FC4, 0x317A )]
	public class SantasElfBoots : BaseShoes, IArcaneEquip
	{
		public override Race RequiredRace { get { return Race.Elf; } }
		#region Arcane Impl
		private int m_MaxArcaneCharges, m_CurArcaneCharges;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxArcaneCharges
		{
			get{ return m_MaxArcaneCharges; }
			set{ m_MaxArcaneCharges = value; InvalidateProperties(); Update(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int CurArcaneCharges
		{
			get{ return m_CurArcaneCharges; }
			set{ m_CurArcaneCharges = value; InvalidateProperties(); Update(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsArcane
		{
			get{ return ( m_MaxArcaneCharges > 0 && m_CurArcaneCharges >= 0 ); }
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( IsArcane )
				LabelTo( from, 1061837, String.Format( "{0}\t{1}", m_CurArcaneCharges, m_MaxArcaneCharges ) );
		}

		public void Update()
		{
			if ( IsArcane )
				ItemID = 0x317A;
			else if ( ItemID == 0x317A )
				ItemID = 0x2FCA;

			if ( IsArcane && CurArcaneCharges == 0 )
				Hue =  1175;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( IsArcane )
				list.Add( 1061837, "{0}\t{1}", m_CurArcaneCharges, m_MaxArcaneCharges ); // arcane charges: ~1_val~ / ~2_val~
		}

		public void Flip()
		{
			if ( ItemID == 0x2FCA )
				ItemID = 0x317A;
			else if ( ItemID == 0x317A )
				ItemID = 0x2FCA;
		}
		#endregion

		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		[Constructable]
		public SantasElfBoots() : this( 0 )
		{
		}

		[Constructable]
		public SantasElfBoots( int hue ) : base( 0x2FCA, hue )
		{
            Name = "Santa's Elf Boots";
			Weight = 4.0;
			Hue =  1175;
			MaxArcaneCharges= 25;
			CurArcaneCharges = 25;
			Attributes.BonusDex = 3;
		}

		public SantasElfBoots( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			if ( IsArcane )
			{
				writer.Write( true );
				writer.Write( (int) m_CurArcaneCharges );
				writer.Write( (int) m_MaxArcaneCharges );
			}
			else
			{
				writer.Write( false );
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					if ( reader.ReadBool() )
					{
						m_CurArcaneCharges = reader.ReadInt();
						m_MaxArcaneCharges = reader.ReadInt();

						if ( Hue == 2118 )
							Hue = ArcaneGem.DefaultArcaneHue;
					}

					break;
				}
			}
		}
	}
}
