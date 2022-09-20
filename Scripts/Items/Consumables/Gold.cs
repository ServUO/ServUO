using System;

using Server.Accounting;

namespace Server.Items
{
    public class Gold : Item
    {
		public static bool CheckConvertToBank(Item goldORcheck)
		{
			if (!AccountGold.Enabled)
			{
				return false;
			}

			if (goldORcheck?.Deleted != false)
			{
				return false;
			}

			if (!(goldORcheck.Parent is Container parent))
			{
				return false;
			}

			var gold = goldORcheck as Gold;
			var check = goldORcheck as BankCheck;

			if (gold == null && check == null)
			{
				return false;
			}

			Mobile owner = null;
			SecureTradeInfo tradeInfo = null;

			var root = parent;

			while (root?.Parent is Container c)
			{
				root = c;
			}

			parent = root ?? parent;

			if (parent is SecureTradeContainer trade && AccountGold.ConvertOnTrade)
			{
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
			else if (parent is BankBox bank && AccountGold.ConvertOnBank)
			{
				owner = bank.Owner;
			}

			if (owner == null || owner.Account == null)
			{
				return false;
			}

			var amount = gold?.Amount ?? check?.Worth ?? 0;

			if (amount <= 0 || !owner.Account.DepositGold(amount))
			{
				return false;
			}

			if (tradeInfo != null)
			{
				if (owner.NetState != null && !owner.NetState.NewSecureTrading)
				{
					var total = amount / Math.Max(1.0, Account.CurrencyThreshold);
					var platVal = (int)Math.Truncate(total);
					var goldVal = (int)((total - platVal) * Account.CurrencyThreshold);

					tradeInfo.Plat += platVal;
					tradeInfo.Gold += goldVal;
				}

				if (tradeInfo.VirtualCheck != null)
				{
					tradeInfo.VirtualCheck.UpdateTrade(tradeInfo.Mobile);
				}
			}

			owner.SendLocalizedMessage(1042763, amount.ToString("#,0"));

			goldORcheck.Delete();

			if (parent is Item o)
			{
				o.UpdateTotals();
			}

			return true;
		}
		
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
            Stackable = true;
            Amount = amount;
        }

        public Gold(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight => 0.02 / 3;

        public override int GetDropSound()
        {
            if (Amount <= 1)
                return 0x2E4;
            
			if (Amount <= 5)
                return 0x2E5;
            
			return 0x2E6;
        }

		public override void OnAdded(IEntity parent)
		{
			base.OnAdded(parent);

			CheckConvertToBank(this);
		}

		protected override void OnTreeParentChanged(Item sender, IEntity oldParent)
		{
			base.OnTreeParentChanged(sender, oldParent);

			CheckConvertToBank(this);
		}

		public override int GetTotal(TotalType type)
        {
            int baseTotal = base.GetTotal(type);

            if (type == TotalType.Gold)
                baseTotal += Amount;

            return baseTotal;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
			reader.ReadInt();
        }

        protected override void OnAmountChange(int oldValue)
        {
            UpdateTotal(this, TotalType.Gold, Amount - oldValue);
        }
    }
}
