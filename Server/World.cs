#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

using Server.Guilds;
using Server.Network;
#endregion

namespace Server
{
	public static class World
	{
		private static readonly bool m_Metrics = Config.Get("General.Metrics", false);

		private static readonly Dictionary<Serial, IEntity> m_AddQueue = new Dictionary<Serial, IEntity>();
		private static readonly Queue<IEntity> m_DeleteQueue = new Queue<IEntity>();

		private static readonly ManualResetEvent m_DiskWriteHandle = new ManualResetEvent(true);

		public static DateTime LastSave { get; private set; }

		public static bool Saving { get; private set; }
		public static bool Loaded { get; private set; }
		public static bool Loading { get; private set; }

		public static bool Volatile => !Loaded || Saving || NetState.Paused;

		public static Dictionary<Serial, Mobile> Mobiles { get; } = new Dictionary<Serial, Mobile>(0x40000);
		public static Dictionary<Serial, Item> Items { get; } = new Dictionary<Serial, Item>(0x100000);

		public static readonly string MobileIndexPath = Path.Combine("Saves/Mobiles/", "Mobiles.idx");
		public static readonly string MobileTypesPath = Path.Combine("Saves/Mobiles/", "Mobiles.tdb");
		public static readonly string MobileDataPath = Path.Combine("Saves/Mobiles/", "Mobiles.bin");

		public static readonly string ItemIndexPath = Path.Combine("Saves/Items/", "Items.idx");
		public static readonly string ItemTypesPath = Path.Combine("Saves/Items/", "Items.tdb");
		public static readonly string ItemDataPath = Path.Combine("Saves/Items/", "Items.bin");

		public static readonly string GuildIndexPath = Path.Combine("Saves/Guilds/", "Guilds.idx");
		public static readonly string GuildDataPath = Path.Combine("Saves/Guilds/", "Guilds.bin");

		public static void NotifyDiskWriteComplete()
		{
			if (m_DiskWriteHandle.Set())
			{
				Console.WriteLine("Closing Save Files. ");
			}
		}

		public static void WaitForWriteCompletion()
		{
			m_DiskWriteHandle.WaitOne();
		}

		public static bool OnDelete(IEntity entity)
		{
			if (Saving || Loading || !Loaded)
			{
				m_DeleteQueue.Enqueue(entity);

				return false;
			}

			return true;
		}

		public static void Broadcast(int hue, bool ascii, string text)
		{
			Broadcast(hue, ascii, AccessLevel.Player, text);
		}

		public static void Broadcast(int hue, bool ascii, AccessLevel access, string text)
		{
			var e = new WorldBroadcastEventArgs(hue, ascii, access, text);

			EventSink.InvokeWorldBroadcast(e);

			hue = e.Hue;
			ascii = e.Ascii;
			text = e.Text;
			access = e.Access;

			if (String.IsNullOrWhiteSpace(text))
			{
				return;
			}

			Packet p;

			if (ascii)
			{
				p = new AsciiMessage(Serial.MinusOne, -1, MessageType.Regular, hue, 3, "System", text);
			}
			else
			{
				p = new UnicodeMessage(Serial.MinusOne, -1, MessageType.Regular, hue, 3, "ENU", "System", text);
			}

			var list = NetState.Instances;

			p.Acquire();

			foreach (var s in list)
			{
				if (s.Mobile != null && s.Mobile.AccessLevel >= access)
				{
					s.Send(p);
				}
			}

			p.Release();

			NetState.FlushAll();
		}

		public static void Broadcast(int hue, bool ascii, string format, params object[] args)
		{
			Broadcast(hue, ascii, AccessLevel.Player, format, args);
		}

		public static void Broadcast(int hue, bool ascii, AccessLevel access, string format, params object[] args)
		{
			Broadcast(hue, ascii, access, String.Format(format, args));
		}

		private interface IEntityEntry
		{
			Serial Serial { get; }
			int TypeID { get; }
			long Position { get; }
			int Length { get; }
		}

		private sealed class GuildEntry : IEntityEntry
		{
			public BaseGuild Guild { get; }

			public Serial Serial => new Serial(Guild?.Id ?? 0);

			public int TypeID => 0;

