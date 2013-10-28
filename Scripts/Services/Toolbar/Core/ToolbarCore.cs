using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using CustomsFramework;

namespace Services.Toolbar.Core
{
    public partial class ToolbarCore : BaseCore
    {
        public const string SystemVersion = "2.2";
        public const string ReleaseDate = "February 17, 2013";

        public static void Initialize()
        {
            ToolbarCore core = World.GetCore(typeof(ToolbarCore)) as ToolbarCore;

            if (core == null)
                core = new ToolbarCore();

            CommandHandlers.Register("Toolbar", AccessLevel.VIP, Toolbar_OnCommand);
            EventSink.Login += OnLogin;
            EventSink.PlayerDeath += OnPlayerDeath;
        }

        public ToolbarCore()
        {
            this.Enabled = true;
        }

        public ToolbarCore(CustomSerial serial)
            : base(serial)
        {
        }

        public override string Name
        {
            get
            {
                return @"Toolbar Core";
            }
        }

        public override string Description
        {
            get
            {
                return @"Core that maintains the [Toolbar system.";
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

        private static void OnLogin(LoginEventArgs e)
        {
            if (e.Mobile.AccessLevel >= AccessLevel.VIP)
            {
                SendToolbar(e.Mobile);
            }
        }

        public static void OnPlayerDeath(PlayerDeathEventArgs e)
        {
            if (e.Mobile.AccessLevel >= AccessLevel.VIP)
            {
                e.Mobile.CloseGump(typeof(Gumps.ToolbarGump));
                object[] arg = new object[] { e.Mobile };
                Timer.DelayCall(TimeSpan.FromSeconds(2.0), new TimerStateCallback(SendToolbar), arg);
            }
        }

        [Usage("Toolbar")]
        public static void Toolbar_OnCommand(CommandEventArgs e)
        {
            SendToolbar(e.Mobile);
        }

        public static void SendToolbar(Mobile from)
        {
            ToolbarModule module = @from.GetModule(typeof(ToolbarModule)) as ToolbarModule ?? new ToolbarModule(@from);

            from.CloseGump(typeof (Gumps.ToolbarGump));
            from.SendGump(new Gumps.ToolbarGump(module.ToolbarInfo));
        }

        public static void SendToolbar(object state)
        {
            object[] states = (object[])state;

            Mobile m = (Mobile)states[0];
            SendToolbar(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
