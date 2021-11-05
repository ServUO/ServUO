#region References
using Server.Accounting;
using Server.Network;
#endregion

namespace Server.Items
{
	public class SecureTradeContainer : Container
	{
		public SecureTrade Trade { get; }

		public SecureTradeContainer(SecureTrade trade)
			: base(0x1E5E)
		{
			Trade = trade;
			Movable = false;
			Layer = Layer.SecureTrade;
		}

		public SecureTradeContainer(Serial serial)
			: base(serial)
		{ }

		public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, bool checkWeight, int plusItems, int plusWeight)
		{
			if (item == Trade.From.VirtualCheck || item == Trade.To.VirtualCheck)
			{
				return true;
			}

			var to = Trade.From.Container != this ? Trade.From.Mobile : Trade.To.Mobile;

			return m.CheckTrade(to, item, this, message, checkItems, checkWeight, plusItems, plusWeight);
		}

		public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
		{
			reject = LRReason.CannotLift;
			return false;
		}

		public override bool IsAccessibleTo(Mobile check)
		{
			if (!IsChildOf(check) || Trade == null || !Trade.Valid)
			{
				return false;
			}

			return base.IsAccessibleTo(check);
		}

		public override void OnItemAdded(Item item)
		{
			base.OnItemAdded(item);

			UpdateItem(item);
		}

		public override void OnItemRemoved(Item item)
		{
			base.OnItemRemoved(item);

			UpdateItem(item);
		}

		public override void OnSubItemAdded(Item item)
		{
			base.OnSubItemAdded(item);

			UpdateItem(item);
		}

		public override void OnSubItemRemoved(Item item)
		{
			base.OnSubItemRemoved(item);

			UpdateItem(item);
		}

		private void UpdateItem(Item item)
		{
			if (Trade == null || item is VirtualCheck)
			{
				return;
			}

			ClearChecks();

			if (Trade.From != null && Trade.From.Mobile != null && Trade.From.Mobile.NetState != null)
			{
				item.SendInfoTo(Trade.From.Mobile.NetState);
			}

			if (Trade.To != null && Trade.To.Mobile != null && Trade.To.Mobile.NetState != null)
			{
				item.SendInfoTo(Trade.To.Mobile.NetState);
			}
		}

		public void ClearChecks()
		{
			if (Trade != null)
			{
				if (Trade.From != null && !Trade.From.IsDisposed)
				{
					Trade.From.Accepted = false;
				}

				if (Trade.To != null && !Trade.To.IsDisposed)
				{
					Trade.To.Accepted = false;
				}

				Trade.Update();
			}
		}

		public override bool IsChildVisibleTo(Mobile m, Item child)
		{
			if (child is VirtualCheck)
			{
				return !AccountGold.Enabled || m?.NetState?.NewSecureTrading != true;
			}

			return base.IsChildVisibleTo(m, child);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}
