using System;
using Server.Accounting;
using Server.Mobiles;

namespace Server.Items
{
    public class Gold : Item
    {
        [Constructable]
        public Gold()
            : this(1)
        {
        }

        [Constructable]
        public Gold(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Gold(int amount)
            : base(0xEED)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public Gold(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return (Core.ML ? (0.02 / 3) : 0.02);
            }
        }
        public override int GetDropSound()
        {
            if (this.Amount <= 1)
                return 0x2E4;
            else if (this.Amount <= 5)
                return 0x2E5;
            else
                return 0x2E6;
        }

#if NEWPARENT
		public override void OnAdded(IEntity parent)
#else
		public override void OnAdded(object parent)
#endif
		{
			base.OnAdded(parent);

			if (!AccountGold.Enabled)
			{
				return;
			}

			Mobile owner = null;
			SecureTradeInfo tradeInfo = null;

			Container root = parent as Container;

			while (root != null && root.Parent is Container)
			{
				root = (Container)root.Parent;
			}

			parent = root ?? parent;

			if (parent is SecureTradeContainer && AccountGold.ConvertOnTrade)
			{
				var trade = (SecureTradeContainer)parent;

				if (trade.Trade.From.Container == trade)
				{
					tradeInfo = trade.Trade.From;
					owner = tradeInfo.Mobile;
				}
				else if (trade.Trade.To.Container == trade)
				{
					tradeInfo = trade.Trade.To;
					owner = tradeInfo.Mobile;
				}
			}
			else if (parent is BankBox && AccountGold.ConvertOnBank)
			{
				owner = ((BankBox)parent).Owner;
			}

			if (owner == null || owner.Account == null || !owner.Account.DepositGold(Amount))
			{
				return;
			}

			if (tradeInfo != null)
			{
				if (owner.NetState != null && !owner.NetState.NewSecureTrading)
				{
					var total = Amount / Math.Max(1.0, Account.CurrencyThreshold);
					var plat = (int)Math.Truncate(total);
					var gold = (int)((total - plat) * Account.CurrencyThreshold);

					tradeInfo.Plat += plat;
					tradeInfo.Gold += gold;
				}

				if (tradeInfo.VirtualCheck != null)
				{
					tradeInfo.VirtualCheck.UpdateTrade(tradeInfo.Mobile);
				}
			}

			owner.SendLocalizedMessage(1042763, Amount.ToString("#,0"));

			Delete();

			((Container)parent).UpdateTotals();
		}

	    public override int GetTotal(TotalType type)
        {
            int baseTotal = base.GetTotal(type);

            if (type == TotalType.Gold)
                baseTotal += this.Amount;

            return baseTotal;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        protected override void OnAmountChange(int oldValue)
        {
            int newValue = this.Amount;

            this.UpdateTotal(this, TotalType.Gold, newValue - oldValue);
        }
    }
}