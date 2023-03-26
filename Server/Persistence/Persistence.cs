#region References
using System;
using System.IO;
using System.Xml;
#endregion

namespace Server
{
	public static class Persistence
	{
		public static void SerializeBlock(GenericWriter writer, Action<GenericWriter> serializer)
		{
			var data = Array.Empty<byte>();

			if (serializer != null)
			{
				using (var ms = new MemoryStream())
				{
					var w = new BinaryFileWriter(ms, true);

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

			for (var i = 0; i < data.Length; i++)
			{
				writer.Write(data[i]);
			}
		}

		public static void DeserializeBlock(GenericReader reader, Action<GenericReader> deserializer)
		{
			if (reader.PeekInt() == 0x0C0FFEE0 && reader.ReadInt() == 0x0C0FFEE0)
			{
				var length = reader.ReadInt();

				var data = Array.Empty<byte>();

				if (length > 0)
				{
					data = new byte[length];
				}

				for (var i = 0; i < data.Length; i++)
				{
					data[i] = reader.ReadByte();
				}

				if (deserializer != null)
				{
					using (var ms = new MemoryStream(data))
					{
						var r = new BinaryFileReader(new BinaryReader(ms));

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

			using (var fs = file.OpenRead())
			{
				var reader = new BinaryFileReader(new BinaryReader(fs));

				try
				{
					deserializer(reader);
				}
				catch (EndOfStreamException eos)
				{
					if (file.Length > 0)
					{
						throw new Exception(String.Format("[Persistence]: {0}", eos));
					}
				}
				catch (Exception e)
				{
					Utility.WriteLine(ConsoleColor.Red, "[Persistence]: An error was encountered while loading a saved object");

					throw new Exception(String.Format("[Persistence]: {0}", e));
				}
				finally
				{
					reader.Close();
				}
			}
		}

		public static void Save(string path, string root, Action<XmlElement> serializer)
		{
			Save(new FileInfo(path), root, serializer);
		}

		public static void Save(FileInfo file, string root, Action<XmlElement> serializer)
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

			var doc = new XmlDocument();

			doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

			var node = doc.CreateElement(root);

			serializer(node);

			doc.AppendChild(node);

			doc.Save(file.FullName);
		}

		public static void Load(string path, string root, Action<XmlElement> deserializer)
		{
			Load(path, root, deserializer, true);
		}

		public static void Load(FileInfo file, string root, Action<XmlElement> deserializer)
		{
			Load(file, root, deserializer, true);
		}

		public static void Load(string path, string root, Action<XmlElement> deserializer, bool ensure)
		{
			Load(new FileInfo(path), root, deserializer, ensure);
		}

		public static void Load(FileInfo file, string root, Action<XmlElement> deserializer, bool ensure)
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

			try
			{
				var doc = new XmlDocument();

				doc.Load(file.FullName);

				var node = doc[root];

				deserializer(node);
			}
			catch (EndOfStreamException eos)
			{
				if (file.Length > 0)
				{
					throw new Exception(String.Format("[Persistence]: {0}", eos));
				}
			}
			catch (Exception e)
			{
				Utility.WriteLine(ConsoleColor.Red, "[Persistence]: An error was encountered while loading a saved object");

				throw new Exception(String.Format("[Persistence]: {0}", e));
			}
		}
	}
}
