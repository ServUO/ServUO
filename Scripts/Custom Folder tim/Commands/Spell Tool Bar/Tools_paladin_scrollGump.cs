using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Spells.Chivalry;
using Server.Prompts;

namespace Server.Gumps
{
	public class Tools_paladin_scrollGump : Gump
	{
		private Tools_paladin_scroll m_Scroll;

		public Tools_paladin_scrollGump( Mobile from, Tools_paladin_scroll scroll ) : base( 0, 0 )
		{
			m_Scroll = scroll;

			int mN01CleanseByFireSpell = m_Scroll.mN01CleanseByFireSpell;
			int mN02CloseWoundsSpell = m_Scroll.mN02CloseWoundsSpell;
			int mN03ConsecrateWeaponSpell = m_Scroll.mN03ConsecrateWeaponSpell;
			int mN04DispelEvilSpell = m_Scroll.mN04DispelEvilSpell;
			int mN05DivineFurySpell = m_Scroll.mN05DivineFurySpell;
			int mN06EnemyOfOneSpell = m_Scroll.mN06EnemyOfOneSpell;
			int mN07HolyLightSpell = m_Scroll.mN07HolyLightSpell;
			int mN08NobleSacrificeSpell = m_Scroll.mN08NobleSacrificeSpell;
			int mN09RemoveCurseSpell = m_Scroll.mN09RemoveCurseSpell;
			int mN10SacredJourneySpell = m_Scroll.mN10SacredJourneySpell;

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(52, 34, 160, 411, 9200);

			this.AddImage(60, 45, 20736);
			this.AddImage(60, 90, 20737);
			this.AddImage(60, 135, 20738);
			this.AddImage(60, 180, 20739);
			this.AddImage(60, 225, 20740);
			this.AddImage(60, 270, 20741);
			this.AddImage(60, 315, 20742);
			this.AddImage(60, 360, 20743);
			this.AddImage(142, 45, 20744);
			this.AddImage(142, 90, 20745);

			if ( mN01CleanseByFireSpell == 1 ) { this.AddButton( 110, 55,  2361, 2361, 1, GumpButtonType.Reply, 1); }
			if ( mN02CloseWoundsSpell == 1 ) { this.AddButton( 110, 100,  2361, 2361, 2, GumpButtonType.Reply, 1); }
			if ( mN03ConsecrateWeaponSpell == 1 ) { this.AddButton( 110, 145,  2361, 2361, 3, GumpButtonType.Reply, 1); }
			if ( mN04DispelEvilSpell == 1 ) { this.AddButton( 110, 190,  2361, 2361, 4, GumpButtonType.Reply, 1); }
			if ( mN05DivineFurySpell == 1 ) { this.AddButton( 110, 235,  2361, 2361, 5, GumpButtonType.Reply, 1); }
			if ( mN06EnemyOfOneSpell == 1 ) { this.AddButton( 110, 280,  2361, 2361, 6, GumpButtonType.Reply, 1); }
			if ( mN07HolyLightSpell == 1 ) { this.AddButton( 110, 325,  2361, 2361, 7, GumpButtonType.Reply, 1); }
			if ( mN08NobleSacrificeSpell == 1 ) { this.AddButton( 110, 370,  2361, 2361, 8, GumpButtonType.Reply, 1); }
			if ( mN09RemoveCurseSpell == 1 ) { this.AddButton( 192, 55,  2361, 2361, 9, GumpButtonType.Reply, 1); }
			if ( mN10SacredJourneySpell == 1 ) { this.AddButton( 192, 100,  2361, 2361, 10, GumpButtonType.Reply, 1); }

			if ( mN01CleanseByFireSpell == 0 ) { this.AddButton( 110, 55,  2360, 2360, 1, GumpButtonType.Reply, 1); }
			if ( mN02CloseWoundsSpell == 0 ) { this.AddButton( 110, 100,  2360, 2360, 2, GumpButtonType.Reply, 1); }
			if ( mN03ConsecrateWeaponSpell == 0 ) { this.AddButton( 110, 145,  2360, 2360, 3, GumpButtonType.Reply, 1); }
			if ( mN04DispelEvilSpell == 0 ) { this.AddButton( 110, 190,  2360, 2360, 4, GumpButtonType.Reply, 1); }
			if ( mN05DivineFurySpell == 0 ) { this.AddButton( 110, 235,  2360, 2360, 5, GumpButtonType.Reply, 1); }
			if ( mN06EnemyOfOneSpell == 0 ) { this.AddButton( 110, 280,  2360, 2360, 6, GumpButtonType.Reply, 1); }
			if ( mN07HolyLightSpell == 0 ) { this.AddButton( 110, 325,  2360, 2360, 7, GumpButtonType.Reply, 1); }
			if ( mN08NobleSacrificeSpell == 0 ) { this.AddButton( 110, 370,  2360, 2360, 8, GumpButtonType.Reply, 1); }
			if ( mN09RemoveCurseSpell == 0 ) { this.AddButton( 192, 55,  2360, 2360, 9, GumpButtonType.Reply, 1); }
			if ( mN10SacredJourneySpell == 0 ) { this.AddButton( 192, 100,  2360, 2360, 10, GumpButtonType.Reply, 1); }

			this.AddButton(149, 408, 2152, 2152, 11, GumpButtonType.Reply, 1); // TOOLBAR
			this.AddLabel(60, 412, 52, @"Open Toolbar");
		}

