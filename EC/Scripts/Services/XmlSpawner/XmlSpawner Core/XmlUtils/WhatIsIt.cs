using System;
using System.Text;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Commands
{
	public class WhatIsIt
	{

		public static void Initialize()
		{
			CommandSystem.Register("WhatIsIt", AccessLevel.Player, new CommandEventHandler(GenericCommand_OnCommand));
		}

		public class WhatIsItTarget : Target
		{

			public WhatIsItTarget()
				: base(30, true, TargetFlags.None)
			{
				CheckLOS = false;
			}
			protected override void OnTarget( Mobile from, object targeted )
			{
				if(from == null || targeted == null) return;

				string name = String.Empty;
				string typename = targeted.GetType().Name;
				string article = "a";

				if (typename != null && typename.Length > 0)
				{
					if ("aeiouy".IndexOf(typename.ToLower()[0]) >= 0)
					{
						article = "an";
					}
				}

				if(targeted is Item)
				{
					name = ((Item)targeted).Name;
				} else
					if(targeted is Mobile)
					{
						name = ((Mobile)targeted).Name;
					}
				if (name != String.Empty && name != null)
				{
					from.SendMessage("That is {0} {1} named '{2}'", article, typename, name);
				}
				else
				{
					from.SendMessage("That is {0} {1} with no name", article, typename);
				}
			}
		}

		[Usage( "WhatIsIt" )]
		public static void GenericCommand_OnCommand( CommandEventArgs e )
		{
			if(e == null || e.Mobile == null) return;

			e.Mobile.Target = new WhatIsItTarget();
		}
	}
}
