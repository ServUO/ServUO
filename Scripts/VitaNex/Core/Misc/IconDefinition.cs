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

using Ultima;
#endregion

namespace VitaNex
{
	public enum IconType
	{
		GumpArt,
		ItemArt
	}

	[PropertyObject]
	public class IconDefinition
	{
		private static readonly Size _Zero = new Size(0, 0);

		public static void AddTo(Gump gump, int x, int y, IconDefinition icon)
		{
			if (gump != null && icon != null)
			{
				icon.AddToGump(gump, x, y);
			}
		}

		public static void AddTo(Gump gump, int x, int y, int hue, IconDefinition icon)
		{
			if (gump != null && icon != null)
			{
				icon.AddToGump(gump, x, y, hue);
			}
		}

		public static void AddTo(Gump gump, int x, int y, int offsetX, int offsetY, IconDefinition icon)
		{
			if (gump != null && icon != null)
			{
				icon.AddToGump(gump, x, y, offsetX, offsetY);
			}
		}

		public static void AddTo(Gump gump, int x, int y, int hue, int offsetX, int offsetY, IconDefinition icon)
		{
			if (gump != null && icon != null)
			{
				icon.AddToGump(gump, x, y, hue, offsetX, offsetY);
			}
		}

		public static IconDefinition Create(IconType type, int assetID)
		{
			return new IconDefinition(type, assetID);
		}

		public static IconDefinition Create(IconType type, int assetID, int hue)
		{
			return new IconDefinition(type, assetID, hue);
		}

		public static IconDefinition Create(IconType type, int assetID, int offsetX, int offsetY)
		{
			return new IconDefinition(type, assetID, offsetX, offsetY);
		}

		public static IconDefinition Create(IconType type, int assetID, int hue, int offsetX, int offsetY)
		{
			return new IconDefinition(type, assetID, hue, offsetX, offsetY);
		}

		public static IconDefinition FromGump(int gumpID)
		{
			return Create(IconType.GumpArt, gumpID);
		}

		public static IconDefinition FromGump(int gumpID, int hue)
		{
			return Create(IconType.GumpArt, gumpID, hue);
		}

		public static IconDefinition FromGump(int gumpID, int offsetX, int offsetY)
		{
			return Create(IconType.GumpArt, gumpID, offsetX, offsetY);
		}

		public static IconDefinition FromGump(int gumpID, int hue, int offsetX, int offsetY)
		{
			return Create(IconType.GumpArt, gumpID, hue, offsetX, offsetY);
		}

		public static IconDefinition FromItem(int itemID)
		{
			return Create(IconType.ItemArt, itemID);
		}

		public static IconDefinition FromItem(int itemID, int hue)
		{
			return Create(IconType.ItemArt, itemID, hue);
		}

		public static IconDefinition FromItem(int itemID, int offsetX, int offsetY)
		{
			return Create(IconType.ItemArt, itemID, offsetX, offsetY);
		}

		public static IconDefinition FromItem(int itemID, int hue, int offsetX, int offsetY)
		{
			return Create(IconType.ItemArt, itemID, hue, offsetX, offsetY);
		}

		public static IconDefinition FromMobile(int body)
		{
			return FromItem(ShrinkTable.Lookup(body));
		}

		public static IconDefinition FromMobile(int body, int hue)
		{
			return FromItem(ShrinkTable.Lookup(body), hue);
		}

		public static IconDefinition FromMobile(int body, int offsetX, int offsetY)
		{
			return FromItem(ShrinkTable.Lookup(body), offsetX, offsetY);
		}

		public static IconDefinition FromMobile(int body, int hue, int offsetX, int offsetY)
		{
			return FromItem(ShrinkTable.Lookup(body), hue, offsetX, offsetY);
		}

		#region Spell Icons
		public static IconDefinition ItemSpellIcon()
		{
			return FromItem(SpellIcons.RandomItemIcon());
		}

		public static IconDefinition ItemSpellIcon(int hue)
		{
			return FromItem(SpellIcons.RandomItemIcon(), hue);
		}

		public static IconDefinition ItemSpellIcon(int offsetX, int offsetY)
		{
			return FromItem(SpellIcons.RandomItemIcon(), offsetX, offsetY);
		}

		public static IconDefinition ItemSpellIcon(int hue, int offsetX, int offsetY)
		{
			return FromItem(SpellIcons.RandomItemIcon(), hue, offsetX, offsetY);
		}

		public static IconDefinition GumpSpellIcon()
		{
			return FromGump(SpellIcons.RandomGumpIcon());
		}

		public static IconDefinition GumpSpellIcon(int hue)
		{
			return FromGump(SpellIcons.RandomGumpIcon(), hue);
		}

		public static IconDefinition GumpSpellIcon(int offsetX, int offsetY)
		{
			return FromGump(SpellIcons.RandomGumpIcon(), offsetX, offsetY);
		}

		public static IconDefinition GumpSpellIcon(int hue, int offsetX, int offsetY)
		{
			return FromGump(SpellIcons.RandomGumpIcon(), hue, offsetX, offsetY);
		}

		public static IconDefinition SmallSpellIcon()
		{
			return FromGump(SpellIcons.RandomSmallIcon());
		}

		public static IconDefinition SmallSpellIcon(int hue)
		{
			return FromGump(SpellIcons.RandomSmallIcon(), hue);
		}

		public static IconDefinition SmallSpellIcon(int offsetX, int offsetY)
		{
			return FromGump(SpellIcons.RandomSmallIcon(), offsetX, offsetY);
		}

		public static IconDefinition SmallSpellIcon(int hue, int offsetX, int offsetY)
		{
			return FromGump(SpellIcons.RandomSmallIcon(), hue, offsetX, offsetY);
		}

