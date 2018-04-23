using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Prompts;

namespace Server.Gumps
{
	public class Tools_necro_scrollGump : Gump
	{
		private Tools_necro_scroll m_Scroll;

		public Tools_necro_scrollGump( Mobile from, Tools_necro_scroll scroll ) : base( 0, 0 )
		{
			m_Scroll = scroll;

			int mN01AnimateDeadSpell = m_Scroll.mN01AnimateDeadSpell;
			int mN02BloodOathSpell = m_Scroll.mN02BloodOathSpell;
			int mN03CorpseSkinSpell = m_Scroll.mN03CorpseSkinSpell;
			int mN04CurseWeaponSpell = m_Scroll.mN04CurseWeaponSpell;
			int mN05EvilOmenSpell = m_Scroll.mN05EvilOmenSpell;
			int mN06HorrificBeastSpell = m_Scroll.mN06HorrificBeastSpell;
			int mN07LichFormSpell = m_Scroll.mN07LichFormSpell;
			int mN08MindRotSpell = m_Scroll.mN08MindRotSpell;
			int mN09PainSpikeSpell = m_Scroll.mN09PainSpikeSpell;
			int mN10PoisonStrikeSpell = m_Scroll.mN10PoisonStrikeSpell;
			int mN11StrangleSpell = m_Scroll.mN11StrangleSpell;
			int mN12SummonFamiliarSpell = m_Scroll.mN12SummonFamiliarSpell;
			int mN13VampiricEmbraceSpell = m_Scroll.mN13VampiricEmbraceSpell;
			int mN14VengefulSpiritSpell = m_Scroll.mN14VengefulSpiritSpell;
			int mN15WitherSpell = m_Scroll.mN15WitherSpell;
			int mN16WraithFormSpell = m_Scroll.mN16WraithFormSpell;

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(52, 34, 160, 411, 9200);

			this.AddImage(60, 45, 20480);
			this.AddImage(60, 90, 20481);
			this.AddImage(60, 135, 20482);
			this.AddImage(60, 180, 20483);
			this.AddImage(60, 225, 20484);
			this.AddImage(60, 270, 20485);
			this.AddImage(60, 315, 20486);
			this.AddImage(60, 360, 20487);
			this.AddImage(142, 45, 20488);
			this.AddImage(142, 90, 20489);
			this.AddImage(142, 135, 20490);
			this.AddImage(142, 180, 20491);
			this.AddImage(142, 225, 20492);
			this.AddImage(142, 270, 20493);
			this.AddImage(142, 315, 20494);
			this.AddImage(142, 360, 20495);

			if ( mN01AnimateDeadSpell == 1 ) { this.AddButton( 110, 55,  2361, 2361, 1, GumpButtonType.Reply, 1); }
			if ( mN02BloodOathSpell == 1 ) { this.AddButton( 110, 100,  2361, 2361, 2, GumpButtonType.Reply, 1); }
			if ( mN03CorpseSkinSpell == 1 ) { this.AddButton( 110, 145,  2361, 2361, 3, GumpButtonType.Reply, 1); }
			if ( mN04CurseWeaponSpell == 1 ) { this.AddButton( 110, 190,  2361, 2361, 4, GumpButtonType.Reply, 1); }
			if ( mN05EvilOmenSpell == 1 ) { this.AddButton( 110, 235,  2361, 2361, 5, GumpButtonType.Reply, 1); }
			if ( mN06HorrificBeastSpell == 1 ) { this.AddButton( 110, 280,  2361, 2361, 6, GumpButtonType.Reply, 1); }
			if ( mN07LichFormSpell == 1 ) { this.AddButton( 110, 325,  2361, 2361, 7, GumpButtonType.Reply, 1); }
			if ( mN08MindRotSpell == 1 ) { this.AddButton( 110, 370,  2361, 2361, 8, GumpButtonType.Reply, 1); }
			if ( mN09PainSpikeSpell == 1 ) { this.AddButton( 192, 55,  2361, 2361, 9, GumpButtonType.Reply, 1); }
			if ( mN10PoisonStrikeSpell == 1 ) { this.AddButton( 192, 100,  2361, 2361, 10, GumpButtonType.Reply, 1); }
			if ( mN11StrangleSpell == 1 ) { this.AddButton( 192, 145,  2361, 2361, 11, GumpButtonType.Reply, 1); }
			if ( mN12SummonFamiliarSpell == 1 ) { this.AddButton( 192, 190,  2361, 2361, 12, GumpButtonType.Reply, 1); }
			if ( mN13VampiricEmbraceSpell == 1 ) { this.AddButton( 192, 235,  2361, 2361, 13, GumpButtonType.Reply, 1); }
			if ( mN14VengefulSpiritSpell == 1 ) { this.AddButton( 192, 280,  2361, 2361, 14, GumpButtonType.Reply, 1); }
			if ( mN15WitherSpell == 1 ) { this.AddButton( 192, 325,  2361, 2361, 15, GumpButtonType.Reply, 1); }
			if ( mN16WraithFormSpell == 1 ) { this.AddButton( 192, 370,  2361, 2361, 16, GumpButtonType.Reply, 1); }

			if ( mN01AnimateDeadSpell == 0 ) { this.AddButton( 110, 55,  2360, 2360, 1, GumpButtonType.Reply, 1); }
			if ( mN02BloodOathSpell == 0 ) { this.AddButton( 110, 100,  2360, 2360, 2, GumpButtonType.Reply, 1); }
			if ( mN03CorpseSkinSpell == 0 ) { this.AddButton( 110, 145,  2360, 2360, 3, GumpButtonType.Reply, 1); }
			if ( mN04CurseWeaponSpell == 0 ) { this.AddButton( 110, 190,  2360, 2360, 4, GumpButtonType.Reply, 1); }
			if ( mN05EvilOmenSpell == 0 ) { this.AddButton( 110, 235,  2360, 2360, 5, GumpButtonType.Reply, 1); }
			if ( mN06HorrificBeastSpell == 0 ) { this.AddButton( 110, 280,  2360, 2360, 6, GumpButtonType.Reply, 1); }
			if ( mN07LichFormSpell == 0 ) { this.AddButton( 110, 325,  2360, 2360, 7, GumpButtonType.Reply, 1); }
			if ( mN08MindRotSpell == 0 ) { this.AddButton( 110, 370,  2360, 2360, 8, GumpButtonType.Reply, 1); }
			if ( mN09PainSpikeSpell == 0 ) { this.AddButton( 192, 55,  2360, 2360, 9, GumpButtonType.Reply, 1); }
			if ( mN10PoisonStrikeSpell == 0 ) { this.AddButton( 192, 100,  2360, 2360, 10, GumpButtonType.Reply, 1); }
			if ( mN11StrangleSpell == 0 ) { this.AddButton( 192, 145,  2360, 2360, 11, GumpButtonType.Reply, 1); }
			if ( mN12SummonFamiliarSpell == 0 ) { this.AddButton( 192, 190,  2360, 2360, 12, GumpButtonType.Reply, 1); }
			if ( mN13VampiricEmbraceSpell == 0 ) { this.AddButton( 192, 235,  2360, 2360, 13, GumpButtonType.Reply, 1); }
			if ( mN14VengefulSpiritSpell == 0 ) { this.AddButton( 192, 280,  2360, 2360, 14, GumpButtonType.Reply, 1); }
			if ( mN15WitherSpell == 0 ) { this.AddButton( 192, 325,  2360, 2360, 15, GumpButtonType.Reply, 1); }
			if ( mN16WraithFormSpell == 0 ) { this.AddButton( 192, 370,  2360, 2360, 16, GumpButtonType.Reply, 1); }

			this.AddButton(149, 408, 2152, 2152, 17, GumpButtonType.Reply, 1); // TOOLBAR
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
			case 1 : { if ( m_Scroll.mN01AnimateDeadSpell == 0 ) { m_Scroll.mN01AnimateDeadSpell = 1; } else { m_Scroll.mN01AnimateDeadSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 2 : { if ( m_Scroll.mN02BloodOathSpell == 0 ) { m_Scroll.mN02BloodOathSpell = 1; } else { m_Scroll.mN02BloodOathSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 3 : { if ( m_Scroll.mN03CorpseSkinSpell == 0 ) { m_Scroll.mN03CorpseSkinSpell = 1; } else { m_Scroll.mN03CorpseSkinSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 4 : { if ( m_Scroll.mN04CurseWeaponSpell == 0 ) { m_Scroll.mN04CurseWeaponSpell = 1; } else { m_Scroll.mN04CurseWeaponSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 5 : { if ( m_Scroll.mN05EvilOmenSpell == 0 ) { m_Scroll.mN05EvilOmenSpell = 1; } else { m_Scroll.mN05EvilOmenSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 6 : { if ( m_Scroll.mN06HorrificBeastSpell == 0 ) { m_Scroll.mN06HorrificBeastSpell = 1; } else { m_Scroll.mN06HorrificBeastSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 7 : { if ( m_Scroll.mN07LichFormSpell == 0 ) { m_Scroll.mN07LichFormSpell = 1; } else { m_Scroll.mN07LichFormSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 8 : { if ( m_Scroll.mN08MindRotSpell == 0 ) { m_Scroll.mN08MindRotSpell = 1; } else { m_Scroll.mN08MindRotSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 9 : { if ( m_Scroll.mN09PainSpikeSpell == 0 ) { m_Scroll.mN09PainSpikeSpell = 1; } else { m_Scroll.mN09PainSpikeSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 10 : { if ( m_Scroll.mN10PoisonStrikeSpell == 0 ) { m_Scroll.mN10PoisonStrikeSpell = 1; } else { m_Scroll.mN10PoisonStrikeSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 11 : { if ( m_Scroll.mN11StrangleSpell == 0 ) { m_Scroll.mN11StrangleSpell = 1; } else { m_Scroll.mN11StrangleSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 12 : { if ( m_Scroll.mN12SummonFamiliarSpell == 0 ) { m_Scroll.mN12SummonFamiliarSpell = 1; } else { m_Scroll.mN12SummonFamiliarSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 13 : { if ( m_Scroll.mN13VampiricEmbraceSpell == 0 ) { m_Scroll.mN13VampiricEmbraceSpell = 1; } else { m_Scroll.mN13VampiricEmbraceSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 14 : { if ( m_Scroll.mN14VengefulSpiritSpell == 0 ) { m_Scroll.mN14VengefulSpiritSpell = 1; } else { m_Scroll.mN14VengefulSpiritSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 15 : { if ( m_Scroll.mN15WitherSpell == 0 ) { m_Scroll.mN15WitherSpell = 1; } else { m_Scroll.mN15WitherSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 16 : { if ( m_Scroll.mN16WraithFormSpell == 0 ) { m_Scroll.mN16WraithFormSpell = 1; } else { m_Scroll.mN16WraithFormSpell = 0; } from.SendGump( new Tools_necro_scrollGump( from, m_Scroll ) ); break; }
			case 17:
			{
				from.CloseGump( typeof( Tools_tools_necro ) );
				from.SendGump( new Tools_tools_necro( from, m_Scroll ) );
				break;
			}
		}
	}}

	public class Tools_tools_necro : Gump
	{
		public static bool HasSpell( Mobile from, int spellID )
		{
			Spellbook book = Spellbook.Find( from, spellID );
			return ( book != null && book.HasSpell( spellID ) );
		}

		private Tools_necro_scroll m_Scroll;

		public Tools_tools_necro( Mobile from, Tools_necro_scroll scroll ) : base( 0, 0 )
		{
			m_Scroll = scroll;
			this.Closable=false;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddImage(0, 0, 11011, 0);
			int dby = 50;

			if ( HasSpell( from, 100 ) && m_Scroll.mN01AnimateDeadSpell == 1){this.AddButton(dby, 5, 20480,20480, 1, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 101 ) && m_Scroll.mN02BloodOathSpell == 1){this.AddButton(dby, 5, 20481,20481, 2, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 102 ) && m_Scroll.mN03CorpseSkinSpell == 1){this.AddButton(dby, 5, 20482,20482, 3, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 103 ) && m_Scroll.mN04CurseWeaponSpell == 1){this.AddButton(dby, 5, 20483,20483, 4, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 104 ) && m_Scroll.mN05EvilOmenSpell == 1){this.AddButton(dby, 5, 20484,20484, 5, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 105 ) && m_Scroll.mN06HorrificBeastSpell == 1){this.AddButton(dby, 5, 20485,20485, 6, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 106 ) && m_Scroll.mN07LichFormSpell == 1){this.AddButton(dby, 5, 20486,20486, 7, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 107 ) && m_Scroll.mN08MindRotSpell == 1){this.AddButton(dby, 5, 20487,20487, 8, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 108 ) && m_Scroll.mN09PainSpikeSpell == 1){this.AddButton(dby, 5, 20488,20488, 9, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 109 ) && m_Scroll.mN10PoisonStrikeSpell == 1){this.AddButton(dby, 5, 20489,20489, 10, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 110 ) && m_Scroll.mN11StrangleSpell == 1){this.AddButton(dby, 5, 20490,20490, 11, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 111 ) && m_Scroll.mN12SummonFamiliarSpell == 1){this.AddButton(dby, 5, 20491,20491, 12, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 112 ) && m_Scroll.mN13VampiricEmbraceSpell == 1){this.AddButton(dby, 5, 20492,20492, 13, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 113 ) && m_Scroll.mN14VengefulSpiritSpell == 1){this.AddButton(dby, 5, 20493,20493, 14, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 114 ) && m_Scroll.mN15WitherSpell == 1){this.AddButton(dby, 5, 20494,20494, 15, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 115 ) && m_Scroll.mN16WraithFormSpell == 1){this.AddButton(dby, 5, 20495,20495, 16, GumpButtonType.Reply, 1); dby = dby + 45;}
		}
		
		public override void OnResponse( NetState state, RelayInfo info ) 
		{ 
			Mobile from = state.Mobile; 
			switch ( info.ButtonID ) 
			{
				case 0: { break; }
				case 1: { if ( HasSpell( from, 100 ) ) { new AnimateDeadSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 2: { if ( HasSpell( from, 101 ) ) { new BloodOathSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 3: { if ( HasSpell( from, 102 ) ) { new CorpseSkinSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 4: { if ( HasSpell( from, 103 ) ) { new CurseWeaponSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 5: { if ( HasSpell( from, 104 ) ) { new EvilOmenSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 6: { if ( HasSpell( from, 105 ) ) { new HorrificBeastSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 7: { if ( HasSpell( from, 106 ) ) { new LichFormSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 8: { if ( HasSpell( from, 107 ) ) { new MindRotSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 9: { if ( HasSpell( from, 108 ) ) { new PainSpikeSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 10: { if ( HasSpell( from, 109 ) ) { new PoisonStrikeSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 11: { if ( HasSpell( from, 110 ) ) { new StrangleSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 12: { if ( HasSpell( from, 111 ) ) { new SummonFamiliarSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 13: { if ( HasSpell( from, 112 ) ) { new VampiricEmbraceSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 14: { if ( HasSpell( from, 113 ) ) { new VengefulSpiritSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 15: { if ( HasSpell( from, 114 ) ) { new WitherSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
				case 16: { if ( HasSpell( from, 115 ) ) { new WraithFormSpell( from, null ).Cast(); from.SendGump( new Tools_tools_necro( from, m_Scroll ) ); } break; }
			}
		}
	}
}