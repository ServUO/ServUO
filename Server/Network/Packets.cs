#region Header
// **********
// ServUO - Packets.cs
// **********
#endregion

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

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

	/*public enum CMEFlags
	{
	None = 0x00,
	Locked = 0x01,
	Arrow = 0x02,
	x0004 = 0x04,
	Color = 0x20,
	x0040 = 0x40,
	x0080 = 0x80
	}*/

	public sealed class DamagePacketOld : Packet
	{
		public DamagePacketOld(Mobile m, int amount)
			: base(0xBF)
		{
			EnsureCapacity(11);

			m_Stream.Write((short)0x22);
			m_Stream.Write((byte)1);
			m_Stream.Write(m.Serial);

			if (amount > 255)
			{
				amount = 255;
			}
			else if (amount < 0)
			{
				amount = 0;
			}

			m_Stream.Write((byte)amount);
		}
	}

	public sealed class DamagePacket : Packet
	{
		public DamagePacket(IDamageable damageable, int amount)
			: base(0x0B, 7)
		{
            m_Stream.Write(damageable.Serial);

			if (amount > 0xFFFF)
			{
				amount = 0xFFFF;
			}
			else if (amount < 0)
			{
				amount = 0;
			}

			m_Stream.Write((ushort)amount);
		}

		/*public DamagePacket( Mobile m, int amount ) : base( 0xBF )
		{
		EnsureCapacity( 11 );

		m_Stream.Write( (short) 0x22 );
		m_Stream.Write( (byte) 1 );
		m_Stream.Write( (int) m.Serial );

		if ( amount > 255 )
		amount = 255;
		else if ( amount < 0 )
		amount = 0;

		m_Stream.Write( (byte)amount );
		}*/
	}

	public sealed class CancelArrow : Packet
	{
		public CancelArrow()
			: base(0xBA, 6)
		{
			m_Stream.Write((byte)0);
			m_Stream.Write((short)-1);
			m_Stream.Write((short)-1);
		}
	}

	public sealed class SetArrow : Packet
	{
		public SetArrow(int x, int y)
			: base(0xBA, 6)
		{
			m_Stream.Write((byte)1);
			m_Stream.Write((short)x);
			m_Stream.Write((short)y);
		}
	}

	public sealed class CancelArrowHS : Packet
	{
		public CancelArrowHS(int x, int y, Serial s)
			: base(0xBA, 10)
		{
			m_Stream.Write((byte)0);
			m_Stream.Write((short)x);
			m_Stream.Write((short)y);
			m_Stream.Write(s);
		}
	}

	public sealed class SetArrowHS : Packet
	{
		public SetArrowHS(int x, int y, Serial s)
			: base(0xBA, 10)
		{
			m_Stream.Write((byte)1);
			m_Stream.Write((short)x);
			m_Stream.Write((short)y);
			m_Stream.Write(s);
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
			EnsureCapacity(8);

			m_Stream.Write((byte)1); // Close
			m_Stream.Write(cont.Serial);
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
		}
	}

	public sealed class SecureTradeEquip : Packet
	{
		public SecureTradeEquip(Item item, Mobile m)
			: base(0x25, 20)
		{
			m_Stream.Write(item.Serial);
			m_Stream.Write((short)item.ItemID);
			m_Stream.Write((byte)0);
			m_Stream.Write((short)item.Amount);
			m_Stream.Write((short)item.X);
			m_Stream.Write((short)item.Y);
			m_Stream.Write(m.Serial);
			m_Stream.Write((short)item.Hue);
		}
	}

	public sealed class SecureTradeEquip6017 : Packet
	{
		public SecureTradeEquip6017(Item item, Mobile m)
			: base(0x25, 21)
		{
			m_Stream.Write(item.Serial);
			m_Stream.Write((short)item.ItemID);
			m_Stream.Write((byte)0);
			m_Stream.Write((short)item.Amount);
			m_Stream.Write((short)item.X);
			m_Stream.Write((short)item.Y);
			m_Stream.Write((byte)0); // Grid Location?
			m_Stream.Write(m.Serial);
			m_Stream.Write((short)item.Hue);
		}
	}

	public sealed class MapPatches : Packet
	{
		public MapPatches()
			: base(0xBF)
		{
			EnsureCapacity(9 + (3 * 8));

			m_Stream.Write((short)0x0018);

			m_Stream.Write(4);

			m_Stream.Write(Map.Felucca.Tiles.Patch.StaticBlocks);
			m_Stream.Write(Map.Felucca.Tiles.Patch.LandBlocks);

			m_Stream.Write(Map.Trammel.Tiles.Patch.StaticBlocks);
			m_Stream.Write(Map.Trammel.Tiles.Patch.LandBlocks);

			m_Stream.Write(Map.Ilshenar.Tiles.Patch.StaticBlocks);
			m_Stream.Write(Map.Ilshenar.Tiles.Patch.LandBlocks);

			m_Stream.Write(Map.Malas.Tiles.Patch.StaticBlocks);
			m_Stream.Write(Map.Malas.Tiles.Patch.LandBlocks);

			//TODO: Should this include newer facets?
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
		public VendorBuyContent(List<BuyItemState> list)
			: base(0x3c)
		{
			EnsureCapacity(list.Count * 19 + 5);

			m_Stream.Write((short)list.Count);

			//The client sorts these by their X/Y value.
			//OSI sends these in wierd order.  X/Y highest to lowest and serial loest to highest
			//These are already sorted by serial (done by the vendor class) but we have to send them by x/y
			//(the x74 packet is sent in 'correct' order.)
			for (int i = list.Count - 1; i >= 0; --i)
			{
				BuyItemState bis = list[i];

				m_Stream.Write(bis.MySerial);
				m_Stream.Write((ushort)bis.ItemID);
				m_Stream.Write((byte)0); //itemid offset
				m_Stream.Write((ushort)bis.Amount);
				m_Stream.Write((short)(i + 1)); //x
				m_Stream.Write((short)1); //y
				m_Stream.Write(bis.ContainerSerial);
				m_Stream.Write((ushort)bis.Hue);
			}
		}
	}

	public sealed class VendorBuyContent6017 : Packet
	{
		public VendorBuyContent6017(List<BuyItemState> list)
			: base(0x3c)
		{
			EnsureCapacity(list.Count * 20 + 5);

			m_Stream.Write((short)list.Count);

			//The client sorts these by their X/Y value.
			//OSI sends these in wierd order.  X/Y highest to lowest and serial loest to highest
			//These are already sorted by serial (done by the vendor class) but we have to send them by x/y
			//(the x74 packet is sent in 'correct' order.)
			for (int i = list.Count - 1; i >= 0; --i)
			{
				BuyItemState bis = list[i];

				m_Stream.Write(bis.MySerial);
				m_Stream.Write((ushort)bis.ItemID);
				m_Stream.Write((byte)0); //itemid offset
				m_Stream.Write((ushort)bis.Amount);
				m_Stream.Write((short)(i + 1)); //x
				m_Stream.Write((short)1); //y
				m_Stream.Write((byte)0); // Grid Location?
				m_Stream.Write(bis.ContainerSerial);
				m_Stream.Write((ushort)bis.Hue);
			}
		}
	}

	public sealed class DisplayBuyList : Packet
	{
		public DisplayBuyList(Mobile vendor)
			: base(0x24, 7)
		{
			m_Stream.Write(vendor.Serial);
			m_Stream.Write((short)0x30); // buy window id?
		}
	}

	public sealed class DisplayBuyListHS : Packet
	{
		public DisplayBuyListHS(Mobile vendor)
			: base(0x24, 9)
		{
			m_Stream.Write(vendor.Serial);
			m_Stream.Write((short)0x30); // buy window id?
			m_Stream.Write((short)0x00);
		}
	}

	public sealed class VendorBuyList : Packet
	{
		public VendorBuyList(Mobile vendor, List<BuyItemState> list)
			: base(0x74)
		{
			EnsureCapacity(256);

			Container BuyPack = vendor.FindItemOnLayer(Layer.ShopBuy) as Container;
			m_Stream.Write((BuyPack == null ? Serial.MinusOne : BuyPack.Serial));

			m_Stream.Write((byte)list.Count);

			for (int i = 0; i < list.Count; ++i)
			{
				BuyItemState bis = list[i];

				m_Stream.Write(bis.Price);

				string desc = bis.Description;

				if (desc == null)
				{
					desc = "";
				}

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

			foreach (SellItemState state in sis)
			{
				m_Stream.Write(state.Item.Serial);
				m_Stream.Write((ushort)state.Item.ItemID);
				m_Stream.Write((ushort)state.Item.Hue);
				m_Stream.Write((ushort)state.Item.Amount);
				m_Stream.Write((ushort)state.Price);

				string name = state.Item.Name;

				if (name == null || (name = name.Trim()).Length <= 0)
				{
					name = state.Name;
				}

				if (name == null)
				{
					name = "";
				}

				m_Stream.Write((ushort)(name.Length));
				m_Stream.WriteAsciiFixed(name, (ushort)(name.Length));
			}
		}
	}

	public sealed class EndVendorSell : Packet
	{
		public EndVendorSell(Mobile Vendor)
			: base(0x3B, 8)
		{
			m_Stream.Write((ushort)8); //length
			m_Stream.Write(Vendor.Serial);
			m_Stream.Write((byte)0);
		}
	}

	public sealed class EndVendorBuy : Packet
	{
		public EndVendorBuy(Mobile Vendor)
			: base(0x3B, 8)
		{
			m_Stream.Write((ushort)8); //length
			m_Stream.Write(Vendor.Serial);
			m_Stream.Write((byte)0);
		}
	}

	public sealed class DeathAnimation : Packet
	{
		public DeathAnimation(Mobile killed, Item corpse)
			: base(0xAF, 13)
		{
			m_Stream.Write(killed.Serial);
			m_Stream.Write((corpse == null ? Serial.Zero : corpse.Serial));
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
			m_Stream.Write((byte)2);
			m_Stream.Write(m.Serial);
			m_Stream.Write((byte)0);

			int lockBits = 0;

			lockBits |= (int)m.StrLock << 4;
			lockBits |= (int)m.DexLock << 2;
			lockBits |= (int)m.IntLock;

			m_Stream.Write((byte)lockBits);
		}
	}

	public class EquipInfoAttribute
	{
		private readonly int m_Number;
		private readonly int m_Charges;

		public int Number { get { return m_Number; } }

		public int Charges { get { return m_Charges; } }

		public EquipInfoAttribute(int number)
			: this(number, -1)
		{ }

		public EquipInfoAttribute(int number, int charges)
		{
			m_Number = number;
			m_Charges = charges;
		}
	}

	public class EquipmentInfo
	{
		private readonly int m_Number;
		private readonly Mobile m_Crafter;
		private readonly bool m_Unidentified;
		private readonly EquipInfoAttribute[] m_Attributes;

		public int Number { get { return m_Number; } }

		public Mobile Crafter { get { return m_Crafter; } }

		public bool Unidentified { get { return m_Unidentified; } }

		public EquipInfoAttribute[] Attributes { get { return m_Attributes; } }

		public EquipmentInfo(int number, Mobile crafter, bool unidentified, EquipInfoAttribute[] attributes)
		{
			m_Number = number;
			m_Crafter = crafter;
			m_Unidentified = unidentified;
			m_Attributes = attributes;
		}
	}

	public sealed class DisplayEquipmentInfo : Packet
	{
		public DisplayEquipmentInfo(Item item, EquipmentInfo info)
			: base(0xBF)
		{
			var attrs = info.Attributes;

			EnsureCapacity(
				17 + (info.Crafter == null ? 0 : 6 + info.Crafter.TitleName == null ? 0 : info.Crafter.TitleName.Length) +
				(info.Unidentified ? 4 : 0) + (attrs.Length * 6));

			m_Stream.Write((short)0x10);
			m_Stream.Write(item.Serial);

			m_Stream.Write(info.Number);

			if (info.Crafter != null)
			{
				string name = info.Crafter.TitleName;

				m_Stream.Write(-3);

				if (name == null)
				{
					m_Stream.Write((ushort)0);
				}
				else
				{
					int length = name.Length;
					m_Stream.Write((ushort)length);
					m_Stream.WriteAsciiFixed(name, length);
				}
			}

			if (info.Unidentified)
			{
				m_Stream.Write(-4);
			}

			for (int i = 0; i < attrs.Length; ++i)
			{
				m_Stream.Write(attrs[i].Number);
				m_Stream.Write((short)attrs[i].Charges);
			}

			m_Stream.Write(-1);
		}
	}

	public sealed class ChangeUpdateRange : Packet
	{
		private static readonly ChangeUpdateRange[] m_Cache = new ChangeUpdateRange[0x100];

		public static ChangeUpdateRange Instantiate(int range)
		{
			byte idx = (byte)range;
			ChangeUpdateRange p = m_Cache[idx];

			if (p == null)
			{
				m_Cache[idx] = p = new ChangeUpdateRange(range);
				p.SetStatic();
			}

			return p;
		}

		public ChangeUpdateRange(int range)
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
			m_Stream.Write(combatant != null ? combatant.Serial : Serial.Zero);
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
		public TripTimeResponse(int unk)
			: base(0xC9, 6)
		{
			m_Stream.Write((byte)unk);
			m_Stream.Write(Environment.TickCount);
		}
	}

	public sealed class UTripTimeResponse : Packet
	{
		public UTripTimeResponse(int unk)
			: base(0xCA, 6)
		{
			m_Stream.Write((byte)unk);
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
			m_Stream.Write(0);
			m_Stream.Write(0);
			m_Stream.Write((short)0);
		}
	}

	public sealed class ChangeCharacter : Packet
	{
		public ChangeCharacter(IAccount a)
			: base(0x81)
		{
			EnsureCapacity(305);

			int count = 0;

			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != null)
				{
					++count;
				}
			}

			m_Stream.Write((byte)count);
			m_Stream.Write((byte)0);

			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != null)
				{
					string name = a[i].Name;

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
		public static readonly Packet Dead = SetStatic(new DeathStatus(true));
		public static readonly Packet Alive = SetStatic(new DeathStatus(false));

		public static Packet Instantiate(bool dead)
		{
			return (dead ? Dead : Alive);
		}

		public DeathStatus(bool dead)
			: base(0x2C, 2)
		{
			m_Stream.Write((byte)(dead ? 0 : 2));
		}
	}

	public sealed class SpeedControl : Packet
	{
		public static readonly Packet WalkSpeed = SetStatic(new SpeedControl(2));
		public static readonly Packet MountSpeed = SetStatic(new SpeedControl(1));
		public static readonly Packet Disable = SetStatic(new SpeedControl(0));

		public SpeedControl(int speedControl)
			: base(0xBF)
		{
			EnsureCapacity(3);

			m_Stream.Write((short)0x26);
			m_Stream.Write((byte)speedControl);
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

			string question = menu.Question;

			if (question == null)
			{
				m_Stream.Write((byte)0);
			}
			else
			{
				int questionLength = question.Length;
				m_Stream.Write((byte)questionLength);
				m_Stream.WriteAsciiFixed(question, questionLength);
			}

			var entries = menu.Entries;

			int entriesLength = (byte)entries.Length;

			m_Stream.Write((byte)entriesLength);

			for (int i = 0; i < entriesLength; ++i)
			{
				ItemListEntry e = entries[i];

				m_Stream.Write((ushort)e.ItemID);
				m_Stream.Write((short)e.Hue);

				string name = e.Name;

				if (name == null)
				{
					m_Stream.Write((byte)0);
				}
				else
				{
					int nameLength = name.Length;
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
			m_Stream.Write((short)0);

			string question = menu.Question;

			if (question == null)
			{
				m_Stream.Write((byte)0);
			}
			else
			{
				int questionLength = question.Length;
				m_Stream.Write((byte)questionLength);
				m_Stream.WriteAsciiFixed(question, questionLength);
			}

			var answers = menu.Answers;

			int answersLength = (byte)answers.Length;

			m_Stream.Write((byte)answersLength);

			for (int i = 0; i < answersLength; ++i)
			{
				m_Stream.Write(0);

				string answer = answers[i];

				if (answer == null)
				{
					m_Stream.Write((byte)0);
				}
				else
				{
					int answerLength = answer.Length;
					m_Stream.Write((byte)answerLength);
					m_Stream.WriteAsciiFixed(answer, answerLength);
				}
			}
		}
	}

	public sealed class GlobalLightLevel : Packet
	{
		private static readonly GlobalLightLevel[] m_Cache = new GlobalLightLevel[0x100];

		public static GlobalLightLevel Instantiate(int level)
		{
			byte lvl = (byte)level;
			GlobalLightLevel p = m_Cache[lvl];

			if (p == null)
			{
				m_Cache[lvl] = p = new GlobalLightLevel(level);
				p.SetStatic();
			}

			return p;
		}

		public GlobalLightLevel(int level)
			: base(0x4F, 2)
		{
			m_Stream.Write((sbyte)level);
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
			m_Stream.Write((sbyte)level);
		}
	}

	public sealed class PersonalLightLevelZero : Packet
	{
		public PersonalLightLevelZero(Mobile m)
			: base(0x4E, 6)
		{
			m_Stream.Write(m.Serial);
			m_Stream.Write((sbyte)0);
		}
	}

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
		public DisplayContextMenu(ContextMenu menu)
			: base(0xBF)
		{
			var entries = menu.Entries;

			int length = (byte)entries.Length;

			EnsureCapacity(12 + (length * 8));

			m_Stream.Write((short)0x14);
			m_Stream.Write((short)0x02);

			IEntity target = menu.Target as IEntity;

			m_Stream.Write((target == null ? Serial.MinusOne : target.Serial));

			m_Stream.Write((byte)length);

			Point3D p;

			if (target is Mobile)
			{
				p = target.Location;
			}
			else if (target is Item)
			{
				p = ((Item)target).GetWorldLocation();
			}
			else
			{
				p = Point3D.Zero;
			}

			for (int i = 0; i < length; ++i)
			{
				ContextMenuEntry e = entries[i];

				m_Stream.Write(e.Number);
				m_Stream.Write((short)i);

				int range = e.Range;

				if (range == -1)
				{
					range = 18;
				}

				CMEFlags flags = (e.Enabled && menu.From.InRange(p, range)) ? CMEFlags.None : CMEFlags.Disabled;

				flags |= e.Flags;

				m_Stream.Write((short)flags);
			}
		}
	}

	public sealed class DisplayContextMenuOld : Packet
	{
		public DisplayContextMenuOld(ContextMenu menu)
			: base(0xBF)
		{
			var entries = menu.Entries;

			int length = (byte)entries.Length;

			EnsureCapacity(12 + (length * 8));

			m_Stream.Write((short)0x14);
			m_Stream.Write((short)0x01);

			IEntity target = menu.Target as IEntity;

			m_Stream.Write((target == null ? Serial.MinusOne : target.Serial));

			m_Stream.Write((byte)length);

			Point3D p;

			if (target is Mobile)
			{
				p = target.Location;
			}
			else if (target is Item)
			{
				p = ((Item)target).GetWorldLocation();
			}
			else
			{
				p = Point3D.Zero;
			}

			for (int i = 0; i < length; ++i)
			{
				ContextMenuEntry e = entries[i];

				m_Stream.Write((short)i);
				m_Stream.Write((ushort)(e.Number - 3000000));

				int range = e.Range;

				if (range == -1)
				{
					range = 18;
				}

				CMEFlags flags = (e.Enabled && menu.From.InRange(p, range)) ? CMEFlags.None : CMEFlags.Disabled;

				int color = e.Color & 0xFFFF;

				if (color != 0xFFFF)
				{
					flags |= CMEFlags.Colored;
				}

				flags |= e.Flags;

				m_Stream.Write((short)flags);

				if ((flags & CMEFlags.Colored) != 0)
				{
					m_Stream.Write((short)color);
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

			m_Stream.Write((realSerial ? m.Serial : Serial.Zero));
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
			Serial parentSerial;

			if (item.Parent is Mobile)
			{
				parentSerial = ((Mobile)item.Parent).Serial;
			}
			else
			{
				Console.WriteLine("Warning: EquipUpdate on item with !(parent is Mobile)");
				parentSerial = Serial.Zero;
			}

			int hue = item.Hue;

			if (item.Parent is Mobile)
			{
				Mobile mob = (Mobile)item.Parent;

				if (mob.SolidHueOverride >= 0)
				{
					hue = mob.SolidHueOverride;
				}
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
		public WorldItem(Item item)
			: base(0x1A)
		{
			EnsureCapacity(20);

			// 14 base length
			// +2 - Amount
			// +2 - Hue
			// +1 - Flags

			uint serial = (uint)item.Serial.Value;
			int itemID = item.ItemID & 0x3FFF;
			int amount = item.Amount;
			Point3D loc = item.Location;
			int x = loc.m_X;
			int y = loc.m_Y;
			int hue = item.Hue;
			int flags = item.GetPacketFlags();
			int direction = (int)item.Direction;

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

			if (direction != 0)
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

			if (direction != 0)
			{
				m_Stream.Write((byte)direction);
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

	public sealed class WorldItemSA : Packet
	{
		public WorldItemSA(Item item)
			: base(0xF3, 24)
		{
			m_Stream.Write((short)0x1);

			int itemID = item.ItemID;

			if (item is BaseMulti)
			{
				m_Stream.Write((byte)0x02);

				m_Stream.Write(item.Serial);

				itemID &= 0x3FFF;

				m_Stream.Write((short)itemID);

				m_Stream.Write((byte)0);
				/*} else if (  ) {
			m_Stream.Write( (byte) 0x01 );
			m_Stream.Write( (int) item.Serial );
			m_Stream.Write( (short) itemID ); 
			m_Stream.Write( (byte) item.Direction );*/
			}
			else
			{
				m_Stream.Write((byte)0x00);

				m_Stream.Write(item.Serial);

				itemID &= 0x7FFF;

				m_Stream.Write((short)itemID);

				m_Stream.Write((byte)0);
			}

			int amount = item.Amount;
			m_Stream.Write((short)amount);
			m_Stream.Write((short)amount);

			Point3D loc = item.Location;
			int x = loc.m_X & 0x7FFF;
			int y = loc.m_Y & 0x3FFF;
			m_Stream.Write((short)x);
			m_Stream.Write((short)y);
			m_Stream.Write((sbyte)loc.m_Z);

			m_Stream.Write((byte)item.Light);
			m_Stream.Write((short)item.Hue);
			m_Stream.Write((byte)item.GetPacketFlags());
		}
	}

	public sealed class WorldItemHS : Packet
	{
		public WorldItemHS(Item item)
			: base(0xF3, 26)
		{
			m_Stream.Write((short)0x1);

			int itemID = item.ItemID;

            if(item is BaseMulti)
                m_Stream.Write((byte)0x02);
            else if (item is IDamageable)
                m_Stream.Write((byte)0x03);
            else
                m_Stream.Write((byte)0x00);

			if (item is BaseMulti)
			{
				m_Stream.Write(item.Serial);

				itemID &= 0x3FFF;

				m_Stream.Write((ushort)itemID);

				m_Stream.Write((byte)0);
			}
			else
			{
				m_Stream.Write(item.Serial);

				itemID &= 0xFFFF;

				m_Stream.Write((ushort)itemID);

				m_Stream.Write((byte)0);
			}

			int amount = item.Amount;
			m_Stream.Write((short)amount);
			m_Stream.Write((short)amount);

			Point3D loc = item.Location;
			int x = loc.m_X & 0x7FFF;
			int y = loc.m_Y & 0x3FFF;
			m_Stream.Write((short)x);
			m_Stream.Write((short)y);
			m_Stream.Write((sbyte)loc.m_Z);

			m_Stream.Write((byte)item.Light);
			m_Stream.Write((short)item.Hue);
			m_Stream.Write((byte)item.GetPacketFlags());

			m_Stream.Write((short)0x00); // ??
		}

        public WorldItemHS(Item item, PacketWriter stream)
            : base(0xF3, 26, stream)
        {
            stream.Write((short)0x1);

            int itemID = item.ItemID;

            if (item is BaseMulti)
                m_Stream.Write((byte)0x02);
            else if (item is IDamageable)
                m_Stream.Write((byte)0x03);
            else
                m_Stream.Write((byte)0x00);

            if (item is BaseMulti)
            {
                stream.Write((int)item.Serial);
                itemID &= 0x3FFF;
                stream.Write((ushort)itemID);
                stream.Write((byte)0);
            }
            else
            {
                stream.Write((int)item.Serial);
                itemID &= 0xFFFF;
                stream.Write((ushort)itemID);
                stream.Write((byte)0);
            }

            int amount = item.Amount;
            stream.Write((short)amount);
            stream.Write((short)amount);

            Point3D loc = item.Location;
            int x = loc.m_X & 0x7FFF;
            int y = loc.m_Y & 0x3FFF;
            stream.Write((short)x);
            stream.Write((short)y);
            stream.Write((sbyte)loc.m_Z);

            stream.Write((byte)item.Light);
            stream.Write((short)item.Hue);
            stream.Write((byte)item.GetPacketFlags());

            stream.Write((short)0x00); // ??
        }

        public WorldItemHS(Mobile mob, PacketWriter stream)
            : base(0xF3, 26, stream)
        {
            stream.Write((short)0x1);

            stream.Write((byte)0x01);
            stream.Write((int)mob.Serial);
            stream.Write((ushort)mob.BodyValue);
            stream.Write((byte)0);

            int amount = 1;
            stream.Write((short)amount);
            stream.Write((short)amount);

            Point3D loc = mob.Location;
            int x = loc.m_X & 0x7FFF;
            int y = loc.m_Y & 0x3FFF;
            stream.Write((short)x);
            stream.Write((short)y);
            stream.Write((sbyte)loc.m_Z);

            stream.Write((byte)mob.LightLevel);
            stream.Write((short)mob.Hue);
            stream.Write((byte)mob.GetPacketFlags());

            stream.Write((short)0x00); // ??
        }
	}

	public sealed class LiftRej : Packet
	{
		public LiftRej(LRReason reason)
			: base(0x27, 2)
		{
			m_Stream.Write((byte)reason);
		}
	}

	public sealed class LogoutAck : Packet
	{
		public LogoutAck()
			: base(0xD1, 2)
		{
			m_Stream.Write((byte)0x01);
		}
	}

	public sealed class Weather : Packet
	{
		public Weather(int v1, int v2, int v3)
			: base(0x65, 4)
		{
			m_Stream.Write((byte)v1);
			m_Stream.Write((byte)v2);
			m_Stream.Write((byte)v3);
		}
	}

	public sealed class UnkD3 : Packet
	{
		public UnkD3(Mobile beholder, Mobile beheld)
			: base(0xD3)
		{
			EnsureCapacity(256);

			//int
			//short
			//short
			//short
			//byte
			//byte
			//short
			//byte
			//byte
			//short
			//short
			//short
			//while ( int != 0 )
			//{
			//short
			//byte
			//short
			//}

			m_Stream.Write(beheld.Serial);
			m_Stream.Write((short)beheld.Body);
			m_Stream.Write((short)beheld.X);
			m_Stream.Write((short)beheld.Y);
			m_Stream.Write((sbyte)beheld.Z);
			m_Stream.Write((byte)beheld.Direction);
			m_Stream.Write((ushort)beheld.Hue);
			m_Stream.Write((byte)beheld.GetPacketFlags());
			m_Stream.Write((byte)Notoriety.Compute(beholder, beheld));

			m_Stream.Write((short)0);
			m_Stream.Write((short)0);
			m_Stream.Write((short)0);

			m_Stream.Write(0);
		}
	}

	public sealed class GQRequest : Packet
	{
		public GQRequest()
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
		public PlayerMove(Direction d)
			: base(0x97, 2)
		{
			m_Stream.Write((byte)d);
			// @4C63B0
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
		public ClientVersionReq()
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
		public ParticleEffect(
			EffectType type,
			Serial from,
			Serial to,
			int itemID,
			Point3D fromPoint,
			Point3D toPoint,
			int speed,
			int duration,
			bool fixedDirection,
			bool explode,
			int hue,
			int renderMode,
			int effect,
			int explodeEffect,
			int explodeSound,
			Serial serial,
			int layer,
			int unknown)
			: base(0xC7, 49)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write(@from);
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

		public ParticleEffect(
			EffectType type,
			Serial from,
			Serial to,
			int itemID,
			IPoint3D fromPoint,
			IPoint3D toPoint,
			int speed,
			int duration,
			bool fixedDirection,
			bool explode,
			int hue,
			int renderMode,
			int effect,
			int explodeEffect,
			int explodeSound,
			Serial serial,
			int layer,
			int unknown)
			: base(0xC7, 49)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write(@from);
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

	public class HuedEffect : Packet
	{
		public HuedEffect(
			EffectType type,
			Serial from,
			Serial to,
			int itemID,
			Point3D fromPoint,
			Point3D toPoint,
			int speed,
			int duration,
			bool fixedDirection,
			bool explode,
			int hue,
			int renderMode)
			: base(0xC0, 36)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write(@from);
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

		public HuedEffect(
			EffectType type,
			Serial from,
			Serial to,
			int itemID,
			IPoint3D fromPoint,
			IPoint3D toPoint,
			int speed,
			int duration,
			bool fixedDirection,
			bool explode,
			int hue,
			int renderMode)
			: base(0xC0, 36)
		{
			m_Stream.Write((byte)type);
			m_Stream.Write(@from);
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
		}
	}

	public sealed class TargetParticleEffect : ParticleEffect
	{
		public TargetParticleEffect(
			IEntity e, int itemID, int speed, int duration, int hue, int renderMode, int effect, int layer, int unknown)
			: base(
				EffectType.FixedFrom,
				e.Serial,
				Serial.Zero,
				itemID,
				e.Location,
				e.Location,
				speed,
				duration,
				true,
				false,
				hue,
				renderMode,
				effect,
				1,
				0,
				e.Serial,
				layer,
				unknown)
		{ }
	}

	public sealed class TargetEffect : HuedEffect
	{
		public TargetEffect(IEntity e, int itemID, int speed, int duration, int hue, int renderMode)
			: base(
				EffectType.FixedFrom,
				e.Serial,
				Serial.Zero,
				itemID,
				e.Location,
				e.Location,
				speed,
				duration,
				true,
				false,
				hue,
				renderMode)
		{ }
	}

	public sealed class LocationParticleEffect : ParticleEffect
	{
		public LocationParticleEffect(
			IEntity e, int itemID, int speed, int duration, int hue, int renderMode, int effect, int unknown)
			: base(
				EffectType.FixedXYZ,
				e.Serial,
				Serial.Zero,
				itemID,
				e.Location,
				e.Location,
				speed,
				duration,
				true,
				false,
				hue,
				renderMode,
				effect,
				1,
				0,
				e.Serial,
				255,
				unknown)
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
		public MovingParticleEffect(
			IEntity from,
			IEntity to,
			int itemID,
			int speed,
			int duration,
			bool fixedDirection,
			bool explodes,
			int hue,
			int renderMode,
			int effect,
			int explodeEffect,
			int explodeSound,
			EffectLayer layer,
			int unknown)
			: base(
				EffectType.Moving,
				from.Serial,
				to.Serial,
				itemID,
				from.Location,
				to.Location,
				speed,
				duration,
				fixedDirection,
				explodes,
				hue,
				renderMode,
				effect,
				explodeEffect,
				explodeSound,
				Serial.Zero,
				(int)layer,
				unknown)
		{ }
	}

	public sealed class MovingEffect : HuedEffect
	{
		public MovingEffect(
			IEntity from,
			IEntity to,
			int itemID,
			int speed,
			int duration,
			bool fixedDirection,
			bool explodes,
			int hue,
			int renderMode)
			: base(
				EffectType.Moving,
				from.Serial,
				to.Serial,
				itemID,
				from.Location,
				to.Location,
				speed,
				duration,
				fixedDirection,
				explodes,
				hue,
				renderMode)
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

	public class ScreenEffect : Packet
	{
		public ScreenEffect(ScreenEffectType type)
			: base(0x70, 28)
		{
			m_Stream.Write((byte)0x04);
			m_Stream.Fill(8);
			m_Stream.Write((short)type);
			m_Stream.Fill(16);
		}
	}

	public sealed class ScreenFadeOut : ScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new ScreenFadeOut());

		public ScreenFadeOut()
			: base(ScreenEffectType.FadeOut)
		{ }
	}

	public sealed class ScreenFadeIn : ScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new ScreenFadeIn());

		public ScreenFadeIn()
			: base(ScreenEffectType.FadeIn)
		{ }
	}

	public sealed class ScreenFadeInOut : ScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new ScreenFadeInOut());

		public ScreenFadeInOut()
			: base(ScreenEffectType.FadeInOut)
		{ }
	}

	public sealed class ScreenLightFlash : ScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new ScreenLightFlash());

		public ScreenLightFlash()
			: base(ScreenEffectType.LightFlash)
		{ }
	}

	public sealed class ScreenDarkFlash : ScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new ScreenDarkFlash());

		public ScreenDarkFlash()
			: base(ScreenEffectType.DarkFlash)
		{ }
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

	/*public sealed class MovingEffect : Packet
{
public MovingEffect( IEntity from, IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool turn, int hue, int renderMode ) : base( 0xC0, 36 )
{
m_Stream.Write( (byte) 0x00 );
m_Stream.Write( (int) from.Serial );
m_Stream.Write( (int) to.Serial );
m_Stream.Write( (short) itemID );
m_Stream.Write( (short) from.Location.m_X );
m_Stream.Write( (short) from.Location.m_Y );
m_Stream.Write( (sbyte) from.Location.m_Z );
m_Stream.Write( (short) to.Location.m_X );
m_Stream.Write( (short) to.Location.m_Y );
m_Stream.Write( (sbyte) to.Location.m_Z );
m_Stream.Write( (byte) speed );
m_Stream.Write( (byte) duration );
m_Stream.Write( (byte) 0 );
m_Stream.Write( (byte) 0 );
m_Stream.Write( (bool) fixedDirection );
m_Stream.Write( (bool) turn );
m_Stream.Write( (int) hue );
m_Stream.Write( (int) renderMode );
}
}*/

	/*public sealed class LocationEffect : Packet
{
public LocationEffect( IPoint3D p, int itemID, int duration, int hue, int renderMode ) : base( 0xC0, 36 )
{
m_Stream.Write( (byte) 0x02 );
m_Stream.Write( (int) Serial.Zero );
m_Stream.Write( (int) Serial.Zero );
m_Stream.Write( (short) itemID );
m_Stream.Write( (short) p.X );
m_Stream.Write( (short) p.Y );
m_Stream.Write( (sbyte) p.Z );
m_Stream.Write( (short) p.X );
m_Stream.Write( (short) p.Y );
m_Stream.Write( (sbyte) p.Z );
m_Stream.Write( (byte) 10 );
m_Stream.Write( (byte) duration );
m_Stream.Write( (byte) 0 );
m_Stream.Write( (byte) 0 );
m_Stream.Write( (byte) 1 );
m_Stream.Write( (byte) 0 );
m_Stream.Write( (int) hue );
m_Stream.Write( (int) renderMode );
}
}*/

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
			m_Stream.Write((byte)0); // duration
			m_Stream.Write((short)0); // unk
			m_Stream.Write(false); // fixed direction
			m_Stream.Write(false); // explode
			m_Stream.Write(hue);
			m_Stream.Write(0); // render mode
		}
	}

	public sealed class DisplaySpellbook : Packet
	{
		public DisplaySpellbook(Item book)
			: base(0x24, 7)
		{
			m_Stream.Write(book.Serial);
			m_Stream.Write((short)-1);
		}
	}

	public sealed class DisplaySpellbookHS : Packet
	{
		public DisplaySpellbookHS(Item book)
			: base(0x24, 9)
		{
			m_Stream.Write(book.Serial);
			m_Stream.Write((short)-1);
			m_Stream.Write((short)0x7D);
		}
	}

	public sealed class NewSpellbookContent : Packet
	{
		public NewSpellbookContent(Item item, int graphic, int offset, ulong content)
			: base(0xBF)
		{
			EnsureCapacity(23);

			m_Stream.Write((short)0x1B);
			m_Stream.Write((short)0x01);

			m_Stream.Write(item.Serial);
			m_Stream.Write((short)graphic);
			m_Stream.Write((short)offset);

			for (int i = 0; i < 8; ++i)
			{
				m_Stream.Write((byte)(content >> (i * 8)));
			}
		}
	}

	public sealed class SpellbookContent : Packet
	{
		public SpellbookContent(int count, int offset, ulong content, Item item)
			: base(0x3C)
		{
			EnsureCapacity(5 + (count * 19));

			int written = 0;

			m_Stream.Write((ushort)0);

			ulong mask = 1;

			for (int i = 0; i < 64; ++i, mask <<= 1)
			{
				if ((content & mask) != 0)
				{
					m_Stream.Write((0x7FFFFFFF - i));
					m_Stream.Write((ushort)0);
					m_Stream.Write((byte)0);
					m_Stream.Write((ushort)(i + offset));
					m_Stream.Write((short)0);
					m_Stream.Write((short)0);
					m_Stream.Write(item.Serial);
					m_Stream.Write((short)0);

					++written;
				}
			}

			m_Stream.Seek(3, SeekOrigin.Begin);
			m_Stream.Write((ushort)written);
		}
	}

	public sealed class SpellbookContent6017 : Packet
	{
		public SpellbookContent6017(int count, int offset, ulong content, Item item)
			: base(0x3C)
		{
			EnsureCapacity(5 + (count * 20));

			int written = 0;

			m_Stream.Write((ushort)0);

			ulong mask = 1;

			for (int i = 0; i < 64; ++i, mask <<= 1)
			{
				if ((content & mask) != 0)
				{
					m_Stream.Write((0x7FFFFFFF - i));
					m_Stream.Write((ushort)0);
					m_Stream.Write((byte)0);
					m_Stream.Write((ushort)(i + offset));
					m_Stream.Write((short)0);
					m_Stream.Write((short)0);
					m_Stream.Write((byte)0); // Grid Location?
					m_Stream.Write(item.Serial);
					m_Stream.Write((short)0);

					++written;
				}
			}

			m_Stream.Seek(3, SeekOrigin.Begin);
			m_Stream.Write((ushort)written);
		}
	}

	public sealed class ContainerDisplay : Packet
	{
		public ContainerDisplay(Container c)
			: base(0x24, 7)
		{
			m_Stream.Write(c.Serial);
			m_Stream.Write((short)c.GumpID);
		}
	}

	public sealed class ContainerDisplayHS : Packet
	{
		public ContainerDisplayHS(Container c)
			: base(0x24, 9)
		{
			m_Stream.Write(c.Serial);
			m_Stream.Write((short)c.GumpID);
			m_Stream.Write((short)0x7D);
		}
	}

	public sealed class ContainerContentUpdate : Packet
	{
		public ContainerContentUpdate(Item item)
			: base(0x25, 20)
		{
			Serial parentSerial;

			if (item.Parent is Item)
			{
				parentSerial = ((Item)item.Parent).Serial;
			}
			else
			{
				Console.WriteLine("Warning: ContainerContentUpdate on item with !(parent is Item)");
				parentSerial = Serial.Zero;
			}

			m_Stream.Write(item.Serial);
			m_Stream.Write((ushort)item.ItemID);
			m_Stream.Write((byte)0); // signed, itemID offset
			m_Stream.Write((ushort)item.Amount);
			m_Stream.Write((short)item.X);
			m_Stream.Write((short)item.Y);
			m_Stream.Write(parentSerial);
			m_Stream.Write((ushort)(item.QuestItem ? Item.QuestItemHue : item.Hue));
		}
	}

	public sealed class ContainerContentUpdate6017 : Packet
	{
		public ContainerContentUpdate6017(Item item)
			: base(0x25, 21)
		{
			Serial parentSerial;

			if (item.Parent is Item)
			{
				parentSerial = ((Item)item.Parent).Serial;
			}
			else
			{
				Console.WriteLine("Warning: ContainerContentUpdate on item with !(parent is Item)");
				parentSerial = Serial.Zero;
			}

			m_Stream.Write(item.Serial);
			m_Stream.Write((ushort)item.ItemID);
			m_Stream.Write((byte)0); // signed, itemID offset
			m_Stream.Write((ushort)item.Amount);
			m_Stream.Write((short)item.X);
			m_Stream.Write((short)item.Y);
			m_Stream.Write((byte)0); // Grid Location?
			m_Stream.Write(parentSerial);
			m_Stream.Write((ushort)(item.QuestItem ? Item.QuestItemHue : item.Hue));
		}
	}

	public sealed class ContainerContent : Packet
	{
		public ContainerContent(Mobile beholder, Item beheld)
			: base(0x3C)
		{
			var items = beheld.Items;
			int count = items.Count;

			EnsureCapacity(5 + (count * 19));

			long pos = m_Stream.Position;

			int written = 0;

			m_Stream.Write((ushort)0);

			for (int i = 0; i < count; ++i)
			{
				Item child = items[i];

				if (!child.Deleted && beholder.CanSee(child))
				{
					Point3D loc = child.Location;

					m_Stream.Write(child.Serial);
					m_Stream.Write((ushort)child.ItemID);
					m_Stream.Write((byte)0); // signed, itemID offset
					m_Stream.Write((ushort)child.Amount);
					m_Stream.Write((short)loc.m_X);
					m_Stream.Write((short)loc.m_Y);
					m_Stream.Write(beheld.Serial);
					m_Stream.Write((ushort)(child.QuestItem ? Item.QuestItemHue : child.Hue));

					++written;
				}
			}

			m_Stream.Seek(pos, SeekOrigin.Begin);
			m_Stream.Write((ushort)written);
		}
	}

	public sealed class ContainerContent6017 : Packet
	{
		public ContainerContent6017(Mobile beholder, Item beheld)
			: base(0x3C)
		{
			var items = beheld.Items;
			int count = items.Count;

			EnsureCapacity(5 + (count * 20));

			long pos = m_Stream.Position;

			int written = 0;

			m_Stream.Write((ushort)0);

			for (int i = 0; i < count; ++i)
			{
				Item child = items[i];

				if (!child.Deleted && beholder.CanSee(child))
				{
					Point3D loc = child.Location;

					m_Stream.Write(child.Serial);
					m_Stream.Write((ushort)child.ItemID);
					m_Stream.Write((byte)0); // signed, itemID offset
					m_Stream.Write((ushort)child.Amount);
					m_Stream.Write((short)loc.m_X);
					m_Stream.Write((short)loc.m_Y);
					m_Stream.Write((byte)0); // Grid Location?
					m_Stream.Write(beheld.Serial);
					m_Stream.Write((ushort)(child.QuestItem ? Item.QuestItemHue : child.Hue));

					++written;
				}
			}

			m_Stream.Seek(pos, SeekOrigin.Begin);
			m_Stream.Write((ushort)written);
		}
	}

	public sealed class SetWarMode : Packet
	{
		public static readonly Packet InWarMode = SetStatic(new SetWarMode(true));
		public static readonly Packet InPeaceMode = SetStatic(new SetWarMode(false));

		public static Packet Instantiate(bool mode)
		{
			return (mode ? InWarMode : InPeaceMode);
		}

		public SetWarMode(bool mode)
			: base(0x72, 5)
		{
			m_Stream.Write(mode);
			m_Stream.Write((byte)0x00);
			m_Stream.Write((byte)0x32);
			m_Stream.Write((byte)0x00);
			//m_Stream.Fill();
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
		public NullFastwalkStack()
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

			for (int i = 0; i < skills.Length; ++i)
			{
				Skill s = skills[i];

				double v = s.NonRacialValue;
				int uv = (int)(v * 10);

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
		public Sequence(int num)
			: base(0x7B, 2)
		{
			m_Stream.Write((byte)num);
		}
	}

	public sealed class SkillChange : Packet
	{
		public SkillChange(Skill skill)
			: base(0x3A)
		{
			EnsureCapacity(13);

			double v = skill.NonRacialValue;
			int uv = (int)(v * 10);

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
			/*m_Stream.Write( (short) skill.Info.SkillID );
	m_Stream.Write( (short) (skill.Value * 10.0) );
	m_Stream.Write( (short) (skill.Base * 10.0) );
	m_Stream.Write( (byte) skill.Lock );
	m_Stream.Write( (short) skill.CapFixedPoint );*/
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
		private static readonly MessageLocalized[] m_Cache_IntLoc = new MessageLocalized[15000];
		private static readonly MessageLocalized[] m_Cache_CliLoc = new MessageLocalized[100000];
		private static readonly MessageLocalized[] m_Cache_CliLocCmp = new MessageLocalized[5000];

		public static MessageLocalized InstantiateGeneric(int number)
		{
			MessageLocalized[] cache = null;
			int index = 0;

			if (number >= 3000000)
			{
				cache = m_Cache_IntLoc;
				index = number - 3000000;
			}
			else if (number >= 1000000)
			{
				cache = m_Cache_CliLoc;
				index = number - 1000000;
			}
			else if (number >= 500000)
			{
				cache = m_Cache_CliLocCmp;
				index = number - 500000;
			}

			MessageLocalized p;

			if (cache != null && index >= 0 && index < cache.Length)
			{
				p = cache[index];

				if (p == null)
				{
					cache[index] = p = new MessageLocalized(Serial.MinusOne, -1, MessageType.Regular, 0x3B2, 3, number, "System", "");
					p.SetStatic();
				}
			}
			else
			{
				p = new MessageLocalized(Serial.MinusOne, -1, MessageType.Regular, 0x3B2, 3, number, "System", "");
			}

			return p;
		}

		public MessageLocalized(
			Serial serial, int graphic, MessageType type, int hue, int font, int number, string name, string args)
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
		public MobileMoving(Mobile m, int noto)
			: base(0x77, 17)
		{
			Point3D loc = m.Location;

			int hue = m.Hue;

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
			m_Stream.Write((byte)m.GetPacketFlags());
			m_Stream.Write((byte)noto);
		}
	}

	// Pre-7.0.0.0 Mobile Moving
	public sealed class MobileMovingOld : Packet
	{
		public MobileMovingOld(Mobile m, int noto)
			: base(0x77, 17)
		{
			Point3D loc = m.Location;

			int hue = m.Hue;

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
			m_Stream.Write((byte)m.GetOldPacketFlags());
			m_Stream.Write((byte)noto);
		}
	}

	public sealed class MultiTargetReqHS : Packet
	{
		public MultiTargetReqHS(MultiTarget t)
			: base(0x99, 30)
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
		}
	}

	public sealed class MultiTargetReq : Packet
	{
		public MultiTargetReq(MultiTarget t)
			: base(0x99, 26)
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
		}
	}

	public sealed class CancelTarget : Packet
	{
		public static readonly Packet Instance = SetStatic(new CancelTarget());

		public CancelTarget()
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
		public int TextEntries { get; set; }
		public int Switches { get; set; }

		private readonly Gump m_Gump;

		private readonly PacketWriter m_Layout;
		private readonly PacketWriter m_Strings;

		private int m_StringCount;

		public DisplayGumpPacked(Gump gump)
			: base(0xDD)
		{
			m_Gump = gump;

			m_Layout = PacketWriter.CreateInstance(8192);
			m_Strings = PacketWriter.CreateInstance(8192);
		}

		private static readonly byte[] m_True = Gump.StringToBuffer(" 1");
		private static readonly byte[] m_False = Gump.StringToBuffer(" 0");

		private static readonly byte[] m_BeginTextSeparator = Gump.StringToBuffer(" @");
		private static readonly byte[] m_EndTextSeparator = Gump.StringToBuffer("@");

		private static readonly byte[] m_Buffer = new byte[48];

		static DisplayGumpPacked()
		{
			m_Buffer[0] = (byte)' ';
		}

		public void AppendLayout(bool val)
		{
			AppendLayout(val ? m_True : m_False);
		}

		public void AppendLayout(int val)
		{
			string toString = val.ToString();
			int bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1) + 1;

			m_Layout.Write(m_Buffer, 0, bytes);
		}

		public void AppendLayoutNS(int val)
		{
			string toString = val.ToString();
			int bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1);

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

			for (int i = 0; i < strings.Count; ++i)
			{
				string v = strings[i];

				if (v == null)
				{
					v = String.Empty;
				}

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

			PacketWriter.ReleaseInstance(m_Layout);
			PacketWriter.ReleaseInstance(m_Strings);
		}

		private const int GumpBufferSize = 0x5000;
		private static readonly BufferPool m_PackBuffers = new BufferPool("Gump", 4, GumpBufferSize);

		private void WritePacked(PacketWriter src)
		{
			var buffer = src.UnderlyingStream.GetBuffer();
			int length = (int)src.Length;

			if (length == 0)
			{
				m_Stream.Write(0);
				return;
			}

			int wantLength = 1 + ((buffer.Length * 1024) / 1000);

			wantLength += 4095;
			wantLength &= ~4095;

			byte[] m_PackBuffer;
			lock (m_PackBuffers)
				m_PackBuffer = m_PackBuffers.AcquireBuffer();

			if (m_PackBuffer.Length < wantLength)
			{
				Console.WriteLine("Notice: DisplayGumpPacked creating new {0} byte buffer", wantLength);
				m_PackBuffer = new byte[wantLength];
			}

			int packLength = m_PackBuffer.Length;

			Compression.Pack(m_PackBuffer, ref packLength, buffer, length, ZLibQuality.Default);

			m_Stream.Write((4 + packLength));
			m_Stream.Write(length);
			m_Stream.Write(m_PackBuffer, 0, packLength);

			lock (m_PackBuffers)
				m_PackBuffers.ReleaseBuffer(m_PackBuffer);
		}
	}

	public sealed class DisplayGumpFast : Packet, IGumpWriter
	{
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

		private static readonly byte[] m_True = Gump.StringToBuffer(" 1");
		private static readonly byte[] m_False = Gump.StringToBuffer(" 0");

		private static readonly byte[] m_BeginTextSeparator = Gump.StringToBuffer(" @");
		private static readonly byte[] m_EndTextSeparator = Gump.StringToBuffer("@");

		private readonly byte[] m_Buffer = new byte[48];

		public void AppendLayout(bool val)
		{
			AppendLayout(val ? m_True : m_False);
		}

		public void AppendLayout(int val)
		{
			string toString = val.ToString();
			int bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1) + 1;

			m_Stream.Write(m_Buffer, 0, bytes);
			m_LayoutLength += bytes;
		}

		public void AppendLayoutNS(int val)
		{
			string toString = val.ToString();
			int bytes = Encoding.ASCII.GetBytes(toString, 0, toString.Length, m_Buffer, 1);

			m_Stream.Write(m_Buffer, 1, bytes);
			m_LayoutLength += bytes;
		}

		public void AppendLayout(string text)
		{
			AppendLayout(m_BeginTextSeparator);

			int length = text.Length;
			m_Stream.WriteAsciiFixed(text, length);
			m_LayoutLength += length;

			AppendLayout(m_EndTextSeparator);
		}

		public void AppendLayout(byte[] buffer)
		{
			int length = buffer.Length;
			m_Stream.Write(buffer, 0, length);
			m_LayoutLength += length;
		}

		public void WriteStrings(List<string> text)
		{
			m_Stream.Seek(19, SeekOrigin.Begin);
			m_Stream.Write((ushort)m_LayoutLength);
			m_Stream.Seek(0, SeekOrigin.End);

			m_Stream.Write((ushort)text.Count);

			for (int i = 0; i < text.Count; ++i)
			{
				string v = text[i];

				if (v == null)
				{
					v = String.Empty;
				}

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

			for (int i = 0; i < text.Length; ++i)
			{
				string v = text[i];

				if (v == null)
				{
					v = "";
				}

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
		public PopupMessage(PMMessage msg)
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
		public static readonly Packet InvalidInstance = SetStatic(new PlayMusic(MusicName.Invalid));

		private static readonly Packet[] m_Instances = new Packet[60];

		public static Packet GetInstance(MusicName name)
		{
			if (name == MusicName.Invalid)
			{
				return InvalidInstance;
			}

			int v = (int)name;
			Packet p;

			if (v >= 0 && v < m_Instances.Length)
			{
				p = m_Instances[v];

				if (p == null)
				{
					m_Instances[v] = p = SetStatic(new PlayMusic(name));
				}
			}
			else
			{
				p = new PlayMusic(name);
			}

			return p;
		}

		public PlayMusic(MusicName name)
			: base(0x6D, 3)
		{
			m_Stream.Write((short)name);
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
			DateTime now = DateTime.UtcNow;

			m_Stream.Write((byte)now.Hour);
			m_Stream.Write((byte)now.Minute);
			m_Stream.Write((byte)now.Second);
		}
	}

	public sealed class MapChange : Packet
	{
		public MapChange(Mobile m)
			: base(0xBF)
		{
			EnsureCapacity(6);

			m_Stream.Write((short)0x08);
			m_Stream.Write((byte)(m.Map == null ? 0 : m.Map.MapID));
		}
	}

	public sealed class SeasonChange : Packet
	{
		private static readonly SeasonChange[][] m_Cache = new SeasonChange[5][]
		{new SeasonChange[2], new SeasonChange[2], new SeasonChange[2], new SeasonChange[2], new SeasonChange[2]};

		public static SeasonChange Instantiate(int season)
		{
			return Instantiate(season, true);
		}

		public static SeasonChange Instantiate(int season, bool playSound)
		{
			if (season >= 0 && season < m_Cache.Length)
			{
				int idx = playSound ? 1 : 0;

				SeasonChange p = m_Cache[season][idx];

				if (p == null)
				{
					m_Cache[season][idx] = p = new SeasonChange(season, playSound);
					p.SetStatic();
				}

				return p;
			}
			else
			{
				return new SeasonChange(season, playSound);
			}
		}

		public SeasonChange(int season)
			: this(season, true)
		{ }

		public SeasonChange(int season, bool playSound)
			: base(0xBC, 3)
		{
			m_Stream.Write((byte)season);
			m_Stream.Write(playSound);
		}
	}

	public sealed class SupportedFeatures : Packet
	{
		private static FeatureFlags m_AdditionalFlags;

		public static FeatureFlags Value { get { return m_AdditionalFlags; } set { m_AdditionalFlags = value; } }

		public static SupportedFeatures Instantiate(NetState ns)
		{
			return new SupportedFeatures(ns);
		}

		public SupportedFeatures(NetState ns)
			: base(0xB9, ns.ExtendedSupportedFeatures ? 5 : 3)
		{
			FeatureFlags flags = ExpansionInfo.CoreExpansion.SupportedFeatures;

			flags |= m_AdditionalFlags;

			IAccount acct = ns.Account;

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

			if (ns.ExtendedSupportedFeatures)
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
		private static int m_Maximum = 25;
		private static bool m_Enabled = true;

		public static int Maximum { get { return m_Maximum; } set { m_Maximum = value; } }

		public static bool Enabled { get { return m_Enabled; } set { m_Enabled = value; } }

		public static void Write(PacketWriter stream, int cur, int max)
		{
			if (m_Enabled && max != 0)
			{
				stream.Write((short)m_Maximum);
				stream.Write((short)((cur * m_Maximum) / max));
			}
			else
			{
				stream.Write((short)max);
				stream.Write((short)cur);
			}
		}

		public static void WriteReverse(PacketWriter stream, int cur, int max)
		{
			if (m_Enabled && max != 0)
			{
				stream.Write((short)((cur * m_Maximum) / max));
				stream.Write((short)m_Maximum);
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

	// unsure of proper format, client crashes
	public sealed class MobileName : Packet
	{
		public MobileName(Mobile m)
			: base(0x98)
		{
			string name = m.Name;

			if (name == null)
			{
				name = "";
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
		public NewMobileAnimation(Mobile m, int action, int frameCount, int delay)
			: base(0xE2, 10)
		{
			m_Stream.Write(m.Serial);
			m_Stream.Write((short)action);
			m_Stream.Write((short)frameCount);
			m_Stream.Write((byte)delay);
		}
	}

	public sealed class MobileStatusCompact : Packet
	{
		public MobileStatusCompact(bool canBeRenamed, IDamageable d)
			: base(0x11)
		{
            string name = d.Name == null ? "" : d.Name;

			EnsureCapacity(43);

			m_Stream.Write(d.Serial);
			m_Stream.WriteAsciiFixed(name, 30);

			AttributeNormalizer.WriteReverse(m_Stream, d.Hits, d.HitsMax);

			m_Stream.Write(canBeRenamed);

			m_Stream.Write((byte)0); // type
		}
	}

	public sealed class MobileStatusExtended : Packet
	{
		public MobileStatusExtended(Mobile m)
			: this(m, m.NetState)
		{ }

		public MobileStatusExtended(Mobile m, NetState ns)
			: base(0x11)
		{
			string name = m.Name;
			if (name == null)
			{
				name = "";
			}

			int type;

			if (Core.HS && ns != null && ns.ExtendedStatus)
			{
				type = 6;
				EnsureCapacity(121);
			}
			else if (Core.ML && ns != null && ns.SupportsExpansion(Expansion.ML))
			{
				type = 5;
				EnsureCapacity(91);
			}
			else
			{
				type = Core.AOS ? 4 : 3;
				EnsureCapacity(88);
			}

			m_Stream.Write(m.Serial);
			m_Stream.WriteAsciiFixed(name, 30);

			m_Stream.Write((short)m.Hits);
			m_Stream.Write((short)m.HitsMax);

			m_Stream.Write(m.CanBeRenamedBy(m));

			m_Stream.Write((byte)type);

			m_Stream.Write(m.Female);

			m_Stream.Write((short)m.Str);
			m_Stream.Write((short)m.Dex);
			m_Stream.Write((short)m.Int);

			m_Stream.Write((short)m.Stam);
			m_Stream.Write((short)m.StamMax);

			m_Stream.Write((short)m.Mana);
			m_Stream.Write((short)m.ManaMax);

			m_Stream.Write(m.TotalGold);
			m_Stream.Write((short)(Core.AOS ? m.PhysicalResistance : (int)(m.ArmorRating + 0.5)));
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

				IWeapon weapon = m.Weapon;

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
				for (int i = 0; i < 15; ++i)
				{
					m_Stream.Write((short)m.GetAOSStatus(i));
				}
			}
		}
	}

    public sealed class MobileStatus : Packet
    {
        public MobileStatus(Mobile beholder, Mobile beheld)
            : this(beholder, beheld, beheld.NetState)
        { }

        public MobileStatus(Mobile beholder, Mobile beheld, NetState ns)
            : base(0x11)
        {
            string name = beheld.Name;
            if (name == null)
            {
                name = "";
            }

            int type;

            if (beholder != beheld)
            {
                type = 0;
                EnsureCapacity(43);
            }
            else if (Core.HS && ns != null && ns.ExtendedStatus)
            {
                type = 6;
                EnsureCapacity(121);
            }
            else if (Core.ML && ns != null && ns.SupportsExpansion(Expansion.ML))
            {
                type = 5;
                EnsureCapacity(91);
            }
            else
            {
                type = Core.AOS ? 4 : 3;
                EnsureCapacity(88);
            }

            m_Stream.Write(beheld.Serial);

            m_Stream.WriteAsciiFixed(name, 30);

            if (beholder == beheld)
            {
                WriteAttr(beheld.Hits, beheld.HitsMax);
            }
            else
            {
                WriteAttrNorm(beheld.Hits, beheld.HitsMax);
            }

            m_Stream.Write(beheld.CanBeRenamedBy(beholder));

            m_Stream.Write((byte)type);

            if (type > 0)
            {
                m_Stream.Write(beheld.Female);

                m_Stream.Write((short)beheld.Str);
                m_Stream.Write((short)beheld.Dex);
                m_Stream.Write((short)beheld.Int);

                WriteAttr(beheld.Stam, beheld.StamMax);
                WriteAttr(beheld.Mana, beheld.ManaMax);

                m_Stream.Write(beheld.TotalGold);
                m_Stream.Write((short)(Core.AOS ? beheld.PhysicalResistance : (int)(beheld.ArmorRating + 0.5)));
                m_Stream.Write((short)(Mobile.BodyWeight + beheld.TotalWeight));

                if (type >= 5)
                {
                    m_Stream.Write((short)beheld.MaxWeight);
                    m_Stream.Write((byte)(beheld.Race.RaceID + 1)); // Would be 0x00 if it's a non-ML enabled account but...
                }

                m_Stream.Write((short)beheld.StatCap);

                m_Stream.Write((byte)beheld.Followers);
                m_Stream.Write((byte)beheld.FollowersMax);

                if (type >= 4)
                {
                    m_Stream.Write((short)beheld.FireResistance); // Fire
                    m_Stream.Write((short)beheld.ColdResistance); // Cold
                    m_Stream.Write((short)beheld.PoisonResistance); // Poison
                    m_Stream.Write((short)beheld.EnergyResistance); // Energy
                    m_Stream.Write((short)beheld.Luck); // Luck

                    IWeapon weapon = beheld.Weapon;

                    int min = 0, max = 0;

                    if (weapon != null)
                    {
                        weapon.GetStatusDamage(beheld, out min, out max);
                    }

                    m_Stream.Write((short)min); // Damage min
                    m_Stream.Write((short)max); // Damage max

                    m_Stream.Write(beheld.TithingPoints);
                }

                if (type >= 6)
                {
                    for (int i = 0; i < 15; ++i)
                    {
                        m_Stream.Write((short)beheld.GetAOSStatus(i));
                    }
                }
            }
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

			Poison p = m.Poison;

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

	public sealed class MobileUpdate : Packet
	{
		public MobileUpdate(Mobile m)
			: base(0x20, 19)
		{
			int hue = m.Hue;

			if (m.SolidHueOverride >= 0)
			{
				hue = m.SolidHueOverride;
			}

			m_Stream.Write(m.Serial);
			m_Stream.Write((short)m.Body);
			m_Stream.Write((byte)0);
			m_Stream.Write((short)hue);
			m_Stream.Write((byte)m.GetPacketFlags());
			m_Stream.Write((short)m.X);
			m_Stream.Write((short)m.Y);
			m_Stream.Write((short)0);
			m_Stream.Write((byte)m.Direction);
			m_Stream.Write((sbyte)m.Z);
		}
	}

	// Pre-7.0.0.0 Mobile Update
	public sealed class MobileUpdateOld : Packet
	{
		public MobileUpdateOld(Mobile m)
			: base(0x20, 19)
		{
			int hue = m.Hue;

			if (m.SolidHueOverride >= 0)
			{
				hue = m.SolidHueOverride;
			}

			m_Stream.Write(m.Serial);
			m_Stream.Write((short)m.Body);
			m_Stream.Write((byte)0);
			m_Stream.Write((short)hue);
			m_Stream.Write((byte)m.GetOldPacketFlags());
			m_Stream.Write((short)m.X);
			m_Stream.Write((short)m.Y);
			m_Stream.Write((short)0);
			m_Stream.Write((byte)m.Direction);
			m_Stream.Write((sbyte)m.Z);
		}
	}

	public sealed class MobileIncoming : Packet
	{
		public static Packet Create(NetState ns, Mobile beholder, Mobile beheld)
		{
			if (ns.NewMobileIncoming)
			{
				return new MobileIncoming(beholder, beheld);
			}
			else if (ns.StygianAbyss)
			{
				return new MobileIncomingSA(beholder, beheld);
			}
			else
			{
				return new MobileIncomingOld(beholder, beheld);
			}
		}

		private static readonly ThreadLocal<int[]> m_DupedLayersTL = new ThreadLocal<int[]>(() => { return new int[256]; });
		private static readonly ThreadLocal<int> m_VersionTL = new ThreadLocal<int>();

		public Mobile m_Beheld;

		public MobileIncoming(Mobile beholder, Mobile beheld)
			: base(0x78)
		{
			m_Beheld = beheld;

			int m_Version = ++(m_VersionTL.Value);
			var m_DupedLayers = m_DupedLayersTL.Value;

			var eq = beheld.Items;
			int count = eq.Count;

			if (beheld.HairItemID > 0)
			{
				count++;
			}
			if (beheld.FacialHairItemID > 0)
			{
				count++;
			}

			EnsureCapacity(23 + (count * 9));

			int hue = beheld.Hue;

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
			m_Stream.Write((byte)beheld.GetPacketFlags());
			m_Stream.Write((byte)Notoriety.Compute(beholder, beheld));

			for (int i = 0; i < eq.Count; ++i)
			{
				Item item = eq[i];

				byte layer = (byte)item.Layer;

				if (!item.Deleted && beholder.CanSee(item) && m_DupedLayers[layer] != m_Version)
				{
					m_DupedLayers[layer] = m_Version;

					hue = item.Hue;

					if (beheld.SolidHueOverride >= 0)
					{
						hue = beheld.SolidHueOverride;
					}

					int itemID = item.ItemID & 0xFFFF;

					m_Stream.Write(item.Serial);
					m_Stream.Write((ushort)itemID);
					m_Stream.Write(layer);

					m_Stream.Write((short)hue);
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

					int itemID = beheld.HairItemID & 0xFFFF;

					m_Stream.Write(HairInfo.FakeSerial(beheld));
					m_Stream.Write((ushort)itemID);
					m_Stream.Write((byte)Layer.Hair);

					m_Stream.Write((short)hue);
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

					int itemID = beheld.FacialHairItemID & 0xFFFF;

					m_Stream.Write(FacialHairInfo.FakeSerial(beheld));
					m_Stream.Write((ushort)itemID);
					m_Stream.Write((byte)Layer.FacialHair);

					m_Stream.Write((short)hue);
				}
			}

			m_Stream.Write(0); // terminate
		}
	}

	public sealed class MobileIncomingSA : Packet
	{
		private static readonly ThreadLocal<int[]> m_DupedLayersTL = new ThreadLocal<int[]>(() => { return new int[256]; });
		private static readonly ThreadLocal<int> m_VersionTL = new ThreadLocal<int>();

		public Mobile m_Beheld;

		public MobileIncomingSA(Mobile beholder, Mobile beheld)
			: base(0x78)
		{
			m_Beheld = beheld;

			int m_Version = ++(m_VersionTL.Value);
			var m_DupedLayers = m_DupedLayersTL.Value;

			var eq = beheld.Items;
			int count = eq.Count;

			if (beheld.HairItemID > 0)
			{
				count++;
			}
			if (beheld.FacialHairItemID > 0)
			{
				count++;
			}

			EnsureCapacity(23 + (count * 9));

			int hue = beheld.Hue;

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
			m_Stream.Write((byte)beheld.GetPacketFlags());
			m_Stream.Write((byte)Notoriety.Compute(beholder, beheld));

			for (int i = 0; i < eq.Count; ++i)
			{
				Item item = eq[i];

				byte layer = (byte)item.Layer;

				if (!item.Deleted && beholder.CanSee(item) && m_DupedLayers[layer] != m_Version)
				{
					m_DupedLayers[layer] = m_Version;

					hue = item.Hue;

					if (beheld.SolidHueOverride >= 0)
					{
						hue = beheld.SolidHueOverride;
					}

					int itemID = item.ItemID & 0x7FFF;
					bool writeHue = (hue != 0);

					if (writeHue)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(item.Serial);
					m_Stream.Write((ushort)itemID);
					m_Stream.Write(layer);

					if (writeHue)
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

					int itemID = beheld.HairItemID & 0x7FFF;

					bool writeHue = (hue != 0);

					if (writeHue)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(HairInfo.FakeSerial(beheld));
					m_Stream.Write((ushort)itemID);
					m_Stream.Write((byte)Layer.Hair);

					if (writeHue)
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

					int itemID = beheld.FacialHairItemID & 0x7FFF;

					bool writeHue = (hue != 0);

					if (writeHue)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(FacialHairInfo.FakeSerial(beheld));
					m_Stream.Write((ushort)itemID);
					m_Stream.Write((byte)Layer.FacialHair);

					if (writeHue)
					{
						m_Stream.Write((short)hue);
					}
				}
			}

			m_Stream.Write(0); // terminate
		}
	}

	// Pre-7.0.0.0 Mobile Incoming
	public sealed class MobileIncomingOld : Packet
	{
		private static readonly ThreadLocal<int[]> m_DupedLayersTL = new ThreadLocal<int[]>(() => { return new int[256]; });
		private static readonly ThreadLocal<int> m_VersionTL = new ThreadLocal<int>();

		public Mobile m_Beheld;

		public MobileIncomingOld(Mobile beholder, Mobile beheld)
			: base(0x78)
		{
			m_Beheld = beheld;

			int m_Version = ++(m_VersionTL.Value);
			var m_DupedLayers = m_DupedLayersTL.Value;

			var eq = beheld.Items;
			int count = eq.Count;

			if (beheld.HairItemID > 0)
			{
				count++;
			}
			if (beheld.FacialHairItemID > 0)
			{
				count++;
			}

			EnsureCapacity(23 + (count * 9));

			int hue = beheld.Hue;

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
			m_Stream.Write((byte)beheld.GetOldPacketFlags());
			m_Stream.Write((byte)Notoriety.Compute(beholder, beheld));

			for (int i = 0; i < eq.Count; ++i)
			{
				Item item = eq[i];

				byte layer = (byte)item.Layer;

				if (!item.Deleted && beholder.CanSee(item) && m_DupedLayers[layer] != m_Version)
				{
					m_DupedLayers[layer] = m_Version;

					hue = item.Hue;

					if (beheld.SolidHueOverride >= 0)
					{
						hue = beheld.SolidHueOverride;
					}

					int itemID = item.ItemID & 0x7FFF;
					bool writeHue = (hue != 0);

					if (writeHue)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(item.Serial);
					m_Stream.Write((ushort)itemID);
					m_Stream.Write(layer);

					if (writeHue)
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

					int itemID = beheld.HairItemID & 0x7FFF;

					bool writeHue = (hue != 0);

					if (writeHue)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(HairInfo.FakeSerial(beheld));
					m_Stream.Write((ushort)itemID);
					m_Stream.Write((byte)Layer.Hair);

					if (writeHue)
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

					int itemID = beheld.FacialHairItemID & 0x7FFF;

					bool writeHue = (hue != 0);

					if (writeHue)
					{
						itemID |= 0x8000;
					}

					m_Stream.Write(FacialHairInfo.FakeSerial(beheld));
					m_Stream.Write((ushort)itemID);
					m_Stream.Write((byte)Layer.FacialHair);

					if (writeHue)
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
		public UnicodeMessage(
			Serial serial, int graphic, MessageType type, int hue, int font, string lang, string name, string text)
			: base(0xAE)
		{
			if (string.IsNullOrEmpty(lang))
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

			EnsureCapacity(50 + (text.Length * 2));

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
		private static readonly PingAck[] m_Cache = new PingAck[0x100];

		public static PingAck Instantiate(byte ping)
		{
			PingAck p = m_Cache[ping];

			if (p == null)
			{
				m_Cache[ping] = p = new PingAck(ping);
				p.SetStatic();
			}

			return p;
		}

		public PingAck(byte ping)
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
		private static readonly MovementAck[][] m_Cache = new MovementAck[8][]
		{
			new MovementAck[256], new MovementAck[256], new MovementAck[256], new MovementAck[256], new MovementAck[256],
			new MovementAck[256], new MovementAck[256], new MovementAck[256]
		};

		public static MovementAck Instantiate(int seq, Mobile m)
		{
			int noto = Notoriety.Compute(m, m);

			MovementAck p = m_Cache[noto][seq];

			if (p == null)
			{
				m_Cache[noto][seq] = p = new MovementAck(seq, noto);
				p.SetStatic();
			}

			return p;
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

			Map map = m.Map;

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
		public static readonly Packet Instance = SetStatic(new LoginComplete());

		public LoginComplete()
			: base(0x55, 1)
		{ }
	}

	public sealed class CityInfo
	{
		private Point3D m_Location;

		public CityInfo(string city, string building, int description, int x, int y, int z, Map m)
		{
			City = city;
			Building = building;
			Description = description;
			m_Location = new Point3D(x, y, z);
			Map = m;
		}

		public CityInfo(string city, string building, int x, int y, int z, Map m)
			: this(city, building, 0, x, y, z, m)
		{ }

		public CityInfo(string city, string building, int description, int x, int y, int z)
			: this(city, building, description, x, y, z, Map.Trammel)
		{ }

		public CityInfo(string city, string building, int x, int y, int z)
			: this(city, building, 0, x, y, z, Map.Trammel)
		{ }

		public string City { get; set; }
		public string Building { get; set; }
		public int Description { get; set; }
		public int X { get { return m_Location.X; } set { m_Location.X = value; } }
		public int Y { get { return m_Location.Y; } set { m_Location.Y = value; } }
		public int Z { get { return m_Location.Z; } set { m_Location.Z = value; } }
		public Point3D Location { get { return m_Location; } set { m_Location = value; } }
		public Map Map { get; set; }
	}

	public sealed class CharacterListUpdate : Packet
	{
		public CharacterListUpdate(IAccount a)
			: base(0x86)
		{
			EnsureCapacity(4 + (a.Length * 60));

			int highSlot = -1;

			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != null)
				{
					highSlot = i;
				}
			}

			int count = Math.Max(Math.Max(highSlot + 1, a.Limit), 5);

			m_Stream.Write((byte)count);

			for (int i = 0; i < count; ++i)
			{
				Mobile m = a[i];

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
		FilterWeather = 1 << 0,
		FilterLight = 1 << 1,

		SmartTarget = 1 << 2,
		RangedTarget = 1 << 3,

		AutoOpenDoors = 1 << 4,

		DequipOnCast = 1 << 5,
		AutoPotionEquip = 1 << 6,

		ProtectHeals = 1 << 7,

		LoopedMacros = 1 << 8,

		UseOnceAgent = 1 << 9,
		RestockAgent = 1 << 10,
		SellAgent = 1 << 11,
		BuyAgent = 1 << 12,

		PotionHotkeys = 1 << 13,

		RandomTargets = 1 << 14,
		ClosestTargets = 1 << 15, // All closest target hotkeys
		OverheadHealth = 1 << 16, // Health and Mana/Stam messages shown over player's heads

		AutolootAgent = 1 << 17,
		BoneCutterAgent = 1 << 18,
		AdvancedMacros = 1 << 19,
		AutoRemount = 1 << 20,
		AutoBandage = 1 << 21,
		EnemyTargetShare = 1 << 22,
		FilterSeason = 1 << 23,
		SpellTargetShare = 1 << 24,

		All = ulong.MaxValue
	}

	public static class FeatureProtection
	{
		private static ThirdPartyFeature m_Disabled = 0;

		public static ThirdPartyFeature DisabledFeatures { get { return m_Disabled; } }

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
				m_Disabled |= feature;
			}
			else
			{
				m_Disabled &= ~feature;
			}
		}
	}

	public sealed class CharacterList : Packet
	{
		public CharacterList(IAccount a, CityInfo[] info)
			: base(0xA9)
		{
			EnsureCapacity(11 + (a.Length * 60) + (info.Length * 89));

			int highSlot = -1;

			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != null)
				{
					highSlot = i;
				}
			}

			int count = Math.Max(Math.Max(highSlot + 1, a.Limit), 5);

			m_Stream.Write((byte)count);

			for (int i = 0; i < count; ++i)
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

			for (int i = 0; i < info.Length; ++i)
			{
				CityInfo ci = info[i];

				m_Stream.Write((byte)i);
				m_Stream.WriteAsciiFixed(ci.City, 32);
				m_Stream.WriteAsciiFixed(ci.Building, 32);
				m_Stream.Write(ci.X);
				m_Stream.Write(ci.Y);
				m_Stream.Write(ci.Z);
				m_Stream.Write(ci.Map.MapID);
				m_Stream.Write(ci.Description);
				m_Stream.Write(0);
			}

			CharacterListFlags flags = ExpansionInfo.CoreExpansion.CharacterListFlags;

			if (count > 6)
			{
				flags |= (CharacterListFlags.SeventhCharacterSlot | CharacterListFlags.SixthCharacterSlot);
			}
				// 7th Character Slot - TODO: Is SixthCharacterSlot Required?
			else if (count == 6)
			{
				flags |= CharacterListFlags.SixthCharacterSlot; // 6th Character Slot
			}
			else if (a.Limit == 1)
			{
				flags |= (CharacterListFlags.SlotLimit & CharacterListFlags.OneCharacterSlot); // Limit Characters & One Character
			}

			m_Stream.Write((int)(flags | m_AdditionalFlags)); // Additional Flags

			m_Stream.Write((short)-1);

			ThirdPartyFeature disabled = FeatureProtection.DisabledFeatures;

			if (disabled != 0)
			{
				if (m_MD5Provider == null)
				{
					m_MD5Provider = new MD5CryptoServiceProvider();
				}

				m_Stream.UnderlyingStream.Flush();

				var hashCode = m_MD5Provider.ComputeHash(
					m_Stream.UnderlyingStream.GetBuffer(), 0, (int)m_Stream.UnderlyingStream.Length);
				var buffer = new byte[28];

				for (int i = 0; i < count; ++i)
				{
					Utility.RandomBytes(buffer);

					m_Stream.Seek(35 + (i * 60), SeekOrigin.Begin);
					m_Stream.Write(buffer, 0, buffer.Length);
				}

				m_Stream.Seek(35, SeekOrigin.Begin);
				m_Stream.Write((int)((long)disabled >> 32));
				m_Stream.Write((int)disabled);

				m_Stream.Seek(95, SeekOrigin.Begin);
				m_Stream.Write(hashCode, 0, hashCode.Length);
			}
		}

		private static MD5CryptoServiceProvider m_MD5Provider;

		private static CharacterListFlags m_AdditionalFlags;

		public static CharacterListFlags AdditionalFlags { get { return m_AdditionalFlags; } set { m_AdditionalFlags = value; } }
	}

	public sealed class CharacterListOld : Packet
	{
		public CharacterListOld(IAccount a, CityInfo[] info)
			: base(0xA9)
		{
			EnsureCapacity(9 + (a.Length * 60) + (info.Length * 63));

			int highSlot = -1;

			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != null)
				{
					highSlot = i;
				}
			}

			int count = Math.Max(Math.Max(highSlot + 1, a.Limit), 5);

			m_Stream.Write((byte)count);

			for (int i = 0; i < count; ++i)
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

			for (int i = 0; i < info.Length; ++i)
			{
				CityInfo ci = info[i];

				m_Stream.Write((byte)i);
				m_Stream.WriteAsciiFixed(ci.City, 31);
				m_Stream.WriteAsciiFixed(ci.Building, 31);
			}

			CharacterListFlags flags = ExpansionInfo.CoreExpansion.CharacterListFlags;

			if (count > 6)
			{
				flags |= (CharacterListFlags.SeventhCharacterSlot | CharacterListFlags.SixthCharacterSlot);
			}
				// 7th Character Slot - TODO: Is SixthCharacterSlot Required?
			else if (count == 6)
			{
				flags |= CharacterListFlags.SixthCharacterSlot; // 6th Character Slot
			}
			else if (a.Limit == 1)
			{
				flags |= (CharacterListFlags.SlotLimit & CharacterListFlags.OneCharacterSlot); // Limit Characters & One Character
			}

			m_Stream.Write((int)(flags | CharacterList.AdditionalFlags)); // Additional Flags

			ThirdPartyFeature disabled = FeatureProtection.DisabledFeatures;

			if (disabled != 0)
			{
				if (m_MD5Provider == null)
				{
					m_MD5Provider = new MD5CryptoServiceProvider();
				}

				m_Stream.UnderlyingStream.Flush();

				var hashCode = m_MD5Provider.ComputeHash(
					m_Stream.UnderlyingStream.GetBuffer(), 0, (int)m_Stream.UnderlyingStream.Length);
				var buffer = new byte[28];

				for (int i = 0; i < count; ++i)
				{
					Utility.RandomBytes(buffer);

					m_Stream.Seek(35 + (i * 60), SeekOrigin.Begin);
					m_Stream.Write(buffer, 0, buffer.Length);
				}

				m_Stream.Seek(35, SeekOrigin.Begin);
				m_Stream.Write((int)((long)disabled >> 32));
				m_Stream.Write((int)disabled);

				m_Stream.Seek(95, SeekOrigin.Begin);
				m_Stream.Write(hashCode, 0, hashCode.Length);
			}
		}

		private static MD5CryptoServiceProvider m_MD5Provider;
	}

	public sealed class ClearWeaponAbility : Packet
	{
		public static readonly Packet Instance = SetStatic(new ClearWeaponAbility());

		public ClearWeaponAbility()
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
		public AccountLoginRej(ALRReason reason)
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
		public MessageLocalizedAffix(
			Serial serial,
			int graphic,
			MessageType messageType,
			int hue,
			int font,
			int number,
			string name,
			AffixType affixType,
			string affix,
			string args)
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
			m_Stream.WriteBigUniNull(args);
		}
	}

	public sealed class ServerInfo
	{
		public string Name { get; set; }

		public int FullPercent { get; set; }

		public int TimeZone { get; set; }

		public IPEndPoint Address { get; set; }

		public ServerInfo(string name, int fullPercent, TimeZone tz, IPEndPoint address)
		{
			Name = name;
			FullPercent = fullPercent;
			TimeZone = tz.GetUtcOffset(DateTime.Now).Hours;
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

			for (int i = 0; i < info.Length; ++i)
			{
				ServerInfo si = info[i];

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
			m_Stream.Write((short)(unknown.Length));
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
		internal static int m_AuthID = -1;

		public PlayServerAck(ServerInfo si)
			: base(0x8C, 11)
		{
			int addr = Utility.GetAddressValue(si.Address.Address);

			m_Stream.Write((byte)addr);
			m_Stream.Write((byte)(addr >> 8));
			m_Stream.Write((byte)(addr >> 16));
			m_Stream.Write((byte)(addr >> 24));

			m_Stream.Write((short)si.Address.Port);
			m_Stream.Write(m_AuthID);
		}
	}

	public abstract class Packet
	{
		[Flags]
		private enum State
		{
			Inactive = 0x00,
			Static = 0x01,
			Acquired = 0x02,
			Accessed = 0x04,
			Buffered = 0x08,
			Warned = 0x10
		}

		protected PacketWriter m_Stream;
		private readonly int m_PacketID;
		private readonly int m_Length;
		private State m_State;

		public int PacketID { get { return m_PacketID; } }

        protected Packet(int packetID)
        {
            m_PacketID = packetID;

            if (Core.Profiling)
            {
                PacketSendProfile prof = PacketSendProfile.Acquire(GetType());
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
                PacketSendProfile prof = PacketSendProfile.Acquire(GetType());
                prof.Increment();
            }
        }

        public void EnsureCapacity(int length)
        {
            m_Stream = PacketWriter.CreateInstance(length);// new PacketWriter( length );
            m_Stream.Write((byte)m_PacketID);
            m_Stream.Write((short)0);
        }

		public PacketWriter UnderlyingStream { get { return m_Stream; } }

		private const int CompressorBufferSize = 0x10000;
		private static readonly BufferPool m_CompressorBuffers = new BufferPool("Compressor", 4, CompressorBufferSize);

		private const int BufferSize = 4096;
		private static readonly BufferPool m_Buffers = new BufferPool("Compressed", 16, BufferSize);

		public static Packet SetStatic(Packet p)
		{
			p.SetStatic();
			return p;
		}

		public static Packet Acquire(Packet p)
		{
			p.Acquire();
			return p;
		}

		public static void Release(ref ObjectPropertyList p)
		{
			if (p != null)
			{
				p.Release();
			}

			p = null;
		}

		public static void Release(ref RemoveItem p)
		{
			if (p != null)
			{
				p.Release();
			}

			p = null;
		}

		public static void Release(ref RemoveMobile p)
		{
			if (p != null)
			{
				p.Release();
			}

			p = null;
		}

		public static void Release(ref OPLInfo p)
		{
			if (p != null)
			{
				p.Release();
			}

			p = null;
		}

		public static void Release(ref Packet p)
		{
			if (p != null)
			{
				p.Release();
			}

			p = null;
		}

		public static void Release(Packet p)
		{
			if (p != null)
			{
				p.Release();
			}
		}

		public void SetStatic()
		{
			m_State |= State.Static | State.Acquired;
		}

		public void Acquire()
		{
			m_State |= State.Acquired;
		}

		public void OnSend()
		{
			Core.Set();

			lock (this)
			{
				if ((m_State & (State.Acquired | State.Static)) == 0)
				{
					Free();
				}
			}
		}

		private void Free()
		{
			if (m_CompiledBuffer == null)
			{
				return;
			}

			if ((m_State & State.Buffered) != 0)
			{
				m_Buffers.ReleaseBuffer(m_CompiledBuffer);
			}

			m_State &= ~(State.Static | State.Acquired | State.Buffered);

			m_CompiledBuffer = null;
		}

		public void Release()
		{
			if ((m_State & State.Acquired) != 0)
			{
				Free();
			}
		}

		private byte[] m_CompiledBuffer;
		private int m_CompiledLength;

		public byte[] Compile(bool compress, out int length)
		{
			lock (this)
			{
				if (m_CompiledBuffer == null)
				{
					if ((m_State & State.Accessed) == 0)
					{
						m_State |= State.Accessed;
					}
					else
					{
						if ((m_State & State.Warned) == 0)
						{
							m_State |= State.Warned;

							try
							{
								using (StreamWriter op = new StreamWriter("net_opt.log", true))
								{
									op.WriteLine("Redundant compile for packet {0}, use Acquire() and Release()", GetType());
									op.WriteLine(new StackTrace());
								}
							}
							catch
							{ }
						}

						m_CompiledBuffer = new byte[0];
						m_CompiledLength = 0;

						length = m_CompiledLength;
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
				long streamLen = m_Stream.Length;

				m_Stream.Seek(1, SeekOrigin.Begin);
				m_Stream.Write((ushort)streamLen);
			}
			else if (m_Stream.Length != m_Length)
			{
				int diff = (int)m_Stream.Length - m_Length;

				Console.WriteLine("Packet: 0x{0:X2}: Bad packet length! ({1}{2} bytes)", m_PacketID, diff >= 0 ? "+" : "", diff);
			}

			MemoryStream ms = m_Stream.UnderlyingStream;

			m_CompiledBuffer = ms.GetBuffer();
			int length = (int)ms.Length;

			if (compress)
			{
				byte[] buffer;
				lock (m_CompressorBuffers)
					buffer = m_CompressorBuffers.AcquireBuffer();

				Compression.Compress(m_CompiledBuffer, 0, length, buffer, ref length);

				if (length <= 0)
				{
					Console.WriteLine(
						"Warning: Compression buffer overflowed on packet 0x{0:X2} ('{1}') (length={2})",
						m_PacketID,
						GetType().Name,
						length);
					using (StreamWriter op = new StreamWriter("compression_overflow.log", true))
					{
						op.WriteLine(
							"{0} Warning: Compression buffer overflowed on packet 0x{1:X2} ('{2}') (length={3})",
							DateTime.UtcNow,
							m_PacketID,
							GetType().Name,
							length);
						op.WriteLine(new StackTrace());
					}
				}
				else
				{
					m_CompiledLength = length;

					if (length > BufferSize || (m_State & State.Static) != 0)
					{
						m_CompiledBuffer = new byte[length];
					}
					else
					{
						lock (m_Buffers)
							m_CompiledBuffer = m_Buffers.AcquireBuffer();
						m_State |= State.Buffered;
					}

					Buffer.BlockCopy(buffer, 0, m_CompiledBuffer, 0, length);

					lock (m_CompressorBuffers)
						m_CompressorBuffers.ReleaseBuffer(buffer);
				}
			}
			else if (length > 0)
			{
				var old = m_CompiledBuffer;
				m_CompiledLength = length;

				if (length > BufferSize || (m_State & State.Static) != 0)
				{
					m_CompiledBuffer = new byte[length];
				}
				else
				{
					lock (m_Buffers)
						m_CompiledBuffer = m_Buffers.AcquireBuffer();
					m_State |= State.Buffered;
				}

				Buffer.BlockCopy(old, 0, m_CompiledBuffer, 0, length);
			}

			PacketWriter.ReleaseInstance(m_Stream);
			m_Stream = null;
		}
	}
}