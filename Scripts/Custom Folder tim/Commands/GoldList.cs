using System;
using System.IO;
using Server;
using Server.Commands;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;

using Server.Accounting;
using Server.ContextMenus;

using Acc = Server.Accounting.Account;

namespace Server.Misc
{
	public class GoldList
	{
		public static void Initialize()
		{
			CommandSystem.Register( "GoldList", AccessLevel.Owner, new CommandEventHandler( goldlistCommand ) );
		}

		[Usage( "GoldList <players/world/dump>" )]
		[Description( "Displays amount of gold on players, in world, or dump to file.." )]
		private static void goldlistCommand( CommandEventArgs e )
		{
			float goldcount = 0;
			string formatworld = "";
			int decimalworld = 0;
			string goldtype = "";

			if ( e.Length > 0 )
				goldtype = e.ArgString;

			switch ( goldtype )
			{
				case "players":
					goldcount = GoldOnPlayers();
					break;
				case "world":
					goldcount = GoldInWorld();
					break;
				case "dump":
					goldcount = GoldDump( e.Mobile );
					break;
				default:
					e.Mobile.SendMessage( "Usage: Gold <players/world/dump>" );
					return;
			}

			if ( goldcount > 999999 )
			{
				goldcount = goldcount / 1000000;
				formatworld = " Million";
				decimalworld = 3;
			}
			else if( goldcount >= 1000 )
			{
				goldcount = goldcount / 1000;
				formatworld = " Thousand";
				decimalworld = 1;
			}

			switch ( goldtype )
			{
				case "players":
					e.Mobile.SendMessage( "There is " + String.Format( "{0:f" + decimalworld + "}", goldcount) + formatworld + " gold owned by players." );
					break;
				case "world":
				case "dump":
					e.Mobile.SendMessage( "There is " + String.Format( "{0:f" + decimalworld + "}", goldcount) + formatworld + " gold in the world." );
					break;
			}
		}

		public static float GoldOnPlayers()
		{
			float GoP = 0;

			foreach( Mobile MiW in World.Mobiles.Values )
			{
				if( MiW is PlayerMobile )
				{
					PlayerMobile pm = (PlayerMobile)MiW;

					if ( pm.AccessLevel < AccessLevel.GameMaster )
					{
						GoP += Banker.GetBalance( pm );//Mobile.Account.TotalGold
						GoP += SearchForGold( pm.Backpack );
					}
				}
			}
			return GoP;
		}

		public static float GoldInWorld()
		{
			float goldcount = 0;

			foreach( Item i in World.Items.Values )
				if( i is Gold )
					goldcount += ((Gold)i).Amount;
				else if( i is BankCheck )
					goldcount += ((BankCheck)i).Worth;
			return goldcount;
		}

		public static float GoldDump( Mobile m )
		{
			StreamWriter w = new StreamWriter( "gold.txt" );
			float goldcount = 0;
			float bankgold = 0;
			float packgold = 0;
			float inbank = 0;
			float inpack = 0;

			m.SendMessage( "Exporting list of gold to \"gold.txt\"..." );

			w.WriteLine( "Character Gold (bank pack total name)" );
			foreach( Mobile MiW in World.Mobiles.Values )
			{
				if( MiW is PlayerMobile )
				{
					PlayerMobile pm = (PlayerMobile)MiW;
					if ( pm.AccessLevel < AccessLevel.GameMaster )
					{
						inbank = Banker.GetBalance( pm );
						inpack = SearchForGold( pm.Backpack );
						w.WriteLine( "{0}\t{1}\t{2}\t{3}", inbank, inpack, inbank + inpack, pm.Name );
						bankgold += inbank;
						packgold += inpack;
					}
				}
			}

			w.WriteLine( "" );
			w.WriteLine( "Total in banks: {0}", bankgold );
			w.WriteLine( "Total in packs: {0}", packgold );
			w.WriteLine( "Total player gold: {0}", bankgold + packgold );

			// Total gold in world
			foreach( Item i in World.Items.Values )
			{
				if( i is Gold )
					goldcount += ((Gold)i).Amount;
				else if( i is BankCheck )
					goldcount += ((BankCheck)i).Worth;
			}

			w.WriteLine( "" );
			w.WriteLine( "Total in world: {0}", goldcount );
			w.Close();

			m.SendMessage( "Export complete." );
			return goldcount;
		}

		public static float SearchForGold( Container c )
		{
			float goldcount = 0;

			foreach( Item i in c.Items )
				if( i is Container )
					goldcount += SearchForGold( (Container)i );
				else if( i is Gold )
					goldcount += ((Gold)i).Amount;
				else if( i is BankCheck )
					goldcount += ((BankCheck)i).Worth;
			return goldcount;
		}
	}
}