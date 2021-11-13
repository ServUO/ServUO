using System;
using System.IO;
using System.Diagnostics;

using Server.Commands;

namespace Server.Misc
{
    public static class AutoRestart
	{
		public static Timer Timer { get; private set; }
		public static Timer TimerAction { get; private set; }

		public static DateTime RestartTime { get; private set; }

        public static bool Restarting { get; private set; }
        public static bool DoneWarning { get; private set; }

        public static bool Enabled
		{
			get => Config.Get("AutoRestart.Enabled", false);
			set
			{
				Config.Set("AutoRestart.Enabled", value);

				Initialize();
			}
		}

		public static int Hour { get => Config.Get("AutoRestart.Hour", 12); set => Config.Set("AutoRestart.Hour", value); }
		public static int Minute { get => Config.Get("AutoRestart.Minute", 0); set => Config.Set("AutoRestart.Minute", value); }

		public static int Frequency { get => Config.Get("AutoRestart.Frequency", 24); set => Config.Set("AutoRestart.Frequency", value); }

		public static TimeSpan RestartDelay { get => Config.Get("AutoRestart.RestartDelay", TimeSpan.FromSeconds(10.0)); set => Config.Set("AutoRestart.RestartDelay", value); }
		public static TimeSpan WarningDelay { get => Config.Get("AutoRestart.WarningDelay", TimeSpan.FromMinutes(1.0)); set => Config.Set("AutoRestart.WarningDelay", value); }

		public static string RecompilePath => Path.Combine(Core.BaseDirectory, $"_win{(Core.Debug ? "debug" : "release")}.bat");

		public static void Configure()
		{
			CommandSystem.Register("Restart", AccessLevel.Administrator, Restart_OnCommand);
			CommandSystem.Register("Shutdown", AccessLevel.Administrator, Shutdown_OnCommand);
		}

		public static void Initialize()
		{
			if (Enabled)
			{
				var now = DateTime.Now;
				var time = new DateTime(now.Year, now.Month, now.Day, Hour, Minute, 0);

				if (now > time)
				{
					time += TimeSpan.FromHours(Frequency);
				}

				RestartTime = time;

				BeginTimer();

				Utility.WriteLine(ConsoleColor.Magenta, $"Auto Restart: Configured for {time.Hour}:{time.Minute}, every {Frequency} hour{(Frequency == 1 ? "" : "s")}!");
				Utility.WriteLine(ConsoleColor.Magenta, $"Auto Restart: Next Shard Restart: {time}");
			}
			else
			{
				Restarting = false;
				DoneWarning = false;

				StopTimer();

				if (TimerAction != null)
				{
					TimerAction.Stop();
					TimerAction = null;
				}

				Utility.WriteLine(ConsoleColor.Magenta, "Auto Restart: Disabled");
			}
        }

		private static void Restart_OnCommand(CommandEventArgs e)
        {
            if (Restarting)
            {
                e.Mobile.SendMessage("The server is already restarting.");
				return;
            }

            e.Mobile.SendMessage("You have initiated server restart...");

            var recompile = e.GetBoolean(0);

            if (recompile)
            {
                if (!File.Exists(RecompilePath))
                {
                    recompile = false;

                    e.Mobile.SendMessage($"Unable to recompile due to missing file: {RecompilePath}");
                }
                else
                {
                    e.Mobile.SendMessage("Recompiling after restart...");
                }
			}

            StopTimer();

			if (TimerAction != null)
			{
				TimerAction.Stop();
				TimerAction = null;
			}

			TimerAction = Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
            {
                AutoSave.Save();

                Restarting = true;

                TimedShutdown(true, recompile);
            });
        }

		private static void Shutdown_OnCommand(CommandEventArgs e)
        {
            if (Restarting)
            {
                e.Mobile.SendMessage("The server is already shutting down.");
				return;
            }

            e.Mobile.SendMessage("You have initiated server shutdown.");

            StopTimer();

			if (TimerAction != null)
			{
				TimerAction.Stop();
				TimerAction = null;
			}

			TimerAction = Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
            {
                AutoSave.Save();

                Restarting = true;

                TimedShutdown(false);
            });
        }

        private static void BeginTimer()
        {
            StopTimer();

			Timer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), CheckRestart);
        }

        private static void StopTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        private static void CheckRestart()
		{
			if (Restarting || !Enabled)
			{
				return;
			}

			var warning = WarningDelay;

			if (warning > TimeSpan.Zero && !DoneWarning && RestartTime - warning < DateTime.Now)
			{
				World.Broadcast(0x22, true, $"The server will restart in {warning.TotalMinutes:N0} minute{(Math.Truncate(warning.TotalMinutes) == 1 ? "" : "s")}.");

				DoneWarning = true;
				return;
			}

			if (DateTime.Now >= RestartTime)
			{
				AutoSave.Save();

				Restarting = true;

				TimedShutdown(true);
			}
		}

		private static void TimedShutdown(bool restart)
        {
            TimedShutdown(restart, false);
        }

        private static void TimedShutdown(bool restart, bool recompile)
        {
			var delay = RestartDelay;

			if (delay > TimeSpan.Zero)
			{
				World.Broadcast(0x22, true, $"The server restart in {delay.TotalSeconds:N0} second{(Math.Truncate(delay.TotalMinutes) == 1 ? "" : "s")}!");
			}

			if (TimerAction != null)
			{
				TimerAction.Stop();
				TimerAction = null;
			}

			TimerAction = Timer.DelayCall(delay, () =>
			{
				if (recompile)
				{
					try
					{
						Process.Start(RecompilePath);

						Core.Kill(false);

						return;
					}
					catch { }
				}

				Core.Kill(restart);
			});
        }
    }
}
