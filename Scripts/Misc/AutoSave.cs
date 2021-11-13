using System;
using System.IO;

using Server.Commands;

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

		public static Timer Timer { get; private set; }

		public static bool SavesEnabled
		{
			get => Config.Get("AutoSave.Enabled", true);
			set
			{
				Config.Set("AutoSave.Enabled", value);

				Initialize();
			}
		}

		public static TimeSpan Delay
		{
			get => Config.Get("AutoSave.Frequency", TimeSpan.FromMinutes(5.0));
			set
			{
				Config.Set("AutoSave.Frequency", value);

				StartTimer();
			}
		}

		public static TimeSpan Warning
		{
			get => Config.Get("AutoSave.WarningTime", TimeSpan.Zero);
			set
			{
				Config.Set("AutoSave.WarningTime", value);

				StartTimer();
			}
		}

		public static void Configure()
		{
			CommandSystem.Register("SetSaves", AccessLevel.Administrator, SetSaves_OnCommand);
		}

		public static void Initialize()
		{
			StartTimer();
		}

		private static void StartTimer()
		{
			StopTimer();

			Timer = Timer.DelayCall(Delay - Warning, Delay, Tick);
		}

		private static void StopTimer()
		{
			if (Timer != null)
			{
				Timer.Stop();
				Timer = null;
			}
		}

		[Usage("SetSaves <true | false>")]
		[Description("Enables or disables automatic shard saving.")]
		public static void SetSaves_OnCommand(CommandEventArgs e)
		{
			if (e.Length == 1)
			{
				SavesEnabled = e.GetBoolean(0);

				e.Mobile.SendMessage($"Saves have been {(SavesEnabled ? "enabled" : "disabled")}.");
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
			if (AutoRestart.Restarting || CreateWorld.WorldCreating)
			{
				return;
			}

			World.WaitForWriteCompletion();

			try
			{
				if (!Backup())
				{
					Console.WriteLine("WARNING: Automatic backup FAILED");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"WARNING: Automatic backup FAILED:\n{e}");

				Diagnostics.ExceptionLogging.LogException(e);
			}

			World.Save(true, permitBackgroundWrite);
		}

		private static void Tick()
		{
			if (!SavesEnabled || AutoRestart.Restarting || CreateWorld.WorldCreating)
			{
				return;
			}

			if (Warning <= TimeSpan.Zero)
			{
				Save();
			}
			else
			{
				var s = (int)Warning.TotalSeconds;
				var m = s / 60;

				s %= 60;

				if (m > 0 && s > 0)
				{
					World.Broadcast(0x35, false, $"The world will save in {m} minute{(m != 1 ? "s" : "")} and {s} second{(s != 1 ? "s" : "")}.");
				}
				else if (m > 0)
				{
					World.Broadcast(0x35, false, $"The world will save in {m} minute{(m != 1 ? "s" : "")}.");
				}
				else
				{
					World.Broadcast(0x35, false, $"The world will save in {s} second{(s != 1 ? "s" : "")}.");
				}

				Timer.DelayCall(Warning, Save);
			}
		}

		private static bool Backup()
		{
			if (m_Backups.Length == 0)
			{
				return false;
			}

			var root = Path.Combine(Core.BaseDirectory, "Backups", "Automatic");

			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}

			var tempRoot = Path.Combine(Core.BaseDirectory, "Backups", "Temp");

			if (Directory.Exists(tempRoot))
			{
				Directory.Delete(tempRoot, true);
			}

			var existing = Directory.GetDirectories(root);

			var anySuccess = existing.Length == 0;

			for (var i = 0; i < m_Backups.Length; ++i)
			{
				var dir = Match(existing, m_Backups[i]);

				if (dir == null)
				{
					continue;
				}

				if (i > 0)
				{
					try
					{
						dir.MoveTo(Path.Combine(root, m_Backups[i - 1]));

						anySuccess = true;
					}
					catch (Exception e)
					{
						Diagnostics.ExceptionLogging.LogException(e);
					}
				}
				else
				{
					var delete = true;

					try
					{
						dir.MoveTo(tempRoot);

						delete = !ArchivedSaves.Process(tempRoot);
					}
					catch (Exception e)
					{
						Diagnostics.ExceptionLogging.LogException(e);
					}

					if (delete)
					{
						try
						{
							dir.Delete(true);
						}
						catch (Exception e)
						{
							Diagnostics.ExceptionLogging.LogException(e);
						}
					}
				}
			}

			var saves = Path.Combine(Core.BaseDirectory, "Saves");

			if (Directory.Exists(saves))
			{
				Directory.Move(saves, Path.Combine(root, m_Backups[m_Backups.Length - 1]));
			}

			return anySuccess;
		}

		private static DirectoryInfo Match(string[] paths, string match)
		{
			for (var i = 0; i < paths.Length; ++i)
			{
				var info = new DirectoryInfo(paths[i]);

				if (info.Name.StartsWith(match))
				{
					return info;
				}
			}

			return null;
		}
	}
}
