using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public class PetBroker : BaseBazaarBroker
    {
        private readonly List<PetBrokerEntry> m_BrokerEntries = new List<PetBrokerEntry>();
        public List<PetBrokerEntry> BrokerEntries => m_BrokerEntries;

        public static readonly int MaxEntries = 10;

        public PetBroker(MaginciaBazaarPlot plot) : base(plot)
        {
            FollowersMax = 500;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 4) && Plot != null)
            {
                if (Plot.Owner == from)
                {
                    from.CloseGump(typeof(PetBrokerGump));
                    from.SendGump(new PetBrokerGump(this, from));
                }
                else
                {
                    from.CloseGump(typeof(PetInventoryGump));
                    from.SendGump(new PetInventoryGump(this, from));
                }
            }
            else
                base.OnDoubleClick(from);
        }

        public bool TryAddEntry(BaseCreature bc, Mobile from, int cost)
        {
            if (bc == null || HasEntry(bc) || !bc.Alive || !bc.IsStabled)
                from.SendLocalizedMessage(1150342); // That pet is not in the stables. The pet must remain in the stables in order to be transferred to the broker's inventory.
            else if (m_BrokerEntries.Count >= MaxEntries)
                from.SendLocalizedMessage(1150631); // You cannot add more pets to this animal broker's inventory at this time, because the shop inventory is full.
            else if (!from.Stabled.Contains(bc))
                from.SendLocalizedMessage(1150344); // Transferring the pet from the stables to the animal broker's inventory failed for an unknown reason.
            else
            {
                m_BrokerEntries.Add(new PetBrokerEntry(bc, cost));
                return true;
            }

            return false;
        }

        public void RemoveEntry(PetBrokerEntry entry)
        {
            if (m_BrokerEntries.Contains(entry))
            {
                m_BrokerEntries.Remove(entry);
            }
        }

        public override bool HasValidEntry(Mobile m)
        {
            bool hasValid = false;

            foreach (PetBrokerEntry entry in m_BrokerEntries)
            {
                if (entry.Pet != null)
                {
                    if (m.Stabled.Count < AnimalTrainer.GetMaxStabled(m))
                    {
                        SendToStables(m, entry.Pet);
                    }
                    else
                    {
                        hasValid = true;
                    }
                }
            }

            return hasValid;
        }

        public bool HasEntry(BaseCreature bc)
        {
            foreach (PetBrokerEntry entry in m_BrokerEntries)
            {
                if (entry.Pet == bc)
                    return true;
            }

            return false;
        }

        public void CheckInventory()
        {
            List<PetBrokerEntry> entries = new List<PetBrokerEntry>(m_BrokerEntries);

            foreach (PetBrokerEntry entry in entries)
            {
                if (entry.Pet == null || entry.Pet.Deleted)
                    m_BrokerEntries.Remove(entry);
            }
        }

        public override int GetWeeklyFee()
        {
            int total = 0;

            foreach (PetBrokerEntry entry in m_BrokerEntries)
            {
                if (entry.SalePrice > 0)
                    total += entry.SalePrice;
            }

            double perc = total * .05;
            return (int)perc;
        }

        public int TryBuyPet(Mobile from, PetBrokerEntry entry)
        {
            if (from == null || entry == null || entry.Pet == null)
            {
                return 1150377; // Unable to complete the desired transaction at this time.
            }

            int cost = entry.SalePrice;
            int toAdd = cost - (int)(cost * (ComissionFee / 100.0));
            BaseCreature pet = entry.Pet;

            if (!m_BrokerEntries.Contains(entry) || entry.Pet == null || entry.Pet.Deleted)
            {
                return 1150377; // Unable to complete the desired transaction at this time.
            }
            else if (pet.GetControlChance(from) <= 0.0)
            {
                return 1150379; // Unable to transfer that pet to you because you have no chance at all of controlling it.
            }
            else if (from.Stabled.Count >= AnimalTrainer.GetMaxStabled(from))
            {
                return 1150376; // You do not have any available stable slots. The Animal Broker can only transfer pets to your stables. Please make a stables slot available and try again.
            }
            else if (!Banker.Withdraw(from, cost, true))
            {
                return 1150252; // You do not have the funds needed to make this trade available in your bank box. Brokers are only able to transfer funds from your bank box. Please deposit the necessary funds into your bank box and try again.
            }
            else
            {
                BankBalance += toAdd;
                pet.IsBonded = false;

                SendToStables(from, pet);

                from.SendLocalizedMessage(1150380, string.Format("{0}\t{1}", entry.TypeName, pet.Name)); // You have purchased ~1_TYPE~ named "~2_NAME~". The animal is now in the stables and you may retrieve it there.
                m_BrokerEntries.Remove(entry);
                return 0;
            }
        }

        public static void SendToStables(Mobile to, BaseCreature pet)
        {
            EndViewTimer(pet);

            pet.Blessed = false;
            pet.ControlTarget = null;
            pet.ControlOrder = OrderType.Stay;
            pet.Internalize();
            pet.SetControlMaster(null);
            pet.SummonMaster = null;
            pet.IsStabled = true;
            pet.Loyalty = MaxLoyalty;
            to.Stabled.Add(pet);
        }

        public static void SendToBrokerStables(BaseCreature pet)
        {
            if (pet is BaseMount)
                ((BaseMount)pet).Rider = null;

            pet.ControlTarget = null;
            pet.ControlOrder = OrderType.Stay;
            pet.Internalize();

            pet.SetControlMaster(null);
            pet.SummonMaster = null;

            pet.IsStabled = true;
            pet.Loyalty = MaxLoyalty;

            pet.Home = Point3D.Zero;
            pet.RangeHome = 10;
            pet.Blessed = false;

            EndViewTimer(pet);
        }

        private static readonly Dictionary<BaseCreature, Timer> m_ViewTimer = new Dictionary<BaseCreature, Timer>();

        public static void AddToViewTimer(BaseCreature bc)
        {
            if (m_ViewTimer.ContainsKey(bc))
            {
                if (m_ViewTimer[bc] != null)
                    m_ViewTimer[bc].Stop();
            }

            m_ViewTimer[bc] = new InternalTimer(bc);
            m_ViewTimer[bc].Start();
        }

        public static void EndViewTimer(BaseCreature bc)
        {
            if (m_ViewTimer.ContainsKey(bc))
            {
                if (m_ViewTimer[bc] != null)
                    m_ViewTimer[bc].Stop();

                m_ViewTimer.Remove(bc);
            }
        }

        private class InternalTimer : Timer
        {
            readonly BaseCreature m_Creature;

            public InternalTimer(BaseCreature bc) : base(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2))
            {
                m_Creature = bc;
            }

            protected override void OnTick()
            {
                SendToBrokerStables(m_Creature);
            }
        }

        public PetBroker(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_BrokerEntries.Count);
            foreach (PetBrokerEntry entry in m_BrokerEntries)
                entry.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
                m_BrokerEntries.Add(new PetBrokerEntry(reader));

            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                {
                    foreach (PetBrokerEntry entry in m_BrokerEntries)
                    {
                        if (entry.Pet != null && !entry.Pet.IsStabled)
                        {
                            AddToViewTimer(entry.Pet);
                        }
                    }
                });
        }
    }
}
