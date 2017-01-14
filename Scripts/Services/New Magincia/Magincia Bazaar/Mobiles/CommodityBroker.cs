using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
	public class CommodityBroker : BaseBazaarBroker
	{
		private List<CommodityBrokerEntry> m_CommodityEntries = new List<CommodityBrokerEntry>();
		public List<CommodityBrokerEntry> CommodityEntries { get { return m_CommodityEntries; } }
		
		public static readonly int MaxEntries = 50;
		
		public CommodityBroker(MaginciaBazaarPlot plot) : base(plot)
		{
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if(from.InRange(this.Location, 4) && Plot != null)
			{
				if(Plot.Owner == from)
				{
					from.CloseGump(typeof(CommodityBrokerGump));
					from.SendGump(new CommodityBrokerGump(this, from));
				}
				else
				{
					if(m_CommodityEntries.Count > 0)
					{
						from.CloseGump(typeof(CommodityInventoryGump));
						from.SendGump(new CommodityInventoryGump(this));
					}
					else
						from.SendLocalizedMessage(1150638); // There are no commodities in this broker's inventory.
				}
			}
			else
				base.OnDoubleClick(from);
		}
		
		public override int GetWeeklyFee()
		{
			int total = 0;
			
			foreach(CommodityBrokerEntry entry in m_CommodityEntries)
			{
				if(entry.SellPricePer > 0)
					total += entry.Stock * entry.SellPricePer;
			}
			
			double perc = (double)total * .05;
			return (int)perc;
		}
		
		public bool TryAddBrokerEntry(Item item, Mobile from)
		{
            Item realItem = item;

            if (item is CommodityDeed)
                realItem = ((CommodityDeed)item).Commodity;

            Type type = realItem.GetType();
            int amount = realItem.Amount;

			if(HasType(type))
				return false;

            CommodityBrokerEntry entry = new CommodityBrokerEntry(realItem, this, amount);
            m_CommodityEntries.Add(entry);

            if(amount > 0)
                from.SendLocalizedMessage(1150220, String.Format("{0}\t#{1}\t{2}", amount.ToString(), entry.Label, Plot.ShopName == null ? "an unnamed shop" : Plot.ShopName)); // You have added ~1_QUANTITY~ units of ~2_ITEMNAME~ to the inventory of "~3_SHOPNAME~"

            item.Delete();

			return true;
		}
		
		public void RemoveEntry(Mobile from, CommodityBrokerEntry entry)
		{
			if(m_CommodityEntries.Contains(entry))
			{
                if(entry.Stock > 0)
				    WithdrawInventory(from, GetStock(entry), entry);
				m_CommodityEntries.Remove(entry);
			}
		}

        public bool HasValidEntry()
        {
            foreach (CommodityBrokerEntry entry in m_CommodityEntries)
            {
                if (entry.Stock > 0)
                    return true;
            }

            return BankBalance > 0;
        }
		
		public void AddInventory(Mobile from, Item item)
		{
			Type type = item.GetType();
			int amountToAdd = item.Amount;
			
			if(item is CommodityDeed)
			{
				type = ((CommodityDeed)item).Commodity.GetType();
				amountToAdd = ((CommodityDeed)item).Commodity.Amount;
			}
				
			foreach(CommodityBrokerEntry entry in m_CommodityEntries)
			{
				if(entry.CommodityType == type)
				{
					entry.Stock += amountToAdd;
					item.Delete();
					
                    if(from != null && Plot.Owner == from)
                        from.SendLocalizedMessage(1150220, String.Format("{0}\t#{1}\t{2}", amountToAdd.ToString(), entry.Label, Plot.ShopName == null ? "an unnamed shop" : Plot.ShopName)); // You have added ~1_QUANTITY~ units of ~2_ITEMNAME~ to the inventory of "~3_SHOPNAME~"
					break;
				}
			}
		}
		
		private bool HasType(Type type)
		{
			foreach(CommodityBrokerEntry entry in m_CommodityEntries)
			{
				if(entry.CommodityType == type)
					return true;
			}
			
			return false;
		}
		
		public int GetStock(CommodityBrokerEntry entry)
		{
			return entry.Stock;
		}
		
		public void WithdrawInventory(Mobile from, int amount, CommodityBrokerEntry entry)
		{
			if(from == null || Plot == null || entry == null || !m_CommodityEntries.Contains(entry))
				return;
				
			Container pack = from.Backpack;
			
			if(pack != null)
			{
				while(amount > 60000)
				{
					CommodityDeed deed = new CommodityDeed();
					Item item = Loot.Construct(entry.CommodityType);
					item.Amount = 60000;
					deed.SetCommodity(item);
					pack.DropItem(deed);
					amount -= 60000;
					entry.Stock -= 60000;
				}
				
				CommodityDeed deed2 = new CommodityDeed();
				Item newitem = Loot.Construct(entry.CommodityType);
                newitem.Amount = amount;
                deed2.SetCommodity(newitem);
				pack.DropItem(deed2);
				entry.Stock -= amount;
			}
			
			if(Plot != null && from == Plot.Owner)
				from.SendLocalizedMessage(1150221, String.Format("{0}\t#{1}\t{2}", amount.ToString(), entry.Label, Plot.ShopName != null ? Plot.ShopName : "a shop with no name")); // You have removed ~1_QUANTITY~ units of ~2_ITEMNAME~ from the inventory of "~3_SHOPNAME~"
		}
		
		// Called when a player BUYS the commodity from teh broker...this is fucking confusing
		public void TryBuyCommodity(Mobile from, CommodityBrokerEntry entry, int amount)
		{
			int totalCost = entry.SellPricePer * amount;
			int toDeduct = totalCost + (int)((double)totalCost * ((double)ComissionFee / 100.0));
			
			if(Banker.Withdraw(from, toDeduct))
			{
				from.SendLocalizedMessage(1150643, String.Format("{0}\t#{1}", amount.ToString("###,###,###"), entry.Label)); // A commodity deed worth ~1_AMOUNT~ ~2_ITEM~ has been placed in your backpack.
				WithdrawInventory(from, amount, entry);
                BankBalance += totalCost;
			}
			else
			{
				from.SendLocalizedMessage(1150252); // You do not have the funds needed to make this trade available in your bank box. Brokers are only able to transfer funds from your bank box. Please deposit the necessary funds into your bank box and try again.
			}
		}
		
		// Called when a player SELLs the commodity from teh broker...this is fucking confusing
		public void TrySellCommodity(Mobile from, CommodityBrokerEntry entry, int amount)
		{
			int totalCost = entry.BuyPricePer * amount;
			Type type = entry.CommodityType;
			
			if(BankBalance < totalCost)
			{
				//No message, this should have already been handled elsewhere
			}
			else if(from.Backpack != null)
			{
				int total = amount;
				int typeAmount = from.Backpack.GetAmount(type);
				int commodityAmount = GetCommodityType(from.Backpack, type);
				
				if(typeAmount + commodityAmount < total)
					from.SendLocalizedMessage(1150667); // You do not have the requested amount of that commodity (either in item or deed form) in your backpack to trade. Note that commodities cannot be traded from your bank box.
				else if(Banker.Deposit(from, totalCost))
				{
					TakeItems(from.Backpack, type, ref total);
					
					if(total > 0)
						TakeCommodities(from.Backpack, type, ref total);

                    BankBalance -= totalCost + (int)((double)totalCost * ((double)ComissionFee / 100.0));
					from.SendLocalizedMessage(1150668, String.Format("{0}\t#{1}", amount.ToString(), entry.Label)); // You have sold ~1_QUANTITY~ units of ~2_COMMODITY~ to the broker. These have been transferred from deeds and/or items in your backpack.
				}
				else
					from.SendLocalizedMessage(1150265); // Your bank box cannot hold the proceeds from this transaction.
			}
		}
		
		private void TakeCommodities(Container c, Type type, ref int amount)
		{
            if (c == null)
                return;

			Item[] items = c.FindItemsByType(typeof(CommodityDeed));
			List<Item> toSell = new List<Item>();
			
			foreach(Item item in items)
			{
                CommodityDeed commodityDeed = item as CommodityDeed;

                if (commodityDeed != null && commodityDeed.Commodity != null && commodityDeed.Commodity.GetType() == type)
				{
					Item commodity = commodityDeed.Commodity;

                    if (commodity.Amount <= amount)
					{
						toSell.Add(item);
                        amount -= commodity.Amount;
					}
					else
					{
						CommodityDeed newDeed = new CommodityDeed();
						Item newItem = Loot.Construct(type);
						newItem.Amount = amount;
						newDeed.SetCommodity(newItem);
						
						commodity.Amount -= amount;
						commodityDeed.InvalidateProperties();
						toSell.Add(newDeed);
						amount = 0;
					}
				}
			}
			
			foreach(Item item in toSell)
			{
				AddInventory(null, item);
			}
		}
		
		private void TakeItems(Container c, Type type, ref int amount)
		{
            if (c == null)
                return;

			Item[] items = c.FindItemsByType(type);
			List<Item> toSell = new List<Item>();
			
			foreach(Item item in items)
			{
				if(amount <= 0)
					break;
					
				if(item.Amount <= amount)
				{
					toSell.Add(item);
					amount -= item.Amount;
				}
				else
				{
					Item newItem = Loot.Construct(type);
					newItem.Amount = amount;
					item.Amount -= amount;
					toSell.Add(newItem);
					amount = 0;
				}
			}
			
			foreach(Item item in toSell)
			{
				AddInventory(null, item);
			}
		}

        public int GetCommodityType(Container c, Type type)
        {
            if (c == null)
                return 0;

            Item[] items = c.FindItemsByType(typeof(CommodityDeed));
            int amt = 0;

            foreach (Item item in items)
            {
                if (item is CommodityDeed && ((CommodityDeed)item).Commodity != null && ((CommodityDeed)item).Commodity.GetType() == type)
                    amt += ((CommodityDeed)item).Commodity.Amount;
            }

            return amt;
        }
		
		public int GetItemID(CommodityBrokerEntry entry)
		{
			/*Item[] items = BuyPack.FindItemsByType(typeof(CommodityDeed));
			
			foreach(Item item in items)
			{
				if(item is CommodityDeed)
				{
					CommodityDeed deed = (CommodityDeed)item;
					
					if(deed.Commodity != null && deed.Commodity.GetType() == entry.CommodityType)
						return deed.Commodity.ItemID;
				}
			}
			
			Item item = Loot.Construct(entry.CommodityType);
			int id = 0;
			
			if(item != null)
			{
				id = item.ItemID;
				item.Delete();
			}*/
			
			return entry != null ? entry.ItemID : 1;
		}
		
		public CommodityBroker(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			writer.Write(m_CommodityEntries.Count);
			foreach(CommodityBrokerEntry entry in m_CommodityEntries)
				entry.Serialize(writer);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			int count = reader.ReadInt();
			for(int i = 0; i < count; i++)
				m_CommodityEntries.Add(new CommodityBrokerEntry(reader));
		}
	}
}