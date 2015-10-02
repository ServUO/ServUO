#region Header
// **********
// ServUO - Persistence.cs
// **********
#endregion

#region References
using System;
using System.IO;
#endregion

namespace Server
{
	public static class Persistence
	{
		public static void Serialize(string path, Action<GenericWriter> serializer)
		{
			Serialize(new FileInfo(path), serializer);
		}

		public static void Serialize(FileInfo file, Action<GenericWriter> serializer)
		{
			if (file.Directory != null && !file.Directory.Exists)
			{
				file.Directory.Create();
			}

			if (!file.Exists)
			{
				file.Create().Close();
			}

			using (var fs = file.OpenWrite())
			{
				var writer = new BinaryFileWriter(fs, true);

				try
				{
					serializer(writer);
				}
				finally
				{
					writer.Flush();
					writer.Close();
				}
			}
		}

		public static void Deserialize(string path, Action<GenericReader> deserializer)
		{
			Deserialize(new FileInfo(path), deserializer);
		}

		public static void Deserialize(FileInfo file, Action<GenericReader> deserializer)
		{
			if (file.Directory != null && !file.Directory.Exists)
			{
				throw new DirectoryNotFoundException();
			}

			if (!file.Exists)
			{
				throw new FileNotFoundException();
			}

			using (var fs = file.OpenRead())
			{
				var reader = new BinaryFileReader(new BinaryReader(fs));

				try
				{
					deserializer(reader);
				}
				finally
				{
					reader.Close();
				}
			}
		}
	}
}