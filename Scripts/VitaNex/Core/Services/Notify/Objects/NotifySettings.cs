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
using System;
using System.Collections.Generic;

using Server;
using Server.Accounting;
#endregion

namespace VitaNex.Notify
{
	public sealed class NotifySettings : PropertyObject
	{
		public Dictionary<IAccount, NotifySettingsState> States { get; private set; }

		[CommandProperty(Notify.Access)]
		public Dictionary<IAccount, NotifySettingsState>.KeyCollection Keys
		{
			get => States.Keys;
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
			}
		}

		[CommandProperty(Notify.Access)]
		public Dictionary<IAccount, NotifySettingsState>.ValueCollection Values
		{
			get => States.Values;
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
			}
		}

		[CommandProperty(Notify.Access, true)]
		public Type Type { get; set; }

		[CommandProperty(Notify.Access)]
		public string Name { get; set; }

		[CommandProperty(Notify.Access)]
		public string Desc { get; set; }

		[CommandProperty(Notify.Access)]
		public AccessLevel Access { get; set; }

		[CommandProperty(Notify.Access)]
		public bool CanIgnore { get; set; }

		[CommandProperty(Notify.Access)]
		public bool CanAutoClose { get; set; }

		public NotifySettings(Type t)
		{
			Type = t;
			Name = t.Name;

			Desc = String.Empty;
			Access = AccessLevel.Player;

			CanIgnore = true;
			CanAutoClose = true;

			States = new Dictionary<IAccount, NotifySettingsState>();
		}

		public NotifySettings(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			States.Clear();
		}

		public override void Reset()
		{
			States.Clear();
		}

		public bool IsAutoClose(Mobile m)
		{
			return CanAutoClose && m != null && m.Account != null && States.ContainsKey(m.Account) && States[m.Account].AutoClose;
		}

		public bool IsIgnored(Mobile m)
		{
			return CanIgnore && m != null && m.Account != null && States.ContainsKey(m.Account) && States[m.Account].Ignore;
		}

		public bool IsTextOnly(Mobile m)
		{
			return m != null && m.Account != null && States.ContainsKey(m.Account) && States[m.Account].TextOnly;
		}

		public bool IsAnimated(Mobile m)
		{
			return m == null || m.Account == null || !States.ContainsKey(m.Account) || States[m.Account].Animate;
		}

		public void AlterTime(Mobile m, ref double value)
		{
			if (m == null || m.Account == null || !States.ContainsKey(m.Account) || value <= 0.0)
			{
				return;
			}

			var speed = Math.Max(0, Math.Min(200, (int)States[m.Account].Speed));

			if (speed > 100)
			{
				value -= value * ((speed - 100) / 100.0);
			}
			else if (speed < 100)
			{
				value += value * ((100 - speed) / 100.0);
			}
		}

		public NotifySettingsState EnsureState(IAccount a)
		{
			var s = States.GetValue(a);

			if (s == null || s.Settings != this)
			{
				States[a] = s = new NotifySettingsState(this, a);
			}

			return s;
		}

		public override string ToString()
		{
			var name = Name.Replace("Notify", String.Empty).Replace("Gump", String.Empty);

			if (String.IsNullOrWhiteSpace(name))
			{
				name = "Notifications";
			}

			return name;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.WriteType(Type);
			writer.Write(Name);
			writer.Write(Desc);

			writer.Write(CanIgnore);
			writer.Write(CanAutoClose);

			writer.WriteBlockDictionary(States, (w, k, v) => v.Serialize(w));
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Type = reader.ReadType();
			Name = reader.ReadString();
			Desc = reader.ReadString();

			CanIgnore = reader.ReadBool();
			CanAutoClose = reader.ReadBool();

			States = reader.ReadBlockDictionary(
				r =>
				{
					var state = new NotifySettingsState(this, r);

					return new KeyValuePair<IAccount, NotifySettingsState>(state.Owner, state);
				},
				States);
		}
	}
}