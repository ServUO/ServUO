#region Header
// **********
// ServUO - ToolbarInfo.cs
// **********
#endregion

#region References
using System.Collections.Generic;

using CustomsFramework;

using Server;
using Server.Commands;
#endregion

namespace Services.Toolbar.Core
{
	public class ToolbarInfo
	{
		private int _Font, _Skin;
		private bool _Phantom, _Stealth, _Reverse, _Lock;
		private Point2D _Dimensions;
		private List<string> _Entries = new List<string>();
		private List<Point3D> _Points = new List<Point3D>();

		public ToolbarInfo(
			Point2D dimensions, List<string> entries, int skin, List<Point3D> points, int font, bool[] switches)
		{
			_Dimensions = dimensions;
			_Entries = entries;
			_Skin = skin;
			_Points = points;
			_Font = font;
			_Phantom = switches[0];
			_Stealth = switches[1];
			_Reverse = switches[2];
			_Lock = switches[3];
		}

		public int Font { get { return _Font; } set { _Font = value; } }
		public int Skin { get { return _Skin; } set { _Skin = value; } }
		public bool Phantom { get { return _Phantom; } set { _Phantom = value; } }
		public bool Stealth { get { return _Stealth; } set { _Stealth = value; } }
		public bool Reverse { get { return _Reverse; } set { _Reverse = value; } }
		public bool Lock { get { return _Lock; } set { _Lock = value; } }
		public int Rows { get { return _Dimensions.X; } set { _Dimensions.X = value; } }
		public int Collumns { get { return _Dimensions.Y; } set { _Dimensions.Y = value; } }
		public List<string> Entries { get { return _Entries; } set { _Entries = value; } }
		public List<Point3D> Points { get { return _Points; } set { _Points = value; } }

		public static ToolbarInfo CreateNew(Mobile from)
		{
			Point2D dimensions = DefaultDimensions(from.AccessLevel);
			var entries = DefaultEntries(from.AccessLevel);
			var points = new List<Point3D>();

			for (int i = entries.Count; i <= 135; i++)
			{
				entries.Add("-*UNUSED*-");
			}

			return new ToolbarInfo(dimensions, entries, 0, points, 0, new[] {true, false, false, true});
		}

		public static List<string> DefaultEntries(AccessLevel level)
		{
			var entries = new List<string>();

			switch (level)
			{
				case AccessLevel.Player:
					{
						break;
					}
				case AccessLevel.VIP:
					{
						break;
					}
				case AccessLevel.Counselor:
					{
						entries.Add(CommandSystem.Prefix + "GMBody");
						entries.Add(CommandSystem.Prefix + "StaffRunebook");
						entries.Add(CommandSystem.Prefix + "SpeedBoost");
						entries.Add(CommandSystem.Prefix + "M Tele");
						entries.Add(CommandSystem.Prefix + "Where");
						entries.Add(CommandSystem.Prefix + "Who");

						break;
					}
				case AccessLevel.Decorator:
					{
						entries.Add(CommandSystem.Prefix + "GMBody");
						entries.Add(CommandSystem.Prefix + "StaffRunebook");
						entries.Add(CommandSystem.Prefix + "SpeedBoost");
						entries.Add(CommandSystem.Prefix + "M Tele");
						entries.Add(CommandSystem.Prefix + "Where");
						entries.Add(CommandSystem.Prefix + "Who");

						for (int j = 0; j < 3; j++)
						{
							entries.Add("-*UNUSED*-");
						}

						entries.Add(CommandSystem.Prefix + "Add");
						entries.Add(CommandSystem.Prefix + "Remove");
						entries.Add(CommandSystem.Prefix + "Move");
						entries.Add(CommandSystem.Prefix + "ShowArt");
						entries.Add(CommandSystem.Prefix + "Get ItemID");
						entries.Add(CommandSystem.Prefix + "Get Hue");

						break;
					}
				case AccessLevel.Spawner:
					{
						entries.Add(CommandSystem.Prefix + "GMBody");
						entries.Add(CommandSystem.Prefix + "StaffRunebook");
						entries.Add(CommandSystem.Prefix + "SpeedBoost");
						entries.Add(CommandSystem.Prefix + "M Tele");
						entries.Add(CommandSystem.Prefix + "Where");
						entries.Add(CommandSystem.Prefix + "Who");

						for (int j = 0; j < 3; j++)
						{
							entries.Add("-*UNUSED*-");
						}

						entries.Add(CommandSystem.Prefix + "Add");
						entries.Add(CommandSystem.Prefix + "Remove");
						entries.Add(CommandSystem.Prefix + "XmlAdd");
						entries.Add(CommandSystem.Prefix + "XmlFind");
						entries.Add(CommandSystem.Prefix + "XmlShow");
						entries.Add(CommandSystem.Prefix + "XmlHide");

						break;
					}
				case AccessLevel.Seer:
				case AccessLevel.GameMaster:
					{
						entries.Add(CommandSystem.Prefix + "GMBody");
						entries.Add(CommandSystem.Prefix + "StaffRunebook");
						entries.Add(CommandSystem.Prefix + "SpeedBoost");
						entries.Add(CommandSystem.Prefix + "M Tele");
						entries.Add(CommandSystem.Prefix + "Where");
						entries.Add(CommandSystem.Prefix + "Who");

						for (int j = 0; j < 3; j++)
						{
							entries.Add("-*UNUSED*-");
						}

						entries.Add(CommandSystem.Prefix + "Add");
						entries.Add(CommandSystem.Prefix + "Remove");
						entries.Add(CommandSystem.Prefix + "Props");
						entries.Add(CommandSystem.Prefix + "Move");
						entries.Add(CommandSystem.Prefix + "Kill");
						entries.Add(CommandSystem.Prefix + "Follow");

						break;
					}
				case AccessLevel.Administrator:
                case AccessLevel.Developer:
                case AccessLevel.CoOwner:
                case AccessLevel.Owner:
					{
						entries.Add(CommandSystem.Prefix + "Admin");
						entries.Add(CommandSystem.Prefix + "StaffRunebook");
						entries.Add(CommandSystem.Prefix + "SpeedBoost");
						entries.Add(CommandSystem.Prefix + "M Tele");
						entries.Add(CommandSystem.Prefix + "Where");
						entries.Add(CommandSystem.Prefix + "Who");

						for (int j = 0; j < 3; j++)
						{
							entries.Add("-*UNUSED*-");
						}

						entries.Add(CommandSystem.Prefix + "Props");
						entries.Add(CommandSystem.Prefix + "Move");
						entries.Add(CommandSystem.Prefix + "Add");
						entries.Add(CommandSystem.Prefix + "Remove");
						entries.Add(CommandSystem.Prefix + "ViewEquip");
						entries.Add(CommandSystem.Prefix + "Kill");

						break;
					}
			}
			return entries;
		}

