using System;
using Server.Commands;

namespace Server.Misc
{
    public class AutoRestart : Timer
    {
        private static readonly TimeSpan RestartDelay = TimeSpan.Zero;// how long the server should remain active before restart (period of 'server wars')
        private static readonly TimeSpan WarningDelay = TimeSpan.FromMinutes(1.0);// at what interval should the shutdown message be displayed?
        
        public static DateTime RestartTime { get; private set; }
        public static bool Restarting { get; private set; }
        public static Timer Timer { get; private set; }
        public static bool DoneWarning { get; private set; }

        public static bool Enabled = Config.Get("AutoRestart.Enabled", false);
        public static int Hour = Config.Get("AutoRestart.Hour", 12);
        public static int Minutes = Config.Get("AutoRestart.Minute", 0);
        public static int Frequency = Config.Get("AutoRestart.Frequency", 24);
        
        public AutoRestart()
            : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
        {
        }

        public static void Initialize()
        {
			CommandSystem.Register("Restart", AccessLevel.Administrator, new CommandEventHandler(Restart_OnCommand));
			CommandSystem.Register("Shutdown", AccessLevel.Administrator, new CommandEventHandler(Shutdown_OnCommand));

            if (Enabled)
            {
                var now = DateTime.Now;
                var force = new DateTime(now.Year, now.Month, now.Day, Hour, Minutes, 0);

                if (now > force)
                {
                    force += TimeSpan.FromHours(Frequency);
                }

                RestartTime = force;

                Timer = new AutoRestart();
                Timer.Start();

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
                Restarting = true;

                Timer.Stop();
                Timer = null;

                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                    {
                        AutoSave.Save();
                        Core.Kill(true);
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
                Restarting = true;

                Timer.Stop();
                Timer = null;

                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    AutoSave.Save();
                    Core.Kill(false);
                });
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

            Restarting = true;
            AutoSave.Save();

            if (RestartDelay > TimeSpan.Zero)
                World.Broadcast(0x22, true, String.Format("The server will be going down in about {0} seconds!", RestartDelay.TotalSeconds.ToString()));

            Timer.DelayCall(RestartDelay, () => Core.Kill(true));
        }
    }
}
