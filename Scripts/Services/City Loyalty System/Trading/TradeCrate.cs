using System;
using System.Collections.Generic;

using Server;
using Server.ContextMenus;
using Server.Items;
using Server.Gumps;
using System.Linq;
using Server.Network;
using Server.Engines.SeasonalEvents;

namespace Server.Engines.CityLoyalty
{
	public class TradeOrderCrate : Container
	{
        public override int LabelNumber { get { return CityTradeSystem.KrampusEncounterActive ? 1123594 : base.LabelNumber; } }
        
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

        public override int DefaultMaxWeight { get { return 1000; } }

        public TradeOrderCrate(Mobile from, TradeEntry entry)
            : base(GetID())
		{
            Owner = from;
			Entry = entry;

            if (CityTradeSystem.KrampusEncounterActive)
            {
                Weight = 10.0;
                Hue = Utility.Random(100);
            }

            Expires = DateTime.UtcNow + TimeSpan.FromHours(CityTradeSystem.CrateDuration);
		}

        private static int GetID()
        {
            if (CityTradeSystem.KrampusEncounterActive)
            {
                return Utility.Random(0x46A2, 6);
            }

            return Utility.Random(0xE3C, 4);
        }

        public override int DefaultGumpID
        {
            get
            {
                switch(ItemID)
                {
                    default:
                        return base.DefaultGumpID;
                    case 0x46A2:
                        return 0x11B;
                    case 0x46A3:
                        return 0x11C;
                    case 0x46A4:
                        return 0x11D;
                    case 0x46A5:
                    case 0x46A6:
                        return 0x11E;
                }
            }
        }

        public override bool DisplaysContent { get { return false; } }

        public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

            list.Add(1151737, String.Format("#{0}", CityLoyaltySystem.CityLocalization(Entry.Destination))); // Destination City: ~1_city~
            list.Add(1076255); // NO-TRADE

            int weight = Items.Sum(x => x.Amount);

            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", Items.Count, DefaultMaxItems, weight, DefaultMaxWeight); // Contents: ~1_COUNT~/~2_MAXCOUNT items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones

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

                if (weight > 0)
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
                Player.CloseGump(typeof(CancelTradeOrderGump));
                Player.SendGump(new CancelTradeOrderGump(Crate, Player));
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

    public class CancelTradeOrderGump : Gump
    {
        public TradeOrderCrate Crate { get; private set; }
        public Mobile Player { get; private set; }

        public CancelTradeOrderGump(TradeOrderCrate crate, Mobile player)
            : base(100, 100)
        {
            Crate = crate;
            Player = player;

            AddPage(0);

            AddBackground(0, 0, 291, 93, 0x13BE);
            AddImageTiled(5, 5, 280, 60, 0xA40);

            AddHtmlLocalized(9, 9, 272, 60, 1151728, 0x7FFF, false, false); // Are you sure you wish to cancel this trade order?  All contents of the trade crate will be placed in your backpack. 

            AddButton(160, 67, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 69, 120, 20, 1006044, 0x7FFF, false, false); // OKAY

            AddButton(5, 67, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 69, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (Crate != null && Crate.IsChildOf(Player.Backpack))
                {
                    CityTradeSystem.CancelTradeOrder(Player, Crate);
                }
            }
        }
    }
}
