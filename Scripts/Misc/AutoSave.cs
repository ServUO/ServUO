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

		private static bool m_SavesEnabled = Config.Get("AutoSave.Enabled", true);

        public AutoSave()
            : base(m_Delay - m_Warning, m_Delay)
        {
            this.Priority = TimerPriority.OneMinute;
        }

        public static bool SavesEnabled
        {
            get
            {
                return m_SavesEnabled;
            }
            set
            {
                m_SavesEnabled = value;
            }
        }

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
                m_SavesEnabled = e.GetBoolean(0);
                e.Mobile.SendMessage("Saves have been {0}.", m_SavesEnabled ? "enabled" : "disabled");
            }
            else
            {
                e.Mobile.SendMessage("Format: SetSaves <true | false>");
            }
        }

        public static void Save()
        {
            AutoSave.Save(false);
        }

        public static void Save(bool permitBackgroundWrite)
        {
            if (AutoRestart.Restarting || Commands.CreateWorld.WorldCreating)
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
            if (!m_SavesEnabled || AutoRestart.Restarting)
                return;

            if (m_Warning == TimeSpan.Zero)
            {
                Save(true);
            }
            else
            {
                int s = (int)m_Warning.TotalSeconds;
                int m = s / 60;
                s %= 60;

                if (m > 0 && s > 0)
                    World.Broadcast(0x35, true, "The world will save in {0} minute{1} and {2} second{3}.", m, m != 1 ? "s" : "", s, s != 1 ? "s" : "");
                else if (m > 0)
                    World.Broadcast(0x35, true, "The world will save in {0} minute{1}.", m, m != 1 ? "s" : "");
                else
                    World.Broadcast(0x35, true, "The world will save in {0} second{1}.", s, s != 1 ? "s" : "");

                Timer.DelayCall(m_Warning, new TimerCallback(Save));
            }
        }

        private static void Backup()
        {
            if (m_Backups.Length == 0)
                return;

            string root = Path.Combine(Core.BaseDirectory, "Backups/Automatic");

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            string[] existing = Directory.GetDirectories(root);

            for (int i = 0; i < m_Backups.Length; ++i)
            {
                DirectoryInfo dir = Match(existing, m_Backups[i]);

                if (dir == null)
                    continue;

                if (i > 0)
                {
                    string timeStamp = FindTimeStamp(dir.Name);

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

            string saves = Path.Combine(Core.BaseDirectory, "Saves");

            if (Directory.Exists(saves))
                DirectoryCopy(saves, FormatDirectory(root, m_Backups[m_Backups.Length - 1], GetTimeStamp()), true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }

                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, false);
                }

                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
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

        private static string FormatDirectory(string root, string name, string timeStamp)
        {
            return Path.Combine(root, String.Format("{0} ({1})", name, timeStamp));
        }

        private static string FindTimeStamp(string input)
        {
            int start = input.IndexOf('(');

            if (start >= 0)
            {
                int end = input.IndexOf(')', ++start);

                if (end >= start)
                    return input.Substring(start, end - start);
            }

            return null;
        }

        private static string GetTimeStamp()
        {
            DateTime now = DateTime.UtcNow;

            return String.Format("{0}-{1}-{2} {3}-{4:D2}-{5:D2}",
                now.Day,
                now.Month,
                now.Year,
                now.Hour,
                now.Minute,
                now.Second);
        }
    }
}