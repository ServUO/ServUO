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
using System.Drawing;

using Server;
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Toolbar
{
	public class ToolbarLink : ToolbarEntry, IEquatable<ToolbarLink>
	{
		public ToolbarLink()
		{ }

		public ToolbarLink(
			string url,
			string label = null,
			bool canDelete = true,
			bool canEdit = true,
			bool highlight = false,
			Color? labelColor = null)
			: base(url, label, canDelete, canEdit, highlight, labelColor)
		{ }

		public ToolbarLink(GenericReader reader)
			: base(reader)
		{ }

		public override string GetDisplayLabel()
		{
			return "<u>" + base.GetDisplayLabel() + "</u>";
		}

		protected override void CompileOptions(ToolbarGump toolbar, GumpButton clicked, Point loc, MenuGumpOptions opts)
		{
			if (toolbar == null)
			{
				return;
			}

			base.CompileOptions(toolbar, clicked, loc, opts);

			var user = toolbar.State.User;

			if (!CanEdit && user.AccessLevel < Toolbars.Access)
			{
				return;
			}

			opts.Replace(
				"Set Value",
				new ListGumpEntry(
					"Set URL",
					b => new InputDialogGump(user, toolbar)
					{
						Title = "Set URL",
						Html = "Set the URL for this Link entry.",
						InputText = Value,
						Callback = (cb, text) =>
						{
							Value = text;
							toolbar.Refresh(true);
						}
					}.Send(),
					toolbar.HighlightHue));
		}

		public override bool ValidateState(ToolbarState state)
		{
			return base.ValidateState(state) && !String.IsNullOrWhiteSpace(Value);
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
				() => user.LaunchBrowser(FullValue),
				ex =>
				{
					Console.WriteLine("{0} => {1} => ({2}) => {3}", user, GetType().Name, FullValue, ex);
					Toolbars.CMOptions.ToConsole(ex);
				});
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is ToolbarLink && Equals((ToolbarLink)obj);
		}

		public bool Equals(ToolbarLink other)
		{
			return !ReferenceEquals(other, null) && base.Equals(other);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}

		public static bool operator ==(ToolbarLink l, ToolbarLink r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(ToolbarLink l, ToolbarLink r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}