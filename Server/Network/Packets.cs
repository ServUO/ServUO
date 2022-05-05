#region References
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Server.Accounting;
using Server.ContextMenus;
using Server.Diagnostics;
using Server.Gumps;
using Server.HuePickers;
using Server.Items;
using Server.Menus;
using Server.Menus.ItemLists;
using Server.Menus.Questions;
using Server.Mobiles;
using Server.Prompts;
using Server.Targeting;
#endregion

namespace Server.Network
{
	public enum PMMessage : byte
	{
		CharNoExist = 1,
		CharExists = 2,
		CharInWorld = 5,
		LoginSyncError = 6,
		IdleWarning = 7
	}

	public enum LRReason : byte
	{
		CannotLift = 0,
		OutOfRange = 1,
		OutOfSight = 2,
		TryToSteal = 3,
		AreHolding = 4,
		Inspecific = 5
	}

	public sealed class DamagePacket : Packet
	{
		public static bool Send(NetState ns, IEntity e, int amount)
		{
			return ns != null && Send(ns, Instantiate(ns, e, amount));
		}

		public static DamagePacket Instantiate(NetState ns, IEntity e, int amount)
		{
			Utility.FixMin(ref amount, 0);

			if (ns.DamagePacket)
			{
				Utility.FixMax(ref amount, UInt16.MaxValue);

				return new DamagePacket(e.Serial, (ushort)amount);
			}

			Utility.FixMax(ref amount, Byte.MaxValue);

			return new DamagePacket(e.Serial, (byte)amount);
		}

		private DamagePacket(Serial s, ushort amount)
			: base(0x0B, 7)
		{
			m_Stream.Write(s);
			m_Stream.Write(amount);
		}

		private DamagePacket(Serial s, byte amount)
			: base(0xBF)
		{
			EnsureCapacity(11);

			m_Stream.Write((short)0x22);
			m_Stream.Write((byte)1);
			m_Stream.Write(s);
			m_Stream.Write(amount);
		}
	}

	public sealed class CancelArrow : Packet
	{
		public static bool Send(NetState ns, IPoint2D p)
		{
			return ns != null && Send(ns, Instantiate(ns, p));
		}

		public static CancelArrow Instantiate(NetState ns, IPoint2D p)
		{
			if (ns.HighSeas)
			{
				if (p is IEntity e)
				{
					return new CancelArrow(e.X, e.Y, e.Serial);
				}

				if (p != null)
				{
					return new CancelArrow(p.X, p.Y, Serial.MinusOne);
				}

				return new CancelArrow(-1, -1, Serial.MinusOne);
			}

			return new CancelArrow(-1, -1, null);
		}

		private CancelArrow(int x, int y, Serial? s)
			: base(0xBA, s != null ? 10 : 6)
		{
			m_Stream.Write((byte)0);
			m_Stream.Write((short)x);
			m_Stream.Write((short)y);

			if (s != null)
			{
				m_Stream.Write(s.Value);
			}
		}
	}

	public sealed class SetArrow : Packet
	{
		public static bool Send(NetState ns, IPoint2D p)
		{
			return ns != null && Send(ns, Instantiate(ns, p));
		}

		public static SetArrow Instantiate(NetState ns, IPoint2D p)
		{
			if (ns.HighSeas)
			{
				if (p is IEntity e)
				{
					return new SetArrow(e.X, e.Y, e.Serial);
				}

				if (p != null)
				{
					return new SetArrow(p.X, p.Y, Serial.MinusOne);
				}

				return new SetArrow(-1, -1, Serial.MinusOne);
			}

			if (p != null)
			{
				return new SetArrow(p.X, p.Y, null);
			}

			return new SetArrow(-1, -1, null);
		}

		private SetArrow(int x, int y, Serial? s)
			: base(0xBA, s != null ? 10 : 6)
		{
			m_Stream.Write((byte)1);
			m_Stream.Write((short)x);
			m_Stream.Write((short)y);

			if (s != null)
			{
				m_Stream.Write(s.Value);
			}
		}
	}

	public sealed class DisplaySecureTrade : Packet
	{
		public DisplaySecureTrade(Mobile them, Container first, Container second, string name)
			: base(0x6F)
		{
			if (name == null)
			{
				name = "";
			}

			EnsureCapacity(18 + name.Length);

			m_Stream.Write((byte)0); // Display
			m_Stream.Write(them.Serial);
			m_Stream.Write(first.Serial);
			m_Stream.Write(second.Serial);
			m_Stream.Write(true);

			m_Stream.WriteAsciiFixed(name, 30);
		}
	}

	public sealed class CloseSecureTrade : Packet
	{
		public CloseSecureTrade(Container cont)
			: base(0x6F)
		{
			EnsureCapacity(17);

			m_Stream.Write((byte)1); // Close
			m_Stream.Write(cont.Serial);
			m_Stream.Write(0);
			m_Stream.Write(0);
			m_Stream.Write(false);
		}
	}

	public enum TradeFlag : byte
	{
		Display = 0x0,
		Close = 0x1,
		Update = 0x2,
		UpdateGold = 0x3,
		UpdateLedger = 0x4
	}

	public sealed class UpdateSecureTrade : Packet
	{
		public UpdateSecureTrade(Container cont, bool first, bool second)
			: this(cont, TradeFlag.Update, first ? 1 : 0, second ? 1 : 0)
		{ }

		public UpdateSecureTrade(Container cont, TradeFlag flag, int first, int second)
			: base(0x6F)
		{
			EnsureCapacity(17);

			m_Stream.Write((byte)flag);
			m_Stream.Write(cont.Serial);
			m_Stream.Write(first);
			m_Stream.Write(second);
			m_Stream.Write(false);
		}
	}

	public sealed class SecureTradeEquip : Packet
	{
		public static bool Send(NetState ns, Item item, Mobile m)
		{
			return ns != null && Send(ns, Instantiate(ns, item, m));
		}

		public static SecureTradeEquip Instantiate(NetState ns, Item item, Mobile m)
		{
			if (ns.ContainerGridLines)
			{
				return new SecureTradeEquip(item, m, 0);
			}

			return new SecureTradeEquip(item, m, null);
		}

		private SecureTradeEquip(Item item, Mobile m, int? gridLocation)
			: base(0x25, gridLocation != null ? 21 : 20)
		{
			m_Stream.Write(item.Serial);
			m_Stream.Write((short)item.ItemID);
			m_Stream.Write((byte)0);
			m_Stream.Write((short)item.Amount);
			m_Stream.Write((short)item.X);
			m_Stream.Write((short)item.Y);

			if (gridLocation != null)
			{
				m_Stream.Write((byte)gridLocation.Value);
			}

			m_Stream.Write(m.Serial);
			m_Stream.Write((short)item.Hue);
		}
	}

	public sealed class MapPatches : Packet
	{
		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static MapPatches Instantiate(NetState ns)
		{
			if (ns.Flags.HasFlag(ClientFlags.TerMur))
			{
				return PacketCache<MapPatches>.Acquire(6, c => new MapPatches(c));
			}

			if (ns.Flags.HasFlag(ClientFlags.Tokuno))
			{
				return PacketCache<MapPatches>.Acquire(5, c => new MapPatches(c));
			}

			if (ns.Flags.HasFlag(ClientFlags.Malas))
			{
				return PacketCache<MapPatches>.Acquire(4, c => new MapPatches(c));
			}

			if (ns.Flags.HasFlag(ClientFlags.Ilshenar))
			{
				return PacketCache<MapPatches>.Acquire(3, c => new MapPatches(c));
			}

			if (ns.Flags.HasFlag(ClientFlags.Trammel))
			{
				return PacketCache<MapPatches>.Acquire(2, c => new MapPatches(c));
			}

			if (ns.Flags.HasFlag(ClientFlags.Felucca))
			{
				return PacketCache<MapPatches>.Acquire(1, c => new MapPatches(c));
			}

			return PacketCache<MapPatches>.Acquire(0, c => new MapPatches(c));
		}

		private MapPatches(int count)
			: base(0xBF)
		{
			EnsureCapacity(9 + (8 * count));

			m_Stream.Write((short)0x0018);

			m_Stream.Write(count);

			if (count >= 1)
			{
				m_Stream.Write(Map.Felucca.Tiles.Patch.StaticBlocks);
				m_Stream.Write(Map.Felucca.Tiles.Patch.LandBlocks);
			}

			if (count >= 2)
			{
				m_Stream.Write(Map.Trammel.Tiles.Patch.StaticBlocks);
				m_Stream.Write(Map.Trammel.Tiles.Patch.LandBlocks);
			}

			if (count >= 3)
			{
				m_Stream.Write(Map.Ilshenar.Tiles.Patch.StaticBlocks);
				m_Stream.Write(Map.Ilshenar.Tiles.Patch.LandBlocks);
			}

			if (count >= 4)
			{
				m_Stream.Write(Map.Malas.Tiles.Patch.StaticBlocks);
				m_Stream.Write(Map.Malas.Tiles.Patch.LandBlocks);
			}

			if (count >= 5)
			{
				m_Stream.Write(Map.Tokuno.Tiles.Patch.StaticBlocks);
				m_Stream.Write(Map.Tokuno.Tiles.Patch.LandBlocks);
			}

			if (count >= 6)
			{
				m_Stream.Write(Map.TerMur.Tiles.Patch.StaticBlocks);
				m_Stream.Write(Map.TerMur.Tiles.Patch.LandBlocks);
			}
		}
	}

	public sealed class ObjectHelpResponse : Packet
	{
		public ObjectHelpResponse(IEntity e, string text)
			: base(0xB7)
		{
			EnsureCapacity(9 + (text.Length * 2));

			m_Stream.Write(e.Serial);
			m_Stream.WriteBigUniNull(text);
		}
	}

	public sealed class VendorBuyContent : Packet
	{
		public static bool Send(NetState ns, List<BuyItemState> list)
		{
			return ns != null && Send(ns, Instantiate(ns, list));
		}

		public static VendorBuyContent Instantiate(NetState ns, List<BuyItemState> list)
		{
			if (ns.ContainerGridLines)
			{
				return new VendorBuyContent(list, true);
			}

			return new VendorBuyContent(list, false);
		}

		private VendorBuyContent(List<BuyItemState> list, bool gridLocs)
			: base(0x3C)
		{
			EnsureCapacity(list.Count * (gridLocs ? 20 : 19) + 5);

			m_Stream.Write((short)list.Count);

			//The client sorts these by their X/Y value.
			//OSI sends these in wierd order.  X/Y highest to lowest and serial loest to highest
			//These are already sorted by serial (done by the vendor class) but we have to send them by x/y
			//(the x74 packet is sent in 'correct' order.)
			for (var i = list.Count - 1; i >= 0; --i)
			{
				var bis = list[i];

				m_Stream.Write(bis.MySerial);
				m_Stream.Write((ushort)bis.ItemID);
				m_Stream.Write((byte)0); //itemid offset
				m_Stream.Write((ushort)bis.Amount);
				m_Stream.Write((short)(i + 1)); //x
				m_Stream.Write((short)1); //y

				if (gridLocs)
				{
					m_Stream.Write((byte)i);
				}

				m_Stream.Write(bis.ContainerSerial);
				m_Stream.Write((ushort)bis.Hue);
			}
		}
	}

	public sealed class DisplayBuyList : Packet
	{
		public static bool Send(NetState ns, Mobile vendor)
		{
			return ns != null && Send(ns, Instantiate(ns, vendor));
		}

		public static DisplayBuyList Instantiate(NetState ns, Mobile vendor)
		{
			if (ns.HighSeas)
			{
				return new DisplayBuyList(vendor, 0);
			}

			return new DisplayBuyList(vendor, null);
		}

		private DisplayBuyList(Mobile vendor, int? unk)
			: base(0x24, unk != null ? 9 : 7)
		{
			m_Stream.Write(vendor.Serial);
			m_Stream.Write((short)0x30); // buy window id?

			if (unk != null)
			{
				m_Stream.Write((short)unk.Value);
			}
		}
	}

	public sealed class VendorBuyList : Packet
	{
		public VendorBuyList(Mobile vendor, List<BuyItemState> list)
			: base(0x74)
		{
			EnsureCapacity(256);

			var buyPack = vendor.FindItemOnLayer(Layer.ShopBuy) as Container;

			m_Stream.Write(buyPack?.Serial ?? Serial.MinusOne);

			m_Stream.Write((byte)list.Count);

			for (var i = 0; i < list.Count; ++i)
			{
				var bis = list[i];

				m_Stream.Write(bis.Price);

				var desc = bis.Description ?? String.Empty;

				m_Stream.Write((byte)(desc.Length + 1));
				m_Stream.WriteAsciiNull(desc);
			}
		}
	}

	public sealed class VendorSellList : Packet
	{
		public VendorSellList(Mobile shopkeeper, ICollection<SellItemState> sis)
			: base(0x9E)
		{
			EnsureCapacity(256);

			m_Stream.Write(shopkeeper.Serial);

			m_Stream.Write((ushort)sis.Count);

			foreach (var state in sis)
			{
				m_Stream.Write(state.Item.Serial);
				m_Stream.Write((ushort)state.Item.ItemID);
				m_Stream.Write((ushort)state.Item.Hue);
				m_Stream.Write((ushort)state.Item.Amount);
				m_Stream.Write((ushort)state.Price);

				var name = state.Item.Name;

				if (name == null || (name = name.Trim()).Length <= 0)
				{
					name = state.Name;
				}

				if (name == null)
				{
					name = "";
				}

				m_Stream.Write((ushort)name.Length);
				m_Stream.WriteAsciiFixed(name, name.Length);
			}
		}
	}

	public sealed class EndVendorSell : Packet
	{
		public EndVendorSell(Mobile vendor)
			: base(0x3B, 8)
		{
			m_Stream.Write((ushort)8); //length
			m_Stream.Write(vendor.Serial);
			m_Stream.Write((byte)0);
		}
	}

	public sealed class EndVendorBuy : Packet
	{
		public EndVendorBuy(Mobile vendor)
			: base(0x3B, 8)
		{
			m_Stream.Write((ushort)8); //length
			m_Stream.Write(vendor.Serial);
			m_Stream.Write((byte)0);
		}
	}

	public sealed class DeathAnimation : Packet
	{
		public DeathAnimation(Mobile killed, Item corpse)
			: base(0xAF, 13)
		{
			m_Stream.Write(killed.Serial);
			m_Stream.Write(corpse?.Serial ?? Serial.Zero);
			m_Stream.Write(0);
		}
	}

	public sealed class StatLockInfo : Packet
	{
		public StatLockInfo(Mobile m)
			: base(0xBF)
		{
			EnsureCapacity(12);

			m_Stream.Write((short)0x19);

			if (m.NetState.IsEnhancedClient)
			{
				m_Stream.Write((byte)5);
			}
			else
			{
				m_Stream.Write((byte)2);
			}

			m_Stream.Write(m.Serial);
			m_Stream.Write((byte)0);

			var lockBits = 0;

			lockBits |= (int)m.StrLock << 4;
			lockBits |= (int)m.DexLock << 2;
			lockBits |= (int)m.IntLock;

			m_Stream.Write((byte)lockBits);
		}
	}

	public class EquipInfoAttribute
	{
		public int Number { get; }
		public int Charges { get; }

		public EquipInfoAttribute(int number)
			: this(number, -1)
		{ }

		public EquipInfoAttribute(int number, int charges)
		{
			Number = number;
			Charges = charges;
		}
	}

	public class EquipmentInfo
	{
		public int Number { get; }

		public Mobile Crafter { get; }

		public bool Unidentified { get; }

		public EquipInfoAttribute[] Attributes { get; }

		public EquipmentInfo(int number, Mobile crafter, bool unidentified, EquipInfoAttribute[] attributes)
		{
			Number = number;
			Crafter = crafter;
			Unidentified = unidentified;
			Attributes = attributes;
		}
	}

	public sealed class DisplayEquipmentInfo : Packet
	{
		public DisplayEquipmentInfo(Item item, EquipmentInfo info)
			: base(0xBF)
		{
			var attrs = info.Attributes;

			var size = 17;

			if (info.Crafter != null)
			{
				size += 6;

				if (info.Crafter.TitleName != null)
				{
					size += info.Crafter.TitleName.Length;
				}
			}

			if (info.Unidentified)
			{
				size += 4;
			}

			size += attrs.Length * 6;

			EnsureCapacity(size);

			m_Stream.Write((short)0x10);
			m_Stream.Write(item.Serial);

			m_Stream.Write(info.Number);

			if (info.Crafter != null)
			{
				var name = info.Crafter.TitleName;

				m_Stream.Write(-3);

				if (name == null)
				{
					m_Stream.Write((ushort)0);
				}
				else
				{
					var length = name.Length;

					m_Stream.Write((ushort)length);
					m_Stream.WriteAsciiFixed(name, length);
				}
			}

			if (info.Unidentified)
			{
				m_Stream.Write(-4);
			}

			for (var i = 0; i < attrs.Length; ++i)
			{
				m_Stream.Write(attrs[i].Number);
				m_Stream.Write((short)attrs[i].Charges);
			}

			m_Stream.Write(-1);
		}
	}

