using Server.Accounting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Multis;
using Server.Prompts;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class VendorRentalDuration
    {
        public static readonly VendorRentalDuration[] Instances = new VendorRentalDuration[]
        {
            new VendorRentalDuration(TimeSpan.FromDays(7.0), 1062361), // 1 Week
            new VendorRentalDuration(TimeSpan.FromDays(14.0), 1062362), // 2 Weeks
            new VendorRentalDuration(TimeSpan.FromDays(21.0), 1062363), // 3 Weeks
            new VendorRentalDuration(TimeSpan.FromDays(28.0), 1062364)// 1 Month
        };

        private readonly TimeSpan m_Duration;
        private readonly int m_Name;

        private VendorRentalDuration(TimeSpan duration, int name)
        {
            m_Duration = duration;
            m_Name = name;
        }

        public TimeSpan Duration => m_Duration;
        public int Name => m_Name;
        public int ID
        {
            get
            {
                for (int i = 0; i < Instances.Length; i++)
                {
                    if (Instances[i] == this)
                        return i;
                }

                return 0;
            }
        }
    }

    public class RentedVendor : PlayerVendor
    {
        private VendorRentalDuration m_RentalDuration;
        private int m_RentalPrice;
        private bool m_LandlordRenew;
        private bool m_RenterRenew;
        private int m_RenewalPrice;
        private int m_RentalGold;
        private DateTime m_RentalExpireTime;

        public RentedVendor(Mobile owner, BaseHouse house, VendorRentalDuration duration, int rentalPrice, bool landlordRenew, int rentalGold)
            : base(owner, house)
        {
            m_RentalDuration = duration;
            m_RentalPrice = m_RenewalPrice = rentalPrice;
            m_LandlordRenew = landlordRenew;
            m_RenterRenew = false;

            m_RentalGold = rentalGold;

            m_RentalExpireTime = DateTime.UtcNow + duration.Duration;
        }

        public RentedVendor(Serial serial)
            : base(serial)
        {
        }

        public VendorRentalDuration RentalDuration => m_RentalDuration;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RentalPrice
        {
            get
            {
                return m_RentalPrice;
            }
            set
            {
                m_RentalPrice = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LandlordRenew
        {
            get
            {
                return m_LandlordRenew;
            }
            set
            {
                m_LandlordRenew = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RenterRenew
        {
            get
            {
                return m_RenterRenew;
            }
            set
            {
                m_RenterRenew = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Renew => LandlordRenew && RenterRenew && House != null && House.DecayType != DecayType.Condemned;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RenewalPrice
        {
            get
            {
                return m_RenewalPrice;
            }
            set
            {
                m_RenewalPrice = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RentalGold
        {
            get
            {
                return m_RentalGold;
            }
            set
            {
                m_RentalGold = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RentalExpireTime
        {
            get
            {
                return m_RentalExpireTime;
            }
            set
            {
                m_RentalExpireTime = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Landlord
        {
            get
            {
                if (House != null)
                    return House.Owner;

                return null;
            }
        }
        public override bool IsOwner(Mobile m)
        {
            return m == Owner || m.AccessLevel >= AccessLevel.GameMaster || AccountHandler.CheckAccount(m, Owner);
        }

        public bool IsLandlord(Mobile m)
        {
            return House != null && House.IsOwner(m);
        }

        public void ComputeRentalExpireDelay(out int days, out int hours)
        {
            TimeSpan delay = RentalExpireTime - DateTime.UtcNow;

            if (delay <= TimeSpan.Zero)
            {
                days = 0;
                hours = 0;
            }
            else
            {
                days = delay.Days;
                hours = delay.Hours;
            }
        }

        public void SendRentalExpireMessage(Mobile to)
        {
            int days, hours;
            ComputeRentalExpireDelay(out days, out hours);

            to.SendLocalizedMessage(1062464, days.ToString() + "\t" + hours.ToString()); // The rental contract on this vendor will expire in ~1_DAY~ day(s) and ~2_HOUR~ hour(s).
        }

        public override void Destroy(bool toBackpack)
        {
            if (RentalGold > 0 && House != null)
            {
                if (AccountGold.Enabled && Landlord != null)
                {
                    Banker.Deposit(Landlord, RentalGold, true);
                }
                else
                {
                    if (House.MovingCrate == null)
                        House.MovingCrate = new MovingCrate(House);

                    Banker.Deposit(House.MovingCrate, RentalGold);
                }

                RentalGold = 0;
            }

            base.Destroy(toBackpack);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)
            {
                if (IsOwner(from))
                {
                    list.Add(new ContractOptionsEntry(this));
                }
                else if (IsLandlord(from))
                {
                    if (RentalGold > 0)
                        list.Add(new CollectRentEntry(this));

                    list.Add(new TerminateContractEntry(this));
                    list.Add(new ContractOptionsEntry(this));
                }
            }

            base.GetContextMenuEntries(from, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(m_RentalDuration.ID);

            writer.Write(m_RentalPrice);
            writer.Write(m_LandlordRenew);
            writer.Write(m_RenterRenew);
            writer.Write(m_RenewalPrice);

            writer.Write(m_RentalGold);

            writer.WriteDeltaTime(m_RentalExpireTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            int durationID = reader.ReadEncodedInt();
            if (durationID < VendorRentalDuration.Instances.Length)
                m_RentalDuration = VendorRentalDuration.Instances[durationID];
            else
                m_RentalDuration = VendorRentalDuration.Instances[0];

            m_RentalPrice = reader.ReadInt();
            m_LandlordRenew = reader.ReadBool();
            m_RenterRenew = reader.ReadBool();
            m_RenewalPrice = reader.ReadInt();

            m_RentalGold = reader.ReadInt();

            m_RentalExpireTime = reader.ReadDeltaTime();
        }

        private class ContractOptionsEntry : ContextMenuEntry
        {
            private readonly RentedVendor m_Vendor;
            public ContractOptionsEntry(RentedVendor vendor)
                : base(6209)
            {
                m_Vendor = vendor;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (m_Vendor.Deleted || !from.CheckAlive())
                    return;

                if (m_Vendor.IsOwner(from))
                {
                    from.CloseGump(typeof(RenterVendorRentalGump));
                    from.SendGump(new RenterVendorRentalGump(m_Vendor));

                    m_Vendor.SendRentalExpireMessage(from);
                }
                else if (m_Vendor.IsLandlord(from))
                {
                    from.CloseGump(typeof(LandlordVendorRentalGump));
                    from.SendGump(new LandlordVendorRentalGump(m_Vendor));

                    m_Vendor.SendRentalExpireMessage(from);
                }
            }
        }

        private class CollectRentEntry : ContextMenuEntry
        {
            private readonly RentedVendor m_Vendor;
            public CollectRentEntry(RentedVendor vendor)
                : base(6212)
            {
                m_Vendor = vendor;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (m_Vendor.Deleted || !from.CheckAlive() || !m_Vendor.IsLandlord(from))
                    return;

                if (m_Vendor.RentalGold > 0)
                {
                    int depositedGold = Banker.DepositUpTo(from, m_Vendor.RentalGold);
                    m_Vendor.RentalGold -= depositedGold;

                    if (depositedGold > 0)
                        from.SendLocalizedMessage(1060397, depositedGold.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.

                    if (m_Vendor.RentalGold > 0)
                        from.SendLocalizedMessage(500390); // Your bank box is full.
                }
            }
        }

        private class TerminateContractEntry : ContextMenuEntry
        {
            private readonly RentedVendor m_Vendor;
            public TerminateContractEntry(RentedVendor vendor)
                : base(6218)
            {
                m_Vendor = vendor;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (m_Vendor.Deleted || !from.CheckAlive() || !m_Vendor.IsLandlord(from))
                    return;

                from.SendLocalizedMessage(1062503); // Enter the amount of gold you wish to offer the renter in exchange for immediate termination of this contract?
                from.Prompt = new RefundOfferPrompt(m_Vendor);
            }
        }

        private class RefundOfferPrompt : Prompt
        {
            private readonly RentedVendor m_Vendor;
            public RefundOfferPrompt(RentedVendor vendor)
            {
                m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_Vendor.CanInteractWith(from, false) || !m_Vendor.IsLandlord(from))
                    return;

                text = text.Trim();

                int amount;

                if (!int.TryParse(text, out amount))
                    amount = -1;

                Mobile owner = m_Vendor.Owner;
                if (owner == null)
                    return;

                if (amount < 0)
                {
                    from.SendLocalizedMessage(1062506); // You did not enter a valid amount.  Offer canceled.
                }
                else if (Banker.GetBalance(from) < amount)
                {
                    from.SendLocalizedMessage(1062507); // You do not have that much money in your bank account.
                }
                else if (owner.Map != m_Vendor.Map || !owner.InRange(m_Vendor, 5))
                {
                    from.SendLocalizedMessage(1062505); // The renter must be closer to the vendor in order for you to make this offer.
                }
                else
                {
                    from.SendLocalizedMessage(1062504); // Please wait while the renter considers your offer.

                    owner.CloseGump(typeof(VendorRentalRefundGump));
                    owner.SendGump(new VendorRentalRefundGump(m_Vendor, from, amount));
                }
            }
        }
    }
}