			public long Position { get; }
			public int Length { get; }

			public GuildEntry(BaseGuild g, long pos, int length)
			{
				Guild = g;
				Position = pos;
				Length = length;
			}
		}

		private sealed class ItemEntry : IEntityEntry
		{
			public Item Item { get; }

			public Serial Serial => Item == null ? Serial.MinusOne : Item.Serial;

			public int TypeID { get; }
			public string TypeName { get; }

			public long Position { get; }
			public int Length { get; }

			public ItemEntry(Item item, int typeID, string typeName, long pos, int length)
			{
				Item = item;
				TypeID = typeID;
				TypeName = typeName;
				Position = pos;
				Length = length;
			}
		}

		private sealed class MobileEntry : IEntityEntry
		{
			public Mobile Mobile { get; }

			public Serial Serial => Mobile == null ? Serial.MinusOne : Mobile.Serial;

			public int TypeID { get; }
			public string TypeName { get; }

			public long Position { get; }
			public int Length { get; }

			public MobileEntry(Mobile mobile, int typeID, string typeName, long pos, int length)
			{
				Mobile = mobile;
				TypeID = typeID;
				TypeName = typeName;
				Position = pos;
				Length = length;
			}
		}

		public static string LoadingType { get; private set; }

		private static readonly Type[] m_SerialTypeArray = new Type[1] { typeof(Serial) };

		private static List<Tuple<ConstructorInfo, string>> ReadTypes(BinaryReader tdbReader)
		{
			var count = tdbReader.ReadInt32();

			var types = new List<Tuple<ConstructorInfo, string>>(count);

			for (var i = 0; i < count; ++i)
			{
				var typeName = tdbReader.ReadString();

				var t = ScriptCompiler.FindTypeByFullName(typeName);

				if (t == null)
				{
					Console.WriteLine("failed");

					if (!Core.Service)
					{
						Console.WriteLine($"Error: Type '{typeName}' was not found. Delete all of those types? (y/n)");

						if (typeName.StartsWith("Server.Engines.MLQuests"))
						{
							types.Add(null);
							Console.Write($"World: Loading... Deleted '{typeName}'");
							continue;
						}

						if (Console.ReadKey(true).Key == ConsoleKey.Y)
						{
							types.Add(null);
							Utility.PushColor(ConsoleColor.Yellow);
							Console.Write("World: Loading...");
							Utility.PopColor();
							continue;
						}

						Console.WriteLine("Types will not be deleted. An exception will be thrown.");
					}
					else
					{
						Console.WriteLine($"Error: Type '{typeName}' was not found.");
					}

					throw new Exception($"Missing type '{typeName}'");
				}

				if (t.IsAbstract)
				{
					foreach (var at in ScriptCompiler.FindTypesByFullName(t.FullName))
					{
						if (at != t && !at.IsAbstract)
						{
							t = at;
							typeName = at.FullName;
							break;
						}
					}

					if (t.IsAbstract)
					{
						Console.WriteLine("failed");

						if (!Core.Service)
						{
							Console.WriteLine($"Error: Type '{typeName}' is abstract. Delete all of those types? (y/n)");

							if (Console.ReadKey(true).Key == ConsoleKey.Y)
							{
								types.Add(null);
								Utility.PushColor(ConsoleColor.Yellow);
								Console.Write("World: Loading...");
								Utility.PopColor();
								continue;
							}

							Console.WriteLine("Types will not be deleted. An exception will be thrown.");
						}
						else
						{
							Console.WriteLine($"Error: Type '{typeName}' is abstract.");
						}

						throw new Exception($"Abstract type '{typeName}'");
					}
				}

				var ctor = t.GetConstructor(m_SerialTypeArray);

				if (ctor != null)
				{
					types.Add(new Tuple<ConstructorInfo, string>(ctor, typeName));
				}
				else
				{
					throw new Exception($"Type '{t}' does not have a serialization constructor");
				}
			}

			return types;
		}

