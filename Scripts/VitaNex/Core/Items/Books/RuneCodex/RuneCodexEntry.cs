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
using System.Text;

using Server;
using Server.Items;
using Server.Spells.Fourth;
using Server.Spells.Seventh;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Items
{
	public class RuneCodexEntry : PropertyObject
	{
		public static string DefaultName = "Unknown Place";
		public static string DefaultDescription = String.Empty;

		private string _Name = DefaultName;
		private string _Description = DefaultDescription;
		private MapPoint _Location = MapPoint.Empty;

		[CommandProperty(AccessLevel.GameMaster)]
		public string Name { get => _Name; set => SetName(value); }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Description { get => _Description; set => SetDescription(value); }

		[CommandProperty(AccessLevel.GameMaster)]
		public MapPoint Location { get => _Location; set => SetLocation(value); }

		public RuneCodexEntry()
			: this(DefaultName, DefaultDescription, MapPoint.Empty)
		{ }

		public RuneCodexEntry(string name, string desc, MapPoint dest)
		{
			Name = name ?? DefaultName;
			Description = desc ?? DefaultDescription;
			Location = dest;
		}

		public RuneCodexEntry(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			UseDefaults();
			Location = MapPoint.Empty;
		}

		public override void Reset()
		{
			UseDefaults();
			Location = MapPoint.Empty;
		}

		public virtual void UseDefaults()
		{
			Name = DefaultName;
			Description = DefaultDescription;
		}

		public void SetName(string name)
		{
			_Name = String.IsNullOrWhiteSpace(name) ? DefaultName : name;
		}

		public void SetDescription(string desc)
		{
			_Description = String.IsNullOrWhiteSpace(desc) ? DefaultDescription : desc;
		}

		public void SetLocation(MapPoint dest)
		{
			_Location = dest;
		}

		public bool Recall(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (_Location.Internal || _Location.Zero)
			{
				m.SendMessage("That rune is blank.");
				return false;
			}

			if (m.Player)
			{
				var t = GetType();

				if (m.AccessLevel < AccessLevel.Counselor && !m.CanBeginAction(t))
				{
					m.SendMessage("You must wait before using the rune codex again.");
					return false;
				}

				var spell = new RecallSpell(
					m,
					null,
					new RunebookEntry(_Location.Location, _Location.Map, _Description, null),
					null);

				if (!spell.Cast())
				{
					return false;
				}

				m.BeginAction(t, TimeSpan.FromSeconds(3));
			}
			else
			{
				_Location.MoveToWorld(m);
			}

			return true;
		}

		public bool Gate(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (_Location.Internal || _Location.Zero)
			{
				m.SendMessage("That rune is blank.");
				return false;
			}

			if (m.Player)
			{
				var t = GetType();

				if (m.AccessLevel < AccessLevel.Counselor && !m.CanBeginAction(t))
				{
					m.SendMessage("You must wait before using the rune codex again.");
					return false;
				}

				var spell = new GateTravelSpell(m, null, new RunebookEntry(_Location.Location, _Location.Map, _Description, null));

				if (!spell.Cast())
				{
					return false;
				}

				m.BeginAction(t, TimeSpan.FromSeconds(3));
			}
			else
			{
				_Location.MoveToWorld(m);
			}

			return true;
		}

		public virtual string ToHtmlString(Mobile viewer = null)
		{
			var html = new StringBuilder();

			GetHtmlString(html);

			return html.ToString();
		}

		public virtual void GetHtmlString(StringBuilder html, Mobile viewer = null)
		{
			html.Append("".WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));

			html.AppendLine("Facet: {0}", Location.Map);
			html.AppendLine("Location: {0}, {1}, {2}", Location.X, Location.Y, Location.Z);

			int xLong = 0, yLat = 0;
			int xMins = 0, yMins = 0;
			bool xEast = false, ySouth = false;

			if (Sextant.Format(Location, Location.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
			{
				html.AppendLine(
					"Coords: {0}° {1}'{2}, {3}° {4}'{5}",
					yLat,
					yMins,
					ySouth ? "S" : "N",
					xLong,
					xMins,
					xEast ? "E" : "W");
			}

			if (!String.IsNullOrWhiteSpace(Description))
			{
				html.AppendLine();
				html.AppendLine(Description.WrapUOHtmlColor(Color.LawnGreen, SuperGump.DefaultHtmlColor));
				html.AppendLine();
			}

			html.Append("".WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.Write(_Name);
					goto case 0;
				case 0:
				{
					writer.Write(_Description);
					_Location.Serialize(writer);
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
					_Name = reader.ReadString();
					goto case 0;
				case 0:
				{
					_Description = reader.ReadString();
					_Location = new MapPoint(reader);
				}
				break;
			}
		}
	}
}