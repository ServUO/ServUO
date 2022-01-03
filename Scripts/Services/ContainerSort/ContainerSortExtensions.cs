using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Multis;

namespace Server.Services.ContainerSort
{
	public static class ContainerSortExtensions
	{
		public static List<ItemSortType> GetItemSortTypes(this Container container)
		{
			return new List<ItemSortType>(container.SortItems.Select(i => (ItemSortType)i));
		}

		public static void SetItemSortTypes(this Container container, List<ItemSortType> itemSortTypes)
		{
			container.SortItems = itemSortTypes.Select(i => (int)i).ToList();
		}

		public static bool ContainsSortItemType(this Container container, ItemSortType itemSortType)
		{
			return container.GetItemSortTypes().Contains(itemSortType);
		}

		public static void RemoveSortItemType(this Container container, ItemSortType itemSortType)
		{
			if (container.ContainsSortItemType(itemSortType))
			{
				container.SetItemSortTypes(container.GetItemSortTypes().FindAll(i => i != itemSortType));
			}
		}

		public static void AddSortItemType(this Container container, ItemSortType itemSortType)
		{
			var items = container.GetItemSortTypes();

			if (!items.Contains(itemSortType))
			{
				items.Add(itemSortType);

				container.SetItemSortTypes(items);
			}
		}

		public static bool OwnsContainer(this Mobile mobile, Container container)
		{
			if (container.Parent == mobile) /* This works for bank boxes and player backpacks */
			{
				return true;
			}
			else if (container.Parent is Container) /* This container is inside another container so check parent for ownership */
			{
				return mobile.OwnsContainer(container.Parent as Container);
			}
			//The following is checking if the container is in a house owned by the player.
			else if (container.Parent == null) /* container is on the ground */
			{
				var houseOfMobile = BaseHouse.FindHouseAt(mobile);
				var houseOfContainer = BaseHouse.FindHouseAt(container);

				//If the player and container are both in the same house
				if (houseOfMobile != null && houseOfContainer != null && houseOfContainer == houseOfMobile)
				{
					var owners = new List<Mobile>();

					if (houseOfMobile.Owner != null)
					{
						owners.Add(houseOfMobile.Owner);
					}

					owners.AddRange(houseOfMobile.CoOwners ?? new List<Mobile>());

					return owners.Contains(mobile);
				}
			}

			return false;
		}

		public static List<ItemSortType> GetAllSortItemTypesIncludingSubcontainers(this Container container)
		{
			var totalItemTypes = new List<ItemSortType>(container.GetItemSortTypes());

			foreach(var item in container.Items)
			{
				if(item is Container)
				{
					totalItemTypes.AddRange((item as Container).GetAllSortItemTypesIncludingSubcontainers());
				}
			}

			return totalItemTypes.Distinct().ToList();
		}
	}
}