	public override void OnResponse( NetState state, RelayInfo info )
	{
		Mobile from = state.Mobile;

		switch ( info.ButtonID )
		{
			case 0:
			{
				break;
			}
			case 1 : { if ( m_Scroll.mN01CleanseByFireSpell == 0 ) { m_Scroll.mN01CleanseByFireSpell = 1; } else { m_Scroll.mN01CleanseByFireSpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 2 : { if ( m_Scroll.mN02CloseWoundsSpell == 0 ) { m_Scroll.mN02CloseWoundsSpell = 1; } else { m_Scroll.mN02CloseWoundsSpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 3 : { if ( m_Scroll.mN03ConsecrateWeaponSpell == 0 ) { m_Scroll.mN03ConsecrateWeaponSpell = 1; } else { m_Scroll.mN03ConsecrateWeaponSpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 4 : { if ( m_Scroll.mN04DispelEvilSpell == 0 ) { m_Scroll.mN04DispelEvilSpell = 1; } else { m_Scroll.mN04DispelEvilSpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 5 : { if ( m_Scroll.mN05DivineFurySpell == 0 ) { m_Scroll.mN05DivineFurySpell = 1; } else { m_Scroll.mN05DivineFurySpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 6 : { if ( m_Scroll.mN06EnemyOfOneSpell == 0 ) { m_Scroll.mN06EnemyOfOneSpell = 1; } else { m_Scroll.mN06EnemyOfOneSpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 7 : { if ( m_Scroll.mN07HolyLightSpell == 0 ) { m_Scroll.mN07HolyLightSpell = 1; } else { m_Scroll.mN07HolyLightSpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 8 : { if ( m_Scroll.mN08NobleSacrificeSpell == 0 ) { m_Scroll.mN08NobleSacrificeSpell = 1; } else { m_Scroll.mN08NobleSacrificeSpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 9 : { if ( m_Scroll.mN09RemoveCurseSpell == 0 ) { m_Scroll.mN09RemoveCurseSpell = 1; } else { m_Scroll.mN09RemoveCurseSpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 10 : { if ( m_Scroll.mN10SacredJourneySpell == 0 ) { m_Scroll.mN10SacredJourneySpell = 1; } else { m_Scroll.mN10SacredJourneySpell = 0; } from.SendGump( new Tools_paladin_scrollGump( from, m_Scroll ) ); break; }
			case 11:
			{
				from.CloseGump( typeof( Tools_tools_paladin ) );
				from.SendGump( new Tools_tools_paladin( from, m_Scroll ) );
				break;
			}
		}
	}}

	public class Tools_tools_paladin : Gump
	{
		public static bool HasSpell( Mobile from, int spellID )
		{
			Spellbook book = Spellbook.Find( from, spellID );
			return ( book != null && book.HasSpell( spellID ) );
		}

		private Tools_paladin_scroll m_Scroll;

		public Tools_tools_paladin( Mobile from, Tools_paladin_scroll scroll ) : base( 0, 0 )
		{
			m_Scroll = scroll;
			this.Closable=false;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddImage(0, 0, 11012, 1149);
			int dby = 50;

			if ( HasSpell( from, 200 ) && m_Scroll.mN01CleanseByFireSpell == 1){this.AddButton(dby, 5, 20736, 20736, 1, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 201 ) && m_Scroll.mN02CloseWoundsSpell == 1){this.AddButton(dby, 5, 20737, 20737, 2, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 202 ) && m_Scroll.mN03ConsecrateWeaponSpell == 1){this.AddButton(dby, 5, 20738, 20738, 3, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 203 ) && m_Scroll.mN04DispelEvilSpell == 1){this.AddButton(dby, 5, 20739, 20739, 4, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 204 ) && m_Scroll.mN05DivineFurySpell == 1){this.AddButton(dby, 5, 20740, 20740, 5, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 205 ) && m_Scroll.mN06EnemyOfOneSpell == 1){this.AddButton(dby, 5, 20741, 20741, 6, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 206 ) && m_Scroll.mN07HolyLightSpell == 1){this.AddButton(dby, 5, 20742, 20742, 7, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 207 ) && m_Scroll.mN08NobleSacrificeSpell == 1){this.AddButton(dby, 5, 20743, 20743, 8, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 208 ) && m_Scroll.mN09RemoveCurseSpell == 1){this.AddButton(dby, 5, 20744, 20744, 9, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 209 ) && m_Scroll.mN10SacredJourneySpell == 1){this.AddButton(dby, 5, 20745, 20745, 10, GumpButtonType.Reply, 1); dby = dby + 45;}
		}
		
		public override void OnResponse( NetState state, RelayInfo info ) 
		{ 
			Mobile from = state.Mobile; 
			switch ( info.ButtonID ) 
			{
				case 0: { break; }
				case 1 : { if ( HasSpell( from, 200 ) ) { new CleanseByFireSpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
				case 2 : { if ( HasSpell( from, 201 ) ) { new CloseWoundsSpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
				case 3 : { if ( HasSpell( from, 202 ) ) { new ConsecrateWeaponSpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
				case 4 : { if ( HasSpell( from, 203 ) ) { new DispelEvilSpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
				case 5 : { if ( HasSpell( from, 204 ) ) { new DivineFurySpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
				case 6 : { if ( HasSpell( from, 205 ) ) { new EnemyOfOneSpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
				case 7 : { if ( HasSpell( from, 206 ) ) { new HolyLightSpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
				case 8 : { if ( HasSpell( from, 207 ) ) { new NobleSacrificeSpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
				case 9 : { if ( HasSpell( from, 208 ) ) { new RemoveCurseSpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
				case 10 : { if ( HasSpell( from, 209 ) ) { new SacredJourneySpell( from, null ).Cast(); from.SendGump( new Tools_tools_paladin( from, m_Scroll ) ); } break; }
			}
		}
	}
}