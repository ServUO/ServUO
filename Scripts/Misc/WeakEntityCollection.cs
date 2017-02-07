#region Header
// **********
// ServUO - WeakEntityCollection.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace Server
{
	public static class WeakEntityCollection
	{
		public sealed class EntityCollection : List<IEntity>
		{
			public IEnumerable<Item> Items { get { return this.OfType<Item>(); } }
			public IEnumerable<Mobile> Mobiles { get { return this.OfType<Mobile>(); } }

			public EntityCollection()
				: this(0x400)
			{ }

			public EntityCollection(int capacity)
				: base(capacity)
			{ }
		}

		private static readonly string _FilePath = Path.Combine("Saves", "WeakEntityCollection", "WeakEntityCollection.bin");

		private static readonly Dictionary<string, EntityCollection> _Collections =
			new Dictionary<string, EntityCollection>(StringComparer.OrdinalIgnoreCase);

		public static void Configure()
		{
			EventSink.WorldSave += OnWorldSave;
			EventSink.WorldLoad += OnWorldLoad;
		}

		private static void OnWorldSave(WorldSaveEventArgs e)
		{
			Save();
		}

		private static void OnWorldLoad()
		{
			Load();
		}

		public static void Save()
		{
			Persistence.Serialize(
				_FilePath,
				writer =>
				{
					writer.Write(1); // Version

					writer.Write(_Collections.Count);

					foreach (var kv in _Collections)
					{
						writer.Write(kv.Key);

						kv.Value.RemoveAll(ent => ent == null || ent.Deleted);

						writer.Write(kv.Value.Count);

						foreach (var ent in kv.Value)
						{
							writer.Write(ent.Serial);
						}
					}
				});
		}

		public static void Load()
		{
			Persistence.Deserialize(
				_FilePath,
				reader =>
				{
					var version = reader.ReadInt();

					switch (version)
					{
						case 1:
						{
							var entries = reader.ReadInt();

							while (--entries >= 0)
							{
								var key = reader.ReadString();

								var ents = reader.ReadInt();

								var col = new EntityCollection(ents);

								IEntity ent;

								while (--ents >= 0)
								{
									ent = World.FindEntity(reader.ReadInt());

									if (ent != null && !ent.Deleted)
									{
										col.Add(ent);
									}
								}

								_Collections[key] = col;
							}
						}
							break;
						case 0:
						{
							var entries = reader.ReadInt();

							while (--entries >= 0)
							{
								var key = reader.ReadString();

								var items = reader.ReadStrongItemList();
								var mobiles = reader.ReadStrongMobileList();

								var col = new EntityCollection(items.Count + mobiles.Count);

								col.AddRange(items);
								col.AddRange(mobiles);

								_Collections[key] = col;
							}
						}
							break;
					}
				});
		}

		public static EntityCollection GetCollection(string name)
		{
			EntityCollection col;

			if (!_Collections.TryGetValue(name, out col) || col == null)
			{
				_Collections[name] = col = new EntityCollection();
			}

			return col;
		}

        public static bool HasCollection(string name)
        {
            return name != null && _Collections.ContainsKey(name);
        }

		public static void Add(string key, IEntity entity)
		{
			if (entity == null || entity.Deleted)
			{
				return;
			}

			var col = GetCollection(key);

			if (col != null && !col.Contains(entity))
			{
				col.Add(entity);
			}
		}

		public static bool Remove(string key, IEntity entity)
		{
			if (entity == null)
			{
				return false;
			}

			var col = GetCollection(key);

			if (col != null)
			{
				return col.Remove(entity);
			}

			return false;
		}

		public static int Clean(string key)
		{
			var removed = 0;

			var col = GetCollection(key);

			if (col != null)
			{
				var ents = col.Count;

				while (--ents >= 0)
				{
					if (ents < col.Count && col[ents].Deleted)
					{
						col.RemoveAt(ents);

						++removed;
					}
				}
			}

			return removed;
		}

		public static int Delete(string key)
		{
			var deleted = 0;

			var col = GetCollection(key);

			if (col != null)
			{
				var ents = col.Count;

				while (--ents >= 0)
				{
					if (ents < col.Count)
					{
						col[ents].Delete();

						++deleted;
					}
				}

				col.Clear();
			}

			_Collections.Remove(key);

			return deleted;
		}
	}
}