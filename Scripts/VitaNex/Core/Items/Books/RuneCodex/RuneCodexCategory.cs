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
using System.Text;

using Server;
using Server.Items;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Items
{
	public class RuneCodexCategory : PropertyObject
	{
		public static string DefaultName = "Misc Places";
		public static string DefaultDescription = "A collection of places.";
		public static int DefaultHue = 85;

		private string _Name = DefaultName;
		private string _Description = DefaultDescription;
		private int _Hue = DefaultHue;

		[CommandProperty(AccessLevel.GameMaster)]
		public string Name { get => _Name; set => SetName(value); }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Description { get => _Description; set => SetDescription(value); }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Hue { get => _Hue; set => SetHue(value); }

		[CommandProperty(AccessLevel.GameMaster)]
		public RuneCodexEntryGrid Entries { get; set; }

		public RuneCodexCategory()
			: this(DefaultName, DefaultDescription, DefaultHue)
		{ }

		public RuneCodexCategory(string name, string desc, int hue)
		{
			Name = name;
			Description = desc;
			Hue = hue;
			Entries = new RuneCodexEntryGrid();
		}

		public RuneCodexCategory(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			UseDefaults();
			Empty();
		}

		public override void Reset()
		{
			UseDefaults();
			Empty();
		}

		public virtual void UseDefaults()
		{
			Name = DefaultName;
			Description = DefaultDescription;
			Hue = DefaultHue;
		}

		public virtual void Empty()
		{
			Entries.SetAllContent(null, e => e != null);
		}

		public void SetName(string name)
		{
			_Name = String.IsNullOrWhiteSpace(name) ? DefaultName : name;
		}

		public void SetDescription(string desc)
		{
			_Description = String.IsNullOrWhiteSpace(desc) ? DefaultDescription : desc;
		}

		public void SetHue(int hue)
		{
			_Hue = hue <= 0 ? DefaultHue : hue;
		}

		public bool AddRunebook(Mobile m, Runebook book, bool message)
		{
			if (m == null || m.Deleted || book == null || book.Deleted)
			{
				return false;
			}

			var mrb = Insensitive.Equals(book.GetType().Name, "InternalRunebook");

			if (book.Entries.Count == 0)
			{
				if (!mrb && message)
				{
					m.SendMessage("That rune book is empty.");
				}

				return false;
			}

			if (Entries.Count >= Entries.Capacity)
			{
				if (!mrb && message)
				{
					m.SendMessage("The category \"{0}\" can't hold more runes.", _Name);
				}

				return false;
			}

			if (Entries.Count + book.Entries.Count > Entries.Capacity)
			{
				if (!mrb && message)
				{
					m.SendMessage("That rune book won't fit in the category \"{0}\".", _Name);
				}

				return false;
			}

			var bEntries = new Queue<RunebookEntry>(book.Entries);

			Entries.ForEach(
				(x, y, e) =>
				{
					if (e != null || bEntries.Count <= 0)
					{
						return;
					}

					var be = bEntries.Dequeue();

					Entries.SetContent(x, y, new RuneCodexEntry(book.Name, be.Description, be.Location.ToMapPoint(be.Map)));
				});

			if (mrb)
			{
				book.Entries.Clear();
				return true;
			}

			book.Delete();

			if (message)
			{
				m.SendMessage("You add the rune book to the category \"{0}\".", _Name);
			}

			return true;
		}

		public bool SetRune(Mobile m, RecallRune rune, Point2D loc, bool message)
		{
			if (m == null || m.Deleted || rune == null || rune.Deleted)
			{
				return false;
			}

			if (!rune.Marked || rune.Target == Point3D.Zero || rune.TargetMap == Map.Internal)
			{
				if (message)
				{
					m.SendMessage("That rune is blank.");
				}

				return false;
			}

			if (Entries.Count >= Entries.Capacity)
			{
				if (message)
				{
					m.SendMessage("The category \"{0}\" can't hold more runes.", _Name);
				}

				return false;
			}

			if (Entries[loc.X, loc.Y] != null)
			{
				return AddRune(m, rune, message);
			}

			Entries.SetContent(
				loc.X,
				loc.Y,
				new RuneCodexEntry(rune.Description, rune.Name, rune.Target.ToMapPoint(rune.TargetMap)));
			rune.Delete();

			if (message)
			{
				m.SendMessage("You add the rune to the category \"{0}\".", _Name);
			}

			return true;
		}

		public bool AddRune(Mobile m, RecallRune rune, bool message)
		{
			if (m == null || m.Deleted || rune == null || rune.Deleted)
			{
				return false;
			}

			if (!rune.Marked || rune.Target == Point3D.Zero || rune.TargetMap == Map.Internal)
			{
				if (message)
				{
					m.SendMessage("That rune is blank.");
				}

				return false;
			}

			if (Entries.Count >= Entries.Capacity)
			{
				if (message)
				{
					m.SendMessage("The category \"{0}\" can't hold more runes.", _Name);
				}

				return false;
			}

			Entries.ForEach(
				(x, y, e) =>
				{
					if (e != null || rune.Deleted)
					{
						return;
					}

					Entries.SetContent(x, y, new RuneCodexEntry(rune.Description, rune.Name, rune.Target.ToMapPoint(rune.TargetMap)));
					rune.Delete();
				});

			if (message)
			{
				m.SendMessage("You add the rune to the rune codex category \"{0}\".", _Name);
			}

			return true;
		}

		public bool Remove(RuneCodexEntry entry)
		{
			for (var x = 0; x < Entries.Width; x++)
			{
				for (var y = 0; y < Entries.Height; y++)
				{
					if (Entries[x, y] != entry)
					{
						continue;
					}

					Entries[x, y] = null;
					return true;
				}
			}

			return false;
		}

		public virtual string ToHtmlString(Mobile viewer = null)
		{
			var html = new StringBuilder();

			GetHtmlString(html, viewer);

			return html.ToString();
		}

		public virtual void GetHtmlString(StringBuilder html, Mobile viewer = null)
		{
			html.Append("".WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));

			html.AppendLine("Entries: {0:#,0} / {1:#,0}", Entries.Count, Entries.Capacity);
			html.AppendLine();

			if (!String.IsNullOrWhiteSpace(Description))
			{
				html.AppendLine(Description.WrapUOHtmlColor(Color.LawnGreen, SuperGump.DefaultHtmlColor));
			}

			html.Append("".WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(_Name);
					writer.Write(_Description);
					writer.Write(_Hue);

					Entries.Serialize(writer);
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
				case 0:
				{
					Name = reader.ReadString();
					Description = reader.ReadString();
					Hue = reader.ReadInt();

					Entries = new RuneCodexEntryGrid(reader);
				}
				break;
			}
		}
	}
}