		public static void Load()
		{
			if (Loaded)
			{
				return;
			}

			Loaded = true;
			LoadingType = null;

			Utility.PushColor(ConsoleColor.Yellow);
			Console.Write("World: Loading...");
			Utility.PopColor();

			var watch = Stopwatch.StartNew();

			Loading = true;

			var ctorArgs = new object[1];

			var items = new List<ItemEntry>();
			var mobiles = new List<MobileEntry>();
			var guilds = new List<GuildEntry>();

			if (File.Exists(MobileIndexPath) && File.Exists(MobileTypesPath))
			{
				using (var idx = new FileStream(MobileIndexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var idxReader = new BinaryReader(idx);

					using (var tdb = new FileStream(MobileTypesPath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						var tdbReader = new BinaryReader(tdb);

						var types = ReadTypes(tdbReader);

						var mobileCount = idxReader.ReadInt32();

						for (var i = 0; i < mobileCount; ++i)
						{
							var typeID = idxReader.ReadInt32();
							var serial = idxReader.ReadInt32();
							var pos = idxReader.ReadInt64();
							var length = idxReader.ReadInt32();

							var objs = types[typeID];

							if (objs == null)
							{
								continue;
							}

							Mobile m = null;
							var ctor = objs.Item1;
							var typeName = objs.Item2;

							try
							{
								ctorArgs[0] = new Serial(serial);
								m = (Mobile)ctor.Invoke(ctorArgs);
							}
							catch (Exception ex)
							{
								Diagnostics.ExceptionLogging.LogException(ex);
							}

							if (m != null)
							{
								mobiles.Add(new MobileEntry(m, typeID, typeName, pos, length));
								AddMobile(m);
							}
						}

						tdbReader.Close();
					}

					idxReader.Close();
				}
			}

			if (File.Exists(ItemIndexPath) && File.Exists(ItemTypesPath))
			{
				using (var idx = new FileStream(ItemIndexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var idxReader = new BinaryReader(idx);

					using (var tdb = new FileStream(ItemTypesPath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						var tdbReader = new BinaryReader(tdb);

						var types = ReadTypes(tdbReader);

						var itemCount = idxReader.ReadInt32();

						for (var i = 0; i < itemCount; ++i)
						{
							var typeID = idxReader.ReadInt32();
							var serial = idxReader.ReadInt32();
							var pos = idxReader.ReadInt64();
							var length = idxReader.ReadInt32();

							var objs = types[typeID];

							if (objs == null)
							{
								continue;
							}

							Item item = null;
							var ctor = objs.Item1;
							var typeName = objs.Item2;

							try
							{
								ctorArgs[0] = new Serial(serial);
								item = (Item)ctor.Invoke(ctorArgs);
							}
							catch (Exception e)
							{
								Diagnostics.ExceptionLogging.LogException(e);
							}

							if (item != null)
							{
								items.Add(new ItemEntry(item, typeID, typeName, pos, length));
								AddItem(item);
							}
						}

						tdbReader.Close();
					}

					idxReader.Close();
				}
			}

			if (File.Exists(GuildIndexPath))
			{
				using (var idx = new FileStream(GuildIndexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var idxReader = new BinaryReader(idx);

					var guildCount = idxReader.ReadInt32();

					var createEventArgs = new CreateGuildEventArgs(-1);

					for (var i = 0; i < guildCount; ++i)
					{
						idxReader.ReadInt32(); //no typeid for guilds

						var id = idxReader.ReadInt32();
						var pos = idxReader.ReadInt64();
						var length = idxReader.ReadInt32();

						createEventArgs.Id = id;
						createEventArgs.Guild = null;

						var guild = CreateGuild?.Invoke(createEventArgs);

						if (guild != null)
						{
							guilds.Add(new GuildEntry(guild, pos, length));
						}
					}

					idxReader.Close();
				}
			}

			bool failedMobiles = false, failedItems = false, failedGuilds = false;
			Type failedType = null;
			var failedSerial = Serial.Zero;
			Exception failed = null;
			var failedTypeID = 0;

			if (File.Exists(MobileDataPath))
			{
				using (var bin = new FileStream(MobileDataPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920))
				{
					var reader = new BinaryFileReader(new BinaryReader(bin));

					for (var i = 0; i < mobiles.Count; ++i)
					{
						var entry = mobiles[i];
						var m = entry.Mobile;

						if (m != null)
						{
							reader.Seek(entry.Position, SeekOrigin.Begin);

							try
							{
								LoadingType = entry.TypeName;

								m.Deserialize(reader);

								if (reader.Position != (entry.Position + entry.Length))
								{
									throw new Exception($"***** Bad serialize on {m.GetType()} *****");
								}
							}
							catch (Exception e)
							{
								mobiles.RemoveAt(i);

								failed = e;
								failedMobiles = true;
								failedType = m.GetType();
								failedTypeID = entry.TypeID;
								failedSerial = m.Serial;

								break;
							}
						}
					}

					reader.Close();
				}
			}

			if (!failedMobiles && File.Exists(ItemDataPath))
			{
				using (var bin = new FileStream(ItemDataPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920))
				{
					var reader = new BinaryFileReader(new BinaryReader(bin));

					for (var i = 0; i < items.Count; ++i)
					{
						var entry = items[i];
						var item = entry.Item;

						if (item != null)
						{
							reader.Seek(entry.Position, SeekOrigin.Begin);

							try
							{
								LoadingType = entry.TypeName;

								item.Deserialize(reader);

								if (reader.Position != (entry.Position + entry.Length))
								{
									throw new Exception($"***** Bad serialize on {item.GetType()} *****");
								}
							}
							catch (Exception e)
							{
								items.RemoveAt(i);

								failed = e;
								failedItems = true;
								failedType = item.GetType();
								failedTypeID = entry.TypeID;
								failedSerial = item.Serial;

								break;
							}
						}
					}

					reader.Close();
				}
			}

			LoadingType = null;

			if (!failedMobiles && !failedItems && File.Exists(GuildDataPath))
			{
				using (var bin = new FileStream(GuildDataPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920))
				{
					var reader = new BinaryFileReader(new BinaryReader(bin));

					for (var i = 0; i < guilds.Count; ++i)
					{
						var entry = guilds[i];
						var g = entry.Guild;

						if (g != null)
						{
							reader.Seek(entry.Position, SeekOrigin.Begin);

							try
							{
								g.Deserialize(reader);

								if (reader.Position != (entry.Position + entry.Length))
								{
									throw new Exception($"***** Bad serialize on Guild {g.Id} *****");
								}
							}
							catch (Exception e)
							{
								guilds.RemoveAt(i);

								failed = e;
								failedGuilds = true;
								failedType = typeof(BaseGuild);
								failedTypeID = g.Id;
								failedSerial = new Serial(g.Id);

								break;
							}
						}
					}

					reader.Close();
				}
			}

			if (failedItems || failedMobiles || failedGuilds)
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("An error was encountered while loading a saved object");
				Utility.PopColor();

				Console.WriteLine($" - Type: {failedType}");
				Console.WriteLine($" - Serial: {failedSerial}");

				if (!Core.Service)
				{
					Console.WriteLine("Delete the object? (y/n)");

					if (Console.ReadKey(true).Key == ConsoleKey.Y)
					{
						if (failedType != typeof(BaseGuild))
						{
							Console.WriteLine("Delete all objects of that type? (y/n)");

							if (Console.ReadKey(true).Key == ConsoleKey.Y)
							{
								if (failedMobiles)
								{
									for (var i = 0; i < mobiles.Count;)
									{
										if (mobiles[i].TypeID == failedTypeID)
										{
											mobiles.RemoveAt(i);
										}
										else
										{
											++i;
										}
									}
								}
								else if (failedItems)
								{
									for (var i = 0; i < items.Count;)
									{
										if (items[i].TypeID == failedTypeID)
										{
											items.RemoveAt(i);
										}
										else
										{
											++i;
										}
									}
								}
							}
						}

						SaveIndex(mobiles, MobileIndexPath);
						SaveIndex(items, ItemIndexPath);
						SaveIndex(guilds, GuildIndexPath);
					}

					Console.WriteLine("After pressing return an exception will be thrown and the server will terminate.");
					Console.ReadLine();
				}
				else
				{
					Utility.PushColor(ConsoleColor.Red);
					Console.WriteLine("An exception will be thrown and the server will terminate.");
					Utility.PopColor();
				}

				throw new Exception($"Load failed (items={failedItems}, mobiles={failedMobiles}, guilds={failedGuilds}, type={failedType}, serial={failedSerial})", failed);
			}

			EventSink.InvokeWorldLoad();

			Loading = false;

			ProcessSafetyQueues();

			var list = new HashSet<object>();

			list.UnionWith(Items.Values);
			list.UnionWith(Mobiles.Values);

			if (list.Count > 0)
			{
				foreach (var o in list)
				{
					if (o is Item i)
					{
						if (i.Parent == null)
							i.UpdateTotals();

						i.ClearProperties();
					}
					else if (o is Mobile m)
					{
						m.UpdateRegion();
						m.UpdateTotals();
						m.ClearProperties();
					}
				}

				list.Clear();
				list.TrimExcess();
			}

			watch.Stop();

			Utility.PushColor(ConsoleColor.Green);
			Console.WriteLine($"done ({Items.Count} items, {Mobiles.Count} mobiles) ({watch.Elapsed.TotalSeconds:F2} seconds)");
			Utility.PopColor();
		}

		public static Func<CreateGuildEventArgs, BaseGuild> CreateGuild { get; set; } = e =>
		{
			EventSink.InvokeCreateGuild(e);

			return e.Guild;
		};

		static internal void ProcessSafetyQueues()
		{
			int itemsAdded = 0, itemConflicts = 0, itemsDeleted = 0;
			int mobilesAdded = 0, mobileConflicts = 0, mobilesDeleted = 0;

			if (m_AddQueue.Count > 0)
			{
				foreach (var entry in m_AddQueue)
				{
					if (entry.Value is Item i)
					{
						if (Items.ContainsKey(i.Serial))
						{
							i.NewSerial();

							++itemConflicts;
						}

						Items[i.Serial] = i;

						++itemsAdded;
					}
					else if (entry.Value is Mobile m)
					{
						if (Mobiles.ContainsKey(m.Serial))
						{
							m.NewSerial();

							++mobileConflicts;
						}

						Mobiles[m.Serial] = m;

						++mobilesAdded;
					}
				}

				m_AddQueue.Clear();
			}

			while (m_DeleteQueue.Count > 0)
			{
				var entity = m_DeleteQueue.Dequeue();

				if (entity != null)
				{
					entity.Delete();

					if (entity is Item)
						++itemsDeleted;
					else if (entity is Mobile)
						++mobilesDeleted;
				}
			}

			if (itemsAdded > 0 || mobilesAdded > 0)
				Console.WriteLine("Added   ({0:#,0} items, {1:#,0} mobiles)", itemsAdded, mobilesAdded);

			if (itemsDeleted > 0 || mobilesDeleted > 0)
				Console.WriteLine("Deleted ({0:#,0} items, {1:#,0} mobiles)", itemsDeleted, mobilesDeleted);

			if (itemConflicts > 0 || mobileConflicts > 0)
				Console.WriteLine("Fixed   ({0:#,0} items, {1:#,0} mobiles)", itemConflicts, mobileConflicts);
		}

		private static void SaveIndex<T>(List<T> list, string path) where T : IEntityEntry
		{
			if (!Directory.Exists("Saves/Mobiles/"))
			{
				Directory.CreateDirectory("Saves/Mobiles/");
			}

			if (!Directory.Exists("Saves/Items/"))
			{
				Directory.CreateDirectory("Saves/Items/");
			}

			if (!Directory.Exists("Saves/Guilds/"))
			{
				Directory.CreateDirectory("Saves/Guilds/");
			}

			using (var idx = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				var idxWriter = new BinaryWriter(idx);

				idxWriter.Write(list.Count);

				for (var i = 0; i < list.Count; ++i)
				{
					var e = list[i];

					idxWriter.Write(e.TypeID);
					idxWriter.Write(e.Serial);
					idxWriter.Write(e.Position);
					idxWriter.Write(e.Length);
				}

				idxWriter.Close();
			}
		}

		static internal int m_Saves;

		public static void Save()
		{
			Save(true, false);
		}

		public static void Save(bool message, bool permitBackgroundWrite)
		{
			if (Saving)
			{
				return;
			}

			++m_Saves;

			NetState.FlushAll();
			NetState.Pause();

			WaitForWriteCompletion(); //Blocks Save until current disk flush is done.

			Saving = true;

			m_DiskWriteHandle.Reset();

			if (message)
			{
				Broadcast(0x35, false, AccessLevel.Player, "The world is saving, please wait.");
			}

			var strategy = SaveStrategy.Acquire();

			Console.WriteLine($"Core: Using {strategy.Name.ToLowerInvariant()} save strategy");

			Console.WriteLine("World: Saving...");

			var watch = Stopwatch.StartNew();

			if (!Directory.Exists("Saves/Mobiles/"))
			{
				Directory.CreateDirectory("Saves/Mobiles/");
			}

			if (!Directory.Exists("Saves/Items/"))
			{
				Directory.CreateDirectory("Saves/Items/");
			}

			if (!Directory.Exists("Saves/Guilds/"))
			{
				Directory.CreateDirectory("Saves/Guilds/");
			}

			try
			{
				EventSink.InvokeBeforeWorldSave(new BeforeWorldSaveEventArgs());
			}
			catch (Exception e)
			{
				throw new Exception("FATAL: Exception in EventSink.BeforeWorldSave", e);
			}

			if (m_Metrics)
			{
				using (var metrics = new SaveMetrics())
				{
					strategy.Save(metrics, permitBackgroundWrite);
				}
			}
			else
			{
				strategy.Save(null, permitBackgroundWrite);
			}

			try
			{
				EventSink.InvokeWorldSave(new WorldSaveEventArgs(message));
			}
			catch (Exception e)
			{
				throw new Exception("FATAL: Exception in EventSink.WorldSave", e);
			}

			watch.Stop();

			Saving = false;

			if (!permitBackgroundWrite)
			{
				NotifyDiskWriteComplete();
				//Sets the DiskWriteHandle.  If we allow background writes, we leave this upto the individual save strategies.
			}

			strategy.ProcessDecay();

			Console.WriteLine($"Save finished in {watch.Elapsed.TotalSeconds:F2} seconds.");

			if (message)
			{
				Broadcast(0x35, false, AccessLevel.Player, $"World save done in {watch.Elapsed.TotalSeconds:F1} seconds.");
			}

			NetState.Resume();

			try
			{
				EventSink.InvokeAfterWorldSave(new AfterWorldSaveEventArgs());
			}
			catch (Exception e)
			{
				throw new Exception("FATAL: Exception in EventSink.AfterWorldSave", e);
			}

			LastSave = DateTime.UtcNow;
		}

		static internal List<Type> m_ItemTypes = new List<Type>();
		static internal List<Type> m_MobileTypes = new List<Type>();

		public static IEntity FindEntity(Serial serial)
		{
			if (serial.IsItem)
			{
				return FindItem(serial);
			}
			else if (serial.IsMobile)
			{
				return FindMobile(serial);
			}

			return null;
		}

		public static Mobile FindMobile(Serial serial)
		{
			if (!serial.IsMobile)
				return null;

			if (Mobiles.TryGetValue(serial, out var mob))
				return mob;

			//if (!Volatile)
			//	return null;

			if (m_AddQueue.TryGetValue(serial, out var entity))
				return entity as Mobile;

			return null;
		}

		public static void AddMobile(Mobile m)
		{
			if (Volatile)
				m_AddQueue[m.Serial] = m;
			else
				Mobiles[m.Serial] = m;
		}

		public static Item FindItem(Serial serial)
		{
			if (!serial.IsItem)
				return null;

			if (Items.TryGetValue(serial, out var item))
				return item;

			//if (!Volatile)
			//	return null;

			if (m_AddQueue.TryGetValue(serial, out var entity))
				return entity as Item;

			return null;
		}

		public static void AddItem(Item item)
		{
			if (Volatile)
				m_AddQueue[item.Serial] = item;
			else
				Items[item.Serial] = item;
		}

		public static void RemoveMobile(Mobile m)
		{
			if (!Mobiles.Remove(m.Serial))
				m_AddQueue.Remove(m.Serial);
		}

		public static void RemoveItem(Item item)
		{
			if (!Items.Remove(item.Serial))
				m_AddQueue.Remove(item.Serial);
		}
	}
}
