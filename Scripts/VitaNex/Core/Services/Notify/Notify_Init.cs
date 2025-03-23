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

using Server;

using VitaNex.IO;
#endregion

namespace VitaNex.Notify
{
	[CoreService("Notify", "3.0.0.0", TaskPriority.Highest)]
	public static partial class Notify
	{
		static Notify()
		{
			CSOptions = new CoreServiceOptions(typeof(Notify));

			GumpTypes = typeof(NotifyGump).GetChildren(t => !t.IsNested);

			NestedTypes = new Dictionary<Type, Type[]>();

			SettingsMap = new Dictionary<Type, Type>();

			Settings = new BinaryDataStore<Type, NotifySettings>(VitaNexCore.SavesDirectory + "/Notify", "Settings")
			{
				Async = true,
				OnSerialize = Serialize,
				OnDeserialize = Deserialize
			};
		}

		private static void CSConfig()
		{
			foreach (var t in GumpTypes)
			{
				EnsureSettings(t);
			}
		}

		private static void CSInvoke()
		{
			CommandUtility.Register("Notify", AccessLevel.Player, HandleNotify);
			CommandUtility.Register("NotifyAC", AccessLevel.Seer, HandleNotifyAC);
			CommandUtility.Register("NotifyNA", AccessLevel.Seer, HandleNotifyNA);
			CommandUtility.Register("NotifyACNA", AccessLevel.Seer, HandleNotifyACNA);
		}

		private static void CSLoad()
		{
			Settings.Import();

			Settings.RemoveRange(o => !GumpTypes.Contains(o.Key) || o.Value == null);
		}

		private static void CSSave()
		{
			Settings.Export();
		}

		private static bool Serialize(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.WriteBlockDictionary(
				Settings,
				(w, k, v) =>
				{
					w.WriteType(k);
					v.Serialize(w);
				});

			return true;
		}

		private static bool Deserialize(GenericReader reader)
		{
			reader.GetVersion();

			reader.ReadBlockDictionary(
				r =>
				{
					var k = r.ReadType();
					var v = EnsureSettings(k);

					if (v != null)
					{
						v.Deserialize(r);

						if (v.Type == null)
						{
							v.Type = k;
						}
					}

					return new KeyValuePair<Type, NotifySettings>(k, v);
				},
				Settings);

			return true;
		}
	}
}