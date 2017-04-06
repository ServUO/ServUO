using System;
using System.IO;

using Server.Commands;

namespace Server.Misc
{
    public class AutoSave : Timer
    {
        private static readonly TimeSpan m_Delay = Config.Get("AutoSave.Frequency", TimeSpan.FromMinutes(5.0d));
        private static readonly TimeSpan m_Warning = Config.Get("AutoSave.WarningTime", TimeSpan.Zero);

        private static readonly string[] m_Backups = new string[]
        {
            "Third Backup",
            "Second Backup",
            "Most Recent"
        };

        public AutoSave()
            : base(m_Delay - m_Warning, m_Delay)
        {
            Priority = TimerPriority.OneMinute;
        }

        public static bool SavesEnabled { get; set; } = Config.Get("AutoSave.Enabled", true);

        public static void Initialize()
        {
            new AutoSave().Start();
            CommandSystem.Register("SetSaves", AccessLevel.Administrator, new CommandEventHandler(SetSaves_OnCommand));
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
            {
                e.Mobile.SendMessage("Format: SetSaves <true | false>");
            }
        }

        public static void Save()
        {
            Save(false);
        }

        public static void Save(bool permitBackgroundWrite)
        {
            if (AutoRestart.Restarting)
                return;

            World.WaitForWriteCompletion();

            try
            {
                Backup();
            }
            catch (Exception e)
            {
                Console.WriteLine("WARNING: Automatic backup FAILED: {0}", e);
            }

            World.Save(true, permitBackgroundWrite);
        }

        protected override void OnTick()
        {
            if (!SavesEnabled || AutoRestart.Restarting)
                return;

            if (m_Warning == TimeSpan.Zero)
            {
                Save(true);
            }
            else
            {
                var s = (int)m_Warning.TotalSeconds;
                var m = s / 60;
                s %= 60;

                if (m > 0 && s > 0)
                    World.Broadcast(0x35, true, "The world will save in {0} minute{1} and {2} second{3}.", m, m != 1 ? "s" : "", s, s != 1 ? "s" : "");
                else if (m > 0)
                    World.Broadcast(0x35, true, "The world will save in {0} minute{1}.", m, m != 1 ? "s" : "");
                else
                    World.Broadcast(0x35, true, "The world will save in {0} second{1}.", s, s != 1 ? "s" : "");

                DelayCall(m_Warning, new TimerCallback(Save));
            }
        }

        private static void Backup()
        {
            if (m_Backups.Length == 0)
                return;

            var root = Path.Combine(Core.BackupsDirectory, "Automatic");

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var existing = Directory.GetDirectories(root);

            for (var i = 0; i < m_Backups.Length; ++i)
            {
                var dir = Match(existing, m_Backups[i]);

                if (dir == null)
                    continue;

                if (i > 0)
                {
                    var timeStamp = FindTimeStamp(dir.Name);

                    if (timeStamp != null)
                    {
                        try
                        {
                            dir.MoveTo(FormatDirectory(root, m_Backups[i - 1], timeStamp));
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch
                    {
                    }
                }
            }

            var saves = Core.SavesDirectory;

            if (Directory.Exists(saves))
                Directory.Move(saves, FormatDirectory(root, m_Backups[m_Backups.Length - 1], GetTimeStamp()));
        }

        private static DirectoryInfo Match(string[] paths, string match)
        {
            for (var i = 0; i < paths.Length; ++i)
            {
                var info = new DirectoryInfo(paths[i]);

                if (info.Name.StartsWith(match))
                    return info;
            }

            return null;
        }

        private static string FormatDirectory(string root, string name, string timeStamp)
        {
            return Path.Combine(root, string.Format("{0} ({1})", name, timeStamp));
        }

        private static string FindTimeStamp(string input)
        {
            var start = input.IndexOf('(');

            if (start >= 0)
            {
                var end = input.IndexOf(')', ++start);

                if (end >= start)
                    return input.Substring(start, end - start);
            }

            return null;
        }

        private static string GetTimeStamp()
        {
            var now = DateTime.UtcNow;

            return string.Format("{0}-{1}-{2} {3}-{4:D2}-{5:D2}",
                now.Day,
                now.Month,
                now.Year,
                now.Hour,
                now.Minute,
                now.Second);
        }
    }
}
