using System;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Engines.Points;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.CityLoyalty
{
	public class TradeOrderCrate : Container
	{
        [CommandProperty(AccessLevel.GameMaster)]
		public TradeEntry Entry { get; set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner { get; set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Fulfilled
		{
			get
			{
				if(Entry == null)
					return false;

                foreach (var details in Entry.Details)
                {
                    if (GetAmount(details.ItemType) < details.Amount)
                        return false;
                }

                return true;
			}
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expires { get; set; }

        public bool Expired { get { return Expires < DateTime.UtcNow; } }

        public TradeOrderCrate(Mobile from, TradeEntry entry)
            : base(Utility.Random(0xE3C, 4))
		{
			Owner = from;
			Entry = entry;

            Expires = DateTime.UtcNow + TimeSpan.FromHours(CityTradeSystem.CrateDuration);
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

            list.Add(1151737, String.Format("#{0}", CityLoyaltySystem.CityLocalization(Entry.Destination))); // Destination City: ~1_city~
            list.Add(1076255); // NO-TRADE
            list.Add(1072210, "75"); // Weight reduction: ~1_PERCENTAGE~%

            if (Entry.Details != null)
            {
                for (int i = 0; i < Entry.Details.Count; i++)
                {
                    if(Utility.ToInt32(Entry.Details[i].Name) > 0)
                        list.Add(1116453 + i, String.Format("#{0}\t{1}\t{2}", Entry.Details[i].Name, GetAmount(Entry.Details[i].ItemType), Entry.Details[i].Amount)); // ~1_val~: ~2_val~/~3_val~
                    else
                        list.Add(1116453 + i, String.Format("{0}\t{1}\t{2}", Entry.Details[i].Name, GetAmount(Entry.Details[i].ItemType), Entry.Details[i].Amount)); // ~1_val~: ~2_val~/~3_val~
                }
            }

            if (!Expired)
            {
                int hours = (int)Math.Max(1, (Expires - DateTime.UtcNow).TotalHours);
                list.Add(1153090, hours.ToString()); // Lifespan: ~1_val~ hours
            }
		}

        public override void Delete()
        {
            CityLoyaltySystem.CityTrading.RemoveCrate(Owner, this);

            base.Delete();
        }
		
		public override bool TryDropItem(Mobile from, Item item, bool message)
		{
			if(Entry == null)
				return false;
				
			if(TryAddItem(from, item, message))
				return base.TryDropItem(from, item, message);
				
			return false;
		}
		
		public bool TryAddItem(Mobile from, Item item, bool message = true)
		{
            bool canAdd = false;

            foreach (var details in Entry.Details)
            {
                if (item.GetType() == details.ItemType)
                {
                    int hasAmount = GetAmount(item.GetType());

                    if (hasAmount + item.Amount > details.Amount)
                    {
                        if (message)
                            from.SendLocalizedMessage(1151726); // You are trying to add too many of this item to the trade order. Only add the required quantity
                        
                        break;
                    }
                    else
                    {
                        canAdd = true;
                        break;
                    }
                }
            }

            if (!canAdd && message)
                from.SendLocalizedMessage(1151725); // This trade order does not require this item.

            return canAdd;
		}

        public override int GetTotal(TotalType type)
        {
            if (type == TotalType.Weight)
            {
                int weight = base.GetTotal(type);

                if(weight > 0)
                    return (int)Math.Max(1, (base.GetTotal(type) * .25));
            }

            return base.GetTotal(type);
        }
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries(from, list);
			
			if(IsChildOf(from.Backpack))
			{
				list.Add(new FillFromPackEntry(this, from));
				list.Add(new CancelOrderEntry(this, from));
			}
		}
		
		private class FillFromPackEntry : ContextMenuEntry
		{
			public TradeOrderCrate Crate { get; private set; }
			public Mobile Player { get; private set; }
			
			public FillFromPackEntry(TradeOrderCrate crate, Mobile player) : base(1154908, 3) // Fill from pack
			{
				Crate = crate; 
				Player = player;
			}
			
			public override void OnClick()
			{
				if(Crate.IsChildOf(Player.Backpack) && !Crate.Deleted && Crate.Entry != null)
				{
                    foreach (TradeEntry.TradeDetails detail in Crate.Entry.Details)
                    {
                        Item[] items = Player.Backpack.FindItemsByType(detail.ItemType);

                        foreach (Item item in items)
                        {
                            if (item.Amount == 1 && Crate.TryAddItem(Player, item, false))
                                Crate.DropItem(item);
                        }
                    }
				}
			}
		}
		
		private class CancelOrderEntry : ContextMenuEntry
		{
			public TradeOrderCrate Crate { get; private set; }
			public Mobile Player { get; private set; }
			
			public CancelOrderEntry(TradeOrderCrate crate, Mobile player) : base(1151727, 3) // cancel trade order
			{
				Crate = crate; 
				Player = player;
			}
			
			public override void OnClick()
			{
				//Cancel Trade Order
				//Are you sure you wish to cancel this trade order?  All contents of the trade crate will be placed in your backpack.
                Player.SendGump(new WarningGump(1151727, 30720, 1151728, 32512, 300, 200, (m, ok, obj) =>
                {
                    if (ok)
                    {
                        TradeOrderCrate crate = obj as TradeOrderCrate;

                        if (crate != null && crate.IsChildOf(Player.Backpack))
                        {
                            CityTradeSystem.CancelTradeOrder(Player, crate);
                        }
                    }
                }, Crate, true));
			}
		}

        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            from.SendLocalizedMessage(1076254); // That item cannot be dropped.
            return false;
        }

        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (target == from.Backpack)
                return base.OnDroppedInto(from, target, p);

            from.SendLocalizedMessage(1076254); // That item cannot be dropped.
            return false;
        }
 
        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            from.SendLocalizedMessage(1076254); // That item cannot be dropped.
            return false;
        }
		
		public override bool OnDroppedToWorld(Mobile from, Point3D point)
        {
            from.SendLocalizedMessage(1076254); // That item cannot be dropped.
            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            from.SendLocalizedMessage(1076256); // That item cannot be traded.
            return false;
        }
		
		public TradeOrderCrate(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);

            writer.Write(Owner);
            writer.Write(Expires);

            Entry.Serialize(writer);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();

            Owner = reader.ReadMobile();
            Expires = reader.ReadDateTime();

            Entry = new TradeEntry(reader);
		}
	}
}
