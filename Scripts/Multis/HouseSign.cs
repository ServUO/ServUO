using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Multis
{
    public class HouseSign : Item
    {
        private BaseHouse m_Owner;
        private Mobile m_OrgOwner;

        public HouseSign(BaseHouse owner)
            : base(0xBD2)
        {
            this.m_Owner = owner;
            this.m_OrgOwner = this.m_Owner.Owner;
            this.Movable = false;
        }

        public HouseSign(Serial serial)
            : base(serial)
        {
        }

        public string GetName()
        {
            if (this.Name == null)
                return "An Unnamed House";

            return this.Name;
        }

        public BaseHouse Owner
        {
            get
            {
                return this.m_Owner;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RestrictDecay
        {
            get
            {
                return (this.m_Owner != null && this.m_Owner.RestrictDecay);
            }
            set
            {
                if (this.m_Owner != null)
                    this.m_Owner.RestrictDecay = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile OriginalOwner
        {
            get
            {
                return this.m_OrgOwner;
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Owner != null && !this.m_Owner.Deleted)
                this.m_Owner.Delete();
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1061638); // A House Sign
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }

        private bool m_GettingProperties;

        public bool GettingProperties
        {
            get
            {
                return this.m_GettingProperties;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1061639, Utility.FixHtml(this.GetName())); // Name: ~1_NAME~
            list.Add(1061640, (this.m_Owner == null || this.m_Owner.Owner == null) ? "nobody" : this.m_Owner.Owner.Name); // Owner: ~1_OWNER~

            if (this.m_Owner != null)
            {
                list.Add(this.m_Owner.Public ? 1061641 : 1061642); // This House is Open to the Public : This is a Private Home

                this.m_GettingProperties = true;
                DecayLevel level = this.m_Owner.DecayLevel;
                this.m_GettingProperties = false;

                if (level == DecayLevel.DemolitionPending)
                {
                    list.Add(1062497); // Demolition Pending
                }
                else if (level != DecayLevel.Ageless)
                {
                    if (level == DecayLevel.Collapsed)
                        level = DecayLevel.IDOC;

                    list.Add(1062028, String.Format("#{0}", 1043009 + (int)level)); // Condition: This structure is ...
                }
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.m_Owner != null && BaseHouse.DecayEnabled && this.m_Owner.DecayPeriod != TimeSpan.Zero)
            {
                string message;

                switch ( this.m_Owner.DecayLevel )
                {
                    case DecayLevel.Ageless:
                        message = "ageless";
                        break;
                    case DecayLevel.Fairly:
                        message = "fairly worn";
                        break;
                    case DecayLevel.Greatly:
                        message = "greatly worn";
                        break;
                    case DecayLevel.LikeNew:
                        message = "like new";
                        break;
                    case DecayLevel.Slightly:
                        message = "slightly worn";
                        break;
                    case DecayLevel.Somewhat:
                        message = "somewhat worn";
                        break;
                    default:
                        message = "in danger of collapsing";
                        break;
                }

                this.LabelTo(from, "This house is {0}.", message);
            }

            base.OnSingleClick(from);
        }

        public void ShowSign(Mobile m)
        {
            if (this.m_Owner != null)
            {
                if (this.m_Owner.IsFriend(m) && m.AccessLevel < AccessLevel.GameMaster)
                {
                    #region Mondain's Legacy
                    if ((Core.ML && this.m_Owner.IsOwner(m)) || !Core.ML)
                        this.m_Owner.RefreshDecay();
                    #endregion
                    if (!Core.AOS)
                        m.SendLocalizedMessage(501293); // Welcome back to the house, friend!
                }

                if (this.m_Owner.IsAosRules)
                    m.SendGump(new HouseGumpAOS(HouseGumpPageAOS.Information, m, this.m_Owner));
                else
                    m.SendGump(new HouseGump(m, this.m_Owner));
            }
        }

        public void ClaimGump_Callback(Mobile from, bool okay, object state)
        {
            if (okay && this.m_Owner != null && this.m_Owner.Owner == null && this.m_Owner.DecayLevel != DecayLevel.DemolitionPending)
            {
                bool canClaim = false;

                if (this.m_Owner.CoOwners == null || this.m_Owner.CoOwners.Count == 0)
                    canClaim = this.m_Owner.IsFriend(from);
                else
                    canClaim = this.m_Owner.IsCoOwner(from);

                if (canClaim && !BaseHouse.HasAccountHouse(from))
                {
                    this.m_Owner.Owner = from;
                    this.m_Owner.LastTraded = DateTime.UtcNow;
                }
            }

            this.ShowSign(from);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (this.m_Owner == null)
                return;

            if (m.AccessLevel < AccessLevel.GameMaster && this.m_Owner.Owner == null && this.m_Owner.DecayLevel != DecayLevel.DemolitionPending)
            {
                bool canClaim = false;

                if (this.m_Owner.CoOwners == null || this.m_Owner.CoOwners.Count == 0)
                    canClaim = this.m_Owner.IsFriend(m);
                else
                    canClaim = this.m_Owner.IsCoOwner(m);

                if (canClaim && !BaseHouse.HasAccountHouse(m))
                {
                    /* You do not currently own any house on any shard with this account,
                    * and this house currently does not have an owner.  If you wish, you
                    * may choose to claim this house and become its rightful owner.  If
                    * you do this, it will become your Primary house and automatically
                    * refresh.  If you claim this house, you will be unable to place
                    * another house or have another house transferred to you for the
                    * next 7 days.  Do you wish to claim this house?
                    */
                    m.SendGump(new WarningGump(501036, 32512, 1049719, 32512, 420, 280, new WarningGumpCallback(ClaimGump_Callback), null));
                }
            }

            this.ShowSign(m);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (BaseHouse.NewVendorSystem && from.Alive && this.Owner != null && this.Owner.IsAosRules)
            {
                if (this.Owner.AreThereAvailableVendorsFor(from))
                    list.Add(new VendorsEntry(this));

                if (this.Owner.VendorInventories.Count > 0)
                    list.Add(new ReclaimVendorInventoryEntry(this));
            }
        }

        private class VendorsEntry : ContextMenuEntry
        {
            private readonly HouseSign m_Sign;

            public VendorsEntry(HouseSign sign)
                : base(6211)
            {
                this.m_Sign = sign;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (!from.CheckAlive() || this.m_Sign.Deleted || this.m_Sign.Owner == null || !this.m_Sign.Owner.AreThereAvailableVendorsFor(from))
                    return;

                if (from.Map != this.m_Sign.Map || !from.InRange(this.m_Sign, 5))
                {
                    from.SendLocalizedMessage(1062429); // You must be within five paces of the house sign to use this option.
                }
                else
                {
                    from.SendGump(new HouseGumpAOS(HouseGumpPageAOS.Vendors, from, this.m_Sign.Owner));
                }
            }
        }

        private class ReclaimVendorInventoryEntry : ContextMenuEntry
        {
            private readonly HouseSign m_Sign;

            public ReclaimVendorInventoryEntry(HouseSign sign)
                : base(6213)
            {
                this.m_Sign = sign;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (this.m_Sign.Deleted || this.m_Sign.Owner == null || this.m_Sign.Owner.VendorInventories.Count == 0 || !from.CheckAlive())
                    return;

                if (from.Map != this.m_Sign.Map || !from.InRange(this.m_Sign, 5))
                {
                    from.SendLocalizedMessage(1062429); // You must be within five paces of the house sign to use this option.
                }
                else
                {
                    from.CloseGump(typeof(VendorInventoryGump));
                    from.SendGump(new VendorInventoryGump(this.m_Sign.Owner, from));
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Owner);
            writer.Write(this.m_OrgOwner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Owner = reader.ReadItem() as BaseHouse;
                        this.m_OrgOwner = reader.ReadMobile();

                        break;
                    }
            }

            if (this.Name == "a house sign")
                this.Name = null;
        }
    }
}