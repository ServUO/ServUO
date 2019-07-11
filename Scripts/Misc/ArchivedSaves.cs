using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Server.Misc
{
    public static class ArchivedSaves
    {
        public static bool Enabled = Config.Get("AutoSave.ArchivesEnabled", false);
        public static int ArchiveDuration = Config.Get("AutoSave.Duration", 3);
        public static string Destination = Config.Get("AutoSave.ArchiveDestination", DefaultDestination);

        public static string DefaultDestination = Path.Combine(Core.BaseDirectory, "ArchivedSaves");

        public static DateTime LastArchive { get; set; }

        public static void Initialize()
        {
            if (Enabled)
            {
                Utility.WriteConsoleColor(
                    ConsoleColor.Cyan, String.Format("Save Archives set for every {0}, destination folder: {1}",
                    ArchiveDuration == -1 ? "world save" : String.Format("{0} {1}", ArchiveDuration.ToString(), ArchiveDuration == 1 ? "hour" : "hours"),
                    !String.IsNullOrEmpty(Destination) && Destination.Length > 0 ? Destination : DefaultDestination));
            }
        }

        private static long _TickCount;

        public static void RunArchive(string oiginDir)
        {
            if (ArchiveDuration == -1 || DateTime.UtcNow > LastArchive + TimeSpan.FromHours(ArchiveDuration))
            {
                LastArchive = DateTime.UtcNow;
                var resultTask = RunArchiveTask(oiginDir);

                var pollingTimer = new TaskPollingTimer<string>(resultTask, results =>
                {
                    Utility.WriteConsoleColor(ConsoleColor.Cyan, "...Complete, save archive created in {0} milliseconds. Next archive: {1}", Core.TickCount - _TickCount, DateTime.Now + TimeSpan.FromHours(ArchiveDuration));
                });

                resultTask.Start();
                pollingTimer.Start();

                Utility.WriteConsoleColor(ConsoleColor.Cyan, "Creating backup save archive...");
                _TickCount = Core.TickCount;
            }
        }

        public static Task<string> RunArchiveTask(string originDir)
        {
            return new Task<string>(() => Archive(originDir));
        }

        public static string Archive(string originDir)
        {
            var destination = !String.IsNullOrEmpty(Destination) && Destination.Length > 0 ? Destination : DefaultDestination;

            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            var year = DateTime.UtcNow.Year.ToString();
            var month = new DateTimeFormatInfo().GetMonthName(DateTime.UtcNow.Month);

            if (!Directory.Exists(Path.Combine(destination, year)))
                Directory.CreateDirectory(Path.Combine(destination, year));

            if (!Directory.Exists(Path.Combine(destination, year, month)))
                Directory.CreateDirectory(Path.Combine(destination, year, month));

            ZipFile.CreateFromDirectory(originDir, Path.Combine(destination, year, month, AutoSave.GetTimeStamp() + ".zip"), CompressionLevel.Optimal, true);

            return destination;
        }
    }
}
