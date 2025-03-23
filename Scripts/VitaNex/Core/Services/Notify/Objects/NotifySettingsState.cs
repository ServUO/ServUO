#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using Server;
using Server.Accounting;
#endregion

namespace VitaNex.Notify
{
	public sealed class NotifySettingsState : SettingsObject<NotifyFlags>
	{
		[CommandProperty(Notify.Access)]
		public NotifySettings Settings { get; private set; }

		[CommandProperty(Notify.Access, true)]
		public IAccount Owner { get; private set; }

		[CommandProperty(Notify.Access)]
		public override NotifyFlags Flags { get => base.Flags; set => base.Flags = value; }

		[CommandProperty(Notify.Access)]
		public bool AutoClose
		{
			get => GetFlag(NotifyFlags.AutoClose);
			set => SetFlag(NotifyFlags.AutoClose, value);
		}

		[CommandProperty(Notify.Access)]
		public bool Ignore { get => GetFlag(NotifyFlags.Ignore); set => SetFlag(NotifyFlags.Ignore, value); }

		[CommandProperty(Notify.Access)]
		public bool TextOnly { get => GetFlag(NotifyFlags.TextOnly); set => SetFlag(NotifyFlags.TextOnly, value); }

		[CommandProperty(Notify.Access)]
		public bool Animate { get => GetFlag(NotifyFlags.Animate); set => SetFlag(NotifyFlags.Animate, value); }

		[CommandProperty(Notify.Access)]
		public byte Speed { get; set; }

		public NotifySettingsState(NotifySettings settings, IAccount owner)
		{
			Settings = settings;
			Owner = owner;

			SetDefaults();
		}

		public NotifySettingsState(NotifySettings settings, GenericReader reader)
			: base(reader)
		{
			Settings = settings;
		}

		public override void Clear()
		{
			SetDefaults();
		}

		public override void Reset()
		{
			SetDefaults();
		}

		public void SetDefaults()
		{
			AutoClose = false;
			Ignore = false;
			TextOnly = false;
			Animate = true;

			Speed = 100;
		}

		public override string ToString()
		{
			if (Owner != null)
			{
				if (Settings != null)
				{
					return Owner + ": " + Settings;
				}

				return Owner.ToString();
			}

			if (Settings != null)
			{
				return Settings.ToString();
			}

			return base.ToString();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Owner);

			writer.Write(Speed);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Owner = reader.ReadAccount();

			Speed = reader.ReadByte();
		}
	}
}