	public sealed class ChangeUpdateRange : Packet
	{
		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static ChangeUpdateRange Instantiate(NetState ns)
		{
			var range = ns.UpdateRange;

			if (range >= 0 && range <= Core.GlobalMaxUpdateRange + 8)
			{
				return PacketCache<ChangeUpdateRange>.Acquire(range, r => new ChangeUpdateRange(r));
			}

			return new ChangeUpdateRange(range);
		}

		private ChangeUpdateRange(int range)
			: base(0xC8, 2)
		{
			m_Stream.Write((byte)range);
		}
	}

	public sealed class ChangeCombatant : Packet
	{
		public ChangeCombatant(IEntity combatant)
			: base(0xAA, 5)
		{
			m_Stream.Write(combatant?.Serial ?? Serial.Zero);
		}
	}

	public sealed class DisplayHuePicker : Packet
	{
		public DisplayHuePicker(HuePicker huePicker)
			: base(0x95, 9)
		{
			m_Stream.Write(huePicker.Serial);
			m_Stream.Write((short)0);
			m_Stream.Write((short)huePicker.ItemID);
		}
	}

	public sealed class TripTimeResponse : Packet
	{
		public static bool Send(NetState ns, byte ping, bool unk)
		{
			return ns != null && Send(ns, Instantiate(ns, ping, unk));
		}

		public static TripTimeResponse Instantiate(NetState ns, byte ping, bool unk)
		{
			return new TripTimeResponse(ping, unk);
		}

		private TripTimeResponse(byte ping, bool state)
			: base(state ? 0xC9 : 0xCA, 6)
		{
			m_Stream.Write(ping);
			m_Stream.Write(Environment.TickCount);
		}
	}

	public sealed class UnicodePrompt : Packet
	{
		public UnicodePrompt(Prompt prompt)
			: base(0xC2)
		{
			EnsureCapacity(21);

			m_Stream.Write(prompt.Serial);
			m_Stream.Write(prompt.Serial);

			m_Stream.Write(0); // type
			m_Stream.Write(0); // language
			m_Stream.Write((short)0); // text
		}
	}

	public sealed class ChangeCharacter : Packet
	{
		public ChangeCharacter(IAccount a)
			: base(0x81)
		{
			EnsureCapacity(305);

			var count = 0;

			m_Stream.Write((byte)0); // this line was missing: investigate

			for (var i = 0; i < a.Length; ++i)
			{
				if (a[i] != null)
				{
					++count;
				}
			}

			m_Stream.Write((byte)count);
			m_Stream.Write((byte)0);

			for (var i = 0; i < a.Length; ++i)
			{
				if (a[i] != null)
				{
					var name = a[i].Name;

					if (name == null)
					{
						name = "-null-";
					}
					else if ((name = name.Trim()).Length == 0)
					{
						name = "-empty-";
					}

					m_Stream.WriteAsciiFixed(name, 30);
					m_Stream.Fill(30); // password
				}
				else
				{
					m_Stream.Fill(60);
				}
			}
		}
	}

	public sealed class DeathStatus : Packet
	{
		public static DeathStatus Dead => PacketCache<DeathStatus>.Acquire(false, s => new DeathStatus(s));
		public static DeathStatus Alive => PacketCache<DeathStatus>.Acquire(true, s => new DeathStatus(s));

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static DeathStatus Instantiate(NetState ns)
		{
			var state = ns.Mobile?.Alive ?? false;

			return PacketCache<DeathStatus>.Acquire(state, s => new DeathStatus(s));
		}

		private DeathStatus(bool alive)
			: base(0x2C, 2)
		{
			m_Stream.Write((byte)(alive ? 2 : 0));
		}
	}

	public enum SpeedControlType : byte
	{
		Disable,
		MountSpeed,
		WalkSpeed,
		WalkSpeedFast,
		TeleportSpeed
	}

	public sealed class SpeedControl : Packet
	{
		public static SpeedControl TeleportSpeed => PacketCache<SpeedControl>.Acquire(SpeedControlType.TeleportSpeed, s => new SpeedControl(s));
		public static SpeedControl WalkSpeedFast => PacketCache<SpeedControl>.Acquire(SpeedControlType.WalkSpeedFast, s => new SpeedControl(s));
		public static SpeedControl WalkSpeed => PacketCache<SpeedControl>.Acquire(SpeedControlType.WalkSpeed, s => new SpeedControl(s));
		public static SpeedControl MountSpeed => PacketCache<SpeedControl>.Acquire(SpeedControlType.MountSpeed, s => new SpeedControl(s));
		public static SpeedControl Disable => PacketCache<SpeedControl>.Acquire(SpeedControlType.Disable, s => new SpeedControl(s));

		public static bool Send(NetState ns, SpeedControlType speed)
		{
			return ns != null && Send(ns, Instantiate(ns, speed));
		}

		public static SpeedControl Instantiate(NetState ns, SpeedControlType speed)
		{
			return PacketCache<SpeedControl>.Acquire(speed, s => new SpeedControl(s));
		}

		private SpeedControl(SpeedControlType type)
			: base(0xBF)
		{
			EnsureCapacity(3);

			m_Stream.Write((short)0x26);
			m_Stream.Write((byte)type);
		}
	}

	public sealed class InvalidMapEnable : Packet
	{
		public InvalidMapEnable()
			: base(0xC6, 1)
		{ }
	}

	public sealed class BondedStatus : Packet
	{
		public BondedStatus(int val1, Serial serial, int val2)
			: base(0xBF)
		{
			EnsureCapacity(11);

			m_Stream.Write((short)0x19);
			m_Stream.Write((byte)val1);
			m_Stream.Write(serial);
			m_Stream.Write((byte)val2);
		}
	}

	public sealed class ToggleSpecialAbility : Packet
	{
		public ToggleSpecialAbility(int abilityID, bool active)
			: base(0xBF)
		{
			EnsureCapacity(7);

			m_Stream.Write((short)0x25);

			m_Stream.Write((short)abilityID);
			m_Stream.Write(active);
		}
	}

	public sealed class DisplayItemListMenu : Packet
	{
		public DisplayItemListMenu(ItemListMenu menu)
			: base(0x7C)
		{
			EnsureCapacity(256);

			m_Stream.Write(((IMenu)menu).Serial);
			m_Stream.Write((short)0);

			var question = menu.Question;

			if (question == null)
			{
				m_Stream.Write((byte)0);
			}
			else
			{
				var questionLength = question.Length;

				m_Stream.Write((byte)questionLength);
				m_Stream.WriteAsciiFixed(question, questionLength);
			}

			var entries = menu.Entries;
			var entriesLength = entries.Length;

			m_Stream.Write((byte)entriesLength);

			for (var i = 0; i < entriesLength; ++i)
			{
				var e = entries[i];

				m_Stream.Write((ushort)e.ItemID);
				m_Stream.Write((short)e.Hue);

				var name = e.Name;

				if (name == null)
				{
					m_Stream.Write((byte)0);
				}
				else
				{
					var nameLength = name.Length;

					m_Stream.Write((byte)nameLength);
					m_Stream.WriteAsciiFixed(name, nameLength);
				}
			}
		}
	}

	public sealed class DisplayQuestionMenu : Packet
	{
		public DisplayQuestionMenu(QuestionMenu menu)
			: base(0x7C)
		{
			EnsureCapacity(256);

			m_Stream.Write(((IMenu)menu).Serial);
			m_Stream.Write((short)menu.GumpID);

			var question = menu.Question;

			if (question == null)
			{
				m_Stream.Write((byte)0);
			}
			else
			{
				var questionLength = question.Length;

				m_Stream.Write((byte)questionLength);
				m_Stream.WriteAsciiFixed(question, questionLength);
			}

			var answers = menu.Answers;

			var answersLength = answers.Length;

			m_Stream.Write((byte)answersLength);

			for (var i = 0; i < answersLength; ++i)
			{
				m_Stream.Write(0);

				var answer = answers[i];

				if (answer == null)
				{
					m_Stream.Write((byte)0);
				}
				else
				{
					var answerLength = answer.Length;

					m_Stream.Write((byte)answerLength);
					m_Stream.WriteAsciiFixed(answer, answerLength);
				}
			}
		}
	}

	public sealed class GlobalLightLevel : Packet
	{
		public static bool Send(NetState ns, int level)
		{
			return ns != null && Send(ns, Instantiate(ns, level));
		}

		public static GlobalLightLevel Instantiate(NetState ns, int level)
		{
			level = Math.Max(0x100, level);

			return PacketCache<GlobalLightLevel>.Acquire((byte)level, l => new GlobalLightLevel(l));
		}

		private GlobalLightLevel(byte level)
			: base(0x4F, 2)
		{
			m_Stream.Write(level);
		}
	}

	public sealed class PersonalLightLevel : Packet
	{
		public PersonalLightLevel(Mobile m)
			: this(m, m.LightLevel)
		{ }

		public PersonalLightLevel(Mobile m, int level)
			: base(0x4E, 6)
		{
			m_Stream.Write(m.Serial);
			m_Stream.Write((byte)level);
		}
	}

	public sealed class PersonalLightLevelZero : Packet
	{
		public PersonalLightLevelZero(Mobile m)
			: base(0x4E, 6)
		{
			m_Stream.Write(m.Serial);
			m_Stream.Write((byte)0);
		}
	}

	[Flags]
	public enum CMEFlags
	{
		None = 0x00,
		Disabled = 0x01,
		Arrow = 0x02,
		Highlighted = 0x04,
		Colored = 0x20
	}

	public sealed class DisplayContextMenu : Packet
	{
		public static bool Send(NetState ns, ContextMenu menu)
		{
			return ns != null && Send(ns, Instantiate(ns, menu));
		}

		public static DisplayContextMenu Instantiate(NetState ns, ContextMenu menu)
		{
			if (ns.NewHaven && menu.RequiresNewPacket)
			{
				return new DisplayContextMenu(menu, false);
			}

			return new DisplayContextMenu(menu, true);
		}

		private DisplayContextMenu(ContextMenu menu, bool retro)
			: base(0xBF)
		{
			var entries = menu.Entries;
			var length = entries.Length;

			var size = 12 + (length * 8);

			if (retro)
			{
				for (var i = 0; i < entries.Length; i++)
				{
					if ((entries[i].Flags & CMEFlags.Colored) != 0)
					{
						size += 2;
					}
				}
			}

			EnsureCapacity(size);

			m_Stream.Write((short)0x14);
			m_Stream.Write((short)0x02);

			var target = menu.Target;

			m_Stream.Write(target?.Serial ?? Serial.MinusOne);

			m_Stream.Write((byte)length);

			Point3D p;

			if (target is Mobile)
			{
				p = target.Location;
			}
			else if (target is Item it)
			{
				p = it.GetWorldLocation();
			}
			else
			{
				p = Point3D.Zero;
			}

			for (var i = 0; i < length; ++i)
			{
				var e = entries[i];

				if (retro && e.Number <= 65535)
				{
					m_Stream.Write(e.Number + 3000000);
				}
				else
				{
					m_Stream.Write(e.Number);
				}

				m_Stream.Write((short)i);

				var range = e.Range;

				if (range == -1)
				{
					range = Core.GlobalMaxUpdateRange;
				}

				var flags = e.Flags;

				if (!e.Enabled || !menu.From.InRange(p, range))
				{
					flags |= CMEFlags.Disabled;
				}

				if (!retro)
				{
					flags &= ~CMEFlags.Colored;
				}

				m_Stream.Write((short)flags);

				if ((flags & CMEFlags.Colored) != 0)
				{
					m_Stream.Write((short)(e.Color & 0xFFFF));
				}
			}
		}
	}

	public sealed class DisplayProfile : Packet
	{
		public DisplayProfile(bool realSerial, Mobile m, string header, string body, string footer)
			: base(0xB8)
		{
			if (header == null)
			{
				header = "";
			}

			if (body == null)
			{
				body = "";
			}

			if (footer == null)
			{
				footer = "";
			}

			EnsureCapacity(12 + header.Length + (footer.Length * 2) + (body.Length * 2));

			m_Stream.Write(realSerial ? m.Serial : Serial.Zero);
			m_Stream.WriteAsciiNull(header);
			m_Stream.WriteBigUniNull(footer);
			m_Stream.WriteBigUniNull(body);
		}
	}

	public sealed class CloseGump : Packet
	{
		public CloseGump(int typeID, int buttonID)
			: base(0xBF)
		{
			EnsureCapacity(13);

			m_Stream.Write((short)0x04);
			m_Stream.Write(typeID);
			m_Stream.Write(buttonID);
		}
	}

	public sealed class EquipUpdate : Packet
	{
		public EquipUpdate(Item item)
			: base(0x2E, 15)
		{
			var hue = item.Hue;

			Serial parentSerial;

			if (item.Parent is Mobile p)
			{
				parentSerial = p.Serial;

				if (p.SolidHueOverride >= 0)
				{
					hue = p.SolidHueOverride;
				}
			}
			else
			{
				parentSerial = Serial.Zero;
			}

			m_Stream.Write(item.Serial);
			m_Stream.Write((short)item.ItemID);
			m_Stream.Write((byte)0);
			m_Stream.Write((byte)item.Layer);
			m_Stream.Write(parentSerial);
			m_Stream.Write((short)hue);
		}
	}

	public sealed class WorldItem : Packet
	{
		public static bool Send(NetState ns, Item item)
		{
			return ns != null && Send(ns, Instantiate(ns, item));
		}

		public static WorldItem Instantiate(NetState ns, Item item)
		{
			var hash = (long)item.Serial.Value << 32;

			if (ns.HighSeas)
			{
				hash |= (long)Expansion.HS;

				return PacketCache<WorldItem>.Acquire(hash, () => new WorldItem(item, 0));
			}

			if (ns.StygianAbyss)
			{
				hash |= (long)Expansion.SA;

				return PacketCache<WorldItem>.Acquire(hash, () => new WorldItem(item, null));
			}

			hash |= (long)Expansion.None;

			return PacketCache<WorldItem>.Acquire(hash, () => new WorldItem(item));
		}

		public static void Free(Item item)
		{
			var hash = (long)item.Serial.Value << 32;

			PacketCache<WorldItem>.Free(hash | (long)Expansion.HS);
			PacketCache<WorldItem>.Free(hash | (long)Expansion.SA);
			PacketCache<WorldItem>.Free(hash | (long)Expansion.None);
		}

		private WorldItem(Item item, int? unk)
			: base(0xF3, unk != null ? 26 : 24)
		{
			m_Stream.Write((short)0x1);

			var itemID = item.ItemID;

			if (item is BaseMulti)
			{
				m_Stream.Write((byte)0x02);
				m_Stream.Write(item.Serial);

				itemID &= 0x3FFF;

				m_Stream.Write((ushort)itemID);

				m_Stream.Write((byte)0);
			}
			else
			{
				if (unk != null && item is IDamageable d && !d.Invulnerable)
				{
					m_Stream.Write((byte)0x03);
				}
				else
				{
					m_Stream.Write((byte)0x00);
				}

				m_Stream.Write(item.Serial);

				itemID &= 0xFFFF;

				m_Stream.Write((ushort)itemID);

				m_Stream.Write((byte)0);
			}

			var amount = item.Amount;

			m_Stream.Write((short)amount);
			m_Stream.Write((short)amount);

			var loc = item.Location;
			var x = loc.m_X & 0x7FFF;
			var y = loc.m_Y & 0x3FFF;

			m_Stream.Write((short)x);
			m_Stream.Write((short)y);
			m_Stream.Write((sbyte)loc.m_Z);

			m_Stream.Write((byte)item.Light);
			m_Stream.Write((short)item.Hue);
			m_Stream.Write((byte)item.GetPacketFlags());

			if (unk != null)
			{
				m_Stream.Write((short)unk.Value); // ??
			}
		}

