using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Server.Misc
{
    public static class ArchivedSaves
    {
        public enum MergeType
        {
            Months,
            Days,
            Hours,
            Minutes
        }

        private static readonly string _DefaultDestination;

        private static string _Destination;

        public static string Destination
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Destination) || _Destination.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    return _DefaultDestination;

                return _Destination;
            }
            set
            {
                if (_Destination == value)
                    return;

                _Destination = value;

                if (_Enabled)
                    Utility.WriteConsoleColor(ConsoleColor.Cyan, "Archives: {0}", Destination);
            }
        }

        private static bool _Enabled;

        public static bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value)
                    return;

                _Enabled = value;

                var dest = _Enabled ? Destination : "Disabled";

                Utility.WriteConsoleColor(ConsoleColor.Cyan, "Archives: {0}", dest);
            }
        }

        public static bool Async { get; set; }

        public static TimeSpan ExpireAge { get; set; }

        public static MergeType Merge { get; set; }

        private static readonly List<IAsyncResult> _Tasks = new List<IAsyncResult>(0x40);

        private static readonly object _TaskRoot = ((ICollection)_Tasks).SyncRoot;

        private static readonly AutoResetEvent _Sync = new AutoResetEvent(true);

        private static readonly Action<string> _Pack = InternalPack;
        private static readonly Action<DateTime> _Prune = InternalPrune;

        public static int PendingTasks
        {
            get
            {
                lock (_TaskRoot)
                    return _Tasks.Count - _Tasks.RemoveAll(t => t.IsCompleted);
            }
        }

        static ArchivedSaves()
        {
            _DefaultDestination = Path.Combine(Core.BaseDirectory, "Backups", "Archived");

            _Destination = Config.Get("AutoSave.ArchivesPath", (string)null);

            Enabled = Config.Get("AutoSave.ArchivesEnabled", false);

            Async = Config.Get("AutoSave.ArchivesAsync", true);

            ExpireAge = Config.Get("AutoSave.ArchivesExpire", TimeSpan.Zero);

            Merge = Config.GetEnum("AutoSave.ArchivesMerging", MergeType.Minutes);
        }

        [CallPriority(int.MaxValue - 10)]
        public static void Configure()
        {
            EventSink.Shutdown += Wait;
            EventSink.WorldSave += Wait;
        }

        public static bool Process(string source)
        {
            if (!Enabled)
                return false;

            if (!Directory.Exists(Destination))
                Directory.CreateDirectory(Destination);

            if (ExpireAge > TimeSpan.Zero)
                BeginPrune(DateTime.UtcNow - ExpireAge);

            if (!string.IsNullOrWhiteSpace(source))
                BeginPack(source);

            return true;
        }

        private static void Wait(WorldSaveEventArgs e)
        {
            WaitForTaskCompletion();
        }

        private static void Wait(ShutdownEventArgs e)
        {
            WaitForTaskCompletion();
        }

        private static void WaitForTaskCompletion()
        {
            if (!Core.Crashed && !Core.Closing)
                return;

            var pending = PendingTasks;

            if (pending <= 0)
                return;

            Utility.WriteConsoleColor(ConsoleColor.Cyan, "Archives: Waiting for {0:#,0} pending tasks...", pending);

            while (pending > 0)
            {
                _Sync.WaitOne(10);

                pending = PendingTasks;
            }

            Utility.WriteConsoleColor(ConsoleColor.Cyan, "Archives: All tasks completed.");
        }

        private static void InternalPack(string source)
        {
            Utility.WriteConsoleColor(ConsoleColor.Cyan, "Archives: Packing started...");

            var sw = Stopwatch.StartNew();

            try
            {
                var now = DateTime.Now;

                var ampm = now.Hour < 12 ? "AM" : "PM";
                var hour12 = now.Hour > 12 ? now.Hour - 12 : now.Hour <= 0 ? 12 : now.Hour;

                string date;

                switch (Merge)
                {
                    case MergeType.Months: date = string.Format("{0}-{1}", now.Month, now.Year); break;
                    case MergeType.Days: date = string.Format("{0}-{1}-{2}", now.Day, now.Month, now.Year); break;
                    case MergeType.Hours: date = string.Format("{0}-{1}-{2} {3:D2} {4}", now.Day, now.Month, now.Year, hour12, ampm); break;
                    case MergeType.Minutes: default: date = string.Format("{0}-{1}-{2} {3:D2}-{4:D2} {5}", now.Day, now.Month, now.Year, hour12, now.Minute, ampm); break;
                }

                var file = string.Format("{0} Saves ({1}).zip", ServerList.ServerName, date);
                var dest = Path.Combine(Destination, file);

                try { File.Delete(dest); }
                catch { }

                ZipFile.CreateFromDirectory(source, dest, CompressionLevel.Optimal, false);
            }
            catch { }

            try { Directory.Delete(source, true); }
            catch { }

            sw.Stop();

            Utility.WriteConsoleColor(ConsoleColor.Cyan, "Archives: Packing done in {0:F1} seconds.", sw.Elapsed.TotalSeconds);
        }

        private static void BeginPack(string source)
        {
            // Do not use async packing during a crash state or when closing.
            if (!Async || Core.Crashed || Core.Closing)
            {
                _Pack.Invoke(source);
                return;
            }

            _Sync.Reset();

            var t = _Pack.BeginInvoke(source, EndPack, source);

            lock (_TaskRoot)
                _Tasks.Add(t);
        }

        private static void EndPack(IAsyncResult r)
        {
            _Pack.EndInvoke(r);

            lock (_TaskRoot)
                _Tasks.Remove(r);

            _Sync.Set();
        }

        private static void InternalPrune(DateTime threshold)
        {
            if (!Directory.Exists(Destination))
                return;

            Utility.WriteConsoleColor(ConsoleColor.Cyan, "Archives: Pruning started...");

            var sw = Stopwatch.StartNew();

            try
            {
                var root = new DirectoryInfo(Destination);

                foreach (FileInfo archive in root.GetFiles("*.zip", SearchOption.AllDirectories))
                {
                    try
                    {
                        if (archive.LastWriteTimeUtc < threshold)
                            archive.Delete();
                    }
                    catch { }
                }
            }
            catch { }

            sw.Stop();

            Utility.WriteConsoleColor(ConsoleColor.Cyan, "Archives: Pruning done in {0:F1} seconds.", sw.Elapsed.TotalSeconds);
        }

        private static void BeginPrune(DateTime threshold)
        {
            // Do not use async pruning during a crash state or when closing.
            if (!Async || Core.Crashed || Core.Closing)
            {
                _Prune.Invoke(threshold);
                return;
            }

            _Sync.Reset();

            var t = _Prune.BeginInvoke(threshold, EndPrune, threshold);

            lock (_TaskRoot)
                _Tasks.Add(t);
        }

        private static void EndPrune(IAsyncResult r)
        {
            _Prune.EndInvoke(r);

            lock (_TaskRoot)
                _Tasks.Remove(r);

            _Sync.Set();
        }
    }
}
