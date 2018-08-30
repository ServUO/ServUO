using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Commands;

namespace Server.Misc
{
  public class BondInfoCommand
	{
		public static void Initialize()
			{
	      CommandSystem.Register( "BondInfo", AccessLevel.Player, new CommandEventHandler( BondInfo_OnCommand ) );
			}
			
		[Usage( "BondInfo" )]
		[Description( "Tells you how much time remaining until your next BOD is available." )]
		public static void BondInfo_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			e.Mobile.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( BondInfo_OnTarget ) );
			e.Mobile.SendMessage("Target the pet you wish to know the bonding timer of");
		}	
		
		
		public static void BondInfo_OnTarget( Mobile from, object targeted )	
		{	
			if ( targeted is BaseCreature )
			 {
				 BaseCreature targ = (BaseCreature)targeted;		
						if ( targ.ControlMaster == from ) 
							{
								if( targ.BondingBegin == DateTime.MinValue )
								{
									from.SendMessage("Your pet hasn't started to bond yet, please feed it and try again." );
								}
						else	
						{
								DateTime today = DateTime.Now;
								DateTime willbebonded = targ.BondingBegin.AddDays(7);
								TimeSpan bondedfor = targ.BondingBegin - today;
								TimeSpan daystobond = willbebonded - today;
								string BondInfo = string.Format("The pet started bonding with you at {0}. Its {1} days, {2} hours and {3} minutes until it bonds", targ.BondingBegin, daystobond.Days, 
								daystobond.Hours, daystobond.Minutes  );
								from.SendMessage ( BondInfo );
						}		
							
			}
			else
				{ 
          from.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( BondInfo_OnTarget ) );
          from.SendMessage( "That is not your pet!" ); 
        } 	
			
		}			
 		else
			{
				from.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( BondInfo_OnTarget ) );
			 	from.SendMessage("That is not a pet!" ); 
		 }
	}
 }
}