using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.AccountVault
{
    public static class VaultTimer
    {
        private static Timer _Timer;

        public static void Initialize()
        {
            StartTimer();
        }

        public static void OnTick()
        {
            foreach (var vault in AccountVault.Vaults.Where(v => v.Account != null))
            {
                vault.OnTick();
            }
        }

        public static void StartTimer()
        {
            if (_Timer == null)
            {
                _Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), OnTick);
            }

            if (!_Timer.Running)
            {
                _Timer.Start();
            }
        }

        public static void StopTimer()
        {
            if (_Timer != null && _Timer.Running)
            {
                _Timer.Stop();
            }
        }
    }

    public class VaultAuctionClaimTimer : Timer
    {
        public Mobile Winner { get; set; }
        public DateTime Deadline { get; set; }
        public AccountVaultContainer Container { get; set; }

        public VaultAuctionClaimTimer(Mobile winner, AccountVaultContainer container, DateTime deadline)
            : base(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1))
        {
            Winner = winner;
            Container = container;
            Deadline = deadline;

            TryUnload();
        }

        protected override void OnTick()
        {
            if (Deadline < DateTime.UtcNow)
            {
                if (!TryUnload())
                {
                    Container.Delete();
                }
            }
            else if (TryUnload())
            {
                Container.StopTimer();
                Winner.SendMessage("Comlete");
            }
            else
            {
                Winner.SendMessage("Remaining items: {0} / {1} Stones", Container.Items.Count, Container.TotalWeight);
            }
        }

        private bool TryUnload()
        {
            var items = new List<Item>(Container.Items);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (Winner.Backpack != null)
                {
                    Winner.Backpack.TryDropItem(Winner, item, false);
                }
            }

            if (Winner.Backpack != null && Container.Items.Count == 0 && Winner.Backpack.TryDropItem(Winner, Container, false))
            {
                Container.Movable = true;
                return true;
            }

            return false;
        }
    }
}
