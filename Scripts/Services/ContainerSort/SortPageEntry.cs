using System.Collections.Generic;

namespace Server.Services.ContainerSort
{
	public class SortPageEntry
	{
		public string DisplayName { get; set; }
		public List<SortItemEntry> Items { get; set; }
	}
}