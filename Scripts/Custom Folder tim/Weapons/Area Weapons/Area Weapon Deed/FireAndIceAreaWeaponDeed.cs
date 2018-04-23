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
	public class FireAndIceAreaWeaponDeed : BaseSword
	{
		[Constructable]
		public FireAndIceAreaWeaponDeed() : base( 0x227B )
		{
			Name = "Fire & Ice Area Weapon Deed";
			Weight = 1.0;
			Hue = 2100;
			AosElementDamages.Fire = 25;
			AosElementDamages.Cold = 25;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else from.SendGump( new InternalGump( from, this ) );
		}

		private class InternalGump : Gump
		{
			private Mobile m_From;
			private FireAndIceAreaWeaponDeed m_Deed;

			public InternalGump( Mobile from, FireAndIceAreaWeaponDeed deed ) : base( 50, 50 )
			{
				m_From = from;
				m_Deed = deed;

				from.CloseGump( typeof( InternalGump ) );

				AddPage ( 0 );
				AddBackground( 10, 10, 465, 405, 0xA28 );

				AddImage(442,35, 10441);

				AddPage ( 1 );

				AddLabel( 120, 25, 0x34, "Select the Type of Weapon you Prefer.");

				AddLabel( 75,  55, 59, "Area Axes");
				AddLabel( 75,  85, 59, "Area Bows");
				AddLabel( 75, 115, 59, "Area Knives");
				AddLabel( 75, 145, 59, "Area Maces");
				AddLabel( 75, 175, 59, "Area Pole Arms");
				AddLabel( 75, 205, 59, "Area Spears and Forks");
				AddLabel( 75, 235, 59, "Area Staves");
				AddLabel( 75, 265, 59, "Area Swords");

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

				AddLabel( 75,  55, 59, "Area Axe");
				AddLabel( 75,  85, 59, "Area Battle Axe");
				AddLabel( 75, 115, 59, "Area Double Axe");
				AddLabel( 75, 145, 59, "Area Executioner's Axe");
				AddLabel( 75, 175, 59, "Area Hatchet");
				AddLabel( 75, 205, 59, "Area Large Battle Axe");
				AddLabel( 75, 235, 59, "Area Pickaxe");
				AddLabel( 75, 265, 59, "Area Two Handed Axe");
				AddLabel( 75, 295, 59, "Area War Axe");

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

				AddLabel( 75,  55, 59, "Area Bow");
				AddLabel( 75,  85, 59, "Area Composite Bow");
				AddLabel( 75, 115, 59, "Area Crossbow");
				AddLabel( 75, 145, 59, "Area Heavy Crossbow");
				AddLabel( 75, 175, 59, "Area Repeating Crossbow");

				AddButton( 40,  58, 0x2623, 0x2622, 10, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 11, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 12, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 13, GumpButtonType.Reply, 0 );
				AddButton( 40, 178, 0x2623, 0x2622, 14, GumpButtonType.Reply, 0 );

				AddPage ( 4 );

				AddLabel( 160, 25, 0x34, "Select the Knife you Desire.");

				AddLabel( 75,  55, 59, "Area Butcher Knife");
				AddLabel( 75,  85, 59, "Area Cleaver");
				AddLabel( 75, 115, 59, "Area Dagger");
				AddLabel( 75, 145, 59, "Area Skinning Knife");

				AddButton( 40,  58, 0x2623, 0x2622, 15, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 16, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 17, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 18, GumpButtonType.Reply, 0 );

				AddPage ( 5 );

				AddLabel( 160, 25, 0x34, "Select the Mace you Desire.");

				AddLabel( 75,  55, 59, "Area Club");
				AddLabel( 75,  85, 59, "Area Hammer Pick");
				AddLabel( 75, 115, 59, "Area Mace");
				AddLabel( 75, 145, 59, "Area Maul");
				AddLabel( 75, 175, 59, "Area Scepter");
				AddLabel( 75, 205, 59, "Area War Hammer");
				AddLabel( 75, 235, 59, "Area War Mace");

				AddButton( 40,  58, 0x2623, 0x2622, 19, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 20, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 21, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 22, GumpButtonType.Reply, 0 );
				AddButton( 40, 178, 0x2623, 0x2622, 23, GumpButtonType.Reply, 0 );
				AddButton( 40, 208, 0x2623, 0x2622, 24, GumpButtonType.Reply, 0 );
				AddButton( 40, 238, 0x2623, 0x2622, 25, GumpButtonType.Reply, 0 );

				AddPage ( 6 );

				AddLabel( 140, 25, 0x34, "Select the Pole Arm you Desire.");

				AddLabel( 75,  55, 59, "Area Bardiche");
				AddLabel( 75,  85, 59, "Area Halberd");
				AddLabel( 75, 115, 59, "Area Scythe");

				AddButton( 40,  58, 0x2623, 0x2622, 26, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 27, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 28, GumpButtonType.Reply, 0 );

				AddPage ( 7 );

				AddLabel( 130, 25, 0x34, "Select the Spear or Fork you Desire.");

				AddLabel( 75,  55, 59, "Area Bladed Staff");
				AddLabel( 75,  85, 59, "Area Double Bladed Staff");
				AddLabel( 75, 115, 59, "Area Pike");
				AddLabel( 75, 145, 59, "Area Pitchfork");
				AddLabel( 75, 175, 59, "Area Short Spear");
				AddLabel( 75, 205, 59, "Area Spear");
				AddLabel( 75, 235, 59, "Area War Fork");

				AddButton( 40,  58, 0x2623, 0x2622, 29, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 30, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 31, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 32, GumpButtonType.Reply, 0 );
				AddButton( 40, 178, 0x2623, 0x2622, 33, GumpButtonType.Reply, 0 );
				AddButton( 40, 208, 0x2623, 0x2622, 34, GumpButtonType.Reply, 0 );
				AddButton( 40, 238, 0x2623, 0x2622, 35, GumpButtonType.Reply, 0 );

				AddPage ( 8 );

				AddLabel( 160, 25, 0x34, "Select the Staff you Desire.");

				AddLabel( 75,  55, 59, "Area Black Staff");
				AddLabel( 75,  85, 59, "Area Gnarled Staff");
				AddLabel( 75, 115, 59, "Area Quarter Staff");
				AddLabel( 75, 145, 59, "Area Shepherd's Crook");

				AddButton( 40,  58, 0x2623, 0x2622, 36, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 37, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 38, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 39, GumpButtonType.Reply, 0 );

				AddPage ( 9 );

				AddLabel( 160, 25, 0x34, "Select the Sword you Desire.");

				AddLabel( 75,  55, 59, "Area Bone Harvester");
				AddLabel( 75,  85, 59, "Area Broad Sword");
				AddLabel( 75, 115, 59, "Area Crescent Blade");
				AddLabel( 75, 145, 59, "Area Cutlass");
				AddLabel( 75, 175, 59, "Area Katana");
				AddLabel( 75, 205, 59, "Area Kryss");
				AddLabel( 75, 235, 59, "Area Lance");
				AddLabel( 75, 265, 59, "Area Long Sword");
				AddLabel( 75, 295, 59, "Area Scimitar");
				AddLabel( 75, 325, 59, "Area Viking Sword");

				AddButton( 40,  58, 0x2623, 0x2622, 40, GumpButtonType.Reply, 0 );
				AddButton( 40,  88, 0x2623, 0x2622, 41, GumpButtonType.Reply, 0 );
				AddButton( 40, 118, 0x2623, 0x2622, 42, GumpButtonType.Reply, 0 );
				AddButton( 40, 148, 0x2623, 0x2622, 43, GumpButtonType.Reply, 0 );
				AddButton( 40, 178, 0x2623, 0x2622, 44, GumpButtonType.Reply, 0 );
				AddButton( 40, 208, 0x2623, 0x2622, 45, GumpButtonType.Reply, 0 );
				AddButton( 40, 238, 0x2623, 0x2622, 46, GumpButtonType.Reply, 0 );
				AddButton( 40, 268, 0x2623, 0x2622, 47, GumpButtonType.Reply, 0 );
				AddButton( 40, 298, 0x2623, 0x2622, 48, GumpButtonType.Reply, 0 );
				AddButton( 40, 328, 0x2623, 0x2622, 48, GumpButtonType.Reply, 0 );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted )
					return;

				BaseWeapon weapon = null;

				switch ( info.ButtonID )
				{
					case 0: return;
					case 1: weapon = new AreaAxe() ; break;
					case 2: weapon = new AreaBattleAxe() ; break;
					case 3: weapon = new AreaDoubleAxe() ; break;
					case 4: weapon = new AreaExecutionersAxe() ; break;
					case 5: weapon = new AreaHatchet() ; break;
					case 6: weapon = new AreaLargeBattleAxe() ; break;
					case 7: weapon = new AreaPickaxe() ; break;
					case 8: weapon = new AreaTwoHandedAxe() ; break;
					case 9: weapon = new AreaWarAxe() ; break;
					case 10: weapon = new AreaBow() ; break;
					case 11: weapon = new AreaCompositeBow() ; break;
					case 12: weapon = new AreaCrossbow() ; break;
					case 13: weapon = new AreaHeavyCrossbow() ; break;
					case 14: weapon = new AreaRepeatingCrossbow() ; break;
					case 15: weapon = new AreaButcherKnife() ; break;
					case 16: weapon = new AreaCleaver() ; break;
					case 17: weapon = new AreaDagger() ; break;
					case 18: weapon = new AreaSkinningKnife() ; break;
					case 19: weapon = new AreaClub() ; break;
					case 20: weapon = new AreaHammerPick() ; break;
					case 21: weapon = new AreaMace() ; break;
					case 22: weapon = new AreaMaul() ; break;
					case 23: weapon = new AreaScepter() ; break;
					case 24: weapon = new AreaWarHammer() ; break;
					case 25: weapon = new AreaWarMace() ; break;
					case 26: weapon = new AreaBardiche() ; break;
					case 27: weapon = new AreaHalberd() ; break;
					case 28: weapon = new AreaScythe() ; break;
					case 29: weapon = new AreaBladedStaff() ; break;
					case 30: weapon = new AreaDoubleBladedStaff() ; break;
					case 31: weapon = new AreaPike() ; break;
					case 32: weapon = new AreaPitchfork() ; break;
					case 33: weapon = new AreaShortSpear() ; break;
					case 34: weapon = new AreaSpear() ; break;
					case 35: weapon = new AreaWarFork() ; break;
					case 36: weapon = new AreaBlackStaff() ; break;
					case 37: weapon = new AreaGnarledStaff() ; break;
					case 38: weapon = new AreaQuarterStaff() ; break;
					case 39: weapon = new AreaShepherdsCrook() ; break;
					case 40: weapon = new AreaBoneHarvester() ; break;
					case 41: weapon = new AreaBroadSword() ; break;
					case 42: weapon = new AreaCrescentBlade() ; break;
					case 43: weapon = new AreaCutlass() ; break;
					case 44: weapon = new AreaKatana() ; break;
					case 45: weapon = new AreaKryss() ; break;
					case 46: weapon = new AreaLance() ; break;
					case 47: weapon = new AreaLongSword() ; break;
					case 48: weapon = new AreaScimitar() ; break;
					case 49: weapon = new AreaVikingSword() ; break;
				}

				if ( weapon != null )
				{
					weapon.Name = weapon.Name + " of Fire & Ice";
					weapon.AosElementDamages.Fire = 25;
					weapon.AosElementDamages.Cold = 25;
					weapon.WeaponAttributes.HitFireArea = 75;
					weapon.WeaponAttributes.HitColdArea = 75;
					weapon.Quality = m_Deed.Quality;
					weapon.Resource = m_Deed.Resource;
					if ( m_Deed.Crafter != null ) weapon.Crafter = m_Deed.Crafter;
					m_From.Backpack.DropItem( weapon );
					m_From.SendMessage( "You summon the Area Weapon!" );
					m_Deed.Delete();
				}
			}
		}

		public FireAndIceAreaWeaponDeed( Serial serial ) : base( serial ) { }
		public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}
}