		private WorldItem(Item item)
			: base(0x1A)
		{
			var serial = (uint)item.Serial.Value;
			var itemID = item.ItemID & 0x3FFF;
			var amount = item.Amount;
			var loc = item.Location;
			var x = loc.m_X;
			var y = loc.m_Y;
			var hue = item.Hue;
			var flags = item.GetPacketFlags();
			var light = (int)item.Light;

			var size = 14;

			if (amount != 0)
			{
				size += 2;
			}

			if (light != 0)
			{
				size += 1;
			}

			if (hue != 0)
			{
				size += 2;
			}

			if (flags != 0)
			{
				size += 1;
			}

			EnsureCapacity(size);

			if (amount != 0)
			{
				serial |= 0x80000000;
			}
			else
			{
				serial &= 0x7FFFFFFF;
			}

			m_Stream.Write(serial);

			if (item is BaseMulti)
			{
				m_Stream.Write((short)(itemID | 0x4000));
			}
			else
			{
				m_Stream.Write((short)itemID);
			}

			if (amount != 0)
			{
				m_Stream.Write((short)amount);
			}

			x &= 0x7FFF;

			if (light != 0)
			{
				x |= 0x8000;
			}

			m_Stream.Write((short)x);

			y &= 0x3FFF;

			if (hue != 0)
			{
				y |= 0x8000;
			}

			if (flags != 0)
			{
				y |= 0x4000;
			}

			m_Stream.Write((short)y);

			if (light != 0)
			{
				m_Stream.Write((byte)light);
			}

			m_Stream.Write((sbyte)loc.m_Z);

			if (hue != 0)
			{
				m_Stream.Write((ushort)hue);
			}

			if (flags != 0)
			{
				m_Stream.Write((byte)flags);
			}
		}
	}

	public sealed class LiftRej : Packet
	{
		public static LiftRej CannotLift => PacketCache<LiftRej>.Acquire(LRReason.CannotLift, r => new LiftRej(r));
		public static LiftRej OutOfRange => PacketCache<LiftRej>.Acquire(LRReason.OutOfRange, r => new LiftRej(r));
		public static LiftRej OutOfSight => PacketCache<LiftRej>.Acquire(LRReason.OutOfSight, r => new LiftRej(r));
		public static LiftRej TryToSteal => PacketCache<LiftRej>.Acquire(LRReason.TryToSteal, r => new LiftRej(r));
		public static LiftRej AreHolding => PacketCache<LiftRej>.Acquire(LRReason.AreHolding, r => new LiftRej(r));
		public static LiftRej Inspecific => PacketCache<LiftRej>.Acquire(LRReason.Inspecific, r => new LiftRej(r));

		public static bool Send(NetState ns, LRReason reason)
		{
			return ns != null && Send(ns, Instantiate(ns, reason));
		}

		public static LiftRej Instantiate(NetState ns, LRReason reason)
		{
			return PacketCache<LiftRej>.Acquire(reason, r => new LiftRej(r));
		}

		private LiftRej(LRReason reason)
			: base(0x27, 2)
		{
			m_Stream.Write((byte)reason);
		}
	}

	public sealed class LogoutAck : Packet
	{
		public static LogoutAck Instance => PacketCache<LogoutAck>.Global(() => new LogoutAck());

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static LogoutAck Instantiate(NetState ns)
		{
			return PacketCache<LogoutAck>.Global(() => new LogoutAck());
		}

		private LogoutAck()
			: base(0xD1, 2)
		{
			m_Stream.Write((byte)0x01);
		}
	}

	public enum WeatherType
	{
		/// <summary>
		/// 0x00: "It starts to rain."
		/// </summary>
		Raining = 0x00,
		/// <summary>
		/// 0x01: "A fierce storm approaches."
		/// </summary>
		StormFierce = 0x01,
		/// <summary>
		/// 0x02: "It begins to snow"
		/// </summary>
		Snowing = 0x02,
		/// <summary>
		/// 0x03: "A storm is brewing."
		/// </summary>
		Storm = 0x03,
		/// <summary>
		/// 0x0E: Disable Effect
		/// </summary>
		DisableEffect = 0x0E,
		/// <summary>
		/// 0x0F: Disable Sound
		/// </summary>
		DisableSound = 0x0F,
		/// <summary>
		/// 0xFF: Disable
		/// </summary>
		Disable = 0xFF
	}

