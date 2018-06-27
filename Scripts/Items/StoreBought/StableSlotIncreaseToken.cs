using System;
using Server;
using Server.Mobiles;
using Server.Accounting;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class StableSlotIncreaseToken : Item, IAccountRestricted, IPromotionalToken
    {
        public const int SlotIncrease = 3;
        public const int MaxPerChar = 21;

        public override int LabelNumber { get { return 1070997; } } // A promotional token
        public TextDefinition ItemName { get { return 1157618; } } // your Stable Slot Increase (Account-Bound)

        public Type GumpType { get { return typeof(StableSlotIncreaseToken.InternalGump); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Account { get; set; }

        [Constructable]
        public StableSlotIncreaseToken()
            : this(null)
        {
        }

        [Constructable]
        public StableSlotIncreaseToken(string account)
            : base(0x2AAA)
        {
            Account = account;

            LootType = LootType.Blessed;
            Light = LightType.Circle300;
            Weight = 5.0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, ItemName.ToString()); // Use this to redeem<br>your Stable Slot Increase (Account-Bound)
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!IsChildOf(m.Backpack))
            {
                m.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
            else
            {
                Account acct = m.Account as Account;

                if (acct == null || acct.Username != Account)
                {
                    m.SendLocalizedMessage(1157610); // You may not use this item as it is an account-bound item that is bound to a different account.
                    return;
                }

                m.CloseGump(typeof(InternalGump));
                m.SendGump(new InternalGump(this));
            }
        }

        public void OnUsed(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm != null)
            {
                if (pm.RewardStableSlots >= 21)
                {
                    pm.SendLocalizedMessage(1157612); // You cannot increase the number of stable slots on this character. You have already reached the cap of 21 additional token stable slots or 42 total stable slots.
                }
                else
                {
                    pm.RewardStableSlots += SlotIncrease;
                    pm.SendLocalizedMessage(1157611, AnimalTrainer.GetMaxStabled(from).ToString()); // You have increased your stable slot count by 3. Your total stable count is now ~1_VAL~.

                    Delete();
                }
            }
        }

        private class InternalGump : Gump
        {
            private StableSlotIncreaseToken m_Token;

            public InternalGump(StableSlotIncreaseToken token)
                : base(10, 10)
            {
                m_Token = token;

                AddPage(0);

                AddBackground(0, 0, 240, 235, 0x2422);
                AddHtmlLocalized(15, 15, 210, 175, 1157609, 0x0, true, false); // This account-bound token will increase the maximum number of creatures a character can house at the stables by 3, up to a maximum of 42 total stables slots from all sources.  A character may use a total of 7 tokens.

                AddButton(160, 195, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0);	//Okay
                AddButton(90, 195, 0xF2, 0xF1, 0, GumpButtonType.Reply, 0);	//Cancel
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID != 1)
                    return;

                Mobile from = sender.Mobile;

                if (!m_Token.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                }
                else
                {
                    m_Token.OnUsed(from);
                }
            }
        }

        public StableSlotIncreaseToken(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Account);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Account = reader.ReadString();
        }
    }
}