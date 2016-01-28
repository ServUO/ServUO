using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Server
{
	public class WeakEntityCollection
	{
		public class EntityCollection
		{
			public List<Item> Items;
			public List<Mobile> Mobiles;
			public EntityCollection()
			{
				Items = new List<Item>();
				Mobiles = new List<Mobile>();
			}
		}
		private static string FilePath = Path.Combine("Saves", "WeakEntityCollection", "WeakEntityCollection.bin");
		private static Dictionary<string, EntityCollection> m_Collections = new Dictionary<string, EntityCollection>();
		public static void Configure()
		{
			EventSink.WorldSave += EventSink_WorldSave;
			EventSink.WorldLoad += EventSink_WorldLoad;
		}
		public static void EventSink_WorldSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
				FilePath,
				writer =>
				{
					writer.Write(0); // Version

					writer.Write(m_Collections.Count);
					foreach(string key in m_Collections.Keys)
					{
						EntityCollection col = m_Collections[key];
						writer.Write(key);
						col.Items = CleanList(col.Items);
						writer.WriteItemList(col.Items);
						col.Mobiles = CleanList(col.Mobiles);
						writer.WriteMobileList(col.Mobiles);
					}
				});
		}
		public static void EventSink_WorldLoad()
		{
			Persistence.Deserialize(
				FilePath,
				reader =>
				{
					int version = reader.ReadInt();
					switch(version)
					{
						case 0:
							int entries = reader.ReadInt();
							for(int i = 0; i < entries; ++i)
							{
								string key = reader.ReadString();
								EntityCollection col = new EntityCollection();
								col.Items = reader.ReadItemList().Cast<Item>().ToList<Item>();
								col.Mobiles = reader.ReadMobileList().Cast<Mobile>().ToList<Mobile>();
								m_Collections.Add(key, col);
							}
							break;
					}
				});
		}
		private static List<Item> CleanList(List<Item> list)
		{
			List<Item> ret = new List<Item>();

			foreach (Item item in list)
			{
				if (!item.Deleted)
					ret.Add(item);
			}

			return ret;
		}
		private static List<Mobile> CleanList(List<Mobile> list)
		{
			List<Mobile> ret = new List<Mobile>();

			foreach (Mobile mob in list)
			{
				if (!mob.Deleted)
					ret.Add(mob);
			}

			return ret;
		}
		public static EntityCollection GetCollection(string name)
		{
			EntityCollection ret;
			if (m_Collections.TryGetValue(name, out ret))
				return ret;
			ret = new EntityCollection();
			m_Collections.Add(name, ret);
			return ret;
		}
		public static void Add(string key, Item item)
		{
			EntityCollection col = GetCollection(key);
			col.Items.Add(item);
		}
		public static void Add(string key, Mobile mob)
		{
			EntityCollection col = GetCollection(key);
			col.Mobiles.Add(mob);
		}
		public static void DeleteEntities(string key)
		{
			EntityCollection col = GetCollection(key);

			foreach(Item item in col.Items)
			{
				if (!item.Deleted)
					item.Delete();
			}

			foreach(Mobile mob in col.Mobiles)
			{
				if(!mob.Deleted)
					mob.Delete();
			}

			m_Collections.Remove(key);
		}
	}
}
