#region Header
// **********
// ServUO - SecureTrade.cs
// **********
#endregion

#region References
using System;

using Server.Accounting;
using Server.Items;
using Server.Network;
#endregion

namespace Server
{
	public class SecureTrade
	{
		private readonly SecureTradeInfo m_From;
		private readonly SecureTradeInfo m_To;

		private bool m_Valid;

		public SecureTrade(Mobile from, Mobile to)
		{
			m_Valid = true;

			m_From = new SecureTradeInfo(this, from, new SecureTradeContainer(this));
			m_To = new SecureTradeInfo(this, to, new SecureTradeContainer(this));

			var from6017 = (from.NetState != null && from.NetState.ContainerGridLines);
			var to6017 = (to.NetState != null && to.NetState.ContainerGridLines);

			var from704565 = (from.NetState != null && from.NetState.NewSecureTrading);
			var to704565 = (to.NetState != null && to.NetState.NewSecureTrading);

			from.Send(new MobileStatus(from, to));
			from.Send(new UpdateSecureTrade(m_From.Container, false, false));

			if (from6017)
			{
				from.Send(new SecureTradeEquip6017(m_To.Container, to));
			}
			else
			{
				from.Send(new SecureTradeEquip(m_To.Container, to));
			}

			from.Send(new UpdateSecureTrade(m_From.Container, false, false));

			if (from6017)
			{
				from.Send(new SecureTradeEquip6017(m_From.Container, from));
			}
			else
			{
				from.Send(new SecureTradeEquip(m_From.Container, from));
			}

			from.Send(new DisplaySecureTrade(to, m_From.Container, m_To.Container, to.Name));
			from.Send(new UpdateSecureTrade(m_From.Container, false, false));

			if (from.Account != null && from704565)
			{
				from.Send(
					new UpdateSecureTrade(m_From.Container, TradeFlag.UpdateLedger, from.Account.TotalGold, from.Account.TotalPlat));
			}

			to.Send(new MobileStatus(to, from));
			to.Send(new UpdateSecureTrade(m_To.Container, false, false));

			if (to6017)
			{
				to.Send(new SecureTradeEquip6017(m_From.Container, from));
			}
			else
			{
				to.Send(new SecureTradeEquip(m_From.Container, from));
			}

			to.Send(new UpdateSecureTrade(m_To.Container, false, false));

			if (to6017)
			{
				to.Send(new SecureTradeEquip6017(m_To.Container, to));
			}
			else
			{
				to.Send(new SecureTradeEquip(m_To.Container, to));
			}

			to.Send(new DisplaySecureTrade(from, m_To.Container, m_From.Container, from.Name));
			to.Send(new UpdateSecureTrade(m_To.Container, false, false));

			if (to.Account != null && to704565)
			{
				to.Send(new UpdateSecureTrade(m_To.Container, TradeFlag.UpdateLedger, to.Account.TotalGold, to.Account.TotalPlat));
			}
		}

		public SecureTradeInfo From { get { return m_From; } }
		public SecureTradeInfo To { get { return m_To; } }

		public bool Valid { get { return m_Valid; } }

		public void Cancel()
		{
			if (!m_Valid)
			{
				return;
			}

			var list = m_From.Container.Items;

			for (var i = list.Count - 1; i >= 0; --i)
			{
				if (i < list.Count)
				{
					var item = list[i];

					if (item == m_From.VirtualCheck)
					{
						continue;
					}

					item.OnSecureTrade(m_From.Mobile, m_To.Mobile, m_From.Mobile, false);

					if (!item.Deleted)
					{
						m_From.Mobile.AddToBackpack(item);
					}
				}
			}

			list = m_To.Container.Items;

			for (var i = list.Count - 1; i >= 0; --i)
			{
				if (i < list.Count)
				{
					var item = list[i];

					if (item == m_To.VirtualCheck)
					{
						continue;
					}

					item.OnSecureTrade(m_To.Mobile, m_From.Mobile, m_To.Mobile, false);

					if (!item.Deleted)
					{
						m_To.Mobile.AddToBackpack(item);
					}
				}
			}

			Close();
		}

		public void Close()
		{
			if (!m_Valid)
			{
				return;
			}

			m_From.Mobile.Send(new CloseSecureTrade(m_From.Container));
			m_To.Mobile.Send(new CloseSecureTrade(m_To.Container));

			m_Valid = false;

			var ns = m_From.Mobile.NetState;

			if (ns != null)
			{
				ns.RemoveTrade(this);
			}

			ns = m_To.Mobile.NetState;

			if (ns != null)
			{
				ns.RemoveTrade(this);
			}

			Timer.DelayCall(m_From.Dispose);
			Timer.DelayCall(m_To.Dispose);
		}

