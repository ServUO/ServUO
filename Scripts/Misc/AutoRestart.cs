using Server.Commands;

using System;
using System.IO;
using System.Diagnostics;

namespace Server.Misc
{
    public class AutoRestart : Timer
    {
        private static readonly TimeSpan RestartDelay = TimeSpan.FromSeconds(10);  // how long the server should remain active before restart (period of 'server wars')
        private static readonly TimeSpan WarningDelay = TimeSpan.FromMinutes(1.0); // at what interval should the shutdown message be displayed?

        public static DateTime RestartTime { get; private set; }
        public static bool Restarting { get; private set; }
        public static Timer Timer { get; private set; }
        public static bool DoneWarning { get; private set; }

        public static bool Enabled = Config.Get("AutoRestart.Enabled", false);
        public static int Hour = Config.Get("AutoRestart.Hour", 12);
        public static int Minutes = Config.Get("AutoRestart.Minute", 0);
        public static int Frequency = Config.Get("AutoRestart.Frequency", 24);

        public static readonly string RecompilePath = Path.Combine(Core.BaseDirectory, Core.Debug ? "Compile.WIN - Debug.bat" : "Compile.WIN - Release.bat");

        public AutoRestart()
            : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
        {
        }

        public static void Initialize()
        {
            CommandSystem.Register("Restart", AccessLevel.Administrator, Restart_OnCommand);
            CommandSystem.Register("Shutdown", AccessLevel.Administrator, Shutdown_OnCommand);

            if (Enabled)
            {
                DateTime now = DateTime.Now;
                DateTime force = new DateTime(now.Year, now.Month, now.Day, Hour, Minutes, 0);

                if (now > force)
                {
                    force += TimeSpan.FromHours(Frequency);
                }

                RestartTime = force;

                BeginTimer();

                Utility.WriteConsoleColor(ConsoleColor.Magenta, "[Auto Restart] Configured for {0}:{1}:00, every {2} hours!", RestartTime.Hour, RestartTime.Minute, Frequency);
                Utility.WriteConsoleColor(ConsoleColor.Magenta, "[Auto Restart] Next Shard Restart: {0}", RestartTime.ToString());
            }
        }

        public static void Restart_OnCommand(CommandEventArgs e)
        {
            if (Restarting)
            {
                e.Mobile.SendMessage("The server is already restarting.");
            }
            else
            {
                e.Mobile.SendMessage("You have initiated server restart.");

                StopTimer();

                bool recompile = e.Arguments.Length > 0 && e.Arguments[0].ToLower() == "true";

                if (recompile)
                {
                    if (!File.Exists(RecompilePath))
                    {
                        e.Mobile.SendMessage("Unable to Re-Compile due to missing file: {0}", RecompilePath);
                        recompile = false;
                    }
                    else
                    {
                        e.Mobile.SendMessage("Recompiling after restart!");
                    }
                }

                DelayCall(TimeSpan.FromSeconds(1), () =>
                    {
                        AutoSave.Save();

                        Restarting = true;
                        TimedShutdown(true, recompile);
                    });
            }
        }

        public static void Shutdown_OnCommand(CommandEventArgs e)
        {
            if (Restarting)
            {
                e.Mobile.SendMessage("The server is already shutting down.");
            }
            else
            {
                e.Mobile.SendMessage("You have initiated server shutdown.");

                StopTimer();

                DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    AutoSave.Save();
                    Restarting = true;

                    TimedShutdown(false);
                });
            }
        }

        private static void BeginTimer()
        {
            StopTimer();

            Timer = new AutoRestart();
            Timer.Start();
        }

        private static void StopTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        protected override void OnTick()
        {
            if (Restarting || !Enabled)
                return;

            if (WarningDelay > TimeSpan.Zero && !DoneWarning && RestartTime - WarningDelay < DateTime.Now)
            {
                World.Broadcast(0x22, true, "The server will be going down in about {0} minute{1}.", WarningDelay.TotalMinutes.ToString(), WarningDelay.TotalMinutes == 1 ? "" : "s");

                DoneWarning = true;
                return;
            }

            if (DateTime.Now < RestartTime)
            {
                return;
            }

            AutoSave.Save();
            Restarting = true;

            TimedShutdown(true);
        }

        private static void TimedShutdown(bool restart)
        {
            TimedShutdown(restart, false);
        }

        private static void TimedShutdown(bool restart, bool recompile)
        {
            World.Broadcast(0x22, true, string.Format("The server will be going down in about {0} seconds!", RestartDelay.TotalSeconds.ToString()));
            DelayCall(RestartDelay, (rest, recomp) =>
                {
                    if (recomp)
                    {
                        Process.Start(RecompilePath);
                        Core.Kill();
                    }
                    else
                    {
                        Core.Kill(rest);
                    }
                },
                restart, recompile);
        }
    }
}
