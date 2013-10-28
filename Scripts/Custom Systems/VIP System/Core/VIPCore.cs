using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;

namespace CustomsFramework.Systems.VIPSystem
{
    public partial class VIPCore : BaseCore
    {
        public const string SystemVersion = @"1.0";
        public List<VIPModule> _VIPModules;
        private TimeSpan _ServiceTimespan;
        private double _ExchangeRate;
        private int _GoldFee;
        private int _SilverFee;
        private int _BronzeFee;
        private int _GoldBonusFee;
        private int _SilverBonusFee;
        private int _BronzeBonusFee;
        public VIPCore()
            : base()
        {
            this.Enabled = false;

            this._ServiceTimespan = TimeSpan.FromDays(30.0);
            this._ExchangeRate = 0.1;

            this._GoldFee = 500;
            this._SilverFee = 250;
            this._BronzeFee = 100;

            this._GoldBonusFee = 250;
            this._SilverBonusFee = 125;
            this._BronzeBonusFee = 50;
        }

        public VIPCore(CustomSerial serial)
            : base(serial)
        {
        }

        public override string Name
        {
            get
            {
                return @"VIP Core";
            }
        }
        public override string Description
        {
            get
            {
                return @"Core that contains everything for the VIP system.";
            }
        }
        public override string Version
        {
            get
            {
                return SystemVersion;
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
                return null;
            }
        }
        public TimeSpan ServiceTimespan
        {
            get
            {
                return this._ServiceTimespan;
            }
            set
            {
                this._ServiceTimespan = value;
            }
        }
        public double ExchangeRate
        {
            get
            {
                return this._ExchangeRate;
            }
            set
            {
                this._ExchangeRate = value;
            }
        }
        public int GoldFee
        {
            get
            {
                return this._GoldFee;
            }
            set
            {
                this._GoldFee = value;
            }
        }
        public int SilverFee
        {
            get
            {
                return this._SilverFee;
            }
            set
            {
                this._SilverFee = value;
            }
        }
        public int BronzeFee
        {
            get
            {
                return this._BronzeFee;
            }
            set
            {
                this._BronzeFee = value;
            }
        }
        public int GoldBonusFee
        {
            get
            {
                return this._GoldBonusFee;
            }
            set
            {
                this._GoldBonusFee = value;
            }
        }
        public int SilverBonusFee
        {
            get
            {
                return this._SilverBonusFee;
            }
            set
            {
                this._SilverBonusFee = value;
            }
        }
        public int BronzeBonusFee
        {
            get
            {
                return this._BronzeBonusFee;
            }
            set
            {
                this._BronzeBonusFee = value;
            }
        }
        public static void Initialize()
        {
            //VIPCore core = World.GetCore(typeof(VIPCore)) as VIPCore;

            //if (core == null)
            //{
            //    core = new VIPCore();
            //    core.Prep();
            //}
        }

        public static void VIPHook_Login(LoginEventArgs e)
        {
            CheckModule(e.Mobile);
        }

        public static void VIPHook_Logout(LogoutEventArgs e)
        {
            CheckModule(e.Mobile);
        }

        public static void VIPHook_Disconnected(DisconnectedEventArgs e)
        {
            CheckModule(e.Mobile);
        }

        public override void Prep() // Called after all cores are loaded
        {
            Server.EventSink.Login += new LoginEventHandler(VIPHook_Login);
            Server.EventSink.Logout += new LogoutEventHandler(VIPHook_Logout);
            Server.EventSink.Disconnected += new DisconnectedEventHandler(VIPHook_Disconnected);
            CommandSystem.Register("VIP", AccessLevel.VIP, new CommandEventHandler(Command_VIP));
        }

        public int GetBalance(Mobile from)
        {
            int balance = 0;

            Container bank = from.FindBankNoCreate();
            Container backpack = from.Backpack;
            Item[] DonatorDeeds;

            if (bank != null)
            {
                DonatorDeeds = bank.FindItemsByType(typeof(DonatorDeed));

                for (int i = 0; i < DonatorDeeds.Length; i++)
                    balance += DonatorDeeds[i].Amount;
            }

            if (backpack != null)
            {
                DonatorDeeds = backpack.FindItemsByType(typeof(DonatorDeed));

                for (int i = 0; i < DonatorDeeds.Length; i++)
                    balance += DonatorDeeds[i].Amount;
            }

            return balance;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 0);

            // Version 0
            writer.Write(this._ServiceTimespan);
            writer.Write(this._ExchangeRate);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        this._ServiceTimespan = reader.ReadTimeSpan();
                        this._ExchangeRate = reader.ReadDouble();
                        break;
                    }
            }
        }

        private static void CheckModule(Mobile from)
        {
            VIPModule module = from.GetModule(typeof(VIPModule)) as VIPModule;

            if (module != null)
                module.Check();
        }
    }
}