		public static Point2D DefaultDimensions(AccessLevel level)
		{
			Point2D dimensions = new Point2D();

			switch (level)
			{
				case AccessLevel.Player:
				case AccessLevel.VIP:
					{
						dimensions.X = 0;
						dimensions.Y = 0;
						break;
					}
				case AccessLevel.Counselor:
					{
						dimensions.X = 6;
						dimensions.Y = 1;
						break;
					}
				case AccessLevel.Decorator:
                case AccessLevel.Spawner:
                case AccessLevel.GameMaster:
                case AccessLevel.Seer:
                case AccessLevel.Administrator:
                case AccessLevel.Developer:
                case AccessLevel.CoOwner:
                case AccessLevel.Owner:
					{
						dimensions.X = 6;
						dimensions.Y = 2;
						break;
					}

			}
			return dimensions;
		}

		public ToolbarInfo(GenericReader reader)
		{
			Deserialize(reader);
		}

		public void Serialize(GenericWriter writer)
		{
			writer.WriteVersion(0);

			writer.Write(_Font);
			writer.Write(_Phantom);
			writer.Write(_Stealth);
			writer.Write(_Reverse);
			writer.Write(_Lock);

			writer.Write(_Dimensions);

			writer.Write(_Entries.Count);

			foreach (string t in _Entries)
			{
				writer.Write(t);
			}

			writer.Write(_Skin);

			writer.Write(_Points.Count);

			foreach (Point3D t in _Points)
			{
				writer.Write(t);
			}
		}

		private void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			_Dimensions = new Point2D();
			_Entries = new List<string>();
			_Points = new List<Point3D>();

			switch (version)
			{
				case 0:
					{
						_Font = reader.ReadInt();
						_Phantom = reader.ReadBool();
						_Stealth = reader.ReadBool();
						_Reverse = reader.ReadBool();
						_Lock = reader.ReadBool();

						_Dimensions = reader.ReadPoint2D();

						int count = reader.ReadInt();

						for (int i = 0; i < count; i++)
						{
							_Entries.Add(reader.ReadString());
						}

						_Skin = reader.ReadInt();

						count = reader.ReadInt();

						for (int i = 0; i < count; i++)
						{
							_Points.Add(reader.ReadPoint3D());
						}

						break;
					}
			}
		}
	}
}