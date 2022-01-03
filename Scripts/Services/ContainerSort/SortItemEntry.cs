using System.Collections.Generic;

namespace Server.Services.ContainerSort
{
	public class SortItemEntry
	{
		public ItemSortType SortType { get; set; }
		public int TextVerticalOffset { get; set; }
		public List<string> DisplayNames { get; set; }
		public SortIconInfo SortIconInfo { get; set; }
	}
}