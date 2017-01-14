using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class VendorRentalContract : Item
    {
        private VendorRentalDuration m_Duration;
        private int m_Price;
        private bool m_LandlordRenew;
        private Mobile m_Offeree;
        private Timer m_OfferExpireTimer;
        [Constructable]
        public VendorRentalContract()
            : base(0x14F0)
        {
            this.Weight = 1.0;
            this.Hue = 0x672;

            this.m_Duration = VendorRentalDuration.Instances[0];
            this.m_Price = 1500;
        }

        public VendorRentalContract(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062332;
            }
        }// a vendor rental contract
        public VendorRentalDuration Duration
        {
            get
            {
                return this.m_Duration;
            }
            set
            {
                if (value != null)
                    this.m_Duration = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Price
        {
            get
            {
                return this.m_Price;
            }
            set
            {
                this.m_Price = value;
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
        public Mobile Offeree
        {
            get
            {
                return this.m_Offeree;
            }
            set
            {
                if (this.m_OfferExpireTimer != null)
                {
                    this.m_OfferExpireTimer.Stop();
                    this.m_OfferExpireTimer = null;
                }

                this.m_Offeree = value;

                if (value != null)
                {
                    this.m_OfferExpireTimer = new OfferExpireTimer(this);
                    this.m_OfferExpireTimer.Start();
                }

                this.InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.Offeree != null)
                list.Add(1062368, this.Offeree.Name); // Being Offered To ~1_NAME~
        }

        public bool IsLandlord(Mobile m)
        {
            if (this.IsLockedDown)
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null && house.DecayType != DecayType.Condemned)
                    return house.IsOwner(m);
            }

            return false;
        }

        public bool IsUsableBy(Mobile from, bool byLandlord, bool byBackpack, bool noOfferee, bool sendMessage)
        {
            if (this.Deleted || !from.CheckAlive(sendMessage))
                return false;

            if (noOfferee && this.Offeree != null)
            {
                if (sendMessage)
                    from.SendLocalizedMessage(1062343); // That item is currently in use.

                return false;
            }

            if (byBackpack && this.IsChildOf(from.Backpack))
                return true;

            if (byLandlord && this.IsLandlord(from))
            {
                if (from.Map != this.Map || !from.InRange(this, 5))
                {
                    if (sendMessage)
                        from.SendLocalizedMessage(501853); // Target is too far away.

                    return false;
                }

                return true;
            }

            return false;
        }

        public override void OnDelete()
        {
            if (this.IsLockedDown)
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null)
                {
                    house.VendorRentalContracts.Remove(this);
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.Offeree != null)
            {
                from.SendLocalizedMessage(1062343); // That item is currently in use.
            }
            else if (!this.IsLockedDown)
            {
                if (!this.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                    return;
                }

                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house == null || !house.IsOwner(from))
                {
                    from.SendLocalizedMessage(1062333); // You must be standing inside of a house that you own to make use of this contract.
                }
                else if (!house.IsAosRules)
                {
                    from.SendMessage("Rental contracts can only be placed in AOS-enabled houses.");
                }
                else if (!house.Public)
                {
                    from.SendLocalizedMessage(1062335); // Rental contracts can only be placed in public houses.
                }
                else if (!house.CanPlaceNewVendor())
                {
                    from.SendLocalizedMessage(1062352); // You do not have enought storage available to place this contract.
                }
                else
                {
                    from.SendLocalizedMessage(1062337); // Target the exact location you wish to rent out.
                    from.Target = new RentTarget(this);
                }
            }
            else if (this.IsLandlord(from))
            {
                if (from.InRange(this, 5))
                {
                    from.CloseGump(typeof(VendorRentalContractGump));
                    from.SendGump(new VendorRentalContractGump(this, from));
                }
                else
                {
                    from.SendLocalizedMessage(501853); // Target is too far away.
                }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (this.IsUsableBy(from, true, true, true, false))
            {
                list.Add(new ContractOptionEntry(this));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(this.m_Duration.ID);

            writer.Write((int)this.m_Price);
            writer.Write((bool)this.m_LandlordRenew);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            int durationID = reader.ReadEncodedInt();
            if (durationID < VendorRentalDuration.Instances.Length)
                this.m_Duration = VendorRentalDuration.Instances[durationID];
            else
                this.m_Duration = VendorRentalDuration.Instances[0];

            this.m_Price = reader.ReadInt();
            this.m_LandlordRenew = reader.ReadBool();
        }

        private class ContractOptionEntry : ContextMenuEntry
        {
            private readonly VendorRentalContract m_Contract;
            public ContractOptionEntry(VendorRentalContract contract)
                : base(6209)
            {
                this.m_Contract = contract;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (this.m_Contract.IsUsableBy(from, true, true, true, true))
                {
                    from.CloseGump(typeof(VendorRentalContractGump));
                    from.SendGump(new VendorRentalContractGump(this.m_Contract, from));
                }
            }
        }

        private class RentTarget : Target
        {
            private readonly VendorRentalContract m_Contract;
            public RentTarget(VendorRentalContract contract)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Contract = contract;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!this.m_Contract.IsUsableBy(from, false, true, true, true))
                    return;

                IPoint3D location = targeted as IPoint3D;
                if (location == null)
                    return;

                Point3D pLocation = new Point3D(location);
                Map map = from.Map;

                BaseHouse house = BaseHouse.FindHouseAt(pLocation, map, 0);

                if (house == null || !house.IsOwner(from))
                {
                    from.SendLocalizedMessage(1062338); // The location being rented out must be inside of your house.
                }
                else if (BaseHouse.FindHouseAt(from) != house)
                {
                    from.SendLocalizedMessage(1062339); // You must be located inside of the house in which you are trying to place the contract.
                }
                else if (!house.IsAosRules)
                {
                    from.SendMessage("Rental contracts can only be placed in AOS-enabled houses.");
                }
                else if (!house.Public)
                {
                    from.SendLocalizedMessage(1062335); // Rental contracts can only be placed in public houses.
                }
                else if (house.DecayType == DecayType.Condemned)
                {
                    from.SendLocalizedMessage(1062468); // You cannot place a contract in a condemned house.
                }
                else if (!house.CanPlaceNewVendor())
                {
                    from.SendLocalizedMessage(1062352); // You do not have enought storage available to place this contract.
                }
                else if (!map.CanFit(pLocation, 16, false, false))
                {
                    from.SendLocalizedMessage(1062486); // A vendor cannot exist at that location.  Please try again.
                }
                else
                {
                    bool vendor, contract;
                    BaseHouse.IsThereVendor(pLocation, map, out vendor, out contract);

                    if (vendor)
                    {
                        from.SendLocalizedMessage(1062342); // You may not place a rental contract at this location while other beings occupy it.
                    }
                    else if (contract)
                    {
                        from.SendLocalizedMessage(1062341); // That location is cluttered.  Please clear out any objects there and try again.
                    }
                    else
                    {
                        this.m_Contract.MoveToWorld(pLocation, map);

                        if (!house.LockDown(from, this.m_Contract))
                        {
                            from.AddToBackpack(this.m_Contract);
                        }
                    }
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.SendLocalizedMessage(1062336); // You decide not to place the contract at this time.
            }
        }

        private class OfferExpireTimer : Timer
        {
            private readonly VendorRentalContract m_Contract;
            public OfferExpireTimer(VendorRentalContract contract)
                : base(TimeSpan.FromSeconds(30.0))
            {
                this.m_Contract = contract;

                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                Mobile offeree = this.m_Contract.Offeree;

                if (offeree != null)
                {
                    offeree.CloseGump(typeof(VendorRentalOfferGump));

                    this.m_Contract.Offeree = null;
                }
            }
        }
    }
}