/**************************** WorldOmniporter.cs v2.4***********************
 *
 *					(C) 2011, Lokai
 *			
 * Description: Revised daat99's World Teleporter, with added
 * 		features, such as limiting who can use it, charging a
 * 		fee for each use, limiting pets, etc.
 *	  
 * Updated November 1, 2011: Optimized the code, to make it so
 * 		that a server reboot is not necessary to initialize the
 * 		system.
 * 
 * See notes below for details on available options.
 *   
/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
/*#####	
##### FILES IN THIS RELEASE:															
#####																			
##### -- WorldOmniporter.cs															
##### -- ConfirmGump.cs																
#####																			
#####																			
##### INSTALLATION NOTES:															
#####																			
##### -- Drop anywhere in Scripts folder.													
##### -- To remove moongates run command "[global delete where publicmoongate" 
##### -- Run command "[WorldOmniGen" in the game to add World Omniporters
##### -- To remove Omniporters run command "[WorldOmniDel"   																			
##### -- Use [props to adjust parameters.											
#####
#####
#####
#####
#####																		
##### WHAT'S NEW?															
#####																			
##### -- Twelve new flags:															
##### -- % -- WO_OnlyAllowYoung	= When 'true' only YOUNG players can use the Omniporter				
##### -- % -- WO_FeluccaNoYoung	= When 'true' YOUNG players may not travel to Felucca					
##### -- % -- WO_DungeonsNoYoung	= When 'true' YOUNG players may not travel to Dungeons				
##### -- % -- WO_AllPayToUse		= When 'true' the Omniporter costs Gold to use each trip				
##### -- % -- WO_SkillsCost		= When 'true' higher skill totals make Omniporters more expensive			
##### -- % -- WO_KarmaDiscount		= When 'true' higher Karma gives players discounts on the cost			  				
##### -- % -- WO_DelayAfterUse		= When 'true' players must wait a TimeDelay after using the Omniporter		
##### -- % -- WO_PayBeforeDelay	= When 'true' players can pay to avoid waiting for the TimeDelay			
##### -- % -- WO_PetsMayTravel		= When 'true' pets may follow their owners through the Omniporter			
##### -- % -- WO_PetsMustPay		= When 'true' owners must pay for pets also						
#####																			
##### -- Each new Flag is also subject to the 'Local' or 'Global' setting on it. If Local, any						
#####    changes made to the Omniporter through [props will only affect that Omniporter. When Global,				
#####    any changes made affect all Omniporters that are also Global. The only exception is the			 		
#####    LocalUses variable, which is only used to track Local usage.									
#####																		
##### -- The Omniporter keeps track of how many times it is used. This can be useful to determine				
#####    trends on your shard, or can be used to determine if you want to start charging for a					
#####    particular Omniporter. Every time they use one, staff members will see Omniporter usage				
#####    in the Selection Gump.															
#####																			
##### -- The 'UseEntry' keeps track of the Name of every player, and the Date they last used it,					
#####    and the total number of times they have used.											
#####																		
##### -- The Skill premium and Karma discount can be further enhanced with Skill or Karma Bonus modifiers.
#####																			
##### -- A friendly ConfirmGump warns players before they spend their hard-earned cash on the trip.				
#####																			
#####																			
		
#####																			
#####																			
############################################################################  [WorldOmniGen
##################################################################### 
###########################################################################*/
//////////////////////////////////// WORLD TELEPORTER NOTES /////////////////////////////////////////
///
///daat99's World Teleporter (remade by ACME_INC)
///
///changelog v2.5
///03.28.2015: Darkshard maps Thanimur and Sosaria included but commented out for those with custom maps like me.
///01.21.2015: Added sparkle effect to gate when used.  
///11.12.2014: Removed Staff locations - most shards have a staff runebook so it was redundant. 
///09.01.2014: Rebuilt Gump to be transparent and make it look more like the other gumps in game.
///09.01.2014: Cleaned up map colors to help modernize the gump.       
///09.01.2014: Changed Moongate Animation.  This will require newer clients - Tested on 7.0.34.6 but may work a few revisions back.
///09.01.2014: Converted DateTime.Now to DateTime.UtcNow
///09.01.2014: Removed m_MaxUses,WO_UsesLimit,TimeExpiration,WO_TimeLimit,GlobalMaxUses
///
///
///
///changelog v2.4
///03.08.2014: Added TerMur, ML dungeons, changed staff location to end and modified validations, updated client optflags.
///03.08.2014: Modified gumps to handle additional map locations.
///
///changelog v1.1 ==> XxSP1DERxX
///04/06/2005: Bug with Tokuno taking you to Trammel locations.
///04/06/2005: Optimized the gump code.
///
///changelog v1.0 ==> remade by XxSP1DERxX
///02/06/2005: Changed all boolean properties to 1 bitwise flag
///02/06/2005: Added a global/local switch (so that admins can set their global settings )
///02/06/2005: Optimized the Gump code to use "virtual pages"
///02/06/2005: Optimized the Location list generation code
///02/06/2005: Optimized the gump response code to use an algorithm
///02/06/2005: Added triple redundant validity checks
///02/06/2005: Added staff check
///02/06/2005: Put Map hueing into a function to allow easy changing
///02/06/2005: Created default global settings in the Initialize() function
///
///changelog v0.8
///08/03/2005: fixed a warning.
///08/03/2005: Fixed when you disable fel or tram it still showed them in the public moongate.
///
///changelog v0.7
///08/03/2005: Added colors to Ilshenar Malas and Tokuno maps, Thanx Kiara for choosing the colors :)
///07/03/2005: Added more locations to tokuno map.
///07/03/2005: Added ilshenar shrines map.
///07/03/2005: Added trammel and felucca public moongates.
///
///changelog v0.6
///02/03/2005: Added a [WorldTeleGen command to generate the world teleporter in all the exits. It'll generate the teleporters only on the maps that set to true in the [props and delete all existing unmovable teles.
///02/03/2005: Rewrote the teleporter with arrays and hash table to simplify the modifications and to shorten the code. (ThanX A Lot UOT) ;)
///
///changelog v0.5
///24/02/2005: Added Spider Cave in Ilshenar.
///
///changelog v0.4
///24/02/2005: Added a check not to play sounds if hidden staff member use it.
///24/02/2005: Added a check to see if you're near the destination you selected (1 tile away) requested by Raider
///24/02/2005: A few modifications to the teleporter code.
///
///changelog v0.3
///23/02/2005: Added exodus dungeon, rock dungeon, solen hive and orc cave, thanx to Tark for requesting them :)
///23/02/2005: Moved mook town to custom where it was supposed to be in the first place.
///
///changelog v0.2
///22/02/2005: Fixed some typos, thanx Crepti ;)
///
///changelog v0.1
///22/02/2005: Made by daat99.
///
///Made by daat99, v1.0 remade by XxSP1DERxX
///Special thanks for XxSP1DERxX for rewriting and optimizing the code A LOT ;)
///Special thanks for Kiara for the idea ;) (sorry I forgot you before).
///Special thanks goes to UOT, Thraxus and Muzrin, thanx for the HUGE help guys.
///Also thanx for Crepti (you tried to help me there with the gump hues ;) ).
///Many of the locations were copied from Traveling Books script by Broze The Newb.
///Thanx to Wolf for adding some locations into the mix.
///
///Description:
///A moongate that hold all the locations and can be easily customized in the script.
///Added a UseGlobal bool value for each teleporter, when set to false that particular teleporter will have independent settings then the rest.
///Admin can turn on\off each map in the teleporter (effect all the teleporters in the world by default) via [props.
///Can be set to allow reds in tram\ilsh\malas\tokuno or not via [props (AllowAllMurdr will allow reds in all maps otherwise only in fel).
///The teleporter come with example custom map which is turned off by default, enable it with [props and set the locations inside the script.
///Use [worldtelegen to generate world teleporters in all the active connections (if trammel is active it'll add teleporters to all the locatios in trammel).
///
///Enjoy :)
////////////////////////////END WORLD TELEPORTER NOTES///////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
namespace Server.Items
{
	
	public class UseEntry
	{
		private string m_Name;
		private int m_Uses;
		private DateTime m_LastUse;
		
		public string Name{ get{ return m_Name; } }
		public int Uses{ get{ return m_Uses; } }
		public DateTime LastUse{ get{ return m_LastUse; } }
		
		public UseEntry( string name, int uses, DateTime lastUse )
		{
			m_Name = name;
			m_Uses = uses;
			m_LastUse = lastUse;
		}
	}
	
