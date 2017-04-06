using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;

using Server.Accounting;
using Server.Network;

namespace Server.Misc
{
    public class CrashGuard
    {
        private static readonly bool Enabled = true;
        private static readonly bool SaveBackup = true;
        private static readonly bool RestartServer = true;
        private static readonly bool GenerateReport = true;

        public static void Initialize()
        {
            if (Enabled) // If enabled, register our crash event handler
                EventSink.Crashed += CrashGuard_OnCrash;
        }

        public static void CrashGuard_OnCrash(CrashedEventArgs e)
        {
            if (GenerateReport)
                GenerateCrashReport(e);

            World.WaitForWriteCompletion();

            if (SaveBackup)
                Backup();

            /*if ( Core.Service )
            e.Close = true;
            else */
            if (RestartServer)
                Restart(e);
        }

        private static void SendEmail(string filePath)
        {
            Console.Write("Crash: Sending email...");

            var message = new MailMessage(Email.FromAddress, Email.CrashAddresses)
            {
                Subject = "Automated ServUO Crash Report",
                Body = "Automated ServUO Crash Report. See attachment for details."
            };

            message.Attachments.Add(new Attachment(filePath));

            if (Email.Send(message))
                Console.WriteLine("done");
            else
                Console.WriteLine("failed");
        }

        private static string Combine(string path1, string path2)
        {
            if (path1.Length == 0)
                return path2;

            return Path.Combine(path1, path2);
        }

        private static void Restart(CrashedEventArgs e)
        {
            Console.Write("Crash: Restarting...");

            try
            {
                Process.Start(Core.ExePath, Core.Arguments);
                Console.WriteLine("done");

                e.Close = true;
            }
            catch
            {
                Console.WriteLine("failed");
            }
        }

        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static void CreateDirectory(string path1, string path2)
        {
            CreateDirectory(Combine(path1, path2));
        }

        private static void CopyFile(string rootOrigin, string rootBackup, string path)
        {
            var originPath = Combine(rootOrigin, path);
            var backupPath = Combine(rootBackup, path);

            try
            {
                if (File.Exists(originPath))
                    File.Copy(originPath, backupPath);
            }
            catch
            {
            }
        }

        private static void Backup()
        {
            Console.Write("Crash: Backing up...");

            try
            {
                var timeStamp = GetTimeStamp();

                var rootBackup = Path.Combine(Core.BackupsDirectory, "Crashed", timeStamp);
                var rootOrigin = Core.SavesDirectory;

                // Create new directories
                CreateDirectory(rootBackup);
                CreateDirectory(rootBackup, "Accounts/");
                CreateDirectory(rootBackup, "Items/");
                CreateDirectory(rootBackup, "Mobiles/");
                CreateDirectory(rootBackup, "Guilds/");
                CreateDirectory(rootBackup, "Regions/");

                // Copy files
                CopyFile(rootOrigin, rootBackup, "Accounts/Accounts.xml");

                CopyFile(rootOrigin, rootBackup, "Items/Items.bin");
                CopyFile(rootOrigin, rootBackup, "Items/Items.idx");
                CopyFile(rootOrigin, rootBackup, "Items/Items.tdb");

                CopyFile(rootOrigin, rootBackup, "Mobiles/Mobiles.bin");
                CopyFile(rootOrigin, rootBackup, "Mobiles/Mobiles.idx");
                CopyFile(rootOrigin, rootBackup, "Mobiles/Mobiles.tdb");

                CopyFile(rootOrigin, rootBackup, "Guilds/Guilds.bin");
                CopyFile(rootOrigin, rootBackup, "Guilds/Guilds.idx");

                CopyFile(rootOrigin, rootBackup, "Regions/Regions.bin");
                CopyFile(rootOrigin, rootBackup, "Regions/Regions.idx");

                Console.WriteLine("done");
            }
            catch
            {
                Console.WriteLine("failed");
            }
        }

        private static void GenerateCrashReport(CrashedEventArgs e)
        {
            Console.Write("Crash: Generating report...");

            try
            {
                var timeStamp = GetTimeStamp();
                var logFileName = string.Format("Crash {0}.log", timeStamp);

                var logFilePath = Path.Combine(Core.LogsDirectory, logFileName);

                using (var op = new StreamWriter(logFilePath))
                {
                    var ver = Core.Assembly.GetName().Version;

                    op.WriteLine("Server Crash Report");
                    op.WriteLine("===================");
                    op.WriteLine();
                    op.WriteLine("ServUO Version {0}.{1}, Build {2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
                    op.WriteLine("Operating System: {0}", Environment.OSVersion);
                    op.WriteLine(".NET Framework: {0}", Environment.Version);
                    op.WriteLine("Time: {0}", DateTime.UtcNow);

                    try
                    {
                        op.WriteLine("Mobiles: {0}", World.Mobiles.Count);
                    }
                    catch
                    {
                    }

                    try
                    {
                        op.WriteLine("Items: {0}", World.Items.Count);
                    }
                    catch
                    {
                    }

                    op.WriteLine("Exception:");
                    op.WriteLine(e.Exception);
                    op.WriteLine();

                    op.WriteLine("Clients:");

                    try
                    {
                        var states = NetState.Instances;

                        op.WriteLine("- Count: {0}", states.Count);

                        for (var i = 0; i < states.Count; ++i)
                        {
                            var state = states[i];

                            op.Write("+ {0}:", state);

                            var a = state.Account as Account;

                            if (a != null)
                                op.Write(" (account = {0})", a.Username);

                            var m = state.Mobile;

                            if (m != null)
                                op.Write(" (mobile = 0x{0:X} '{1}')", m.Serial.Value, m.Name);

                            op.WriteLine();
                        }
                    }
                    catch
                    {
                        op.WriteLine("- Failed");
                    }
                }

                Console.WriteLine("done");

                if (Email.FromAddress != null && Email.CrashAddresses != null)
                    SendEmail(logFilePath);
            }
            catch
            {
                Console.WriteLine("failed");
            }
        }

        private static string GetTimeStamp()
        {
            var now = DateTime.UtcNow;

            return string.Format("{0}-{1}-{2}-{3}-{4}-{5}",
                now.Day,
                now.Month,
                now.Year,
                now.Hour,
                now.Minute,
                now.Second);
        }
    }
}
