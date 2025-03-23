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
using Server.Accounting;

using VitaNex.IO;
using VitaNex.Web;
#endregion

namespace VitaNex.Modules.AutoDonate
{
	[CoreModule("Auto Donate", "3.1.0.0", true, TaskPriority.Highest)]
	public static partial class AutoDonate
	{
		static AutoDonate()
		{
			CMOptions = new DonationOptions();

			Transactions = new BinaryDataStore<string, DonationTransaction>(
				VitaNexCore.SavesDirectory + "/AutoDonate",
				"Transactions")
			{
				OnSerialize = SerializeTransactions,
				OnDeserialize = DeserializeTransactions
			};

			Profiles = new BinaryDirectoryDataStore<IAccount, DonationProfile>(
				VitaNexCore.SavesDirectory + "/AutoDonate",
				"Profiles",
				"pro")
			{
				OnSerialize = SerializeProfile,
				OnDeserialize = DeserializeProfile
			};
		}

		private static void CMConfig()
		{
			CommandUtility.Register("CheckDonate", AccessLevel.Player, e => CheckDonate(e.Mobile));
			CommandUtility.Register("DonateConfig", Access, e => CheckConfig(e.Mobile));

			CommandUtility.RegisterAlias("DonateConfig", "DonateAdmin");

			EventSink.Login += OnLogin;

			WebAPI.Register("/donate/ipn", HandleIPN);
			WebAPI.Register("/donate/acc", HandleAccountCheck);
			WebAPI.Register("/donate/form", HandleWebForm);
		}

		private static void CMEnabled()
		{
			WebAPI.Register("/donate/ipn", HandleIPN);
			WebAPI.Register("/donate/acc", HandleAccountCheck);
			WebAPI.Register("/donate/form", HandleWebForm);
		}

		private static void CMDisabled()
		{
			WebAPI.Unregister("/donate/ipn");
			WebAPI.Unregister("/donate/acc");
			WebAPI.Unregister("/donate/form");
		}

		private static void CMSave()
		{
			Transactions.Export();
			Profiles.Export();
		}

		private static void CMLoad()
		{
			Transactions.Import();
			Profiles.Import();
		}

		private static void CMInvoke()
		{
			var owner = Accounts.GetAccounts().FirstOrDefault(ac => ac.AccessLevel == AccessLevel.Owner);

			foreach (var trans in Transactions.Values)
			{
				if (trans.Account == null && owner != null)
				{
					trans.SetAccount(owner);
				}

				if (trans.Account == null)
				{
					continue;
				}

				var p = EnsureProfile(trans.Account);

				if (p != null)
				{
					p.Transactions[trans.ID] = trans;
				}
			}
		}

		private static bool SerializeTransactions(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.WriteBlockDictionary(Transactions, (w, k, v) => v.Serialize(w));

			return true;
		}

		private static bool DeserializeTransactions(GenericReader reader)
		{
			reader.GetVersion();

			reader.ReadBlockDictionary(
				r =>
				{
					var t = new DonationTransaction(r);

					return new KeyValuePair<string, DonationTransaction>(t.ID, t);
				},
				Transactions);

			return true;
		}

		private static bool SerializeProfile(GenericWriter writer, IAccount key, DonationProfile val)
		{
			var version = writer.SetVersion(1);

			writer.WriteBlock(
				w =>
				{
					w.Write(key);

					switch (version)
					{
						case 1:
							val.Serialize(w);
							break;
						case 0:
							w.WriteType(val, t => val.Serialize(w));
							break;
					}
				});

			return true;
		}

		private static Tuple<IAccount, DonationProfile> DeserializeProfile(GenericReader reader)
		{
			var version = reader.GetVersion();

			return reader.ReadBlock(r =>
			{
				var key = r.ReadAccount();

				DonationProfile val = null;

				switch (version)
				{
					case 1:
						val = new DonationProfile(r);
						break;
					case 0:
						val = r.ReadTypeCreate<DonationProfile>(r);
						break;
				}

				if (key == null && val != null && val.Account != null)
				{
					key = val.Account;
				}

				return Tuple.Create(key, val);
			});
		}
	}
}