	public class WorldOmniporter : Item
	{
		[Flags]
		public enum OptFlags
		{
			None				= 0x00000000,
			Felucca				= 0x00000001, ///-- When 'true' transport is allowed to the 'Felucca' region
			Trammel				= 0x00000002, ///-- When 'true' transport is allowed to the 'Trammel' region
			Ilshenar			= 0x00000004, ///-- When 'true' transport is allowed to the 'Ilshenar' region
			Malas				= 0x00000008, ///-- When 'true' transport is allowed to the 'Malas' region
			Tokuno				= 0x00000010, ///-- When 'true' transport is allowed to the 'Tokuno' region
			TerMur				= 0x00000020, ///-- When 'true' transport is allowed to the 'TerMur' region
			//Thanimur				= 0x08000000, ///-- When 'true' transport is allowed to the 'Thanimur' region
			//Sosaria				= 0x08000001, ///-- When 'true' transport is allowed to the 'Thanimur' region
			Custom				= 0x00000400, ///-- When 'true' transport is allowed to the 'Custom' region
			UseGlobal			= 0x09000800, ///-- When 'true' changes made to this Omniporter affect all others, and vice versa
			PublicMoongates		= 0x09000010, ///-- When 'true' transport is allowed to the 'PublicMoongates' region
			IlshenarShrines		= 0x09000040, ///-- When 'true' transport is allowed to the 'IlshenarShrines' region
			TramDungeons		= 0x09000002, ///-- When 'true' transport is allowed to the 'TramDungeons' region
			FelDungeons			= 0x09000008, ///-- When 'true' transport is allowed to the 'FelDungeons' region
			AllowReds			= 0x09000200, ///-- When 'true' transport is allowed by Reds ( 5+ Murdercount )
			WO_OnlyAllowYoung	= 0x09001000, //##### -- % -- When 'true' only YOUNG players can use the Omniporter 
			WO_FeluccaNoYoung	= 0x09002000, //##### -- % -- When 'true' YOUNG players may not travel to Felucca
			WO_DungeonsNoYoung	= 0x09004000, //##### -- % -- When 'true' YOUNG players may not travel to Dungeons
			WO_AllPayToUse		= 0x09008000, //##### -- % -- When 'true' the Omniporter costs Gold to use each trip
			WO_SkillsCost		= 0x09010000, //##### -- % -- When 'true' higher skill totals make Omniporters more expensive
			WO_KarmaDiscount	= 0x09020000, //##### -- % -- When 'true' higher Karma gives players discounts on the cost
			WO_DelayAfterUse	= 0x09100000, //##### -- % -- When 'true' players must wait a TimeDelay after using the Omniporter
			WO_PayBeforeDelay	= 0x09200000, //##### -- % -- When 'true' players can pay to avoid waiting for the TimeDelay
			WO_PetsMayTravel	= 0x09400000, //##### -- % -- When 'true' pets may follow their owners through the Omniporter
			WO_PetsMustPay		= 0x09800000  //##### -- % -- When 'true' owners must pay for pets also
		}
		
		// These variables are Local or Global (static) and which one is used depends on the UseGlobal variable.
		private static	int 		m_GlobalBasePrice;					// Sets the Base price to travel - Global
		private 		int 		m_BasePrice;						//  - Local
		private static	int 		m_GlobalSkillsCostBonus;			// Adjusts Skill premium for travel - Global
		private 		int 		m_SkillsCostBonus;					//  - Local
		private static	int 		m_GlobalKarmaDiscountBonus;			// Adjusts Karma discount for travel - Global
		private 		int 		m_KarmaDiscountBonus;				//  - Local
		private static	TimeSpan 	m_GlobalTimeExpiration;				// Sets amount of time until Omni expires - Global
		private static	TimeSpan 	m_GlobalTimeDelay;					// Sets Delay between uses for each user.
		private 		TimeSpan 	m_TimeDelay;						//  - Local
		public static	Hashtable 	GlobalMobileUse = new Hashtable();	// Stores usage of all Omniporters as a group
		private 		Hashtable 	m_MobileUse = new Hashtable();		// Stores usage of a single Omni by users
		
		// These variables are Local only.
		private 		int 		m_LocalUses;						//  - Local - The number of uses of this Omni
		private 		DateTime 	m_Birth = DateTime.UtcNow;				//  - Local - The DateTime that the Omni was created
		
		[CommandProperty(AccessLevel.Administrator)]
		public int LocalUses { get { return m_LocalUses; } set { m_LocalUses = value < 0 ? 0 : value; } }
		
		[CommandProperty(AccessLevel.Administrator)]
		public DateTime Birth{ get{ return m_Birth; } set{ m_Birth = value; } }
		
		public string EntryName( Mobile m )
		{
			return GetUseEntry( m ).Name;
		}
		
		public int EntryUses( Mobile m )
		{
			int uses = 0;
			try {
				uses = GetUseEntry( m ).Uses;
			}
			catch{
				Console.WriteLine("Exception caught, so Adding '0' entry to GlobalMobileUse.");
				GlobalMobileUse = new Hashtable();
				try { GlobalMobileUse.Add( "0", new UseEntry( "None", 0, DateTime.UtcNow ) ); }
				catch { Console.WriteLine("Exception caught Adding '0' entry to GlobalMobileUse."); }
			
			}
			return uses;
		}
		
		public void RaiseUses( Mobile m, int raise )
		{
			int uses;
			try {
				uses = EntryUses( m );
				if ( uses < 1 ) RegisterUse( m, new UseEntry( m.Name, raise, DateTime.UtcNow ) );
				else RegisterUse( m, new UseEntry( m.Name, uses + raise, DateTime.UtcNow ) );
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception trapped in RaiseUses().");
				Console.WriteLine(e.ToString());
			}
			DeleteExpiredOmni();
		}

		private static void DeleteExpiredOmni()
		{
			ArrayList olist = new ArrayList();
			WorldOmniporter wo;

			foreach ( Item item in World.Items.Values )
			{
				if ( item is WorldOmniporter && !item.Movable ) 
				{
					wo = item as WorldOmniporter;
				}
			}

			foreach ( Item item in olist )
				item.Delete();

			if ( olist.Count > 0 )
				World.Broadcast( 0x35, true, "{0} world Omniporters expired.", olist.Count );
		}
		
		public UseEntry GetUseEntry( Mobile m )
		{
			try
			{ 
				if ( UseGlobal && GlobalMobileUse.ContainsKey( m.Serial.ToString() ) )
					return ( (UseEntry)GlobalMobileUse[ m.Serial.ToString() ] );
				else if ( !UseGlobal && m_MobileUse.ContainsKey( m.Serial.ToString() ) )
					return ( (UseEntry)m_MobileUse[ m.Serial.ToString() ] );
				else return ( (UseEntry)GlobalMobileUse["0"] ); 
			}
			catch
			{
				Console.WriteLine( "Exception trapped in GetUseEntry()." );
				return ( (UseEntry)GlobalMobileUse["0"] );
			}
			return ( (UseEntry)GlobalMobileUse["0"] );
		}
		
		public void RegisterUse( Mobile from, UseEntry entry )
		{
			PlayerMobile m;
			if ( from is PlayerMobile ) m = from as PlayerMobile;
			else { Console.WriteLine("Not a PlayerMobile using the Omniporter?"); return; }
			try {
				if ( UseGlobal )
				{
					if ( GlobalMobileUse.ContainsKey( m.Serial.ToString() ) )
						GlobalMobileUse.Remove( m.Serial.ToString() );
					GlobalMobileUse.Add( m.Serial.ToString(), entry );
				}
				else
				{
					if ( m_MobileUse.ContainsKey( m.Serial.ToString() ) )
						m_MobileUse.Remove( m.Serial.ToString() );
					m_MobileUse.Add( m.Serial.ToString(), entry );
				}
			}
			catch ( Exception e )
				{ Console.WriteLine( String.Format( "Error in RegisterUse: {0}.", e.ToString() ) ); }
		}
		
		[CommandProperty(AccessLevel.Administrator)]
		public int BasePrice{ get{ return UseGlobal? m_GlobalBasePrice: m_BasePrice; }
							set{ if (UseGlobal) m_GlobalBasePrice = value; else m_BasePrice = value; } }
		
		[CommandProperty(AccessLevel.Administrator)]
		public int SkillsCostBonus{ get{ return UseGlobal? m_GlobalSkillsCostBonus: m_SkillsCostBonus; }
							set{ if (UseGlobal) m_GlobalSkillsCostBonus = value; else m_SkillsCostBonus = value; } }
		
		[CommandProperty(AccessLevel.Administrator)]
		public int KarmaDiscountBonus{ get{ return UseGlobal? m_GlobalKarmaDiscountBonus: m_KarmaDiscountBonus; }
							set{ if (UseGlobal) m_GlobalKarmaDiscountBonus = value; else m_KarmaDiscountBonus = value; } }
		
		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan TimeDelay{ get{ return UseGlobal? m_GlobalTimeDelay: m_TimeDelay; }
							set{ if (UseGlobal) m_GlobalTimeDelay = value; else m_TimeDelay = value; } }
		
		public int GetPrice( Mobile m )
		{
			return GetPrice( m, 1, false );
		}
		
		public int GetPrice( Mobile m, int num, bool pets )
		{
			double k = (double)KarmaDiscountBonus;
			double s = (double)SkillsCostBonus;
			double price = (double)BasePrice;
			double cap = (double)m.Skills.Cap;
			double tot = (double)m.Skills.Total;
			double delay = TimeDelay.TotalMinutes;
			double next = ( NextUseTime( m ) - DateTime.UtcNow ).TotalMinutes;
			tot = tot > cap ? cap : tot;
			if ( WO_AllPayToUse || ( WO_DelayAfterUse && WO_PayBeforeDelay && NextUseTime( m ) > DateTime.UtcNow ) )
			{
				if ( WO_SkillsCost ) price = ( ( price + s ) / ( 1.05 - ( tot / cap ) ) );
				if ( WO_KarmaDiscount ) price = ( ( price - k ) / ( m.Karma > 9999? 4: m.Karma > 4999? 3: m.Karma > 1999? 2: 1 ) );
				if ( !WO_AllPayToUse ) price *= ( delay + ( next < 0? 0: next ) ) / delay;
				if ( price < 0.0 ) price = 0.0;
				if ( pets && WO_PetsMustPay ) price *= num;
				return (int)Math.Ceiling( price );
			}
			return 0;
		}

