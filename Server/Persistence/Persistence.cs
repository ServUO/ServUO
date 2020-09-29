#region References
using System;
using System.IO;
#endregion

namespace Server
{
	public static class Persistence
    {
        public static void SerializeBlock(GenericWriter writer, Action<GenericWriter> serializer)
        {
            byte[] data = Array.Empty<byte>();

            if (serializer != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFileWriter w = new BinaryFileWriter(ms, true);

                    try
                    {
                        serializer(w);

                        w.Flush();

                        data = ms.ToArray();
                    }
                    finally
                    {
                        w.Close();
                    }
                }
            }

            writer.Write(0x0C0FFEE0);
            writer.Write(data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                writer.Write(data[i]);
            }
        }

        public static void DeserializeBlock(GenericReader reader, Action<GenericReader> deserializer)
        {
            if (reader.PeekInt() == 0x0C0FFEE0 && reader.ReadInt() == 0x0C0FFEE0)
            {
                int length = reader.ReadInt();

                byte[] data = Array.Empty<byte>();

                if (length > 0)
                {
                    data = new byte[length];
                }

                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = reader.ReadByte();
                }

                if (deserializer != null)
                {
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        BinaryFileReader r = new BinaryFileReader(new BinaryReader(ms));

                        try
                        {
                            deserializer(r);
                        }
                        finally
                        {
                            r.Close();
                        }
                    }
                }
            }
            else 
            {
                deserializer?.Invoke(reader);
            }
        }

        public static void Serialize(string path, Action<GenericWriter> serializer)
		{
			Serialize(new FileInfo(path), serializer);
		}

		public static void Serialize(FileInfo file, Action<GenericWriter> serializer)
		{
			file.Refresh();

			if (file.Directory != null && !file.Directory.Exists)
			{
				file.Directory.Create();
			}

			if (!file.Exists)
			{
				file.Create().Close();
			}

			file.Refresh();

			using (FileStream fs = file.OpenWrite())
			{
				BinaryFileWriter writer = new BinaryFileWriter(fs, true);

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
			Deserialize(path, deserializer, true);
		}

		public static void Deserialize(FileInfo file, Action<GenericReader> deserializer)
		{
			Deserialize(file, deserializer, true);
		}

		public static void Deserialize(string path, Action<GenericReader> deserializer, bool ensure)
		{
			Deserialize(new FileInfo(path), deserializer, ensure);
		}

		public static void Deserialize(FileInfo file, Action<GenericReader> deserializer, bool ensure)
		{
			file.Refresh();

			if (file.Directory != null && !file.Directory.Exists)
			{
				if (!ensure)
				{
					throw new DirectoryNotFoundException();
				}

				file.Directory.Create();
			}

			if (!file.Exists)
			{
				if (!ensure)
				{
					throw new FileNotFoundException
					{
						Source = file.FullName
					};
				}

				file.Create().Close();
			}

			file.Refresh();

			using (FileStream fs = file.OpenRead())
			{
				BinaryFileReader reader = new BinaryFileReader(new BinaryReader(fs));

				try
				{
					deserializer(reader);
				}
				catch (EndOfStreamException eos)
				{
					if (file.Length > 0)
					{
						throw new Exception(string.Format("[Persistence]: {0}", eos));
					}
				}
				catch (Exception e)
				{
					Utility.WriteConsoleColor(ConsoleColor.Red, "[Persistence]: An error was encountered while loading a saved object");

					throw new Exception(string.Format("[Persistence]: {0}", e));
				}
				finally
				{
					reader.Close();
				}
			}
		}
	}
}
