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
using System.Linq;

using Server;
using Server.Gumps;

using VitaNex.Text;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class OPLGump : SuperGump
	{
		private readonly IconDefinition _DefIcon = IconDefinition.FromItem(1, 0);
		private readonly string[] _DefProperties = { "Unknown Object" };

		public IEntity Entity { get; set; }

		public IconDefinition Icon { get; set; }
		public string[] Properties { get; set; }

		public OPLGump(Mobile user, Gump parent = null)
			: base(user, parent)
		{
			CanClose = true;
			CanDispose = true;
			CanMove = true;
			CanResize = false;
		}

		public OPLGump(Mobile user, IEntity entity, Gump parent = null)
			: this(user, parent)
		{
			Entity = entity;
		}

		public OPLGump(Mobile user, int itemID, string name, Gump parent = null)
			: this(user, itemID, 0, name, parent)
		{ }

		public OPLGump(Mobile user, int itemID, int hue, string name, Gump parent = null)
			: this(user, itemID, hue, new[] { name }, parent)
		{ }

		public OPLGump(Mobile user, int itemID, string[] props, Gump parent = null)
			: this(user, itemID, 0, props, parent)
		{ }

		public OPLGump(Mobile user, int itemID, int hue, string[] props, Gump parent = null)
			: this(user, parent)
		{
			Icon = IconDefinition.FromItem(itemID, hue);

			Properties = props;
		}

		protected override void Compile()
		{
			if (Entity != null)
			{
				var id = 0;

				if (Entity is Mobile)
				{
					id = ShrinkTable.Lookup((Mobile)Entity);
				}
				else if (Entity is Item)
				{
					id = ((Item)Entity).ItemID;
				}


				if (Entity.GetPropertyValue("Hue", out
				int hue))
				{
					Icon = IconDefinition.FromItem(id, hue);
				}
				else
				{
					Icon = IconDefinition.FromItem(id);
				}

				Properties = Entity.GetOPLStrings(User).ToArray();
			}

			if (Icon == null || Icon.IsEmpty)
			{
				Icon = _DefIcon;
			}

			if (Properties.IsNullOrEmpty())
			{
				Properties = _DefProperties;
			}

			base.Compile();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var sup = SupportsUltimaStore;
			var ec = IsEnhancedClient;
			var pad = sup ? 15 : 10;
			var pad2 = pad * 2;
			var bgID = ec ? 83 : sup ? 40000 : 5054;

			var size = Icon.Size;

			var innerHeight = UOFont.GetUnicodeHeight(1, Properties);

			var outerHeight = innerHeight + pad2;

			if (outerHeight < size.Height + pad2)
			{
				outerHeight = size.Height + pad2;
				innerHeight = outerHeight - pad2;
			}

			layout.Add(
				"bg",
				() =>
				{
					AddBackground(0, 0, size.Width + pad2, size.Height + pad2, bgID);
					AddBackground(size.Width + pad2, 0, 400, outerHeight, bgID);

					if (!ec)
					{
						AddImageTiled(pad, pad, size.Width, size.Height, 2624);
						AddAlphaRegion(pad, pad, size.Width, size.Height);

						AddImageTiled(size.Width + pad2 + pad, pad, 400 - pad2, innerHeight, 2624);
						AddAlphaRegion(size.Width + pad2 + pad, pad, 400 - pad2, innerHeight);
					}
				});

			layout.Add(
				"info",
				() =>
				{
					Icon.AddToGump(this, pad, pad);

					var lines = Properties.Select((s, i) => i == 0 ? s.ToUpperWords().WrapUOHtmlColor(Color.Yellow) : s);
					var value = String.Join("\n", lines);

					value = value.ToUpperWords();
					value = value.WrapUOHtmlCenter();
					value = value.WrapUOHtmlColor(Color.White, false);

					AddHtml(size.Width + pad2 + pad, pad, 400 - pad2, innerHeight, value, false, false);
				});
		}
	}
}