		public void UpdateFromCurrency()
		{
			UpdateCurrency(m_From, m_To);
		}

		public void UpdateToCurrency()
		{
			UpdateCurrency(m_To, m_From);
		}

		private static void UpdateCurrency(SecureTradeInfo left, SecureTradeInfo right)
		{
			if (left.Mobile.NetState != null && left.Mobile.NetState.NewSecureTrading)
			{
				var plat = left.Mobile.Account.TotalPlat;
				var gold = left.Mobile.Account.TotalGold;

				left.Mobile.Send(new UpdateSecureTrade(left.Container, TradeFlag.UpdateLedger, gold, plat));
			}

			if (right.Mobile.NetState != null && right.Mobile.NetState.NewSecureTrading)
			{
				right.Mobile.Send(new UpdateSecureTrade(right.Container, TradeFlag.UpdateGold, left.Gold, left.Plat));
			}
		}

		public void Update()
		{
			if (!m_Valid)
			{
				return;
			}

			if (!m_From.IsDisposed && m_From.Accepted && !m_To.IsDisposed && m_To.Accepted)
			{
				var list = m_From.Container.Items;

				var allowed = true;

				for (var i = list.Count - 1; allowed && i >= 0; --i)
				{
					if (i < list.Count)
					{
						var item = list[i];

						if (item == m_From.VirtualCheck)
						{
							continue;
						}

						if (!item.AllowSecureTrade(m_From.Mobile, m_To.Mobile, m_To.Mobile, true))
						{
							allowed = false;
						}
					}
				}

				list = m_To.Container.Items;

				for (var i = list.Count - 1; allowed && i >= 0; --i)
				{
					if (i < list.Count)
					{
						var item = list[i];

						if (item == m_To.VirtualCheck)
						{
							continue;
						}

						if (!item.AllowSecureTrade(m_To.Mobile, m_From.Mobile, m_From.Mobile, true))
						{
							allowed = false;
						}
					}
				}

				if (AccountGold.Enabled)
				{
					if (m_From.Mobile.Account != null)
					{
						var cur = m_From.Mobile.Account.TotalCurrency;
						var off = m_From.Plat + (m_From.Gold / Math.Max(1.0, AccountGold.CurrencyThreshold));

						if (off > cur)
						{
							allowed = false;
							m_From.Mobile.SendMessage("You do not have enough currency to complete this trade.");
						}
					}

					if (m_To.Mobile.Account != null)
					{
						var cur = m_To.Mobile.Account.TotalCurrency;
						var off = m_To.Plat + (m_To.Gold / Math.Max(1.0, AccountGold.CurrencyThreshold));

						if (off > cur)
						{
							allowed = false;
							m_To.Mobile.SendMessage("You do not have enough currency to complete this trade.");
						}
					}
				}

				if (!allowed)
				{
					m_From.Accepted = false;
					m_To.Accepted = false;

					m_From.Mobile.Send(new UpdateSecureTrade(m_From.Container, m_From.Accepted, m_To.Accepted));
					m_To.Mobile.Send(new UpdateSecureTrade(m_To.Container, m_To.Accepted, m_From.Accepted));

					return;
				}

				if (AccountGold.Enabled && m_From.Mobile.Account != null && m_To.Mobile.Account != null)
				{
					HandleAccountGoldTrade();
				}

				list = m_From.Container.Items;

				for (var i = list.Count - 1; i >= 0; --i)
				{
					if (i < list.Count)
					{
						var item = list[i];

						if (item == m_From.VirtualCheck)
						{
							continue;
						}

						item.OnSecureTrade(m_From.Mobile, m_To.Mobile, m_To.Mobile, true);

						if (!item.Deleted)
						{
							m_To.Mobile.AddToBackpack(item);
						}
					}
				}

				list = m_To.Container.Items;

				for (var i = list.Count - 1; i >= 0; --i)
				{
					if (i < list.Count)
					{
						var item = list[i];

						if (item == m_To.VirtualCheck)
						{
							continue;
						}

						item.OnSecureTrade(m_To.Mobile, m_From.Mobile, m_From.Mobile, true);

						if (!item.Deleted)
						{
							m_From.Mobile.AddToBackpack(item);
						}
					}
				}

				Close();
			}
			else if (!m_From.IsDisposed && !m_To.IsDisposed)
			{
				m_From.Mobile.Send(new UpdateSecureTrade(m_From.Container, m_From.Accepted, m_To.Accepted));
				m_To.Mobile.Send(new UpdateSecureTrade(m_To.Container, m_To.Accepted, m_From.Accepted));
			}
		}

