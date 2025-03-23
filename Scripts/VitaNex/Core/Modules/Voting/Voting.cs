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
using System.Linq;

using Server;
using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.Voting
{
	public static partial class Voting
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static Type[] SiteTypes { get; private set; }

		public static VotingOptions CMOptions { get; private set; }

		public static BinaryDataStore<int, IVoteSite> VoteSites { get; private set; }
		public static BinaryDataStore<PlayerMobile, VoteProfile> Profiles { get; private set; }

		public static event Action<VoteRequestEventArgs> OnVoteRequest;

		public static void Invoke(this VoteRequestEventArgs e)
		{
			if (OnVoteRequest != null && e != null)
			{
				OnVoteRequest(e);
			}
		}

		public static IVoteSite FindSite(int uid)
		{
			VoteSites.TryGetValue(uid, out var site);
			
			return site;
		}

		public static void Remove(this IVoteSite site)
		{
			VoteSites.Remove(site.UID);

			foreach (var h in Profiles.Values.SelectMany(p => p.History.Values))
			{
				h.RemoveAll(
					e =>
					{
						if (e.VoteSite != site)
						{
							return false;
						}

						e.VoteSite = null;
						return true;
					});
			}

			site.Delete();
		}

		public static VoteProfile EnsureProfile(PlayerMobile m, bool replace = false)
		{
			if (!Profiles.ContainsKey(m))
			{
				Profiles.Add(m, new VoteProfile(m));
			}
			else if (replace || Profiles[m] == null || Profiles[m].Deleted)
			{
				Profiles[m] = new VoteProfile(m);
			}

			return Profiles[m];
		}

		public static void Remove(this VoteProfile profile)
		{
			Profiles.Remove(profile.Owner);
		}

		public static void Prune()
		{
			Profiles.Values.Where(p => p.Owner == null || p.Owner.Deleted || p.History == null || p.History.Count == 0)
					.ForEach(p => p.Delete());
		}
	}
}