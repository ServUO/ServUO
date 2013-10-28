using System;
using Server;
using Server.Gumps;

namespace CustomsFramework.Systems.VIPSystem
{
    public enum VIPTier
    {
        None,
        Bronze,
        Silver,
        Gold
    }

    public class VIPModule : BaseModule
    {
        private bool _Canceled;
        private DateTime _TimeStarted;
        private TimeSpan _ServicePeriod;
        private VIPTier _Tier;
        private Bonuses _Bonuses;
        public VIPModule(Mobile from, BaseVIPDeed deed)
            : base()
        {
            this._Canceled = false;
            this._TimeStarted = DateTime.MinValue;
            this._ServicePeriod = TimeSpan.Zero;

            if (deed != null)
            {
                this._Tier = deed.Tier;
                this._Bonuses = deed.Bonuses;
            }

            this.LinkMobile(from);
        }

        public VIPModule(CustomSerial serial)
            : base(serial)
        {
        }

        public override string Name
        {
            get
            {
                if (this.LinkedMobile != null)
                    return String.Format(@"VIP Module - {0}", this.LinkedMobile.Name);
                else
                    return @"Unlinked VIP Module";
            }
        }
        public override string Description
        {
            get
            {
                if (this.LinkedMobile != null)
                    return String.Format(@"VIP Module that is linked to {0}, was linked on {1}, and expires on {2}", this.LinkedMobile.Name, 0, 0);
                else
                    return @"Unlinked VIP Module";
            }
        }
        public override string Version
        {
            get
            {
                return VIPCore.SystemVersion;
            }
        }
        public override AccessLevel EditLevel
        {
            get
            {
                return AccessLevel.Developer;
            }
        }
        public override Gump SettingsGump
        {
            get
            {
                return base.SettingsGump; // TODO: Create a settings gump.
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public bool Canceled
        {
            get
            {
                return this._Canceled;
            }
            set
            {
                this._Canceled = value;
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public DateTime TimeStarted
        {
            get
            {
                return this._TimeStarted;
            }
            set
            {
                this._TimeStarted = value;
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public TimeSpan ServicePeriod
        {
            get
            {
                return this._ServicePeriod;
            }
            set
            {
                this._ServicePeriod = value;
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public VIPTier Tier
        {
            get
            {
                return this._Tier;
            }
            set
            {
                this._Tier = value;
                this.SetTier(value);
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonuses Bonuses
        {
            get
            {
                return this._Bonuses;
            }
            set
            {
                this._Bonuses = value;
            }
        }
        public void SetTier(VIPTier tier)
        {
            if (tier == VIPTier.None)
            {
                foreach (Bonus bonus in this._Bonuses)
                {
                    bonus.Enabled = false;
                }
            }
            else if (tier == VIPTier.Bronze)
            {
                this._Bonuses[0].Enabled = true;
                this._Bonuses[1].Enabled = true;
                this._Bonuses[2].Enabled = true;
                this._Bonuses[3].Enabled = true;
                this._Bonuses[4].Enabled = true;
            }
            else if (tier == VIPTier.Silver)
            {
                this._Bonuses[5].Enabled = true;
                this._Bonuses[6].Enabled = true;
                this._Bonuses[7].Enabled = true;
                this._Bonuses[8].Enabled = true;
                this._Bonuses[9].Enabled = true;
            }
            else if (tier == VIPTier.Gold)
            {
                this._Bonuses[10].Enabled = true;
                this._Bonuses[11].Enabled = true;
                this._Bonuses[12].Enabled = true;
                this._Bonuses[13].Enabled = true;
                this._Bonuses[14].Enabled = true;
            }
        }

        public override void Prep()
        {
            base.Prep();

            this.Check();
        }

        public void Check()
        {
            if (!this.LinkedMobile.Deleted || this.LinkedMobile != null)
            {
                switch (this._Tier)
                {
                    case VIPTier.None:
                        {
                            foreach (Bonus bonus in this._Bonuses)
                            {
                                if (bonus.TimeStarted + bonus.ServicePeriod >= DateTime.UtcNow)
                                {
                                    bonus.Enabled = false;
                                    bonus.ServicePeriod = TimeSpan.Zero;
                                    bonus.TimeStarted = DateTime.MinValue;
                                }
                            }
                            break;
                        }
                    case VIPTier.Bronze:
                        {
                            if (this._TimeStarted + this._ServicePeriod >= DateTime.UtcNow)
                            {
                                this._TimeStarted = DateTime.MinValue;
                                this._ServicePeriod = TimeSpan.Zero;
                            }

                            this._Canceled = true;
                            this.LinkedMobile.AccessLevel = AccessLevel.Player;

                            goto case VIPTier.None;
                        }
                    case VIPTier.Silver:
                        {
                            goto case VIPTier.Bronze;
                        }
                    case VIPTier.Gold:
                        {
                            goto case VIPTier.Bronze;
                        }
                }
            }
            else
            {
                this._Canceled = true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 0);

            // Version 0
            writer.Write(this._Canceled);
            writer.Write(this._TimeStarted);
            writer.Write(this._ServicePeriod);
            this._Bonuses.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        this._Canceled = reader.ReadBool();
                        this._TimeStarted = reader.ReadDateTime();
                        this._ServicePeriod = reader.ReadTimeSpan();
                        this._Bonuses = new Bonuses(reader);
                        break;
                    }
            }
        }
    }
}