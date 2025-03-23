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
using System.Drawing;
using System.Linq;

using Server;
using Server.Commands;
using Server.Gumps;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Toolbar
{
	public class ToolbarCommand : ToolbarEntry, IEquatable<ToolbarCommand>
	{
		[CommandProperty(Toolbars.Access)]
		public AccessLevel? MinAccess { get; set; }

		public List<string> Args { get; set; }

		[CommandProperty(Toolbars.Access)]
		public override string FullValue => String.Format(
					"{0}{1} {2}",
					CommandSystem.Prefix,
					base.FullValue,
					String.Join(" ", Args ?? (Args = new List<string>())));

		public ToolbarCommand()
		{
			Args = new List<string>();
		}

		public ToolbarCommand(
			string cmd,
			string label = null,
			bool canDelete = true,
			bool canEdit = true,
			bool highlight = false,
			Color? labelColor = null,
			AccessLevel? minAccess = null,
			params string[] args)
			: base(cmd, label, canDelete, canEdit, highlight, labelColor)
		{
			MinAccess = minAccess;
			Args = args != null && args.Length > 0 ? new List<string>(args) : new List<string>();
		}

		public ToolbarCommand(GenericReader reader)
			: base(reader)
		{ }

		protected override void CompileOptions(ToolbarGump toolbar, GumpButton clicked, Point loc, MenuGumpOptions opts)
		{
			if (toolbar == null)
			{
				return;
			}

			base.CompileOptions(toolbar, clicked, loc, opts);

			var user = toolbar.User;

			if (!CanEdit && user.AccessLevel < Toolbars.Access)
			{
				return;
			}

			opts.Replace(
				"Set Value",
				new ListGumpEntry(
					"Set Command",
					b => SuperGump.Send(
						new InputDialogGump(user, toolbar)
						{
							Title = "Set Command",
							Html = "Set the command for this Command entry.",
							InputText = Value,
							Callback = (cb, text) =>
							{
								Value = text;
								toolbar.Refresh(true);
							}
						}),
					toolbar.HighlightHue));

			opts.AppendEntry(
				new ListGumpEntry(
					"Set Args",
					b => SuperGump.Send(
						new InputDialogGump(user, toolbar)
						{
							Title = "Set Command Arguments",
							Html = "Set the Arguments for this Command entry.\nSeparate your entries with a semi-colon- ;",
							InputText = String.Join(";", Args),
							Callback = (cb, text) =>
							{
								Args.Clear();
								Args.AddRange(text.Split(';'));
								toolbar.Refresh(true);
							}
						}),
					toolbar.HighlightHue));
		}

		public override bool ValidateState(ToolbarState state)
		{
			if (!base.ValidateState(state))
			{
				return false;
			}

			var user = state.User;

			if (MinAccess != null && user.AccessLevel < MinAccess)
			{
				return false;
			}

			if (String.IsNullOrEmpty(Value))
			{
				return false;
			}

			if (!CommandSystem.Entries.TryGetValue(Value, out var cmd))
			{
				return false;
			}

			if (cmd == null || cmd.AccessLevel > (MinAccess ?? user.AccessLevel))
			{
				return false;
			}

			return true;
		}

		protected override void OnCloned(ToolbarEntry clone)
		{
			base.OnCloned(clone);

			var cmd = clone as ToolbarCommand;

			if (cmd == null)
			{
				return;
			}

			cmd.Args = (Args != null && Args.Count > 0) ? Args.ToList() : new List<string>();
			cmd.MinAccess = MinAccess;
		}

		public override void Invoke(ToolbarState state)
		{
			if (state == null)
			{
				return;
			}

			var user = state.User;

			if (user == null || user.Deleted || user.NetState == null)
			{
				return;
			}

			VitaNexCore.TryCatch(
				() => CommandSystem.Handle(user, FullValue),
				ex =>
				{
					Console.WriteLine("{0} => {1} => ({2}) => {3}", user, GetType().Name, FullValue, ex);
					Toolbars.CMOptions.ToConsole(ex);
				});
		}

		public override void Reset(ToolbarGump toolbar)
		{
			base.Reset(toolbar);

			if (toolbar == null)
			{
				return;
			}

			var user = toolbar.User;

			if (CanEdit || user.AccessLevel >= Toolbars.Access)
			{
				Args.Clear();
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^
						   (Args != null ? Args.Aggregate(Args.Count, (c, h) => (c * 397) ^ h.GetHashCode()) : 0);
				hashCode = (hashCode * 397) ^ (MinAccess.HasValue ? MinAccess.Value.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ToolbarCommand && Equals((ToolbarCommand)obj);
		}

		public bool Equals(ToolbarCommand other)
		{
			return !ReferenceEquals(other, null) && base.Equals(other) && MinAccess == other.MinAccess &&
				   String.Join("", Args) == String.Join("", other.Args);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				case 0:
				{
					writer.Write((MinAccess != null) ? (int)MinAccess : -1);

					if (version > 0)
					{
						writer.WriteList(Args, writer.Write);
					}
					else
					{
						writer.Write(Args.Count);
						Args.ForEach(writer.Write);
					}
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 1:
				case 0:
				{
					var minAccess = reader.ReadInt();
					MinAccess = (minAccess < 0) ? null : (AccessLevel?)minAccess;

					if (version > 0)
					{
						Args = reader.ReadList(reader.ReadString);
					}
					else
					{
						var count = reader.ReadInt();

						Args = new List<string>(count);

						for (var i = 0; i < count; i++)
						{
							Args.Add(reader.ReadString());
						}
					}
				}
				break;
			}
		}

		public static bool operator ==(ToolbarCommand l, ToolbarCommand r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(ToolbarCommand l, ToolbarCommand r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}