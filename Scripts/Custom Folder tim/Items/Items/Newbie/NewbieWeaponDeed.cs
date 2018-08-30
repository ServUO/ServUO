/**************************************
*Script Name: Newbie Weapon Pack      *
*Author: Poseidon AKA Vermund         *
*Weapons By: Gizmo's Uo Quest Maker   *
*For use with RunUO 2.2               *
**************************************/
using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public class NewbieWeaponDeed : Item
	{
		[Constructable]
		public NewbieWeaponDeed() : base( 0x14F0 )
		{
			Name = "Newbie Weapon Deed";
			Weight = 1.0;
			Hue = 88;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else from.SendGump( new InternalGump( from, this ) );
		}

		private class InternalGump : Gump
		{
			private Mobile m_From;
			private NewbieWeaponDeed m_Deed;

			public InternalGump( Mobile from, NewbieWeaponDeed deed ) : base( 50, 50 )
			{
				m_From = from;
				m_Deed = deed;

				from.CloseGump( typeof( InternalGump ) );

				AddPage ( 0 );
				AddBackground( 10, 10, 465, 405, 0xA28 );

				AddImage(442,35, 10441);

				AddPage ( 1 );

				AddLabel( 120, 25, 0x34, "Select the Type of Weapon you Prefer.");

				AddLabel( 75,  55, 59, "Newbie Axes");
				AddLabel( 75,  85, 59, "Newbie Bows");
				AddLabel( 75, 115, 59, "Newbie Knives");
				AddLabel( 75, 145, 59, "Newbie Maces");
				AddLabel( 75, 175, 59, "Newbie Pole Arms");
				AddLabel( 75, 205, 59, "Newbie Spears and Forks");
				AddLabel( 75, 235, 59, "Newbie Staves");
				AddLabel( 75, 265, 59, "Newbie Swords");

				AddButton( 40,  58, 0x2623, 0x2622, 1, GumpButtonType.Page, 2 );
				AddButton( 40,  88, 0x2623, 0x2622, 2, GumpButtonType.Page, 3 );
				AddButton( 40, 118, 0x2623, 0x2622, 3, GumpButtonType.Page, 4 );
				AddButton( 40, 148, 0x2623, 0x2622, 4, GumpButtonType.Page, 5 );
				AddButton( 40, 178, 0x2623, 0x2622, 5, GumpButtonType.Page, 6 );
				AddButton( 40, 208, 0x2623, 0x2622, 6, GumpButtonType.Page, 7 );
				AddButton( 40, 238, 0x2623, 0x2622, 7, GumpButtonType.Page, 8 );
				AddButton( 40, 268, 0x2623, 0x2622, 8, GumpButtonType.Page, 9 );

				AddPage ( 2 );

				AddLabel( 160, 25, 0x34, "Select the Axe you Desire.");

				AddLabel( 75,  55, 59, "Newbie Axe");
				AddLabel( 75,  85, 59, "Newbie Battle Axe");
				AddLabel( 75, 115, 59, "Newbie Double Axe");
				AddLabel( 75, 145, 59, "Newbie Executioner's Axe");
				AddLabel( 75, 175, 59, "Newbie Daisho");
				AddLabel( 75, 205, 59, "Newbie Large Battle Axe");
				AddLabel( 75, 235, 59, "Newbie Sai");
				AddLabel( 75, 265, 59, "Newbie Two Handed Axe");
				AddLabel( 75, 295, 59, "Newbie War Axe");

				AddButton( 40,  58, 0x2623, 0x2622, 1, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 2, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 3, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 4, GumpButtonType.Reply, 0 );
				AddButton( 40, 178, 0x2623, 0x2622, 5, GumpButtonType.Reply, 0 );
				AddButton( 40, 208, 0x2623, 0x2622, 6, GumpButtonType.Reply, 0 );
				AddButton( 40, 238, 0x2623, 0x2622, 7, GumpButtonType.Reply, 0 );
				AddButton( 40, 268, 0x2623, 0x2622, 8, GumpButtonType.Reply, 0 );
				AddButton( 40, 298, 0x2623, 0x2622, 9, GumpButtonType.Reply, 0 );

				AddPage ( 3 );

				AddLabel( 160, 25, 0x34, "Select the Bow you Desire.");

				AddLabel( 75,  55, 59, "Newbie Bow");
				AddLabel( 75,  85, 59, "Newbie Composite Bow");
				AddLabel( 75, 115, 59, "Newbie Crossbow");
				AddLabel( 75, 145, 59, "Newbie Heavy Crossbow");
				AddLabel( 75, 175, 59, "Newbie Repeating Crossbow");

				AddButton( 40,  58, 0x2623, 0x2622, 10, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 11, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 12, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 13, GumpButtonType.Reply, 0 );
				AddButton( 40, 178, 0x2623, 0x2622, 14, GumpButtonType.Reply, 0 );

				AddPage ( 4 );

				AddLabel( 160, 25, 0x34, "Select the Knife you Desire.");

				AddLabel( 75,  55, 59, "Newbie Butcher Knife");
				AddLabel( 75,  85, 59, "Newbie Cleaver");
				AddLabel( 75, 115, 59, "Newbie Dagger");
				AddLabel( 75, 145, 59, "Newbie Skinning Knife");

				AddButton( 40,  58, 0x2623, 0x2622, 15, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 16, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 17, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 18, GumpButtonType.Reply, 0 );

				AddPage ( 5 );

				AddLabel( 160, 25, 0x34, "Select the Mace you Desire.");

				AddLabel( 75,  55, 59, "Newbie Diamond Mace");
				AddLabel( 75,  85, 59, "Newbie Hammer Pick");
				AddLabel( 75, 115, 59, "Newbie Mace");
				AddLabel( 75, 145, 59, "Newbie Maul");
				AddLabel( 75, 175, 59, "Newbie Scepter");
				AddLabel( 75, 205, 59, "Newbie War Hammer");
				AddLabel( 75, 235, 59, "Newbie War Mace");

				AddButton( 40,  58, 0x2623, 0x2622, 19, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 20, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 21, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 22, GumpButtonType.Reply, 0 );
				AddButton( 40, 178, 0x2623, 0x2622, 23, GumpButtonType.Reply, 0 );
				AddButton( 40, 208, 0x2623, 0x2622, 24, GumpButtonType.Reply, 0 );
				AddButton( 40, 238, 0x2623, 0x2622, 25, GumpButtonType.Reply, 0 );

				AddPage ( 6 );

				AddLabel( 140, 25, 0x34, "Select the Pole Arm you Desire.");

				AddLabel( 75,  55, 59, "Newbie Bardiche");
				AddLabel( 75,  85, 59, "Newbie Halberd");
				AddLabel( 75, 115, 59, "Newbie Scythe");

				AddButton( 40,  58, 0x2623, 0x2622, 26, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 27, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 28, GumpButtonType.Reply, 0 );

				AddPage ( 7 );

				AddLabel( 130, 25, 0x34, "Select the Spear or Fork you Desire.");

				AddLabel( 75,  55, 59, "Newbie Bladed Staff");
				AddLabel( 75,  85, 59, "Newbie Double Bladed Staff");
				AddLabel( 75, 115, 59, "Newbie Pike");
				AddLabel( 75, 145, 59, "Newbie Pitchfork");
				AddLabel( 75, 175, 59, "Newbie Short Spear");
				AddLabel( 75, 205, 59, "Newbie Tessen");
				AddLabel( 75, 235, 59, "Newbie Tetsubo");

				AddButton( 40,  58, 0x2623, 0x2622, 29, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 30, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 31, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 32, GumpButtonType.Reply, 0 );
				AddButton( 40, 178, 0x2623, 0x2622, 33, GumpButtonType.Reply, 0 );
				AddButton( 40, 208, 0x2623, 0x2622, 34, GumpButtonType.Reply, 0 );
				AddButton( 40, 238, 0x2623, 0x2622, 35, GumpButtonType.Reply, 0 );

				AddPage ( 8 );

				AddLabel( 160, 25, 0x34, "Select the Staff you Desire.");

				AddLabel( 75,  55, 59, "Newbie Wakizashi");
				AddLabel( 75,  85, 59, "Newbie Gnarled Staff");
				AddLabel( 75, 115, 59, "Newbie Quarter Staff");
				AddLabel( 75, 145, 59, "Newbie Shepherd's Crook");

				AddButton( 40,  58, 0x2623, 0x2622, 36, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 37, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 38, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 39, GumpButtonType.Reply, 0 );

				AddPage ( 9 );

				AddLabel( 160, 25, 0x34, "Select the Sword you Desire.");

				AddLabel( 75,  55, 59, "Newbie Bone Harvester");
				AddLabel( 75,  85, 59, "Newbie Broad Sword");
				AddLabel( 75, 115, 59, "Newbie Elven Spellblade");
				AddLabel( 75, 145, 59, "Newbie Cutlass");
				AddLabel( 75, 175, 59, "Newbie Katana");
				AddLabel( 75, 205, 59, "Newbie Kryss");
				AddLabel( 75, 235, 59, "Newbie Lance");
				AddLabel( 75, 265, 59, "Newbie Long Sword");
				AddLabel( 75, 295, 59, "Newbie Scimitar");
				AddLabel( 75, 325, 59, "Newbie Viking Sword");

				AddButton( 40,  58, 0x2623, 0x2622, 40, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 41, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 42, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 43, GumpButtonType.Reply, 0 );
				AddButton( 40, 178, 0x2623, 0x2622, 44, GumpButtonType.Reply, 0 );
				AddButton( 40, 208, 0x2623, 0x2622, 45, GumpButtonType.Reply, 0 );
				AddButton( 40, 238, 0x2623, 0x2622, 46, GumpButtonType.Reply, 0 );
				AddButton( 40, 268, 0x2623, 0x2622, 47, GumpButtonType.Reply, 0 );
				AddButton( 40, 298, 0x2623, 0x2622, 48, GumpButtonType.Reply, 0 );
				AddButton( 40, 328, 0x2623, 0x2622, 49, GumpButtonType.Reply, 0 );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted ) return;

				BaseWeapon weapon = null;

				switch ( info.ButtonID )
				{
					case 0: return;
					case 1: weapon = new NewbieAxe() ; break;
					case 2: weapon = new NewbieBattleAxe() ; break;
					case 3: weapon = new NewbieDoubleAxe() ; break;
					case 4: weapon = new NewbieExecutionersAxe() ; break;
					case 5: weapon = new NewbieDaisho() ; break;
					case 6: weapon = new NewbieLargeBattleAxe() ; break;
					case 7: weapon = new NewbieSai() ; break;
					case 8: weapon = new NewbieTwoHandedAxe() ; break;
					case 9: weapon = new NewbieWarAxe() ; break;
					case 10: weapon = new NewbieBow() ; break;
					case 11: weapon = new NewbieCompositeBow() ; break;
					case 12: weapon = new NewbieCrossbow() ; break;
					case 13: weapon = new NewbieHeavyCrossbow() ; break;
					case 14: weapon = new NewbieRepeatingCrossbow() ; break;
					case 15: weapon = new NewbieButcher() ; break;
					case 16: weapon = new NewbieCleaver() ; break;
					case 17: weapon = new NewbieDagger() ; break;
					case 18: weapon = new NewbieSkinningKnife() ; break;
					case 19: weapon = new NewbieDiamondMace() ; break;
					case 20: weapon = new NewbieHammerPick() ; break;
					case 21: weapon = new NewbieMace() ; break;
					case 22: weapon = new NewbieMaul() ; break;
					case 23: weapon = new NewbieScepter() ; break;
					case 24: weapon = new NewbieWarHammer() ; break;
					case 25: weapon = new NewbieWarMace() ; break;
					case 26: weapon = new NewbieBardiche() ; break;
					case 27: weapon = new NewbieHalberd() ; break;
					case 28: weapon = new NewbieScythe() ; break;
					case 29: weapon = new NewbieBladedStaff() ; break;
					case 30: weapon = new NewbieDoubleBladedStaff() ; break;
					case 31: weapon = new NewbiePike() ; break;
					case 32: weapon = new NewbiePitchfork() ; break;
					case 33: weapon = new NewbieShortSpear() ; break;
					case 34: weapon = new NewbieTessen() ; break;
					case 35: weapon = new NewbieTetsubo() ; break;
					case 36: weapon = new NewbieWakizashi() ; break;
					case 37: weapon = new NewbieGnarledStaff() ; break;
					case 38: weapon = new NewbieQuarterStaff() ; break;
					case 39: weapon = new NewbieShepherdsCrook() ; break;
					case 40: weapon = new NewbieHarvester() ; break;
					case 41: weapon = new NewbieBroadsword() ; break;
					case 42: weapon = new NewbieElvenSpellblade() ; break;
					case 43: weapon = new NewbieCutlass() ; break;
					case 44: weapon = new NewbieKatana() ; break;
					case 45: weapon = new NewbieKryss() ; break;
					case 46: weapon = new NewbieLance() ; break;
					case 47: weapon = new NewbieLongsword() ; break;
					case 48: weapon = new NewbieScimitar() ; break;
					case 49: weapon = new NewbieVikingSword() ; break;
					case 50: weapon = new NewbieNoDachi() ; break;
				}

				if ( weapon != null )
				{
					
					m_From.Backpack.DropItem( weapon );
					m_From.SendMessage( "You summon the Newbie Weapon!" );
					m_Deed.Delete();
				}
			}
		}

		public NewbieWeaponDeed( Serial serial ) : base( serial ) { }
		public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}
}