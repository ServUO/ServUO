#region References
using System.Collections.Generic;
#endregion

namespace Server.Guilds
{
	public abstract class BaseGuild : ISerializable
	{
		private readonly int m_Id;

		protected BaseGuild(int Id) //serialization ctor
		{
			m_Id = Id;
			m_GuildList.Add(m_Id, this);
			if (m_Id + 1 > m_NextID)
			{
				m_NextID = m_Id + 1;
			}
		}

		protected BaseGuild()
		{
			m_Id = m_NextID++;
			m_GuildList.Add(m_Id, this);
		}

		[CommandProperty(AccessLevel.Counselor)]
		public int Id => m_Id;

		int ISerializable.TypeReference => 0;

		int ISerializable.SerialIdentity => m_Id;

		public abstract void Deserialize(GenericReader reader);
		public abstract void Serialize(GenericWriter writer);

		public abstract string Abbreviation { get; set; }
		public abstract string Name { get; set; }
		public abstract bool Disbanded { get; }
		public abstract void OnDelete(Mobile mob);

		private static readonly Dictionary<int, BaseGuild> m_GuildList = new Dictionary<int, BaseGuild>();
		private static int m_NextID = 1;

		public static Dictionary<int, BaseGuild> List => m_GuildList;

		public static BaseGuild Find(int id)
		{

			m_GuildList.TryGetValue(id, out BaseGuild g);

			return g;
		}

		public static BaseGuild FindByName(string name)
		{
			foreach (BaseGuild g in m_GuildList.Values)
			{
				if (g.Name == name)
				{
					return g;
				}
			}

			return null;
		}

		public static BaseGuild FindByAbbrev(string abbr)
		{
			foreach (BaseGuild g in m_GuildList.Values)
			{
				if (g.Abbreviation == abbr)
				{
					return g;
				}
			}

			return null;
		}

		public static List<BaseGuild> Search(string find)
		{
			string[] words = find.ToLower().Split(' ');
			List<BaseGuild> results = new List<BaseGuild>();

			foreach (BaseGuild g in m_GuildList.Values)
			{
				bool match = true;
				string name = g.Name.ToLower();
				for (int i = 0; i < words.Length; i++)
				{
					if (name.IndexOf(words[i]) == -1)
					{
						match = false;
						break;
					}
				}

				if (match)
				{
					results.Add(g);
				}
			}

			return results;
		}

		public override string ToString()
		{
			return string.Format("0x{0:X} \"{1} [{2}]\"", m_Id, Name, Abbreviation);
		}
	}
}
