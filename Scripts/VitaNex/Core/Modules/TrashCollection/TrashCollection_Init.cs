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
using VitaNex.Network;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	[CoreModule("Trash Collection", "1.0.0.1")]
	public static partial class TrashCollection
	{
		static TrashCollection()
		{
			HandlerTypes = typeof(BaseTrashHandler).GetConstructableChildren();

			CMOptions = new TrashCollectionOptions();

			Handlers = new BinaryDataStore<string, BaseTrashHandler>(VitaNexCore.SavesDirectory + "/TrashCollection", "Handlers")
			{
				OnSerialize = SerializeHandlers,
				OnDeserialize = DeserializeHandlers
			};

			Profiles = new BinaryDataStore<Mobile, TrashProfile>(VitaNexCore.SavesDirectory + "/TrashCollection", "Profiles")
			{
				Async = true,
				OnSerialize = SerializeProfiles,
				OnDeserialize = DeserializeProfiles
			};
		}

		private static void CMConfig()
		{
			var handlers = new List<BaseTrashHandler>();

			var count = 0;

			HandlerTypes.ForEach(
				type =>
				{
					var handler = type.CreateInstance<BaseTrashHandler>();

					if (handler == null)
					{
						return;
					}

					handlers.Update(handler);

					if (CMOptions.ModuleDebug)
					{
						CMOptions.ToConsole(
							"Created trash handler '{0}' ({1})",
							handler.GetType().Name,
							handler.Enabled ? "Enabled" : "Disabled");
					}

					++count;
				});

			CMOptions.ToConsole("Created {0:#,0} trash handler{1}", count, count != 1 ? "s" : String.Empty);

			handlers.ForEach(h => Handlers[h.UID] = h);
			handlers.Free(true);

			ExtendedOPL.OnItemOPLRequest += AddTrashProperties;
			ExtendedOPL.OnMobileOPLRequest += AddTrashProperties;
		}

		private static void CMEnabled()
		{
			ExtendedOPL.OnItemOPLRequest += AddTrashProperties;
			ExtendedOPL.OnMobileOPLRequest += AddTrashProperties;
		}

		private static void CMDisabled()
		{
			ExtendedOPL.OnItemOPLRequest -= AddTrashProperties;
			ExtendedOPL.OnMobileOPLRequest -= AddTrashProperties;
		}

		private static void CMInvoke()
		{
			InternalHandlerSort();
		}

		private static void CMSave()
		{
			var result = Handlers.Export();
			CMOptions.ToConsole("{0:#,0} handlers saved, {1}", Handlers.Count, result);

			result = Profiles.Export();
			CMOptions.ToConsole("{0:#,0} profiles saved, {1}", Profiles.Count, result);
		}

		private static void CMLoad()
		{
			var result = Handlers.Import();
			CMOptions.ToConsole("{0:#,0} handlers loaded, {1}.", Handlers.Count, result);

			result = Profiles.Import();
			CMOptions.ToConsole("{0:#,0} profiles loaded, {1}.", Profiles.Count, result);
		}

		private static bool SerializeHandlers(GenericWriter writer)
		{
			writer.WriteBlockCollection(Handlers.Values, (w, obj) => w.WriteType(obj, t => obj.Serialize(w)));

			return true;
		}

		private static bool DeserializeHandlers(GenericReader reader)
		{
			foreach (var h in reader.ReadBlockCollection(r => r.ReadTypeCreate<BaseTrashHandler>(r)))
			{
				if (h != null && h.UID != null)
				{
					Handlers[h.UID] = h;
				}
			}

			InternalHandlerSort();
			return true;
		}

		private static bool SerializeProfiles(GenericWriter writer)
		{
			writer.WriteBlockCollection(Profiles.Values, (w, obj) => obj.Serialize(w));

			return true;
		}

		private static bool DeserializeProfiles(GenericReader reader)
		{
			foreach (var p in reader.ReadBlockCollection(r => new TrashProfile(r)))
			{
				if (p != null && p.Owner != null)
				{
					Profiles[p.Owner] = p;
				}
			}

			return true;
		}
	}
}