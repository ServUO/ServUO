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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
#endregion

namespace VitaNex.Items
{
	public class RuneCodex : Item, ISecurable
	{
		public sealed class UICache : IEquatable<UICache>, IEquatable<PlayerMobile>
		{
			public enum ViewMode
			{
				Categories,
				Entries
			}

			public PlayerMobile User { get; private set; }

			public RuneCodexCategory Category { get; set; }
			public Point2D CategoryScroll { get; set; }
			public Point2D CategoryPoint { get; set; }

			public RuneCodexEntry Entry { get; set; }
			public Point2D EntryScroll { get; set; }
			public Point2D EntryPoint { get; set; }

			public ViewMode Mode { get; set; }
			public bool EditMode { get; set; }

			public UICache(PlayerMobile user)
			{
				User = user;
			}

			public override int GetHashCode()
			{
				return User == null ? 0 : User.Serial.Value;
			}

			public override bool Equals(object obj)
			{
				if (obj is UICache)
				{
					return Equals((UICache)obj);
				}

				if (obj is PlayerMobile)
				{
					return Equals((PlayerMobile)obj);
				}

				return false;
			}

			public bool Equals(UICache other)
			{
				return other != null && Equals(other.User);
			}

			public bool Equals(PlayerMobile other)
			{
				return User == other;
			}
		}

		public static string DefaultDescription = "World Locations";

		private string _Descripton = DefaultDescription;
		private int _Charges;

		[CommandProperty(AccessLevel.GameMaster)]
		public string Descripton { get => _Descripton; set => SetDescription(value); }

