/*
 *made by Ttxman
 * 
 */
using System;
using System.Reflection;
using Server.Items;
using Server.Targeting;
using System.Collections;
using Server.Engines.BulkOrders;

namespace Server.Commands
{
	public class FillBulk
	{
		public static void Initialize()
		{
			CommandSystem.Register( "FillBOD", AccessLevel.GameMaster, new CommandEventHandler( FillBulk_OnCommand ) );
		}

		[Usage( "FillBOD" )]
		[Description( "Fills BOD" )]
		private static void FillBulk_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new FillBulkTarget();
			e.Mobile.SendMessage("Target a BOD to fill it");
		}
		private class FillBulkTarget : Target
		{
			public FillBulkTarget() : base( 20, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if (!((targ is SmallBOD) || (targ is LargeBOD)))
					return;
				
				if (targ is SmallBOD)
				{
					SmallBOD x = targ as SmallBOD;
					x.AmountCur =x.AmountMax;
					x.InvalidateProperties();
				}else if (targ is LargeBOD)
				{
					LargeBOD y = targ as LargeBOD;
					foreach (LargeBulkEntry e in y.Entries)
					{
						e.Amount = y.AmountMax;
					}
					y.InvalidateProperties();
				}
			}
			
		}
	}
}
