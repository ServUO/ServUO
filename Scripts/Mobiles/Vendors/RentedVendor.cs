using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Multis;
using Server.Prompts;

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
            this.m_Duration = duration;
            this.m_Name = name;
        }

        public TimeSpan Duration
        {
            get
            {
                return this.m_Duration;
            }
        }
        public int Name
        {
            get
            {
                return this.m_Name;
            }
        }
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
        private Timer m_RentalExpireTimer;
        public RentedVendor(Mobile owner, BaseHouse house, VendorRentalDuration duration, int rentalPrice, bool landlordRenew, int rentalGold)
            : base(owner, house)
        {
            this.m_RentalDuration = duration;
            this.m_RentalPrice = this.m_RenewalPrice = rentalPrice;
            this.m_LandlordRenew = landlordRenew;
            this.m_RenterRenew = false;

            this.m_RentalGold = rentalGold;

            this.m_RentalExpireTime = DateTime.UtcNow + duration.Duration;
            this.m_RentalExpireTimer = new RentalExpireTimer(this, duration.Duration);
            this.m_RentalExpireTimer.Start();
        }

        public RentedVendor(Serial serial)
            : base(serial)
        {
        }

        public VendorRentalDuration RentalDuration
        {
            get
            {
                return this.m_RentalDuration;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RentalPrice
        {
            get
            {
                return this.m_RentalPrice;
            }
            set
            {
                this.m_RentalPrice = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LandlordRenew
        {
            get
            {
                return this.m_LandlordRenew;
            }
            set
            {
                this.m_LandlordRenew = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RenterRenew
        {
            get
            {
                return this.m_RenterRenew;
            }
            set
            {
                this.m_RenterRenew = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Renew
        {
            get
            {
                return this.LandlordRenew && this.RenterRenew && this.House != null && this.House.DecayType != DecayType.Condemned;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RenewalPrice
        {
            get
            {
                return this.m_RenewalPrice;
            }
            set
            {
                this.m_RenewalPrice = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RentalGold
        {
            get
            {
                return this.m_RentalGold;
            }
            set
            {
                this.m_RentalGold = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RentalExpireTime
        {
            get
            {
                return this.m_RentalExpireTime;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Landlord
        {
            get
            {
                if (this.House != null)
                    return this.House.Owner;

                return null;
            }
        }
        public override bool IsOwner(Mobile m)
        {
            return m == this.Owner || m.AccessLevel >= AccessLevel.GameMaster || (Core.ML && AccountHandler.CheckAccount(m, this.Owner));
        }

        public bool IsLandlord(Mobile m)
        {
            return this.House != null && this.House.IsOwner(m);
        }

        public void ComputeRentalExpireDelay(out int days, out int hours)
        {
            TimeSpan delay = this.RentalExpireTime - DateTime.UtcNow;

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
            this.ComputeRentalExpireDelay(out days, out hours);

            to.SendLocalizedMessage(1062464, days.ToString() + "\t" + hours.ToString()); // The rental contract on this vendor will expire in ~1_DAY~ day(s) and ~2_HOUR~ hour(s).
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            this.m_RentalExpireTimer.Stop();
        }

        public override void Destroy(bool toBackpack)
        {
            if (this.RentalGold > 0 && this.House != null && this.House.IsAosRules)
            {
                if (this.House.MovingCrate == null)
                    this.House.MovingCrate = new MovingCrate(this.House);

                Banker.Deposit(this.House.MovingCrate, this.RentalGold);
                this.RentalGold = 0;
            }

            base.Destroy(toBackpack);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)
            {
                if (this.IsOwner(from))
                {
                    list.Add(new ContractOptionsEntry(this));
                }
                else if (this.IsLandlord(from))
                {
                    if (this.RentalGold > 0)
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

            writer.WriteEncodedInt(this.m_RentalDuration.ID);

            writer.Write((int)this.m_RentalPrice);
            writer.Write((bool)this.m_LandlordRenew);
            writer.Write((bool)this.m_RenterRenew);
            writer.Write((int)this.m_RenewalPrice);

            writer.Write((int)this.m_RentalGold);

            writer.WriteDeltaTime((DateTime)this.m_RentalExpireTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            int durationID = reader.ReadEncodedInt();
            if (durationID < VendorRentalDuration.Instances.Length)
                this.m_RentalDuration = VendorRentalDuration.Instances[durationID];
            else
                this.m_RentalDuration = VendorRentalDuration.Instances[0];

            this.m_RentalPrice = reader.ReadInt();
            this.m_LandlordRenew = reader.ReadBool();
            this.m_RenterRenew = reader.ReadBool();
            this.m_RenewalPrice = reader.ReadInt();

            this.m_RentalGold = reader.ReadInt();

            this.m_RentalExpireTime = reader.ReadDeltaTime();

            TimeSpan delay = this.m_RentalExpireTime - DateTime.UtcNow;
            this.m_RentalExpireTimer = new RentalExpireTimer(this, delay > TimeSpan.Zero ? delay : TimeSpan.Zero);
            this.m_RentalExpireTimer.Start();
        }

        private class ContractOptionsEntry : ContextMenuEntry
        {
            private readonly RentedVendor m_Vendor;
            public ContractOptionsEntry(RentedVendor vendor)
                : base(6209)
            {
                this.m_Vendor = vendor;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (this.m_Vendor.Deleted || !from.CheckAlive())
                    return;

                if (this.m_Vendor.IsOwner(from))
                {
                    from.CloseGump(typeof(RenterVendorRentalGump));
                    from.SendGump(new RenterVendorRentalGump(this.m_Vendor));

                    this.m_Vendor.SendRentalExpireMessage(from);
                }
                else if (this.m_Vendor.IsLandlord(from))
                {
                    from.CloseGump(typeof(LandlordVendorRentalGump));
                    from.SendGump(new LandlordVendorRentalGump(this.m_Vendor));

                    this.m_Vendor.SendRentalExpireMessage(from);
                }
            }
        }

        private class CollectRentEntry : ContextMenuEntry
        {
            private readonly RentedVendor m_Vendor;
            public CollectRentEntry(RentedVendor vendor)
                : base(6212)
            {
                this.m_Vendor = vendor;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (this.m_Vendor.Deleted || !from.CheckAlive() || !this.m_Vendor.IsLandlord(from))
                    return;

                if (this.m_Vendor.RentalGold > 0)
                {
                    int depositedGold = Banker.DepositUpTo(from, this.m_Vendor.RentalGold);
                    this.m_Vendor.RentalGold -= depositedGold;

                    if (depositedGold > 0)
                        from.SendLocalizedMessage(1060397, depositedGold.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.

                    if (this.m_Vendor.RentalGold > 0)
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
                this.m_Vendor = vendor;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (this.m_Vendor.Deleted || !from.CheckAlive() || !this.m_Vendor.IsLandlord(from))
                    return;

                from.SendLocalizedMessage(1062503); // Enter the amount of gold you wish to offer the renter in exchange for immediate termination of this contract?
                from.Prompt = new RefundOfferPrompt(this.m_Vendor);
            }
        }

        private class RefundOfferPrompt : Prompt
        {
            private readonly RentedVendor m_Vendor;
            public RefundOfferPrompt(RentedVendor vendor)
            {
                this.m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!this.m_Vendor.CanInteractWith(from, false) || !this.m_Vendor.IsLandlord(from))
                    return;

                text = text.Trim();

                int amount;

                if (!int.TryParse(text, out amount))
                    amount = -1;

                Mobile owner = this.m_Vendor.Owner;
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
                else if (owner.Map != this.m_Vendor.Map || !owner.InRange(this.m_Vendor, 5))
                {
                    from.SendLocalizedMessage(1062505); // The renter must be closer to the vendor in order for you to make this offer.
                }
                else
                {
                    from.SendLocalizedMessage(1062504); // Please wait while the renter considers your offer.

                    owner.CloseGump(typeof(VendorRentalRefundGump));
                    owner.SendGump(new VendorRentalRefundGump(this.m_Vendor, from, amount));
                }
            }
        }

        private class RentalExpireTimer : Timer
        {
            private readonly RentedVendor m_Vendor;
            public RentalExpireTimer(RentedVendor vendor, TimeSpan delay)
                : base(delay, vendor.RentalDuration.Duration)
            {
                this.m_Vendor = vendor;

                this.Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                int renewalPrice = this.m_Vendor.RenewalPrice;

                if (this.m_Vendor.Renew && this.m_Vendor.HoldGold >= renewalPrice)
                {
                    this.m_Vendor.HoldGold -= renewalPrice;
                    this.m_Vendor.RentalGold += renewalPrice;

                    this.m_Vendor.RentalPrice = renewalPrice;

                    this.m_Vendor.m_RentalExpireTime = DateTime.UtcNow + this.m_Vendor.RentalDuration.Duration;
                }
                else
                {
                    this.m_Vendor.Destroy(false);
                }
            }
        }
    }
}