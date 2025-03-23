#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		public bool Validate(Mobile viewer = null)
		{
			return Validate(viewer, new List<string>());
		}

		public virtual bool Validate(Mobile viewer, List<string> errors, bool pop = true)
		{
			if (Deleted)
			{
				errors.Add("This battle has been deleted.");
				return false;
			}

			if (String.IsNullOrWhiteSpace(Name))
			{
				errors.Add("Select a valid Name.");
				errors.Add("[Options] -> [Edit Options]");

				if (pop)
				{
					return false;
				}
			}

			if (String.IsNullOrWhiteSpace(Description))
			{
				errors.Add("Select a valid Description.");
				errors.Add("[Options] -> [Edit Options]");

				if (pop)
				{
					return false;
				}
			}

			if (SpectateAllowed)
			{
				if (SpectateRegion == null)
				{
					errors.Add("Select a valid Spectate Region.");
					errors.Add("[Options] -> [Edit Spectage Region]");

					if (pop)
					{
						return false;
					}
				}
				else if (Options.Locations.SpectateBounds.Count == 0)
				{
					errors.Add("The Spectate Region has no zones.");
					errors.Add("[Options] -> [Edit Spectage Region]");

					if (pop)
					{
						return false;
					}
				}
			}

			if (BattleRegion == null)
			{
				errors.Add("Select a valid Battle Region.");
				errors.Add("[Options] -> [Edit Battle Region]");

				if (pop)
				{
					return false;
				}
			}
			else if (Options.Locations.BattleBounds.Count == 0)
			{
				errors.Add("The Battle Region has no zones.");
				errors.Add("[Options] -> [Edit Battle Region]");

				if (pop)
				{
					return false;
				}
			}

			if (Options.Locations.Map == Map.Internal)
			{
				errors.Add("The Battle Map must not be Internal.");
				errors.Add("[Options] -> [Edit Advanced Options] -> [Locations]");

				if (pop)
				{
					return false;
				}
			}

			if (Options.Locations.Eject.Internal)
			{
				errors.Add("The Eject Map must not be Internal.");
				errors.Add("[Options] -> [Edit Advanced Options] -> [Locations]");

				if (pop)
				{
					return false;
				}
			}

			if (Options.Locations.Eject == Point3D.Zero)
			{
				errors.Add("Select a valid Eject Location.");
				errors.Add("[Options] -> [Edit Advanced Options] -> [Locations]");

				if (pop)
				{
					return false;
				}
			}
			else if (BattleRegion != null &&
					 BattleRegion.Contains(Options.Locations.Eject.Location, Options.Locations.Eject.Map))
			{
				errors.Add("Eject Location must be outside the Battle Region.");
				errors.Add("[Options] -> [Edit Advanced Options] -> [Locations]");

				if (pop)
				{
					return false;
				}
			}

			if (SpectateAllowed)
			{
				if (Options.Locations.SpectateJoin == Point3D.Zero)
				{
					errors.Add("Select a valid Spectator Join location.");
					errors.Add("[Options] -> [Edit Advanced Options] -> [Locations]");

					if (pop)
					{
						return false;
					}
				}
				else if (SpectateRegion != null && !SpectateRegion.Contains(Options.Locations.SpectateJoin))
				{
					errors.Add("Spectate Join Location must be within the Spectate Region.");
					errors.Add("[Options] -> [Edit Advanced Options] -> [Locations]");

					if (pop)
					{
						return false;
					}
				}
			}

			if (Schedule == null)
			{
				errors.Add("No Schedule has been set for this battle.");
				errors.Add("[Options] -> [View Schedule]");

				if (pop)
				{
					return false;
				}
			}
			else if (Schedule.Enabled && Schedule.NextGlobalTick == null)
			{
				errors.Add("The Schedule has no more future dates.");
				errors.Add("[Options] -> [View Schedule]");

				if (pop)
				{
					return false;
				}
			}

			if (IdleKick && IdleThreshold.TotalSeconds < 10.0)
			{
				errors.Add("The Idle Threshold must be greater than, or equal to 10 seconds.");
				errors.Add("[Options] -> [Edit Options]");

				if (pop)
				{
					return false;
				}
			}

			if (Teams.Count == 0)
			{
				errors.Add("There are no teams available for this Battle.");
				errors.Add("[Options] -> [View Teams]");

				if (pop)
				{
					return false;
				}
			}
			else if (Teams.Any(t => !t.Validate(viewer)))
			{
				errors.Add("One or more teams did not pass validation.");
				errors.Add("[Options] -> [View Teams]");

				if (pop)
				{
					return false;
				}
			}

			return errors.Count == 0;
		}
	}
}