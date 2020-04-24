using Server.Commands;
using System;
using System.IO;

namespace Server.Misc
{
    public static class AutoSave
    {
        private static readonly string[] m_Backups = new[]
        {
            "Third Backup",
            "Second Backup",
            "Most Recent"
        };

        private static readonly TimeSpan m_Delay;
        private static readonly TimeSpan m_Warning;

        private static readonly Timer m_Timer;

        public static bool SavesEnabled { get; set; }

        static AutoSave()
        {
            SavesEnabled = Config.Get("AutoSave.Enabled", true);

            m_Delay = Config.Get("AutoSave.Frequency", TimeSpan.FromMinutes(5.0));
            m_Warning = Config.Get("AutoSave.WarningTime", TimeSpan.Zero);

            m_Timer = Timer.DelayCall(m_Delay - m_Warning, m_Delay, Tick);
            m_Timer.Stop();
        }

        public static void Initialize()
        {
            m_Timer.Start();

            CommandSystem.Register("SetSaves", AccessLevel.Administrator, SetSaves_OnCommand);
        }

        [Usage("SetSaves <true | false>")]
        [Description("Enables or disables automatic shard saving.")]
        public static void SetSaves_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 1)
            {
                SavesEnabled = e.GetBoolean(0);

                e.Mobile.SendMessage("Saves have been {0}.", SavesEnabled ? "enabled" : "disabled");
            }
            else
                e.Mobile.SendMessage("Format: SetSaves <true | false>");
        }

        public static void Save()
        {
            Save(false);
        }

        public static void Save(bool permitBackgroundWrite)
        {
            if (AutoRestart.Restarting || CreateWorld.WorldCreating)
                return;

            World.WaitForWriteCompletion();

            try
            {
                if (!Backup())
                    Console.WriteLine("WARNING: Automatic backup FAILED");
            }
            catch (Exception e)
            {
                Console.WriteLine("WARNING: Automatic backup FAILED:\n{0}", e);
            }

            World.Save(true, permitBackgroundWrite);
        }

        private static void Tick()
        {
            if (!SavesEnabled || AutoRestart.Restarting || Commands.CreateWorld.WorldCreating)
                return;

            if (m_Warning == TimeSpan.Zero)
                Save();
            else
            {
                int s = (int)m_Warning.TotalSeconds;
                int m = s / 60;
                s %= 60;

                if (m > 0 && s > 0)
                    World.Broadcast(0x35, false, "The world will save in {0} minute{1} and {2} second{3}.", m, m != 1 ? "s" : "", s, s != 1 ? "s" : "");
                else if (m > 0)
                    World.Broadcast(0x35, false, "The world will save in {0} minute{1}.", m, m != 1 ? "s" : "");
                else
                    World.Broadcast(0x35, false, "The world will save in {0} second{1}.", s, s != 1 ? "s" : "");

                Timer.DelayCall(m_Warning, Save);
            }
        }

        private static bool Backup()
        {
            if (m_Backups.Length == 0)
                return false;

            string root = Path.Combine(Core.BaseDirectory, "Backups/Automatic");

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            string tempRoot = Path.Combine(Core.BaseDirectory, "Backups/Temp");

            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, true);

            string[] existing = Directory.GetDirectories(root);

            bool anySuccess = existing.Length == 0;

            for (int i = 0; i < m_Backups.Length; ++i)
            {
                DirectoryInfo dir = Match(existing, m_Backups[i]);

                if (dir == null)
                    continue;

                if (i > 0)
                {
                    try
                    {
                        dir.MoveTo(Path.Combine(root, m_Backups[i - 1]));

                        anySuccess = true;
                    }
                    catch { }
                }
                else
                {
                    bool delete = true;

                    try
                    {
                        dir.MoveTo(tempRoot);

                        delete = !ArchivedSaves.Process(tempRoot);
                    }
                    catch { }

                    if (delete)
                    {
                        try { dir.Delete(true); }
                        catch { }
                    }
                }
            }

            string saves = Path.Combine(Core.BaseDirectory, "Saves");

            if (Directory.Exists(saves))
                Directory.Move(saves, Path.Combine(root, m_Backups[m_Backups.Length - 1]));

            return anySuccess;
        }

        private static DirectoryInfo Match(string[] paths, string match)
        {
            for (int i = 0; i < paths.Length; ++i)
            {
                DirectoryInfo info = new DirectoryInfo(paths[i]);

                if (info.Name.StartsWith(match))
                    return info;
            }

            return null;
        }
    }
}
