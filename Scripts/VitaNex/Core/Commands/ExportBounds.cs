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
using System.IO;

using Server;
using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace VitaNex.Commands
{
	public static class ExportBoundsCommand
	{
		public static void Initialize()
		{
			CommandUtility.Register(
				"ExportBounds2D",
				AccessLevel.GameMaster,
				e =>
				{
					if (e != null && e.Mobile != null)
					{
						OnExportBounds2D(e.Mobile, e.GetString(0), e.GetString(1));
					}
				});
			CommandUtility.RegisterAlias("ExportBounds2D", "XB2D");

			CommandUtility.Register(
				"ExportBounds3D",
				AccessLevel.GameMaster,
				e =>
				{
					if (e != null && e.Mobile != null)
					{
						OnExportBounds3D(e.Mobile, e.GetString(0), e.GetString(1));
					}
				});
			CommandUtility.RegisterAlias("ExportBounds3D", "XB3D");
		}

		public static void OnExportBounds2D(Mobile m, string speech, string comment)
		{
			if (m == null || m.Deleted || !(m is PlayerMobile))
			{
				return;
			}

			if (String.IsNullOrWhiteSpace(speech))
			{
				speech = "Bounds";
			}

			BoundingBoxPicker.Begin(
				m,
				(from, map, start, end, state) =>
				{
					var r = new Rectangle2D(start, end.Clone2D(1, 1));

					IOUtility.EnsureFile(VitaNexCore.DataDirectory + "/Export/Bounds/2D/" + IOUtility.GetSafeFileName(speech) + ".txt")
							 .AppendText(
								 false,
								 String.Format(
									 "new Rectangle2D({0}, {1}, {2}, {3}), //{4}",
									 //
									 r.Start.X,
									 r.Start.Y,
									 r.Width,
									 r.Height,
									 comment ?? String.Empty));
				},
				null);
		}

		public static void OnExportBounds3D(Mobile m, string speech, string comment)
		{
			if (m == null || m.Deleted || !(m is PlayerMobile))
			{
				return;
			}

			if (String.IsNullOrWhiteSpace(speech))
			{
				speech = "Bounds";
			}

			BoundingBoxPicker.Begin(
				m,
				(from, map, start, end, state) =>
				{
					var r = new Rectangle3D(start, end.Clone3D(1, 1));

					IOUtility
						.EnsureFile(VitaNexCore.DataDirectory + "/Export/Bounds/3D/" + IOUtility.GetSafeFileName(speech) + ".txt")
						.AppendText(
							false,
							String.Format(
								"new Rectangle3D({0}, {1}, {2}, {3}, {4}, {5}), //{6}",
								//
								r.Start.X,
								r.Start.Y,
								r.Start.Z,
								r.Width,
								r.Height,
								r.Depth,
								comment ?? String.Empty));
				},
				null);
		}
	}
}