		public static IconDefinition LargeSpellIcon()
		{
			return FromGump(SpellIcons.RandomLargeIcon());
		}

		public static IconDefinition LargeSpellIcon(int hue)
		{
			return FromGump(SpellIcons.RandomLargeIcon(), hue);
		}

		public static IconDefinition LargeSpellIcon(int offsetX, int offsetY)
		{
			return FromGump(SpellIcons.RandomLargeIcon(), offsetX, offsetY);
		}

		public static IconDefinition LargeSpellIcon(int hue, int offsetX, int offsetY)
		{
			return FromGump(SpellIcons.RandomLargeIcon(), hue, offsetX, offsetY);
		}
		#endregion

		public static IconDefinition Empty()
		{
			return new IconDefinition();
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public IconType AssetType { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int AssetID { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool IsEmpty => AssetID <= (IsItemArt ? 1 : 0);

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool IsGumpArt => AssetType == IconType.GumpArt;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool IsItemArt => AssetType == IconType.ItemArt;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Hue { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int OffsetX { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int OffsetY { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool ComputeOffset { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Size Size
		{
			get
			{
				if (IsGumpArt)
				{
					return GumpsExtUtility.GetImageSize(AssetID);
				}

				if (IsItemArt)
				{
					return ArtExtUtility.GetImageSize(AssetID);
				}

				return _Zero;
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool IsSpellIcon => SpellIcons.IsIcon(AssetID);

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool IsItemSpellIcon => IsItemArt && SpellIcons.IsItemIcon(AssetID);

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool IsGumpSpellIcon => IsGumpArt && SpellIcons.IsGumpIcon(AssetID);

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool IsSmallSpellIcon => IsGumpArt && SpellIcons.IsSmallIcon(AssetID);

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool IsLargeSpellIcon => IsGumpArt && SpellIcons.IsLargeIcon(AssetID);

		public IconDefinition()
			: this(IconType.ItemArt, 0)
		{ }

		public IconDefinition(IconType assetType, int assetID)
			: this(assetType, assetID, 0)
		{ }

		public IconDefinition(IconType assetType, int assetID, int hue)
			: this(assetType, assetID, hue, 0, 0)
		{ }

		public IconDefinition(IconType assetType, int assetID, int offsetX, int offsetY)
			: this(assetType, assetID, 0, offsetX, offsetY)
		{ }

		public IconDefinition(IconDefinition icon)
			: this(icon.AssetType, icon.AssetID, icon.Hue, icon.OffsetX, icon.OffsetY)
		{
			ComputeOffset = icon.ComputeOffset;
		}

		public IconDefinition(IconType assetType, int assetID, int hue, int offsetX, int offsetY)
		{
			AssetType = assetType;
			AssetID = assetID;

			Hue = hue;

			OffsetX = offsetX;
			OffsetY = offsetY;

			ComputeOffset = true;
		}

		public IconDefinition(GenericReader reader)
		{
			Deserialize(reader);
		}

		public virtual void AddToGump(Gump g, int x, int y)
		{
			AddToGump(g, x, y, -1, -1, -1);
		}

		public virtual void AddToGump(Gump g, int x, int y, int hue)
		{
			AddToGump(g, x, y, hue, -1, -1);
		}

		public virtual void AddToGump(Gump g, int x, int y, int offsetX, int offsetY)
		{
			AddToGump(g, x, y, -1, offsetX, offsetY);
		}

		public virtual void AddToGump(Gump g, int x, int y, int hue, int offsetX, int offsetY)
		{
			if (IsEmpty)
			{
				return;
			}

			if (offsetX < 0)
			{
				x += OffsetX;
			}
			else if (offsetX > 0)
			{
				x += offsetX;
			}

			if (offsetY < 0)
			{
				y += OffsetY;
			}
			else if (offsetY > 0)
			{
				y += offsetY;
			}

			if (hue < 0)
			{
				hue = Hue;
			}

			switch (AssetType)
			{
				case IconType.ItemArt:
				{
					if (ComputeOffset)
					{
						var o = ArtExtUtility.GetImageOffset(AssetID);

						if (o != Point.Empty)
						{
							x += o.X;
							y += o.Y;
						}
					}

					x = Math.Max(0, x);
					y = Math.Max(0, y);

					if (hue > 0)
					{
						g.AddItem(x, y, AssetID, hue);
					}
					else
					{
						g.AddItem(x, y, AssetID);
					}
				}
				break;
				case IconType.GumpArt:
				{
					x = Math.Max(0, x);
					y = Math.Max(0, y);

					if (hue > 0)
					{
						g.AddImage(x, y, AssetID, hue);
					}
					else
					{
						g.AddImage(x, y, AssetID);
					}
				}
				break;
			}
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(3);

			switch (version)
			{
				case 3:
					writer.Write(ComputeOffset);
					goto case 2;
				case 2:
				{
					writer.Write(OffsetX);
					writer.Write(OffsetY);
				}
				goto case 1;
				case 1:
					writer.Write(Hue);
					goto case 0;
				case 0:
				{
					writer.WriteFlag(AssetType);
					writer.Write(AssetID);
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 3:
					ComputeOffset = reader.ReadBool();
					goto case 2;
				case 2:
				{
					OffsetX = reader.ReadInt();
					OffsetY = reader.ReadInt();
				}
				goto case 1;
				case 1:
					Hue = reader.ReadInt();
					goto case 0;
				case 0:
				{
					AssetType = reader.ReadFlag<IconType>();
					AssetID = reader.ReadInt();
				}
				break;
			}

			if (version < 3)
			{
				ComputeOffset = true;
			}
		}
	}
}