using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

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

		private static readonly string _DefaultDestination = Path.Combine(Core.BaseDirectory, "Backups", "Archived");

		public static string Destination
		{
			get
			{
				var dest = Config.Get("AutoSave.ArchivesPath", (string)null);

				if (String.IsNullOrWhiteSpace(dest) || dest.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
					return _DefaultDestination;

				return dest;
			}
			set
			{
				Config.Set("AutoSave.ArchivesPath", value);

				if (Enabled)
					Utility.WriteLine(ConsoleColor.Cyan, $"Archives: {value}");
			}
		}

		public static bool Enabled
		{
			get => Config.Get("AutoSave.ArchivesEnabled", false);
			set
			{
				Config.Set("AutoSave.ArchivesEnabled", value);

				var dest = value ? Destination : "Disabled";

				Utility.WriteLine(ConsoleColor.Cyan, $"Archives: {dest}");
			}
		}

		public static bool Async { get => Config.Get("AutoSave.ArchivesAsync", true); set => Config.Set("AutoSave.ArchivesAsync", value); }

		public static TimeSpan ExpireAge { get => Config.Get("AutoSave.ArchivesExpire", TimeSpan.Zero); set => Config.Set("AutoSave.ArchivesExpire", value); }

		public static MergeType Merge { get => Config.GetEnum("AutoSave.ArchivesMerging", MergeType.Minutes); set => Config.SetEnum("AutoSave.ArchivesMerging", value); }

		private static readonly List<Task> _Tasks = new List<Task>(0x40);

		private static readonly object _TaskRoot = ((ICollection)_Tasks).SyncRoot;

		private static readonly AutoResetEvent _Sync = new AutoResetEvent(true);

		public static int PendingTasks
		{
			get
			{
				lock (_TaskRoot)
					return _Tasks.Count - _Tasks.RemoveAll(t => t.IsCompleted);
			}
		}

		[CallPriority(Int32.MaxValue - 10)]
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

			if (!String.IsNullOrWhiteSpace(source))
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

			Utility.WriteLine(ConsoleColor.Cyan, $"Archives: Waiting for {pending:#,0} pending tasks...");

			while (pending > 0)
			{
				_Sync.WaitOne(10);

				pending = PendingTasks;
			}

			Utility.WriteLine(ConsoleColor.Cyan, "Archives: All tasks completed.");
		}

		private static void EndTask(Task t)
		{
			lock (_TaskRoot)
				_Tasks.Remove(t);

			_Sync.Set();
		}

		private static void InternalPack(string source)
		{
			Utility.WriteLine(ConsoleColor.Cyan, "Archives: Packing started...");

			var sw = Stopwatch.StartNew();

			var dest = Destination;

			try
			{
				var now = DateTime.Now;

				var ampm = now.Hour < 12 ? "AM" : "PM";
				var hour12 = now.Hour > 12 ? now.Hour - 12 : now.Hour <= 0 ? 12 : now.Hour;

				string date;

				switch (Merge)
				{
					case MergeType.Months: date = $"{now.Month}-{now.Year}"; break;
					case MergeType.Days: date = $"{now.Day}-{now.Month}-{now.Year}"; break;
					case MergeType.Hours: date = $"{now.Day}-{now.Month}-{now.Year} {hour12:D2} {ampm}"; break;
					case MergeType.Minutes: default: date = $"{now.Day}-{now.Month}-{now.Year} {hour12:D2}-{now.Minute:D2} {ampm}"; break;
				}

				var file = $"{ServerList.ServerName} Saves ({date}).zip";

				dest = Path.Combine(Destination, file);

				try { File.Delete(dest); }
				catch { }

				ZipFile.CreateFromDirectory(source, dest, CompressionLevel.Optimal, false);
			}
			catch (Exception e)
			{
				Utility.WriteLine(ConsoleColor.Yellow, $"Archives: Failed to create archive '{source}' -> '{dest}':\n{e}");
			}

			try { Directory.Delete(source, true); }
			catch { }

			sw.Stop();

			Utility.WriteLine(ConsoleColor.Cyan, $"Archives: Packing done in {sw.Elapsed.TotalSeconds:F1} seconds.");
		}

		private static void BeginPack(string source)
		{
			// Do not use async packing during a crash state or when closing.
			if (!Async || Core.Crashed || Core.Closing)
			{
				InternalPack(source);
				return;
			}

			_Sync.Reset();

			var t = Task.Factory.StartNew(o => InternalPack((string)o), source, TaskCreationOptions.LongRunning);

			lock (_TaskRoot)
				_Tasks.Add(t);

			t.ContinueWith(EndTask);
		}

		private static void InternalPrune(DateTime threshold)
		{
			if (!Directory.Exists(Destination))
				return;

			Utility.WriteLine(ConsoleColor.Cyan, "Archives: Pruning started...");

			var sw = Stopwatch.StartNew();

			try
			{
				var root = new DirectoryInfo(Destination);

				foreach (var archive in root.GetFiles("*.zip", SearchOption.AllDirectories))
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

			Utility.WriteLine(ConsoleColor.Cyan, $"Archives: Pruning done in {sw.Elapsed.TotalSeconds:F1} seconds.");
		}

		private static void BeginPrune(DateTime threshold)
		{
			// Do not use async pruning during a crash state or when closing.
			if (!Async || Core.Crashed || Core.Closing)
			{
				InternalPrune(threshold);
				return;
			}

			_Sync.Reset();

			var t = Task.Factory.StartNew(o => InternalPrune((DateTime)o), threshold, TaskCreationOptions.LongRunning);

			lock (_TaskRoot)
				_Tasks.Add(t);

			t.ContinueWith(EndTask);
		}
	}
}
