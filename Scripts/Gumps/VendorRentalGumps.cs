using System;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Prompts;
using Server.Targeting;

namespace Server.Gumps
{
    public abstract class BaseVendorRentalGump : Gump
    {
        protected BaseVendorRentalGump(GumpType type, VendorRentalDuration duration, int price, int renewalPrice,
            Mobile landlord, Mobile renter, bool landlordRenew, bool renterRenew, bool renew)
            : base(100, 100)
        {
            if (type == GumpType.Offer)
                this.Closable = false;

            this.AddPage(0);

            this.AddImage(0, 0, 0x1F40);
            this.AddImageTiled(20, 37, 300, 308, 0x1F42);
            this.AddImage(20, 325, 0x1F43);

            this.AddImage(35, 8, 0x39);
            this.AddImageTiled(65, 8, 257, 10, 0x3A);
            this.AddImage(290, 8, 0x3B);

            this.AddImageTiled(70, 55, 230, 2, 0x23C5);

            this.AddImage(32, 33, 0x2635);
            this.AddHtmlLocalized(70, 35, 270, 20, 1062353, 0x1, false, false); // Vendor Rental Contract

            this.AddPage(1);

            if (type != GumpType.UnlockedContract)
            {
                this.AddImage(65, 60, 0x827);
                this.AddHtmlLocalized(79, 58, 270, 20, 1062370, 0x1, false, false); // Landlord:
                this.AddLabel(150, 58, 0x64, landlord != null ? landlord.Name : "");

                this.AddImageTiled(70, 80, 230, 2, 0x23C5);
            }

            if (type == GumpType.UnlockedContract || type == GumpType.LockedContract)
                this.AddButton(30, 96, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 2);
            this.AddHtmlLocalized(50, 95, 150, 20, 1062354, 0x1, false, false); // Contract Length
            this.AddHtmlLocalized(230, 95, 270, 20, duration.Name, 0x1, false, false);

            if (type == GumpType.UnlockedContract || type == GumpType.LockedContract)
                this.AddButton(30, 116, 0x15E1, 0x15E5, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(50, 115, 150, 20, 1062356, 0x1, false, false); // Price Per Rental
            this.AddLabel(230, 115, 0x64, price > 0 ? price.ToString() : "FREE");

            this.AddImageTiled(50, 160, 250, 2, 0x23BF);

            if (type == GumpType.Offer)
            {
                this.AddButton(67, 180, 0x482, 0x483, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(100, 180, 270, 20, 1049011, 0x28, false, false); // I accept!

                this.AddButton(67, 210, 0x47F, 0x480, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(100, 210, 270, 20, 1049012, 0x28, false, false); // No thanks, I decline.
            }
            else
            {
                this.AddImage(49, 170, 0x61);
                this.AddHtmlLocalized(60, 170, 250, 20, 1062355, 0x1, false, false); // Renew On Expiration?

                if (type == GumpType.LockedContract || type == GumpType.UnlockedContract || type == GumpType.VendorLandlord)
                    this.AddButton(30, 192, 0x15E1, 0x15E5, 3, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(85, 190, 250, 20, 1062359, 0x1, false, false); // Landlord:
                this.AddHtmlLocalized(230, 190, 270, 20, landlordRenew ? 1049717 : 1049718, 0x1, false, false); // YES / NO

                if (type == GumpType.VendorRenter)
                    this.AddButton(30, 212, 0x15E1, 0x15E5, 4, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(85, 210, 250, 20, 1062360, 0x1, false, false); // Renter:
                this.AddHtmlLocalized(230, 210, 270, 20, renterRenew ? 1049717 : 1049718, 0x1, false, false); // YES / NO

                if (renew)
                {
                    this.AddImage(49, 233, 0x939);
                    this.AddHtmlLocalized(70, 230, 250, 20, 1062482, 0x1, false, false); // Contract WILL renew
                }
                else
                {
                    this.AddImage(49, 233, 0x938);
                    this.AddHtmlLocalized(70, 230, 250, 20, 1062483, 0x1, false, false); // Contract WILL NOT renew
                }
            }

            this.AddImageTiled(30, 283, 257, 30, 0x5D);
            this.AddImage(285, 283, 0x5E);
            this.AddImage(20, 288, 0x232C);

            if (type == GumpType.LockedContract)
            {
                this.AddButton(67, 295, 0x15E1, 0x15E5, 5, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(85, 294, 270, 20, 1062358, 0x28, false, false); // Offer Contract To Someone
            }
            else if (type == GumpType.VendorLandlord || type == GumpType.VendorRenter)
            {
                if (type == GumpType.VendorLandlord)
                    this.AddButton(30, 250, 0x15E1, 0x15E1, 6, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(85, 250, 250, 20, 1062499, 0x1, false, false); // Renewal Price
                this.AddLabel(230, 250, 0x64, renewalPrice.ToString());

                this.AddHtmlLocalized(60, 294, 270, 20, 1062369, 0x1, false, false); // Renter:
                this.AddLabel(120, 293, 0x64, renter != null ? renter.Name : "");
            }

            if (type == GumpType.UnlockedContract || type == GumpType.LockedContract)
            {
                this.AddPage(2);

                for (int i = 0; i < VendorRentalDuration.Instances.Length; i++)
                {
                    VendorRentalDuration durationItem = VendorRentalDuration.Instances[i];

                    this.AddButton(30, 76 + i * 20, 0x15E1, 0x15E5, 0x10 | i, GumpButtonType.Reply, 1);
                    this.AddHtmlLocalized(50, 75 + i * 20, 150, 20, durationItem.Name, 0x1, false, false);
                }
            }
        }

        protected enum GumpType
        {
            UnlockedContract,
            LockedContract,
            Offer,
            VendorLandlord,
            VendorRenter
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (!this.IsValidResponse(from))
                return;

            if ((info.ButtonID & 0x10) != 0) // Contract duration
            {
                int index = info.ButtonID & 0xF;

                if (index < VendorRentalDuration.Instances.Length)
                {
                    this.SetContractDuration(from, VendorRentalDuration.Instances[index]);
                }
            }
            else
            {
                switch ( info.ButtonID )
                {
                    case 1: // Price Per Rental
                        this.SetPricePerRental(from);
                        break;
                    case 2: // Accept offer
                        this.AcceptOffer(from);
                        break;
                    case 3: // Renew on expiration - landlord
                        this.LandlordRenewOnExpiration(from);
                        break;
                    case 4: // Renew on expiration - renter
                        this.RenterRenewOnExpiration(from);
                        break;
                    case 5: // Offer Contract To Someone
                        this.OfferContract(from);
                        break;
                    case 6: // Renewal price
                        this.SetRenewalPrice(from);
                        break;
                    default:
                        this.Cancel(from);
                        break;
                }
            }
        }

        protected abstract bool IsValidResponse(Mobile from);

        protected virtual void SetContractDuration(Mobile from, VendorRentalDuration duration)
        {
        }

        protected virtual void SetPricePerRental(Mobile from)
        {
        }

        protected virtual void AcceptOffer(Mobile from)
        {
        }

        protected virtual void LandlordRenewOnExpiration(Mobile from)
        {
        }

        protected virtual void RenterRenewOnExpiration(Mobile from)
        {
        }

        protected virtual void OfferContract(Mobile from)
        {
        }

        protected virtual void SetRenewalPrice(Mobile from)
        {
        }

        protected virtual void Cancel(Mobile from)
        {
        }
    }

    public class VendorRentalContractGump : BaseVendorRentalGump
    {
        private readonly VendorRentalContract m_Contract;
        public VendorRentalContractGump(VendorRentalContract contract, Mobile from)
            : base(
            contract.IsLockedDown ? GumpType.LockedContract : GumpType.UnlockedContract, contract.Duration,
            contract.Price, contract.Price, from, null, contract.LandlordRenew, false, false)
        {
            this.m_Contract = contract;
        }

        protected override bool IsValidResponse(Mobile from)
        {
            return this.m_Contract.IsUsableBy(from, true, true, true, true);
        }

        protected override void SetContractDuration(Mobile from, VendorRentalDuration duration)
        {
            this.m_Contract.Duration = duration;

            from.SendGump(new VendorRentalContractGump(this.m_Contract, from));
        }

        protected override void SetPricePerRental(Mobile from)
        {
            from.SendLocalizedMessage(1062365); // Please enter the amount of gold that should be charged for this contract (ESC to cancel):
            from.Prompt = new PricePerRentalPrompt(this.m_Contract);
        }

        protected override void LandlordRenewOnExpiration(Mobile from)
        {
            this.m_Contract.LandlordRenew = !this.m_Contract.LandlordRenew;

            from.SendGump(new VendorRentalContractGump(this.m_Contract, from));
        }

        protected override void OfferContract(Mobile from)
        {
            if (this.m_Contract.IsLandlord(from))
            {
                from.SendLocalizedMessage(1062371); // Please target the person you wish to offer this contract to.
                from.Target = new OfferContractTarget(this.m_Contract);
            }
        }

        private class PricePerRentalPrompt : Prompt
        {
            public override int MessageCliloc { get { return 1062365; } }
            private readonly VendorRentalContract m_Contract;
            public PricePerRentalPrompt(VendorRentalContract contract)
            {
                this.m_Contract = contract;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!this.m_Contract.IsUsableBy(from, true, true, true, true))
                    return;

                text = text.Trim();

                int price;

                if (!int.TryParse(text, out price))
                    price = -1;

                if (price < 0)
                {
                    from.SendLocalizedMessage(1062485); // Invalid entry.  Rental fee set to 0.
                    this.m_Contract.Price = 0;
                }
                else if (price > 5000000)
                {
                    this.m_Contract.Price = 5000000;
                }
                else
                {
                    this.m_Contract.Price = price;
                }

                from.SendGump(new VendorRentalContractGump(this.m_Contract, from));
            }

            public override void OnCancel(Mobile from)
            {
                if (this.m_Contract.IsUsableBy(from, true, true, true, true))
                    from.SendGump(new VendorRentalContractGump(this.m_Contract, from));
            }
        }

        private class OfferContractTarget : Target
        {
            private readonly VendorRentalContract m_Contract;
            public OfferContractTarget(VendorRentalContract contract)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Contract = contract;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!this.m_Contract.IsUsableBy(from, true, false, true, true))
                    return;

                Mobile mob = targeted as Mobile;

                if (mob == null || !mob.Player || !mob.Alive || mob == from)
                {
                    from.SendLocalizedMessage(1071984); //That is not a valid target for a rental contract!
                }
                else if (!mob.InRange(this.m_Contract, 5))
                {
                    from.SendLocalizedMessage(501853); // Target is too far away.
                }
                else
                {
                    from.SendLocalizedMessage(1062372); // Please wait while that person considers your offer.

                    mob.SendLocalizedMessage(1062373, from.Name); // ~1_NAME~ is offering you a vendor rental.   If you choose to accept this offer, you have 30 seconds to do so.
                    mob.SendGump(new VendorRentalOfferGump(this.m_Contract, from));

                    this.m_Contract.Offeree = mob;
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.SendLocalizedMessage(1062380); // You decide against offering the contract to anyone.
            }
        }
    }

    public class VendorRentalOfferGump : BaseVendorRentalGump
    {
        private readonly VendorRentalContract m_Contract;
        private readonly Mobile m_Landlord;
        public VendorRentalOfferGump(VendorRentalContract contract, Mobile landlord)
            : base(
            GumpType.Offer, contract.Duration, contract.Price, contract.Price,
            landlord, null, contract.LandlordRenew, false, false)
        {
            this.m_Contract = contract;
            this.m_Landlord = landlord;
        }

        protected override bool IsValidResponse(Mobile from)
        {
            return this.m_Contract.IsUsableBy(this.m_Landlord, true, false, false, false) && from.CheckAlive() && this.m_Contract.Offeree == from;
        }

        protected override void AcceptOffer(Mobile from)
        {
            this.m_Contract.Offeree = null;

            if (!this.m_Contract.Map.CanFit(this.m_Contract.Location, 16, false, false))
            {
                this.m_Landlord.SendLocalizedMessage(1062486); // A vendor cannot exist at that location.  Please try again.
                return;
            }

            BaseHouse house = BaseHouse.FindHouseAt(this.m_Contract);
            if (house == null)
                return;

            int price = this.m_Contract.Price;
            int goldToGive;

            if (price > 0)
            {
                if (Banker.Withdraw(from, price))
                {
                    from.SendLocalizedMessage(1060398, price.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

                    int depositedGold = Banker.DepositUpTo(this.m_Landlord, price);
                    goldToGive = price - depositedGold;

                    if (depositedGold > 0)
                        this.m_Landlord.SendLocalizedMessage(1060397, price.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.

                    if (goldToGive > 0)
                        this.m_Landlord.SendLocalizedMessage(500390); // Your bank box is full.
                }
                else
                {
                    from.SendLocalizedMessage(1062378); // You do not have enough gold in your bank account to cover the cost of the contract.
                    this.m_Landlord.SendLocalizedMessage(1062374, from.Name); // ~1_NAME~ has declined your vendor rental offer.

                    return;
                }
            }
            else
            {
                goldToGive = 0;
            }

            PlayerVendor vendor = new RentedVendor(from, house, this.m_Contract.Duration, price, this.m_Contract.LandlordRenew, goldToGive);
            vendor.MoveToWorld(this.m_Contract.Location, this.m_Contract.Map);

            this.m_Contract.Delete();

            from.SendLocalizedMessage(1062377); // You have accepted the offer and now own a vendor in this house.  Rental contract options and details may be viewed on this vendor via the 'Contract Options' context menu.
            this.m_Landlord.SendLocalizedMessage(1062376, from.Name); // ~1_NAME~ has accepted your vendor rental offer.  Rental contract details and options may be viewed on this vendor via the 'Contract Options' context menu.
        }

        protected override void Cancel(Mobile from)
        {
            this.m_Contract.Offeree = null;

            from.SendLocalizedMessage(1062375); // You decline the offer for a vendor space rental.
            this.m_Landlord.SendLocalizedMessage(1062374, from.Name); // ~1_NAME~ has declined your vendor rental offer.
        }
    }

    public class RenterVendorRentalGump : BaseVendorRentalGump
    {
        private readonly RentedVendor m_Vendor;
        public RenterVendorRentalGump(RentedVendor vendor)
            : base(
            GumpType.VendorRenter, vendor.RentalDuration, vendor.RentalPrice, vendor.RenewalPrice,
            vendor.Landlord, vendor.Owner, vendor.LandlordRenew, vendor.RenterRenew, vendor.Renew)
        {
            this.m_Vendor = vendor;
        }

        protected override bool IsValidResponse(Mobile from)
        {
            return this.m_Vendor.CanInteractWith(from, true);
        }

        protected override void RenterRenewOnExpiration(Mobile from)
        {
            this.m_Vendor.RenterRenew = !this.m_Vendor.RenterRenew;

            from.SendGump(new RenterVendorRentalGump(this.m_Vendor));
        }
    }

    public class LandlordVendorRentalGump : BaseVendorRentalGump
    {
        private readonly RentedVendor m_Vendor;
        public LandlordVendorRentalGump(RentedVendor vendor)
            : base(
            GumpType.VendorLandlord, vendor.RentalDuration, vendor.RentalPrice, vendor.RenewalPrice,
            vendor.Landlord, vendor.Owner, vendor.LandlordRenew, vendor.RenterRenew, vendor.Renew)
        {
            this.m_Vendor = vendor;
        }

        protected override bool IsValidResponse(Mobile from)
        {
            return this.m_Vendor.CanInteractWith(from, false) && this.m_Vendor.IsLandlord(from);
        }

        protected override void LandlordRenewOnExpiration(Mobile from)
        {
            this.m_Vendor.LandlordRenew = !this.m_Vendor.LandlordRenew;

            from.SendGump(new LandlordVendorRentalGump(this.m_Vendor));
        }

        protected override void SetRenewalPrice(Mobile from)
        {
            from.SendLocalizedMessage(1062500); // Enter contract renewal price:

            from.Prompt = new ContractRenewalPricePrompt(this.m_Vendor);
        }

        private class ContractRenewalPricePrompt : Prompt
        {
            public override int MessageCliloc { get { return 1062500; } }
            private readonly RentedVendor m_Vendor;
            public ContractRenewalPricePrompt(RentedVendor vendor)
            {
                this.m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!this.m_Vendor.CanInteractWith(from, false) || !this.m_Vendor.IsLandlord(from))
                    return;

                text = text.Trim();

                int price;

                if (!int.TryParse(text, out price))
                    price = -1;

                if (price < 0)
                {
                    from.SendLocalizedMessage(1062485); // Invalid entry.  Rental fee set to 0.
                    this.m_Vendor.RenewalPrice = 0;
                }
                else if (price > 5000000)
                {
                    this.m_Vendor.RenewalPrice = 5000000;
                }
                else
                {
                    this.m_Vendor.RenewalPrice = price;
                }

                this.m_Vendor.RenterRenew = false;

                from.SendGump(new LandlordVendorRentalGump(this.m_Vendor));
            }

            public override void OnCancel(Mobile from)
            {
                if (this.m_Vendor.CanInteractWith(from, false) && this.m_Vendor.IsLandlord(from))
                    from.SendGump(new LandlordVendorRentalGump(this.m_Vendor));
            }
        }
    }

    public class VendorRentalRefundGump : Gump
    {
        private readonly RentedVendor m_Vendor;
        private readonly Mobile m_Landlord;
        private readonly int m_RefundAmount;
        public VendorRentalRefundGump(RentedVendor vendor, Mobile landlord, int refundAmount)
            : base(50, 50)
        {
            this.m_Vendor = vendor;
            this.m_Landlord = landlord;
            this.m_RefundAmount = refundAmount;

            this.AddBackground(0, 0, 420, 320, 0x13BE);

            this.AddImageTiled(10, 10, 400, 300, 0xA40);
            this.AddAlphaRegion(10, 10, 400, 300);

            /* The landlord for this vendor is offering you a partial refund of your rental fee
            * in exchange for immediate termination of your rental contract.<BR><BR>
            * 
            * If you accept this offer, the vendor will be immediately dismissed.  You will then
            * be able to claim the inventory and any funds the vendor may be holding for you via
            * a context menu on the house sign for this house.
            */
            this.AddHtmlLocalized(10, 10, 400, 150, 1062501, 0x7FFF, false, true);

            this.AddHtmlLocalized(10, 180, 150, 20, 1062508, 0x7FFF, false, false); // Vendor Name:
            this.AddLabel(160, 180, 0x480, vendor.Name);

            this.AddHtmlLocalized(10, 200, 150, 20, 1062509, 0x7FFF, false, false); // Shop Name:
            this.AddLabel(160, 200, 0x480, vendor.ShopName);

            this.AddHtmlLocalized(10, 220, 150, 20, 1062510, 0x7FFF, false, false); // Refund Amount:
            this.AddLabel(160, 220, 0x480, refundAmount.ToString());

            this.AddButton(10, 268, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 268, 350, 20, 1062511, 0x7FFF, false, false); // Agree, and <strong>dismiss vendor</strong>

            this.AddButton(10, 288, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 288, 350, 20, 1062512, 0x7FFF, false, false); // No, I want to <strong>keep my vendor</strong>
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (!this.m_Vendor.CanInteractWith(from, true) || !this.m_Vendor.CanInteractWith(this.m_Landlord, false) || !this.m_Vendor.IsLandlord(this.m_Landlord))
                return;

            if (info.ButtonID == 1)
            {
                if (Banker.Withdraw(this.m_Landlord, this.m_RefundAmount))
                {
                    this.m_Landlord.SendLocalizedMessage(1060398, this.m_RefundAmount.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

                    int depositedGold = Banker.DepositUpTo(from, this.m_RefundAmount);

                    if (depositedGold > 0)
                        from.SendLocalizedMessage(1060397, depositedGold.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.

                    this.m_Vendor.HoldGold += this.m_RefundAmount - depositedGold;

                    this.m_Vendor.Destroy(false);

                    from.SendLocalizedMessage(1071990); //Remember to claim your vendor's belongings from the house sign!
                }
                else
                {
                    this.m_Landlord.SendLocalizedMessage(1062507); // You do not have that much money in your bank account.
                }
            }
            else
            {
                this.m_Landlord.SendLocalizedMessage(1062513); // The renter declined your offer.
            }
        }
    }
}