		public override void OnSingleClick( Mobile from )
		{
			from.SendMessage( NextUseMessage( from ) );
		}
		
		public DateTime NextUseTime( Mobile m )
		{
			if ( !WO_DelayAfterUse ) return DateTime.UtcNow;
			DateTime dt = GetUseEntry( m ).LastUse;
			if ( dt == DateTime.MinValue ) return DateTime.UtcNow;
			else return ( dt + TimeDelay );
		}
		
		public string NextUseMessage( Mobile m )
		{
			return NextUseMessage( m, 1, false );
		}
		
		public string NextUseMessage( Mobile m, int num, bool pets )
		{
			string delay = "";
			string pay = "";
			string reason = "";
			if ( pets && !WO_PetsMayTravel ) reason = " because your pets might try to follow you";
			if ( CanUse( m, pets ) )
			{
				if ( !MustWait( m ) && !MustPay( m, pets ) )
					return "You may use the Omniporter now.";
				int minutes = (int)Math.Ceiling( ( (TimeSpan)( NextUseTime( m ) - DateTime.UtcNow ) ).TotalMinutes );
				if ( MustPay( m, pets ) )
				{
					if ( !CanDelay( m ) ) pay = String.Format( "\nYou must pay {0} Gold to use {1} Omniporter.",
						GetPrice( m, num, pets ), UseGlobal? "any": "this" );
					else delay = String.Format( " or pay {0} Gold now", GetPrice( m, num, pets ) );
				}
				if ( MustWait( m ) ) pay = String.Format( "You must wait {0} minute{1} to use {2} Omniporter{3}.{4}", 
					minutes, minutes == 1? "": "s", UseGlobal? "any": "this", delay, pay );
				return ( String.Format( pay ) );
			}
			else
				return String.Format( "You may not use {0} Omniporter{1}.", UseGlobal? "any": "this", reason );
		}
		
		public bool CanDelay( Mobile m )
		{
			if ( WO_DelayAfterUse && !WO_AllPayToUse ) return true;
			return false;
		}
		
		public bool MustWait( Mobile m )
		{
			if ( WO_DelayAfterUse && !WO_PayBeforeDelay && NextUseTime( m ) > DateTime.UtcNow ) return true;
			return false;
		}
		
		public bool MustPay( Mobile m, bool pets )
		{
			if ( pets && WO_PetsMayTravel && WO_PetsMustPay ) return true;
			
			if ( WO_AllPayToUse || ( WO_DelayAfterUse && WO_PayBeforeDelay && NextUseTime( m ) > DateTime.UtcNow ) ) return true;
			return false;
		}
		
		public bool MustPay( Mobile m )
		{
			return MustPay( m, false );
		}
		
		public bool CanUse( Mobile from, bool dungeon, bool felucca )
		{
			PlayerMobile m;
			if ( from is PlayerMobile ) m = from as PlayerMobile; else return false;
			if ( !AllowReds && m.Kills > 4 ) return false;
			if ( WO_OnlyAllowYoung && !m.Young ) return false;
			if ( WO_FeluccaNoYoung && felucca && m.Young ) return false;
			if ( WO_DungeonsNoYoung && dungeon && m.Young ) return false;
			return true;
		}
		
		public bool CanUse( Mobile from, bool pets )
		{
			if ( pets && !WO_PetsMayTravel ) return false;
			return CanUse( from );
		}
		
		public bool CanUse( Mobile from )
		{
			PlayerMobile m;
			if ( from is PlayerMobile ) m = from as PlayerMobile; else return false;
			if ( !AllowReds && m.Kills > 4 ) return false;
			if ( WO_OnlyAllowYoung && !m.Young ) return false;
			return true;
		}

		public static void Initialize()
		{
			
			//Global Default Flags for WorldOmniGen: 0x1FF
			if ( m_GlobalFlags == OptFlags.None )
			{
				SetOptFlag( ref m_GlobalFlags, OptFlags.Trammel, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.TramDungeons, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Felucca, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.FelDungeons, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.PublicMoongates, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Ilshenar, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.IlshenarShrines, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Malas, Core.AOS );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Tokuno, Core.SE );
				SetOptFlag( ref m_GlobalFlags, OptFlags.TerMur, Core.SA );
				//SetOptFlag( ref m_GlobalFlags, OptFlags.Thanimur, true );
				//SetOptFlag( ref m_GlobalFlags, OptFlags.Sosaria, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Custom, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.UseGlobal, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_OnlyAllowYoung, false );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_FeluccaNoYoung, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_DungeonsNoYoung, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_AllPayToUse, false );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_SkillsCost, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_KarmaDiscount, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_DelayAfterUse, false );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_PayBeforeDelay, false );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_PetsMayTravel, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.WO_PetsMustPay, false );
			}

			GlobalEntries.Add("Trammel", new OmniEntry[]
			{
				new OmniEntry("Britain", new Point3D(1434, 1699, 2), Map.Trammel ),
				new OmniEntry("Bucs Den", new Point3D(2705, 2162, 0), Map.Trammel ),
				new OmniEntry("Cove", new Point3D(2237, 1214, 0), Map.Trammel ),
				new OmniEntry("Delucia", new Point3D(5274, 3991, 37), Map.Trammel ),
				new OmniEntry( "New Haven", new Point3D(3493, 2577, 14), Map.Trammel ),
				new OmniEntry("Jhelom", new Point3D(1417, 3821, 0), Map.Trammel ),
				new OmniEntry("Magincia", new Point3D(3728, 2164, 20), Map.Trammel ),
				new OmniEntry("Minoc", new Point3D(2525, 582, 0), Map.Trammel ),
				new OmniEntry("Moonglow", new Point3D(4471, 1177, 0), Map.Trammel ),
				new OmniEntry("Nujel'm", new Point3D(3770, 1308, 0), Map.Trammel ),
				new OmniEntry("Papua", new Point3D(5729, 3208, -6), Map.Trammel ),
				new OmniEntry("Serpents Hold", new Point3D(2895, 3479, 15), Map.Trammel ),
				new OmniEntry("Skara Brae", new Point3D(596, 2138, 0), Map.Trammel ),
				new OmniEntry("Trinsic", new Point3D(1823, 2821, 0), Map.Trammel ),
				new OmniEntry("Vesper", new Point3D(2899, 676, 0), Map.Trammel ),
				new OmniEntry("Wind", new Point3D(1361, 895, 0), Map.Trammel ),
				new OmniEntry("Yew", new Point3D(542, 985, 0), Map.Trammel )
			});

			GlobalEntries.Add("Trammel Dungeons", new OmniEntry[]
			{
			    new OmniEntry("Blighted Grove", new Point3D(586, 1643, -5), Map.Trammel ),
				new OmniEntry("Covetous", new Point3D(2498, 921, 0), Map.Trammel ),
				new OmniEntry("Daemon Temple", new Point3D(4591, 3647, 80), Map.Trammel ),
				new OmniEntry("Deceit", new Point3D(4111, 434, 5), Map.Trammel ),
				new OmniEntry("Despise", new Point3D(1301, 1080, 0), Map.Trammel ),
				new OmniEntry("Destard", new Point3D(1176, 2640, 2), Map.Trammel ),
				new OmniEntry("Fire", new Point3D(2923, 3409, 8), Map.Trammel ),
				new OmniEntry("Hythloth", new Point3D(4721, 3824, 0), Map.Trammel ),
				new OmniEntry("Ice", new Point3D(1999, 81, 4), Map.Trammel ),
				new OmniEntry("Ophidian Temple", new Point3D(5766, 2634, 43), Map.Trammel ),
				new OmniEntry("Orc Caves", new Point3D(1017, 1429, 0), Map.Trammel ),
				new OmniEntry("Painted Caves", new Point3D(1716, 2993, 0), Map.Trammel ),
				new OmniEntry("Paroxysmus", new Point3D(5569, 3019, 31), Map.Trammel ),
				new OmniEntry("Prism of Light", new Point3D(3789, 1095, 20), Map.Trammel ),
				new OmniEntry("Sanctuary", new Point3D(759, 1642, 0), Map.Trammel ),
				new OmniEntry("Shame", new Point3D(511, 1565, 0), Map.Trammel ),
				new OmniEntry("Solen Hive", new Point3D(2607, 763, 0), Map.Trammel ),
				new OmniEntry("Terathan Keep", new Point3D(5451, 3143, -60), Map.Trammel ),
				new OmniEntry("Wrong", new Point3D(2043, 238, 10), Map.Trammel )
			});

