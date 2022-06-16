using System.Collections.Generic;
using System.IO;

using Server.Engines.Quests;
using Server.Mobiles;

namespace Server.Services.Community_Collections
{
	public class CollectionsSystem
	{
		private static readonly Dictionary<Collection, CollectionData> m_Collections = new Dictionary<Collection, CollectionData>();

		private static readonly HashSet<BaseCollectionMobile> m_Mobiles = new HashSet<BaseCollectionMobile>();

		private static readonly string m_Path = Path.Combine(Core.BaseDirectory, "Saves", "CommunityCollections.bin");

		public static void Configure()
		{
			EventSink.WorldSave += EventSink_WorldSave;
			EventSink.WorldLoad += EventSink_WorldLoad;
		}

		public static void RegisterMobile(BaseCollectionMobile mob)
		{
			if (m_Mobiles.Add(mob) && m_Collections.TryGetValue(mob.CollectionID, out var data))
			{
				mob.SetData(data);
			}
		}

		public static void UnregisterMobile(BaseCollectionMobile mob)
		{
			m_Collections[mob.CollectionID] = mob.GetData();

			m_Mobiles.Remove(mob);
		}

		private static void EventSink_WorldSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(m_Path, writer =>
			{
				writer.WriteMobileSet(m_Mobiles, true);

				writer.Write(m_Mobiles.Count);

				foreach (var mob in m_Mobiles)
				{
					writer.Write((int)mob.CollectionID);

					var data = mob.GetData();

					data.Write(writer);

					m_Collections[mob.CollectionID] = data;
				}
			});
		}

		private static void EventSink_WorldLoad()
		{
			Persistence.Deserialize(m_Path, reader =>
			{
				var mobs = reader.ReadMobileSet<BaseCollectionMobile>();

				var count = reader.ReadInt();

				while (--count >= 0)
				{
					var collection = (Collection)reader.ReadInt();

					var data = new CollectionData();

					data.Read(reader);

					BaseCollectionMobile toRemove = null;

					foreach (var mob in mobs)
					{
						if (mob.CollectionID == collection)
						{
							mob.SetData(data);

							toRemove = mob;

							break;
						}
					}

					if (toRemove != null)
					{
						mobs.Remove(toRemove);
					}
				}

				m_Mobiles.UnionWith(mobs);
			});
		}
	}

	public class CollectionData
	{
		public Collection Collection;
		public long Points;
		public long StartTier;
		public long NextTier;
		public long DailyDecay;
		public int Tier;
		public TextDefinition DonationTitle;

		public List<HashSet<object>> Tiers { get; } = new List<HashSet<object>>();

		public void Write(GenericWriter writer)
		{
			writer.Write(0); // version

			writer.Write((int)Collection);
			writer.Write(Points);
			writer.Write(StartTier);
			writer.Write(NextTier);
			writer.Write(DailyDecay);
			writer.Write(Tier);

			QuestWriter.Object(writer, DonationTitle);

			writer.Write(Tiers.Count);

			foreach (var tier in Tiers)
			{
				writer.Write(tier.Count);

				foreach (var obj in tier)
				{
					QuestWriter.Object(writer, obj);
				}
			}
		}

		public void Read(GenericReader reader)
		{
			reader.ReadInt();

			Collection = (Collection)reader.ReadInt();
			Points = reader.ReadLong();
			StartTier = reader.ReadLong();
			NextTier = reader.ReadLong();
			DailyDecay = reader.ReadLong();
			Tier = reader.ReadInt();

			var title = QuestReader.Object(reader);

			if (title is TextDefinition def)
			{
				DonationTitle = def;
			}
			else if (title is string str)
			{
				DonationTitle = str;
			}
			else if (title is int num)
			{
				DonationTitle = num;
			}

			for (var i = reader.ReadInt(); i > 0; i--)
			{
				var list = new HashSet<object>();

				for (var j = reader.ReadInt(); j > 0; j--)
				{
					list.Add(QuestReader.Object(reader));
				}

				Tiers.Add(list);
			}
		}
	}
}
