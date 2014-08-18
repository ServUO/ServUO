using System;
using Server;

namespace CustomsFramework.Systems.VIPSystem
{
    public class BaseVIPDeed : Item
    {
        private Bonuses _Bonuses;
        private VIPTier _Tier;
        public BaseVIPDeed()
            : base(0x14F0)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
            this._Tier = VIPTier.None;
            this._Bonuses = new Bonuses();

            for (int i = 0; i < this._Bonuses.Length; i++)
            {
                this._Bonuses[i].Enabled = false;
            }
        }

        public BaseVIPDeed(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "A VIP Deed";
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
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            VIPModule module = from.GetModule(typeof(VIPModule)) as VIPModule;

            if (module == null || module.Canceled)
            {
                if (this.Tier == VIPTier.None)
                {
                    if (this.Bonuses != null)
                    {
                        module = new VIPModule(from, this);
                        this.DonatorMessage(from);
                    }
                    else
                    {
                        from.SendMessage("There is something wrong with your deed, please report to a staff member.");
                        this.Name = "Error Deed";
                    }
                }
                else
                {
                    if (this.Tier == VIPTier.Gold)
                    {
                        module = new VIPModule(from, this);
                        from.AccessLevel = AccessLevel.VIP;
                        from.SendMessage("Thanks for donating to become a Gold VIP player!");
                        module.Bonuses.StartBonuses();
                    }
                    else if (this.Tier == VIPTier.Silver)
                    {
                        module = new VIPModule(from, this);
                        from.AccessLevel = AccessLevel.VIP;
                        from.SendMessage("Thanks for donating to become a Silver VIP player!");
                        module.Bonuses.StartBonuses();
                    }
                    else if (this.Tier == VIPTier.Bronze)
                    {
                        module = new VIPModule(from, this);
                        from.AccessLevel = AccessLevel.VIP;
                        from.SendMessage("Thanks for donating to become a Bronze VIP player!");
                        module.Bonuses.StartBonuses();
                    }
                    this.Delete();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 0);

            writer.Write((byte)this._Tier);
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
                        this._Tier = (VIPTier)reader.ReadByte();
                        this._Bonuses = new Bonuses(reader);
                        break;
                    }
            }
        }

        private void DonatorMessage(Mobile from)
        {
            VIPCore core = World.GetCore(typeof(VIPCore)) as VIPCore;
			
			if (core != null)
			{
				from.SendMessage("Thank you for donating and helping make this a better place!");
				from.SendMessage("You have been given your VIP Bonuses, they will expire in {0}.", core.ServiceTimespan.TotalDays);
				from.SendMessage("Use [VIP to view your VIP bonuses and settings.");
			}
        }
    }
}