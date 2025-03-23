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
using System.Diagnostics;

using Server;
using Server.Network;
#endregion

namespace VitaNex.Commands
{
	/// <summary>
	///     Force full Garbage Collection cycle on all generations.
	/// </summary>
	public static class GCCommand
	{
		private static bool _Initialized;
		private static bool _Optimizing;

		public static void Initialize()
		{
			if (_Initialized)
			{
				return;
			}

			_Initialized = true;

			CommandUtility.Register(
				"GC",
				AccessLevel.Administrator,
				e =>
				{
					var message = true;

					if (e.Arguments != null && e.Arguments.Length > 0)
					{
						message = e.GetBoolean(0);
					}

					Optimize(e.Mobile, message);
				});

			CommandUtility.RegisterAlias("GC", "Optimize");
		}

		public static void Optimize(bool message)
		{
			Optimize(null, message);
		}

		public static void Optimize(Mobile m, bool message)
		{
			if (World.Saving || World.Loading || _Optimizing)
			{
				return;
			}

			NetState.FlushAll();
			NetState.Pause();

			_Optimizing = true;

			var now = DateTime.UtcNow;

			if (message)
			{
				World.Broadcast(0x35, true, "[{0}]: The world is optimizing, please wait.", now.ToShortTimeString());
			}

			var watch = new Stopwatch();

			watch.Start();

			double mem = GC.GetTotalMemory(false);

			GC.Collect();

			mem -= GC.GetTotalMemory(false);
			mem = (mem / 1024.0) / 1024.0;

			watch.Stop();
			_Optimizing = false;

			if (m != null)
			{
				m.SendMessage("[{0}]: GC done in {1:F2} seconds.", now.ToShortTimeString(), watch.Elapsed.TotalSeconds);
				m.SendMessage("[{0}]: GC reports {1:#,0.00} MB freed memory.", now.ToShortTimeString(), mem);
			}

			Console.WriteLine("[{0}]: GC done in {1:F2} seconds.", now.ToShortTimeString(), watch.Elapsed.TotalSeconds);
			Console.WriteLine("[{0}]: GC reports {1:#,0.00} MB freed memory.", now.ToShortTimeString(), mem);

			if (message)
			{
				World.Broadcast(
					0x35,
					true,
					"[{0}]: World optimization complete.  The entire process took {1:F1} seconds.",
					DateTime.UtcNow.ToShortTimeString(),
					watch.Elapsed.TotalSeconds);
			}

			NetState.Resume();
		}
	}
}