	public sealed class Weather : Packet
	{
		public Weather(WeatherType type, int density, int temperature)
			: base(0x65, 4)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write((byte)Math.Min(70, density));
			m_Stream.Write((byte)Math.Min(Byte.MaxValue, temperature));
		}
	}

	public sealed class UnkD3 : Packet
	{
		public UnkD3(Mobile beholder, Mobile beheld)
			: base(0xD3)
		{
			EnsureCapacity(256);

			m_Stream.Write(beheld.Serial);
			m_Stream.Write((short)beheld.Body);
			m_Stream.Write((short)beheld.X);
			m_Stream.Write((short)beheld.Y);
			m_Stream.Write((sbyte)beheld.Z);
			m_Stream.Write((byte)beheld.Direction);
			m_Stream.Write((ushort)beheld.Hue);
			m_Stream.Write((byte)beheld.GetPacketFlags());
			m_Stream.Write((byte)beheld.GetNotoriety(beholder));

			m_Stream.Fill(10);
		}
	}

	public sealed class GQRequest : Packet
	{
		public static GQRequest Instance => PacketCache<GQRequest>.Global(() => new GQRequest());

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static GQRequest Instantiate(NetState ns)
		{
			return PacketCache<GQRequest>.Global(() => new GQRequest());
		}

		private GQRequest()
			: base(0xC3)
		{
			EnsureCapacity(256);

			m_Stream.Write(1);
			m_Stream.Write(2); // ID
			m_Stream.Write(3); // Customer ? (this)
			m_Stream.Write(4); // Customer this (?)
			m_Stream.Write(0);
			m_Stream.Write((short)0);
			m_Stream.Write((short)6);
			m_Stream.Write((byte)'r');
			m_Stream.Write((byte)'e');
			m_Stream.Write((byte)'g');
			m_Stream.Write((byte)'i');
			m_Stream.Write((byte)'o');
			m_Stream.Write((byte)'n');
			m_Stream.Write(7); // Call time in seconds
			m_Stream.Write((short)2); // Map (0=fel,1=tram,2=ilsh)
			m_Stream.Write(8); // X
			m_Stream.Write(9); // Y
			m_Stream.Write(10); // Z
			m_Stream.Write(11); // Volume
			m_Stream.Write(12); // Rank
			m_Stream.Write(-1);
			m_Stream.Write(1); // type
		}
	}

	/// <summary>
	///     Causes the client to walk in a given direction. It does not send a movement request.
	/// </summary>
	public sealed class PlayerMove : Packet
	{
		public static PlayerMove WalkNorth => PacketCache<PlayerMove>.Acquire(Direction.North, d => new PlayerMove(d));
		public static PlayerMove WalkRight => PacketCache<PlayerMove>.Acquire(Direction.Right, d => new PlayerMove(d));
		public static PlayerMove WalkEast => PacketCache<PlayerMove>.Acquire(Direction.East, d => new PlayerMove(d));
		public static PlayerMove WalkDown => PacketCache<PlayerMove>.Acquire(Direction.Down, d => new PlayerMove(d));
		public static PlayerMove WalkSouth => PacketCache<PlayerMove>.Acquire(Direction.South, d => new PlayerMove(d));
		public static PlayerMove WalkLeft => PacketCache<PlayerMove>.Acquire(Direction.Left, d => new PlayerMove(d));
		public static PlayerMove WalkWest => PacketCache<PlayerMove>.Acquire(Direction.West, d => new PlayerMove(d));
		public static PlayerMove WalkUp => PacketCache<PlayerMove>.Acquire(Direction.Up, d => new PlayerMove(d));

		public static PlayerMove RunNorth => PacketCache<PlayerMove>.Acquire(Direction.North | Direction.Running, d => new PlayerMove(d));
		public static PlayerMove RunRight => PacketCache<PlayerMove>.Acquire(Direction.Right | Direction.Running, d => new PlayerMove(d));
		public static PlayerMove RunEast => PacketCache<PlayerMove>.Acquire(Direction.East | Direction.Running, d => new PlayerMove(d));
		public static PlayerMove RunDown => PacketCache<PlayerMove>.Acquire(Direction.Down | Direction.Running, d => new PlayerMove(d));
		public static PlayerMove RunSouth => PacketCache<PlayerMove>.Acquire(Direction.South | Direction.Running, d => new PlayerMove(d));
		public static PlayerMove RunLeft => PacketCache<PlayerMove>.Acquire(Direction.Left | Direction.Running, d => new PlayerMove(d));
		public static PlayerMove RunWest => PacketCache<PlayerMove>.Acquire(Direction.West | Direction.Running, d => new PlayerMove(d));
		public static PlayerMove RunUp => PacketCache<PlayerMove>.Acquire(Direction.Up | Direction.Running, d => new PlayerMove(d));

		public static bool Send(NetState ns, Direction dir)
		{
			return ns != null && Send(ns, Instantiate(ns, dir));
		}

		public static PlayerMove Instantiate(NetState ns, Direction dir)
		{
			return PacketCache<PlayerMove>.Acquire(dir, d => new PlayerMove(d));
		}

		private PlayerMove(Direction d)
			: base(0x97, 2)
		{
			m_Stream.Write((byte)d); // @4C63B0
		}
	}

	/// <summary>
	///     Displays a message "There are currently [count] available calls in the global queue.".
	/// </summary>
	public sealed class GQCount : Packet
	{
		public GQCount(int unk, int count)
			: base(0xCB, 7)
		{
			m_Stream.Write((short)unk);
			m_Stream.Write(count);
		}
	}

	/// <summary>
	///     Asks the client for it's version
	/// </summary>
	public sealed class ClientVersionReq : Packet
	{
		public static ClientVersionReq Instance => PacketCache<ClientVersionReq>.Global(() => new ClientVersionReq());

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static ClientVersionReq Instantiate(NetState ns)
		{
			return PacketCache<ClientVersionReq>.Global(() => new ClientVersionReq());
		}

		private ClientVersionReq()
			: base(0xBD)
		{
			EnsureCapacity(3);
		}
	}

	/// <summary>
	///     Asks the client for it's "assist version". (Perhaps for UOAssist?)
	/// </summary>
	public sealed class AssistVersionReq : Packet
	{
		public AssistVersionReq(int unk)
			: base(0xBE)
		{
			EnsureCapacity(7);

			m_Stream.Write(unk);
		}
	}

	public enum EffectType
	{
		Moving = 0x00,
		Lightning = 0x01,
		FixedXYZ = 0x02,
		FixedFrom = 0x03
	}

	public class ParticleEffect : Packet
	{
		public ParticleEffect(EffectType type, Serial from, Serial to, int itemID, Point3D fromPoint, Point3D toPoint, int speed, int duration, bool fixedDirection, bool explode, int hue, int renderMode, int effect, int explodeEffect, int explodeSound, Serial serial, int layer, int unknown)
			: base(0xC7, 49)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write(from);
			m_Stream.Write(to);
			m_Stream.Write((short)itemID);
			m_Stream.Write((short)fromPoint.m_X);
			m_Stream.Write((short)fromPoint.m_Y);
			m_Stream.Write((sbyte)fromPoint.m_Z);
			m_Stream.Write((short)toPoint.m_X);
			m_Stream.Write((short)toPoint.m_Y);
			m_Stream.Write((sbyte)toPoint.m_Z);
			m_Stream.Write((byte)speed);
			m_Stream.Write((byte)duration);
			m_Stream.Write((byte)0);
			m_Stream.Write((byte)0);
			m_Stream.Write(fixedDirection);
			m_Stream.Write(explode);
			m_Stream.Write(hue);
			m_Stream.Write(renderMode);
			m_Stream.Write((short)effect);
			m_Stream.Write((short)explodeEffect);
			m_Stream.Write((short)explodeSound);
			m_Stream.Write(serial);
			m_Stream.Write((byte)layer);
			m_Stream.Write((short)unknown);
		}

		public ParticleEffect(EffectType type, Serial from, Serial to, int itemID, IPoint3D fromPoint, IPoint3D toPoint, int speed, int duration, bool fixedDirection, bool explode, int hue, int renderMode, int effect, int explodeEffect, int explodeSound, Serial serial, int layer, int unknown)
			: base(0xC7, 49)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write(from);
			m_Stream.Write(to);
			m_Stream.Write((short)itemID);
			m_Stream.Write((short)fromPoint.X);
			m_Stream.Write((short)fromPoint.Y);
			m_Stream.Write((sbyte)fromPoint.Z);
			m_Stream.Write((short)toPoint.X);
			m_Stream.Write((short)toPoint.Y);
			m_Stream.Write((sbyte)toPoint.Z);
			m_Stream.Write((byte)speed);
			m_Stream.Write((byte)duration);
			m_Stream.Write((byte)0);
			m_Stream.Write((byte)0);
			m_Stream.Write(fixedDirection);
			m_Stream.Write(explode);
			m_Stream.Write(hue);
			m_Stream.Write(renderMode);
			m_Stream.Write((short)effect);
			m_Stream.Write((short)explodeEffect);
			m_Stream.Write((short)explodeSound);
			m_Stream.Write(serial);
			m_Stream.Write((byte)layer);
			m_Stream.Write((short)unknown);
		}
	}

	public class GraphicalEffect : Packet
	{
		public GraphicalEffect(EffectType type, Serial from, Serial to, int itemID, Point3D fromPoint, Point3D toPoint, int speed, int duration, bool fixedDirection, bool explode)
			: this(type, from, to, itemID, fromPoint, toPoint, speed, duration, fixedDirection, explode ? 1 : 0)
		{
		}

		public GraphicalEffect(EffectType type, Serial from, Serial to, int itemID, Point3D fromPoint, Point3D toPoint, int speed, int duration, bool fixedDirection, int explode)
			: base(0x70, 28)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write(from);
			m_Stream.Write(to);
			m_Stream.Write((short)itemID);
			m_Stream.Write((short)fromPoint.X);
			m_Stream.Write((short)fromPoint.Y);
			m_Stream.Write((sbyte)fromPoint.Z);
			m_Stream.Write((short)toPoint.X);
			m_Stream.Write((short)toPoint.Y);
			m_Stream.Write((sbyte)toPoint.Z);
			m_Stream.Write((byte)speed);
			m_Stream.Write((byte)duration);
			m_Stream.Write((byte)0);
			m_Stream.Write((byte)0);
			m_Stream.Write(fixedDirection);
			m_Stream.Write((byte)explode);
		}
	}

	public class HuedEffect : Packet
	{
		public HuedEffect(EffectType type, Serial from, Serial to, int itemID, Point3D fromPoint, Point3D toPoint, int speed, int duration, bool fixedDirection, bool explode, int hue, int renderMode)
			: base(0xC0, 36)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write(from);
			m_Stream.Write(to);
			m_Stream.Write((short)itemID);
			m_Stream.Write((short)fromPoint.m_X);
			m_Stream.Write((short)fromPoint.m_Y);
			m_Stream.Write((sbyte)fromPoint.m_Z);
			m_Stream.Write((short)toPoint.m_X);
			m_Stream.Write((short)toPoint.m_Y);
			m_Stream.Write((sbyte)toPoint.m_Z);
			m_Stream.Write((byte)speed);
			m_Stream.Write((byte)duration);
			m_Stream.Write((byte)0);
			m_Stream.Write((byte)0);
			m_Stream.Write(fixedDirection);
			m_Stream.Write(explode);
			m_Stream.Write(hue);
			m_Stream.Write(renderMode);
		}

		public HuedEffect(EffectType type, Serial from, Serial to, int itemID, IPoint3D fromPoint, IPoint3D toPoint, int speed, int duration, bool fixedDirection, bool explode, int hue, int renderMode)
			: this(type, from, to, itemID, fromPoint, toPoint, speed, duration, fixedDirection, explode, hue, renderMode, 0)
		{
		}

		public HuedEffect(EffectType type, Serial from, Serial to, int itemID, IPoint3D fromPoint, IPoint3D toPoint, int speed, int duration, bool fixedDirection, bool explode, int hue, int renderMode, int effect)
			: base(0xC0, 36)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write(from);
			m_Stream.Write(to);
			m_Stream.Write((short)itemID);
			m_Stream.Write((short)fromPoint.X);
			m_Stream.Write((short)fromPoint.Y);
			m_Stream.Write((sbyte)fromPoint.Z);
			m_Stream.Write((short)toPoint.X);
			m_Stream.Write((short)toPoint.Y);
			m_Stream.Write((sbyte)toPoint.Z);
			m_Stream.Write((byte)speed);
			m_Stream.Write((byte)duration);
			m_Stream.Write((byte)0);
			m_Stream.Write((byte)0);
			m_Stream.Write(fixedDirection);
			m_Stream.Write(explode);
			m_Stream.Write(hue);
			m_Stream.Write(effect);
		}
	}

	public sealed class TargetParticleEffect : ParticleEffect
	{
		public TargetParticleEffect(IEntity e, int itemID, int speed, int duration, int hue, int renderMode, int effect, int layer, int unknown)
			: base(EffectType.FixedFrom, e.Serial, Serial.Zero, itemID, e.Location, e.Location, speed, duration, true, false, hue, renderMode, effect, 1, 0, e.Serial, layer, unknown)
		{ }
	}

	public sealed class TargetEffect : HuedEffect
	{
		public TargetEffect(IEntity e, int itemID, int speed, int duration, int hue, int renderMode)
			: base(EffectType.FixedFrom, e.Serial, Serial.Zero, itemID, e.Location, e.Location, speed, duration, true, false, hue, renderMode)
		{ }
	}

	public sealed class LocationParticleEffect : ParticleEffect
	{
		public LocationParticleEffect(IEntity e, int itemID, int speed, int duration, int hue, int renderMode, int effect, int unknown)
			: base(EffectType.FixedXYZ, e.Serial, Serial.Zero, itemID, e.Location, e.Location, speed, duration, true, false, hue, renderMode, effect, 1, 0, e.Serial, 255, unknown)
		{ }
	}

	public sealed class LocationEffect : HuedEffect
	{
		public LocationEffect(IPoint3D p, int itemID, int speed, int duration, int hue, int renderMode)
			: base(EffectType.FixedXYZ, Serial.Zero, Serial.Zero, itemID, p, p, speed, duration, true, false, hue, renderMode)
		{ }
	}

	public sealed class MovingParticleEffect : ParticleEffect
	{
		public MovingParticleEffect(IEntity from, IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int hue, int renderMode, int effect, int explodeEffect, int explodeSound, EffectLayer layer, int unknown)
			: base(EffectType.Moving, from.Serial, to.Serial, itemID, from.Location, to.Location, speed, duration, fixedDirection, explodes, hue, renderMode, effect, explodeEffect, explodeSound, Serial.Zero, (int)layer, unknown)
		{ }
	}

	public sealed class MovingEffect : HuedEffect
	{
		public MovingEffect(IEntity from, IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int hue, int renderMode)
			: base(EffectType.Moving, from.Serial, to.Serial, itemID, from.Location, to.Location, speed, duration, fixedDirection, explodes, hue, renderMode)
		{ }
	}

	public enum ScreenEffectType
	{
		FadeOut = 0x00,
		FadeIn = 0x01,
		LightFlash = 0x02,
		FadeInOut = 0x03,
		DarkFlash = 0x04
	}

	public sealed class ScreenEffect : Packet
	{
		public static ScreenEffect FadeOut => PacketCache<ScreenEffect>.Acquire(ScreenEffectType.FadeOut, fx => new ScreenEffect(fx));
		public static ScreenEffect FadeIn => PacketCache<ScreenEffect>.Acquire(ScreenEffectType.FadeIn, fx => new ScreenEffect(fx));
		public static ScreenEffect FadeInOut => PacketCache<ScreenEffect>.Acquire(ScreenEffectType.FadeInOut, fx => new ScreenEffect(fx));
		public static ScreenEffect LightFlash => PacketCache<ScreenEffect>.Acquire(ScreenEffectType.LightFlash, fx => new ScreenEffect(fx));
		public static ScreenEffect DarkFlash => PacketCache<ScreenEffect>.Acquire(ScreenEffectType.DarkFlash, fx => new ScreenEffect(fx));

		public static bool Send(NetState ns, ScreenEffectType effect)
		{
			return ns != null && Send(ns, Instantiate(ns, effect));
		}

		public static ScreenEffect Instantiate(NetState ns, ScreenEffectType effect)
		{
			return PacketCache<ScreenEffect>.Acquire(effect, fx => new ScreenEffect(fx));
		}

		private ScreenEffect(ScreenEffectType type)
			: base(0x70, 28)
		{
			m_Stream.Write((byte)0x04);
			m_Stream.Fill(8);
			m_Stream.Write((short)type);
			m_Stream.Fill(16);
		}
	}

	public enum DeleteResultType
	{
		PasswordInvalid,
		CharNotExist,
		CharBeingPlayed,
		CharTooYoung,
		CharQueued,
		BadRequest
	}

	public sealed class DeleteResult : Packet
	{
		public DeleteResult(DeleteResultType res)
			: base(0x85, 2)
		{
			m_Stream.Write((byte)res);
		}
	}

	public sealed class BoltEffect : Packet
	{
		public BoltEffect(IEntity target, int hue)
			: base(0xC0, 36)
		{
			m_Stream.Write((byte)0x01); // type
			m_Stream.Write(target.Serial);
			m_Stream.Write(Serial.Zero);
			m_Stream.Write((short)0); // itemID
			m_Stream.Write((short)target.X);
			m_Stream.Write((short)target.Y);
			m_Stream.Write((sbyte)target.Z);
			m_Stream.Write((short)target.X);
			m_Stream.Write((short)target.Y);
			m_Stream.Write((sbyte)target.Z);
			m_Stream.Write((byte)0); // speed
			m_Stream.Write((byte)100); // duration
			m_Stream.Write((short)0); // unk
			m_Stream.Write(false); // fixed direction
			m_Stream.Write(false); // explode
			m_Stream.Write(hue);
			m_Stream.Write(0); // render mode
		}
	}

	public sealed class BoltEffectNew : Packet
	{
		public BoltEffectNew(IEntity target)
			: base(0x70, 28)
		{
			m_Stream.Write((byte)0x01); // type
			m_Stream.Write(target.Serial);
			m_Stream.Write(Serial.Zero);
			m_Stream.Write((short)0); // itemID
			m_Stream.Write((short)target.X);
			m_Stream.Write((short)target.Y);
			m_Stream.Write((sbyte)target.Z);
			m_Stream.Write((short)target.X);
			m_Stream.Write((short)target.Y);
			m_Stream.Write((sbyte)target.Z);
			m_Stream.Write((byte)0); // speed
			m_Stream.Write((byte)0); // duration
			m_Stream.Write((short)0); // unk
			m_Stream.Write(true); // fixed direction
			m_Stream.Write(true); // explode
		}
	}

	public sealed class DisplaySpellbook : Packet
	{
		public static bool Send(NetState ns, Item book)
		{
			return ns != null && Send(ns, Instantiate(ns, book));
		}

		public static DisplaySpellbook Instantiate(NetState ns, Item book)
		{
			if (ns.HighSeas)
			{
				return new DisplaySpellbook(book, 0x7D);
			}

			return new DisplaySpellbook(book, null);
		}

		private DisplaySpellbook(Item book, int? unk)
			: base(0x24, unk != null ? 9 : 7)
		{
			m_Stream.Write(book.Serial);
			m_Stream.Write((short)-1);

			if (unk != null)
			{
				m_Stream.Write((short)unk.Value);
			}
		}
	}

	public sealed class SpellbookContent : Packet
	{
		public static bool Send(NetState ns, Item item, int offset, ulong content)
		{
			return ns != null && Send(ns, Instantiate(ns, item, offset, content));
		}

		public static SpellbookContent Instantiate(NetState ns, Item item, int offset, ulong content)
		{
			if (ns.NewSpellbook)
			{
				return new SpellbookContent(item.Serial, item.ItemID, offset, content);
			}

			if (ns.ContainerGridLines)
			{
				return new SpellbookContent(item.Serial, offset, content, true);
			}

			return new SpellbookContent(item.Serial, offset, content, false);
		}

		private SpellbookContent(Serial s, int graphic, int offset, ulong content)
			: base(0xBF)
		{
			EnsureCapacity(23);

			m_Stream.Write((short)0x1B);
			m_Stream.Write((short)0x01);

			m_Stream.Write(s);
			m_Stream.Write((short)graphic);
			m_Stream.Write((short)offset);

			for (var i = 0; i < 8; ++i)
			{
				m_Stream.Write((byte)(content >> (i * 8)));
			}
		}

		private SpellbookContent(Serial s, int offset, ulong content, bool gridLocs)
		   : base(0x3C)
		{
			var count = 0;

			for (var i = 0; i < 64; ++i)
			{
				if ((content & (1UL << i)) != 0)
				{
					++count;
				}
			}

			EnsureCapacity(5 + (count * (gridLocs ? 20 : 19)));

			m_Stream.Write(count);

			for (var i = 0; i < 64; ++i)
			{
				if ((content & (1UL << i)) != 0)
				{
					m_Stream.Write((0x7FFFFFFF - i));
					m_Stream.Write((ushort)0);
					m_Stream.Write((byte)0);
					m_Stream.Write((ushort)(i + offset));
					m_Stream.Write((short)0);
					m_Stream.Write((short)0);

					if (gridLocs)
					{
						m_Stream.Write((byte)0); // Grid Location?
					}

					m_Stream.Write(s);
					m_Stream.Write((short)0);
				}
			}
		}
	}

	public sealed class ContainerDisplay : Packet
	{
		public static bool Send(NetState ns, Container c)
		{
			return ns != null && Send(ns, Instantiate(ns, c));
		}

		public static ContainerDisplay Instantiate(NetState ns, Container c)
		{
			if (ns.HighSeas)
			{
				return new ContainerDisplay(c, 0x7D);
			}

			return new ContainerDisplay(c, null);
		}

		private ContainerDisplay(Container c, short? unk)
			: base(0x24, unk != null ? 9 : 7)
		{
			m_Stream.Write(c.Serial);
			m_Stream.Write((short)c.GumpID);

			if (unk != null)
			{
				m_Stream.Write(unk.Value);
			}
		}
	}

	public sealed class ContainerContentUpdate : Packet
	{
		public static bool Send(NetState ns, Item c)
		{
			return ns != null && Send(ns, Instantiate(ns, c));
		}

		public static ContainerContentUpdate Instantiate(NetState ns, Item c)
		{
			if (ns.ContainerGridLines)
			{
				return new ContainerContentUpdate(c, true);
			}

			return new ContainerContentUpdate(c, false);
		}

		private ContainerContentUpdate(Item item, bool gridLocs)
			: base(0x25, gridLocs ? 21 : 20)
		{
			Serial parentSerial;

			if (item.Parent is Item ip)
			{
				parentSerial = ip.Serial;
			}
			else
			{
				parentSerial = Serial.Zero;
			}

			m_Stream.Write(item.Serial);
			m_Stream.Write((ushort)item.ItemID);
			m_Stream.Write((sbyte)0); // itemID offset
			m_Stream.Write((ushort)item.Amount);
			m_Stream.Write((short)item.X);
			m_Stream.Write((short)item.Y);

			if (gridLocs)
			{
				m_Stream.Write(item.GridLocation);
			}

			m_Stream.Write(parentSerial);
			m_Stream.Write((ushort)(item.QuestItem ? item.QuestItemHue : item.Hue));
		}
	}

	public sealed class ContainerContent : Packet
	{
		public static bool Send(NetState ns, Item c)
		{
			return ns != null && Send(ns, Instantiate(ns, c));
		}

		public static ContainerContent Instantiate(NetState ns, Item c)
		{
			if (ns.ContainerGridLines)
			{
				return new ContainerContent(ns.Mobile, c, true);
			}

			return new ContainerContent(ns.Mobile, c, false);
		}

		private ContainerContent(Mobile beholder, Item beheld, bool gridLocs)
			: base(0x3C)
		{
			var items = beheld.Items;
			var count = items.Count;

			EnsureCapacity(5 + (count * (gridLocs ? 20 : 19)));

			var written = 0;

			m_Stream.Write((ushort)0);

			for (var i = 0; i < count; ++i)
			{
				var child = items[i];

				if (!child.Deleted && beholder.CanSee(child))
				{
					if (child.GridLocation == 0xFF)
					{
						child.GridLocation = (byte)(count - written);
					}

					m_Stream.Write(child.Serial);
					m_Stream.Write((ushort)child.ItemID);
					m_Stream.Write((sbyte)0); // itemID offset
					m_Stream.Write((ushort)child.Amount);
					m_Stream.Write((short)child.X);
					m_Stream.Write((short)child.Y);

					if (gridLocs)
					{
						m_Stream.Write(child.GridLocation);
					}

					m_Stream.Write(beheld.Serial);
					m_Stream.Write((ushort)(child.QuestItem ? child.QuestItemHue : child.Hue));

					++written;
				}
			}

			m_Stream.Seek(3, SeekOrigin.Begin);
			m_Stream.Write((ushort)written);
		}
	}

	public sealed class SetWarMode : Packet
	{
		public static SetWarMode InWarMode => PacketCache<SetWarMode>.Acquire(true, s => new SetWarMode(s));
		public static SetWarMode InPeaceMode => PacketCache<SetWarMode>.Acquire(false, s => new SetWarMode(s));

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static SetWarMode Instantiate(NetState ns)
		{
			var state = ns.Mobile?.Warmode ?? false;

			return PacketCache<SetWarMode>.Acquire(state, s => new SetWarMode(s));
		}

		private SetWarMode(bool mode)
			: base(0x72, 5)
		{
			m_Stream.Write(mode);
			m_Stream.Write((byte)0x00);
			m_Stream.Write((byte)0x32);
			m_Stream.Write((byte)0x00);
		}
	}

	public sealed class Swing : Packet
	{
		public Swing(int flag, Mobile attacker, IDamageable defender)
			: base(0x2F, 10)
		{
			m_Stream.Write((byte)flag);
			m_Stream.Write(attacker.Serial);
			m_Stream.Write(defender.Serial);
		}
	}

	public sealed class NullFastwalkStack : Packet
	{
		public static NullFastwalkStack Instance => PacketCache<NullFastwalkStack>.Global(() => new NullFastwalkStack());

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static NullFastwalkStack Instantiate(NetState ns)
		{
			return PacketCache<NullFastwalkStack>.Global(() => new NullFastwalkStack());
		}

		private NullFastwalkStack()
			: base(0xBF)
		{
			EnsureCapacity(256);

			m_Stream.Write((short)0x1);
			m_Stream.Write(0x0);
			m_Stream.Write(0x0);
			m_Stream.Write(0x0);
			m_Stream.Write(0x0);
			m_Stream.Write(0x0);
			m_Stream.Write(0x0);
		}
	}

	public sealed class RemoveItem : Packet
	{
		public RemoveItem(Item item)
			: base(0x1D, 5)
		{
			m_Stream.Write(item.Serial);
		}
	}

	public sealed class RemoveMobile : Packet
	{
		public RemoveMobile(Mobile m)
			: base(0x1D, 5)
		{
			m_Stream.Write(m.Serial);
		}
	}

	public sealed class ServerChange : Packet
	{
		public ServerChange(Mobile m, Map map)
			: base(0x76, 16)
		{
			m_Stream.Write((short)m.X);
			m_Stream.Write((short)m.Y);
			m_Stream.Write((short)m.Z);
			m_Stream.Write((byte)0);
			m_Stream.Write((short)0);
			m_Stream.Write((short)0);
			m_Stream.Write((short)map.Width);
			m_Stream.Write((short)map.Height);
		}
	}

	public sealed class SkillUpdate : Packet
	{
		public SkillUpdate(Skills skills)
			: base(0x3A)
		{
			EnsureCapacity(6 + (skills.Length * 9));

			m_Stream.Write((byte)0x02); // type: absolute, capped

			foreach (var s in skills)
			{
				var v = s.NonRacialValue;
				var uv = (int)(v * 10);

				if (uv < 0)
				{
					uv = 0;
				}
				else if (uv >= 0x10000)
				{
					uv = 0xFFFF;
				}

				m_Stream.Write((ushort)(s.Info.SkillID + 1));
				m_Stream.Write((ushort)uv);
				m_Stream.Write((ushort)s.BaseFixedPoint);
				m_Stream.Write((byte)s.Lock);
				m_Stream.Write((ushort)s.CapFixedPoint);
			}

			m_Stream.Write((short)0); // terminate
		}
	}

	public sealed class Sequence : Packet
	{
		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static Sequence Instantiate(NetState ns)
		{
			var seq = ns.Sequence % 0x100;

			return PacketCache<Sequence>.Acquire(seq, key => new Sequence(key));
		}

		private Sequence(int seq)
			: base(0x7B, 2)
		{
			m_Stream.Write((byte)seq);
		}
	}

	public sealed class SkillChange : Packet
	{
		public SkillChange(Skill skill)
			: base(0x3A)
		{
			EnsureCapacity(13);

			var v = skill.NonRacialValue;
			var uv = (int)(v * 10);

			if (uv < 0)
			{
				uv = 0;
			}
			else if (uv >= 0x10000)
			{
				uv = 0xFFFF;
			}

			m_Stream.Write((byte)0xDF); // type: delta, capped
			m_Stream.Write((ushort)skill.Info.SkillID);
			m_Stream.Write((ushort)uv);
			m_Stream.Write((ushort)skill.BaseFixedPoint);
			m_Stream.Write((byte)skill.Lock);
			m_Stream.Write((ushort)skill.CapFixedPoint);
		}
	}

	public sealed class LaunchBrowser : Packet
	{
		public LaunchBrowser(string url)
			: base(0xA5)
		{
			if (url == null)
			{
				url = "";
			}

			EnsureCapacity(4 + url.Length);

			m_Stream.WriteAsciiNull(url);
		}
	}

	public sealed class MessageLocalized : Packet
	{
		public static MessageLocalized InstantiateGeneric(int number)
		{
			var cache = 0;
			var limit = 0;
			var index = 0;

			if (number >= 3000000)
			{
				cache = 1;
				limit = 15000;
				index = number - 3000000;
			}
			else if (number >= 1000000)
			{
				cache = 2;
				limit = 100000;
				index = number - 1000000;
			}
			else if (number >= 500000)
			{
				cache = 3;
				limit = 5000;
				index = number - 500000;
			}

			if (cache > 0 && limit > 0 && index >= 0 && index < limit)
			{
				index |= cache << 24;

				return PacketCache<MessageLocalized>.Acquire(index, () => new MessageLocalized(Serial.MinusOne, -1, MessageType.Regular, 0x3B2, 3, number, "System", ""));
			}

			return new MessageLocalized(Serial.MinusOne, -1, MessageType.Regular, 0x3B2, 3, number, "System", "");
		}

		public MessageLocalized(Serial serial, int graphic, MessageType type, int hue, int font, int number, string name, string args)
			: base(0xC1)
		{
			if (name == null)
			{
				name = "";
			}

			if (args == null)
			{
				args = "";
			}

			if (hue == 0)
			{
				hue = 0x3B2;
			}

			EnsureCapacity(50 + (args.Length * 2));

			m_Stream.Write(serial);
			m_Stream.Write((short)graphic);
			m_Stream.Write((byte)type);
			m_Stream.Write((short)hue);
			m_Stream.Write((short)font);
			m_Stream.Write(number);
			m_Stream.WriteAsciiFixed(name, 30);
			m_Stream.WriteLittleUniNull(args);
		}
	}

	public sealed class MobileMoving : Packet
	{
		public static bool Send(NetState ns, Mobile moving)
		{
			return ns != null && Send(ns, Instantiate(ns, moving));
		}

		public static MobileMoving Instantiate(NetState ns, Mobile moving)
		{
			var hash = (long)moving.Serial.Value << 32;

			var noto = Math.Max(0, Math.Min(7, moving.GetNotoriety(ns.Mobile)));

			hash |= (long)noto << 24;

			if (ns.StygianAbyss)
			{
				hash |= (long)Expansion.SA;

				return PacketCache<MobileMoving>.Acquire(hash, () => new MobileMoving(moving, noto, moving.GetPacketFlags()));
			}

			hash |= (long)Expansion.None;

			return PacketCache<MobileMoving>.Acquire(hash, () => new MobileMoving(moving, noto, moving.GetOldPacketFlags()));
		}

		public static void Free(Mobile moved)
		{
			var hash = (long)moved.Serial.Value << 32;

			for (var noto = 0L; noto <= 7; noto++)
			{
				var h = hash | (noto << 24);

				PacketCache<MobileMoving>.Free(h | (long)Expansion.SA);
				PacketCache<MobileMoving>.Free(h | (long)Expansion.None);
			}
		}

		private MobileMoving(Mobile m, int noto, int flags)
			: base(0x77, 17)
		{
			var loc = m.Location;
			var hue = m.Hue;

			if (m.SolidHueOverride >= 0)
			{
				hue = m.SolidHueOverride;
			}

			m_Stream.Write(m.Serial);
			m_Stream.Write((short)m.Body);
			m_Stream.Write((short)loc.m_X);
			m_Stream.Write((short)loc.m_Y);
			m_Stream.Write((sbyte)loc.m_Z);
			m_Stream.Write((byte)m.Direction);
			m_Stream.Write((short)hue);
			m_Stream.Write((byte)flags);
			m_Stream.Write((byte)noto);
		}
	}

	public sealed class MultiTargetReq : Packet
	{
		public static bool Send(NetState ns, MultiTarget t)
		{
			return ns != null && Send(ns, Instantiate(ns, t));
		}

		public static MultiTargetReq Instantiate(NetState ns, MultiTarget t)
		{
			if (ns.HighSeas)
			{
				return new MultiTargetReq(t, 0);
			}

			return new MultiTargetReq(t, null);
		}

		private MultiTargetReq(MultiTarget t, int? unk)
			: base(0x99, unk != null ? 30 : 26)
		{
			m_Stream.Write(t.AllowGround);
			m_Stream.Write(t.TargetID);
			m_Stream.Write((byte)t.Flags);

			m_Stream.Fill();

			m_Stream.Seek(18, SeekOrigin.Begin);
			m_Stream.Write((short)t.MultiID);
			m_Stream.Write((short)t.Offset.X);
			m_Stream.Write((short)t.Offset.Y);
			m_Stream.Write((short)t.Offset.Z);

			if (unk != null)
			{
				m_Stream.Write(unk.Value);
			}
		}
	}

	public sealed class CancelTarget : Packet
	{
		public static CancelTarget Instance => PacketCache<CancelTarget>.Global(() => new CancelTarget());

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static CancelTarget Instantiate(NetState ns)
		{
			return PacketCache<CancelTarget>.Global(() => new CancelTarget());
		}

		private CancelTarget()
			: base(0x6C, 19)
		{
			m_Stream.Write((byte)0);
			m_Stream.Write(0);
			m_Stream.Write((byte)3);
			m_Stream.Fill();
		}
	}

	public sealed class TargetReq : Packet
	{
		public TargetReq(Target t)
			: base(0x6C, 19)
		{
			m_Stream.Write(t.AllowGround);
			m_Stream.Write(t.TargetID);
			m_Stream.Write((byte)t.Flags);
			m_Stream.Fill();
		}
	}

	public sealed class DragEffect : Packet
	{
		public DragEffect(IEntity src, IEntity trg, int itemID, int hue, int amount)
			: base(0x23, 26)
		{
			m_Stream.Write((short)itemID);
			m_Stream.Write((byte)0);
			m_Stream.Write((short)hue);
			m_Stream.Write((short)amount);
			m_Stream.Write(src.Serial);
			m_Stream.Write((short)src.X);
			m_Stream.Write((short)src.Y);
			m_Stream.Write((sbyte)src.Z);
			m_Stream.Write(trg.Serial);
			m_Stream.Write((short)trg.X);
			m_Stream.Write((short)trg.Y);
			m_Stream.Write((sbyte)trg.Z);
		}
	}

	public interface IGumpWriter
	{
		int TextEntries { get; set; }
		int Switches { get; set; }

		void AppendLayout(bool val);
		void AppendLayout(int val);
		void AppendLayoutNS(int val);
		void AppendLayout(string text);
		void AppendLayout(byte[] buffer);

		void WriteStrings(List<string> strings);

		void Flush();
	}

	public sealed class DisplayGumpPacked : Packet, IGumpWriter
	{
		private const int GumpBufferSize = 0x10000;

		private static readonly BufferPool m_PackBuffers = new BufferPool("Gump", 4, GumpBufferSize);

		private static readonly byte[] m_True = Gump.StringToBuffer(" 1");
		private static readonly byte[] m_False = Gump.StringToBuffer(" 0");

		private static readonly byte[] m_BeginTextSeparator = Gump.StringToBuffer(" @");
		private static readonly byte[] m_EndTextSeparator = Gump.StringToBuffer("@");

		private static readonly byte[] m_Buffer = new byte[48];

		static DisplayGumpPacked()
		{
			m_Buffer[0] = (byte)' ';
		}

		public int TextEntries { get; set; }
		public int Switches { get; set; }

		private readonly Gump m_Gump;

		private PacketWriter m_Layout;
		private PacketWriter m_Strings;

		private int m_StringCount;

		public DisplayGumpPacked(Gump gump)
			: base(0xDD)
		{
			m_Gump = gump;

			m_Layout = PacketWriter.CreateInstance(8192);
			m_Strings = PacketWriter.CreateInstance(8192);
		}

		public void AppendLayout(bool val)
		{
			AppendLayout(val ? m_True : m_False);
		}

		public void AppendLayout(int val)
		{
			var toString = val.ToString();
			var bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1) + 1;

			m_Layout.Write(m_Buffer, 0, bytes);
		}

		public void AppendLayoutNS(int val)
		{
			var toString = val.ToString();
			var bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1);

			m_Layout.Write(m_Buffer, 1, bytes);
		}

		public void AppendLayout(string text)
		{
			AppendLayout(m_BeginTextSeparator);

			m_Layout.WriteAsciiFixed(text, text.Length);

			AppendLayout(m_EndTextSeparator);
		}

		public void AppendLayout(byte[] buffer)
		{
			m_Layout.Write(buffer, 0, buffer.Length);
		}

		public void WriteStrings(List<string> strings)
		{
			m_StringCount = strings.Count;

			for (var i = 0; i < strings.Count; ++i)
			{
				var v = strings[i] ?? String.Empty;

				m_Strings.Write((ushort)v.Length);
				m_Strings.WriteBigUniFixed(v, v.Length);
			}
		}

		public void Flush()
		{
			EnsureCapacity(28 + (int)m_Layout.Length + (int)m_Strings.Length);

			m_Stream.Write(m_Gump.Serial);
			m_Stream.Write(m_Gump.TypeID);
			m_Stream.Write(m_Gump.X);
			m_Stream.Write(m_Gump.Y);

			// Note: layout MUST be null terminated
			m_Layout.Write((byte)0);

			WritePacked(m_Layout);

			m_Stream.Write(m_StringCount);

			WritePacked(m_Strings);

			PacketWriter.ReleaseInstance(ref m_Layout);
			PacketWriter.ReleaseInstance(ref m_Strings);
		}

		private void WritePacked(PacketWriter src)
		{
			var buffer = src.UnderlyingStream.GetBuffer();
			var length = (int)src.Length;

			if (length == 0)
			{
				m_Stream.Write(0);
				return;
			}

			var wantLength = 1 + (buffer.Length * 1024 / 1000);

			wantLength += 4095;
			wantLength &= ~4095;

			var packBuffer = m_PackBuffers.AcquireBuffer();

			if (packBuffer.Length < wantLength)
			{
				packBuffer = new byte[wantLength];
			}

			var packLength = packBuffer.Length;

			Compression.Pack(packBuffer, ref packLength, buffer, length, ZLibQuality.Default);

			m_Stream.Write(4 + packLength);
			m_Stream.Write(length);
			m_Stream.Write(packBuffer, 0, packLength);

			m_PackBuffers.ReleaseBuffer(ref packBuffer);
		}
	}

	public sealed class DisplayGumpFast : Packet, IGumpWriter
	{
		private static readonly byte[] m_True = Gump.StringToBuffer(" 1");
		private static readonly byte[] m_False = Gump.StringToBuffer(" 0");

		private static readonly byte[] m_BeginTextSeparator = Gump.StringToBuffer(" @");
		private static readonly byte[] m_EndTextSeparator = Gump.StringToBuffer("@");

		private readonly byte[] m_Buffer = new byte[48];

		private int m_LayoutLength;

		public int TextEntries { get; set; }
		public int Switches { get; set; }

		public DisplayGumpFast(Gump g)
			: base(0xB0)
		{
			m_Buffer[0] = (byte)' ';

			EnsureCapacity(4096);

			m_Stream.Write(g.Serial);
			m_Stream.Write(g.TypeID);
			m_Stream.Write(g.X);
			m_Stream.Write(g.Y);
			m_Stream.Write((ushort)0xFFFF);
		}

		public void AppendLayout(bool val)
		{
			AppendLayout(val ? m_True : m_False);
		}

		public void AppendLayout(int val)
		{
			var toString = val.ToString();
			var bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1) + 1;

			m_Stream.Write(m_Buffer, 0, bytes);

			m_LayoutLength += bytes;
		}

		public void AppendLayoutNS(int val)
		{
			var toString = val.ToString();
			var bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1);

			m_Stream.Write(m_Buffer, 1, bytes);

			m_LayoutLength += bytes;
		}

		public void AppendLayout(string text)
		{
			AppendLayout(m_BeginTextSeparator);

			var length = text.Length;

			m_Stream.WriteAsciiFixed(text, length);

			m_LayoutLength += length;

			AppendLayout(m_EndTextSeparator);
		}

		public void AppendLayout(byte[] buffer)
		{
			var length = buffer.Length;

			m_Stream.Write(buffer, 0, length);

			m_LayoutLength += length;
		}

		public void WriteStrings(List<string> text)
		{
			m_Stream.Seek(19, SeekOrigin.Begin);
			m_Stream.Write((ushort)m_LayoutLength);

			m_Stream.Seek(0, SeekOrigin.End);
			m_Stream.Write((ushort)text.Count);

			for (var i = 0; i < text.Count; ++i)
			{
				var v = text[i] ?? String.Empty;

				int length = (ushort)v.Length;

				m_Stream.Write((ushort)length);
				m_Stream.WriteBigUniFixed(v, length);
			}
		}

		public void Flush()
		{ }
	}

	public sealed class DisplayGump : Packet
	{
		public DisplayGump(Gump g, string layout, string[] text)
			: base(0xB0)
		{
			if (layout == null)
			{
				layout = "";
			}

			EnsureCapacity(256);

			m_Stream.Write(g.Serial);
			m_Stream.Write(g.TypeID);
			m_Stream.Write(g.X);
			m_Stream.Write(g.Y);
			m_Stream.Write((ushort)(layout.Length + 1));
			m_Stream.WriteAsciiNull(layout);

			m_Stream.Write((ushort)text.Length);

			for (var i = 0; i < text.Length; ++i)
			{
				var v = text[i] ?? String.Empty;

				int length = (ushort)v.Length;

				m_Stream.Write((ushort)length);
				m_Stream.WriteBigUniFixed(v, length);
			}
		}
	}

	public sealed class DisplayPaperdoll : Packet
	{
		public DisplayPaperdoll(Mobile m, string text, bool canLift)
			: base(0x88, 66)
		{
			byte flags = 0x00;

			if (m.Warmode)
			{
				flags |= 0x01;
			}

			if (canLift)
			{
				flags |= 0x02;
			}

			m_Stream.Write(m.Serial);
			m_Stream.WriteAsciiFixed(text, 60);
			m_Stream.Write(flags);
		}
	}

	public sealed class PopupMessage : Packet
	{
		public static PopupMessage CharNoExist => PacketCache<PopupMessage>.Acquire(PMMessage.CharNoExist, m => new PopupMessage(m));
		public static PopupMessage CharExists => PacketCache<PopupMessage>.Acquire(PMMessage.CharExists, m => new PopupMessage(m));
		public static PopupMessage CharInWorld => PacketCache<PopupMessage>.Acquire(PMMessage.CharInWorld, m => new PopupMessage(m));
		public static PopupMessage LoginSyncError => PacketCache<PopupMessage>.Acquire(PMMessage.LoginSyncError, m => new PopupMessage(m));
		public static PopupMessage IdleWarning => PacketCache<PopupMessage>.Acquire(PMMessage.IdleWarning, m => new PopupMessage(m));

		public static bool Send(NetState ns, PMMessage msg)
		{
			return ns != null && Send(ns, Instantiate(ns, msg));
		}

		public static PopupMessage Instantiate(NetState ns, PMMessage msg)
		{
			return PacketCache<PopupMessage>.Acquire(msg, m => new PopupMessage(m));
		}

		private PopupMessage(PMMessage msg)
			: base(0x53, 2)
		{
			m_Stream.Write((byte)msg);
		}
	}

	public sealed class PlaySound : Packet
	{
		public PlaySound(int soundID, IPoint3D target)
			: base(0x54, 12)
		{
			m_Stream.Write((byte)1); // flags
			m_Stream.Write((short)soundID);
			m_Stream.Write((short)0); // volume
			m_Stream.Write((short)target.X);
			m_Stream.Write((short)target.Y);
			m_Stream.Write((short)target.Z);
		}
	}

	public sealed class PlayMusic : Packet
	{
		public static PlayMusic Invalid => PacketCache<PlayMusic>.Acquire(MusicName.Invalid, n => new PlayMusic(n));

		public static bool Send(NetState ns, MusicName name)
		{
			return ns != null && Send(ns, Instantiate(ns, name));
		}

		public static PlayMusic Instantiate(NetState ns, MusicName name)
		{
			return PacketCache<PlayMusic>.Acquire(name, n => new PlayMusic(n));
		}

		private PlayMusic(MusicName music)
			: base(0x6D, 3)
		{
			m_Stream.Write((short)music);
		}
	}

	public sealed class ScrollMessage : Packet
	{
		public ScrollMessage(int type, int tip, string text)
			: base(0xA6)
		{
			if (text == null)
			{
				text = "";
			}

			EnsureCapacity(10 + text.Length);

			m_Stream.Write((byte)type);
			m_Stream.Write(tip);
			m_Stream.Write((ushort)text.Length);
			m_Stream.WriteAsciiFixed(text, text.Length);
		}
	}

	public sealed class CurrentTime : Packet
	{
		public CurrentTime()
			: base(0x5B, 4)
		{
			var now = DateTime.UtcNow;

			m_Stream.Write((byte)now.Hour);
			m_Stream.Write((byte)now.Minute);
			m_Stream.Write((byte)now.Second);
		}
	}

	public sealed class MapChange : Packet
	{
		public static MapChange Felucca => PacketCache<MapChange>.Acquire(0, id => new MapChange(id));
		public static MapChange Trammel => PacketCache<MapChange>.Acquire(1, id => new MapChange(id));
		public static MapChange Ilshenar => PacketCache<MapChange>.Acquire(2, id => new MapChange(id));
		public static MapChange Malas => PacketCache<MapChange>.Acquire(3, id => new MapChange(id));
		public static MapChange Tokuno => PacketCache<MapChange>.Acquire(4, id => new MapChange(id));
		public static MapChange TerMur => PacketCache<MapChange>.Acquire(5, id => new MapChange(id));

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static MapChange Instantiate(NetState ns)
		{
			var mapID = ns.Mobile?.Map?.MapID ?? 0;

			return PacketCache<MapChange>.Acquire(mapID, id => new MapChange(id));
		}

		private MapChange(int mapID)
			: base(0xBF)
		{
			EnsureCapacity(6);

			m_Stream.Write((short)0x08);
			m_Stream.Write((byte)mapID);
		}
	}

	public sealed class SeasonChange : Packet
	{
		public static SeasonChange Spring => PacketCache<SeasonChange>.Acquire(1 << 0, () => new SeasonChange(0, false));
		public static SeasonChange Summer => PacketCache<SeasonChange>.Acquire(1 << 1, () => new SeasonChange(1, false));
		public static SeasonChange Autumn => PacketCache<SeasonChange>.Acquire(1 << 2, () => new SeasonChange(2, false));
		public static SeasonChange Winter => PacketCache<SeasonChange>.Acquire(1 << 3, () => new SeasonChange(3, false));
		public static SeasonChange Desolate => PacketCache<SeasonChange>.Acquire(1 << 4, () => new SeasonChange(4, false));

		public static SeasonChange SpringSFX => PacketCache<SeasonChange>.Acquire(1 << 5, () => new SeasonChange(0, true));
		public static SeasonChange SummerSFX => PacketCache<SeasonChange>.Acquire(1 << 6, () => new SeasonChange(1, true));
		public static SeasonChange AutumnSFX => PacketCache<SeasonChange>.Acquire(1 << 7, () => new SeasonChange(2, true));
		public static SeasonChange WinterSFX => PacketCache<SeasonChange>.Acquire(1 << 8, () => new SeasonChange(3, true));
		public static SeasonChange DesolateSFX => PacketCache<SeasonChange>.Acquire(1 << 9, () => new SeasonChange(4, true));

		public static bool Send(NetState ns, bool playSound)
		{
			return ns != null && Send(ns, Instantiate(ns, playSound));
		}

		public static SeasonChange Instantiate(NetState ns, bool playSound)
		{
			var season = Math.Max(0, Math.Min(4, ns.Mobile?.GetSeason() ?? 0));

			var uid = 1 << (season + (playSound ? 5 : 0));

			return PacketCache<SeasonChange>.Acquire(uid, () => new SeasonChange(season, playSound));
		}

		private SeasonChange(int season, bool playSound)
			: base(0xBC, 3)
		{
			m_Stream.Write((byte)season);
			m_Stream.Write(playSound);
		}
	}

	public delegate void FeatureValidator(IAccount acct, ref FeatureFlags flags);

	public sealed class SupportedFeatures : Packet
	{
		public static event FeatureValidator Validate;

		public static FeatureFlags Value { get; set; }

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static SupportedFeatures Instantiate(NetState ns)
		{
			if (ns.ExtendedSupportedFeatures)
			{
				return new SupportedFeatures(ns.Account, true);
			}

			return new SupportedFeatures(ns.Account, false);
		}

		private SupportedFeatures(IAccount acct, bool extended)
			: base(0xB9, extended ? 5 : 3)
		{
			var flags = ExpansionInfo.CoreExpansion.SupportedFeatures;

			flags |= Value;

			if (acct != null && acct.Limit >= 6)
			{
				flags |= FeatureFlags.LiveAccount;
				flags &= ~FeatureFlags.UOTD;

				if (acct.Limit > 6)
				{
					flags |= FeatureFlags.SeventhCharacterSlot;
				}
				else
				{
					flags |= FeatureFlags.SixthCharacterSlot;
				}
			}

			Validate?.Invoke(acct, ref flags);

			if (extended)
			{
				m_Stream.Write((uint)flags);
			}
			else
			{
				m_Stream.Write((ushort)flags);
			}
		}
	}

	public static class AttributeNormalizer
	{
		public static int Maximum { get; set; } = 25;

		public static bool Enabled { get; set; } = true;

		public static void Write(PacketWriter stream, int cur, int max)
		{
			if (Enabled && max != 0)
			{
				stream.Write((short)Maximum);
				stream.Write((short)(cur * Maximum / max));
			}
			else
			{
				stream.Write((short)max);
				stream.Write((short)cur);
			}
		}

		public static void WriteReverse(PacketWriter stream, int cur, int max)
		{
			if (Enabled && max != 0)
			{
				stream.Write((short)(cur * Maximum / max));
				stream.Write((short)Maximum);
			}
			else
			{
				stream.Write((short)cur);
				stream.Write((short)max);
			}
		}
	}

	public sealed class MobileHits : Packet
	{
		public MobileHits(Mobile m)
			: base(0xA1, 9)
		{
			m_Stream.Write(m.Serial);

			m_Stream.Write((short)m.HitsMax);
			m_Stream.Write((short)m.Hits);
		}
	}

	public sealed class MobileHitsN : Packet
	{
		public MobileHitsN(IDamageable d)
			: base(0xA1, 9)
		{
			m_Stream.Write(d.Serial);

			AttributeNormalizer.Write(m_Stream, d.Hits, d.HitsMax);
		}
	}

	public sealed class MobileMana : Packet
	{
		public MobileMana(Mobile m)
			: base(0xA2, 9)
		{
			m_Stream.Write(m.Serial);

			m_Stream.Write((short)m.ManaMax);
			m_Stream.Write((short)m.Mana);
		}
	}

	public sealed class MobileManaN : Packet
	{
		public MobileManaN(Mobile m)
			: base(0xA2, 9)
		{
			m_Stream.Write(m.Serial);

			AttributeNormalizer.Write(m_Stream, m.Mana, m.ManaMax);
		}
	}

	public sealed class MobileStam : Packet
	{
		public MobileStam(Mobile m)
			: base(0xA3, 9)
		{
			m_Stream.Write(m.Serial);

			m_Stream.Write((short)m.StamMax);
			m_Stream.Write((short)m.Stam);
		}
	}

	public sealed class MobileStamN : Packet
	{
		public MobileStamN(Mobile m)
			: base(0xA3, 9)
		{
			m_Stream.Write(m.Serial);

			AttributeNormalizer.Write(m_Stream, m.Stam, m.StamMax);
		}
	}

	public sealed class MobileAttributes : Packet
	{
		public MobileAttributes(Mobile m)
			: base(0x2D, 17)
		{
			m_Stream.Write(m.Serial);

			m_Stream.Write((short)m.HitsMax);
			m_Stream.Write((short)m.Hits);

			m_Stream.Write((short)m.ManaMax);
			m_Stream.Write((short)m.Mana);

			m_Stream.Write((short)m.StamMax);
			m_Stream.Write((short)m.Stam);
		}
	}

	public sealed class MobileAttributesN : Packet
	{
		public MobileAttributesN(Mobile m)
			: base(0x2D, 17)
		{
			m_Stream.Write(m.Serial);

			AttributeNormalizer.Write(m_Stream, m.Hits, m.HitsMax);
			AttributeNormalizer.Write(m_Stream, m.Mana, m.ManaMax);
			AttributeNormalizer.Write(m_Stream, m.Stam, m.StamMax);
		}
	}

	public sealed class PathfindMessage : Packet
	{
		public PathfindMessage(IPoint3D p)
			: base(0x38, 7)
		{
			m_Stream.Write((short)p.X);
			m_Stream.Write((short)p.Y);
			m_Stream.Write((short)p.Z);
		}
	}

	public sealed class MobileName : Packet
	{
		public MobileName(Mobile m)
			: base(0x98)
		{
			var name = m.Name ?? String.Empty;

			if (!String.IsNullOrWhiteSpace(name) && name.IndexOfAny(new[] { '<', '>' }) >= 0)
			{
				name = Regex.Replace(name, @"<[^>]*>", String.Empty);
			}

			EnsureCapacity(37);

			m_Stream.Write(m.Serial);
			m_Stream.WriteAsciiFixed(name, 30);
		}
	}

	public sealed class MobileAnimation : Packet
	{
		public MobileAnimation(Mobile m, int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
			: base(0x6E, 14)
		{
			m_Stream.Write(m.Serial);

			m_Stream.Write((short)action);
			m_Stream.Write((short)frameCount);
			m_Stream.Write((short)repeatCount);
			m_Stream.Write(!forward); // protocol has really "reverse" but I find this more intuitive
			m_Stream.Write(repeat);
			m_Stream.Write((byte)delay);
		}
	}

	public sealed class NewMobileAnimation : Packet
	{
		public NewMobileAnimation(Mobile m, AnimationType type, int action, int delay)
			: base(0xE2, 10)
		{
			m_Stream.Write(m.Serial);

			m_Stream.Write((short)type);
			m_Stream.Write((short)action);
			m_Stream.Write((byte)delay);
		}
	}

	public sealed class MobileStatus : Packet
	{
		public static bool Send(NetState ns, IDamageable beheld)
		{
			return ns != null && Send(ns, Instantiate(ns, beheld));
		}

		public static MobileStatus Instantiate(NetState ns, IDamageable beheld)
		{
			int type;

			if (ns.Mobile != beheld)
			{
				type = 0;
			}
			else if (Core.ML && ns.ExtendedStatus)
			{
				type = 6;
			}
			else if (Core.ML && ns.SupportsExpansion(Expansion.ML))
			{
				type = 5;
			}
			else if (Core.AOS)
			{
				type = 4;
			}
			else
			{
				type = 3;
			}

			return new MobileStatus(ns.Mobile, beheld, type, ns.IsEnhancedClient);
		}

		private MobileStatus(Mobile beholder, IDamageable beheld, int type, bool enhanced)
			: base(0x11)
		{
			var size = 3;

			switch (type)
			{
				case 0:
				size += 40;
				break;
				case 3:
				size += 85;
				break;
				case 4:
				size += 85;
				break;
				case 5:
				size += 88;
				break;
				case 6:
				size += enhanced ? 148 : 118;
				break;
				default:
				type = 0;
				goto case 0;
			}

			EnsureCapacity(size);

			var name = beheld?.Name ?? String.Empty;

			m_Stream.Write(beheld.Serial);
			m_Stream.WriteAsciiFixed(name, 30);

			if (type == 0)
			{
				WriteAttrNorm(beheld.Hits, beheld.HitsMax);
			}
			else
			{
				WriteAttr(beheld.Hits, beheld.HitsMax);
			}

			if (beheld is Mobile r)
			{
				m_Stream.Write(r.CanBeRenamedBy(beholder));
			}
			else
			{
				m_Stream.Write(false);
			}

			m_Stream.Write((byte)type);

			if (type > 0 && beheld is Mobile m)
			{
				m_Stream.Write(m.Female);

				m_Stream.Write((short)m.Str);
				m_Stream.Write((short)m.Dex);
				m_Stream.Write((short)m.Int);

				WriteAttr(m.Stam, m.StamMax);
				WriteAttr(m.Mana, m.ManaMax);

				m_Stream.Write(m.TotalGold);

				if (Core.AOS)
				{
					m_Stream.Write((short)m.PhysicalResistance);
				}
				else
				{
					m_Stream.Write((short)(m.ArmorRating + 0.5));
				}

				m_Stream.Write((short)(Mobile.BodyWeight + m.TotalWeight));

				if (type >= 5)
				{
					m_Stream.Write((short)m.MaxWeight);
					m_Stream.Write((byte)(m.Race.RaceID + 1)); // Would be 0x00 if it's a non-ML enabled account but...
				}

				m_Stream.Write((short)m.StatCap);

				m_Stream.Write((byte)m.Followers);
				m_Stream.Write((byte)m.FollowersMax);

				if (type >= 4)
				{
					m_Stream.Write((short)m.FireResistance); // Fire
					m_Stream.Write((short)m.ColdResistance); // Cold
					m_Stream.Write((short)m.PoisonResistance); // Poison
					m_Stream.Write((short)m.EnergyResistance); // Energy
					m_Stream.Write((short)m.Luck); // Luck

					var weapon = m.Weapon;

					int min = 0, max = 0;

					if (weapon != null)
					{
						weapon.GetStatusDamage(m, out min, out max);
					}

					m_Stream.Write((short)min); // Damage min
					m_Stream.Write((short)max); // Damage max

					m_Stream.Write(m.TithingPoints);
				}

				if (type >= 6)
				{
					var count = enhanced ? 28 : 14;

					for (var i = 0; i <= count; ++i)
					{
						m_Stream.Write((short)m.GetAOSStatus(i));
					}
				}
			}

			m_Stream.Fill();
		}

		private void WriteAttr(int current, int maximum)
		{
			m_Stream.Write((short)current);
			m_Stream.Write((short)maximum);
		}

		private void WriteAttrNorm(int current, int maximum)
		{
			AttributeNormalizer.WriteReverse(m_Stream, current, maximum);
		}
	}

	public sealed class HealthbarPoison : Packet
	{
		public HealthbarPoison(Mobile m)
			: base(0x17)
		{
			EnsureCapacity(12);

			m_Stream.Write(m.Serial);

			m_Stream.Write((short)1);
			m_Stream.Write((short)1);

			var p = m.Poison;

			if (p != null)
			{
				m_Stream.Write((byte)(p.Level + 1));
			}
			else
			{
				m_Stream.Write((byte)0);
			}
		}
	}

	public sealed class HealthbarYellow : Packet
	{
		public HealthbarYellow(Mobile m)
			: base(0x17)
		{
			EnsureCapacity(12);

			m_Stream.Write(m.Serial);

			m_Stream.Write((short)1);
			m_Stream.Write((short)2);

			if (m.Blessed || m.YellowHealthbar)
			{
				m_Stream.Write((byte)1);
			}
			else
			{
				m_Stream.Write((byte)0);
			}
		}
	}

	public sealed class HealthbarYellowEC : Packet
	{
		public HealthbarYellowEC(Mobile m)
			: base(0x16)
		{
			EnsureCapacity(12);

			m_Stream.Write(m.Serial);

			m_Stream.Write((short)1);
			m_Stream.Write((short)2);

			if (m.Blessed || m.YellowHealthbar)
			{
				m_Stream.Write((byte)1);
			}
			else
			{
				m_Stream.Write((byte)0);
			}
		}
	}

	public sealed class HealthbarPoisonEC : Packet
	{
		public HealthbarPoisonEC(Mobile m)
			: base(0x16)
		{
			EnsureCapacity(12);

			m_Stream.Write(m.Serial);

			m_Stream.Write((short)1);
			m_Stream.Write((short)1);

			var p = m.Poison;

			if (p != null)
			{
				m_Stream.Write((byte)(p.Level + 1));
			}
			else
			{
				m_Stream.Write((byte)0);
			}
		}
	}

	public sealed class MobileUpdate : Packet
	{
		public static bool Send(NetState ns, Mobile beheld)
		{
			return ns != null && Send(ns, Instantiate(ns, beheld));
		}

		public static MobileUpdate Instantiate(NetState ns, Mobile beheld)
		{
			if (ns.StygianAbyss)
			{
				return new MobileUpdate(beheld, beheld.GetPacketFlags());
			}

			return new MobileUpdate(beheld, beheld.GetOldPacketFlags());
		}

		private MobileUpdate(Mobile m, int flags)
			: base(0x20, 19)
		{
			var hue = m.Hue;

			if (m.SolidHueOverride >= 0)
			{
				hue = m.SolidHueOverride;
			}

			m_Stream.Write(m.Serial);
			m_Stream.Write((short)m.Body);
			m_Stream.Write((byte)0);
			m_Stream.Write((short)hue);
			m_Stream.Write((byte)flags);
			m_Stream.Write((short)m.X);
			m_Stream.Write((short)m.Y);
			m_Stream.Write((short)0);
			m_Stream.Write((byte)m.Direction);
			m_Stream.Write((sbyte)m.Z);
		}
	}

	public sealed class MobileIncoming : Packet
	{
		private static readonly ThreadLocal<int[]> m_DupedLayersTL = new ThreadLocal<int[]>(() => new int[256]);
		private static readonly ThreadLocal<int> m_VersionTL = new ThreadLocal<int>();

		public static bool Send(NetState ns, Mobile beheld)
		{
			return ns != null && Send(ns, Instantiate(ns, beheld));
		}

		public static MobileIncoming Instantiate(NetState ns, Mobile beheld)
		{
			if (ns.NewMobileIncoming)
			{
				return new MobileIncoming(ns.Mobile, beheld, beheld.GetPacketFlags(), true, false);
			}

			if (ns.StygianAbyss)
			{
				return new MobileIncoming(ns.Mobile, beheld, beheld.GetPacketFlags(), true, true);
			}

			return new MobileIncoming(ns.Mobile, beheld, beheld.GetOldPacketFlags(), false, true);
		}

		private MobileIncoming(Mobile beholder, Mobile beheld, int flags, bool useFace, bool prefixHues)
			: base(0x78)
		{
			var m_Version = ++m_VersionTL.Value;
			var m_DupedLayers = m_DupedLayersTL.Value;

			var eq = beheld.Items;
			var count = eq.Count;

			if (beheld.HairItemID > 0)
			{
				count++;
			}

			if (beheld.FacialHairItemID > 0)
			{
				count++;
			}

			if (useFace && beheld.FaceItemID > 0)
			{
				count++;
			}

			EnsureCapacity(23 + (count * 9));

			var hue = beheld.Hue;

			if (beheld.SolidHueOverride >= 0)
			{
				hue = beheld.SolidHueOverride;
			}

			m_Stream.Write(beheld.Serial);
			m_Stream.Write((short)beheld.Body);
			m_Stream.Write((short)beheld.X);
			m_Stream.Write((short)beheld.Y);
			m_Stream.Write((sbyte)beheld.Z);
			m_Stream.Write((byte)beheld.Direction);
			m_Stream.Write((short)hue);
			m_Stream.Write((byte)flags);
			m_Stream.Write((byte)beheld.GetNotoriety(beholder));

			for (var i = 0; i < eq.Count; ++i)
			{
				var item = eq[i];

				var layer = (byte)item.Layer;

				if (!item.Deleted && beholder.CanSee(item) && m_DupedLayers[layer] != m_Version)
				{
					m_DupedLayers[layer] = m_Version;

					hue = item.Hue;

					if (beheld.SolidHueOverride >= 0)
					{
						hue = beheld.SolidHueOverride;
					}

					var itemID = item.ItemID & (prefixHues ? 0x7FFF : 0xFFFF);

					if (prefixHues && hue != 0)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(item.Serial);
					m_Stream.Write((ushort)itemID);
					m_Stream.Write(layer);

					if (!prefixHues || hue != 0)
					{
						m_Stream.Write((short)hue);
					}
				}
			}

			if (beheld.HairItemID > 0)
			{
				if (m_DupedLayers[(int)Layer.Hair] != m_Version)
				{
					m_DupedLayers[(int)Layer.Hair] = m_Version;
					hue = beheld.HairHue;

					if (beheld.SolidHueOverride >= 0)
					{
						hue = beheld.SolidHueOverride;
					}

					var itemID = beheld.HairItemID & (prefixHues ? 0x7FFF : 0xFFFF);

					if (prefixHues && hue != 0)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(HairInfo.FakeSerial(beheld));
					m_Stream.Write((ushort)itemID);
					m_Stream.Write((byte)Layer.Hair);

					if (!prefixHues || hue != 0)
					{
						m_Stream.Write((short)hue);
					}
				}
			}

			if (beheld.FacialHairItemID > 0)
			{
				if (m_DupedLayers[(int)Layer.FacialHair] != m_Version)
				{
					m_DupedLayers[(int)Layer.FacialHair] = m_Version;
					hue = beheld.FacialHairHue;

					if (beheld.SolidHueOverride >= 0)
					{
						hue = beheld.SolidHueOverride;
					}

					var itemID = beheld.FacialHairItemID & (prefixHues ? 0x7FFF : 0xFFFF);

					if (prefixHues && hue != 0)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(FacialHairInfo.FakeSerial(beheld));
					m_Stream.Write((ushort)itemID);
					m_Stream.Write((byte)Layer.FacialHair);

					if (!prefixHues || hue != 0)
					{
						m_Stream.Write((short)hue);
					}
				}
			}

			if (useFace && beheld.FaceItemID > 0)
			{
				if (m_DupedLayers[(int)Layer.Face] != m_Version)
				{
					m_DupedLayers[(int)Layer.Face] = m_Version;
					hue = beheld.FaceHue;

					if (beheld.SolidHueOverride >= 0)
					{
						hue = beheld.SolidHueOverride;
					}

					var itemID = beheld.FaceItemID & (prefixHues ? 0x7FFF : 0xFFFF);

					if (prefixHues && hue != 0)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(FaceInfo.FakeSerial(beheld));
					m_Stream.Write((ushort)itemID);
					m_Stream.Write((byte)Layer.Face);

					if (!prefixHues || hue != 0)
					{
						m_Stream.Write((short)hue);
					}
				}
			}

			m_Stream.Write(0); // terminate
		}
	}

	public sealed class AsciiMessage : Packet
	{
		public AsciiMessage(Serial serial, int graphic, MessageType type, int hue, int font, string name, string text)
			: base(0x1C)
		{
			if (name == null)
			{
				name = "";
			}

			if (text == null)
			{
				text = "";
			}

			if (hue == 0)
			{
				hue = 0x3B2;
			}

			EnsureCapacity(45 + text.Length);

			m_Stream.Write(serial);
			m_Stream.Write((short)graphic);
			m_Stream.Write((byte)type);
			m_Stream.Write((short)hue);
			m_Stream.Write((short)font);
			m_Stream.WriteAsciiFixed(name, 30);
			m_Stream.WriteAsciiNull(text);
		}
	}

	public sealed class UnicodeMessage : Packet
	{
		public UnicodeMessage(Serial serial, int graphic, MessageType type, int hue, int font, string lang, string name, string text)
			: base(0xAE)
		{
			if (String.IsNullOrEmpty(lang))
			{
				lang = "ENU";
			}

			if (name == null)
			{
				name = "";
			}

			if (text == null)
			{
				text = "";
			}

			if (hue == 0)
			{
				hue = 0x3B2;
			}

			EnsureCapacity(48 + (text.Length * 2) + 2);

			m_Stream.Write(serial);
			m_Stream.Write((short)graphic);
			m_Stream.Write((byte)type);
			m_Stream.Write((short)hue);
			m_Stream.Write((short)font);
			m_Stream.WriteAsciiFixed(lang, 4);
			m_Stream.WriteAsciiFixed(name, 30);
			m_Stream.WriteBigUniNull(text);
		}
	}

	public sealed class PingAck : Packet
	{
		public static bool Send(NetState ns, byte ping)
		{
			return ns != null && Send(ns, Instantiate(ns, ping));
		}

		public static PingAck Instantiate(NetState ns, byte ping)
		{
			return PacketCache<PingAck>.Acquire(ping, p => new PingAck(p));
		}

		private PingAck(byte ping)
			: base(0x73, 2)
		{
			m_Stream.Write(ping);
		}
	}

	public sealed class MovementRej : Packet
	{
		public MovementRej(int seq, Mobile m)
			: base(0x21, 8)
		{
			m_Stream.Write((byte)seq);
			m_Stream.Write((short)m.X);
			m_Stream.Write((short)m.Y);
			m_Stream.Write((byte)m.Direction);
			m_Stream.Write((sbyte)m.Z);
		}
	}

	public sealed class MovementAck : Packet
	{
		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static MovementAck Instantiate(NetState ns)
		{
			var noto = Math.Max(0, Math.Min(7, Notoriety.Compute(ns.Mobile, ns.Mobile)));

			var seq = ns.Sequence % 0x100;

			var hash = (noto << 16) | seq;

			return PacketCache<MovementAck>.Acquire(hash, () => new MovementAck(seq, noto));
		}

		private MovementAck(int seq, int noto)
			: base(0x22, 3)
		{
			m_Stream.Write((byte)seq);
			m_Stream.Write((byte)noto);
		}
	}

	public sealed class LoginConfirm : Packet
	{
		public LoginConfirm(Mobile m)
			: base(0x1B, 37)
		{
			m_Stream.Write(m.Serial);
			m_Stream.Write(0);
			m_Stream.Write((short)m.Body);
			m_Stream.Write((short)m.X);
			m_Stream.Write((short)m.Y);
			m_Stream.Write((short)m.Z);
			m_Stream.Write((byte)m.Direction);
			m_Stream.Write((byte)0);
			m_Stream.Write(-1);

			var map = m.Map;

			if (map == null || map == Map.Internal)
			{
				map = m.LogoutMap;
			}

			m_Stream.Write((short)0);
			m_Stream.Write((short)0);
			m_Stream.Write((short)(map == null ? 6144 : map.Width));
			m_Stream.Write((short)(map == null ? 4096 : map.Height));

			m_Stream.Fill();
		}
	}

	public sealed class LoginComplete : Packet
	{
		public static LoginComplete Instance => PacketCache<LoginComplete>.Global(() => new LoginComplete());

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static LoginComplete Instantiate(NetState ns)
		{
			return PacketCache<LoginComplete>.Global(() => new LoginComplete());
		}

		private LoginComplete()
			: base(0x55, 1)
		{ }
	}

	[PropertyObject]
	public sealed class CityInfo : IPoint3D
	{
		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public string City { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public string Building { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int Description { get; set; }

		private Point3D m_Location;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public Point3D Location { get => m_Location; set => m_Location = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int X { get => m_Location.X; set => m_Location.X = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int Y { get => m_Location.Y; set => m_Location.Y = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int Z { get => m_Location.Z; set => m_Location.Z = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public Map Map { get; set; }

		public CityInfo(string city, string building, int description, int x, int y, int z, Map map)
		{
			City = city;
			Building = building;
			Description = description;
			m_Location = new Point3D(x, y, z);
			Map = map;
		}

		public CityInfo(string city, string building, int x, int y, int z, Map map)
			: this(city, building, 0, x, y, z, map)
		{ }

		public CityInfo(string city, string building, int description, int x, int y, int z)
			: this(city, building, description, x, y, z, Map.Trammel)
		{ }

		public CityInfo(string city, string building, int x, int y, int z)
			: this(city, building, 0, x, y, z, Map.Trammel)
		{ }
	}

	public sealed class CharacterListUpdate : Packet
	{
		public CharacterListUpdate(IAccount a)
			: base(0x86)
		{
			EnsureCapacity(4 + (a.Length * 60));

			var highSlot = -1;

			for (var i = 0; i < a.Length; ++i)
			{
				if (a[i] != null)
				{
					highSlot = i;
				}
			}

			var count = Math.Max(Math.Max(highSlot + 1, a.Limit), 5);

			m_Stream.Write((byte)count);

			for (var i = 0; i < count; ++i)
			{
				var m = a[i];

				if (m != null)
				{
					m_Stream.WriteAsciiFixed(m.Name, 30);
					m_Stream.Fill(30); // password
				}
				else
				{
					m_Stream.Fill(60);
				}
			}
		}
	}

	[Flags]
	public enum ThirdPartyFeature : ulong
	{
		None = 0,

		FilterWeather = 1ul << 0,
		FilterLight = 1ul << 1,

		SmartTarget = 1ul << 2,
		RangedTarget = 1ul << 3,

		AutoOpenDoors = 1ul << 4,

		DequipOnCast = 1ul << 5,
		AutoPotionEquip = 1ul << 6,

		ProtectHeals = 1ul << 7,

		LoopedMacros = 1ul << 8,

		UseOnceAgent = 1ul << 9,
		RestockAgent = 1ul << 10,
		SellAgent = 1ul << 11,
		BuyAgent = 1ul << 12,

		PotionHotkeys = 1ul << 13,

		RandomTargets = 1ul << 14,
		ClosestTargets = 1ul << 15, // All closest target hotkeys
		OverheadHealth = 1ul << 16, // Health and Mana/Stam messages shown over player's heads

		AutolootAgent = 1ul << 17,
		BoneCutterAgent = 1ul << 18,
		AdvancedMacros = 1ul << 19,
		AutoRemount = 1ul << 20,
		AutoBandage = 1ul << 21,
		EnemyTargetShare = 1ul << 22,
		FilterSeason = 1ul << 23,
		SpellTargetShare = 1ul << 24,

		All = ~None
	}

	public static class FeatureProtection
	{
		public static ThirdPartyFeature DisabledFeatures { get; private set; } = 0;

		public static void Disable(ThirdPartyFeature feature)
		{
			SetDisabled(feature, true);
		}

		public static void Enable(ThirdPartyFeature feature)
		{
			SetDisabled(feature, false);
		}

		public static void SetDisabled(ThirdPartyFeature feature, bool value)
		{
			if (value)
			{
				DisabledFeatures |= feature;
			}
			else
			{
				DisabledFeatures &= ~feature;
			}
		}
	}

	public sealed class CharacterList : Packet
	{
		private static MD5CryptoServiceProvider m_MD5Provider;

		public static CharacterListFlags AdditionalFlags { get; set; }

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static CharacterList Instantiate(NetState ns)
		{
			if (ns.IsEnhancedClient)
			{
				return new CharacterList(ns.Account, ns.CityInfo, true, true);
			}

			if (ns.NewCharacterList)
			{
				return new CharacterList(ns.Account, ns.CityInfo, true, false);
			}

			return new CharacterList(ns.Account, ns.CityInfo, false, false);
		}

		private CharacterList(IAccount a, CityInfo[] info, bool extended, bool enhanced)
			: base(0xA9)
		{
			var size = 3;

			if (extended)
			{
				size += 8;
				size += a.Length * 60;
				size += info.Length * 89;
			}
			else
			{
				size += 6;
				size += a.Length * 60;
				size += info.Length * 63;
			}

			EnsureCapacity(size);

			var highSlot = -1;

			for (var i = 0; i < a.Length; ++i)
			{
				if (a[i] != null)
				{
					highSlot = i;
				}
			}

			var count = Math.Max(Math.Max(highSlot + 1, a.Limit), 5);

			m_Stream.Write((byte)count);

			for (var i = 0; i < count; ++i)
			{
				if (a[i] != null)
				{
					m_Stream.WriteAsciiFixed(a[i].Name, 30);
					m_Stream.Fill(30); // password
				}
				else
				{
					m_Stream.Fill(60);
				}
			}

			m_Stream.Write((byte)info.Length);

			var fixedLength = extended ? 32 : 31;

			for (var i = 0; i < info.Length; ++i)
			{
				var ci = info[i];

				m_Stream.Write((byte)i);
				m_Stream.WriteAsciiFixed(ci.City, fixedLength);
				m_Stream.WriteAsciiFixed(ci.Building, fixedLength);

				if (!extended)
				{
					continue;
				}

				m_Stream.Write(ci.X);
				m_Stream.Write(ci.Y);
				m_Stream.Write(ci.Z);
				m_Stream.Write(ci.Map.MapID);
				m_Stream.Write(ci.Description);
				m_Stream.Write(0);
			}

			var flags = ExpansionInfo.CoreExpansion.CharacterListFlags;

			if (count > 6)
			{
				flags |= CharacterListFlags.SeventhCharacterSlot | CharacterListFlags.SixthCharacterSlot;
			}
			else if (count == 6)
			{
				flags |= CharacterListFlags.SixthCharacterSlot;
			}
			else if (a.Limit == 1)
			{
				flags |= CharacterListFlags.SlotLimit | CharacterListFlags.OneCharacterSlot; // Limit Characters & One Character
			}

			if (enhanced)
			{
				flags |= CharacterListFlags.KR; // Suppport Enhanced Client / KR flag 1 and 2 (0x200 + 0x400)
			}

			flags |= AdditionalFlags;

			Console.WriteLine("{0}: {1} / {2} [{3}]", a.Username, a.Count, a.Limit, flags);

			m_Stream.Write((int)flags);

			if (extended)
			{
				m_Stream.Write((short)-1);
			}

			var disabled = FeatureProtection.DisabledFeatures;

			if (disabled != 0)
			{
				if (m_MD5Provider == null)
				{
					m_MD5Provider = new MD5CryptoServiceProvider();
				}

				m_Stream.UnderlyingStream.Flush();

				var hashCode = m_MD5Provider.ComputeHash(m_Stream.UnderlyingStream.GetBuffer(), 0, (int)m_Stream.UnderlyingStream.Length);

				var buffer = new byte[28];

				for (var i = 0; i < count; ++i)
				{
					Utility.RandomBytes(buffer);

					m_Stream.Seek(35 + (i * 60), SeekOrigin.Begin);
					m_Stream.Write(buffer, 0, buffer.Length);
				}

				m_Stream.Seek(35, SeekOrigin.Begin);
				m_Stream.Write((int)((ulong)disabled >> 32));
				m_Stream.Write((int)disabled);

				m_Stream.Seek(95, SeekOrigin.Begin);
				m_Stream.Write(hashCode, 0, hashCode.Length);
			}
		}
	}

	public sealed class ClearWeaponAbility : Packet
	{
		public static ClearWeaponAbility Instance => PacketCache<ClearWeaponAbility>.Global(() => new ClearWeaponAbility());

		public static bool Send(NetState ns)
		{
			return ns != null && Send(ns, Instantiate(ns));
		}

		public static ClearWeaponAbility Instantiate(NetState ns)
		{
			return PacketCache<ClearWeaponAbility>.Global(() => new ClearWeaponAbility());
		}

		private ClearWeaponAbility()
			: base(0xBF)
		{
			EnsureCapacity(5);

			m_Stream.Write((short)0x21);
		}
	}

	public enum ALRReason : byte
	{
		Invalid = 0x00,
		InUse = 0x01,
		Blocked = 0x02,
		BadPass = 0x03,
		Idle = 0xFE,
		BadComm = 0xFF
	}

	public sealed class AccountLoginRej : Packet
	{
		public static AccountLoginRej Invalid => PacketCache<AccountLoginRej>.Acquire(ALRReason.Invalid, m => new AccountLoginRej(m));
		public static AccountLoginRej InUse => PacketCache<AccountLoginRej>.Acquire(ALRReason.InUse, m => new AccountLoginRej(m));
		public static AccountLoginRej Blocked => PacketCache<AccountLoginRej>.Acquire(ALRReason.Blocked, m => new AccountLoginRej(m));
		public static AccountLoginRej BadPass => PacketCache<AccountLoginRej>.Acquire(ALRReason.BadPass, m => new AccountLoginRej(m));
		public static AccountLoginRej Idle => PacketCache<AccountLoginRej>.Acquire(ALRReason.Idle, m => new AccountLoginRej(m));
		public static AccountLoginRej BadComm => PacketCache<AccountLoginRej>.Acquire(ALRReason.BadComm, m => new AccountLoginRej(m));

		public static bool Send(NetState ns, ALRReason reason)
		{
			return ns != null && Send(ns, Instantiate(ns, reason));
		}

		public static AccountLoginRej Instantiate(NetState ns, ALRReason reason)
		{
			return PacketCache<AccountLoginRej>.Acquire(reason, r => new AccountLoginRej(r));
		}

		private AccountLoginRej(ALRReason reason)
			: base(0x82, 2)
		{
			m_Stream.Write((byte)reason);
		}
	}

	public enum AffixType : byte
	{
		Append = 0x00,
		Prepend = 0x01,
		System = 0x02
	}

	public sealed class MessageLocalizedAffix : Packet
	{
		public MessageLocalizedAffix(Serial serial, int graphic, MessageType messageType, int hue, int font, int number, string name, AffixType affixType, string affix, string args)
			: this(null, serial, graphic, messageType, hue, font, number, name, affixType, affix, args)
		{
		}

		public MessageLocalizedAffix(NetState state, Serial serial, int graphic, MessageType messageType, int hue, int font, int number, string name, AffixType affixType, string affix, string args)
			: base(0xCC)
		{
			if (name == null)
			{
				name = "";
			}

			if (affix == null)
			{
				affix = "";
			}

			if (args == null)
			{
				args = "";
			}

			if (hue == 0)
			{
				hue = 0x3B2;
			}

			EnsureCapacity(52 + affix.Length + (args.Length * 2));

			m_Stream.Write(serial);
			m_Stream.Write((short)graphic);
			m_Stream.Write((byte)messageType);
			m_Stream.Write((short)hue);
			m_Stream.Write((short)font);
			m_Stream.Write(number);
			m_Stream.Write((byte)affixType);
			m_Stream.WriteAsciiFixed(name, 30);
			m_Stream.WriteAsciiNull(affix);

			if (state != null && state.IsEnhancedClient)
			{
				m_Stream.WriteLittleUniNull(args);
			}
			else
			{
				m_Stream.WriteBigUniNull(args);
			}
		}
	}

	public sealed class ServerInfo
	{
		public string Name { get; set; }

		public int FullPercent { get; set; }

		public int TimeZone { get; set; }

		public IPEndPoint Address { get; set; }

		public ServerInfo(string name, int fullPercent, int tzOffset, IPEndPoint address)
		{
			Name = name;
			FullPercent = fullPercent;
			TimeZone = tzOffset;
			Address = address;
		}
	}

	public sealed class FollowMessage : Packet
	{
		public FollowMessage(Serial serial1, Serial serial2)
			: base(0x15, 9)
		{
			m_Stream.Write(serial1);
			m_Stream.Write(serial2);
		}
	}

	public sealed class AccountLoginAck : Packet
	{
		public AccountLoginAck(ServerInfo[] info)
			: base(0xA8)
		{
			EnsureCapacity(6 + (info.Length * 40));

			m_Stream.Write((byte)0x5D); // Unknown
			m_Stream.Write((ushort)info.Length);

			for (var i = 0; i < info.Length; ++i)
			{
				var si = info[i];

				m_Stream.Write((ushort)i);
				m_Stream.WriteAsciiFixed(si.Name, 32);
				m_Stream.Write((byte)si.FullPercent);
				m_Stream.Write((sbyte)si.TimeZone);
				m_Stream.Write(Utility.GetAddressValue(si.Address.Address));
			}
		}
	}

	public sealed class DisplaySignGump : Packet
	{
		public DisplaySignGump(Serial serial, int gumpID, string unknown, string caption)
			: base(0x8B)
		{
			if (unknown == null)
			{
				unknown = "";
			}

			if (caption == null)
			{
				caption = "";
			}

			EnsureCapacity(16 + unknown.Length + caption.Length);

			m_Stream.Write(serial);
			m_Stream.Write((short)gumpID);
			m_Stream.Write((short)unknown.Length);
			m_Stream.WriteAsciiFixed(unknown, unknown.Length);
			m_Stream.Write((short)(caption.Length + 1));
			m_Stream.WriteAsciiFixed(caption, caption.Length + 1);
		}
	}

	public sealed class GodModeReply : Packet
	{
		public GodModeReply(bool reply)
			: base(0x2B, 2)
		{
			m_Stream.Write(reply);
		}
	}

	public sealed class PlayServerAck : Packet
	{
		public PlayServerAck(ServerInfo si, uint auth)
			: base(0x8C, 11)
		{
			var addr = Utility.GetAddressValue(si.Address.Address);

			m_Stream.Write((byte)addr);
			m_Stream.Write((byte)(addr >> 8));
			m_Stream.Write((byte)(addr >> 16));
			m_Stream.Write((byte)(addr >> 24));

			m_Stream.Write((short)si.Address.Port);
			m_Stream.Write(auth);
		}
	}

	public static class PacketCache<P> where P : Packet
	{
		private static readonly ConcurrentDictionary<object, P> m_Instances = new ConcurrentDictionary<object, P>();

		private static volatile P m_Instance;

		public static P Global(Func<P> ctor)
		{
			if ((m_Instance?.State & PacketState.Static) != PacketState.Static)
			{
				return m_Instance = Packet.SetStatic(ctor());
			}

			return m_Instance;
		}

		public static P Acquire<K>(K key, Func<P> ctor)
		{
			if (!m_Instances.TryGetValue(key, out var p) || (p?.State & PacketState.Static) != PacketState.Static)
			{
				m_Instances[key] = p = Packet.SetStatic(ctor());
			}

			return p;
		}

		public static P Acquire<K>(K key, Func<K, P> ctor)
		{
			if (!m_Instances.TryGetValue(key, out var p) || (p?.State & PacketState.Static) != PacketState.Static)
			{
				m_Instances[key] = p = Packet.SetStatic(ctor(key));
			}

			return p;
		}

		public static void Free()
		{
			Packet.Release(Interlocked.Exchange(ref m_Instance, null));
		}

		public static void Free<K>(K key)
		{
			if (m_Instances.TryRemove(key, out var p))
			{
				Packet.Release(ref p);
			}
		}

		public static void Clear()
		{
			Free();

			if (m_Instances.Count > 0)
			{
				Parallel.ForEach(m_Instances.Values, Packet.Release);

				m_Instances.Clear();
			}
		}
	}

	[Flags]
	public enum PacketState
	{
		Inactive = 0x00,
		Static = 0x01,
		Acquired = 0x02,
		Accessed = 0x04,
		Buffered = 0x08,
		Warned = 0x10
	}

	public abstract class Packet
	{
		private const int CompressorBufferSize = 0x10000;
		private const int BufferSize = 4096;

		private static readonly BufferPool m_CompressorBuffers = new BufferPool("Compressor", 4, CompressorBufferSize);
		private static readonly BufferPool m_Buffers = new BufferPool("Compressed", 16, BufferSize);

		public static bool Send(NetState ns, Packet p)
		{
			if (ns != null && p != null && ns.Socket != null && !ns.IsDisposing)
			{
				ns.Send(p);
				return true;
			}

			return false;
		}

		public static T SetStatic<T>(T p) where T : Packet
		{
			p?.SetStatic();

			return p;
		}

		public static T Acquire<T>(T p) where T : Packet
		{
			p?.Acquire();

			return p;
		}

		public static void Release<T>(T p) where T : Packet
		{
			p?.Release();
		}

		public static void Release<T>(ref T p) where T : Packet
		{
			p?.Release();

			p = null;
		}

		private readonly int m_PacketID;
		private readonly int m_Length;

		private volatile byte[] m_CompiledBuffer;
		private volatile int m_CompiledLength;

		private volatile PacketState m_State;

		protected PacketWriter m_Stream;

		public int PacketID => m_PacketID;

		public PacketState State => m_State;
		public PacketWriter Stream => m_Stream;

		protected Packet(int packetID)
		{
			m_PacketID = packetID;

			if (Core.Profiling)
			{
				var prof = PacketSendProfile.Acquire(GetType());
				prof.Increment();
			}
		}

		protected Packet(int packetID, int length)
			: this(packetID, length, PacketWriter.CreateInstance(length))
		{ }

		protected Packet(int packetID, int length, PacketWriter stream)
		{
			m_PacketID = packetID;
			m_Length = length;

			m_Stream = stream;
			m_Stream.Write((byte)packetID);

			if (Core.Profiling)
			{
				var prof = PacketSendProfile.Acquire(GetType());

				prof.Increment();
			}
		}

		public void EnsureCapacity(int length)
		{
			m_Stream = PacketWriter.CreateInstance(length);

			m_Stream.Write((byte)m_PacketID);
			m_Stream.Write((short)0);
		}

		public void SetStatic()
		{
			m_State |= PacketState.Static | PacketState.Acquired;
		}

		public void Acquire()
		{
			m_State |= PacketState.Acquired;
		}

		public void OnSend()
		{
			Core.Set();

			if ((m_State & (PacketState.Acquired | PacketState.Static)) == 0)
			{
				Free();
			}
		}

		protected virtual void Free()
		{
			var buffer = Interlocked.Exchange(ref m_CompiledBuffer, null);

			if (buffer == null)
			{
				return;
			}

			if ((m_State & PacketState.Buffered) != 0)
			{
				m_Buffers.ReleaseBuffer(ref buffer);
			}

			m_State &= ~(PacketState.Static | PacketState.Acquired | PacketState.Buffered);
		}

		public void Release()
		{
			if ((m_State & PacketState.Acquired) != 0)
			{
				Free();
			}
		}

		public byte[] Compile(bool compress, out int length)
		{
			lock (this)
			{
				if (m_CompiledBuffer == null)
				{
					if ((m_State & PacketState.Accessed) == 0)
					{
						m_State |= PacketState.Accessed;
					}
					else
					{
						if ((m_State & PacketState.Warned) == 0)
						{
							m_State |= PacketState.Warned;

							try
							{
								var trace = new StackTrace();
								var notice = $"Redundant compile for packet 0x{m_PacketID:X2} ('{GetType().Name}'), use Acquire() and Release()";

								Console.WriteLine($"Warning: {notice}");

								File.AppendAllText("packet_errors.log", $"{DateTime.UtcNow}{Environment.NewLine}{notice}{Environment.NewLine}{trace}{Environment.NewLine}{Environment.NewLine}");
							}
							catch (Exception e)
							{
								ExceptionLogging.LogException(e);
							}
						}

						m_CompiledBuffer = new byte[0];

						length = m_CompiledLength = 0;

						return m_CompiledBuffer;
					}

					InternalCompile(compress);
				}

				length = m_CompiledLength;

				return m_CompiledBuffer;
			}
		}

		private void InternalCompile(bool compress)
		{
			if (m_Length == 0)
			{
				var streamLen = m_Stream.Length;

				m_Stream.Seek(1, SeekOrigin.Begin);
				m_Stream.Write((ushort)streamLen);
			}
			else if (m_Stream.Length != m_Length)
			{
				var diff = (int)m_Stream.Length - m_Length;

				Console.WriteLine($"Packet: 0x{m_PacketID:X2}: Bad packet length! ({(diff >= 0 ? "+" : "")}{diff} bytes)");
			}

			var ms = m_Stream.UnderlyingStream;

			m_CompiledBuffer = ms.GetBuffer();

			var length = (int)ms.Length;

			if (compress)
			{
				var buffer = m_CompressorBuffers.AcquireBuffer();

				Compression.Compress(m_CompiledBuffer, 0, length, buffer, ref length);

				if (length <= 0)
				{
					Console.WriteLine($"Warning: Compression buffer overflowed on packet 0x{m_PacketID:X2} ('{GetType().Name}') (length={length})");

					using (var op = new StreamWriter("compression_overflow.log", true))
					{
						op.WriteLine($"{DateTime.UtcNow} Warning: Compression buffer overflowed on packet 0x{m_PacketID:X2} ('{GetType().Name}') (length={length})");
						op.WriteLine(new StackTrace());
					}
				}
				else
				{
					m_CompiledLength = length;

					if (length > BufferSize || (m_State & PacketState.Static) != 0)
					{
						m_CompiledBuffer = new byte[length];
					}
					else
					{
						lock (m_Buffers)
						{
							m_CompiledBuffer = m_Buffers.AcquireBuffer();
						}

						m_State |= PacketState.Buffered;
					}

					Buffer.BlockCopy(buffer, 0, m_CompiledBuffer, 0, length);

					m_CompressorBuffers.ReleaseBuffer(ref buffer);
				}
			}
			else if (length > 0)
			{
				var old = m_CompiledBuffer;

				m_CompiledLength = length;

				if (length > BufferSize || (m_State & PacketState.Static) != 0)
				{
					m_CompiledBuffer = new byte[length];
				}
				else
				{
					m_CompiledBuffer = m_Buffers.AcquireBuffer();

					m_State |= PacketState.Buffered;
				}

				Buffer.BlockCopy(old, 0, m_CompiledBuffer, 0, length);
			}

			PacketWriter.ReleaseInstance(ref m_Stream);
		}
	}
}