		[CommandProperty(AccessLevel.GameMaster)]
		public RuneCodexCategoryGrid Categories { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public SecureLevel Level { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Charges
		{
			get => _Charges;
			set
			{
				_Charges = Math.Max(0, value);
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int RecallChargeCost { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int GateChargeCost { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int CloneEntryChargeCost { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool EditLocked { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AddLocked { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool RemoveLocked { get; set; }

		public Dictionary<Type, int> ChargeTypes { get; private set; }

		public List<UICache> Users { get; private set; }

		[Constructable]
		public RuneCodex()
			: this(1000)
		{ }

		[Constructable]
		public RuneCodex(int charges)
			: base(0x22C5)
		{
			Users = new List<UICache>();

			ChargeTypes = new Dictionary<Type, int>
			{
				{typeof(Gold), 100}
			};

			Categories = new RuneCodexCategoryGrid();
			Charges = Math.Max(0, charges);

			CloneEntryChargeCost = 5;
			RecallChargeCost = 1;
			GateChargeCost = 2;

			Name = "Rune Codex";
			Weight = 1.0;
			Hue = 74;

			LootType = LootType.Blessed;
		}

		public RuneCodex(Serial serial)
			: base(serial)
		{
			Users = new List<UICache>();
		}

		public virtual bool CanAddCategories(Mobile m)
		{
			return m.AccessLevel > AccessLevel.Counselor || BlessedFor == m || (!AddLocked && IsAccessibleTo(m));
		}

		public virtual bool CanEditCategories(Mobile m)
		{
			return m.AccessLevel > AccessLevel.Counselor || BlessedFor == m || (!EditLocked && IsAccessibleTo(m));
		}

		public virtual bool CanRemoveCategories(Mobile m)
		{
			return m.AccessLevel > AccessLevel.Counselor || BlessedFor == m || (!RemoveLocked && IsAccessibleTo(m));
		}

		public virtual bool CanAddEntries(Mobile m)
		{
			return m.AccessLevel > AccessLevel.Counselor || BlessedFor == m || (!AddLocked && IsAccessibleTo(m));
		}

		public virtual bool CanEditEntries(Mobile m)
		{
			return m.AccessLevel > AccessLevel.Counselor || BlessedFor == m || (!EditLocked && IsAccessibleTo(m));
		}

		public virtual bool CanRemoveEntries(Mobile m)
		{
			return m.AccessLevel > AccessLevel.Counselor || BlessedFor == m || (!RemoveLocked && IsAccessibleTo(m));
		}

		public override bool IsAccessibleTo(Mobile check)
		{
			return check.AccessLevel > AccessLevel.Counselor || BlessedFor == check || base.IsAccessibleTo(check);
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			SetSecureLevelEntry.AddTo(from, this, list);
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, 3) || !(m is PlayerMobile))
			{
				return;
			}

			var pm = (PlayerMobile)m;

			var ui = Users.FirstOrDefault(uic => uic.User == pm) ?? new UICache(pm);

			Users.Update(ui);

			new RuneCodexGump(pm, this).Send();
		}

		public void SetDescription(string desc)
		{
			_Descripton = String.IsNullOrWhiteSpace(desc) ? DefaultDescription : desc;
			InvalidateProperties();
		}

		public virtual bool CanChargeWith(Mobile m, Item item, out int cost, bool message)
		{
			cost = 0;

			if (m == null || m.Deleted || item == null || item.Deleted || !item.IsAccessibleTo(m))
			{
				return false;
			}

			var t = item.GetType();

			if (ChargeTypes.ContainsKey(t))
			{
				cost = ChargeTypes[t];
				return true;
			}

			if (message)
			{
				m.SendMessage("That item is not accepted for charging this codex.");
			}

			return false;
		}

		public virtual bool AddCharges(Mobile m, Item item, bool message)
		{
			if (m == null || m.Deleted || item == null || item.Deleted || !item.IsAccessibleTo(m))
			{
				return false;
			}

			if (!CanChargeWith(m, item, out var cost, message))
			{
				return false;
			}

			if (item.Amount < cost)
			{
				if (message)
				{
					m.SendMessage("One charge costs {0:#,0} {1}.", cost, item.ResolveName(m));
				}

				return false;
			}

			var c = (int)Math.Min((long)(Charges + (int)Math.Floor(item.Amount / (double)cost)), Int32.MaxValue) - Charges;

			if (c <= 0)
			{
				return false;
			}

			var con = c * cost;

			item.Consume(con);

			Charges += c;

			if (message)
			{
				m.SendMessage(
					"You added {0:#,0} charge{1} to the codex and consumed {2:#,0} {3}.",
					c,
					c != 1 ? "s" : String.Empty,
					con,
					item.ResolveName());
			}

			InvalidateProperties();
			return true;
		}

		public virtual bool AddRunebook(Mobile m, Runebook book, RuneCodexCategory cat, bool message)
		{
			if (m == null || m.Deleted || book == null || book.Deleted || !book.IsAccessibleTo(m))
			{
				return false;
			}

			if (cat == null)
			{
				var pm = m as PlayerMobile;

				if (pm != null)
				{
					var ui = Users.FirstOrDefault(uic => uic.User == pm);

					if (ui != null)
					{
						cat = ui.Category ?? Categories[ui.CategoryPoint.X, ui.CategoryPoint.Y];
					}
					else
					{
						cat = Categories[0, 0];
					}
				}
				else
				{
					cat = Categories[0, 0];
				}
			}

			if (book.Entries == null || book.Entries.Count == 0)
			{
				if (message)
				{
					m.SendMessage("That rune book is empty.");
				}

				return false;
			}

			if (Categories.Count >= Categories.Capacity)
			{
				if (message)
				{
					m.SendMessage("This rune codex can't hold more categories.");
				}

				return false;
			}

			if (cat != null && cat.AddRunebook(m, book, message))
			{
				InvalidateProperties();
				return true;
			}

			return false;
		}

		public virtual bool AddRune(Mobile m, RecallRune rune, RuneCodexCategory cat, bool message)
		{
			if (m == null || m.Deleted || rune == null || rune.Deleted || !rune.IsAccessibleTo(m))
			{
				return false;
			}

			var pm = m as PlayerMobile;

			var loc = Point2D.Zero;

			if (cat == null)
			{
				if (pm != null)
				{
					var ui = Users.FirstOrDefault(uic => uic.User == pm);

					if (ui != null)
					{
						cat = ui.Category ?? Categories[ui.CategoryPoint.X, ui.CategoryPoint.Y];
						loc = ui.EntryPoint;
					}
					else
					{
						cat = Categories[0, 0];
					}
				}
				else
				{
					cat = Categories[0, 0];
				}
			}

			if (!rune.Marked || rune.Target == Point3D.Zero || rune.TargetMap == Map.Internal)
			{
				if (message)
				{
					m.SendMessage("That rune is blank.");
				}

				return false;
			}

			if (cat != null)
			{
				if (cat.Entries.Count >= cat.Entries.Capacity)
				{
					if (message)
					{
						m.SendMessage("The rune codex category \"{0}\" can't hold more runes.", cat.Description);
					}

					return false;
				}

				if (cat.SetRune(m, rune, loc, message))
				{
					InvalidateProperties();
					return true;
				}
			}

			return false;
		}

		public bool Add(Mobile m, Item item, RuneCodexCategory cat, bool message)
		{
			if (m == null || m.Deleted || item == null || item.Deleted || !item.IsAccessibleTo(m))
			{
				return false;
			}

			if (item is RecallRune)
			{
				return AddRune(m, (RecallRune)item, cat, message);
			}

			if (item is Runebook)
			{
				return AddRunebook(m, (Runebook)item, cat, message);
			}

			#region Master Runebook Support
			//Using Reflection for shards that don't have it installed.
			var t = item.GetType();

			if (Insensitive.Equals(t.Name, "MasterRunebook"))
			{
				var pi = t.GetProperty("Books");

				if (pi != null && pi.CanRead)
				{
					var obj = pi.GetValue(item, null);

					if (obj is ICollection)
					{
						var ex = new Queue<Runebook>(((ICollection)obj).OfType<Runebook>().Where(r => r.Entries.Count > 0));

						if (ex.Count == 0)
						{
							if (message)
							{
								m.SendMessage("That master rune book is empty.");
							}

							return false;
						}

						if (Categories.Count + ex.Count > Categories.Capacity)
						{
							if (message)
							{
								m.SendMessage("That master rune book won't fit in this rune codex.");
							}

							return false;
						}

						var extracted = 0;

						while (ex.Count > 0)
						{
							var b = ex.Dequeue();

							if (AddRunebook(m, b, cat, message))
							{
								++extracted;
							}
						}

						if (extracted > 0)
						{
							if (message)
							{
								m.SendMessage(
									"You extract {0:#,0} book{1} from the master rune book and add them to the codex.",
									extracted,
									extracted != 1 ? "s" : String.Empty);
							}

							return true;
						}

						if (message)
						{
							m.SendMessage("There was nothing in the master rune book to extract.");
						}
					}
				}

				return false;
			}
			#endregion Master Runebook Support

			if (AddCharges(m, item, message))
			{
				return item.Deleted;
			}

			if (message)
			{
				m.SendMessage("Drop a rune book or recall rune on the codex to add them.");
			}

			return false;
		}

		public bool Remove(RuneCodexCategory cat)
		{
			for (var x = 0; x < Categories.Width; x++)
			{
				for (var y = 0; y < Categories.Height; y++)
				{
					if (Categories[x, y] != cat)
					{
						continue;
					}

					Categories[x, y] = null;
					InvalidateProperties();
					return true;
				}
			}

			return false;
		}

		public bool Drop(Mobile m, RuneCodexCategory cat, bool message)
		{
			if (m == null || m.Deleted || cat == null)
			{
				return false;
			}

			if (cat.Entries == null || cat.Entries.Count == 0)
			{
				if (message)
				{
					m.SendMessage("The category \"{0}\" is empty.", cat.Name);
				}

				return false;
			}

			var cost = CloneEntryChargeCost * cat.Entries.Count;

			if (!ConsumeCharges(cost))
			{
				if (message)
				{
					m.SendMessage("This action requires {0:#,0} charge{1}.", cost, cost != 1 ? "s" : String.Empty);
				}

				return false;
			}

			var entries = new Queue<RuneCodexEntry>(cat.Entries.Not(e => e == null));
			var book = new Runebook();
			var count = 1;

			while (entries.Count > 0)
			{
				var entry = entries.Dequeue();

				if (entry == null)
				{
					continue;
				}

				book.Entries.Add(
					new RunebookEntry(
						entry.Location,
						entry.Location,
						entry.Name,
						BaseHouse.FindHouseAt(entry.Location, entry.Location, 16)));

				if (book.Entries.Count < 16 && entries.Count > 0)
				{
					continue;
				}

				m.AddToBackpack(book);

				if (entries.Count == 0)
				{
					continue;
				}

				book = new Runebook();
				++count;
			}

			if (message)
			{
				m.SendMessage(
					"You created {0:#,0} rune book{1} and consumed {2:#,0} charge{3} from the codex.",
					count,
					count != 1 ? "s" : String.Empty,
					cost,
					cost != 1 ? "s" : String.Empty);
			}

			return true;
		}

		public bool Drop(Mobile m, RuneCodexEntry entry, bool message)
		{
			if (m == null || m.Deleted || entry == null)
			{
				return false;
			}

			if (!ConsumeCharges(CloneEntryChargeCost))
			{
				m.SendMessage(
					"This action requires {0:#,0} charge{1}.",
					CloneEntryChargeCost,
					CloneEntryChargeCost != 1 ? "s" : String.Empty);
				return false;
			}

			m.AddToBackpack(
				new RecallRune
				{
					Marked = true,
					Target = entry.Location,
					TargetMap = entry.Location,
					Description = entry.Name,
					House = BaseHouse.FindHouseAt(entry.Location, entry.Location, 16)
				});

			if (message)
			{
				m.SendMessage(
					"You create a recall rune and consume {0:#,0} charge{1} from the codex.",
					CloneEntryChargeCost,
					CloneEntryChargeCost != 1 ? "s" : String.Empty);
			}

			return true;
		}

		public bool Recall(Mobile m, RuneCodexEntry entry, bool message)
		{
			if (m == null || m.Deleted || entry == null)
			{
				return false;
			}

			if (!ConsumeCharges(RecallChargeCost))
			{
				m.SendMessage(
					"This action requires {0:#,0} charge{1}.",
					RecallChargeCost,
					RecallChargeCost != 1 ? "s" : String.Empty);
				return false;
			}

			if (!entry.Recall(m))
			{
				Charges += RecallChargeCost;
				return false;
			}

			return true;
		}

		public bool Gate(Mobile m, RuneCodexEntry entry, bool message)
		{
			if (m == null || m.Deleted || entry == null)
			{
				return false;
			}

			if (!ConsumeCharges(GateChargeCost))
			{
				m.SendMessage("This action requires {0:#,0} charge{1}.", GateChargeCost, GateChargeCost != 1 ? "s" : String.Empty);
				return false;
			}

			if (!entry.Gate(m))
			{
				Charges += GateChargeCost;
				return false;
			}

			return true;
		}

		public override bool OnDragDrop(Mobile m, Item dropped)
		{
			return Add(m, dropped, null, true);
		}

		public override void OnAfterDuped(Item newItem)
		{
			base.OnAfterDuped(newItem);

			var c = newItem as RuneCodex;

			if (c == null)
			{
				return;
			}

			c.AddLocked = AddLocked;
			c.EditLocked = EditLocked;
			c.RemoveLocked = RemoveLocked;

			c.CloneEntryChargeCost = CloneEntryChargeCost;
			c.RecallChargeCost = RecallChargeCost;
			c.GateChargeCost = GateChargeCost;

			c.Charges = Charges;
			c.Descripton = Descripton;

			c.Categories = new RuneCodexCategoryGrid();
			c.ChargeTypes = new Dictionary<Type, int>(ChargeTypes);
			c.Users = new List<UICache>();

			RuneCodexCategory cata, catb;
			RuneCodexEntry entrya, entryb;

			for (var cx = 0; cx < Categories.Width; cx++)
			{
				for (var cy = 0; cy < Categories.Height; cy++)
				{
					cata = Categories[cx, cy];

					if (cata == null)
					{
						continue;
					}

					catb = new RuneCodexCategory(cata.Name, cata.Description, cata.Hue);

					c.Categories.SetContent(cx, cy, catb);

					for (var ex = 0; ex < cata.Entries.Width; ex++)
					{
						for (var ey = 0; ey < cata.Entries.Width; ey++)
						{
							entrya = cata.Entries[ex, ey];

							if (entrya == null)
							{
								continue;
							}

							entryb = new RuneCodexEntry(entrya.Name, entrya.Description, entrya.Location);

							catb.Entries.SetContent(ex, ey, entryb);
						}
					}
				}
			}
		}

		public virtual bool ConsumeCharges(int amount)
		{
			amount = Math.Max(0, amount);

			if (Charges < amount)
			{
				return false;
			}

			Charges -= amount;
			return true;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			// charges: ~1_val~
			list.Add(1060741, Charges.ToString("#,0"));

			var eTotal = 0;
			var eCap = 0;

			foreach (var c in Categories.Not(c => c == null))
			{
				eTotal += c.Entries.Count;
				eCap += c.Entries.Capacity;
			}

			list.Add("Categories: {0} / {1}\nEntries: {2} / {3}", Categories.Count, Categories.Capacity, eTotal, eCap);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(3);

			switch (version)
			{
				case 3:
					writer.WriteFlag(Level);
					goto case 2;
				case 2:
					writer.Write(RemoveLocked);
					goto case 1;
				case 1:
				{
					writer.Write(EditLocked);
					writer.Write(AddLocked);
					writer.Write(CloneEntryChargeCost);

					writer.WriteDictionary(
						ChargeTypes,
						(k, v) =>
						{
							writer.WriteType(k);
							writer.Write(v);
						});
				}
				goto case 0;
				case 0:
				{
					writer.Write(Charges);
					writer.Write(RecallChargeCost);
					writer.Write(GateChargeCost);
					writer.Write(_Descripton);
					Categories.Serialize(writer);
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
				case 3:
					Level = reader.ReadFlag<SecureLevel>();
					goto case 2;
				case 2:
					RemoveLocked = reader.ReadBool();
					goto case 1;
				case 1:
				{
					EditLocked = reader.ReadBool();
					AddLocked = reader.ReadBool();
					CloneEntryChargeCost = reader.ReadInt();

					ChargeTypes = reader.ReadDictionary(
						() =>
						{
							var k = reader.ReadType();
							var v = reader.ReadInt();
							return new KeyValuePair<Type, int>(k, v);
						});
				}
				goto case 0;
				case 0:
				{
					Charges = reader.ReadInt();
					RecallChargeCost = reader.ReadInt();
					GateChargeCost = reader.ReadInt();
					_Descripton = reader.ReadString();
					Categories = new RuneCodexCategoryGrid(reader);
				}
				break;
			}

			if (version > 0)
			{
				return;
			}

			Charges = 1000;
			CloneEntryChargeCost = 5;
			RecallChargeCost = 1;
			GateChargeCost = 2;
			ChargeTypes = new Dictionary<Type, int>
			{
				{typeof(Gold), 100}
			};
		}
	}
}