			GlobalEntries.Add("Felucca", new OmniEntry[]
			{
				new OmniEntry("Britain", new Point3D(1434, 1699, 2), Map.Felucca ),
				new OmniEntry("Bucs Den", new Point3D(2705, 2162, 0), Map.Felucca ),
				new OmniEntry("Cove", new Point3D(2237, 1214, 0), Map.Felucca ),
				new OmniEntry("Delucia", new Point3D(5274, 3991, 37), Map.Felucca ),
				new OmniEntry("Jhelom", new Point3D(1417, 3821, 0), Map.Felucca ),
				new OmniEntry("Magincia", new Point3D(3728, 2164, 20), Map.Felucca ),
				new OmniEntry("Minoc", new Point3D(2525, 582, 0), Map.Felucca ),
				new OmniEntry("Moonglow", new Point3D(4471, 1177, 0), Map.Felucca ),
				new OmniEntry("Nujel'm", new Point3D(3770, 1308, 0), Map.Felucca ),
				new OmniEntry("Ocllo", new Point3D(3626, 2611, 0), Map.Felucca ),
				new OmniEntry("Papua", new Point3D(5729, 3208, -6), Map.Felucca ),
				new OmniEntry("Serpents Hold", new Point3D(2895, 3479, 15), Map.Felucca ),
				new OmniEntry("Skara Brae", new Point3D(596, 2138, 0), Map.Felucca ),
				new OmniEntry("Trinsic", new Point3D(1823, 2821, 0), Map.Felucca ),
				new OmniEntry("Vesper", new Point3D(2899, 676, 0), Map.Felucca ),
				new OmniEntry("Wind", new Point3D(1361, 895, 0), Map.Felucca ),
				new OmniEntry("Yew", new Point3D(542, 985, 0), Map.Felucca )
			});

			GlobalEntries.Add("Felucca Dungeons", new OmniEntry[]
			{
			    new OmniEntry("Blighted Grove", new Point3D(586, 1643, -5), Map.Felucca ),
				new OmniEntry("Covetous", new Point3D(2498, 921, 0), Map.Felucca ),
				new OmniEntry("Daemon Temple", new Point3D(4591, 3647, 80), Map.Felucca ),
				new OmniEntry("Deceit", new Point3D(4111, 434, 5), Map.Felucca ),
				new OmniEntry("Despise", new Point3D(1301, 1080, 0), Map.Felucca ),
				new OmniEntry("Destard", new Point3D(1176, 2640, 2), Map.Felucca ),
				new OmniEntry("Fire", new Point3D(2923, 3409, 8), Map.Felucca ),
				new OmniEntry("Hythloth", new Point3D(4721, 3824, 0), Map.Felucca ),
				new OmniEntry("Ice", new Point3D(1999, 81, 4), Map.Felucca ),
				new OmniEntry("Ophidian Temple", new Point3D(5766, 2634, 43), Map.Felucca ),
				new OmniEntry("Orc Caves", new Point3D(1017, 1429, 0), Map.Felucca ),
				new OmniEntry("Painted Caves", new Point3D(1716, 2993, 0), Map.Felucca ),
				new OmniEntry("Paroxysmus", new Point3D(5569, 3019, 31), Map.Felucca ),
				new OmniEntry("Prism of Light", new Point3D(3789, 1095, 20), Map.Felucca ),
				new OmniEntry("Sanctuary", new Point3D(759, 1642, 0), Map.Felucca ),
				new OmniEntry("Shame", new Point3D(511, 1565, 0), Map.Felucca ),
				new OmniEntry("Solen Hive", new Point3D(2607, 763, 0), Map.Felucca ),
				new OmniEntry("Terathan Keep", new Point3D(5451, 3143, -60), Map.Felucca ),
				new OmniEntry("Wrong", new Point3D(2043, 238, 10), Map.Felucca )
			});

			GlobalEntries.Add("Public Moongates", new OmniEntry[]
			{
				new OmniEntry("Britain", new Point3D(1336, 1997, 5), Map.Trammel, true ),
				new OmniEntry( "New Haven", new Point3D(3450, 2677, 25), Map.Trammel ),
				new OmniEntry("Jhelom", new Point3D(1499, 3771, 5), Map.Trammel, true ),
				new OmniEntry("Magincia", new Point3D(3563, 2139, 34), Map.Trammel, true ),
				new OmniEntry("Minoc", new Point3D(2701, 692, 5), Map.Trammel, true ),
				new OmniEntry("Moonglow", new Point3D(4467, 1283, 5), Map.Trammel, true ),
				new OmniEntry("Skara Brae", new Point3D(643, 2067, 5), Map.Trammel, true ),
				new OmniEntry("Trinsic", new Point3D(1828, 2948, -20), Map.Trammel, true ),
				new OmniEntry("Yew", new Point3D(771, 752, 5), Map.Trammel, true ),
				new OmniEntry("Britain", new Point3D(1336, 1997, 5), Map.Felucca, true ),
				new OmniEntry("Buccaneer's Den", new Point3D(2711, 2234, 0), Map.Felucca, true ),
				new OmniEntry("Jhelom", new Point3D(1499, 3771, 5), Map.Felucca, true ),
				new OmniEntry("Magincia", new Point3D(3563, 2139, 34), Map.Felucca, true ),
				new OmniEntry("Minoc", new Point3D(2701, 692, 5), Map.Felucca, true ),
				new OmniEntry("Moonglow", new Point3D(4467, 1283, 5), Map.Felucca, true ),
				new OmniEntry("Skara Brae", new Point3D(643, 2067, 5), Map.Felucca, true ),
				new OmniEntry("Trinsic", new Point3D(1828, 2948, -20), Map.Felucca, true ),
				new OmniEntry("Yew", new Point3D(771, 752, 5), Map.Felucca, true )

			});

			GlobalEntries.Add("Ilshenar", new OmniEntry[]
			{
				new OmniEntry("Ankh Dungeon", new Point3D(576, 1150, -100), Map.Ilshenar ),
				new OmniEntry("Blood Dungeon", new Point3D(1747, 1171, -2), Map.Ilshenar ),
				new OmniEntry("Exodus Dungeon", new Point3D(854, 778, -80), Map.Ilshenar ),
				new OmniEntry("Gargoyle City", new Point3D(860, 599, -40), Map.Ilshenar ),
				new OmniEntry("Lakeshire", new Point3D(1203, 1124, -25), Map.Ilshenar ),
				new OmniEntry("Mistas", new Point3D(819, 1130, -29), Map.Ilshenar ),
				new OmniEntry("Montor", new Point3D(1706, 205, 104), Map.Ilshenar ),
				new OmniEntry("Rock Dungeon", new Point3D(1787, 572, 69), Map.Ilshenar ),
				new OmniEntry("Savage Camp", new Point3D(1151, 659, -80), Map.Ilshenar ),
				new OmniEntry("Sorceror's Dungeon", new Point3D(548, 462, -53), Map.Ilshenar ),
				new OmniEntry("Spectre Dungeon", new Point3D(1363, 1033, -8), Map.Ilshenar ),
				new OmniEntry("Spider Cave", new Point3D(1420, 913, -16), Map.Ilshenar ),
				new OmniEntry("Wisp Dungeon", new Point3D(651, 1302, -58), Map.Ilshenar )
			});

			GlobalEntries.Add("Ilshenar Shrines", new OmniEntry[]
			{
				new OmniEntry("Compassion", new Point3D(1215, 467, -13), Map.Ilshenar ),
				new OmniEntry("Honesty", new Point3D(722, 1366, -60), Map.Ilshenar ),
				new OmniEntry("Honor", new Point3D(744, 724, -28), Map.Ilshenar ),
				new OmniEntry("Humility", new Point3D(281, 1016, 0), Map.Ilshenar ),
				new OmniEntry("Justice", new Point3D(987, 1011, -32), Map.Ilshenar ),
				new OmniEntry("Sacrifice", new Point3D(1174, 1286, -30), Map.Ilshenar ),
				new OmniEntry("Spirituality", new Point3D(1532, 1340, -3), Map.Ilshenar ),
				new OmniEntry("Valor", new Point3D(528, 216, -45), Map.Ilshenar ),
				new OmniEntry("Choas", new Point3D(1721, 218, 96), Map.Ilshenar )
			});

			GlobalEntries.Add("Malas", new OmniEntry[]
			{
				new OmniEntry("Doom", new Point3D(2368, 1267, -85), Map.Malas ),
				new OmniEntry("Labyrinth", new Point3D(1730, 981, -80), Map.Malas ),
				new OmniEntry("Luna", new Point3D(1015, 527, -65), Map.Malas, true ),
				new OmniEntry("Three Dragons Pub", new Point3D(1020, 570, -90), Map.Malas ),
				new OmniEntry("Hanse's Hostel", new Point3D(1069, 1443, -90), Map.Malas, true ),
				new OmniEntry("Orc Fort 1", new Point3D(912, 215, -90), Map.Malas ),
				new OmniEntry("Orc Fort 2", new Point3D(1678, 374, -50), Map.Malas ),
				new OmniEntry("Orc Fort 3", new Point3D(1375, 621, -86), Map.Malas ),
				new OmniEntry("Orc Fort 4", new Point3D(1184, 715, -89), Map.Malas ),
				new OmniEntry("Orc Fort 5", new Point3D(1279, 1324, -90), Map.Malas ),
				new OmniEntry("Orc Fort 6", new Point3D(1598, 1834, -107), Map.Malas ),
				new OmniEntry("Ruined Temple", new Point3D(1598, 1762, -110), Map.Malas ),
				new OmniEntry("Umbra", new Point3D(1997, 1386, -85), Map.Malas, true )
			});

