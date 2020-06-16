using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class VendorRentalContract : Item
    {
		public override int LabelNumber => 1062332;// a vendor rental contract
		
        private VendorRentalDuration m_Duration;
        private int m_Price;
        private bool m_LandlordRenew;
        private Mobile m_Offeree;
        private Timer m_OfferExpireTimer;
		
        [Constructable]
        public VendorRentalContract()
            : base(0x14F0)
        {
            Weight = 1.0;
            Hue = 0x672;

            m_Duration = VendorRentalDuration.Instances[0];
            m_Price = 1500;
        }

        public VendorRentalContract(Serial serial)
            : base(serial)
        {
        }
		
        public VendorRentalDuration Duration
        {
            get
            {
                return m_Duration;
            }
            set
            {
                if (value != null)
                    m_Duration = value;
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int Price
        {
            get
            {
                return m_Price;
            }
            set
            {
                m_Price = value;
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
		
        public Mobile Offeree
        {
            get
            {
                return m_Offeree;
            }
            set
            {
                if (m_OfferExpireTimer != null)
                {
                    m_OfferExpireTimer.Stop();
                    m_OfferExpireTimer = null;
                }

                m_Offeree = value;

                if (value != null)
                {
                    m_OfferExpireTimer = new OfferExpireTimer(this);
                    m_OfferExpireTimer.Start();
                }

                InvalidateProperties();
            }
        }
		
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Offeree != null)
                list.Add(1062368, Offeree.Name); // Being Offered To ~1_NAME~
        }

        public bool IsLandlord(Mobile m)
        {
            if (IsLockedDown)
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null && house.DecayType != DecayType.Condemned)
                    return house.IsOwner(m);
            }

            return false;
        }

        public bool IsUsableBy(Mobile from, bool byLandlord, bool byBackpack, bool noOfferee, bool sendMessage)
        {
            if (Deleted || !from.CheckAlive(sendMessage))
                return false;

            if (noOfferee && Offeree != null)
            {
                if (sendMessage)
                    from.SendLocalizedMessage(1062343); // That item is currently in use.

                return false;
            }

            if (byBackpack && IsChildOf(from.Backpack))
                return true;

            if (byLandlord && IsLandlord(from))
            {
                if (from.Map != Map || !from.InRange(this, 5))
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
            if (IsLockedDown)
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
            if (Offeree != null)
            {
                from.SendLocalizedMessage(1062343); // That item is currently in use.
            }
            else if (!IsLockedDown)
            {
                if (!IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                    return;
                }

                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house == null || !house.IsOwner(from))
                {
                    from.SendLocalizedMessage(1062333); // You must be standing inside of a house that you own to make use of this contract.
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
            else if (IsLandlord(from))
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

            if (IsUsableBy(from, true, true, true, false))
            {
                list.Add(new ContractOptionEntry(this));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(m_Duration.ID);

            writer.Write(m_Price);
            writer.Write(m_LandlordRenew);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            int durationID = reader.ReadEncodedInt();
            if (durationID < VendorRentalDuration.Instances.Length)
                m_Duration = VendorRentalDuration.Instances[durationID];
            else
                m_Duration = VendorRentalDuration.Instances[0];

            m_Price = reader.ReadInt();
            m_LandlordRenew = reader.ReadBool();
        }

        private class ContractOptionEntry : ContextMenuEntry
        {
            private readonly VendorRentalContract m_Contract;
			
            public ContractOptionEntry(VendorRentalContract contract)
                : base(6209)
            {
                m_Contract = contract;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (m_Contract.IsUsableBy(from, true, true, true, true))
                {
                    from.CloseGump(typeof(VendorRentalContractGump));
                    from.SendGump(new VendorRentalContractGump(m_Contract, from));
                }
            }
        }

        private class RentTarget : Target
        {
            private readonly VendorRentalContract m_Contract;
            public RentTarget(VendorRentalContract contract)
                : base(-1, false, TargetFlags.None)
            {
                m_Contract = contract;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!m_Contract.IsUsableBy(from, false, true, true, true))
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
                        m_Contract.MoveToWorld(pLocation, map);

                        if (!house.LockDown(from, m_Contract))
                        {
                            from.AddToBackpack(m_Contract);
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
                m_Contract = contract;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                Mobile offeree = m_Contract.Offeree;

                if (offeree != null)
                {
                    offeree.CloseGump(typeof(VendorRentalOfferGump));

                    m_Contract.Offeree = null;
                }
            }
        }
    }
}