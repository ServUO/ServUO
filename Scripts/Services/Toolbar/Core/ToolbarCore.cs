#region References
using System;

using CustomsFramework;

using Server;
using Server.Commands;
using Server.Gumps;

using Services.Toolbar.Gumps;
#endregion

namespace Services.Toolbar.Core
{
	public class ToolbarCore : BaseCore
	{
		public const string SystemVersion = "2.3";
		public const string ReleaseDate = "October 28, 2013";

		public static ToolbarCore Instance { get; private set; }

		public static void Initialize()
		{
			Instance = World.GetCore(typeof(ToolbarCore)) as ToolbarCore ?? new ToolbarCore();

			CommandHandlers.Register("Toolbar", AccessLevel.Counselor, Toolbar_OnCommand);

			EventSink.Login += OnLogin;
			EventSink.PlayerDeath += OnPlayerDeath;
		}

		private static void OnLogin(LoginEventArgs e)
		{
			if (e.Mobile.AccessLevel >= AccessLevel.Counselor)
			{
				SendToolbar(e.Mobile);
			}
		}

		public static void OnPlayerDeath(PlayerDeathEventArgs e)
		{
			if (e.Mobile.AccessLevel < AccessLevel.Counselor)
			{
				return;
			}

			e.Mobile.CloseGump(typeof(ToolbarGump));

			Timer.DelayCall(TimeSpan.FromSeconds(2.0), SendToolbar, e.Mobile);
		}

		[Usage("Toolbar")]
		public static void Toolbar_OnCommand(CommandEventArgs e)
		{
			SendToolbar(e.Mobile);
		}

		public static void SendToolbar(Mobile m)
		{
			ToolbarModule module = m.GetModule(typeof(ToolbarModule)) as ToolbarModule ?? new ToolbarModule(m);

			m.CloseGump(typeof(ToolbarGump));
            m.SendGump(new ToolbarGump(module.ToolbarInfo, m));
		}

		public ToolbarCore()
		{
			Enabled = true;
		}

		public ToolbarCore(CustomSerial serial)
			: base(serial)
		{ }

		public override string Name { get { return @"Toolbar Core"; } }
		public override string Description { get { return @"Core that maintains the [Toolbar system."; } }
		public override string Version { get { return SystemVersion; } }
		public override AccessLevel EditLevel { get { return AccessLevel.Developer; } }
		public override Gump SettingsGump { get { return null; } }


		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}