			GlobalEntries.Add("Tokuno", new OmniEntry[]
			{
				new OmniEntry("Bushido Dojo", new Point3D(322, 430, 32), Map.Tokuno ),
				new OmniEntry("Crane Marsh", new Point3D(203, 985, 18), Map.Tokuno ),
				new OmniEntry("Fan Dancer's Dojo", new Point3D(970, 222, 23), Map.Tokuno ),
				new OmniEntry("Isamu-Jima", new Point3D(1169, 998, 41), Map.Tokuno ),
				new OmniEntry("Makoto-Jima", new Point3D(802, 1204, 25), Map.Tokuno ),
				new OmniEntry("Homare-Jima", new Point3D(270, 628, 15), Map.Tokuno ),
				new OmniEntry("Makoto Desert", new Point3D(724, 1050, 33), Map.Tokuno ),
				new OmniEntry("Makoto Zento", new Point3D(741, 1261, 30), Map.Tokuno ),
				new OmniEntry("Mt. Sho Castle", new Point3D(1234, 772, 3), Map.Tokuno ),
				new OmniEntry("Valor Shrine", new Point3D(1044, 523, 15), Map.Tokuno ),
				new OmniEntry("Yomotsu Mine", new Point3D(257, 786, 63), Map.Tokuno )
			});
			
			GlobalEntries.Add("TerMur", new OmniEntry[]
			{
				new OmniEntry("Royal City", new Point3D(852, 3526, -43), Map.TerMur),
				new OmniEntry("Holy City", new Point3D(926, 3989, -36), Map.TerMur),
				new OmniEntry("Fisherman's Reach", new Point3D(612, 3038, 35), Map.TerMur),
				new OmniEntry("Tomb of Kings", new Point3D(997, 3843, -41), Map.TerMur),
				new OmniEntry("Underworld", new Point3D(4194, 3268, 0), Map.Trammel)
			});
			
			//GlobalEntries.Add("Thanimur", new OmniEntry[]
			//{
				//new OmniEntry("Small Town 1", new Point3D(2086, 2168, 0), Map.Thanimur),
				//new OmniEntry("Small Town 2", new Point3D(2837, 2219, 0), Map.Thanimur),
				//new OmniEntry("Small Town 3", new Point3D(889, 952, 2), Map.Thanimur),
				//new OmniEntry("Small Town 4", new Point3D(2332, 3156, 0), Map.Thanimur),
				//new OmniEntry("Small Town 5", new Point3D(2173, 2777, 2), Map.Thanimur),
				//new OmniEntry("Small Town 6", new Point3D(847, 2025, 0), Map.Thanimur),
				//new OmniEntry("Small Town 7", new Point3D(2679, 3194, 0), Map.Thanimur),
				//new OmniEntry("Small Town 8", new Point3D(4221, 2977, 0), Map.Thanimur),
				//new OmniEntry("Medium Town 1", new Point3D(4225, 1457, 0), Map.Thanimur),
				//new OmniEntry("Medium Town 2", new Point3D(3636, 413, 0), Map.Thanimur),
				//new OmniEntry("Large Town 1", new Point3D(1871, 2211, 0), Map.Thanimur),
				//new OmniEntry("Large Town 2", new Point3D(2911, 1276, 0), Map.Thanimur)				
			//});
			
			//GlobalEntries.Add("Sosaria", new OmniEntry[]
			//{
			//new OmniEntry("LCB's Town", new Point3D(2995, 1021, 0), Map.Sosaria),
			//new OmniEntry("Moon", new Point3D(838, 769, 0), Map.Sosaria),
			//new OmniEntry("Grey", new Point3D(876, 2077, 0), Map.Sosaria),
			//new OmniEntry("Montor East", new Point3D(3100, 2614, 0), Map.Sosaria),
			//new OmniEntry("Montor West", new Point3D(3277, 2646, 0), Map.Sosaria),
			//new OmniEntry("Devil Guard", new Point3D(1641, 1467, 2), Map.Sosaria),
			//new OmniEntry("Old Yew", new Point3D(2432, 875, 2), Map.Sosaria),
			//new OmniEntry("Fawn", new Point3D(2087, 270, 0), Map.Sosaria),
			//new OmniEntry("Dawn", new Point3D(5919, 2881, 0), Map.Sosaria),
			//new OmniEntry("Death Gulch", new Point3D(3717, 1543, 0), Map.Sosaria),
			//new OmniEntry("Pirate Isle", new Point3D(1818, 2232, 0), Map.Sosaria),
			//new OmniEntry("Ancient Pyramid", new Point3D(1168, 474, 2), Map.Sosaria),
			//new OmniEntry("Clues", new Point3D(3758, 2045, 0), Map.Sosaria),
			//new OmniEntry("Dardin's Pit", new Point3D(3007, 451, 0), Map.Sosaria),
			//new OmniEntry("Doom", new Point3D(1632, 2558, 0), Map.Sosaria),
			//new OmniEntry("Fires of Hell", new Point3D(3338, 1656, 0), Map.Sosaria),
			//new OmniEntry("Mines of Morinia", new Point3D(1022, 1372, 2), Map.Sosaria),
			//new OmniEntry("Perins Depths", new Point3D(3619, 460, 0), Map.Sosaria),
			//new OmniEntry("Time Awaits", new Point3D(3831, 1508, 4), Map.Sosaria),
			//});

			GlobalEntries.Add("Custom", new OmniEntry[] //add locations to the custom map here
			{
				new OmniEntry("LostLands Shop", new Point3D(720, 1947, -84), Map.Malas ),
				new OmniEntry("LostLands Deco Shop", new Point3D(6542, 2353, 0), Map.Trammel ),
                new OmniEntry("Champ Room", new Point3D(6103, 1238, 44), Map.Trammel ),
				new OmniEntry("The Arena", new Point3D(877, 1858, 0), Map.Felucca ),
				new OmniEntry("Training Room", new Point3D(5407, 1109, 6), Map.Trammel ),
				new OmniEntry("Hue Room", new Point3D(6016, 539, 5), Map.Trammel ),
				// new OmniEntry("Hanse's Hostel", new Point3D(1069, 1443, -90), Map.Malas ),
				new OmniEntry("Gamble Shop", new Point3D(4479, 1126, 0), Map.Trammel ),
				new OmniEntry("Rune Libary", new Point3D(1892, 2830, 30), Map.Trammel ),
				new OmniEntry("Outdoor Wedding Chappel", new Point3D(1306, 572, 30), Map.Trammel )
			});
			
			

		}

