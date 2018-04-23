//--Created by Lucid Nagual - Admin of the Conjuring

using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using System.Collections;

namespace Server.Gumps
{
	public class ArenaEnd : Gump
	{
				
		private static void DeleteEventItems()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Item item in World.Items.Values )
			{
				if ( item is ArenaPortcullisEW )
					list.Add( item );
				if ( item is ArenaPortcullisNS )
					list.Add( item );
				if ( item is ArenaGateEW )
					list.Add( item );
				if ( item is ArenaGateNS )
					list.Add( item );
				if ( item is LifeGate )
					list.Add( item );
				if ( item is ArenaExitMoongate )
					list.Add( item );
				if ( item is ArenaBankStone )
					list.Add( item );

			}
			foreach ( Item item in list )
				item.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}
		
		private static void DeleteMobs()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseHealer )
				{
					BaseHealer bc = (BaseHealer)m;
					
					if ( bc is ArenaMob )
						list.Add( bc );
				}
			}
			foreach ( ArenaMob mobs in list )
				mobs.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}

		private static void DeleteLB()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseHealer )
				{
					BaseHealer lb = (BaseHealer)m;
					
					if ( lb is LordBritish )
						list.Add( lb );
				}
			}
			foreach ( LordBritish lmob in list )
				lmob.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}

		private static void DeleteAM()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseCreature )
				{
					BaseCreature am = (BaseCreature)m;
					
					if ( am is ArenaMinotaur )
						list.Add( am );
				}
			}
			foreach ( ArenaMinotaur amob in list )
				amob.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}

		private static void DeleteAK()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseCreature )
				{
					BaseCreature ak = (BaseCreature)m;
					
					if ( ak is ArenaKnight )
						list.Add( ak );
				}
			}
			foreach ( ArenaKnight akob in list )
				akob.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}

		private static void DeleteAD()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseCreature )
				{
					BaseCreature ad = (BaseCreature)m;
					
					if ( ad is ArenaDaemon )
						list.Add( ad );
				}
			}
			foreach ( ArenaDaemon adob in list )
				adob.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}

		private static void DeleteAS()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseCreature )
				{
					BaseCreature ast = (BaseCreature)m;
					
					if ( ast is Sensei )
						list.Add( ast );
				}
			}
			foreach ( Sensei sob in list )
				sob.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}

		private static void DeleteAST()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseCreature )
				{
					BaseCreature st = (BaseCreature)m;
					
					if ( st is Student )
						list.Add( st );
				}
			}
			foreach ( Student stob in list )
				stob.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}

		private static void DeleteAN()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseCreature )
				{
					BaseCreature an = (BaseCreature)m;
					
					if ( an is ArenaNinja )
						list.Add( an );
				}
			}
			foreach ( ArenaNinja anob in list )
				anob.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}

		private static void DeleteAE()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseCreature )
				{
					BaseCreature ae = (BaseCreature)m;
					
					if ( ae is ArenaEliteNinja )
						list.Add( ae );
				}
			}
			foreach ( ArenaEliteNinja aeob in list )
				aeob.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}

		private static void DeleteAT()
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseCreature )
				{
					BaseCreature at = (BaseCreature)m;
					
					if ( at is ArenaMaster )
						list.Add( at );
				}
			}
			foreach ( ArenaMaster atob in list )
				atob.Delete();
			if ( list.Count > 0 )
				World.Broadcast( 0x35, true, "{0} Resetting Arena", list.Count );
		}
		
		public ArenaEnd() : base( 0, 0 )
		{
			AddImageTiled(  54, 33, 369, 400, 2624 );
			AddAlphaRegion( 54, 33, 369, 400 );
			AddImageTiled( 416, 39, 44, 389, 203 );			
			AddImage( 97, 49, 9005 );
			AddImageTiled( 58, 39, 29, 390, 10460 );
			AddImageTiled( 412, 37, 31, 389, 10460 );
			AddLabel( 140, 60, 0x34, "EXIT ARENA" );
			AddLabel( 130, 120, 0x34, "WARNING, YOU CANNOT RETURN!!" );
			AddLabel( 130, 135, 0x34, "ARENA WILL BE RESET!!!" );
			AddImage( 430, 9, 10441);
			AddImageTiled( 40, 38, 17, 391, 9263 );
			AddImage( 6, 25, 10421 );
			AddImage( 34, 12, 10420 );
			AddImageTiled( 94, 25, 342, 15, 10304 );
			AddImageTiled( 40, 427, 415, 16, 10304 );
			AddImage( -10, 314, 10402 );
			AddImage( 56, 150, 10411 );
			AddImage( 136, 84, 96 );
			AddImage( 215, 160, 5536 );

			AddButton( 225, 390, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0 ); 			
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			
			switch ( info.ButtonID )
			{
				case 1: // End Event Gump
					{
						//--Remove Event Items--------------------------------
						from.SendMessage("You Leave The Arena A Champion!!!");
						from.Map = Map.Felucca;
						from.X = 1425;
						from.Y = 1702;
						from.Z = 3;
						
						DeleteEventItems();
						DeleteMobs();
						DeleteLB();
						DeleteAM();
						DeleteAK();
						DeleteAD();
						DeleteAS();
						DeleteAST();
						DeleteAN();
						DeleteAE();
						DeleteAT();

						break;
					}
				
			}
		}
	}
}