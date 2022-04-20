#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

using Server.ContextMenus;
using Server.Items;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server
{
	/// <summary>
	///     Enumeration of item layer values.
	/// </summary>
	public enum Layer : byte
	{
		/// <summary>
		///     Invalid layer.
		/// </summary>
		Invalid = 0x00,

		/// <summary>
		///     First valid layer. Equivalent to <c>Layer.OneHanded</c>.
		/// </summary>
		FirstValid = 0x01,

		/// <summary>
		///     One handed weapon.
		/// </summary>
		OneHanded = 0x01,

		/// <summary>
		///     Two handed weapon or shield.
		/// </summary>
		TwoHanded = 0x02,

		/// <summary>
		///     Shoes.
		/// </summary>
		Shoes = 0x03,

		/// <summary>
		///     Pants.
		/// </summary>
		Pants = 0x04,

		/// <summary>
		///     Shirts.
		/// </summary>
		Shirt = 0x05,

		/// <summary>
		///     Helmets, hats, and masks.
		/// </summary>
		Helm = 0x06,

		/// <summary>
		///     Gloves.
		/// </summary>
		Gloves = 0x07,

		/// <summary>
		///     Rings.
		/// </summary>
		Ring = 0x08,

		/// <summary>
		///     Talismans.
		/// </summary>
		Talisman = 0x09,

		/// <summary>
		///     Gorgets and necklaces.
		/// </summary>
		Neck = 0x0A,

		/// <summary>
		///     Hair.
		/// </summary>
		Hair = 0x0B,

		/// <summary>
		///     Half aprons.
		/// </summary>
		Waist = 0x0C,

		/// <summary>
		///     Torso, inner layer.
		/// </summary>
		InnerTorso = 0x0D,

		/// <summary>
		///     Bracelets.
		/// </summary>
		Bracelet = 0x0E,

		/// <summary>
		///     Face.
		/// </summary>
		Face = 0x0F,

		/// <summary>
		///     Beards and mustaches.
		/// </summary>
		FacialHair = 0x10,

		/// <summary>
		///     Torso, outer layer.
		/// </summary>
		MiddleTorso = 0x11,

		/// <summary>
		///     Earings.
		/// </summary>
		Earrings = 0x12,

		/// <summary>
		///     Arms and sleeves.
		/// </summary>
		Arms = 0x13,

		/// <summary>
		///     Cloaks.
		/// </summary>
		Cloak = 0x14,

		/// <summary>
		///     Backpacks.
		/// </summary>
		Backpack = 0x15,

		/// <summary>
		///     Torso, outer layer.
		/// </summary>
		OuterTorso = 0x16,

		/// <summary>
		///     Leggings, outer layer.
		/// </summary>
		OuterLegs = 0x17,

		/// <summary>
		///     Leggings, inner layer.
		/// </summary>
		InnerLegs = 0x18,

		/// <summary>
		///     Last valid non-internal layer. Equivalent to <c>Layer.InnerLegs</c>.
		/// </summary>
		LastUserValid = 0x18,

		/// <summary>
		///     Mount item layer.
		/// </summary>
		Mount = 0x19,

		/// <summary>
		///     Vendor 'buy pack' layer.
		/// </summary>
		ShopBuy = 0x1A,

		/// <summary>
		///     Vendor 'resale pack' layer.
		/// </summary>
		ShopResale = 0x1B,

		/// <summary>
		///     Vendor 'sell pack' layer.
		/// </summary>
		ShopSell = 0x1C,

		/// <summary>
		///     Bank box layer.
		/// </summary>
		Bank = 0x1D,

		/// <summary>
		/// Unused, using this layer makes you invisible to other players. Strange.
		/// </summary>
		/// 
		Reserved_1 = 0x1E,

		/// <summary>
		///     Secure Trade Layer
		/// </summary>
		SecureTrade = 0x1F
	}

	/// <summary>
	///     Internal flags used to signal how the item should be updated and resent to nearby clients.
	/// </summary>
	[Flags]
	public enum ItemDelta
	{
		/// <summary>
		///     Nothing.
		/// </summary>
		None = 0x00000000,

		/// <summary>
		///     Resend the item.
		/// </summary>
		Update = 0x00000001,

		/// <summary>
		///     Resend the item only if it is equiped.
		/// </summary>
		EquipOnly = 0x00000002,

		/// <summary>
		///     Resend the item's properties.
		/// </summary>
		Properties = 0x00000004
	}

	/// <summary>
	///     Enumeration containing possible ways to handle item ownership on death.
	/// </summary>
	public enum DeathMoveResult
	{
		/// <summary>
		///     The item should be placed onto the corpse.
		/// </summary>
		MoveToCorpse,

		/// <summary>
		///     The item should remain equiped.
		/// </summary>
		RemainEquiped,

		/// <summary>
		///     The item should be placed into the owners backpack.
		/// </summary>
		MoveToBackpack
	}

	/// <summary>
	///     Enumeration containing all possible light types. These are only applicable to light source items, like lanterns, candles, braziers, etc.
	/// </summary>
	public enum LightType
	{
		/// <summary>
		///     Window shape, arched, ray shining east.
		/// </summary>
		ArchedWindowEast = 0,

		/// <summary>
		///     Medium circular shape.
		/// </summary>
		Circle225 = 1,

		/// <summary>
		///     Small circular shape.
		/// </summary>
		Circle150 = 2,

		/// <summary>
		///     Door shape, shining south.
		/// </summary>
		DoorSouth = 3,

		/// <summary>
		///     Door shape, shining east.
		/// </summary>
		DoorEast = 4,

		/// <summary>
		///     Large semicircular shape (180 degrees), north wall.
		/// </summary>
		NorthBig = 5,

		/// <summary>
		///     Large pie shape (90 degrees), north-east corner.
		/// </summary>
		NorthEastBig = 6,

		/// <summary>
		///     Large semicircular shape (180 degrees), east wall.
		/// </summary>
		EastBig = 7,

		/// <summary>
		///     Large semicircular shape (180 degrees), west wall.
		/// </summary>
		WestBig = 8,

		/// <summary>
		///     Large pie shape (90 degrees), south-west corner.
		/// </summary>
		SouthWestBig = 9,

		/// <summary>
		///     Large semicircular shape (180 degrees), south wall.
		/// </summary>
		SouthBig = 10,

		/// <summary>
		///     Medium semicircular shape (180 degrees), north wall.
		/// </summary>
		NorthSmall = 11,

		/// <summary>
		///     Medium pie shape (90 degrees), north-east corner.
		/// </summary>
		NorthEastSmall = 12,

		/// <summary>
		///     Medium semicircular shape (180 degrees), east wall.
		/// </summary>
		EastSmall = 13,

		/// <summary>
		///     Medium semicircular shape (180 degrees), west wall.
		/// </summary>
		WestSmall = 14,

		/// <summary>
		///     Medium semicircular shape (180 degrees), south wall.
		/// </summary>
		SouthSmall = 15,

		/// <summary>
		///     Shaped like a wall decoration, north wall.
		/// </summary>
		DecorationNorth = 16,

		/// <summary>
		///     Shaped like a wall decoration, north-east corner.
		/// </summary>
		DecorationNorthEast = 17,

		/// <summary>
		///     Small semicircular shape (180 degrees), east wall.
		/// </summary>
		EastTiny = 18,

		/// <summary>
		///     Shaped like a wall decoration, west wall.
		/// </summary>
		DecorationWest = 19,

		/// <summary>
		///     Shaped like a wall decoration, south-west corner.
		/// </summary>
		DecorationSouthWest = 20,

		/// <summary>
		///     Small semicircular shape (180 degrees), south wall.
		/// </summary>
		SouthTiny = 21,

		/// <summary>
		///     Window shape, rectangular, no ray, shining south.
		/// </summary>
		RectWindowSouthNoRay = 22,

		/// <summary>
		///     Window shape, rectangular, no ray, shining east.
		/// </summary>
		RectWindowEastNoRay = 23,

		/// <summary>
		///     Window shape, rectangular, ray shining south.
		/// </summary>
		RectWindowSouth = 24,

		/// <summary>
		///     Window shape, rectangular, ray shining east.
		/// </summary>
		RectWindowEast = 25,

		/// <summary>
		///     Window shape, arched, no ray, shining south.
		/// </summary>
		ArchedWindowSouthNoRay = 26,

		/// <summary>
		///     Window shape, arched, no ray, shining east.
		/// </summary>
		ArchedWindowEastNoRay = 27,

		/// <summary>
		///     Window shape, arched, ray shining south.
		/// </summary>
		ArchedWindowSouth = 28,

		/// <summary>
		///     Large circular shape.
		/// </summary>
		Circle300 = 29,

		/// <summary>
		///     Large pie shape (90 degrees), north-west corner.
		/// </summary>
		NorthWestBig = 30,

		/// <summary>
		///     Negative light. Medium pie shape (90 degrees), south-east corner.
		/// </summary>
		DarkSouthEast = 31,

		/// <summary>
		///     Negative light. Medium semicircular shape (180 degrees), south wall.
		/// </summary>
		DarkSouth = 32,

		/// <summary>
		///     Negative light. Medium pie shape (90 degrees), north-west corner.
		/// </summary>
		DarkNorthWest = 33,

		/// <summary>
		///     Negative light. Medium pie shape (90 degrees), south-east corner. Equivalent to <c>LightType.SouthEast</c>.
		/// </summary>
		DarkSouthEast2 = 34,

		/// <summary>
		///     Negative light. Medium circular shape (180 degrees), east wall.
		/// </summary>
		DarkEast = 35,

		/// <summary>
		///     Negative light. Large circular shape.
		/// </summary>
		DarkCircle300 = 36,

		/// <summary>
		///     Opened door shape, shining south.
		/// </summary>
		DoorOpenSouth = 37,

		/// <summary>
		///     Opened door shape, shining east.
		/// </summary>
		DoorOpenEast = 38,

		/// <summary>
		///     Window shape, square, ray shining east.
		/// </summary>
		SquareWindowEast = 39,

		/// <summary>
		///     Window shape, square, no ray, shining east.
		/// </summary>
		SquareWindowEastNoRay = 40,

		/// <summary>
		///     Window shape, square, ray shining south.
		/// </summary>
		SquareWindowSouth = 41,

		/// <summary>
		///     Window shape, square, no ray, shining south.
		/// </summary>
		SquareWindowSouthNoRay = 42,

		/// <summary>
		///     Empty.
		/// </summary>
		Empty = 43,

		/// <summary>
		///     Window shape, skinny, no ray, shining south.
		/// </summary>
		SkinnyWindowSouthNoRay = 44,

		/// <summary>
		///     Window shape, skinny, ray shining east.
		/// </summary>
		SkinnyWindowEast = 45,

		/// <summary>
		///     Window shape, skinny, no ray, shining east.
		/// </summary>
		SkinnyWindowEastNoRay = 46,

		/// <summary>
		///     Shaped like a hole, shining south.
		/// </summary>
		HoleSouth = 47,

		/// <summary>
		///     Shaped like a hole, shining south.
		/// </summary>
		HoleEast = 48,

		/// <summary>
		///     Large circular shape with a moongate graphic embeded.
		/// </summary>
		Moongate = 49,

		/// <summary>
		///     Unknown usage. Many rows of slightly angled lines.
		/// </summary>
		Strips = 50,

		/// <summary>
		///     Shaped like a small hole, shining south.
		/// </summary>
		SmallHoleSouth = 51,

		/// <summary>
		///     Shaped like a small hole, shining east.
		/// </summary>
		SmallHoleEast = 52,

		/// <summary>
		///     Large semicircular shape (180 degrees), north wall. Identical graphic as <c>LightType.NorthBig</c>, but slightly different positioning.
		/// </summary>
		NorthBig2 = 53,

		/// <summary>
		///     Large semicircular shape (180 degrees), west wall. Identical graphic as <c>LightType.WestBig</c>, but slightly different positioning.
		/// </summary>
		WestBig2 = 54,

		/// <summary>
		///     Large pie shape (90 degrees), north-west corner. Equivalent to <c>LightType.NorthWestBig</c>.
		/// </summary>
		NorthWestBig2 = 55
	}

	/// <summary>
	///     Enumeration of an item's loot and steal state.
	/// </summary>
	public enum LootType : byte
	{
		/// <summary>
		///     Stealable. Lootable.
		/// </summary>
		Regular = 0,

		/// <summary>
		///     Unstealable. Unlootable, unless owned by a murderer.
		/// </summary>
		Newbied = 1,

		/// <summary>
		///     Unstealable. Unlootable, always.
		/// </summary>
		Blessed = 2,

		/// <summary>
		///     Stealable. Lootable, always.
		/// </summary>
		Cursed = 3
	}

	public class BounceInfo
	{
		public Map m_Map;
		public Point3D m_Location, m_WorldLoc;
		public object m_Parent;
		public object m_ParentStack;
		public byte m_GridLocation;
		public Mobile m_Mobile;

		public BounceInfo(Mobile from, Item item)
		{
			m_Map = item.Map;
			m_Location = item.Location;
			m_WorldLoc = item.GetWorldLocation();
			m_Parent = item.Parent;
			m_ParentStack = null;
			m_GridLocation = item.GridLocation;
			m_Mobile = from;
		}

		private BounceInfo(Map map, Point3D loc, Point3D worldLoc, IEntity parent)
		{
			m_Map = map;
			m_Location = loc;
			m_WorldLoc = worldLoc;
			m_Parent = parent;
			m_ParentStack = null;
		}

		public static BounceInfo Deserialize(GenericReader reader)
		{
			if (reader.ReadBool())
			{
				var map = reader.ReadMap();
				var loc = reader.ReadPoint3D();
				var worldLoc = reader.ReadPoint3D();

				IEntity parent;

				Serial serial = reader.ReadInt();

				if (serial.IsItem)
				{
					parent = World.FindItem(serial);
				}
				else if (serial.IsMobile)
				{
					parent = World.FindMobile(serial);
				}
				else
				{
					parent = null;
				}

				return new BounceInfo(map, loc, worldLoc, parent);
			}

			return null;
		}

		public static void Serialize(BounceInfo info, GenericWriter writer)
		{
			if (info == null)
			{
				writer.Write(false);
			}
			else
			{
				writer.Write(true);

				writer.Write(info.m_Map);
				writer.Write(info.m_Location);
				writer.Write(info.m_WorldLoc);

				if (info.m_Parent is Mobile mobile)
				{
					writer.Write(mobile);
				}
				else if (info.m_Parent is Item item)
				{
					writer.Write(item);
				}
				else
				{
					writer.Write((Serial)0);
				}
			}
		}
	}

	public enum TotalType
	{
		Gold,
		Items,
		Weight
	}

	[Flags]
	public enum ExpandFlag
	{
		None = 0x000,

		Name = 0x001,
		Items = 0x002,
		Bounce = 0x004,
		Holder = 0x008,
		Blessed = 0x010,
		TempFlag = 0x020,
		SaveFlag = 0x040,
		Weight = 0x080,
		Spawner = 0x100
	}

	public class Item : IEntity, IHued, IComparable<Item>, ISerializable, ISpawnable
	{
		public static readonly List<Item> EmptyItems = new List<Item>();

		public int CompareTo(IEntity other)
		{
			if (other == null)
			{
				return -1;
			}

			return m_Serial.CompareTo(other.Serial);
		}

		public int CompareTo(Item other)
		{
			return CompareTo((IEntity)other);
		}

		public int CompareTo(object other)
		{
			if (other == null || other is IEntity)
			{
				return CompareTo((IEntity)other);
			}

			throw new ArgumentException();
		}

		#region Standard fields
		private readonly Serial m_Serial;
		private Point3D m_Location;
		private int m_ItemID;
		private int m_Hue;
		private int m_Amount;
		private Layer m_Layer;
		private object m_Parent; // Mobile, Item, or null=World
		private Map m_Map;
		private LootType m_LootType;
		private DateTime m_LastMovedTime;
		private Direction m_Direction;
		private LightType m_Light;
		#endregion

		private ItemDelta m_DeltaFlags;
		private ImplFlag m_Flags;

		#region Packet caches
		private Packet m_WorldPacket;
		private Packet m_RemovePacket;

		private Packet m_OPLPacket;
		private ObjectPropertyList m_PropertyList;
		#endregion

		public int TempFlags
		{
			get
			{
				var info = LookupCompactInfo();

				if (info != null)
				{
					return info.m_TempFlags;
				}

				return 0;
			}
			set
			{
				var info = AcquireCompactInfo();

				info.m_TempFlags = value;

				if (info.m_TempFlags == 0)
				{
					VerifyCompactInfo();
				}
			}
		}

		public int SavedFlags
		{
			get
			{
				var info = LookupCompactInfo();

				if (info != null)
				{
					return info.m_SavedFlags;
				}

				return 0;
			}
			set
			{
				var info = AcquireCompactInfo();

				info.m_SavedFlags = value;

				if (info.m_SavedFlags == 0)
				{
					VerifyCompactInfo();
				}
			}
		}

		/// <summary>
		///     The <see cref="Mobile" /> who is currently <see cref="Mobile.Holding">holding</see> this item.
		/// </summary>
		public Mobile HeldBy
		{
			get
			{
				var info = LookupCompactInfo();

				if (info != null)
				{
					return info.m_HeldBy;
				}

				return null;
			}
			set
			{
				var info = AcquireCompactInfo();

				info.m_HeldBy = value;

				if (info.m_HeldBy == null)
				{
					VerifyCompactInfo();
				}
			}
		}

		private byte m_GridLocation; // Default 0

		[CommandProperty(AccessLevel.GameMaster)]
		public byte GridLocation
		{
			get => m_GridLocation;
			set
			{
				if (Parent is Container)
				{
					if (value < 0 || value > 0x7C || !((Container)Parent).IsFreePosition(value))
					{
						m_GridLocation = ((Container)Parent).GetNewPosition(0);
					}
					else
					{
						m_GridLocation = value;
					}
				}
				else
				{
					m_GridLocation = value;
				}
			}
		}

		[Flags]
		private enum ImplFlag : byte
		{
			None = 0x00,
			Visible = 0x01,
			Movable = 0x02,
			Deleted = 0x04,
			Stackable = 0x08,
			InQueue = 0x10,
			Insured = 0x20,
			PayedInsurance = 0x40,
			QuestItem = 0x80
		}

		private class CompactInfo
		{
			public string m_Name;

			public List<Item> m_Items;
			public BounceInfo m_Bounce;

			public Mobile m_HeldBy;
			public Mobile m_BlessedFor;

			public ISpawner m_Spawner;

			public int m_TempFlags;
			public int m_SavedFlags;

			public double m_Weight = -1;
		}

		private CompactInfo m_CompactInfo;

		public ExpandFlag GetExpandFlags()
		{
			var info = LookupCompactInfo();

			ExpandFlag flags = 0;

			if (info != null)
			{
				if (info.m_BlessedFor != null)
				{
					flags |= ExpandFlag.Blessed;
				}

				if (info.m_Bounce != null)
				{
					flags |= ExpandFlag.Bounce;
				}

				if (info.m_HeldBy != null)
				{
					flags |= ExpandFlag.Holder;
				}

				if (info.m_Items != null)
				{
					flags |= ExpandFlag.Items;
				}

				if (info.m_Name != null)
				{
					flags |= ExpandFlag.Name;
				}

				if (info.m_Spawner != null)
				{
					flags |= ExpandFlag.Spawner;
				}

				if (info.m_SavedFlags != 0)
				{
					flags |= ExpandFlag.SaveFlag;
				}

				if (info.m_TempFlags != 0)
				{
					flags |= ExpandFlag.TempFlag;
				}

				if (info.m_Weight != -1)
				{
					flags |= ExpandFlag.Weight;
				}
			}

			return flags;
		}

		private CompactInfo LookupCompactInfo()
		{
			return m_CompactInfo;
		}

		private CompactInfo AcquireCompactInfo()
		{
			if (m_CompactInfo == null)
			{
				m_CompactInfo = new CompactInfo();
			}

			return m_CompactInfo;
		}

		private void ReleaseCompactInfo()
		{
			m_CompactInfo = null;
		}

		private void VerifyCompactInfo()
		{
			var info = m_CompactInfo;

			if (info == null)
			{
				return;
			}

			var isValid = info.m_Name != null || info.m_Items != null || info.m_Bounce != null || info.m_HeldBy != null || info.m_BlessedFor != null ||
						   info.m_Spawner != null || info.m_TempFlags != 0 || info.m_SavedFlags != 0 || info.m_Weight != -1;

			if (!isValid)
			{
				ReleaseCompactInfo();
			}
		}

		public List<Item> LookupItems()
		{
			if (this is Container)
			{
				return ((Container)this).m_Items;
			}

			var info = LookupCompactInfo();

			if (info != null)
			{
				return info.m_Items;
			}

			return null;
		}

		public List<Item> AcquireItems()
		{
			if (this is Container)
			{
				var cont = (Container)this;

				if (cont.m_Items == null)
				{
					cont.m_Items = new List<Item>();
				}

				return cont.m_Items;
			}

			var info = AcquireCompactInfo();

			if (info.m_Items == null)
			{
				info.m_Items = new List<Item>();
			}

			return info.m_Items;
		}

		public static Bitmap GetBitmap(int itemID)
		{
			try
			{
				return ArtData.GetStatic(itemID);
			}
			catch (Exception e)
			{
				Diagnostics.ExceptionLogging.LogException(e);
			}

			return null;
		}

		public static void Measure(Bitmap bmp, out int xMin, out int yMin, out int xMax, out int yMax)
		{
			ArtData.Measure(bmp, out xMin, out yMin, out xMax, out yMax);
		}

		public static Rectangle MeasureBound(Bitmap bmp)
		{
			Measure(bmp, out var xMin, out var yMin, out var xMax, out var yMax);
			return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		public static Size MeasureSize(Bitmap bmp)
		{
			Measure(bmp, out var xMin, out var yMin, out var xMax, out var yMax);
			return new Size(xMax - xMin, yMax - yMin);
		}

		private void SetFlag(ImplFlag flag, bool value)
		{
			if (value)
			{
				m_Flags |= flag;
			}
			else
			{
				m_Flags &= ~flag;
			}
		}

		private bool GetFlag(ImplFlag flag)
		{
			return (m_Flags & flag) != 0;
		}

		public BounceInfo GetBounce()
		{
			var info = LookupCompactInfo();

			if (info != null)
			{
				return info.m_Bounce;
			}

			return null;
		}

		public void RecordBounce(Mobile from, Item parentstack = null)
		{
			var info = AcquireCompactInfo();

			info.m_Bounce = new BounceInfo(from, this)
			{
				m_ParentStack = parentstack
			};
		}

		public void ClearBounce()
		{
			var info = LookupCompactInfo();

			if (info != null)
			{
				var bounce = info.m_Bounce;

				if (bounce != null)
				{
					info.m_Bounce = null;

					if (bounce.m_Parent is Item itemParent)
					{
						if (!itemParent.Deleted)
						{
							itemParent.OnItemBounceCleared(this);
						}
					}
					else if (bounce.m_Parent is Mobile mobileParent && !mobileParent.Deleted)
					{
						mobileParent.OnItemBounceCleared(this);
					}

					VerifyCompactInfo();
				}
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked when a client, <paramref name="from" />, invokes a 'help request' for the Item. Seemingly no longer functional in newer clients.
		/// </summary>
		public virtual void OnHelpRequest(Mobile from)
		{ }

		/// <summary>
		///     Overridable. Method checked to see if the item can be traded.
		/// </summary>
		/// <returns>True if the trade is allowed, false if not.</returns>
		public virtual bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when a trade has completed, either successfully or not.
		/// </summary>
		public virtual void OnSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
		{ }

		/// <summary>
		///     Overridable. Method checked to see if the elemental resistances of this Item conflict with another Item on the
		///     <see
		///         cref="Mobile" />
		///     .
		/// </summary>
		/// <returns>
		///     <list type="table">
		///         <item>
		///             <term>True</term>
		///             <description>
		///                 There is a confliction. The elemental resistance bonuses of this Item should not be applied to the
		///                 <see
		///                     cref="Mobile" />
		///             </description>
		///         </item>
		///         <item>
		///             <term>False</term>
		///             <description>There is no confliction. The bonuses should be applied.</description>
		///         </item>
		///     </list>
		/// </returns>
		public virtual bool CheckPropertyConfliction(Mobile m)
		{
			return false;
		}

		/// <summary>
		///     Overridable. Sends the <see cref="PropertyList">object property list</see> to <paramref name="from" />.
		/// </summary>
		public virtual void SendPropertiesTo(Mobile from)
		{
			from.Send(PropertyList);
		}

		/// <summary>
		///     Overridable. Adds the name of this item to the given <see cref="ObjectPropertyList" />. This method should be
		///     overriden if the item requires a complex naming format.
		/// </summary>
		public virtual void AddNameProperty(ObjectPropertyList list)
		{
			var name = Name ?? String.Empty;

			if (String.IsNullOrWhiteSpace(name))
			{
				if (m_Amount <= 1)
				{
					list.Add(LabelNumber);
				}
				else
				{
					list.Add(1050039, "{0}\t#{1}", m_Amount, LabelNumber); // ~1_NUMBER~ ~2_ITEMNAME~
				}
			}
			else
			{
				if (m_Amount <= 1)
				{
					list.Add(name);
				}
				else
				{
					list.Add(1050039, "{0}\t{1}", m_Amount, Name); // ~1_NUMBER~ ~2_ITEMNAME~
				}
			}
		}

		/// <summary>
		///     Overridable. Adds the loot type of this item to the given <see cref="ObjectPropertyList" />. By default, this will be either 'blessed', 'cursed', or 'insured'.
		/// </summary>
		public virtual void AddLootTypeProperty(ObjectPropertyList list)
		{
			if (DisplayLootType)
			{
				if (m_LootType == LootType.Blessed)
				{
					list.Add(1038021); // blessed
				}
				else if (m_LootType == LootType.Cursed)
				{
					list.Add(1049643); // cursed
				}
				else if (Insured)
				{
					list.Add(1061682); // <b>insured</b>
				}
			}
		}

		/// <summary>
		///     Overridable. Adds any elemental resistances of this item to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddResistanceProperties(ObjectPropertyList list)
		{
			var v = PhysicalResistance;

			if (v != 0)
			{
				list.Add(1060448, v.ToString()); // physical resist ~1_val~%
			}

			v = FireResistance;

			if (v != 0)
			{
				list.Add(1060447, v.ToString()); // fire resist ~1_val~%
			}

			v = ColdResistance;

			if (v != 0)
			{
				list.Add(1060445, v.ToString()); // cold resist ~1_val~%
			}

			v = PoisonResistance;

			if (v != 0)
			{
				list.Add(1060449, v.ToString()); // poison resist ~1_val~%
			}

			v = EnergyResistance;

			if (v != 0)
			{
				list.Add(1060446, v.ToString()); // energy resist ~1_val~%
			}
		}

		/// <summary>
		///     Overridable. Determines whether the item will show <see cref="AddWeightProperty" />.
		/// </summary>
		public virtual bool DisplayWeight
		{
			get
			{
				if (!Movable && !(IsLockedDown || IsSecure) && ItemData.Weight == 255)
				{
					return false;
				}

				return true;
			}
		}

		/// <summary>
		///     Overridable. Adds header properties. By default, this invokes <see cref="AddNameProperty" />,
		///     <see
		///         cref="AddBlessedForProperty" />
		///     (if applicable), and <see cref="AddLootTypeProperty" /> (if
		///     <see
		///         cref="DisplayLootType" />
		///     ).
		/// </summary>
		public virtual void AddNameProperties(ObjectPropertyList list)
		{
			AddNameProperty(list);

			if (IsSecure)
			{
				AddSecureProperty(list);
			}
			else if (IsLockedDown)
			{
				AddLockedDownProperty(list);
			}

			AddCraftedProperties(list);
			AddLootTypeProperty(list);
			AddUsesRemainingProperties(list);
			AddWeightProperty(list);

			AppendChildNameProperties(list);

			if (QuestItem)
			{
				AddQuestItemProperty(list);
			}
		}

		/// <summary>
		/// Overrideable, used to add crafted by, excpetional, etc properties to items
		/// </summary>
		/// <param name="list"></param>
		public virtual void AddCraftedProperties(ObjectPropertyList list)
		{
		}

		/// <summary>
		/// Overrideable, used for IUsesRemaining UsesRemaining property
		/// </summary>
		/// <param name="list"></param>
		public virtual void AddUsesRemainingProperties(ObjectPropertyList list)
		{
		}

		/// <summary>
		///     Overridable. Displays cliloc 1072788-1072789.
		/// </summary>
		public virtual void AddWeightProperty(ObjectPropertyList list)
		{
			if (DisplayWeight && Weight > 0)
			{
				var weight = PileWeight + TotalWeight;

				if (weight == 1)
				{
					list.Add(1072788, weight.ToString()); //Weight: ~1_WEIGHT~ stone
				}
				else
				{
					list.Add(1072789, weight.ToString()); //Weight: ~1_WEIGHT~ stones
				}
			}
		}

		/// <summary>
		///     Overridable. Adds the "Quest Item" property to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddQuestItemProperty(ObjectPropertyList list)
		{
			list.Add(1072351); // Quest Item
		}

		/// <summary>
		///     Overridable. Adds the "Locked Down & Secure" property to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddSecureProperty(ObjectPropertyList list)
		{
			list.Add(501644); // locked down & secure
		}

		/// <summary>
		///     Overridable. Adds the "Locked Down" property to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddLockedDownProperty(ObjectPropertyList list)
		{
			list.Add(501643); // locked down
		}

		/// <summary>
		///     Overridable. Adds the "Blessed for ~1_NAME~" property to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddBlessedForProperty(ObjectPropertyList list, Mobile m)
		{
			list.Add(1062203, "{0}", m.Name); // Blessed for ~1_NAME~
		}

		public virtual void AddItemSocketProperties(ObjectPropertyList list)
		{
			if (Sockets != null)
			{
				foreach (var socket in Sockets)
				{
					socket.GetProperties(list);
				}
			}
		}

		public virtual void AddItemPowerProperties(ObjectPropertyList list)
		{
		}

		/// <summary>
		///     Overridable. Fills an <see cref="ObjectPropertyList" /> with everything applicable. By default, this invokes
		///     <see
		///         cref="AddNameProperties" />
		///     , then <see cref="Item.GetChildProperties">Item.GetChildProperties</see> or
		///     <see
		///         cref="Mobile.GetChildProperties">
		///         Mobile.GetChildProperties
		///     </see>
		///     . This method should be overriden to add any custom properties.
		/// </summary>
		public virtual void GetProperties(ObjectPropertyList list)
		{
			AddNameProperties(list);

			AddItemSocketProperties(list);

			if (Spawner != null)
			{
				Spawner.GetSpawnProperties(this, list);
			}

			AddItemPowerProperties(list);
		}

		/// <summary>
		///     Overridable. Event invoked when a child (<paramref name="item" />) is building it's <see cref="ObjectPropertyList" />. Recursively calls
		///     <see
		///         cref="Item.GetChildProperties">
		///         Item.GetChildProperties
		///     </see>
		///     or <see cref="Mobile.GetChildProperties">Mobile.GetChildProperties</see>.
		/// </summary>
		public virtual void GetChildProperties(ObjectPropertyList list, Item item)
		{
			if (m_Parent is Item itemParent)
			{
				itemParent.GetChildProperties(list, item);
			}
			else if (m_Parent is Mobile mobileParent)
			{
				mobileParent.GetChildProperties(list, item);
			}
		}

		/// <summary>
		///     Overridable. Event invoked when a child (<paramref name="item" />) is building it's Name
		///     <see
		///         cref="ObjectPropertyList" />
		///     . Recursively calls <see cref="Item.GetChildProperties">Item.GetChildNameProperties</see> or
		///     <see
		///         cref="Mobile.GetChildProperties">
		///         Mobile.GetChildNameProperties
		///     </see>
		///     .
		/// </summary>
		public virtual void GetChildNameProperties(ObjectPropertyList list, Item item)
		{
			if (m_Parent is Item itemParent)
			{
				itemParent.GetChildNameProperties(list, item);
			}
			else if (m_Parent is Mobile mobileParent)
			{
				mobileParent.GetChildNameProperties(list, item);
			}
		}

		public virtual bool IsChildVisibleTo(Mobile m, Item child)
		{
			return true;
		}

		public void Bounce(Mobile from)
		{
			if (m_Parent is Item oip)
			{
				oip.RemoveItem(this);
			}
			else if (m_Parent is Mobile omp)
			{
				omp.RemoveItem(this);
			}

			m_Parent = null;

			var bounce = GetBounce();

			if (bounce != null)
			{
				if (bounce.m_ParentStack is Item ps && !ps.Deleted && ps.IsAccessibleTo(from) && ps.StackWith(from, this))
				{
					ClearBounce();
					return;
				}

				if (bounce.m_Parent is Item ip)
				{
					var rpm = ip.RootParent as Mobile;

					if (!ip.Deleted && ip.IsAccessibleTo(from) && (rpm == null || rpm.CheckNonlocalDrop(from, this, ip)))
					{
						if (!ip.Movable || rpm == from || ip.Map == bounce.m_Map && ip.GetWorldLocation() == bounce.m_WorldLoc)
						{
							if (from != null && ip is Container c && (c.TotalItems >= c.MaxItems || c.TotalWeight >= c.MaxWeight))
							{
								MoveToWorld(from.Location, from.Map);
							}
							else
							{
								Location = bounce.m_Location;

								ip.AddItem(this);
							}
						}
						else
						{
							MoveToWorld(from.Location, from.Map);
						}
					}
					else
					{
						MoveToWorld(from.Location, from.Map);
					}
				}
				else if (bounce.m_Parent is Mobile mp)
				{
					if (mp.Deleted || !mp.EquipItem(this))
					{
						MoveToWorld(bounce.m_WorldLoc, bounce.m_Map);
					}
				}
				else
				{
					MoveToWorld(bounce.m_WorldLoc, bounce.m_Map);
				}

				ClearBounce();
			}
			else
			{
				MoveToWorld(from.Location, from.Map);
			}
		}

		/// <summary>
		///     Overridable. Method checked to see if this item may be equiped while casting a spell. By default, this returns false. It is overriden on spellbook and spell channeling weapons or shields.
		/// </summary>
		/// <returns>True if it may, false if not.</returns>
		/// <example>
		///     <code>
		/// 	public override bool AllowEquipedCast( Mobile from )
		/// 	{
		/// 		if ( from.Int &gt;= 100 )
		/// 			return true;
		/// 		
		/// 		return base.AllowEquipedCast( from );
		///  }</code>
		///     When placed in an Item script, the item may be cast when equiped if the <paramref name="from" /> has 100 or more intelligence. Otherwise, it will drop to their backpack.
		/// </example>
		public virtual bool AllowEquipedCast(Mobile from)
		{
			return false;
		}

		public virtual bool CheckConflictingLayer(Mobile m, Item item, Layer layer)
		{
			return m_Layer == layer;
		}

		public virtual bool CanEquip(Mobile m)
		{
			return m_Layer != Layer.Invalid && m.FindItemOnLayer(m_Layer) == null && CheckEquip(m, true);
		}

		public virtual bool CheckEquip(Mobile m, bool message)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (this == m.Mount || this == m.Backpack || this == m.FindBankNoCreate())
			{
				return true;
			}

			var e = new CheckEquipItemEventArgs(m, this, message);

			EventSink.InvokeCheckEquipItem(e);

			if (e.Item != this || e.Item.Deleted || e.Block)
			{
				return false;
			}

			if (m.AccessLevel < AccessLevel.GameMaster && BlessedFor != null && BlessedFor != m)
			{
				if (message)
				{
					m.SendLocalizedMessage(1153882); // You do not own that.
				}

				return false;
			}

			return true;
		}

		public virtual void GetChildContextMenuEntries(Mobile from, List<ContextMenuEntry> list, Item item)
		{
			if (m_Parent is Item iParent)
			{
				iParent.GetChildContextMenuEntries(from, list, item);
			}
			else if (m_Parent is Mobile mParent)
			{
				mParent.GetChildContextMenuEntries(from, list, item);
			}
		}

		public virtual void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (m_Parent is Item item)
			{
				item.GetChildContextMenuEntries(from, list, this);
			}
			else if (m_Parent is Mobile mobile)
			{
				mobile.GetChildContextMenuEntries(from, list, this);
			}

			if (from.Region != null)
			{
				from.Region.GetContextMenuEntries(from, list, this);
			}

			if (Spawner != null)
			{
				Spawner.GetSpawnContextEntries(this, from, list);
			}
		}

		public virtual bool DisplayContextMenu(Mobile from)
		{
			return ContextMenu.Display(from, this);
		}

		public virtual bool VerifyMove(Mobile from)
		{
			return Movable;
		}

		public virtual void OnParentKill(Mobile target, Container corpse)
		{ }

		public virtual DeathMoveResult OnParentDeath(Mobile parent)
		{
			if (!Movable)
			{
				return DeathMoveResult.RemainEquiped;
			}

			if (parent.KeepsItemsOnDeath)
			{
				return DeathMoveResult.MoveToBackpack;
			}

			if (CheckBlessed(parent))
			{
				return DeathMoveResult.MoveToBackpack;
			}

			if (CheckNewbied() && parent.Kills < 5)
			{
				return DeathMoveResult.MoveToBackpack;
			}

			if (parent.Player && Nontransferable)
			{
				return DeathMoveResult.MoveToBackpack;
			}

			return DeathMoveResult.MoveToCorpse;
		}

		public virtual DeathMoveResult OnInventoryDeath(Mobile parent)
		{
			if (!Movable)
			{
				return DeathMoveResult.MoveToBackpack;
			}

			if (parent.KeepsItemsOnDeath)
			{
				return DeathMoveResult.MoveToBackpack;
			}

			if (CheckBlessed(parent))
			{
				return DeathMoveResult.MoveToBackpack;
			}

			if (CheckNewbied() && parent.Kills < 5)
			{
				return DeathMoveResult.MoveToBackpack;
			}

			if (parent.Player && Nontransferable)
			{
				return DeathMoveResult.MoveToBackpack;
			}

			return DeathMoveResult.MoveToCorpse;
		}

		/// <summary>
		///     Moves the Item to <paramref name="location" />. The Item does not change maps.
		/// </summary>
		public virtual void MoveToWorld(Point3D location)
		{
			MoveToWorld(location, m_Map);
		}

		public void LabelTo(Mobile to, int number)
		{
			to.Send(new MessageLocalized(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", ""));
		}

		public void LabelTo(Mobile to, int number, string args)
		{
			to.Send(new MessageLocalized(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", args));
		}

		public void LabelTo(Mobile to, string text)
		{
			to.Send(new UnicodeMessage(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, "ENU", "", text));
		}

		public void LabelTo(Mobile to, string format, params object[] args)
		{
			LabelTo(to, String.Format(format, args));
		}

		public void LabelToAffix(Mobile to, int number, AffixType type, string affix)
		{
			to.Send(new MessageLocalizedAffix(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", type, affix, ""));
		}

		public void LabelToAffix(Mobile to, int number, AffixType type, string affix, string args)
		{
			to.Send(new MessageLocalizedAffix(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", type, affix, args));
		}

		public virtual void LabelLootTypeTo(Mobile to)
		{
			if (m_LootType == LootType.Blessed)
			{
				LabelTo(to, 1041362); // (blessed)
			}
			else if (m_LootType == LootType.Cursed)
			{
				LabelTo(to, "(cursed)");
			}
		}

		public bool AtWorldPoint(int x, int y)
		{
			return m_Parent == null && m_Location.m_X == x && m_Location.m_Y == y;
		}

		public bool AtPoint(int x, int y)
		{
			return m_Location.m_X == x && m_Location.m_Y == y;
		}

		/// <summary>
		///     Moves the Item to a given <paramref name="location" /> and <paramref name="map" />.
		/// </summary>
		public void MoveToWorld(Point3D location, Map map)
		{
			if (Deleted)
			{
				return;
			}

			var oldLocation = GetWorldLocation();
			var oldRealLocation = m_Location;

			SetLastMoved();

			if (Parent is Mobile)
			{
				((Mobile)Parent).RemoveItem(this);
			}
			else if (Parent is Item)
			{
				((Item)Parent).RemoveItem(this);
			}

			if (m_Map != map)
			{
				var old = m_Map;

				if (m_Map != null)
				{
					m_Map.OnLeave(this);

					if (oldLocation.m_X != 0)
					{
						var eable = m_Map.GetClientsInRange(oldLocation, Core.GlobalRadarRange - 4);

						foreach (var state in eable)
						{
							var m = state.Mobile;

							if (Utility.InRange(oldLocation, m.Location, GetUpdateRange(m)))
							{
								state.Send(RemovePacket);
							}
						}

						eable.Free();
					}
				}

				m_Location = location;
				OnLocationChange(oldRealLocation);

				ReleaseWorldPackets();

				var items = LookupItems();

				if (items != null)
				{
					for (var i = 0; i < items.Count; ++i)
					{
						items[i].Map = map;
					}
				}

				m_Map = map;

				if (m_Map != null)
				{
					m_Map.OnEnter(this);
				}

				OnMapChange();

				if (m_Map != null)
				{
					var eable = m_Map.GetClientsInRange(m_Location, Core.GlobalRadarRange);

					foreach (var state in eable)
					{
						var m = state.Mobile;

						if (m.CanSee(this) && Utility.InRange(m_Location, m.Location, GetUpdateRange(m)))
						{
							SendInfoTo(state);
						}
					}

					eable.Free();
				}

				RemDelta(ItemDelta.Update);

				if (old == null || old == Map.Internal)
				{
					InvalidateProperties();
				}
			}
			else if (m_Map != null)
			{
				IPooledEnumerable<NetState> eable;

				if (oldLocation.m_X != 0)
				{
					eable = m_Map.GetClientsInRange(oldLocation, Core.GlobalMaxUpdateRange);

					foreach (var state in eable)
					{
						var m = state.Mobile;

						if (!m.InUpdateRange(location))
						{
							state.Send(RemovePacket);
						}
					}

					eable.Free();
				}

				var oldInternalLocation = m_Location;

				m_Location = location;
				OnLocationChange(oldRealLocation);

				ReleaseWorldPackets();

				eable = m_Map.GetClientsInRange(m_Location, Core.GlobalMaxUpdateRange);

				foreach (var state in eable)
				{
					var m = state.Mobile;

					if (m.CanSee(this) && m.InUpdateRange(m_Location))
					{
						SendInfoTo(state);
					}
				}

				eable.Free();

				m_Map.OnMove(oldInternalLocation, this);

				RemDelta(ItemDelta.Update);
			}
			else
			{
				Map = map;
				Location = location;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HonestyItem { get; set; }

		/// <summary>
		///     Has the item been deleted?
		/// </summary>
		public bool Deleted => GetFlag(ImplFlag.Deleted);

		[CommandProperty(AccessLevel.GameMaster)]
		public LootType LootType
		{
			get => m_LootType;
			set
			{
				if (m_LootType != value)
				{
					m_LootType = value;

					if (DisplayLootType)
					{
						InvalidateProperties();
					}
				}
			}
		}

		/// <summary>
		///		If true the item should be considered an artifact
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool IsArtifact => this is IArtifact && ((IArtifact)this).ArtifactRarity > 0;

		private static TimeSpan m_DDT = TimeSpan.FromMinutes(Config.Get("General.DefaultItemDecayTime", 60));

		public static TimeSpan DefaultDecayTime { get => m_DDT; set => m_DDT = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int DecayMultiplier => 1;

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool DefaultDecaySetting => true;

		[CommandProperty(AccessLevel.Decorator)]
		public virtual TimeSpan DecayTime => TimeSpan.FromMinutes(m_DDT.TotalMinutes * DecayMultiplier);

		[CommandProperty(AccessLevel.Decorator)]
		public virtual bool Decays => DefaultDecaySetting && Movable && Visible && !HonestyItem;

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan TimeToDecay => TimeSpan.FromMinutes((DecayTime - (DateTime.UtcNow - LastMoved)).TotalMinutes);

		public virtual bool OnDecay()
		{
			return Decays && Parent == null && Map != Map.Internal && Region.Find(Location, Map).OnDecay(this);
		}

		public void SetLastMoved()
		{
			m_LastMovedTime = DateTime.UtcNow;
		}

		public DateTime LastMoved { get => m_LastMovedTime; set => m_LastMovedTime = value; }

		public virtual bool StackIgnoreItemID => false;
		public virtual bool StackIgnoreHue => false;
		public virtual bool StackIgnoreName => false;

		public bool StackWith(Mobile from, Item dropped)
		{
			return StackWith(from, dropped, true);
		}

		public virtual bool StackWith(Mobile from, Item dropped, bool playSound)
		{
			if (WillStack(from, dropped))
			{
				if (m_LootType != dropped.m_LootType)
				{
					m_LootType = LootType.Regular;
				}

				Amount += dropped.Amount;
				dropped.Delete();

				if (playSound && from != null)
				{
					var soundID = GetDropSound();

					if (soundID == -1)
					{
						soundID = 0x42;
					}

					from.SendSound(soundID, GetWorldLocation());
				}

				return true;
			}

			return false;
		}

		public virtual bool WillStack(Mobile from, Item item)
		{
			if (item == this || item.GetType() != GetType())
			{
				return false;
			}

			if (!item.Stackable || !Stackable)
			{
				return false;
			}

			if (item.Nontransferable || Nontransferable)
			{
				return false;
			}

			if ((!item.StackIgnoreItemID || !StackIgnoreItemID) && item.ItemID != ItemID)
			{
				return false;
			}

			if ((!item.StackIgnoreHue || !StackIgnoreHue) && item.Hue != Hue)
			{
				return false;
			}

			if ((!item.StackIgnoreName || !StackIgnoreName) && item.Name != Name)
			{
				return false;
			}

			if (item.Amount + Amount > 60000)
			{
				return false;
			}

			if (Sockets == null && item.Sockets != null || Sockets != null && item.Sockets == null)
			{
				return false;
			}

			if (Sockets != null && item.Sockets != null)
			{
				if (Sockets.Any(s => !item.HasSocket(s.GetType())))
				{
					return false;
				}

				if (item.Sockets.Any(s => !HasSocket(s.GetType())))
				{
					return false;
				}
			}

			return true;
		}

		public virtual bool OnDragDrop(Mobile from, Item dropped)
		{
			if (Parent is Container)
			{
				return ((Container)Parent).OnStackAttempt(from, this, dropped);
			}

			return StackWith(from, dropped);
		}

		public Rectangle2D GetGraphicBounds()
		{
			var itemID = m_ItemID;
			var doubled = m_Amount > 1;

			if (itemID >= 0xEEA && itemID <= 0xEF2) // Are we coins?
			{
				var coinBase = (itemID - 0xEEA) / 3;
				coinBase *= 3;
				coinBase += 0xEEA;

				doubled = false;

				if (m_Amount <= 1)
				{
					// A single coin
					itemID = coinBase;
				}
				else if (m_Amount <= 5)
				{
					// A stack of coins
					itemID = coinBase + 1;
				}
				else // m_Amount > 5
				{
					// A pile of coins
					itemID = coinBase + 2;
				}
			}

			var bounds = ItemBounds.Table[itemID & 0x3FFF];

			if (doubled)
			{
				bounds.Set(bounds.X, bounds.Y, bounds.Width + 5, bounds.Height + 5);
			}

			return bounds;
		}

		[CommandProperty(AccessLevel.Decorator)]
		public bool Stackable { get => GetFlag(ImplFlag.Stackable); set => SetFlag(ImplFlag.Stackable, value); }

		private readonly object _rpl = new object();

		public Packet RemovePacket
		{
			get
			{
				if (m_RemovePacket == null)
				{
					lock (_rpl)
					{
						if (m_RemovePacket == null)
						{
							m_RemovePacket = new RemoveItem(this);
							m_RemovePacket.SetStatic();
						}
					}
				}

				return m_RemovePacket;
			}
		}

		private readonly object _opll = new object();

		public Packet OPLPacket
		{
			get
			{
				if (m_OPLPacket == null)
				{
					lock (_opll)
					{
						if (m_OPLPacket == null)
						{
							m_OPLPacket = new OPLInfo(PropertyList);
							m_OPLPacket.SetStatic();
						}
					}
				}

				return m_OPLPacket;
			}
		}

		public ObjectPropertyList PropertyList
		{
			get
			{
				if (m_PropertyList == null)
				{
					m_PropertyList = new ObjectPropertyList(this);

					GetProperties(m_PropertyList);
					AppendChildProperties(m_PropertyList);

					m_PropertyList.Terminate();
					m_PropertyList.SetStatic();
				}

				return m_PropertyList;
			}
		}

		public virtual void AppendChildProperties(ObjectPropertyList list)
		{
			if (m_Parent is Item item)
			{
				item.GetChildProperties(list, this);
			}
			else if (m_Parent is Mobile mobile)
			{
				mobile.GetChildProperties(list, this);
			}
		}

		public virtual void AppendChildNameProperties(ObjectPropertyList list)
		{
			if (m_Parent is Item item)
			{
				item.GetChildNameProperties(list, this);
			}
			else if (m_Parent is Mobile mobile)
			{
				mobile.GetChildNameProperties(list, this);
			}
		}

		public void ClearProperties()
		{
			Packet.Release(ref m_PropertyList);
			Packet.Release(ref m_OPLPacket);
		}

		public void InvalidateProperties()
		{
			if (m_Map != null && m_Map != Map.Internal && !World.Loading)
			{
				var oldList = m_PropertyList;
				m_PropertyList = null;
				var newList = PropertyList;

				if (oldList == null || oldList.Hash != newList.Hash)
				{
					Packet.Release(ref m_OPLPacket);
					Delta(ItemDelta.Properties);
				}
			}
			else
			{
				ClearProperties();
			}
		}

		private readonly object _wpl = new object();

		public virtual Packet WorldPacket
		{
			get
			{
				// This needs to be invalidated when any of the following changes:
				//  - ItemID
				//  - Amount
				//  - Location
				//  - Hue
				//  - Packet Flags
				//  - Direction

				if (m_WorldPacket == null)
				{
					lock (_wpl)
					{
						if (m_WorldPacket == null)
						{
							m_WorldPacket = new WorldItem(this);
							m_WorldPacket.SetStatic();
						}
					}
				}

				return m_WorldPacket;
			}
		}

		public virtual void ReleaseWorldPackets()
		{
			Packet.Release(ref m_WorldPacket);
		}

		[CommandProperty(AccessLevel.Decorator)]
		public bool Visible
		{
			get => GetFlag(ImplFlag.Visible);
			set
			{
				if (GetFlag(ImplFlag.Visible) != value)
				{
					SetFlag(ImplFlag.Visible, value);
					ReleaseWorldPackets();

					if (m_Map != null && m_Map != Map.Internal)
					{
						var worldLoc = GetWorldLocation();

						var eable = m_Map.GetClientsInRange(worldLoc, Core.GlobalMaxUpdateRange);

						foreach (var state in eable)
						{
							var m = state.Mobile;

							if (!m.CanSee(this) && m.InRange(worldLoc, GetUpdateRange(m)))
							{
								state.Send(RemovePacket);
							}
						}

						eable.Free();
					}

					Delta(ItemDelta.Update);
				}
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public bool Movable
		{
			get => GetFlag(ImplFlag.Movable);
			set
			{
				if (GetFlag(ImplFlag.Movable) != value)
				{
					SetFlag(ImplFlag.Movable, value);
					ReleaseWorldPackets();
					Delta(ItemDelta.Update);
				}
			}
		}

		public virtual bool ForceShowProperties => IsLockedDown || IsSecure;

		public virtual int GetPacketFlags()
		{
			var flags = 0;

			if (!Visible)
			{
				flags |= 0x80;
			}

			if (Movable || ForceShowProperties)
			{
				flags |= 0x20;
			}

			return flags;
		}

		public virtual bool OnMoveOff(Mobile m)
		{
			return true;
		}

		public virtual bool OnMoveOver(Mobile m)
		{
			return true;
		}

		public virtual bool HandlesOnMovement => false;

		public virtual void OnMovement(Mobile m, Point3D oldLocation)
		{ }

		public void Internalize()
		{
			MoveToWorld(Point3D.Zero, Map.Internal);
		}

		public virtual void OnMapChange()
		{ }

		public virtual void OnRemoved(object parent)
		{ }

		public virtual void OnAdded(object parent)
		{ }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public Map Map
		{
			get => m_Map;
			set
			{
				if (m_Map != value)
				{
					var old = m_Map;

					if (m_Map != null && m_Parent == null)
					{
						m_Map.OnLeave(this);
						SendRemovePacket();
					}

					var items = LookupItems();

					if (items != null)
					{
						for (var i = 0; i < items.Count; ++i)
						{
							items[i].Map = value;
						}
					}

					m_Map = value;

					if (m_Map != null && m_Parent == null)
					{
						m_Map.OnEnter(this);
					}

					Delta(ItemDelta.Update);

					OnMapChange();

					if (old == null || old == Map.Internal)
					{
						InvalidateProperties();
					}
				}
			}
		}

		[Flags]
		private enum SaveFlag
		{
			None = 0x00000000,
			Direction = 0x00000001,
			Bounce = 0x00000002,
			LootType = 0x00000004,
			LocationFull = 0x00000008,
			ItemID = 0x00000010,
			Hue = 0x00000020,
			Amount = 0x00000040,
			Layer = 0x00000080,
			Name = 0x00000100,
			Parent = 0x00000200,
			Items = 0x00000400,
			WeightNot1or0 = 0x00000800,
			Map = 0x00001000,
			Visible = 0x00002000,
			Movable = 0x00004000,
			Stackable = 0x00008000,
			WeightIs0 = 0x00010000,
			LocationSByteZ = 0x00020000,
			LocationShortXY = 0x00040000,
			LocationByteXY = 0x00080000,
			ImplFlags = 0x00100000,
			InsuredFor = 0x00200000,
			BlessedFor = 0x00400000,
			HeldBy = 0x00800000,
			IntWeight = 0x01000000,
			SavedFlags = 0x02000000,
			NullWeight = 0x04000000,
			Light = 0x08000000
		}

		private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
		{
			if (setIf)
			{
				flags |= toSet;
			}
		}

		private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
		{
			return (flags & toGet) != 0;
		}

		int ISerializable.TypeReference => m_TypeRef;

		int ISerializable.SerialIdentity => m_Serial;

		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(14); // version

			// 14
			writer.Write(Sockets != null ? Sockets.Count : 0);

			if (Sockets != null)
			{
				foreach (var socket in Sockets)
				{
					ItemSocket.Save(socket, writer);
				}
			}

			// 13: Merge sync
			// 12: Light no longer backed by Direction

			// 11
			writer.Write(m_GridLocation);

			// 10: Honesty moved to ItemSockets

			// 9
			var flags = SaveFlag.None;

			int x = m_Location.m_X, y = m_Location.m_Y, z = m_Location.m_Z;

			if (x != 0 || y != 0 || z != 0)
			{
				if (x >= Int16.MinValue && x <= Int16.MaxValue && y >= Int16.MinValue && y <= Int16.MaxValue && z >= SByte.MinValue &&
					z <= SByte.MaxValue)
				{
					if (x != 0 || y != 0)
					{
						if (x >= Byte.MinValue && x <= Byte.MaxValue && y >= Byte.MinValue && y <= Byte.MaxValue)
						{
							flags |= SaveFlag.LocationByteXY;
						}
						else
						{
							flags |= SaveFlag.LocationShortXY;
						}
					}

					if (z != 0)
					{
						flags |= SaveFlag.LocationSByteZ;
					}
				}
				else
				{
					flags |= SaveFlag.LocationFull;
				}
			}

			var info = LookupCompactInfo();
			var items = LookupItems();

			if (m_Direction != Direction.North)
			{
				flags |= SaveFlag.Direction;
			}

			if (m_Light != 0)
			{
				flags |= SaveFlag.Light;
			}

			if (info != null && info.m_Bounce != null)
			{
				flags |= SaveFlag.Bounce;
			}

			if (m_LootType != LootType.Regular)
			{
				flags |= SaveFlag.LootType;
			}

			if (m_ItemID != 0)
			{
				flags |= SaveFlag.ItemID;
			}

			if (m_Hue != 0)
			{
				flags |= SaveFlag.Hue;
			}

			if (m_Amount != 1)
			{
				flags |= SaveFlag.Amount;
			}

			if (m_Layer != Layer.Invalid)
			{
				flags |= SaveFlag.Layer;
			}

			if (info != null && info.m_Name != null)
			{
				flags |= SaveFlag.Name;
			}

			if (m_Parent != null)
			{
				flags |= SaveFlag.Parent;
			}

			if (items != null && items.Count > 0)
			{
				flags |= SaveFlag.Items;
			}

			if (m_Map != Map.Internal)
			{
				flags |= SaveFlag.Map;
			}

			if (info != null && info.m_BlessedFor != null && !info.m_BlessedFor.Deleted)
			{
				flags |= SaveFlag.BlessedFor;
			}

			if (info != null && info.m_HeldBy != null && !info.m_HeldBy.Deleted)
			{
				flags |= SaveFlag.HeldBy;
			}

			if (info != null && info.m_SavedFlags != 0)
			{
				flags |= SaveFlag.SavedFlags;
			}

			if (info == null || info.m_Weight == -1)
			{
				flags |= SaveFlag.NullWeight;
			}
			else if (info.m_Weight == 0.0)
			{
				flags |= SaveFlag.WeightIs0;
			}
			else if (info.m_Weight != 1.0)
			{
				if (info.m_Weight == (int)info.m_Weight)
				{
					flags |= SaveFlag.IntWeight;
				}
				else
				{
					flags |= SaveFlag.WeightNot1or0;
				}
			}

			var implFlags = m_Flags & (ImplFlag.Visible | ImplFlag.Movable | ImplFlag.Stackable | ImplFlag.Insured |
										ImplFlag.PayedInsurance | ImplFlag.QuestItem);

			if (implFlags != (ImplFlag.Visible | ImplFlag.Movable))
			{
				flags |= SaveFlag.ImplFlags;
			}

			writer.Write((int)flags);

			/* begin last moved time optimization */
			var ticks = m_LastMovedTime.Ticks;
			var now = DateTime.UtcNow.Ticks;

			TimeSpan d;

			try
			{
				d = new TimeSpan(ticks - now);
			}
			catch
			{
				if (ticks < now)
				{
					d = TimeSpan.MaxValue;
				}
				else
				{
					d = TimeSpan.MaxValue;
				}
			}

			var minutes = -d.TotalMinutes;

			if (minutes < Int32.MinValue)
			{
				minutes = Int32.MinValue;
			}
			else if (minutes > Int32.MaxValue)
			{
				minutes = Int32.MaxValue;
			}

			writer.WriteEncodedInt((int)minutes);
			/* end */

			if (GetSaveFlag(flags, SaveFlag.Direction))
			{
				writer.Write((byte)m_Direction);
			}

			if (GetSaveFlag(flags, SaveFlag.Light))
			{
				writer.Write((byte)m_Light);
			}

			if (GetSaveFlag(flags, SaveFlag.Bounce))
			{
				// ReSharper disable once PossibleNullReferenceException
				BounceInfo.Serialize(info.m_Bounce, writer);
			}

			if (GetSaveFlag(flags, SaveFlag.LootType))
			{
				writer.Write((byte)m_LootType);
			}

			if (GetSaveFlag(flags, SaveFlag.LocationFull))
			{
				writer.WriteEncodedInt(x);
				writer.WriteEncodedInt(y);
				writer.WriteEncodedInt(z);
			}
			else
			{
				if (GetSaveFlag(flags, SaveFlag.LocationByteXY))
				{
					writer.Write((byte)x);
					writer.Write((byte)y);
				}
				else if (GetSaveFlag(flags, SaveFlag.LocationShortXY))
				{
					writer.Write((short)x);
					writer.Write((short)y);
				}

				if (GetSaveFlag(flags, SaveFlag.LocationSByteZ))
				{
					writer.Write((sbyte)z);
				}
			}

			if (GetSaveFlag(flags, SaveFlag.ItemID))
			{
				writer.WriteEncodedInt(m_ItemID);
			}

			if (GetSaveFlag(flags, SaveFlag.Hue))
			{
				writer.WriteEncodedInt(m_Hue);
			}

			if (GetSaveFlag(flags, SaveFlag.Amount))
			{
				writer.WriteEncodedInt(m_Amount);
			}

			if (GetSaveFlag(flags, SaveFlag.Layer))
			{
				writer.Write((byte)m_Layer);
			}

			if (GetSaveFlag(flags, SaveFlag.Name))
			{
				writer.Write(info.m_Name);
			}

			if (GetSaveFlag(flags, SaveFlag.Parent))
			{
				if (m_Parent is Mobile mobile && !mobile.Deleted)
				{
					writer.Write(mobile.Serial);
				}
				else if (m_Parent is Item item && !item.Deleted)
				{
					writer.Write(item.Serial);
				}
				else
				{
					writer.Write(Serial.MinusOne);
				}
			}

			if (GetSaveFlag(flags, SaveFlag.Items))
			{
				writer.Write(items, false);
			}

			if (GetSaveFlag(flags, SaveFlag.IntWeight))
			{
				writer.WriteEncodedInt((int)info.m_Weight);
			}
			else if (GetSaveFlag(flags, SaveFlag.WeightNot1or0))
			{
				writer.Write(info.m_Weight);
			}

			if (GetSaveFlag(flags, SaveFlag.Map))
			{
				writer.Write(m_Map);
			}

			if (GetSaveFlag(flags, SaveFlag.ImplFlags))
			{
				writer.WriteEncodedInt((int)implFlags);
			}

			if (GetSaveFlag(flags, SaveFlag.InsuredFor))
			{
				writer.Write((Mobile)null);
			}

			if (GetSaveFlag(flags, SaveFlag.BlessedFor))
			{
				writer.Write(info.m_BlessedFor);
			}

			if (GetSaveFlag(flags, SaveFlag.HeldBy))
			{
				writer.Write(info.m_HeldBy);
			}

			if (GetSaveFlag(flags, SaveFlag.SavedFlags))
			{
				writer.WriteEncodedInt(info.m_SavedFlags);
			}
		}

		public IPooledEnumerable<IEntity> GetObjectsInRange(int range)
		{
			var map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<IEntity>.Instance;
			}

			if (m_Parent == null)
			{
				return map.GetObjectsInRange(m_Location, range);
			}

			return map.GetObjectsInRange(GetWorldLocation(), range);
		}

		public IPooledEnumerable<Item> GetItemsInRange(int range)
		{
			var map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<Item>.Instance;
			}

			if (m_Parent == null)
			{
				return map.GetItemsInRange(m_Location, range);
			}

			return map.GetItemsInRange(GetWorldLocation(), range);
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(int range)
		{
			var map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<Mobile>.Instance;
			}

			if (m_Parent == null)
			{
				return map.GetMobilesInRange(m_Location, range);
			}

			return map.GetMobilesInRange(GetWorldLocation(), range);
		}

		public IPooledEnumerable<NetState> GetClientsInRange(int range)
		{
			var map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<NetState>.Instance;
			}

			if (m_Parent == null)
			{
				return map.GetClientsInRange(m_Location, range);
			}

			return map.GetClientsInRange(GetWorldLocation(), range);
		}

		private static int m_LockedDownFlag;
		private static int m_SecureFlag;

		public static int LockedDownFlag { get => m_LockedDownFlag; set => m_LockedDownFlag = value; }

		public static int SecureFlag { get => m_SecureFlag; set => m_SecureFlag = value; }

		public bool IsLockedDown
		{
			get => GetTempFlag(m_LockedDownFlag);
			set
			{
				SetTempFlag(m_LockedDownFlag, value);
				InvalidateProperties();

				OnLockDownChange();
			}
		}

		public virtual void OnLockDownChange()
		{
		}

		public bool IsSecure
		{
			get => GetTempFlag(m_SecureFlag);
			set
			{
				SetTempFlag(m_SecureFlag, value);
				InvalidateProperties();

				OnSecureChange();
			}
		}

		public virtual void OnSecureChange()
		{
		}

		public bool GetTempFlag(int flag)
		{
			var info = LookupCompactInfo();

			if (info == null)
			{
				return false;
			}

			return (info.m_TempFlags & flag) != 0;
		}

		public void SetTempFlag(int flag, bool value)
		{
			var info = AcquireCompactInfo();

			if (value)
			{
				info.m_TempFlags |= flag;
			}
			else
			{
				info.m_TempFlags &= ~flag;
			}

			if (info.m_TempFlags == 0)
			{
				VerifyCompactInfo();
			}
		}

		public bool GetSavedFlag(int flag)
		{
			var info = LookupCompactInfo();

			if (info == null)
			{
				return false;
			}

			return (info.m_SavedFlags & flag) != 0;
		}

		public void SetSavedFlag(int flag, bool value)
		{
			var info = AcquireCompactInfo();

			if (value)
			{
				info.m_SavedFlags |= flag;
			}
			else
			{
				info.m_SavedFlags &= ~flag;
			}

			if (info.m_SavedFlags == 0)
			{
				VerifyCompactInfo();
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.ReadInt();

			SetLastMoved();

			switch (version)
			{
				case 14:
				var socketCount = reader.ReadInt();

				for (var i = 0; i < socketCount; i++)
				{
					ItemSocket.Load(this, reader);
				}

				goto case 13;
				case 13:
				case 12:
				case 11:
				m_GridLocation = reader.ReadByte();
				goto case 10;
				case 10:
					{
						// Honesty removed to ItemSockets
						if (version < 14)
						{
							reader.ReadDateTime();
							reader.ReadBool();
							reader.ReadMobile();
							reader.ReadString();

							HonestyItem = reader.ReadBool();
						}

						goto case 9;
					}
				case 9:
				case 8:
				case 7:
				case 6:
					{
						var flags = (SaveFlag)reader.ReadInt();

						if (version < 7)
						{
							LastMoved = reader.ReadDeltaTime();
						}
						else
						{
							var minutes = reader.ReadEncodedInt();

							try
							{
								LastMoved = DateTime.UtcNow - TimeSpan.FromMinutes(minutes);
							}
							catch
							{
								LastMoved = DateTime.UtcNow;
							}
						}

						if (GetSaveFlag(flags, SaveFlag.Direction))
						{
							m_Direction = (Direction)reader.ReadByte();
						}

						if (GetSaveFlag(flags, SaveFlag.Light))
						{
							m_Light = (LightType)reader.ReadByte();
						}
						else if (version < 12)
						{
							m_Light = (LightType)m_Direction;
						}

						if (GetSaveFlag(flags, SaveFlag.Bounce))
						{
							AcquireCompactInfo().m_Bounce = BounceInfo.Deserialize(reader);
						}

						if (GetSaveFlag(flags, SaveFlag.LootType))
						{
							m_LootType = (LootType)reader.ReadByte();
						}

						int x = 0, y = 0, z = 0;

						if (GetSaveFlag(flags, SaveFlag.LocationFull))
						{
							x = reader.ReadEncodedInt();
							y = reader.ReadEncodedInt();
							z = reader.ReadEncodedInt();
						}
						else
						{
							if (GetSaveFlag(flags, SaveFlag.LocationByteXY))
							{
								x = reader.ReadByte();
								y = reader.ReadByte();
							}
							else if (GetSaveFlag(flags, SaveFlag.LocationShortXY))
							{
								x = reader.ReadShort();
								y = reader.ReadShort();
							}

							if (GetSaveFlag(flags, SaveFlag.LocationSByteZ))
							{
								z = reader.ReadSByte();
							}
						}

						m_Location = new Point3D(x, y, z);

						if (GetSaveFlag(flags, SaveFlag.ItemID))
						{
							m_ItemID = reader.ReadEncodedInt();
						}

						if (GetSaveFlag(flags, SaveFlag.Hue))
						{
							m_Hue = reader.ReadEncodedInt();
						}

						if (GetSaveFlag(flags, SaveFlag.Amount))
						{
							m_Amount = reader.ReadEncodedInt();
						}
						else
						{
							m_Amount = 1;
						}

						if (GetSaveFlag(flags, SaveFlag.Layer))
						{
							m_Layer = (Layer)reader.ReadByte();
						}

						if (GetSaveFlag(flags, SaveFlag.Name))
						{
							var name = reader.ReadString();

							if (name != DefaultName)
							{
								AcquireCompactInfo().m_Name = name;
							}
						}

						if (GetSaveFlag(flags, SaveFlag.Parent))
						{
							Serial parent = reader.ReadInt();

							if (parent.IsMobile)
							{
								m_Parent = World.FindMobile(parent);
							}
							else if (parent.IsItem)
							{
								m_Parent = World.FindItem(parent);
							}
							else
							{
								m_Parent = null;
							}

							if (m_Parent == null && (parent.IsMobile || parent.IsItem))
							{
								Delete();
							}
						}

						if (GetSaveFlag(flags, SaveFlag.Items))
						{
							var items = reader.ReadStrongItemList();

							if (this is Container)
							{
								(this as Container).m_Items = items;
							}
							else
							{
								AcquireCompactInfo().m_Items = items;
							}
						}

						if (version < 8 || !GetSaveFlag(flags, SaveFlag.NullWeight))
						{
							double weight;

							if (GetSaveFlag(flags, SaveFlag.IntWeight))
							{
								weight = reader.ReadEncodedInt();
							}
							else if (GetSaveFlag(flags, SaveFlag.WeightNot1or0))
							{
								weight = reader.ReadDouble();
							}
							else if (GetSaveFlag(flags, SaveFlag.WeightIs0))
							{
								weight = 0.0;
							}
							else
							{
								weight = 1.0;
							}

							if (weight != DefaultWeight)
							{
								AcquireCompactInfo().m_Weight = weight;
							}
						}

						if (GetSaveFlag(flags, SaveFlag.Map))
						{
							m_Map = reader.ReadMap();
						}
						else
						{
							m_Map = Map.Internal;
						}

						if (GetSaveFlag(flags, SaveFlag.Visible))
						{
							SetFlag(ImplFlag.Visible, reader.ReadBool());
						}
						else
						{
							SetFlag(ImplFlag.Visible, true);
						}

						if (GetSaveFlag(flags, SaveFlag.Movable))
						{
							SetFlag(ImplFlag.Movable, reader.ReadBool());
						}
						else
						{
							SetFlag(ImplFlag.Movable, true);
						}

						if (GetSaveFlag(flags, SaveFlag.Stackable))
						{
							SetFlag(ImplFlag.Stackable, reader.ReadBool());
						}

						if (GetSaveFlag(flags, SaveFlag.ImplFlags))
						{
							m_Flags = (ImplFlag)reader.ReadEncodedInt();
						}

						if (GetSaveFlag(flags, SaveFlag.InsuredFor))
						{
							/*m_InsuredFor = */
							reader.ReadMobile();
						}

						if (GetSaveFlag(flags, SaveFlag.BlessedFor))
						{
							AcquireCompactInfo().m_BlessedFor = reader.ReadMobile();
						}

						if (GetSaveFlag(flags, SaveFlag.HeldBy))
						{
							AcquireCompactInfo().m_HeldBy = reader.ReadMobile();
						}

						if (GetSaveFlag(flags, SaveFlag.SavedFlags))
						{
							AcquireCompactInfo().m_SavedFlags = reader.ReadEncodedInt();
						}

						if (m_Map != null && m_Parent == null)
						{
							m_Map.OnEnter(this);
						}

						break;
					}
				case 5:
					{
						var flags = (SaveFlag)reader.ReadInt();

						LastMoved = reader.ReadDeltaTime();

						if (GetSaveFlag(flags, SaveFlag.Direction))
						{
							m_Direction = (Direction)reader.ReadByte();
						}

						if (GetSaveFlag(flags, SaveFlag.Bounce))
						{
							AcquireCompactInfo().m_Bounce = BounceInfo.Deserialize(reader);
						}

						if (GetSaveFlag(flags, SaveFlag.LootType))
						{
							m_LootType = (LootType)reader.ReadByte();
						}

						if (GetSaveFlag(flags, SaveFlag.LocationFull))
						{
							m_Location = reader.ReadPoint3D();
						}

						if (GetSaveFlag(flags, SaveFlag.ItemID))
						{
							m_ItemID = reader.ReadInt();
						}

						if (GetSaveFlag(flags, SaveFlag.Hue))
						{
							m_Hue = reader.ReadInt();
						}

						if (GetSaveFlag(flags, SaveFlag.Amount))
						{
							m_Amount = reader.ReadInt();
						}
						else
						{
							m_Amount = 1;
						}

						if (GetSaveFlag(flags, SaveFlag.Layer))
						{
							m_Layer = (Layer)reader.ReadByte();
						}

						if (GetSaveFlag(flags, SaveFlag.Name))
						{
							var name = reader.ReadString();

							if (name != DefaultName)
							{
								AcquireCompactInfo().m_Name = name;
							}
						}

						if (GetSaveFlag(flags, SaveFlag.Parent))
						{
							Serial parent = reader.ReadInt();

							if (parent.IsMobile)
							{
								m_Parent = World.FindMobile(parent);
							}
							else if (parent.IsItem)
							{
								m_Parent = World.FindItem(parent);
							}
							else
							{
								m_Parent = null;
							}

							if (m_Parent == null && (parent.IsMobile || parent.IsItem))
							{
								Delete();
							}
						}

						if (GetSaveFlag(flags, SaveFlag.Items))
						{
							var items = reader.ReadStrongItemList();

							if (this is Container)
							{
								(this as Container).m_Items = items;
							}
							else
							{
								AcquireCompactInfo().m_Items = items;
							}
						}

						double weight;

						if (GetSaveFlag(flags, SaveFlag.IntWeight))
						{
							weight = reader.ReadEncodedInt();
						}
						else if (GetSaveFlag(flags, SaveFlag.WeightNot1or0))
						{
							weight = reader.ReadDouble();
						}
						else if (GetSaveFlag(flags, SaveFlag.WeightIs0))
						{
							weight = 0.0;
						}
						else
						{
							weight = 1.0;
						}

						if (weight != DefaultWeight)
						{
							AcquireCompactInfo().m_Weight = weight;
						}

						if (GetSaveFlag(flags, SaveFlag.Map))
						{
							m_Map = reader.ReadMap();
						}
						else
						{
							m_Map = Map.Internal;
						}

						if (GetSaveFlag(flags, SaveFlag.Visible))
						{
							SetFlag(ImplFlag.Visible, reader.ReadBool());
						}
						else
						{
							SetFlag(ImplFlag.Visible, true);
						}

						if (GetSaveFlag(flags, SaveFlag.Movable))
						{
							SetFlag(ImplFlag.Movable, reader.ReadBool());
						}
						else
						{
							SetFlag(ImplFlag.Movable, true);
						}

						if (GetSaveFlag(flags, SaveFlag.Stackable))
						{
							SetFlag(ImplFlag.Stackable, reader.ReadBool());
						}

						if (m_Map != null && m_Parent == null)
						{
							m_Map.OnEnter(this);
						}

						break;
					}
				case 4: // Just removed variables
				case 3:
					{
						m_Direction = (Direction)reader.ReadInt();

						goto case 2;
					}
				case 2:
					{
						AcquireCompactInfo().m_Bounce = BounceInfo.Deserialize(reader);
						LastMoved = reader.ReadDeltaTime();

						goto case 1;
					}
				case 1:
					{
						m_LootType = (LootType)reader.ReadByte(); //m_Newbied = reader.ReadBool();

						goto case 0;
					}
				case 0:
					{
						m_Location = reader.ReadPoint3D();
						m_ItemID = reader.ReadInt();
						m_Hue = reader.ReadInt();
						m_Amount = reader.ReadInt();
						m_Layer = (Layer)reader.ReadByte();

						var name = reader.ReadString();

						if (name != DefaultName)
						{
							AcquireCompactInfo().m_Name = name;
						}

						Serial parent = reader.ReadInt();

						if (parent.IsMobile)
						{
							m_Parent = World.FindMobile(parent);
						}
						else if (parent.IsItem)
						{
							m_Parent = World.FindItem(parent);
						}
						else
						{
							m_Parent = null;
						}

						if (m_Parent == null && (parent.IsMobile || parent.IsItem))
						{
							Delete();
						}

						var count = reader.ReadInt();

						if (count > 0)
						{
							var items = new List<Item>(count);

							for (var i = 0; i < count; ++i)
							{
								var item = reader.ReadItem();

								if (item != null)
								{
									items.Add(item);
								}
							}

							if (this is Container)
							{
								(this as Container).m_Items = items;
							}
							else
							{
								AcquireCompactInfo().m_Items = items;
							}
						}

						var weight = reader.ReadDouble();

						if (weight != DefaultWeight)
						{
							AcquireCompactInfo().m_Weight = weight;
						}

						if (version <= 3)
						{
							reader.ReadInt();
							reader.ReadInt();
							reader.ReadInt();
						}

						m_Map = reader.ReadMap();
						SetFlag(ImplFlag.Visible, reader.ReadBool());
						SetFlag(ImplFlag.Movable, reader.ReadBool());

						if (version <= 3)
						{
							/*m_Deleted =*/
							reader.ReadBool();
						}

						Stackable = reader.ReadBool();

						if (m_Map != null && m_Parent == null)
						{
							m_Map.OnEnter(this);
						}

						break;
					}
			}

			if (HeldBy != null)
			{
				Timer.DelayCall(TimeSpan.Zero, FixHolding_Sandbox);
			}

			VerifyCompactInfo();

			UpdateLight();
		}

		private void FixHolding_Sandbox()
		{
			var heldBy = HeldBy;

			if (heldBy != null)
			{
				if (GetBounce() != null)
				{
					Bounce(heldBy);
				}
				else
				{
					heldBy.Holding = null;
					heldBy.AddToBackpack(this);
					ClearBounce();
				}
			}
		}

		public virtual int GetMaxUpdateRange()
		{
			return Core.GlobalMaxUpdateRange;
		}

		public virtual int GetUpdateRange(Mobile m)
		{
			return m.NetState == null ? Core.GlobalUpdateRange : m.NetState.UpdateRange;
		}

		public void SendInfoTo(NetState state)
		{
			SendInfoTo(state, state.Mobile != null);
		}

		public virtual void SendInfoTo(NetState state, bool sendOplPacket)
		{
			state.Send(GetWorldPacketFor(state));

			if (sendOplPacket)
			{
				state.Send(OPLPacket);
			}
		}

		protected virtual Packet GetWorldPacketFor(NetState state)
		{
			return WorldPacket;
		}

		public virtual bool IsVirtualItem => false;

		public virtual int GetTotal(TotalType type)
		{
			return 0;
		}

		public virtual void UpdateTotal(Item sender, TotalType type, int delta)
		{
			if (!IsVirtualItem)
			{
				if (m_Parent is Item item)
				{
					item.UpdateTotal(sender, type, delta);
				}
				else if (m_Parent is Mobile mobile)
				{
					mobile.UpdateTotal(sender, type, delta);
				}
				else if (HeldBy != null)
				{
					HeldBy.UpdateTotal(sender, type, delta);
				}
			}
		}

		public virtual void UpdateTotals()
		{ }

		public virtual int LabelNumber
		{
			get
			{
				if (m_ItemID < 0x4000)
				{
					return 1020000 + m_ItemID;
				}

				return 1078872 + m_ItemID;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalGold => GetTotal(TotalType.Gold);

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalItems => GetTotal(TotalType.Items);

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalWeight => GetTotal(TotalType.Weight);

		public virtual double DefaultWeight
		{
			get
			{
				if (m_ItemID < 0 || m_ItemID > TileData.MaxItemValue || this is BaseMulti)
				{
					return 0;
				}

				var weight = TileData.ItemTable[m_ItemID].Weight;

				if (weight == 255 || weight == 0)
				{
					weight = 1;
				}

				return weight;
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public double Weight
		{
			get
			{
				var info = LookupCompactInfo();

				if (info != null && info.m_Weight != -1)
				{
					return info.m_Weight;
				}

				return DefaultWeight;
			}
			set
			{
				if (Weight != value)
				{
					var info = AcquireCompactInfo();

					var oldPileWeight = PileWeight;

					info.m_Weight = value;

					if (info.m_Weight == -1)
					{
						VerifyCompactInfo();
					}

					var newPileWeight = PileWeight;

					UpdateTotal(this, TotalType.Weight, newPileWeight - oldPileWeight);

					InvalidateProperties();
				}
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int PileWeight => (int)Math.Ceiling(Weight * Amount);

		public virtual int HuedItemID => m_ItemID;

		[Hue, CommandProperty(AccessLevel.GameMaster)]
		public virtual int Hue
		{
			get => m_Hue;
			set
			{
				if (m_Hue != value)
				{
					m_Hue = value;
					ReleaseWorldPackets();

					Delta(ItemDelta.Update);
				}
			}
		}

		public virtual bool HiddenQuestItemHue { get; set; }

		public int QuestItemHue => HiddenQuestItemHue ? Hue : 0x04EA;

		public virtual bool Nontransferable => QuestItem;

		public virtual void HandleInvalidTransfer(Mobile from)
		{
			if (QuestItem)
			{
				from.SendLocalizedMessage(1049343);
				// You can only drop quest items into the top-most level of your backpack while you still need them for your quest.
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual Layer Layer
		{
			get => m_Layer;
			set
			{
				if (m_Layer != value)
				{
					m_Layer = value;

					Delta(ItemDelta.EquipOnly);
				}
			}
		}

		public List<Item> Items
		{
			get
			{
				var items = LookupItems();

				if (items == null)
				{
					items = EmptyItems;
				}

				return items;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public object RootParent
		{
			get
			{
				var p = m_Parent;

				while (p is Item item)
				{
					if (item.m_Parent == null)
					{
						break;
					}

					p = item.m_Parent;
				}

				return p;
			}
		}

		public bool ParentsContain<T>() where T : Item
		{
			var p = m_Parent;

			while (p is Item item)
			{
				if (item is T)
				{
					return true;
				}

				if (item.m_Parent == null)
				{
					break;
				}

				p = item.m_Parent;
			}

			return false;
		}

		public virtual void AddItem(Item item)
		{
			if (item == null || item.Deleted || item.m_Parent == this)
			{
				return;
			}

			if (item == this)
			{
				Console.WriteLine(
					"Warning: Adding item to itself: [0x{0:X} {1}].AddItem( [0x{2:X} {3}] )",
					Serial.Value,
					GetType().Name,
					item.Serial.Value,
					item.GetType().Name);
				Console.WriteLine(new StackTrace());
				return;
			}

			if (IsChildOf(item))
			{
				Console.WriteLine(
					"Warning: Adding parent item to child: [0x{0:X} {1}].AddItem( [0x{2:X} {3}] )",
					Serial.Value,
					GetType().Name,
					item.Serial.Value,
					item.GetType().Name);
				Console.WriteLine(new StackTrace());
				return;
			}

			if (item.m_Parent is Mobile mParent)
			{
				mParent.RemoveItem(item);
			}
			else if (item.m_Parent is Item iParent)
			{
				iParent.RemoveItem(item);
			}
			else
			{
				item.SendRemovePacket();
			}

			item.Parent = this;
			item.Map = m_Map;

			var items = AcquireItems();

			items.Add(item);

			if (!item.IsVirtualItem)
			{
				UpdateTotal(item, TotalType.Gold, item.TotalGold);
				UpdateTotal(item, TotalType.Items, item.TotalItems + 1);
				UpdateTotal(item, TotalType.Weight, item.TotalWeight + item.PileWeight);
			}

			item.Delta(ItemDelta.Update);

			item.OnAdded(this);
			OnItemAdded(item);
		}

		private static readonly List<Item> m_DeltaQueue = new List<Item>();

		public void Delta(ItemDelta flags)
		{
			if (m_Map == null || m_Map == Map.Internal)
			{
				return;
			}

			m_DeltaFlags |= flags;

			if (!GetFlag(ImplFlag.InQueue))
			{
				SetFlag(ImplFlag.InQueue, true);

				m_DeltaQueue.Add(this);
			}

			Core.Set();
		}

		public void RemDelta(ItemDelta flags)
		{
			m_DeltaFlags &= ~flags;

			if (GetFlag(ImplFlag.InQueue) && m_DeltaFlags == ItemDelta.None)
			{
				SetFlag(ImplFlag.InQueue, false);

				m_DeltaQueue.Remove(this);
			}
		}

		private bool m_NoMoveHS;

		public bool NoMoveHS { get => m_NoMoveHS; set => m_NoMoveHS = value; }

		public void ProcessDelta()
		{
			var flags = m_DeltaFlags;

			SetFlag(ImplFlag.InQueue, false);
			m_DeltaFlags = ItemDelta.None;

			var map = m_Map;

			if (map != null && !Deleted)
			{
				var sendOPLUpdate = (flags & ItemDelta.Properties) != 0;

				var contParent = m_Parent as Container;

				if (contParent != null && !contParent.IsPublicContainer)
				{
					if ((flags & ItemDelta.Update) != 0)
					{
						var worldLoc = GetWorldLocation();

						var rootParent = contParent.RootParent as Mobile;
						Mobile tradeRecip = null;

						if (rootParent != null)
						{
							var ns = rootParent.NetState;

							if (ns != null)
							{
								if (rootParent.CanSee(this) && rootParent.InRange(worldLoc, GetUpdateRange(rootParent)))
								{
									ns.Send(new ContainerContentUpdate(this));

									ns.Send(OPLPacket);
								}
							}
						}

						var stc = GetSecureTradeCont();

						if (stc != null)
						{
							var st = stc.Trade;

							if (st != null)
							{
								var test = st.From.Mobile;

								if (test != null && test != rootParent)
								{
									tradeRecip = test;
								}

								test = st.To.Mobile;

								if (test != null && test != rootParent)
								{
									tradeRecip = test;
								}

								if (tradeRecip != null)
								{
									var ns = tradeRecip.NetState;

									if (ns != null)
									{
										if (tradeRecip.CanSee(this) && tradeRecip.InRange(worldLoc, GetUpdateRange(tradeRecip)))
										{
											ns.Send(new ContainerContentUpdate(this));

											ns.Send(OPLPacket);
										}
									}
								}
							}
						}

						var openers = contParent.Openers;

						if (openers != null)
						{
							lock (openers)
							{
								for (var i = 0; i < openers.Count; ++i)
								{
									var mob = openers[i];

									var range = GetUpdateRange(mob);

									if (mob.Map != map || !mob.InRange(worldLoc, range))
									{
										openers.RemoveAt(i--);
									}
									else
									{
										if (mob == rootParent || mob == tradeRecip)
										{
											continue;
										}

										var ns = mob.NetState;

										if (ns != null && ns.Seeded)
										{
											if (mob.CanSee(this))
											{
												ns.Send(new ContainerContentUpdate(this));

												ns.Send(OPLPacket);
											}
										}
									}
								}

								if (openers.Count == 0)
								{
									contParent.Openers = null;
								}
							}
						}
						return;
					}
				}

				if ((flags & ItemDelta.Update) != 0)
				{
					Packet p = null;
					var worldLoc = GetWorldLocation();

					var eable = map.GetClientsInRange(worldLoc, Core.GlobalMaxUpdateRange);

					foreach (var state in eable)
					{
						var m = state.Mobile;

						if (m.CanSee(this) && m.InUpdateRange(worldLoc))
						{
							if (m_Parent == null)
							{
								SendInfoTo(state);
							}
							else
							{
								if (p == null)
								{
									if (m_Parent is Item)
									{
										state.Send(new ContainerContentUpdate(this));
									}
									else if (m_Parent is Mobile)
									{
										p = new EquipUpdate(this);
										p.Acquire();

										state.Send(p);
									}
								}
								else
								{
									state.Send(p);
								}

								state.Send(OPLPacket);
							}
						}
					}

					if (p != null)
					{
						Packet.Release(p);
					}

					eable.Free();
					sendOPLUpdate = false;
				}
				else if ((flags & ItemDelta.EquipOnly) != 0)
				{
					if (m_Parent is Mobile)
					{
						Packet p = null;
						var worldLoc = GetWorldLocation();

						var eable = map.GetClientsInRange(worldLoc, Core.GlobalMaxUpdateRange);

						foreach (var state in eable)
						{
							var m = state.Mobile;

							if (m.CanSee(this) && m.InUpdateRange(worldLoc))
							{
								if (p == null)
								{
									p = Packet.Acquire(new EquipUpdate(this));
								}

								state.Send(p);

								state.Send(OPLPacket);
							}
						}

						Packet.Release(p);

						eable.Free();
						sendOPLUpdate = false;
					}
				}

				if (sendOPLUpdate)
				{
					var worldLoc = GetWorldLocation();
					var eable = map.GetClientsInRange(worldLoc, Core.GlobalMaxUpdateRange);

					foreach (var state in eable)
					{
						var m = state.Mobile;

						if (m.CanSee(this) && m.InUpdateRange(worldLoc))
						{
							state.Send(OPLPacket);
						}
					}

					eable.Free();
				}
			}
		}

		private static bool _Processing;

		public static void ProcessDeltaQueue()
		{
			if (_Processing)
			{
				return;
			}

			_Processing = true;

			var i = m_DeltaQueue.Count;

			while (--i >= 0)
			{
				if (i < m_DeltaQueue.Count)
				{
					m_DeltaQueue[i].ProcessDelta();
					m_DeltaQueue.RemoveAt(i);
				}
			}

			_Processing = false;
		}

		public virtual void OnDelete()
		{
			if (Spawner != null)
			{
				Spawner.Remove(this);
				Spawner = null;
			}

			var region = Region.Find(GetWorldLocation(), Map);

			if (region != null)
			{
				region.OnDelete(this);
			}
		}

		public virtual void OnParentDeleted(object parent)
		{
			Delete();
		}

		public virtual void FreeCache()
		{
			ReleaseWorldPackets();
			Packet.Release(ref m_RemovePacket);
			Packet.Release(ref m_OPLPacket);
			Packet.Release(ref m_PropertyList);
		}

		public virtual void Delete()
		{
			if (Deleted)
			{
				return;
			}

			if (!World.OnDelete(this))
			{
				return;
			}

			OnDelete();

			var items = LookupItems();

			if (items != null)
			{
				for (var i = items.Count - 1; i >= 0; --i)
				{
					if (i < items.Count)
					{
						items[i].OnParentDeleted(this);
					}
				}
			}

			SendRemovePacket();

			SetFlag(ImplFlag.Deleted, true);

			if (Parent is Mobile)
			{
				((Mobile)Parent).RemoveItem(this);
			}
			else if (Parent is Item)
			{
				((Item)Parent).RemoveItem(this);
			}

			ClearBounce();

			if (m_Map != null)
			{
				if (m_Parent == null)
				{
					m_Map.OnLeave(this);
				}
				m_Map = null;
			}

			World.RemoveItem(this);

			OnAfterDelete();

			FreeCache();
		}

		public void PublicOverheadMessage(MessageType type, int hue, bool ascii, string text)
		{
			if (m_Map != null)
			{
				Packet p = null;
				var worldLoc = GetWorldLocation();

				var eable = m_Map.GetClientsInRange(worldLoc, Core.GlobalUpdateRange);

				foreach (var state in eable)
				{
					var m = state.Mobile;

					if (m.CanSee(this) && m.InUpdateRange(worldLoc))
					{
						if (p == null)
						{
							if (ascii)
							{
								p = new AsciiMessage(m_Serial, m_ItemID, type, hue, 3, Name, text);
							}
							else
							{
								p = new UnicodeMessage(m_Serial, m_ItemID, type, hue, 3, "ENU", Name, text);
							}

							p.Acquire();
						}

						state.Send(p);
					}
				}

				Packet.Release(p);

				eable.Free();
			}
		}

		public void PublicOverheadMessage(MessageType type, int hue, int number)
		{
			PublicOverheadMessage(type, hue, number, "");
		}

		public void PublicOverheadMessage(MessageType type, int hue, int number, string args)
		{
			if (m_Map != null)
			{
				Packet p = null;
				var worldLoc = GetWorldLocation();

				var eable = m_Map.GetClientsInRange(worldLoc, Core.GlobalUpdateRange);

				foreach (var state in eable)
				{
					var m = state.Mobile;

					if (m.CanSee(this) && m.InUpdateRange(worldLoc))
					{
						if (p == null)
						{
							p = Packet.Acquire(new MessageLocalized(m_Serial, m_ItemID, type, hue, 3, number, Name, args));
						}

						state.Send(p);
					}
				}

				Packet.Release(p);

				eable.Free();
			}
		}

		public void PrivateOverheadMessage(MessageType type, int hue, int number, NetState state, string args = "")
		{
			if (Map != null && state != null)
			{
				Packet p = null;
				var worldLoc = GetWorldLocation();

				var m = state.Mobile;

				if (m != null && m.CanSee(this) && m.InRange(worldLoc, GetUpdateRange(m)))
				{
					p = Packet.Acquire(new MessageLocalized(m_Serial, m_ItemID, type, hue, 3, number, Name, args));

					state.Send(p);
				}

				Packet.Release(p);
			}
		}

		public void PrivateOverheadMessage(MessageType type, int hue, bool ascii, string text, NetState state)
		{
			if (Map != null && state != null)
			{
				var worldLoc = GetWorldLocation();
				var m = state.Mobile;

				Packet asciip = null;
				Packet p = null;

				if (m != null && m.CanSee(this) && m.InRange(worldLoc, GetUpdateRange(m)))
				{
					if (ascii)
					{
						asciip = Packet.Acquire(new AsciiMessage(m_Serial, m_ItemID, type, hue, 3, Name, text));

						state.Send(asciip);
					}
					else
					{
						p = Packet.Acquire(new UnicodeMessage(m_Serial, m_ItemID, type, hue, 3, m.Language, Name, text));

						state.Send(p);
					}
				}

				Packet.Release(asciip);
				Packet.Release(p);
			}
		}

		public Region GetRegion()
		{
			return Region.Find(GetWorldLocation(), Map);
		}

		public double GetDistanceToSqrt(IPoint3D p)
		{
			var loc = GetWorldLocation();

			var xDelta = loc.X - p.X;
			var yDelta = loc.Y - p.Y;

			return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}

		public bool InRange(IPoint3D p, int range)
		{
			var loc = GetWorldLocation();

			return p.X >= (loc.X - range)
				&& p.X <= (loc.X + range)
				&& p.Y >= (loc.Y - range)
				&& p.Y <= (loc.Y + range);
		}

		public bool InLOS(Point3D target)
		{
			if (Deleted || Map == null || Parent != null)
				return false;

			return Map.LineOfSight(this, target);
		}

		public virtual void OnAfterDelete()
		{
			if (Sockets != null)
			{
				Sockets.IterateReverse(socket =>
				{
					socket.Remove();
				});
			}

			Timer.DelayCall(EventSink.InvokeItemDeleted, new ItemDeletedEventArgs(this));
		}

		public virtual void RemoveItem(Item item)
		{
			var items = LookupItems();

			if (items != null && items.Remove(item))
			{
				item.SendRemovePacket();

				if (!item.IsVirtualItem)
				{
					UpdateTotal(item, TotalType.Gold, -item.TotalGold);
					UpdateTotal(item, TotalType.Items, -(item.TotalItems + 1));
					UpdateTotal(item, TotalType.Weight, -(item.TotalWeight + item.PileWeight));
				}

				item.Parent = null;

				item.OnRemoved(this);
				OnItemRemoved(item);
			}
		}

		public virtual void OnAfterDuped(Item newItem)
		{
			if (Sockets != null)
			{
				for (var i = 0; i < Sockets.Count; i++)
				{
					Sockets[i].OnOwnerDuped(newItem);
				}
			}
		}

		public virtual bool OnDragLift(Mobile from)
		{
			return true;
		}

		public virtual bool OnEquip(Mobile from)
		{
			return true;
		}

		public ISpawner Spawner
		{
			get
			{
				var info = LookupCompactInfo();

				if (info != null)
				{
					return info.m_Spawner;
				}

				return null;
			}
			set
			{
				var info = AcquireCompactInfo();

				info.m_Spawner = value;

				if (info.m_Spawner == null)
				{
					VerifyCompactInfo();
				}
			}
		}

		public virtual void OnBeforeSpawn(Point3D location, Map m)
		{ }

		public virtual void OnAfterSpawn()
		{ }

		public virtual int PhysicalResistance => 0;
		public virtual int FireResistance => 0;
		public virtual int ColdResistance => 0;
		public virtual int PoisonResistance => 0;
		public virtual int EnergyResistance => 0;

		[CommandProperty(AccessLevel.Counselor)]
		public Serial Serial => m_Serial;

		[CommandProperty(AccessLevel.GameMaster)]
		public IEntity ParentEntity => Parent as IEntity;

		[CommandProperty(AccessLevel.GameMaster)]
		public IEntity RootParentEntity => RootParent as IEntity;

		#region Location Location Location!
		public virtual void OnLocationChange(Point3D oldLocation)
		{
			var items = Items;

			if (items == null)
			{
				return;
			}

			var i = items.Count;

			while (--i >= 0)
			{
				if (i >= items.Count)
				{
					continue;
				}

				var o = items[i];

				if (o != null)
				{
					o.OnParentLocationChange(oldLocation);
				}
			}
		}

		public virtual void OnParentLocationChange(Point3D oldLocation)
		{ }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public virtual Point3D Location
		{
			get => m_Location;
			set
			{
				var oldLocation = m_Location;

				if (oldLocation != value)
				{
					if (m_Map != null)
					{
						if (m_Parent == null)
						{
							IPooledEnumerable<NetState> eable;

							if (m_Location.m_X != 0)
							{
								eable = m_Map.GetClientsInRange(oldLocation, Core.GlobalMaxUpdateRange);

								foreach (var state in eable)
								{
									var m = state.Mobile;

									if (!m.InUpdateRange(value))
									{
										state.Send(RemovePacket);
									}
								}

								eable.Free();
							}

							var oldLoc = m_Location;
							m_Location = value;
							ReleaseWorldPackets();

							SetLastMoved();

							eable = m_Map.GetClientsInRange(m_Location, Core.GlobalMaxUpdateRange);

							foreach (var state in eable)
							{
								var m = state.Mobile;

								if (m.CanSee(this) && m.InUpdateRange(m_Location) &&
									(!m_NoMoveHS || (m_DeltaFlags & ItemDelta.Update) != 0 ||
									 !m.InRange(oldLoc, GetUpdateRange(m))))
								{
									SendInfoTo(state);
								}
							}

							eable.Free();

							RemDelta(ItemDelta.Update);
						}
						else if (m_Parent is Item)
						{
							m_Location = value;
							ReleaseWorldPackets();

							Delta(ItemDelta.Update);
						}
						else
						{
							m_Location = value;
							ReleaseWorldPackets();
						}

						if (m_Parent == null)
						{
							m_Map.OnMove(oldLocation, this);
						}
					}
					else
					{
						m_Location = value;
						ReleaseWorldPackets();
					}

					OnLocationChange(oldLocation);
				}
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int X { get => m_Location.m_X; set => Location = new Point3D(value, m_Location.m_Y, m_Location.m_Z); }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int Y { get => m_Location.m_Y; set => Location = new Point3D(m_Location.m_X, value, m_Location.m_Z); }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int Z { get => m_Location.m_Z; set => Location = new Point3D(m_Location.m_X, m_Location.m_Y, value); }
		#endregion

		[CommandProperty(AccessLevel.Decorator)]
		public virtual int ItemID
		{
			get => m_ItemID;
			set
			{
				if (m_ItemID != value)
				{
					var oldPileWeight = PileWeight;

					m_ItemID = value;

					ReleaseWorldPackets();

					var newPileWeight = PileWeight;

					UpdateTotal(this, TotalType.Weight, newPileWeight - oldPileWeight);

					UpdateLight();

					InvalidateProperties();
					Delta(ItemDelta.Update);
				}
			}
		}

		public virtual string DefaultName => null;

		[CommandProperty(AccessLevel.Decorator)]
		public string Name
		{
			get
			{
				var info = LookupCompactInfo();

				if (info != null && info.m_Name != null)
				{
					return info.m_Name;
				}

				return DefaultName;
			}
			set
			{
				if (value == null || value != DefaultName)
				{
					var info = AcquireCompactInfo();

					info.m_Name = value;

					if (info.m_Name == null)
					{
						VerifyCompactInfo();
					}

					InvalidateProperties();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual object Parent
		{
			get => m_Parent;
			set
			{
				if (m_Parent == value)
				{
					return;
				}

				var oldParent = m_Parent;

				m_Parent = value;

				OnParentChanged(oldParent);
			}
		}

		protected virtual void OnParentChanged(object oldParent)
		{
			if (m_Map != null)
			{
				if (oldParent != null && m_Parent == null)
				{
					m_Map.OnEnter(this);
				}
				else if (oldParent == null && m_Parent != null)
				{
					m_Map.OnLeave(this);
				}
			}

			if (!World.Loading)
			{
				if (oldParent is Item item)
				{
					oldParent = item.RootParent;
				}

				var root = RootParent as Mobile;

				if (root != null && oldParent != root)
				{
					root.Obtained(this);
				}
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public LightType Light
		{
			get => m_Light;
			set
			{
				if (m_Light != value)
				{
					m_Light = value;
					ReleaseWorldPackets();

					Delta(ItemDelta.Update);
				}
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public Direction Direction
		{
			get => m_Direction;
			set
			{
				if (m_Direction != value)
				{
					m_Direction = value;
					ReleaseWorldPackets();

					Delta(ItemDelta.Update);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Amount
		{
			get => m_Amount;
			set
			{
				var oldValue = m_Amount;

				if (oldValue != value)
				{
					var oldPileWeight = PileWeight;

					m_Amount = value;
					ReleaseWorldPackets();

					var newPileWeight = PileWeight;

					UpdateTotal(this, TotalType.Weight, newPileWeight - oldPileWeight);

					OnAmountChange(oldValue);

					Delta(ItemDelta.Update);

					if (oldValue > 1 || value > 1)
					{
						InvalidateProperties();
					}

					if (!Stackable && m_Amount > 1)
					{
						Console.WriteLine(
							"Warning: 0x{0:X}: Amount changed for non-stackable item '{2}'. ({1})", m_Serial.Value, m_Amount, GetType().Name);
					}
				}
			}
		}

		protected virtual void OnAmountChange(int oldValue)
		{ }

		public virtual bool HandlesOnSpeech => false;

		public virtual void OnSpeech(SpeechEventArgs e)
		{ }

		public virtual bool OnDroppedToMobile(Mobile from, Mobile target)
		{
			if (Nontransferable && from.Player && !from.IsStaff())
			{
				HandleInvalidTransfer(from);
				return false;
			}

			return true;
		}

		public virtual bool DropToMobile(Mobile from, Mobile target, Point3D p)
		{
			if (Deleted || from.Deleted || target.Deleted || from.Map != target.Map || from.Map == null || target.Map == null)
			{
				return false;
			}

			if (from.AccessLevel < AccessLevel.GameMaster && !from.InRange(target.Location, 2))
			{
				return false;
			}

			if (!from.CanSee(target) || !from.InLOS(target))
			{
				return false;
			}

			if (!from.OnDroppedItemToMobile(this, target))
			{
				return false;
			}

			if (!OnDroppedToMobile(from, target))
			{
				return false;
			}

			if (!target.OnDragDrop(from, this))
			{
				return false;
			}

			return true;
		}

		public virtual bool OnDroppedInto(Mobile from, Container target, Point3D p)
		{
			if (!from.OnDroppedItemInto(this, target, p))
			{
				return false;
			}

			if (Nontransferable && from.Player && target != from.Backpack)
			{
				HandleInvalidTransfer(from);
				return false;
			}

			return target.OnDragDropInto(from, this, p);
		}

		public virtual bool OnDroppedOnto(Mobile from, Item target)
		{
			if (Deleted || from.Deleted || target.Deleted || from.Map != target.Map || from.Map == null || target.Map == null)
			{
				return false;
			}

			if (from.AccessLevel < AccessLevel.GameMaster && !from.InRange(target.GetWorldLocation(), 2))
			{
				return false;
			}

			if (!from.CanSee(target) || !from.InLOS(target))
			{
				return false;
			}

			if (!target.IsAccessibleTo(from))
			{
				return false;
			}

			if (!from.OnDroppedItemOnto(this, target))
			{
				return false;
			}

			if (Nontransferable && from.Player && target != from.Backpack && !from.IsStaff())
			{
				HandleInvalidTransfer(from);
				return false;
			}

			return target.OnDragDrop(from, this);
		}

		public virtual bool DropToItem(Mobile from, Item target, Point3D p)
		{
			if (Deleted || from.Deleted || target.Deleted || from.Map != target.Map || from.Map == null || target.Map == null)
			{
				return false;
			}

			var root = target.RootParent;

			if (from.AccessLevel < AccessLevel.GameMaster && !from.InRange(target.GetWorldLocation(), 2))
			{
				return false;
			}

			if (!from.CanSee(target) || !from.InLOS(target))
			{
				return false;
			}

			if (!target.IsAccessibleTo(from))
			{
				return false;
			}

			if (root is Mobile mobile && (!mobile.CheckNonlocalDrop(from, this, target) || !mobile.CheckHasTradeDrop(from, this, target)))
			{
				return false;
			}

			if (!from.OnDroppedItemToItem(this, target, p))
			{
				return false;
			}

			if (target is Container container && p.m_X != -1 && p.m_Y != -1)
			{
				return OnDroppedInto(from, container, p);
			}

			return OnDroppedOnto(from, target);
		}

		public virtual bool OnDroppedToWorld(Mobile from, Point3D p)
		{
			if (Nontransferable && from.Player && !from.IsStaff())
			{
				HandleInvalidTransfer(from);
				return false;
			}

			return true;
		}

		public virtual int GetLiftSound(Mobile from)
		{
			return 0x57;
		}

		private static int m_OpenSlots;

		public virtual bool DropToWorld(Mobile from, Point3D p)
		{
			if (Deleted || from.Deleted || from.Map == null)
			{
				return false;
			}

			if (!from.InRange(p, 2))
			{
				return false;
			}

			if (QuestItem)
			{
				from.SendLocalizedMessage(1074769); // An item must be in your backpack (and not in a container within) to be toggled as a quest item.
				return false;
			}

			var map = from.Map;

			if (map == null)
			{
				return false;
			}

			var dest = FindDropPoint(p, map, from.Z + 17);
			if (dest == Point3D.Zero)
				return false;

			if (!from.InLOS(new Point3D(dest.X, dest.Y, dest.Z + 1)))
			{
				return false;
			}

			if (!from.OnDroppedItemToWorld(this, dest))
			{
				return false;
			}

			if (!OnDroppedToWorld(from, dest))
			{
				return false;
			}

			var soundID = GetDropSound();

			MoveToWorld(dest, from.Map);

			from.SendSound(soundID == -1 ? 0x42 : soundID, GetWorldLocation());

			return true;
		}

		public bool DropToWorld(Point3D p, Map map)
		{
			var dest = FindDropPoint(p, map, Int32.MaxValue);
			if (dest == Point3D.Zero)
				return false;
			MoveToWorld(dest, map);
			return true;
		}

		private Point3D FindDropPoint(Point3D p, Map map, int maxZ)
		{
			if (map == null)
				return Point3D.Zero;

			int x = p.m_X, y = p.m_Y;
			var z = Int32.MinValue;

			var landTile = map.Tiles.GetLandTile(x, y);
			var landFlags = TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags;

			int landZ = 0, landAvg = 0, landTop = 0;
			map.GetAverageZ(x, y, ref landZ, ref landAvg, ref landTop);

			if (!landTile.Ignored && (landFlags & TileFlag.Impassable) == 0)
			{
				if (landAvg <= maxZ)
					z = landAvg;
			}

			var tiles = map.Tiles.GetStaticTiles(x, y, true);

			for (var i = 0; i < tiles.Length; ++i)
			{
				var tile = tiles[i];
				var id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				if (!id.Surface)
					continue;

				var top = tile.Z + id.CalcHeight;

				if (top > maxZ || top < z)
					continue;

				z = top;
			}

			var items = new List<Item>();

			var eable = map.GetItemsInRange(p, 0);

			foreach (var item in eable)
			{
				if (item is BaseMulti || item.ItemID > TileData.MaxItemValue)
					continue;

				items.Add(item);

				var id = item.ItemData;

				if (!id.Surface)
					continue;

				var top = item.Z + id.CalcHeight;

				if (top > maxZ || top < z)
					continue;

				z = top;
			}

			eable.Free();

			if (z == Int32.MinValue)
				return Point3D.Zero;

			if (z > maxZ)
				return Point3D.Zero;

			m_OpenSlots = (1 << 20) - 1;

			var surfaceZ = z;

			for (var i = 0; i < tiles.Length; ++i)
			{
				var tile = tiles[i];
				var id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				var checkZ = tile.Z;
				var checkTop = checkZ + id.CalcHeight;

				if (checkTop == checkZ && !id.Surface)
					++checkTop;

				var zStart = checkZ - z;
				var zEnd = checkTop - z;

				if (zStart >= 20 || zEnd < 0)
					continue;

				if (zStart < 0)
					zStart = 0;

				if (zEnd > 19)
					zEnd = 19;

				var bitCount = zEnd - zStart;

				m_OpenSlots &= ~(((1 << bitCount) - 1) << zStart);
			}

			for (var i = 0; i < items.Count; ++i)
			{
				var item = items[i];
				var id = item.ItemData;

				var checkZ = item.Z;
				var checkTop = checkZ + id.CalcHeight;

				if (checkTop == checkZ && !id.Surface)
					++checkTop;

				var zStart = checkZ - z;
				var zEnd = checkTop - z;

				if (zStart >= 17 || zEnd < 0)
					continue;

				if (zStart < 0)
					zStart = 0;

				if (zEnd > 16)
					zEnd = 16;

				var bitCount = zEnd - zStart;

				m_OpenSlots &= ~(((1 << bitCount) - 1) << zStart);
			}

			var height = ItemData.Height;

			if (height == 0)
				++height;

			if (height > 30)
				height = 30;

			var match = (1 << height) - 1;
			var okay = false;

			for (var i = 0; i < 20; ++i)
			{
				if (i + height > 20)
					match >>= 1;

				okay = ((m_OpenSlots >> i) & match) == match;

				if (okay)
				{
					z += i;
					break;
				}
			}

			if (!okay)
				return Point3D.Zero;

			height = ItemData.Height;

			if (height == 0)
				++height;

			if (landAvg > z && (z + height) > landZ)
				return Point3D.Zero;

			if ((landFlags & TileFlag.Impassable) != 0 && landAvg > surfaceZ && (z + height) > landZ)
				return Point3D.Zero;

			for (var i = 0; i < tiles.Length; ++i)
			{
				var tile = tiles[i];
				var id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				var checkZ = tile.Z;
				var checkTop = checkZ + id.CalcHeight;

				if (checkTop > z && (z + height) > checkZ)
					return Point3D.Zero;

				if ((id.Surface || id.Impassable) && checkTop > surfaceZ && (z + height) > checkZ)
					return Point3D.Zero;
			}

			for (var i = 0; i < items.Count; ++i)
			{
				var item = items[i];
				var id = item.ItemData;

				if ((item.Z + id.CalcHeight) > z && (z + height) > item.Z)
					return Point3D.Zero;
			}

			return new Point3D(x, y, z);
		}

		public void SendRemovePacket()
		{
			if (!Deleted && m_Map != null)
			{
				var worldLoc = GetWorldLocation();

				var eable = m_Map.GetClientsInRange(worldLoc, Core.GlobalRadarRange - 4);

				foreach (var state in eable)
				{
					var m = state.Mobile;

					if (Utility.InRange(worldLoc, m.Location, GetUpdateRange(m)))
					{
						state.Send(RemovePacket);
					}
				}

				eable.Free();
			}
		}

		public virtual int GetDropSound()
		{
			return -1;
		}

		public Point3D GetWorldLocation()
		{
			var root = RootParent;

			if (root == null)
			{
				return m_Location;
			}

			return ((IEntity)root).Location;
		}

		public virtual bool BlocksFit => false;

		public Point3D GetSurfaceTop()
		{
			var root = RootParentEntity;

			if (root == null)
			{
				return new Point3D(m_Location.m_X, m_Location.m_Y, m_Location.m_Z + (ItemData.Surface ? ItemData.CalcHeight : 0));
			}

			return root.Location;
		}

		public Point3D GetWorldTop()
		{
			var root = RootParentEntity;

			if (root == null)
			{
				return new Point3D(m_Location.m_X, m_Location.m_Y, m_Location.m_Z + ItemData.CalcHeight);
			}

			return root.Location;
		}

		public void SendLocalizedMessageTo(Mobile to, int number)
		{
			if (Deleted || !to.CanSee(this))
			{
				return;
			}

			to.Send(new MessageLocalized(Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", ""));
		}

		public void SendLocalizedMessageTo(Mobile to, int number, string args)
		{
			if (Deleted || !to.CanSee(this))
			{
				return;
			}

			to.Send(new MessageLocalized(Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", args));
		}

		public void SendLocalizedMessage(int number, string args)
		{
			if (Deleted || Map == null)
			{
				return;
			}

			IPooledEnumerable eable = Map.GetClientsInRange(Location, Core.GlobalUpdateRange);
			Packet p = Packet.Acquire(new MessageLocalized(m_Serial, m_ItemID, MessageType.Regular, 0x3B2, 3, number, Name, args));

			foreach (NetState ns in eable)
			{
				ns.Send(p);
			}

			Packet.Release(p);
			eable.Free();
		}

		public void SendLocalizedMessage(MessageType type, int number, AffixType affixType, string affix, string args)
		{
			IPooledEnumerable eable = Map.GetClientsInRange(Location, Core.GlobalUpdateRange);
			Packet p = Packet.Acquire(new MessageLocalizedAffix(m_Serial, m_ItemID, type, 0x3B2, 3, number, "", affixType, affix, args));

			foreach (NetState ns in eable)
			{
				ns.Send(p);
			}

			Packet.Release(p);
			eable.Free();
		}

		public void SendLocalizedMessageTo(Mobile to, int number, AffixType affixType, string affix, string args)
		{
			if (Deleted || !to.CanSee(this))
			{
				return;
			}

			to.Send(new MessageLocalizedAffix(Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", affixType, affix, args));
		}

		#region OnDoubleClick[...]
		public virtual void OnDoubleClick(Mobile from)
		{ }

		public virtual void OnDoubleClickOutOfRange(Mobile from)
		{ }

		public virtual void OnDoubleClickCantSee(Mobile from)
		{ }

		public virtual void OnDoubleClickDead(Mobile from)
		{
			from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019048); // I am dead and cannot do that.
		}

		public virtual void OnDoubleClickNotAccessible(Mobile from)
		{
			from.SendLocalizedMessage(500447); // That is not accessible.
		}

		public virtual void OnDoubleClickSecureTrade(Mobile from)
		{
			from.SendLocalizedMessage(500447); // That is not accessible.
		}
		#endregion

		public virtual void OnSnoop(Mobile from)
		{ }

		public bool InSecureTrade => GetSecureTradeCont() != null;

		public SecureTradeContainer GetSecureTradeCont()
		{
			object p = this;

			while (p is Item item)
			{
				if (item is SecureTradeContainer container)
				{
					return container;
				}

				p = item.m_Parent;
			}

			return null;
		}

		public virtual void OnItemAdded(Item item)
		{
			if (m_Parent is Item iParent)
			{
				iParent.OnSubItemAdded(item);
			}
			else if (m_Parent is Mobile mParent)
			{
				mParent.OnSubItemAdded(item);
			}
		}

		public virtual void OnItemRemoved(Item item)
		{
			if (m_Parent is Item iParent)
			{
				iParent.OnSubItemRemoved(item);
			}
			else if (m_Parent is Mobile mParent)
			{
				mParent.OnSubItemRemoved(item);
			}
		}

		public virtual void OnSubItemAdded(Item item)
		{
			if (m_Parent is Item iParent)
			{
				iParent.OnSubItemAdded(item);
			}
			else if (m_Parent is Mobile mParent)
			{
				mParent.OnSubItemAdded(item);
			}
		}

		public virtual void OnSubItemRemoved(Item item)
		{
			if (m_Parent is Item iParent)
			{
				iParent.OnSubItemRemoved(item);
			}
			else if (m_Parent is Mobile mParent)
			{
				mParent.OnSubItemRemoved(item);
			}
		}

		public virtual void OnItemBounceCleared(Item item)
		{
			if (m_Parent is Item iParent)
			{
				iParent.OnSubItemBounceCleared(item);
			}
			else if (m_Parent is Mobile mParent)
			{
				mParent.OnSubItemBounceCleared(item);
			}
		}

		public virtual void OnSubItemBounceCleared(Item item)
		{
			if (m_Parent is Item iParent)
			{
				iParent.OnSubItemBounceCleared(item);
			}
			else if (m_Parent is Mobile mParent)
			{
				mParent.OnSubItemBounceCleared(item);
			}
		}

		public virtual bool CheckTarget(Mobile from, Target targ, object targeted)
		{
			if (m_Parent is Item iParent)
			{
				return iParent.CheckTarget(from, targ, targeted);
			}

			if (m_Parent is Mobile mParent)
			{
				return mParent.CheckTarget(from, targ, targeted);
			}

			return true;
		}

		public virtual void OnStatsQuery(Mobile m)
		{
			if (m == null || m.Deleted || m.Map != Map || m.NetState == null)
			{
				return;
			}

			if (Utility.InUpdateRange(m, this) && m.CanSee(this))
			{
				SendStatusTo(m.NetState);
			}
		}

		public virtual void SendStatusTo(NetState state)
		{
			var p = GetStatusPacketFor(state);

			if (p != null)
			{
				state.Send(p);
			}
		}

		public virtual Packet GetStatusPacketFor(NetState state)
		{
			if (this is IDamageable && state != null && state.Mobile != null)
			{
				return new MobileStatusCompact(CanBeRenamedBy(state.Mobile), (IDamageable)this);
			}

			return null;
		}

		public virtual bool CanBeRenamedBy(Mobile m)
		{
			return m != null && m.AccessLevel >= AccessLevel.GameMaster;
		}

		public virtual bool IsAccessibleTo(Mobile check)
		{
			if (m_Parent is Item item)
			{
				return item.IsAccessibleTo(check);
			}

			var reg = Region.Find(GetWorldLocation(), m_Map);

			return reg.CheckAccessibility(this, check);
		}

		public bool IsChildOf(object o)
		{
			return IsChildOf(o, false);
		}

		public bool IsChildOf(object o, bool allowNull)
		{
			var p = m_Parent;

			if ((p == null || o == null) && !allowNull)
			{
				return false;
			}

			if (p == o)
			{
				return true;
			}

			while (p is Item item)
			{
				if (item.m_Parent == null)
				{
					break;
				}

				p = item.m_Parent;

				if (p == o)
				{
					return true;
				}
			}

			return false;
		}

		public ItemData ItemData => TileData.ItemTable[m_ItemID & TileData.MaxItemValue];

		public virtual void OnItemUsed(Mobile from, Item item)
		{
			if (m_Parent is Item iParent)
			{
				iParent.OnItemUsed(from, item);
			}
			else if (m_Parent is Mobile mParent)
			{
				mParent.OnItemUsed(from, item);
			}
		}

		public bool CheckItemUse(Mobile from)
		{
			return CheckItemUse(from, this);
		}

		public virtual bool CheckItemUse(Mobile from, Item item)
		{
			if (m_Parent is Item iParent)
			{
				return iParent.CheckItemUse(from, item);
			}

			if (m_Parent is Mobile mParent)
			{
				return mParent.CheckItemUse(from, item);
			}

			return true;
		}

		public virtual void OnItemLifted(Mobile from, Item item)
		{
			if (m_Parent is Item iParent)
			{
				iParent.OnItemLifted(from, item);
			}
			else if (m_Parent is Mobile mParent)
			{
				mParent.OnItemLifted(from, item);
			}
		}

		public bool CheckLift(Mobile from)
		{
			var reject = LRReason.Inspecific;

			return CheckLift(from, this, ref reject);
		}

		public virtual bool CheckLift(Mobile from, Item item, ref LRReason reject)
		{
			if (m_Parent is Item iParent)
			{
				return iParent.CheckLift(from, item, ref reject);
			}

			if (m_Parent is Mobile mParent)
			{
				return mParent.CheckLift(from, item, ref reject);
			}

			return true;
		}

		public virtual bool CanTarget => true;
		public virtual bool DisplayLootType => true;

		public virtual void OnAosSingleClick(Mobile from)
		{
			var opl = PropertyList;

			if (opl.Header > 0)
			{
				from.Send(new MessageLocalized(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, opl.Header, Name, opl.HeaderArgs));
			}
		}

		private static bool m_ScissorCopyLootType;

		public static bool ScissorCopyLootType { get => m_ScissorCopyLootType; set => m_ScissorCopyLootType = value; }

		public virtual void ScissorHelper(Mobile from, Item newItem, int amountPerOldItem)
		{
			ScissorHelper(from, newItem, amountPerOldItem, true);
		}

		public virtual void ScissorHelper(Mobile from, Item newItem, int amountPerOldItem, bool carryHue)
		{
			var amount = Amount;

			if (amount > 60000 / amountPerOldItem) // let's not go over 60000
			{
				amount = 60000 / amountPerOldItem;
			}

			Amount -= amount;

			var ourHue = Hue;
			var thisMap = Map;
			var thisParent = m_Parent;
			var worldLoc = GetWorldLocation();
			var type = LootType;

			if (Amount == 0)
			{
				Delete();
			}

			newItem.Amount = amount * amountPerOldItem;

			if (carryHue)
			{
				newItem.Hue = ourHue;
			}

			if (m_ScissorCopyLootType)
			{
				newItem.LootType = type;
			}

			if (!(thisParent is Container) || !((Container)thisParent).TryDropItem(from, newItem, false))
			{
				newItem.MoveToWorld(worldLoc, thisMap);
			}
		}

		public virtual void Consume()
		{
			Consume(1);
		}

		public virtual void Consume(int amount)
		{
			Amount -= amount;

			if (Amount <= 0)
			{
				Delete();
			}
		}

		public virtual void ReplaceWith(Item newItem)
		{
			if (m_Parent is Container container)
			{
				container.AddItem(newItem);
				newItem.Location = m_Location;
			}
			else
			{
				newItem.MoveToWorld(GetWorldLocation(), m_Map);
			}

			Delete();
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool QuestItem
		{
			get => GetFlag(ImplFlag.QuestItem);
			set
			{
				SetFlag(ImplFlag.QuestItem, value);

				InvalidateProperties();

				ReleaseWorldPackets();

				Delta(ItemDelta.Update);
			}
		}

		public bool Insured
		{
			get => GetFlag(ImplFlag.Insured);
			set
			{
				SetFlag(ImplFlag.Insured, value);
				InvalidateProperties();
			}
		}

		public bool PayedInsurance { get => GetFlag(ImplFlag.PayedInsurance); set => SetFlag(ImplFlag.PayedInsurance, value); }

		public Mobile BlessedFor
		{
			get
			{
				var info = LookupCompactInfo();

				if (info != null)
				{
					return info.m_BlessedFor;
				}

				return null;
			}
			set
			{
				var info = AcquireCompactInfo();

				info.m_BlessedFor = value;

				if (info.m_BlessedFor == null)
				{
					VerifyCompactInfo();
				}

				InvalidateProperties();
			}
		}

		public virtual bool CheckBlessed(object o)
		{
			return CheckBlessed(o as Mobile);
		}

		public virtual bool CheckBlessed(Mobile m)
		{
			if (m_LootType == LootType.Blessed || Mobile.InsuranceEnabled && Insured)
			{
				return true;
			}

			return m != null && m == BlessedFor;
		}

		public virtual bool CheckNewbied()
		{
			return m_LootType == LootType.Newbied;
		}

		public virtual bool IsStandardLoot()
		{
			if (Mobile.InsuranceEnabled && Insured)
			{
				return false;
			}

			if (BlessedFor != null)
			{
				return false;
			}

			return m_LootType == LootType.Regular;
		}

		public override string ToString()
		{
			return String.Format("0x{0:X} \"{1}\"", m_Serial.Value, GetType().Name);
		}

		internal int m_TypeRef;

		public Item()
		{
			m_Serial = Serial.NewItem;
			m_Map = Map.Internal;

			m_Light = LightType.Empty;

			m_Amount = 1;

			Visible = true;
			Movable = true;

			SetLastMoved();

			World.AddItem(this);

			var ourType = GetType();
			m_TypeRef = World.m_ItemTypes.IndexOf(ourType);

			if (m_TypeRef == -1)
			{
				World.m_ItemTypes.Add(ourType);
				m_TypeRef = World.m_ItemTypes.Count - 1;
			}

			Timer.DelayCall(() =>
			{
				if (!Deleted)
				{
					EventSink.InvokeItemCreated(new ItemCreatedEventArgs(this));
				}
			});
		}

		[Constructable]
		public Item(int itemID)
			: this()
		{
			m_ItemID = itemID;

			UpdateLight();
		}

		public Item(Serial serial)
		{
			m_Serial = serial;

			var ourType = GetType();
			m_TypeRef = World.m_ItemTypes.IndexOf(ourType);

			if (m_TypeRef == -1)
			{
				World.m_ItemTypes.Add(ourType);
				m_TypeRef = World.m_ItemTypes.Count - 1;
			}
		}

		public virtual void UpdateLight()
		{
			var data = ItemData;

			if (!data.Flags.HasFlag(TileFlag.LightSource))
			{
				return;
			}

			if (m_Light == LightType.Empty)
			{
				var light = data.Quality;

				if (light >= 0 && light <= 255)
				{
					m_Light = (LightType)light;
				}
			}
		}

		public virtual void OnSectorActivate()
		{ }

		public virtual void OnSectorDeactivate()
		{ }

		#region Item Sockets
		public List<ItemSocket> Sockets { get; private set; }

		public void AttachSocket(ItemSocket socket)
		{
			if (Sockets == null)
			{
				Sockets = new List<ItemSocket>();
			}

			Sockets.Add(socket);
			socket.Owner = this;

			InvalidateProperties();
		}

		public bool RemoveSocket<T>()
		{
			var socket = GetSocket(typeof(T));

			if (socket != null)
			{
				RemoveItemSocket(socket);
				return true;
			}

			return false;
		}

		public void RemoveItemSocket(ItemSocket socket)
		{
			if (Sockets == null)
			{
				return;
			}

			Sockets.Remove(socket);
			socket.OnRemoved();

			if (Sockets.Count == 0)
			{
				Sockets = null;
			}

			InvalidateProperties();
		}

		public T GetSocket<T>() where T : ItemSocket
		{
			if (Sockets == null)
			{
				return null;
			}

			return Sockets.FirstOrDefault(s => s.GetType() == typeof(T)) as T;
		}

		public T GetSocket<T>(Func<T, bool> predicate) where T : ItemSocket
		{
			if (Sockets == null)
			{
				return null;
			}

			return Sockets.FirstOrDefault(s => s.GetType() == typeof(T) && (predicate == null || predicate(s as T))) as T;
		}

		public ItemSocket GetSocket(Type type)
		{
			if (Sockets == null)
			{
				return null;
			}

			return Sockets.FirstOrDefault(s => s.GetType() == type);
		}

		public bool HasSocket<T>()
		{
			if (Sockets == null)
			{
				return false;
			}

			return Sockets.Any(s => s.GetType() == typeof(T));
		}

		public bool HasSocket(Type t)
		{
			if (Sockets == null)
			{
				return false;
			}

			return Sockets.Any(s => s.GetType() == t);
		}
		#endregion
	}

	[PropertyObject]
	public class ItemSocket
	{
		private DateTime _Expires;

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Owner { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime Expires { get => _Expires; set { _Expires = value; CheckTimer(); } }

		public virtual TimeSpan TickDuration => TimeSpan.FromMinutes(1);

		public ItemSocket()
			: this(TimeSpan.Zero)
		{
		}

		public ItemSocket(TimeSpan duration)
		{
			if (duration != TimeSpan.Zero)
			{
				Expires = DateTime.UtcNow + duration;
			}
		}

		public override string ToString()
		{
			return "...";
		}

		public void CheckTimer()
		{
			if (Expires != DateTime.MinValue)
			{
				if (!SocketTimer.HasTimer(this))
				{
					SocketTimer.RegisterTimer(this);
				}
			}
			else if (SocketTimer.HasTimer(this))
			{
				SocketTimer.RemoveTimer(this);
			}
		}

		protected void BeginTimer()
		{
			SocketTimer.RegisterTimer(this);
		}

		protected void EndTimer()
		{
			SocketTimer.RemoveTimer(this);
		}

		protected virtual void OnTick()
		{
			if (Expires < DateTime.UtcNow || Owner.Deleted)
			{
				Remove();
			}
		}

		public virtual void Remove()
		{
			SocketTimer.RemoveTimer(this);
			Owner.RemoveItemSocket(this);
		}

		public virtual void OnRemoved()
		{
		}

		public virtual void GetProperties(ObjectPropertyList list)
		{
		}

		public virtual void OnOwnerDuped(Item newItem)
		{
			ItemSocket newSocket = null;

			try
			{
				newSocket = Activator.CreateInstance(GetType()) as ItemSocket;
			}
			catch (Exception e)
			{
				Diagnostics.ExceptionLogging.LogException(e);
			}

			if (newSocket != null)
			{
				newSocket.Expires = Expires;

				if (newSocket.Expires != DateTime.MinValue)
				{
					SocketTimer.RegisterTimer(this);
				}

				newSocket.OnAfterDuped(this);
				newItem.AttachSocket(newSocket);
			}
		}

		public virtual void OnAfterDuped(ItemSocket oldSocket)
		{
		}

		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(0);

			writer.Write(Expires);
		}

		public virtual void Deserialize(Item owner, GenericReader reader)
		{
			reader.ReadInt(); // version

			owner.AttachSocket(this);
			Expires = reader.ReadDateTime();
		}

		public static void Save(ItemSocket socket, GenericWriter writer)
		{
			writer.Write(socket.GetType().Name);
			socket.Serialize(writer);
		}

		public static void Load(Item item, GenericReader reader)
		{
			var typeName = ScriptCompiler.FindTypeByName(reader.ReadString());
			var socket = Activator.CreateInstance(typeName) as ItemSocket;

			socket.Deserialize(item, reader);
		}

		private class SocketTimer : Timer
		{
			public static SocketTimer Instance { get; private set; }
			public Dictionary<ItemSocket, DateTime> TimerRegistry { get; set; } = new Dictionary<ItemSocket, DateTime>();

			public SocketTimer()
				: base(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250))
			{
				Instance = this;
				Start();

				Priority = TimerPriority.FiftyMS;
			}

			public static bool HasTimer(ItemSocket socket)
			{
				return Instance != null && Instance.TimerRegistry.ContainsKey(socket);
			}

			public static void RegisterTimer(ItemSocket socket)
			{
				var timer = Instance;

				if (timer == null)
				{
					timer = new SocketTimer();
				}

				timer.TimerRegistry[socket] = DateTime.UtcNow + socket.TickDuration;
			}

			public static void RemoveTimer(ItemSocket socket)
			{
				var timer = Instance;

				if (timer != null)
				{
					if (timer.TimerRegistry.ContainsKey(socket))
					{
						timer.TimerRegistry.Remove(socket);
					}

					if (timer.TimerRegistry.Count == 0)
					{
						timer.Stop();
						Instance = null;
					}
				}
			}

			protected override void OnTick()
			{
				var list = new List<ItemSocket>();

				foreach (var socket in TimerRegistry.Keys.Where(s => TimerRegistry[s] < DateTime.UtcNow))
				{
					list.Add(socket);
				}

				for (var i = 0; i < list.Count; i++)
				{
					var socket = list[i];
					socket.OnTick();

					if (TimerRegistry.ContainsKey(socket))
					{
						TimerRegistry[socket] = DateTime.UtcNow + socket.TickDuration;
					}
				}

				ColUtility.Free(list);
			}
		}
	}
}
