using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.ACC.PG
{
	public class PGGump : Gump
	{
		private Mobile     m_From;
		private int        m_Page;
		private PublicGate m_Gate;

		private EntryFlag EFlags;
		private void SetFlag( EntryFlag flag, bool value )
		{
			if( value )
				EFlags |= flag;
			else EFlags &= ~flag;
		}

		public PGGump( Mobile from, int Page, PublicGate gate ) : base( 0, 0 )
		{
			if( !PGSystem.Running )
				return;

			if( PGSystem.CategoryList == null || PGSystem.CategoryList.Count == 0 )
			{
				SetFlag( EntryFlag.StaffOnly, true );
				SetFlag( EntryFlag.Generate, false );
				PGSystem.CategoryList = new List<PGCategory>();
				PGSystem.CategoryList.Add( new PGCategory( "Empty System", EFlags ) );
			}

			m_From = from;
			m_Page = Page;
			m_Gate = gate;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			#region Categories
			int Cats = CountCats();

			int CStart = (250 - ((Cats / 2) * 25));
			if( CStart < 0 )
				CStart = 0;

			AddBackground( 0, CStart, 230, 100+Cats*25, 2600 );
            AddHtml(0, CStart + 15, 230, 20, "<BASEFONT COLOR=#58D3F7 SIZE=8><CENTER>Traveler's Traverse</CENTER></BASEFONT>", false, false);

			int CurC = 0;
			for( int i = 0; i < PGSystem.CategoryList.Count; i++ )
			{
				PGCategory PGC = PGSystem.CategoryList[i];
				if( PGC != null )
				{
					if( from.AccessLevel >= PGSystem.PGAccessLevel )
					{
						AddButton( 20, CStart+53+CurC*25, (Page == i ? 209 : 208), (Page == i ? 208 : 209), 100+i, GumpButtonType.Reply, 0);
						AddLabel( 50, CStart+53+CurC*25, (Page == i ? 69 : 0), PGC.Name );
						CurC++;
						continue;
					}

					if( PGC.GetFlag( EntryFlag.StaffOnly ) && from.AccessLevel == AccessLevel.Player )
						continue;
					if( !PGC.GetFlag( EntryFlag.StaffOnly ) )
					{
						if( PGC.GetFlag( EntryFlag.Young ) && !((PlayerMobile)from).Young )
							continue;
						if( !PGC.GetFlag( EntryFlag.Reds ) && from.Kills >= 5 )
							continue;
					}

					AddButton( 20, CStart+53+CurC*25, (Page == i ? 209 : 208), (Page == i ? 208 : 209), 100+i, GumpButtonType.Reply, 0);
					AddLabel( 50, CStart+53+CurC*25, (Page == i ? 69 : 0), PGC.Name );
					CurC++;
				}
			}

			if( from.AccessLevel >= PGSystem.PGAccessLevel )
			{
				AddLabel(   75, CStart+65+Cats*25, 0, "Add" );
				AddButton(  50, CStart+65+Cats*25, 208, 209, 2, GumpButtonType.Reply, 0 );
				AddLabel(  125, CStart+65+Cats*25, 0, "Edit" );
				AddButton( 160, CStart+65+Cats*25, 208, 209, 3, GumpButtonType.Reply, 0 );
			}
			#endregion //Categories

			#region Locations
			int Locs = CountLocs();
			if( Locs == -1 )
				Locs = 0;

			int LStart = (250 - ((Locs / 2) * 25));
			if( LStart < 20 )
				LStart = 20;

			AddBackground( 230, LStart, 300, 100+Locs*25, 2600 );

			int CurL = 0;
			PGCategory PGCL = PGSystem.CategoryList[m_Page];
			if( PGCL != null && PGCL.Locations != null )
			{
				for( int i = 0; i < PGCL.Locations.Count; i++ )
				{
					PGLocation PGL = PGCL.Locations[i];
					if( PGL != null )
					{
						if( from.AccessLevel >= PGSystem.PGAccessLevel )
						{
							AddRadio( 250, LStart+53+CurL*25, 208, 209, false, 200+i );
							AddLabel( 280, LStart+53+CurL*25, 0, PGL.Name );
							CurL++;
							continue;
						}

						if( PGL.GetFlag( EntryFlag.StaffOnly ) && from.AccessLevel == AccessLevel.Player )
							continue;
						if( !PGL.GetFlag( EntryFlag.StaffOnly ) )
						{
							if( PGL.GetFlag( EntryFlag.Young ) && !((PlayerMobile)from).Young )
								continue;
							if( !PGL.GetFlag( EntryFlag.Reds ) && from.Kills >= 5 )
								continue;
						}

						AddRadio( 250, LStart+53+CurL*25, 208, 209, false, 200+i );
						AddLabel( 280, LStart+53+CurL*25, 0, PGL.Name );
						CurL++;
					}
				}
			}

			AddButton( 465, LStart-20, 1417, 1417, 1, GumpButtonType.Reply, 0);
            AddHtml(485, LStart + 10, 40, 40, "<BODY><BASEFONT SIZE=7 COLOR=#2E64FE><CENTER><I><B>GO</B></I></CENTER></BASEFONT></BODY>", false, false);

			if( from.AccessLevel >= PGSystem.PGAccessLevel )
			{
				AddLabel( 305, LStart+15, 0, "Add Current Gate" );
				AddButton( 280, LStart+15, 208, 209, 6, GumpButtonType.Reply, 0 );

				AddLabel( 305, LStart+65+Locs*25, 0, "Add" );
				AddButton( 280, LStart+65+Locs*25, 208, 209, 4, GumpButtonType.Reply, 0 );

				AddLabel( 430, LStart+65+Locs*25, 0, "Edit" );
				AddButton( 460, LStart+65+Locs*25, 208, 209, 5, GumpButtonType.Reply, 0);
			}
			#endregion //Locations
		}

		private int CountCats()
		{
			int count = 0;
			foreach( PGCategory PGC in PGSystem.CategoryList )
			{
				if( (PGC.GetFlag( EntryFlag.StaffOnly ) && m_From.AccessLevel > AccessLevel.Player) ||
					(!PGC.GetFlag( EntryFlag.StaffOnly ) && ((!PGC.GetFlag( EntryFlag.Reds ) && m_From.Kills < 5) || PGC.GetFlag( EntryFlag.Reds ))) ||
					(m_From.AccessLevel > AccessLevel.Player) )
					count++;
			}
			return count;
		}

		private int CountLocs()
		{
			if( m_Page < 0 || m_Page >= PGSystem.CategoryList.Count )
				return -1;

			int count = 0;
			PGCategory PGC = PGSystem.CategoryList[m_Page];
			if( PGC != null && PGC.Locations != null )
			{
                IEnumerator<PGLocation> PGL = PGC.Locations.GetEnumerator();
                while(PGL.MoveNext())
                {
					if( (PGL.Current.GetFlag( EntryFlag.StaffOnly ) && m_From.AccessLevel > AccessLevel.Player) ||
                        (!PGL.Current.GetFlag(EntryFlag.StaffOnly) && ((!PGL.Current.GetFlag(EntryFlag.Reds) && m_From.Kills < 5) || PGL.Current.GetFlag(EntryFlag.Reds))) ||
						(m_From.AccessLevel > AccessLevel.Player) )
						count++;
				}
			}
			return count;
		}

		private Conditions Flags;
		private void SetFlag( Conditions flag, bool value )
		{
			if( value )
				Flags |= flag;
			else Flags &= ~flag;
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			int BID = info.ButtonID;
			int Loc = -1;

			if( !PGSystem.Running )
				return;

			if( m_From.Deleted || m_Gate.Deleted || m_From.Map == null )
				return;

			if( info.Switches.Length > 0 )
				Loc = info.Switches[0];

			Loc -= 200;

			if( BID == 0 )
			{
				from.SendMessage( "You choose not to go anywhere." );
				return;
			}

			if( BID == 1 )
			{
				if( Loc <= -1 )
				{
					from.SendMessage( "You must select a location!" );
					from.SendGump( new PGGump( from, m_Page, m_Gate ) );
					return;
				}

				PGCategory PGC = PGSystem.CategoryList[m_Page];
				if( PGC == null )
					return;

				PGLocation PGL = PGC.Locations[Loc];
				if( PGL == null )
					return;

				if( !from.InRange( m_Gate.GetWorldLocation(), 1 ) || from.Map != m_Gate.Map )
				{
					from.SendLocalizedMessage( 1019002 ); // You are too far away to use the gate.
				}
				else if( Factions.Sigil.ExistsOn( from ) && PGL.Map != Factions.Faction.Facet )
				{
					from.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.
					from.SendGump( new PGGump( from, m_Page, m_Gate ) );
				}
				else if( from.Criminal )
				{
					from.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
				}
				else if( Server.Spells.SpellHelper.CheckCombat( from ) )
				{
					from.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
				}
				else if( from.Spell != null )
				{
					from.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment.
				}
				else if( from.Map == PGL.Map && from.InRange( PGL.Location, 1 ) )
				{
					from.SendLocalizedMessage( 1019003 ); // You are already there.
					from.SendGump( new PGGump( from, m_Page, m_Gate ) );
				}
				else if( PGL.GetFlag( EntryFlag.Young ) && !((PlayerMobile)from).Young && from.AccessLevel == AccessLevel.Player )
				{
					from.SendMessage( "You are too old to travel here." );
					from.SendGump( new PGGump( from, m_Page, m_Gate ) );
				}
				else if( !PGL.GetFlag( EntryFlag.Reds ) && from.Kills >= 5 && from.AccessLevel == AccessLevel.Player )
				{
					from.SendMessage( "You too many murders to travel here." );
					from.SendGump( new PGGump( from, m_Page, m_Gate ) );
				}
				else if( PGL.GetFlag( EntryFlag.StaffOnly ) && from.AccessLevel == AccessLevel.Player )
				{
					from.SendMessage( "You are not allowed to travel here." );
					from.SendGump( new PGGump( from, m_Page, m_Gate ) );
				}
				else
				{
					bool charged = false;
					if( PGC.GetFlag( EntryFlag.Charge ) && PGC.Cost > 0 && from.AccessLevel == AccessLevel.Player )
					{
						Container pack = from.Backpack;
						if( pack == null )
							return;
						if( !pack.ConsumeTotal( typeof( Gold ), PGC.Cost ) )
						{
							from.SendMessage( "You require {0} gold to travel there.", PGC.Cost );
							from.SendGump( new PGGump( from, m_Page, m_Gate ) );
							return;
						}
						charged = true;
					}

					if( !charged && PGL.GetFlag( EntryFlag.Charge ) && PGL.Cost > 0 && from.AccessLevel == AccessLevel.Player )
					{
						Container pack = from.Backpack;
						if( pack == null )
							return;
						if( !pack.ConsumeTotal( typeof( Gold ), PGL.Cost ) )
						{
							from.SendMessage( "You require {0} gold to travel there.", PGL.Cost );
							from.SendGump( new PGGump( from, m_Page, m_Gate ) );
							return;
						}
					}

					BaseCreature.TeleportPets( from, PGL.Location, PGL.Map );

					from.Combatant = null;
					from.Warmode = false;
					from.Hidden = true;

					from.MoveToWorld( PGL.Location, PGL.Map );

					Effects.PlaySound( PGL.Location, PGL.Map, 0x1FE );
					from.SendMessage( "You have been teleported to: " + PGL.Name );
				}
			}

			else if( BID >= 100 )
			{
				from.CloseGump( typeof( PGGump ) );
				from.SendGump( new PGGump( from, BID-100, m_Gate ) );
			}

			else if( BID == 2 && from.AccessLevel >= PGSystem.PGAccessLevel)
			{
				SetFlag( Conditions.Adding, true );
				SetFlag( Conditions.Category, true );

				from.CloseGump( typeof( PGAddEditGump ) );
				from.CloseGump( typeof( PGGump ) );
				from.SendGump( new PGGump( from, m_Page, m_Gate ) );
				from.SendGump( new PGAddEditGump( Flags, m_Page, -1, m_Gate ) );
			}

            else if (BID == 3 && from.AccessLevel >= PGSystem.PGAccessLevel)
			{
				SetFlag( Conditions.Adding, false );
				SetFlag( Conditions.Category, true );

				from.CloseGump( typeof( PGAddEditGump ) );
				from.CloseGump( typeof( PGGump ) );
				from.SendGump( new PGGump( from, m_Page, m_Gate ) );
				from.SendGump( new PGAddEditGump( Flags, m_Page, -1, m_Gate ) );
			}

            else if (BID == 4 && from.AccessLevel >= PGSystem.PGAccessLevel)
			{
				SetFlag( Conditions.Adding, true );
				SetFlag( Conditions.Category, false );

				from.CloseGump( typeof( PGAddEditGump ) );
				from.CloseGump( typeof( PGGump ) );
				from.SendGump( new PGGump( from, m_Page, m_Gate ) );
				from.SendGump( new PGAddEditGump( Flags, m_Page, Loc, m_Gate ) );
			}

            else if (BID == 5 && from.AccessLevel >= PGSystem.PGAccessLevel)
			{
				if( Loc <= -1 )
				{
					from.SendMessage( "You must select a location!" );
					from.SendGump( new PGGump( from, m_Page, m_Gate ) );
					return;
				}
				SetFlag( Conditions.Adding, false );
				SetFlag( Conditions.Category, false );

				from.CloseGump( typeof( PGAddEditGump ) );
				from.CloseGump( typeof( PGGump ) );
				from.SendGump( new PGGump( from, m_Page, m_Gate ) );
				from.SendGump( new PGAddEditGump( Flags, m_Page, Loc, m_Gate ) );
			}

            else if (BID == 6 && from.AccessLevel >= PGSystem.PGAccessLevel)
			{
				if( m_Gate.Parent != null )
				{
					from.SendMessage( "You must place the gate in the World to add it." );
					from.SendGump( new PGGump( from, m_Page, m_Gate ) );
					return;
				}

				PGLocation PGL = new PGLocation( "Un-named Gate", EntryFlag.None, m_Gate.Location, m_Gate.Map, 0 );
				if( PGL == null )
				{
					from.SendMessage( "Could not add." );
					from.SendGump( new PGGump( from, m_Page, m_Gate ) );
					return;
				}

				from.SendMessage( "Added the Gate.  Please edit any other features you wish it to have." );
				PGSystem.CategoryList[m_Page].Locations.Add( PGL );
                from.SendGump(new PGGump(from, m_Page, m_Gate));
                from.SendGump(new PGAddEditGump(Flags, m_Page, PGSystem.CategoryList[m_Page].Locations.Count-1, m_Gate));
			}

			else
			{
				from.SendMessage( "Undefined button pressed: {0}", BID );
			}
		}
	}
}