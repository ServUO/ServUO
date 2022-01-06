using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Network;
using Server.Services.ContainerSort;

namespace Server.Gumps
{
	public class ContainerSortSettingsGump : Gump
	{
		private readonly Mobile m_Mobile;
		private readonly Container m_Container;
		private readonly ItemSortType m_InitialItem;

		private static readonly int GumpWidth = 750;
		private static readonly int GumpHeight = 550;
		private static readonly int XDistanceBetweenButtons = 100;
		private static readonly int YDistanceBetweenButtons = 75;
		private static readonly int NumberOfButtonColumns = 5;

		public ContainerSortSettingsGump(Mobile mobile, Container container, int item = 0)
			: base(50, 50)
		{
			m_Mobile = mobile;
			m_Container = container;
			m_InitialItem = (ItemSortType)item;

			AddPage(0);

			AddBackground(0, 0, GumpWidth, GumpHeight, 0x13BE);
			AddImageTiled(10, 10, GumpWidth - 20, 20, 0xA40);
			AddImageTiled(10, 40, 200, GumpHeight - 80, 0xA40);
			AddImageTiled(220, 40, GumpWidth - 230, GumpHeight - 80, 0xA40);

			AddAlphaRegion(10, 10, GumpWidth - 20, GumpHeight - 20);


			AddHtml(14, 12, GumpWidth - 20, 20, "<BASEFONT COLOR=#FFFFFF><center>Container Filter Settings Menu</center></BASEFONT>", false, false);

			AddButton(20, GumpHeight - 35, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
			AddHtml(55, GumpHeight - 33, 440, 20, "<BASEFONT COLOR=#FFFFFF>Finished</BASEFONT>", false, false);
			AddHtml(10, 44, 200, 20, "<BASEFONT COLOR=#FFFFFF><center>CATEGORIES</center></BASEFONT>", false, false);

			for (int i = 0; i < SortItemMap.Map.Count; i++)
			{
				var categoryEntry = SortItemMap.Map.Select(kvp => kvp.Key).ToList()[i];

				int height = i * 26 + 68;

				AddButton(20, height, 0xFA5, 0xFA7, 0, GumpButtonType.Page, ((int)categoryEntry.CategoryType) + 1);
				AddHtml(55, height + 2, 440, 20, $"<BASEFONT COLOR=#FFFFFF>{ categoryEntry.DisplayName }</BASEFONT>", false, false);
			}

			AddPage(1);

			PopulateInitalPage();

			foreach (var kvp in SortItemMap.Map)
			{
				PopulateCategoryInfo(kvp.Value, kvp.Key.CategoryType);
			}
		}

		public void PopulateInitalPage()
		{
			if (m_InitialItem != ItemSortType.Unknown)
			{
				int? page = GetPageNumberBySortItemType(m_InitialItem);

				if (page.HasValue)
				{
					foreach (var kvp in SortItemMap.Map)
					{
						for (int pageIndex = 0; pageIndex < kvp.Value.Count; pageIndex++)
						{
							int pageNumber = GetPageNumber(pageIndex, kvp.Key.CategoryType);

							if (pageNumber == page.Value)
							{
								if (pageIndex < (kvp.Value.Count - 1))
								{
									AddButton(GumpWidth - 165, GumpHeight - 35, 0xFA5, 0xFA7, 0, GumpButtonType.Page, pageNumber + 100);
									AddHtml(GumpWidth - 130, GumpHeight - 33, 100, 20, $"<BASEFONT COLOR=#FFFFFF>To {kvp.Value[pageIndex + 1].DisplayName}</BASEFONT>", false, false);
								}

								if (pageIndex > 0)
								{
									AddButton(GumpWidth - 315, GumpHeight - 35, 0xFAE, 0xFA7, 0, GumpButtonType.Page, pageNumber - 100);
									AddHtml(GumpWidth - 280, GumpHeight - 33, 100, 20, $"<BASEFONT COLOR=#FFFFFF>To {kvp.Value[pageIndex - 1].DisplayName}</BASEFONT>", false, false);
								}

								PopulatePage(kvp.Value[pageIndex].Items);

								return;
							}
						}
					}
				}
			}
		}

		public void PopulateCategoryInfo(List<SortPageEntry> pageEntries, ItemSortCategoryType categoryType)
		{
			for(int pageIndex = 0; pageIndex < pageEntries.Count; pageIndex++)
			{
				int pageNumber = GetPageNumber(pageIndex, categoryType);

				AddPage(pageNumber);

				if (pageIndex < (pageEntries.Count - 1))
				{
					AddButton(GumpWidth - 165, GumpHeight - 35, 0xFA5, 0xFA7, 0, GumpButtonType.Page, pageNumber + 100);
					AddHtml(GumpWidth - 130, GumpHeight - 33, 100, 20, $"<BASEFONT COLOR=#FFFFFF>To {pageEntries[pageIndex + 1].DisplayName}</BASEFONT>", false, false);
				}

				if(pageIndex > 0)
				{
					AddButton(GumpWidth - 315, GumpHeight - 35, 0xFAE, 0xFA7, 0, GumpButtonType.Page, pageNumber - 100);
					AddHtml(GumpWidth - 280, GumpHeight - 33, 100, 20, $"<BASEFONT COLOR=#FFFFFF>To {pageEntries[pageIndex - 1].DisplayName}</BASEFONT>", false, false);
				}

				PopulatePage(pageEntries[pageIndex].Items);
			}
		}

		public void PopulatePage(List<SortItemEntry> items)
		{
			int row = 0;
			int column = 0;			

			for (int i = 0; i < items.Count; i++)
			{
				var entry = items[i];

				if (column >= NumberOfButtonColumns)
				{
					column = 0;
					row++;
				}

				int x = column * XDistanceBetweenButtons + 240;

				int y = row * YDistanceBetweenButtons + 58;

				var artId = entry.SortIconInfo?.ArtId ?? 0;
				var horizontalOffset = entry.SortIconInfo?.HorizontalOffset ?? 0;
				var verticalOffset = entry.SortIconInfo?.VerticalOffset ?? 0;

				bool isSelected = IsItemSelected(entry.SortType);

				int verticalTextOffset = 38 - entry.TextVerticalOffset;

				AddImageTiledButton(x, y, isSelected ? 0x919 : 0x918, isSelected ? 0x918 : 0x919, (int)entry.SortType, GumpButtonType.Reply, 0, artId, entry.SortIconInfo?.Hue ?? 0x0, horizontalOffset, verticalOffset);

				for(int j = 0; j < entry.DisplayNames.Count; j++)
				{
					int offset = j * 15 + verticalTextOffset;

					AddHtml(x, y + offset, 82, 20, $"<BASEFONT COLOR=#FFFFFF><center>{ entry.DisplayNames[j] }</center></BASEFONT>", false, false);
				}				

				column++;
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			var sortType = (ItemSortType)info.ButtonID;
			int? pageNumber = GetPageNumberBySortItemType(sortType);

			if (pageNumber.HasValue)
			{
				if (IsItemSelected(sortType))
				{
					RemoveItemSelected(sortType);
				}
				else
				{
					AddItemSelected(sortType);
				}

				m_Mobile.SendGump(new ContainerSortSettingsGump(m_Mobile, m_Container, info.ButtonID));
			}
		}

		private int? GetPageNumberBySortItemType(ItemSortType itemSortType)
		{
			foreach(var mapItem in SortItemMap.Map)
			{
				for(int pageIndex = 0; pageIndex < mapItem.Value.Count; pageIndex++)
				{
					if (mapItem.Value[pageIndex].Items.Any(i => i.SortType == itemSortType))
					{
						return GetPageNumber(pageIndex, mapItem.Key.CategoryType);
					}
				}
			}

			return null;
		}

		private int GetPageNumber(int indexOfMapPage, ItemSortCategoryType categoryType)
		{
			return ((int)categoryType + 1) + (100 * indexOfMapPage);
		}

		private bool IsItemSelected(ItemSortType itemSortType)
		{
			if (SortItemGroups.Groups.ContainsKey(itemSortType))
			{
				var containerItemTypes = m_Container.GetItemSortTypes();
				return SortItemGroups.Groups[itemSortType].All(i => containerItemTypes.Contains(i));
			}

			return m_Container.ContainsSortItemType(itemSortType);
		}

		private void AddItemSelected(ItemSortType itemSortType)
		{
			if (SortItemGroups.Groups.ContainsKey(itemSortType))
			{
				foreach (var itemToAdd in SortItemGroups.Groups[itemSortType])
				{
					m_Container.AddSortItemType(itemToAdd);
				}
			}
			else
			{
				m_Container.AddSortItemType(itemSortType);
			}
		}

		private void RemoveItemSelected(ItemSortType itemSortType)
		{
			if (SortItemGroups.Groups.ContainsKey(itemSortType))
			{
				foreach (var itemToAdd in SortItemGroups.Groups[itemSortType])
				{
					m_Container.RemoveSortItemType(itemToAdd);
				}
			}
			else
			{
				m_Container.RemoveSortItemType(itemSortType);
			}
		}
	}
}