#region References
using Server.Accounting;
#endregion

namespace Server.Items
{
    public class BankCheck : Item
    {
        private int m_Worth;

        public BankCheck(Serial serial)
            : base(serial)
        { }

        [Constructable]
        public BankCheck(int worth)
            : base(0x14F0)
        {
            Weight = 1.0;
            Hue = 0x34;
            LootType = LootType.Blessed;

            m_Worth = worth;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Worth
        {
            get { return m_Worth; }
            set
            {
                m_Worth = value;
                InvalidateProperties();
            }
        }

        public override bool DisplayLootType => true;

        public override int LabelNumber => 1041361;  // A bank check

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Worth);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Worth = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060738, m_Worth.ToString("#,0")); // value: ~1_val~
		}

		public override void OnAdded(IEntity parent)
		{
			base.OnAdded(parent);

			Gold.CheckConvertToBank(this);
		}

		protected override void OnTreeParentChanged(Item sender, IEntity oldParent)
		{
			base.OnTreeParentChanged(sender, oldParent);

			Gold.CheckConvertToBank(this);
		}

		public override void OnDoubleClick(Mobile from)
        {
            // This probably isn't OSI accurate, but we can't just make the quests redundant.
            // Double-clicking the BankCheck in your pack will now credit your account.

            Container box = AccountGold.Enabled ? from.Backpack : from.FindBankNoCreate();

            if (box == null || !IsChildOf(box))
            {
                from.SendLocalizedMessage(AccountGold.Enabled ? 1080058 : 1047026);
                // This must be in your backpack to use it. : That must be in your bank box to use it.
                return;
            }

            Delete();

            int deposited = 0;
            int toAdd = m_Worth;

            if (AccountGold.Enabled && from.Account != null && from.Account.DepositGold(toAdd))
            {
                deposited = toAdd;
                toAdd = 0;
            }

            if (toAdd > 0)
            {
                Gold gold;

                while (toAdd > 60000)
                {
                    gold = new Gold(60000);

                    if (box.TryDropItem(from, gold, false))
                    {
                        toAdd -= 60000;
                        deposited += 60000;
                    }
                    else
                    {
                        gold.Delete();

                        from.AddToBackpack(new BankCheck(toAdd));
                        toAdd = 0;

                        break;
                    }
                }

                if (toAdd > 0)
                {
                    gold = new Gold(toAdd);

                    if (box.TryDropItem(from, gold, false))
                    {
                        deposited += toAdd;
                    }
                    else
                    {
                        gold.Delete();

                        from.AddToBackpack(new BankCheck(toAdd));
                    }
                }
            }

            // Gold was deposited in your account:
            from.SendLocalizedMessage(1042672, true, deposited.ToString("#,0"));
        }
    }
}
