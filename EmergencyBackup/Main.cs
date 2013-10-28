#region Header
// **********
// ServUO - Main.cs
// **********
#endregion

#region References
using System;
using System.IO;

using SevenZip;
#endregion

namespace EmergencyBackup
{
	internal class Core
	{
		private static void Main(string[] args)
		{
			if (args.Length < 2 || args[0] == null || args[1] == null)
			{
				return;
			}

			string savePath = args[0];
			string backupPath = args[1];

			if (!Directory.Exists(savePath))
			{
				return;
			}

			if (args.Length == 3)
			{
				int compression;

				if (args[2] != null)
				{
					try
					{
						compression = Convert.ToInt32(args[2]);
						Compress(savePath, backupPath, (CompressionLevel)compression);
					}
					catch
					{
						Compress(savePath, backupPath);
					}
				}
			}
			else
			{
				Compress(savePath, backupPath);
			}
		}

		private static void Compress(
			string savePath, string backupPath, CompressionLevel compressionLevel = CompressionLevel.None)
		{
			SevenZipCompressor compressor = new SevenZipCompressor {
				CompressionLevel = compressionLevel,
				ScanOnlyWritable = true
			};
			compressor.CustomParameters.Add("mt", "on");

			if (!Directory.Exists(backupPath))
			{
				Directory.CreateDirectory(backupPath);
			}

			compressor.CompressDirectory(savePath, Path.Combine(backupPath, GetTimeStamp() + ".7z"));
		}

		private static string GetTimeStamp()
		{
			DateTime now = DateTime.UtcNow;

			string dayNight = now.Hour < 12 ? "Morning" : "Night";

			return String.Format("{0}-{1}-{2}-{3}", now.Year, now.Month, now.Day, dayNight);
		}
	}
}