		private void HandleAccountGoldTrade()
		{
			int fromPlatSend = 0, fromGoldSend = 0, fromPlatRecv = 0, fromGoldRecv = 0;
			int toPlatSend = 0, toGoldSend = 0, toPlatRecv = 0, toGoldRecv = 0;

			if (m_From.Plat > 0 & m_From.Mobile.Account.WithdrawPlat(m_From.Plat))
			{
				fromPlatSend = m_From.Plat;

				if (m_To.Mobile.Account.DepositPlat(m_From.Plat))
				{
					toPlatRecv = fromPlatSend;
				}
			}

			if (m_From.Gold > 0 & m_From.Mobile.Account.WithdrawGold(m_From.Gold))
			{
				fromGoldSend = m_From.Gold;

				if (m_To.Mobile.Account.DepositGold(m_From.Gold))
				{
					toGoldRecv = fromGoldSend;
				}
			}

			if (m_To.Plat > 0 & m_To.Mobile.Account.WithdrawPlat(m_To.Plat))
			{
				toPlatSend = m_To.Plat;

				if (m_From.Mobile.Account.DepositPlat(m_To.Plat))
				{
					fromPlatRecv = toPlatSend;
				}
			}

			if (m_To.Gold > 0 & m_To.Mobile.Account.WithdrawGold(m_To.Gold))
			{
				toGoldSend = m_To.Gold;

				if (m_From.Mobile.Account.DepositGold(m_To.Gold))
				{
					fromGoldRecv = toGoldSend;
				}
			}

			HandleAccountGoldTrade(m_From.Mobile, m_To.Mobile, fromPlatSend, fromGoldSend, fromPlatRecv, fromGoldRecv);
			HandleAccountGoldTrade(m_To.Mobile, m_From.Mobile, toPlatSend, toGoldSend, toPlatRecv, toGoldRecv);
		}

		private static void HandleAccountGoldTrade(
			Mobile left,
			Mobile right,
			int platSend,
			int goldSend,
			int platRecv,
			int goldRecv)
		{
			if (platSend > 0 || goldSend > 0)
			{
				if (platSend > 0 && goldSend > 0)
				{
					left.SendMessage("You traded {0:#,0} platinum and {1:#,0} gold to {2}.", platSend, goldSend, right.RawName);
				}
				else if (platSend > 0)
				{
					left.SendMessage("You traded {0:#,0} platinum to {1}.", platSend, right.RawName);
				}
				else if (goldSend > 0)
				{
					left.SendMessage("You traded {0:#,0} gold to {1}.", goldSend, right.RawName);
				}
			}

			if (platRecv > 0 || goldRecv > 0)
			{
				if (platRecv > 0 && goldRecv > 0)
				{
					left.SendMessage("You received {0:#,0} platinum and {1:#,0} gold from {2}.", platRecv, goldRecv, right.RawName);
				}
				else if (platRecv > 0)
				{
					left.SendMessage("You received {0:#,0} platinum from {1}.", platRecv, right.RawName);
				}
				else if (goldRecv > 0)
				{
					left.SendMessage("You received {0:#,0} gold from {1}.", goldRecv, right.RawName);
				}
			}
		}
	}

	public class SecureTradeInfo : IDisposable
	{
		public SecureTrade Owner { get; private set; }
		public Mobile Mobile { get; private set; }
		public SecureTradeContainer Container { get; private set; }
		public VirtualCheck VirtualCheck { get; private set; }

		public int Gold { get { return VirtualCheck.Gold; } set { VirtualCheck.Gold = value; } }
		public int Plat { get { return VirtualCheck.Plat; } set { VirtualCheck.Plat = value; } }

		public bool Accepted { get; set; }

		public bool IsDisposed { get; private set; }

		public SecureTradeInfo(SecureTrade owner, Mobile m, SecureTradeContainer c)
		{
			Owner = owner;
			Mobile = m;
			Container = c;

			Mobile.AddItem(Container);

			VirtualCheck = new VirtualCheck(0, 0);
			Container.DropItem(VirtualCheck);
		}

		public void Dispose()
		{
			VirtualCheck.Delete();
			VirtualCheck = null;

			Container.Delete();
			Container = null;

			Mobile = null;
			Owner = null;

			IsDisposed = true;
		}
	}
}