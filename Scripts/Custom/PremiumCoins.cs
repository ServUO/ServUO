using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class PremiumCoins : Item
    {
        public static class Config
        {
            public static bool Enabled = true;
            public static int MinutesPerCoin = 10;          // +1 coin every 10 minutes
            public static bool DropToBank = true;           // true = bank, false = backpack
            public static AccessLevel MaxAccessLevel = AccessLevel.Player;
        }

        public static void Initialize()
        {
            if (!Config.Enabled)
                return;

            new CoinDistributionTimer().Start();
        }

        [Constructable]
        public PremiumCoins(int amount)
        {
            Name = "Premium Coins";
            ItemID = 0xEED;
            Hue = 1153;
            LootType = LootType.Blessed;
            Stackable = true;
            Amount = amount;
            Weight = 0;
        }

        [Constructable]
        public PremiumCoins() : this(1) { }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Game Time Reward");
        }

        public override bool DisplayWeight { get { return false; } }

        public PremiumCoins(Serial serial) : base(serial) { }

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

        private class CoinDistributionTimer : Timer
        {
            public CoinDistributionTimer()
                : base(TimeSpan.FromMinutes(Config.MinutesPerCoin), TimeSpan.FromMinutes(Config.MinutesPerCoin))
            {
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                foreach (NetState state in NetState.Instances)
                {
                    PlayerMobile m = state.Mobile as PlayerMobile;

                    if (m == null || m.Deleted)
                        continue;

                    if (m.AccessLevel > Config.MaxAccessLevel)
                        continue;

                    GiveCoins(m, 1);
                    m.SendMessage(0x35, "You have received a Premium Coin for your time online!");
                }
            }
        }

        public static void GiveCoins(PlayerMobile m, int amount)
        {
            if (m == null || m.Deleted || amount <= 0)
                return;

            Container target = Config.DropToBank && m.BankBox != null ? m.BankBox : m.Backpack;

            if (target == null)
                return;

            Item existing = target.FindItemByType(typeof(PremiumCoins), false);

            if (existing != null)
            {
                existing.Amount += amount;
            }
            else
            {
                target.DropItem(new PremiumCoins(amount));
            }
        }
    }
}