		public static int GenerateWorldOmniporters()
		{
			int gen = 0;

			if (GetOptFlag( m_GlobalFlags, OptFlags.Trammel )) gen += GenerateEntry( "Trammel" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.TramDungeons )) gen += GenerateEntry( "Trammel Dungeons" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Felucca )) gen += GenerateEntry( "Felucca" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.FelDungeons )) gen += GenerateEntry( "Felucca Dungeons" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.PublicMoongates)) gen += GenerateEntry( "Public Moongates" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Ilshenar)) gen += GenerateEntry( "Ilshenar" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.IlshenarShrines)) gen += GenerateEntry( "Ilshenar Shrines" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Malas) && Core.AOS) gen += GenerateEntry( "Malas" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Tokuno) && Core.SE) gen += GenerateEntry( "Tokuno" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.TerMur) && Core.SA) gen += GenerateEntry( "TerMur" );
			//gen += GenerateEntry( "Thanimur" );
			//gen += GenerateEntry( "Sosaria" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Custom)) gen += GenerateEntry( "Custom" );
	
			return gen;
		}

		private static int GenerateEntry( string map )
		{
			OmniEntry[] oe = (OmniEntry[])GlobalEntries[map];
			if ( oe != null )
			{
				for (int i = 0; i < oe.Length; i++)
					new WorldOmniporter(oe[i].Moongate ? true : false).MoveToWorld( oe[i].Destination, oe[i].Map );
				return oe.Length;
			}
			return 0;
		}

		public static Hashtable GlobalEntries = new Hashtable();
		private OptFlags m_Flags;
		private static OptFlags m_GlobalFlags;

		[CommandProperty(AccessLevel.Administrator)]
		public bool Trammel{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Trammel ); } set{ SetOptFlag( OptFlags.Trammel, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool TramDungeons{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.TramDungeons ); } set{ SetOptFlag( OptFlags.TramDungeons, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Felucca{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Felucca ); } set{ SetOptFlag( OptFlags.Felucca, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool FelDungeons{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.FelDungeons ); } set{ SetOptFlag( OptFlags.FelDungeons, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool PublicMoongates{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.PublicMoongates ); } set{ SetOptFlag( OptFlags.PublicMoongates, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Ilshenar{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Ilshenar ); } set{ SetOptFlag( OptFlags.Ilshenar, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool IlshenarShrines{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.IlshenarShrines ); } set{ SetOptFlag( OptFlags.IlshenarShrines, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Malas{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Malas ); } set{ SetOptFlag( OptFlags.Malas, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Tokuno{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Tokuno ); } set{ SetOptFlag( OptFlags.Tokuno, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool TerMur{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.TerMur ); } set{ SetOptFlag( OptFlags.TerMur, value ); } }
		
		//[CommandProperty(AccessLevel.Administrator)]
		//public bool Thanimur{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Trammel ); } set{ SetOptFlag( OptFlags.Trammel, value ); } }
		
		//[CommandProperty(AccessLevel.Administrator)]
		//public bool Sosaria{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Sosaria ); } set{ SetOptFlag( OptFlags.Trammel, value ); } }
		
		[CommandProperty(AccessLevel.Administrator)]
		public bool AllowReds{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.AllowReds ); } set{ SetOptFlag( OptFlags.AllowReds, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Custom{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Custom ); } set{ SetOptFlag( OptFlags.Custom, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool UseGlobal{ get{ return GetOptFlag( m_Flags, OptFlags.UseGlobal ); } set{ SetOptFlag( ref m_Flags, OptFlags.UseGlobal, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_OnlyAllowYoung{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_OnlyAllowYoung ); } set{ SetOptFlag( OptFlags.WO_OnlyAllowYoung, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_FeluccaNoYoung{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_FeluccaNoYoung ); } set{ SetOptFlag( OptFlags.WO_FeluccaNoYoung, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_DungeonsNoYoung{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_DungeonsNoYoung ); } set{ SetOptFlag( OptFlags.WO_DungeonsNoYoung, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_AllPayToUse{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_AllPayToUse ); } set{ SetOptFlag( OptFlags.WO_AllPayToUse, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_SkillsCost{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_SkillsCost ); } set{ SetOptFlag( OptFlags.WO_SkillsCost, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_KarmaDiscount{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_KarmaDiscount ); } set{ SetOptFlag( OptFlags.WO_KarmaDiscount, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_DelayAfterUse{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_DelayAfterUse ); } set{ SetOptFlag( OptFlags.WO_DelayAfterUse, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_PayBeforeDelay{ get{ if ( !WO_DelayAfterUse ) return false; return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_PayBeforeDelay ); } set{ SetOptFlag( OptFlags.WO_PayBeforeDelay, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_PetsMayTravel{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_PetsMayTravel ); } set{ SetOptFlag( OptFlags.WO_PetsMayTravel, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool WO_PetsMustPay{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.WO_PetsMustPay ); } set{ SetOptFlag( OptFlags.WO_PetsMustPay, value ); } }

		public void SetOptFlag( OptFlags toSet, bool value )
		{
			if ( UseGlobal )
			{
				if ( value )
					m_GlobalFlags |= toSet;
				else
					m_GlobalFlags &= ~toSet;
			}
			else
			{
				if ( value )
					m_Flags |= toSet;
				else
					m_Flags &= ~toSet;
			}
		}

		public static void SetOptFlag( ref OptFlags flags, OptFlags toSet, bool value )
		{
			if ( value )
				flags |= toSet;
			else
				flags &= ~toSet;
		}

		public static bool GetOptFlag( OptFlags flags, OptFlags flag )
		{
			return ( (flags & flag) != 0 );
		}

	//	[Constructable]
	//	public WorldOmniporter() : this ( (int)m_GlobalFlags )
	//	{
	//	}

		[Constructable]
		public WorldOmniporter(bool moongate) : this ( (int)m_GlobalFlags, moongate )
		{
		}

		[Constructable]
		public WorldOmniporter( int flags ) : this( flags, false )
		{
		}

		[Constructable]
		//public WorldOmniporter( int flags, bool moongate ) : base( moongate? 0xF6C : 7107 )  //Original Daat99 MoonGate Animation
		public WorldOmniporter( int flags, bool moongate ) : base( 7107 )  //MoonGate Animation
		{
			{
			this.Movable = false;
			this.Light = LightType.Circle300;
			}
			
			Hue = 38;
			Name = "World Omniporter";
			
			m_Flags = (OptFlags)flags;
			m_MobileUse = (Hashtable)GlobalMobileUse;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.Player )
				return;
			UseOmniporter(from);
		}

		public override bool OnMoveOver( Mobile from )
		{
			return !from.Player || UseOmniporter(from);
		}
		
		private bool LoadUses( Mobile m )
		{
			OmniEntry[] oe = new OmniEntry[m_Entries.Length];
			
			try {
				for ( int x=0; x<m_Entries.Length; x++ )
				{
					oe = (OmniEntry[])WorldOmniporter.GlobalEntries[m_Entries[x]];
					
					for ( int y=0; y<oe.Length; y++ )
					{
						OmniEntry entry = oe[y];
						try {
							entry.ToCount = loadTo[x][y];
							entry.FromCount = loadFrom[x][y];
						}
						catch (Exception e)
						{
							if (InitializeCounts()) 
							{
								entry.ToCount = loadTo[x][y];
								entry.FromCount = loadFrom[x][y];
							}
							else
							{
								entry.ToCount = 0;
								entry.FromCount = 0;
							}
						}
					}
				}
			}
			catch ( Exception e )
			{
				//This is a legacy catch - should never happen...
				m.SendMessage( "Omniporters have not been initialized. Please restart server." );
				Console.WriteLine(e.ToString());
				return false;
			}
			
			loaded = true;
			return true;
		}
		
		public bool InitializeCounts()
		{
			Console.WriteLine("Initializing Counts...");
			try {
				OmniEntry[] oe;
				
				int outlength = m_Entries.Length;
				int inlength;
				
				loadTo = new int[outlength][];
				loadFrom = new int[outlength][];
				
				for ( int x=0; x<outlength; x++ )
				{
					oe = (OmniEntry[])WorldOmniporter.GlobalEntries[m_Entries[x]];
					
					inlength = oe.Length;
					loadTo[x] = new int[inlength];
					loadFrom[x] = new int[inlength];
					
					for ( int y=0; y<oe.Length; y++ )
					{
						loadTo[x][y] = 0;
						loadFrom[x][y] = 0;
					}
				}
				return true;
			}
			catch ( Exception e ) { Console.WriteLine(e.ToString()); }
			return false;
		}

		public bool UseOmniporter( Mobile m )
		{
			if ( !loaded && !LoadUses( m ) ) return false;
			if ( m.Criminal )
				m.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
			else if ( Server.Spells.SpellHelper.CheckCombat( m ) )
				m.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
			else if ( m.Spell != null )
				m.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment.
			else
			{
				m.CloseGump( typeof( WorldOmniporterGump ) );
				m.SendGump( new WorldOmniporterGump( m, this, 0 ) );

				if ( !m.Hidden || m.AccessLevel == AccessLevel.Player )
					Effects.PlaySound( m.Location, m.Map, 0x20E );
				return true;
			}
			return false;
		}

		public WorldOmniporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version
			
			OmniEntry[] oe;
			
			for ( int x=0; x<m_Entries.Length; x++ )
			{
				oe = (OmniEntry[])WorldOmniporter.GlobalEntries[m_Entries[x]];
				writer.Write( (int) oe.Length );
				for ( int y=0; y<oe.Length; y++ )
				{
					OmniEntry entry = oe[y];
					writer.Write( (int) entry.ToCount );
					writer.Write( (int) entry.FromCount );
				}
			}

			writer.Write( (int) m_LocalUses ); // version 2
			
			writer.Write( (int) m_GlobalBasePrice ); // version 1
			writer.Write( (int) m_BasePrice );
			writer.Write( (int) m_GlobalSkillsCostBonus );
			writer.Write( (int) m_SkillsCostBonus );
			writer.Write( (int) m_GlobalKarmaDiscountBonus );
			writer.Write( (int) m_KarmaDiscountBonus );
			writer.Write( (int) m_GlobalTimeDelay.TotalSeconds );
			writer.Write( (int) m_TimeDelay.TotalSeconds );
			
			writer.Write( (int) m_Flags ); // version 0
			writer.Write( (int) m_GlobalFlags );
		}
		
		private int[][] loadTo;
		private int[][] loadFrom;
		private static bool loaded;  // Global variable: we load all uses at once.

		public override void Deserialize( GenericReader reader )
		{
			loaded = false;
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			switch( version )
			{
				case 3:
				{
					int outlength = m_Entries.Length;
					int inlength;
					
					loadTo = new int[outlength][];
					loadFrom = new int[outlength][];
					
					for ( int x=0; x<outlength; x++ )
					{
						inlength = reader.ReadInt();
						loadTo[x] = new int[inlength];
						loadFrom[x] = new int[inlength];
						
						for ( int y=0; y<loadTo[x].Length; y++ )
						{
							loadTo[x][y] = reader.ReadInt();
							loadFrom[x][y] = reader.ReadInt();
						}
					}
					goto case 2;
				}
				case 2:
				{
					m_LocalUses = reader.ReadInt();
					goto case 1;
				}
				case 1: 
				{
					m_GlobalBasePrice = reader.ReadInt();
					m_BasePrice = reader.ReadInt();
					m_GlobalSkillsCostBonus = reader.ReadInt();
					m_SkillsCostBonus = reader.ReadInt();
					m_GlobalKarmaDiscountBonus = reader.ReadInt();
					m_KarmaDiscountBonus = reader.ReadInt();
					m_GlobalTimeDelay = TimeSpan.FromSeconds( reader.ReadInt() );
					m_TimeDelay = TimeSpan.FromSeconds( reader.ReadInt() );
					GlobalMobileUse = new Hashtable();
					m_MobileUse = new Hashtable();
					try { GlobalMobileUse.Add( "0", new UseEntry( "None", 0, DateTime.UtcNow ) ); }
					catch { Console.WriteLine("Exception caught Deserializing GlobalMobileUse."); }
					m_MobileUse.Add( "0", new UseEntry( "None", 0, DateTime.UtcNow ) );
					goto case 0;
				}
				case 0:
				{
					m_Flags = (OptFlags)reader.ReadInt();
					m_GlobalFlags = (OptFlags)reader.ReadInt();
					break;
				}
			}
		}

	//			private static string[] m_Entries = new string[]
	//	{
	//		"Trammel", "Trammel Dungeons", "Felucca", "Felucca Dungeons",
	//		"Public Moongates", "Ilshenar", "Ilshenar Shrines", "Malas",
	//		"Tokuno", "TerMur", "Thanimur", "Sosaria", "Custom"
	//	};
		
		private static string[] m_Entries = new string[]
		{
			"Trammel", "Trammel Dungeons", "Felucca", "Felucca Dungeons",
			"Public Moongates", "Ilshenar", "Ilshenar Shrines", "Malas",
			"Tokuno", "TerMur", "Custom"
		};
	}
	
	public class WorldOmniporterGump : Gump
	{
		private WorldOmniporter m_WO;
		private Mobile m;
		private OmniEntry m_Entry;
		private int m_Page;
		private bool m_Reds, m_HasLBR, m_HasAOS, m_HasSE, m_HasSA, m_Staff;
		private List<Mobile> move = new List<Mobile>();
		private int go;
		private bool pets;
		
		public void AddBlackAlpha( int x, int y, int width, int height )
		{
		AddImageTiled( 58, 38, 384, 429, 2624 );
		AddImageTiled( 58, 8, 384, 24, 2624 );
		}
		
		public WorldOmniporterGump( Mobile from, WorldOmniporter WO, int page ) : base( 100, 100 )
		
		{
			int flags = from.NetState == null ? 0 : (int)from.NetState.Flags;

			m = from;
			m_WO = WO;
			m_Page = page;
			m_HasLBR = (flags & 0x04) != 0;
			m_HasAOS = (flags & 0x08) != 0;
			m_HasSE = (flags & 0x10) != 0;
			m_HasSA = (flags & 0x20) != 0;
			m_Reds = ( m.Kills < 5 || WO.AllowReds );
			m_Staff = m.AccessLevel > AccessLevel.Player;

			//Did they press an invalid button or supply an invalid argument?
			if ( page < 0 || page > 12 ) // If you add a page, you need to raise the last number here....
				page = 0;

			AddPage( 0 );
			AddBackground( 50, 30, 400, 445, 5054 ); 	
			AddBackground( 50, 0, 400, 475, 5054 );
			AddBlackAlpha( 50, 30, 384, 429);
			AddBlackAlpha( 50, 0, 384, 24);			
			AddAlphaRegion( 58,38, 384, 429 );
			AddAlphaRegion( 58,8, 384, 24 );
			AddImage(0, 0, 10440);  //Left side dragon
			AddImage(418, 0, 10441);  //Right side dragon
			
			
			if ( m_Staff ) AddHtml(363, 40, 50, 15, "<basefont color=#FFFFFF>To</basefont>", false, false );					// staff see Omniporter usage
			if ( m_Staff ) AddHtml(396, 40, 50, 15, "<basefont color=#FFFFFF>From</basefont>", false, false );					// staff see Omniporter usage 
			AddHtml(75, 40, 150, 15, "<basefont color=#FFFFFF>Choose your location:</basefont>", false, false );
			AddHtml(165, 12, 200, 15, "<basefont color=#FFFFFF>DAAT99's World Omniporter</basefont>", false, false ); 

			int p = 1;

			if ( WO.Trammel && m_Reds )
			{
				GenerateMapListing( 1 );
				AddPageButton( "Trammel Towns", Map.Trammel, p++, 1 );
			}

			if ( WO.TramDungeons && m_Reds && WO.CanUse( m, true, false ) )
			{
				GenerateMapListing( 2 );
				AddPageButton( "Trammel Dungeons", Map.Trammel, p++, 2 );
			}

			if ( WO.Felucca && WO.CanUse( m, false, true ) )
			{
				GenerateMapListing( 3 );
				AddPageButton( "Felucca Towns", Map.Felucca, p++, 3 );
			}

			if ( WO.FelDungeons && WO.CanUse( m, true, true ) )
			{
				GenerateMapListing( 4 );
				AddPageButton( "Felucca Dungeons", Map.Felucca, p++, 4 );
			}

			if ( WO.PublicMoongates && (WO.Felucca || (WO.Trammel && m_Reds)) )
			{
				GenerateMapListing( 5 );
				AddPageButton( "<basefont color=#FFFFFF>Public Moongates</basefont>", null, p++, 5 );
			}

			if ( WO.Ilshenar && m_Reds && m_HasLBR )
			{
				GenerateMapListing( 6 );
				AddPageButton( "Ilshenar", Map.Ilshenar, p++, 6 );
			}

			if ( WO.IlshenarShrines && m_Reds && m_HasLBR )
			{
				GenerateMapListing( 7 );
				AddPageButton( "Ilshenar Shrines", Map.Ilshenar, p++, 7 );
			}

			if ( WO.Malas && m_Reds && Core.AOS && m_HasAOS )
			{
				GenerateMapListing( 8 );
				AddPageButton( "Malas", Map.Malas, p++, 8 );
			}

			if ( WO.Tokuno && m_Reds && Core.SE && m_HasSE )
			{
				GenerateMapListing( 9 );
				AddPageButton( "Tokuno", Map.Tokuno, p++, 9 );
			}
			
			if ( WO.TerMur && m_Reds && Core.SA && m_HasSA )
			{
				GenerateMapListing( 10 );
				AddPageButton( "TerMur", Map.TerMur, p++, 10 );
			}
			
			//if ( WO.Thanimur && m_Reds )
			//{
			//	GenerateMapListing( 11 );
			//	AddPageButton( "Thanimur", Map.Thanimur, p++, 11 );
			//}
			
			//if ( WO.Sosaria && m_Reds )
			//{
			//	GenerateMapListing( 12 );
			//	AddPageButton( "Sosaria", Map.Thanimur, p++, 12 );
			//}
			
				if ( WO.Custom )
			{
				GenerateMapListing( 11 );
				AddPageButton( "<basefont color=#FFFFFF>Custom Locations</basefont>", null, p++, 11 );
			}
			
		}

		private void AddPageButton( string text, Map map, int offset, int page )
		{
			string label;
			if ( map != null )
				label = String.Format( "<basefont color=#{0}>{1}</basefont>", MapHue( map ), text );
				
			else
				label = text;
			AddButton( 67, 70 + ((offset - 1) * 25), 2117, 2118, page, GumpButtonType.Reply, 0  );  //Map/Facet Button
			AddHtml( 87, 70 + ((offset - 1) * 25), 150, 20, label, false, false );  // Map/Facet Name
			
		}

		private static OmniEntry GetEntry( string name, int id )
		{
			OmniEntry[] oe = (OmniEntry[])WorldOmniporter.GlobalEntries[name];

			if ( oe != null )
			{
				if ( id < 0 || id >= oe.Length )
					id = 0;
				return oe[id];
			}

			return null;
		}

		private void GenerateMapListing( int page )
		{
			if ( m_Page == 0 )
				m_Page = page;
			else if ( page != m_Page )
				return;

			string name = m_Entries[page-1];

			OmniEntry[] oe = (OmniEntry[])WorldOmniporter.GlobalEntries[name];
			if ( oe == null )
				return;

			int offset = m_Page * 100;
			bool gates = name == "Public Moongates";
			for (int i = 0, l = 0; i < oe.Length; i++ )
			{	
				OmniEntry entry = oe[i];

				if ( ( (gates || name == "Felucca") && entry.Map == Map.Felucca && ( !m_WO.Felucca || !m_WO.CanUse( m, false, true ) ) ) )
					continue;
				else if ( (gates || name == "Trammel") && entry.Map == Map.Trammel && (!m_WO.Trammel || !m_Reds))
					continue;
				else if ( entry.Map == Map.Ilshenar && (!m_WO.Ilshenar || !m_HasLBR || !m_Reds))
					continue;
				else if (entry.Map == Map.Malas && (!Core.AOS || !m_HasAOS || !m_WO.Malas || !m_Reds))
					continue;
				else if (entry.Map == Map.Tokuno && (!Core.SE || !m_HasSE || !m_WO.Tokuno || !m_Reds))
					continue;
				else if (entry.Map == Map.TerMur && (!Core.SA || !m_HasSA || !m_WO.TerMur || !m_Reds))
					continue;
				//else if (entry.Map == Map.Thanimur && (!m_WO.Thanimur || !m_Reds))
				//	continue;
				//else if (entry.Map == Map.Sosaria && (!m_WO.Sosaria || !m_Reds))
				//	continue;
				else
				{
					string label = String.Format( "<basefont color=#{0}>{1}</basefont>", MapHue( entry.Map ), entry.Name );
					if ( m_Staff ) AddHtml( 243, 70+(l*20), 150, 20, label, false, false ); //Staff Location Display Name
					else AddHtml( 273, 70+(l*20), 150, 20, label, false, false );  //Player Location Display Name
					if ( m_Staff ) AddLabel( 365, 70+(l*20), 1152, entry.ToCount.ToString() );  // Staff - Player Location Usage Count To
					if ( m_Staff ) AddLabel( 405, 70+(l*20), 1152, entry.FromCount.ToString() );  // Staff - Player Location Usage Count From
					if ( m_Staff ) AddButton( 208, 70+(l*20), 4015, 4016, (i+offset), GumpButtonType.Reply, 0 );  //Staff Location Buttons
					else AddButton( 238, 70+(l*20), 4015, 4016, (i+offset), GumpButtonType.Reply, 0 );  //Player Location Buttons
					l++;
				}
			}
		}

		private string MapHue( Map map )
		{
			if ( map == Map.Felucca )
				return "FF0000";
			else
				return "FFFFFF";
		}

		//private static string[] m_Entries = new string[]
		//{
		//	"Trammel", "Trammel Dungeons", "Felucca", "Felucca Dungeons",
		//	"Public Moongates", "Ilshenar", "Ilshenar Shrines", "Malas",
		//	"Tokuno", "TerMur", "Thanimur", "Sosaria", "Custom"
		//};
		
		private static string[] m_Entries = new string[]
		{
			"Trammel", "Trammel Dungeons", "Felucca", "Felucca Dungeons",
			"Public Moongates", "Ilshenar", "Ilshenar Shrines", "Malas",
			"Tokuno", "TerMur", "Custom"
		};

		public override void OnResponse( NetState state, RelayInfo info )
		{
			pets = false;
			go = 1;
			Mobile from = state.Mobile;
			string msg = "You may use the Omniporter now.";

			foreach ( Mobile mob in from.GetMobilesInRange( 2 ) )
			{
				if ( mob is BaseCreature )
				{
					BaseCreature pet = (BaseCreature)mob;

					if ( pet.Controlled && pet.ControlMaster == from )
					{
						if ( pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow || pet.ControlOrder == OrderType.Come )
						{
							move.Add( pet );
							go++;
						}
					}
				}
			}
			if ( go > 1 ) pets = true;
			
			string message = m_WO.NextUseMessage( from, go, pets );

			if ( info.ButtonID <= 0 || from == null || from.Deleted || m_WO == null || m_WO.Deleted )
				return;

			int id = info.ButtonID / 100;
			int count = info.ButtonID % 100;

			if ( id == 0 && count < 12 ) //raise this number to add a new page
			{
				from.SendGump( new WorldOmniporterGump( from, m_WO, count ) );
				return;
			}

			//Invalid checks
			//if ( id < 1 || id > 14 )  //change this to add a new page...
				// id = 1;

			string name = m_Entries[id-1];

			m_Entry = GetEntry( name, count );

			bool gates = name == "Public Moongates";

			if ( m_Entry == null )
			
				from.SendMessage( "Error: Invalid Button Response - No Map Entries" );
			else if ( ( (gates || name == "Felucca") && m_Entry.Map == Map.Felucca && !m_WO.Felucca) )
				from.SendMessage( "Error: Invalid Button Response - Felucca Disabled" );
			else if ( (gates || name == "Trammel") && m_Entry.Map == Map.Trammel && (!m_WO.Trammel || !m_Reds))
				from.SendMessage( "Error: Invalid Button Response - Trammel Disabled" );
			else if ( (name == "Ilshenar" ) && m_Entry.Map == Map.Ilshenar && (!m_WO.Ilshenar || !m_HasLBR || !m_Reds))
				from.SendMessage( "Error: Invalid Button Response - Ilshenar Disabled" );
			else if (m_Entry.Map == Map.Malas && (!Core.AOS || !m_HasAOS || !m_WO.Malas || !m_Reds))
				from.SendMessage( "Error: Invalid Button Response - Malas Disabled" );
			else if (m_Entry.Map == Map.Tokuno && (!Core.SE || !m_HasSE || !m_WO.Tokuno || !m_Reds))
				from.SendMessage( "Error: Invalid Button Response - Tokuno Disabled" );
			else if (m_Entry.Map == Map.TerMur && (!Core.SA || !m_HasSA || !m_WO.TerMur || !m_Reds))
				from.SendMessage( "Error: Invalid Button Response - TerMur Disabled" );
			else if ( !from.InRange( m_WO.GetWorldLocation(), 1 ) || from.Map != m_WO.Map )
				from.SendLocalizedMessage( 1019002 ); 				// You are too far away to use the gate.
			else if ( from.Criminal )
				from.SendLocalizedMessage( 1005561, "", 0x22 ); 	// Thou'rt a criminal and cannot escape so easily.
			else if ( Server.Spells.SpellHelper.CheckCombat( from ) )
				from.SendLocalizedMessage( 1005564, "", 0x22 ); 	// Wouldst thou flee during the heat of battle??
			else if ( from.Spell != null )
				from.SendLocalizedMessage( 1049616 ); 				// You are too busy to do that at the moment.
			else if ( from.Map == m_Entry.Map && from.InRange( m_Entry.Destination, 1 ) )
				from.SendLocalizedMessage( 1019003 ); 				// You are already there.
			else if ( message == msg )
				DoTravel( from );									// --- Travel ---
			else if ( m_WO.MustWait( from ) )
				from.SendMessage( message );						// Have to wait for some reason...
			else if ( !m_WO.CanUse( from, pets ) )
				from.SendMessage( message );						// You may not use the Omniporter...
			else if ( m_WO.MustPay( from, pets ) )					// Have to pay to use the Omniporter...
				from.SendGump( new ConfirmGump( "Confirm!", 32767, message,
					32767, 300, 300, new ConfirmGumpCallback( PaymentConfirm_Callback ) ) );
			else
				from.SendMessage( message );						// You may not use the Omniporter...
		}

		public void PaymentConfirm_Callback( Mobile from, bool okay )
		{
			if ( okay )
			{
				Container pack = from.Backpack;
				if ( pack != null && pack.ConsumeTotal( typeof( Gold ), m_WO.GetPrice( from, go, pets ) ) )
					DoTravel( from );								// --- Travel ---
				else
					from.SendLocalizedMessage( 500192 ); 			//Begging thy pardon...
			}
			else
				from.SendMessage("You decide to stay.");
		}
		
		public OmniEntry SourceEntry( Mobile from )
		{
			OmniEntry[] oe;
			//Change the below line every time you add a page....
			for ( int x=0; x<=13; x++ )
			{
				oe = (OmniEntry[])WorldOmniporter.GlobalEntries[m_Entries[x]];
				for ( int y=0; y<oe.Length; y++ )
				{
					OmniEntry entry = oe[y];
					if ( from.Map == entry.Map && from.InRange( entry.Destination, 2 ) )
					{
						return entry;
					}
				}
			}
			return null;
		}
		
		public void DoTravel( Mobile from )
		{
			OmniEntry source = SourceEntry( from );
			from.MoveToWorld( m_Entry.Destination, m_Entry.Map );

			if ( pets ) foreach ( Mobile mob in move )
				mob.MoveToWorld( m_Entry.Destination, m_Entry.Map );
				
			if ( !from.Hidden )
				Effects.PlaySound( m_Entry.Destination, m_Entry.Map, 0x1FE );
				Effects.SendLocationEffect(m_Entry.Destination, m_Entry.Map, 0x3763, 25, 3, 0, 0);
				
			from.Combatant = null;
			m_WO.LocalUses += go;
			m_Entry.ToCount += go;
			source.FromCount += go;
			m_WO.RaiseUses( from, go );
		}
	}

	public class OmniEntry
	{
		private string m_Name;
		private Point3D m_Destination;
		private Map m_Map;
		private int m_ToCount;
		private int m_FromCount;

		public string Name{ get{ return m_Name; } }
		public Point3D Destination{ get{ return m_Destination; } }
		public Map Map{ get{ return m_Map; } }
		public int ToCount{ get{ return m_ToCount; } set { m_ToCount = value; } }
		public int FromCount{ get{ return m_FromCount; } set { m_FromCount = value; } }
		
		private bool m_Moongate;
		public bool Moongate{ get{ return m_Moongate; } set { m_Moongate = value; } }

		public OmniEntry( string name, Point3D p, Map map) : this( name, p, map, false )
		{
		}

		public OmniEntry( string name, Point3D p, Map map, bool moongate)
		{
			m_Name = name;
			m_Destination = p;
			m_Map = map;
			m_ToCount = 0;
			m_FromCount = 0;
			m_Moongate = moongate;
		}
	}

	public class WorldOmniGenCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "WorldOmniDel", AccessLevel.Administrator, new CommandEventHandler( WorldOmniDel_OnCommand ) );
			CommandSystem.Register( "WorldOmniGen", AccessLevel.Administrator, new CommandEventHandler( WorldOmniGen_OnCommand ) );
		}

		[Usage( "WorldOmniDel" )]
		[Description( "Deletes Nonmovable world Omniporters and Teleporters." )]
		public static void WorldOmniDel_OnCommand( CommandEventArgs e )
		{	
			DeleteWorldOmni();
		}

		[Usage( "WorldOmniGen" )]
		[Description( "Generates world Omniporters. Removes all old non movable world Omniporters." )]
		public static void WorldOmniGen_OnCommand( CommandEventArgs e )
		{
			World.Broadcast( 0x35, true, "Generating world Omniporters.");		
			DeleteWorldOmni();
			World.Broadcast( 0x35, true, "Finished generating {0} world Omniporters.", WorldOmniporter.GenerateWorldOmniporters() );
		}

		private static void DeleteWorldOmni()
		{
			ArrayList olist = new ArrayList();
			ArrayList tlist = new ArrayList();

			foreach ( Item item in World.Items.Values )
			{
				if ( ( item is WorldOmniporter ) && !item.Movable)
					olist.Add( item );
				
			}

			foreach ( Item item in olist )
				item.Delete();
			foreach ( Item item in tlist )
				item.Delete();

			if ( olist.Count > 0 )
				World.Broadcast( 0x35, true, "{0} world Omniporters removed.", olist.Count );
			if ( tlist.Count > 0 )
				World.Broadcast( 0x35, true, "{0} world Teleporters removed.", tlist.Count );
		}
	}
}