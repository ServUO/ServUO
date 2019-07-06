using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace Server.Misc
{
    public static class ArchivedSaves
    {
        public static bool Enabled = Config.Get("AutoSave.Enabled", false);
        public static int ArchiveDuration = Config.Get("AutoSave.Duration", 3);
        public static string Destination = Config.Get("AutoSave.ArchiveDestination", DefaultDestination);

        public static string DefaultDestination = Path.Combine(Core.BaseDirectory, "ArchivedSaves");

        public static DateTime LastArchive { get; set; }

        public static void Initialize()
        {
            if (Enabled)
            {
                Utility.WriteConsoleColor(
                    ConsoleColor.Yellow, String.Format("Save Archives set for every {0}, destination folder: {1}",
                    String.Format("{0} {1}", ArchiveDuration.ToString(), ArchiveDuration == 1 ? "hour" : "hours"),
                    !String.IsNullOrEmpty(Destination) && Destination.Length > 0 ? Destination : DefaultDestination));
            }
        }

        public static void Archive(string originDir)
        {
            if (ArchiveDuration == -1 || DateTime.UtcNow > LastArchive + TimeSpan.FromHours(ArchiveDuration))
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

                Utility.WriteConsoleColor(ConsoleColor.Yellow, "New save archive created. Next Archive: {0}", DateTime.UtcNow + TimeSpan.FromHours(ArchiveDuration));

                LastArchive = DateTime.UtcNow;
            }
        }
    }
}
