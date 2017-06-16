#region Header
// **********
// ServUO - SecureTradeContainer.cs
// **********
#endregion

#region References
using Server.Accounting;
using Server.Network;
#endregion

namespace Server.Items
{
	public class SecureTradeContainer : Container
	{
		private readonly SecureTrade m_Trade;

		public SecureTrade Trade { get { return m_Trade; } }

		public SecureTradeContainer(SecureTrade trade)
			: base(0x1E5E)
		{
			m_Trade = trade;
			Movable = false;

            Layer = Layer.SecureTrade;
		}

		public SecureTradeContainer(Serial serial)
			: base(serial)
		{ }

		public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
		{
			if (item == Trade.From.VirtualCheck || item == Trade.To.VirtualCheck)
			{
				return true;
			}

			var to = Trade.From.Container != this ? Trade.From.Mobile : Trade.To.Mobile;

			return m.CheckTrade(to, item, this, message, checkItems, plusItems, plusWeight);
		}

		public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
		{
			reject = LRReason.CannotLift;
			return false;
		}

		public override bool IsAccessibleTo(Mobile check)
		{
			if (!IsChildOf(check) || m_Trade == null || !m_Trade.Valid)
			{
				return false;
			}

			return base.IsAccessibleTo(check);
		}

		public override void OnItemAdded(Item item)
		{
			if (!(item is VirtualCheck))
			{
				ClearChecks();
			}
		}

		public override void OnItemRemoved(Item item)
		{
			if (!(item is VirtualCheck))
			{
				ClearChecks();
			}
		}

		public override void OnSubItemAdded(Item item)
		{
			if (!(item is VirtualCheck))
			{
				ClearChecks();
			}
		}

		public override void OnSubItemRemoved(Item item)
		{
			if (!(item is VirtualCheck))
			{
				ClearChecks();
			}
		}

		public void ClearChecks()
		{
			if (m_Trade != null)
			{
				if (m_Trade.From != null && !m_Trade.From.IsDisposed)
				{
					m_Trade.From.Accepted = false;
				}

				if (m_Trade.To != null && !m_Trade.To.IsDisposed)
				{
					m_Trade.To.Accepted = false;
				}

				m_Trade.Update();
			}
		}

		public override bool IsChildVisibleTo(Mobile m, Item child)
		{
			if (child is VirtualCheck)
			{
				return AccountGold.Enabled && (m.NetState == null || !m.NetState.NewSecureTrading);
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