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
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex.Modules.AntiAdverts
{
	[CoreModule("Anti Adverts", "1.0.0.2")]
	public static partial class AntiAdverts
	{
		static AntiAdverts()
		{
			CMOptions = new AntiAdvertsOptions();

			Reports = new List<AntiAdvertsReport>(100);
		}

		private static void CMConfig()
		{
			EventSink.Speech += OnSpeech;
		}

		private static void CMInvoke()
		{
			CommandUtility.Register("AntiAds", Access, e => new AntiAvertsReportsGump(e.Mobile).Send());
		}

		private static void CMEnabled()
		{
			EventSink.Speech += OnSpeech;
		}

		private static void CMDisabled()
		{
			EventSink.Speech -= OnSpeech;
		}

		private static void CMSave()
		{
			VitaNexCore.TryCatch(() => ReportsFile.Serialize(SerializeReports), CMOptions.ToConsole);
		}

		private static void CMLoad()
		{
			VitaNexCore.TryCatch(() => ReportsFile.Deserialize(DeserializeReports), CMOptions.ToConsole);
		}

		private static void CMDisposed()
		{ }

		private static void SerializeReports(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.WriteBlockList(Reports, (w, r) => r.Serialize(w));
		}

		private static void DeserializeReports(GenericReader reader)
		{
			reader.GetVersion();

			reader.ReadBlockList(r => new AntiAdvertsReport(r), Reports);
		}
	}
}